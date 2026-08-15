using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Package-local Looking Glass authority for registered UI surfaces.
    /// Project code owns whether this object survives scene travel.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EchoUIRoot : MonoBehaviour
    {
        private static EchoUIRoot active;

        [SerializeField]
        private bool startAutomatically = true;

        private readonly Dictionary<string, UISurface> surfaces =
            new Dictionary<string, UISurface>(
                StringComparer.Ordinal);

        private readonly Dictionary<string, string> currentScreenByScope =
            new Dictionary<string, string>(
                StringComparer.Ordinal);

        private readonly Dictionary<string, Stack<string>> historyByScope =
            new Dictionary<string, Stack<string>>(
                StringComparer.Ordinal);

        private readonly UIContextState contextState =
            new UIContextState();

        private readonly UISelectionCoordinator selectionCoordinator =
            new UISelectionCoordinator();

        private readonly Dictionary<UISurface, SurfaceResponseApplicationState>
            responseApplicationStateBySurface =
                new Dictionary<UISurface, SurfaceResponseApplicationState>();

        private UIInputModality inputModality =
            UIInputModality.Pointer;

        [Header("M2 Screen Lifecycle")]
        [SerializeField]
        private List<UILayerHost> screenLayerHosts =
            new List<UILayerHost>();

        [SerializeField]
        private List<UIScreenDefinition> screenDefinitions =
            new List<UIScreenDefinition>();

        [SerializeField, Min(1)]
        private int screenOperationCapacity = 32;

        private readonly Dictionary<string, UISurface> externalScreenViews =
            new Dictionary<string, UISurface>(
                StringComparer.Ordinal);

        private UILayerRegistry screenLayerRegistry;
        private UIScreenNavigator screenNavigator;
        private UIScreenOperationQueue screenOperationQueue;
        private bool processingScreenOperations;
        private long nextScreenOperationSequence;

        [Header("M2 Blocking Modal Lifecycle")]
        [SerializeField]
        private List<UIModalDefinition> modalDefinitions =
            new List<UIModalDefinition>();

        [SerializeField, Min(1)]
        private int modalCapacity = 8;

        [SerializeField]
        private UIModalScreenMutationPolicy modalScreenMutationPolicy =
            UIModalScreenMutationPolicy.Reject;

        [SerializeField, Min(1)]
        private int deferredScreenOperationCapacity = 16;

        private readonly Dictionary<string, UISurface> externalModalViews =
            new Dictionary<string, UISurface>(
                StringComparer.Ordinal);

        private UIModalService modalService;
        private UIScreenOperationQueue deferredScreenOperationQueue;
        private bool shuttingDown;

        public static EchoUIRoot Active =>
            active;

        public bool IsAuthoritative { get; private set; }

        public bool IsInitialized { get; private set; }

        public int RegisteredSurfaceCount =>
            surfaces.Count;

        public UIInputModality InputModality =>
            inputModality;

        public bool IsScreenLifecycleInitialized =>
            screenNavigator != null &&
            screenNavigator.IsValid &&
            screenOperationQueue != null;

        public int ScreenOperationQueueDepth =>
            screenOperationQueue == null
                ? 0
                : screenOperationQueue.Count;

        public int ScreenLayerCount =>
            screenLayerRegistry == null
                ? 0
                : screenLayerRegistry.Count;

        public bool IsModalLifecycleInitialized =>
            modalService != null &&
            modalService.IsValid;

        public bool HasBlockingModal =>
            IsModalLifecycleInitialized &&
            modalService.HasActiveModals;

        public int ActiveModalCount =>
            IsModalLifecycleInitialized
                ? modalService.ActiveCount
                : 0;

        public int DeferredScreenOperationQueueDepth =>
            deferredScreenOperationQueue == null
                ? 0
                : deferredScreenOperationQueue.Count;

        public string TopModalId =>
            IsModalLifecycleInitialized &&
            modalService.TopEntry != null
                ? modalService.TopEntry.ModalId.Value
                : string.Empty;

        private void Awake()
        {
            TryClaimAuthority();
        }

        private void Start()
        {
            if (startAutomatically &&
                IsAuthoritative &&
                !IsInitialized)
            {
                Initialize();
            }
        }

        private void LateUpdate()
        {
            RefreshModalLifecycle();
        }

        private void OnDestroy()
        {
            shuttingDown = true;

            if (active == this)
            {
                active = null;
            }

            IsAuthoritative = false;
            IsInitialized = false;

            if (modalService != null)
            {
                modalService.Shutdown(
                    UIModalAbortReason.RootShutdown);
            }

            if (deferredScreenOperationQueue != null)
            {
                deferredScreenOperationQueue.ClearRejected(
                    "Looking Glass root was destroyed.");
            }

            if (screenOperationQueue != null)
            {
                screenOperationQueue.ClearRejected(
                    "Looking Glass root was destroyed.");
            }

            if (screenNavigator != null)
            {
                screenNavigator.Shutdown();
            }

            modalService = null;
            deferredScreenOperationQueue = null;
            externalModalViews.Clear();

            screenOperationQueue = null;
            screenNavigator = null;
            screenLayerRegistry = null;
            externalScreenViews.Clear();
            processingScreenOperations = false;
            nextScreenOperationSequence = 0;

            surfaces.Clear();
            currentScreenByScope.Clear();
            historyByScope.Clear();
            responseApplicationStateBySurface.Clear();
            contextState.Clear();
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            active = null;
        }

        public UISurfaceOperationResult Initialize()
        {
            if (!IsAuthoritative)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotAuthoritative,
                    message: "Only the authoritative Looking Glass root may initialize.");
            }
            if (IsInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.AlreadyInitialized,
                    message: "Looking Glass is already initialized.");
            }

            UISurface[] discovered =
                GetComponentsInChildren<UISurface>(true);

            Dictionary<string, UISurface> pendingSurfaces =
                new Dictionary<string, UISurface>(
                    StringComparer.Ordinal);
            Dictionary<string, string> pendingCurrentScreens =
                new Dictionary<string, string>(
                    StringComparer.Ordinal);

            for (int index = 0;
                 index < discovered.Length;
                 index++)
            {
                UISurface surface =
                    discovered[index];

                string id =
                    surface.SurfaceId;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.InvalidDefinition,
                        message: "A registered UI surface has an empty stable ID.");
                }
                if (pendingSurfaces.ContainsKey(id))
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.DuplicateSurfaceId,
                        surfaceId: id,
                        message: "Two UI surfaces use the same stable ID.");
                }
                if (surface.Role == UISurfaceRole.Screen &&
                    string.IsNullOrWhiteSpace(
                        surface.NavigationScopeId))
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.InvalidDefinition,
                        surfaceId: id,
                        message: "A Screen surface requires a navigation scope ID.");
                }

                pendingSurfaces.Add(
                    id,
                    surface);

                if (surface.Role != UISurfaceRole.Screen ||
                    !surface.StartVisible)
                {
                    continue;
                }

                string scopeId =
                    surface.NavigationScopeId;
                if (pendingCurrentScreens.ContainsKey(scopeId))
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.InitialScopeConflict,
                        surfaceId: id,
                        scopeId: scopeId,
                        message: "More than one Screen starts visible in the same navigation scope.");
                }

                pendingCurrentScreens.Add(
                    scopeId,
                    id);
            }

            surfaces.Clear();
            currentScreenByScope.Clear();
            historyByScope.Clear();
            responseApplicationStateBySurface.Clear();

            foreach (KeyValuePair<string, UISurface> pair in pendingSurfaces)
            {
                surfaces.Add(
                    pair.Key,
                    pair.Value);
            }
            foreach (KeyValuePair<string, string> pair in pendingCurrentScreens)
            {
                currentScreenByScope.Add(
                    pair.Key,
                    pair.Value);
            }

            for (int index = 0;
                 index < discovered.Length;
                 index++)
            {
                discovered[index].SetVisible(
                    discovered[index].StartVisible);
            }

            IsInitialized = true;

            for (int index = 0;
                 index < discovered.Length;
                 index++)
            {
                ApplyCurrentContext(
                    discovered[index]);
            }

            return UISurfaceOperationResult.Success(
                message: "Looking Glass surface registry initialized.");
        }

        public UISurfaceOperationResult NavigateTo(
            string surfaceId)
        {
            if (IsScreenLifecycleInitialized &&
                screenNavigator.HasDefinition(
                    surfaceId))
            {
                if (HasBlockingModal)
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.BlockedByModal,
                        surfaceId: surfaceId,
                        message:
                            "Synchronous NavigateTo does not queue deferred Screen mutation behind a blocking Modal. Use the Screen handle API for explicit defer behavior.");
                }

                return ToSurfaceOperationResult(
                    PushScreen(
                        surfaceId));
            }

            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface target);

            if (!validation.Succeeded)
            {
                return validation;
            }
            if (target.Role != UISurfaceRole.Screen)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.WrongSurfaceRole,
                    surfaceId: target.SurfaceId,
                    message: "NavigateTo requires a Screen surface.");
            }

            string scopeId =
                target.NavigationScopeId;
            if (currentScreenByScope.TryGetValue(
                    scopeId,
                    out string currentId))
            {
                if (string.Equals(
                        currentId,
                        target.SurfaceId,
                        StringComparison.Ordinal))
                {
                    ActivateSurface(
                        target);
                    return UISurfaceOperationResult.Success(
                        target.SurfaceId,
                        scopeId,
                        "Requested screen is already current.");
                }

                if (!historyByScope.TryGetValue(
                        scopeId,
                        out Stack<string> history))
                {
                    history =
                        new Stack<string>();
                    historyByScope.Add(
                        scopeId,
                        history);
                }

                history.Push(currentId);

                if (surfaces.TryGetValue(
                        currentId,
                        out UISurface currentSurface))
                {
                    DeactivateSurface(
                        currentSurface);
                }
            }

            currentScreenByScope[scopeId] =
                target.SurfaceId;
            ActivateSurface(
                target);

            return UISurfaceOperationResult.Success(
                target.SurfaceId,
                scopeId,
                "Screen navigation completed.");
        }

        public UISurfaceOperationResult Back(
            string scopeId)
        {
            if (HasBlockingModal)
            {
                UIModalCompletionAttemptResult modalBack =
                    modalService.HandleBack();

                if (modalBack.Status ==
                        UIModalCompletionStatus.Succeeded)
                {
                    return UISurfaceOperationResult.Success(
                        scopeId: scopeId,
                        message:
                            "Back was handled by the top blocking Modal.");
                }

                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.BlockedByModal,
                    scopeId: scopeId,
                    message: modalBack.Message);
            }

            if (IsScreenLifecycleInitialized &&
                screenNavigator.HasScope(
                    scopeId))
            {
                return ToSurfaceOperationResult(
                    BackScreen(
                        scopeId));
            }

            if (!IsInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    scopeId: scopeId,
                    message: "Looking Glass must be initialized before Back.");
            }

            string normalizedScope =
                scopeId == null
                    ? string.Empty
                    : scopeId.Trim();
            if (string.IsNullOrWhiteSpace(normalizedScope) ||
                !historyByScope.TryGetValue(
                    normalizedScope,
                    out Stack<string> history))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NoHistory,
                    scopeId: normalizedScope,
                    message: "No navigation history is available for this scope.");
            }

            while (history.Count > 0)
            {
                string previousId =
                    history.Pop();
                if (!surfaces.TryGetValue(
                        previousId,
                        out UISurface previous) ||
                    previous.Role != UISurfaceRole.Screen ||
                    !string.Equals(
                        previous.NavigationScopeId,
                        normalizedScope,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (currentScreenByScope.TryGetValue(
                        normalizedScope,
                        out string currentId) &&
                    surfaces.TryGetValue(
                        currentId,
                        out UISurface current))
                {
                    DeactivateSurface(
                        current);
                }

                currentScreenByScope[normalizedScope] =
                    previous.SurfaceId;
                ActivateSurface(
                    previous);

                return UISurfaceOperationResult.Success(
                    previous.SurfaceId,
                    normalizedScope,
                    "Back navigation completed.");
            }

            return new UISurfaceOperationResult(
                UISurfaceOperationStatus.NoHistory,
                scopeId: normalizedScope,
                message: "No valid navigation history remains for this scope.");
        }

        public UISurfaceOperationResult OpenSurface(
            string surfaceId)
        {
            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface surface);

            if (!validation.Succeeded)
            {
                return validation;
            }

            if (surface.Role == UISurfaceRole.Screen)
            {
                return NavigateTo(surfaceId);
            }

            if (surface.Role == UISurfaceRole.Modal)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.WrongSurfaceRole,
                    surfaceId: surface.SurfaceId,
                    message:
                        "Blocking Modal surfaces must be opened through OpenModal.");
            }

            ActivateSurface(
                surface);

            return UISurfaceOperationResult.Success(
                surface.SurfaceId,
                message: "Independent surface opened.");
        }

        public UISurfaceOperationResult CloseSurface(
            string surfaceId)
        {
            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface surface);
            if (!validation.Succeeded)
            {
                return validation;
            }

            if (surface.Role == UISurfaceRole.Modal)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.WrongSurfaceRole,
                    surfaceId: surface.SurfaceId,
                    message:
                        "Blocking Modal surfaces must settle through their modal handle.");
            }

            if (surface.Role == UISurfaceRole.Screen &&
                IsScreenLifecycleInitialized &&
                screenNavigator.HasDefinition(
                    surface.SurfaceId))
            {
                if (HasBlockingModal)
                {
                    return new UISurfaceOperationResult(
                        UISurfaceOperationStatus.BlockedByModal,
                        surfaceId: surface.SurfaceId,
                        scopeId: surface.NavigationScopeId,
                        message:
                            "Synchronous CloseSurface does not queue deferred Screen mutation behind a blocking Modal. Use the Screen handle API for explicit defer behavior.");
                }

                return ToSurfaceOperationResult(
                    CloseScreen(
                        surface.SurfaceId,
                        surface.NavigationScopeId));
            }

            DeactivateSurface(
                surface);
            if (surface.Role == UISurfaceRole.Screen &&
                currentScreenByScope.TryGetValue(
                    surface.NavigationScopeId,
                    out string currentId) &&
                string.Equals(
                    currentId,
                    surface.SurfaceId,
                    StringComparison.Ordinal))
            {
                currentScreenByScope.Remove(
                    surface.NavigationScopeId);
            }

            return UISurfaceOperationResult.Success(
                surface.SurfaceId,
                surface.NavigationScopeId,
                "Surface closed.");
        }

        public UISurfaceOperationResult ToggleSurface(
            string surfaceId)
        {
            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface surface);
            if (!validation.Succeeded)
            {
                return validation;
            }

            if (surface.Role == UISurfaceRole.Modal)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.WrongSurfaceRole,
                    surfaceId: surface.SurfaceId,
                    message:
                        "Blocking Modal surfaces do not use generic ToggleSurface.");
            }

            if (surface.Role == UISurfaceRole.Screen)
            {
                if (IsScreenLifecycleInitialized &&
                    screenNavigator.HasDefinition(
                        surface.SurfaceId))
                {
                    if (HasBlockingModal)
                    {
                        return new UISurfaceOperationResult(
                            UISurfaceOperationStatus.BlockedByModal,
                            surfaceId: surface.SurfaceId,
                            scopeId: surface.NavigationScopeId,
                            message:
                                "Synchronous ToggleSurface does not queue deferred Screen mutation behind a blocking Modal. Use the Screen handle API for explicit defer behavior.");
                    }

                    string current =
                        GetCurrentScreenId(
                            surface.NavigationScopeId);

                    return string.Equals(
                            current,
                            surface.SurfaceId,
                            StringComparison.Ordinal)
                        ? ToSurfaceOperationResult(
                            CloseScreen(
                                surface.SurfaceId,
                                surface.NavigationScopeId))
                        : ToSurfaceOperationResult(
                            PushScreen(
                                surface.SurfaceId));
                }

                return surface.IsVisible
                    ? CloseSurface(surfaceId)
                    : NavigateTo(surfaceId);
            }

            if (surface.IsVisible)
            {
                DeactivateSurface(
                    surface);
            }
            else
            {
                ActivateSurface(
                    surface);
            }

            return UISurfaceOperationResult.Success(
                surface.SurfaceId,
                message: "Independent surface toggled.");
        }

        public string GetCurrentScreenId(
            string scopeId)
        {
            if (!IsInitialized ||
                string.IsNullOrWhiteSpace(scopeId))
            {
                return string.Empty;
            }

            string normalized =
                scopeId.Trim();

            if (IsScreenLifecycleInitialized &&
                screenNavigator.HasScope(
                    normalized))
            {
                return screenNavigator.GetCurrentScreenId(
                    normalized);
            }

            return currentScreenByScope.TryGetValue(
                    normalized,
                    out string surfaceId)
                ? surfaceId
                : string.Empty;
        }

        public int GetScreenHistoryDepth(
            string scopeId)
        {
            if (!IsScreenLifecycleInitialized)
            {
                return 0;
            }

            return screenNavigator.GetHistoryDepth(
                scopeId);
        }

        public bool IsSurfaceVisible(
            string surfaceId)
        {
            return IsInitialized &&
                surfaceId != null &&
                surfaces.TryGetValue(
                    surfaceId.Trim(),
                    out UISurface surface) &&
                surface.IsVisible;
        }

        public bool IsContextActive(
            string contextId) =>
            contextState.IsActive(
                contextId);

        public UISurfaceOperationResult SetContextActive(
            string contextId,
            bool isActive)
        {
            if (!IsAuthoritative)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotAuthoritative,
                    message: "Only the authoritative Looking Glass root may receive UI context truth.");
            }

            UIContextId normalized =
                new UIContextId(contextId);
            if (!normalized.IsValid)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message: "UI context IDs must be nonempty stable project-authored values.");
            }

            contextState.SetActive(
                normalized,
                isActive);

            if (IsInitialized)
            {
                foreach (UISurface surface in surfaces.Values)
                {
                    ApplyCurrentContext(
                        surface);
                }
            }

            return UISurfaceOperationResult.Success(
                message: isActive
                    ? "UI context activated."
                    : "UI context deactivated.");
        }

        public void SetInputModality(
            UIInputModality modality)
        {
            inputModality = modality;
        }

        public UISurfaceOperationResult SetSurfaceRuntimeOverride(
            string surfaceId,
            UISurfaceRuntimeOverride runtimeOverride)
        {
            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface surface);
            if (!validation.Succeeded)
            {
                return validation;
            }

            surface.SetRuntimeOverride(
                runtimeOverride);
            ApplyCurrentContext(
                surface);

            return UISurfaceOperationResult.Success(
                surface.SurfaceId,
                surface.NavigationScopeId,
                "Transient surface runtime override applied.");
        }

        public UISurfaceOperationResult ClearSurfaceRuntimeOverride(
            string surfaceId)
        {
            UISurfaceOperationResult validation =
                ResolveSurface(
                    surfaceId,
                    out UISurface surface);
            if (!validation.Succeeded)
            {
                return validation;
            }

            surface.ClearRuntimeOverride();
            ApplyCurrentContext(
                surface);

            return UISurfaceOperationResult.Success(
                surface.SurfaceId,
                surface.NavigationScopeId,
                "Transient surface runtime override cleared.");
        }

        public UISurfaceOperationResult InitializeScreenLifecycle()
        {
            return InitializeScreenLifecycle(
                screenLayerHosts,
                screenDefinitions,
                null,
                screenOperationCapacity);
        }

        public UISurfaceOperationResult InitializeScreenLifecycle(
            IEnumerable<UILayerHost> layerHosts,
            IEnumerable<UIScreenDefinition> definitions,
            IUIScreenFactory factory = null,
            int queueCapacity = 32)
        {
            if (!IsAuthoritative)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotAuthoritative,
                    message: "Only the authoritative Looking Glass root may initialize Screen lifecycle.");
            }

            if (!IsInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    message: "Looking Glass surface foundation must initialize before Screen lifecycle.");
            }

            if (IsScreenLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.AlreadyInitialized,
                    message: "Looking Glass Screen lifecycle is already initialized.");
            }

            List<UILayerHost> layerSnapshot =
                layerHosts == null
                    ? null
                    : new List<UILayerHost>(
                        layerHosts);

            List<UIScreenDefinition> definitionSnapshot =
                definitions == null
                    ? null
                    : new List<UIScreenDefinition>(
                        definitions);

            if (!UILayerRegistry.TryCreate(
                    layerSnapshot,
                    out UILayerRegistry registry,
                    out string layerError))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message: layerError);
            }

            if (definitionSnapshot != null)
            {
                HashSet<string> definedScreenIds =
                    new HashSet<string>(
                        StringComparer.Ordinal);

                HashSet<string> definedScopes =
                    new HashSet<string>(
                        StringComparer.Ordinal);

                for (int index = 0;
                     index < definitionSnapshot.Count;
                     index++)
                {
                    UIScreenDefinition definition =
                        definitionSnapshot[index];

                    if (definition == null)
                    {
                        continue;
                    }

                    definedScreenIds.Add(
                        definition.ScreenId);

                    definedScopes.Add(
                        definition.NavigationScopeId);

                    if (definition.OwnershipMode !=
                            UIScreenOwnershipMode.SceneOwned)
                    {
                        continue;
                    }

                    UISurface sceneView =
                        definition.SceneOwnedView;

                    if (sceneView == null ||
                        !surfaces.TryGetValue(
                            sceneView.SurfaceId,
                            out UISurface registeredSceneView) ||
                        registeredSceneView != sceneView)
                    {
                        return new UISurfaceOperationResult(
                            UISurfaceOperationStatus.InvalidDefinition,
                            surfaceId: definition.ScreenId,
                            scopeId: definition.NavigationScopeId,
                            message:
                                "SceneOwned Screen views must already belong to the authoritative Looking Glass surface registry.");
                    }
                }

                foreach (UISurface registered in surfaces.Values)
                {
                    if (registered == null ||
                        registered.Role != UISurfaceRole.Screen ||
                        !definedScopes.Contains(
                            registered.NavigationScopeId))
                    {
                        continue;
                    }

                    if (!definedScreenIds.Contains(
                            registered.SurfaceId))
                    {
                        return new UISurfaceOperationResult(
                            UISurfaceOperationStatus.InvalidDefinition,
                            surfaceId: registered.SurfaceId,
                            scopeId: registered.NavigationScopeId,
                            message:
                                "Every registered Screen in an M2-authoritative navigation scope requires an explicit Screen definition.");
                    }
                }
            }

            UIScreenNavigator navigator =
                new UIScreenNavigator(
                    definitionSnapshot,
                    registry,
                    factory,
                    ResolveExternalScreenView,
                    RegisterRuntimeScreenSurface,
                    UnregisterRuntimeScreenSurface,
                    ActivateScreenEntry,
                    SuspendScreenEntry,
                    ResumeScreenEntry,
                    CloseScreenEntry,
                    out string definitionError);

            if (!string.IsNullOrWhiteSpace(
                    definitionError) ||
                !navigator.IsValid)
            {
                navigator.Shutdown();

                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message: string.IsNullOrWhiteSpace(
                        definitionError)
                        ? "Looking Glass Screen lifecycle definition validation failed."
                        : definitionError);
            }

            screenLayerRegistry =
                registry;
            screenNavigator =
                navigator;
            screenOperationQueue =
                new UIScreenOperationQueue(
                    queueCapacity < 1
                        ? 1
                        : queueCapacity);

            nextScreenOperationSequence = 0;
            processingScreenOperations = false;

            if (definitionSnapshot != null)
            {
                for (int index = 0;
                     index < definitionSnapshot.Count;
                     index++)
                {
                    UIScreenDefinition definition =
                        definitionSnapshot[index];

                    if (definition == null ||
                        definition.OwnershipMode !=
                            UIScreenOwnershipMode.SceneOwned ||
                        definition.SceneOwnedView == null ||
                        !definition.SceneOwnedView.IsVisible)
                    {
                        continue;
                    }

                    UIScreenOperationRequest request =
                        UIScreenOperationRequest.Reset(
                            definition.ScreenId)
                        .WithSequence(
                            ++nextScreenOperationSequence);

                    UIScreenOperationResult seed =
                        screenNavigator.Execute(
                            request);

                    if (seed.Status !=
                            UIScreenOperationStatus.Succeeded &&
                        seed.Status !=
                            UIScreenOperationStatus.NoChange)
                    {
                        screenNavigator.Shutdown();
                        screenNavigator = null;
                        screenOperationQueue = null;
                        screenLayerRegistry = null;

                        return new UISurfaceOperationResult(
                            UISurfaceOperationStatus.InvalidDefinition,
                            surfaceId: definition.ScreenId,
                            scopeId: definition.NavigationScopeId,
                            message:
                                "Failed to establish initial Screen lifecycle state: " +
                                seed.Message);
                    }
                }
            }

            return UISurfaceOperationResult.Success(
                message: "Looking Glass Screen lifecycle initialized.");
        }

        public UISurfaceOperationResult InitializeModalLifecycle()
        {
            return InitializeModalLifecycle(
                modalDefinitions,
                null,
                modalCapacity,
                modalScreenMutationPolicy,
                deferredScreenOperationCapacity);
        }

        public UISurfaceOperationResult InitializeModalLifecycle(
            IEnumerable<UIModalDefinition> definitions,
            IUIModalFactory factory = null,
            int activeCapacity = 8,
            UIModalScreenMutationPolicy screenMutationPolicy =
                UIModalScreenMutationPolicy.Reject,
            int deferredCapacity = 16)
        {
            if (!IsAuthoritative)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotAuthoritative,
                    message:
                        "Only the authoritative Looking Glass root may initialize blocking Modal lifecycle.");
            }

            if (!IsInitialized ||
                !IsScreenLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    message:
                        "Looking Glass surface and Screen lifecycle must initialize before blocking Modal lifecycle.");
            }

            if (IsModalLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.AlreadyInitialized,
                    message:
                        "Looking Glass blocking Modal lifecycle is already initialized.");
            }

            if (screenMutationPolicy !=
                    UIModalScreenMutationPolicy.Reject &&
                screenMutationPolicy !=
                    UIModalScreenMutationPolicy.DeferUntilModalStackClears)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message:
                        "Blocking Modal Screen-mutation policy is unsupported.");
            }

            List<UIModalDefinition> snapshot =
                definitions == null
                    ? null
                    : new List<UIModalDefinition>(
                        definitions);

            if (snapshot != null)
            {
                for (int index = 0;
                     index < snapshot.Count;
                     index++)
                {
                    UIModalDefinition definition =
                        snapshot[index];

                    if (definition == null ||
                        definition.OwnershipMode !=
                            UIScreenOwnershipMode.SceneOwned)
                    {
                        continue;
                    }

                    UISurface sceneView =
                        definition.SceneOwnedView;

                    if (sceneView == null ||
                        !surfaces.TryGetValue(
                            sceneView.SurfaceId,
                            out UISurface registered) ||
                        registered != sceneView ||
                        sceneView.Role != UISurfaceRole.Modal)
                    {
                        return new UISurfaceOperationResult(
                            UISurfaceOperationStatus.InvalidDefinition,
                            surfaceId:
                                definition.ModalId.Value,
                            message:
                                "SceneOwned Modal views must already belong to the authoritative Looking Glass surface registry with Modal role.");
                    }
                }
            }

            UIModalService service =
                new UIModalService(
                    snapshot,
                    screenLayerRegistry,
                    factory,
                    ResolveExternalModalView,
                    RegisterRuntimeModalSurface,
                    UnregisterRuntimeModalSurface,
                    ActivateSurface,
                    DeactivateSurface,
                    OnModalStackChanged,
                    activeCapacity,
                    out string definitionError);

            if (!string.IsNullOrWhiteSpace(
                    definitionError) ||
                !service.IsValid)
            {
                service.Shutdown(
                    UIModalAbortReason.RootShutdown);

                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message:
                        string.IsNullOrWhiteSpace(
                            definitionError)
                            ? "Looking Glass blocking Modal definition validation failed."
                            : definitionError);
            }

            modalService =
                service;

            modalScreenMutationPolicy =
                screenMutationPolicy;

            deferredScreenOperationQueue =
                new UIScreenOperationQueue(
                    deferredCapacity < 1
                        ? 1
                        : deferredCapacity);

            if (snapshot != null)
            {
                for (int index = 0;
                     index < snapshot.Count;
                     index++)
                {
                    UIModalDefinition definition =
                        snapshot[index];

                    if (definition != null &&
                        definition.OwnershipMode ==
                            UIScreenOwnershipMode.SceneOwned &&
                        definition.SceneOwnedView != null)
                    {
                        DeactivateSurface(
                            definition.SceneOwnedView);
                    }
                }
            }

            OnModalStackChanged();

            return UISurfaceOperationResult.Success(
                message:
                    "Looking Glass blocking Modal lifecycle initialized.");
        }

        public UISurfaceOperationResult RegisterExternalModalView(
            string modalId,
            UISurface view)
        {
            if (!IsModalLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    surfaceId: modalId,
                    message:
                        "Blocking Modal lifecycle must initialize before registering ExternalOwned views.");
            }

            if (!modalService.TryGetDefinition(
                    modalId,
                    out UIModalDefinition definition) ||
                definition.OwnershipMode !=
                    UIScreenOwnershipMode.ExternalOwned)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId: modalId,
                    message:
                        "Requested Modal is not an ExternalOwned definition.");
            }

            if (view == null ||
                view.Role != UISurfaceRole.Modal ||
                !string.Equals(
                    view.SurfaceId,
                    definition.ModalId.Value,
                    StringComparison.Ordinal))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId:
                        definition.ModalId.Value,
                    message:
                        "ExternalOwned Modal view identity/role does not match its definition.");
            }

            string normalized =
                definition.ModalId.Value;

            if (externalModalViews.TryGetValue(
                    normalized,
                    out UISurface existing) &&
                existing != null &&
                existing != view)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.DuplicateSurfaceId,
                    surfaceId: normalized,
                    message:
                        "A different ExternalOwned Modal view is already registered for this ID.");
            }

            string registrationError =
                RegisterRuntimeModalSurface(
                    view);

            if (!string.IsNullOrWhiteSpace(
                    registrationError))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.DuplicateSurfaceId,
                    surfaceId: normalized,
                    message:
                        registrationError);
            }

            externalModalViews[normalized] =
                view;

            return UISurfaceOperationResult.Success(
                normalized,
                message:
                    "ExternalOwned Modal view registered.");
        }

        public UISurfaceOperationResult UnregisterExternalModalView(
            string modalId)
        {
            if (!IsModalLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    surfaceId: modalId,
                    message:
                        "Blocking Modal lifecycle is not initialized.");
            }

            string normalized =
                modalId == null
                    ? string.Empty
                    : modalId.Trim();

            if (!externalModalViews.TryGetValue(
                    normalized,
                    out UISurface view))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.UnknownSurface,
                    surfaceId: normalized,
                    message:
                        "No ExternalOwned Modal view is registered for this ID.");
            }

            modalService.AbortByModalId(
                normalized,
                UIModalAbortReason.OwnerLost);

            externalModalViews.Remove(
                normalized);

            if (view != null)
            {
                UnregisterRuntimeModalSurface(
                    view);
            }

            return UISurfaceOperationResult.Success(
                normalized,
                message:
                    "ExternalOwned Modal view unregistered without destroying it.");
        }

        public UIModalHandle OpenModal(
            string modalId)
        {
            if (!IsModalLifecycleInitialized)
            {
                return UIModalHandle.Rejected(
                    new UIModalId(modalId),
                    0,
                    "Looking Glass blocking Modal lifecycle is not initialized.");
            }

            RefreshModalLifecycle();

            return modalService.Open(
                modalId);
        }

        public UIModalCompletionAttemptResult CompleteModal(
            UIModalHandle handle,
            string resultId)
        {
            if (!IsModalLifecycleInitialized)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotFound,
                    "Looking Glass blocking Modal lifecycle is not initialized.");
            }

            RefreshModalLifecycle();

            return modalService.Complete(
                handle,
                resultId);
        }

        public UIModalCompletionAttemptResult AbortModal(
            UIModalHandle handle,
            UIModalAbortReason reason =
                UIModalAbortReason.ExplicitAbort)
        {
            if (!IsModalLifecycleInitialized)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotFound,
                    "Looking Glass blocking Modal lifecycle is not initialized.");
            }

            RefreshModalLifecycle();

            return modalService.Abort(
                handle,
                reason);
        }

        public UIModalCompletionAttemptResult HandleModalBack()
        {
            if (!IsModalLifecycleInitialized)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotFound,
                    "Looking Glass blocking Modal lifecycle is not initialized.");
            }

            RefreshModalLifecycle();

            return modalService.HandleBack();
        }

        public void RefreshModalLifecycle()
        {
            if (!IsModalLifecycleInitialized)
            {
                return;
            }

            if (modalService.SweepLostViews())
            {
                RemoveDestroyedModalSurfaceRegistrations();
            }
        }

        public UISurfaceOperationResult RegisterExternalScreenView(
            string screenId,
            UISurface view)
        {
            if (!IsScreenLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    surfaceId: screenId,
                    message: "Screen lifecycle must initialize before registering ExternalOwned views.");
            }

            if (!screenNavigator.TryGetDefinition(
                    screenId,
                    out UIScreenDefinition definition) ||
                definition.OwnershipMode !=
                    UIScreenOwnershipMode.ExternalOwned)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId: screenId,
                    message: "Requested Screen is not an ExternalOwned definition.");
            }

            if (view == null)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId: definition.ScreenId,
                    scopeId: definition.NavigationScopeId,
                    message: "ExternalOwned Screen view is missing.");
            }

            if (!string.Equals(
                    view.SurfaceId,
                    definition.ScreenId,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    view.NavigationScopeId,
                    definition.NavigationScopeId,
                    StringComparison.Ordinal) ||
                view.Role != UISurfaceRole.Screen)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId: definition.ScreenId,
                    scopeId: definition.NavigationScopeId,
                    message: "ExternalOwned Screen view identity/role does not match its definition.");
            }

            if (externalScreenViews.TryGetValue(
                    definition.ScreenId,
                    out UISurface existing) &&
                existing != null &&
                existing != view)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.DuplicateSurfaceId,
                    surfaceId: definition.ScreenId,
                    scopeId: definition.NavigationScopeId,
                    message: "A different ExternalOwned view is already registered for this Screen.");
            }

            string registrationError =
                RegisterRuntimeScreenSurface(
                    view);

            if (!string.IsNullOrWhiteSpace(
                    registrationError))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.DuplicateSurfaceId,
                    surfaceId: definition.ScreenId,
                    scopeId: definition.NavigationScopeId,
                    message: registrationError);
            }

            externalScreenViews[definition.ScreenId] =
                view;

            return UISurfaceOperationResult.Success(
                definition.ScreenId,
                definition.NavigationScopeId,
                "ExternalOwned Screen view registered.");
        }

        public UISurfaceOperationResult UnregisterExternalScreenView(
            string screenId)
        {
            if (!IsScreenLifecycleInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    surfaceId: screenId,
                    message: "Screen lifecycle is not initialized.");
            }

            string normalized =
                screenId == null
                    ? string.Empty
                    : screenId.Trim();

            if (!externalScreenViews.TryGetValue(
                    normalized,
                    out UISurface view))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.UnknownSurface,
                    surfaceId: normalized,
                    message: "No ExternalOwned Screen view is registered for this ID.");
            }

            externalScreenViews.Remove(
                normalized);

            if (view != null)
            {
                UnregisterRuntimeScreenSurface(
                    view);
            }

            return UISurfaceOperationResult.Success(
                normalized,
                message: "ExternalOwned Screen view unregistered without destroying it.");
        }

        public UIScreenHandle PushScreen(
            string screenId) =>
            SubmitScreenOperation(
                UIScreenOperationRequest.Push(
                    screenId));

        public UIScreenHandle ReplaceScreen(
            string screenId) =>
            SubmitScreenOperation(
                UIScreenOperationRequest.Replace(
                    screenId));

        public UIScreenHandle ResetScreen(
            string screenId) =>
            SubmitScreenOperation(
                UIScreenOperationRequest.Reset(
                    screenId));

        public UIScreenHandle BackScreen(
            string scopeId) =>
            SubmitScreenOperation(
                UIScreenOperationRequest.Back(
                    scopeId));

        public UIScreenHandle CloseScreen(
            string screenId,
            string scopeId = "") =>
            SubmitScreenOperation(
                UIScreenOperationRequest.Close(
                    screenId,
                    scopeId));

        public UIScreenHandle SubmitScreenOperation(
            UIScreenOperationRequest request)
        {
            if (request == null)
            {
                UIScreenOperationRequest placeholder =
                    UIScreenOperationRequest.Push(
                        string.Empty)
                    .WithSequence(
                        ++nextScreenOperationSequence);

                return UIScreenHandle.Rejected(
                    placeholder,
                    "Screen operation request is missing.");
            }

            UIScreenOperationRequest sequenced =
                request.WithSequence(
                    ++nextScreenOperationSequence);

            if (!IsScreenLifecycleInitialized)
            {
                return UIScreenHandle.Rejected(
                    sequenced,
                    "Looking Glass Screen lifecycle is not initialized.");
            }

            if (HasBlockingModal)
            {
                if (modalScreenMutationPolicy ==
                        UIModalScreenMutationPolicy.Reject)
                {
                    return UIScreenHandle.Rejected(
                        sequenced,
                        UIScreenOperationStatus.BlockedByModal,
                        "Screen operation was blocked by an active blocking Modal.");
                }

                if (deferredScreenOperationQueue == null)
                {
                    return UIScreenHandle.Rejected(
                        sequenced,
                        "Deferred Screen operation queue is unavailable.");
                }

                if (!deferredScreenOperationQueue.TryEnqueue(
                        sequenced,
                        out UIScreenHandle deferredHandle))
                {
                    return deferredHandle;
                }

                return deferredHandle;
            }

            if (!screenOperationQueue.TryEnqueue(
                    sequenced,
                    out UIScreenHandle handle))
            {
                return handle;
            }

            DrainScreenOperationQueue();

            return handle;
        }

        public IReadOnlyList<UILayerHost> GetResolvedScreenLayerHosts()
        {
            if (screenLayerRegistry == null)
            {
                return Array.Empty<UILayerHost>();
            }

            return screenLayerRegistry.OrderedHosts;
        }

        private void DrainScreenOperationQueue()
        {
            if (processingScreenOperations ||
                screenOperationQueue == null ||
                screenNavigator == null)
            {
                return;
            }

            processingScreenOperations = true;

            try
            {
                while (screenOperationQueue.TryProcessNext(
                    screenNavigator.Execute,
                    out _))
                {
                }
            }
            finally
            {
                processingScreenOperations = false;
            }
        }

        private void DrainDeferredScreenOperationQueue()
        {
            if (HasBlockingModal ||
                deferredScreenOperationQueue == null ||
                screenNavigator == null)
            {
                return;
            }

            while (deferredScreenOperationQueue.TryProcessNext(
                screenNavigator.Execute,
                out _))
            {
            }
        }

        private void OnModalStackChanged()
        {
            ApplyModalInteractionBlocking();

            if (!shuttingDown &&
                !HasBlockingModal)
            {
                DrainDeferredScreenOperationQueue();
            }
        }

        private void ApplyModalInteractionBlocking()
        {
            UISurface top =
                HasBlockingModal &&
                modalService.TopEntry != null
                    ? modalService.TopEntry.View
                    : null;

            bool hasModal =
                top != null;

            foreach (UISurface surface in surfaces.Values)
            {
                if (surface == null)
                {
                    continue;
                }

                bool block =
                    hasModal &&
                    surface != top;

                surface.SetModalInteractionBlocked(
                    block);

                if (block)
                {
                    selectionCoordinator.ClearSelectionForSurface(
                        surface);
                }
            }
        }

        private void RemoveDestroyedModalSurfaceRegistrations()
        {
            List<string> deadSurfaceIds =
                new List<string>();

            foreach (KeyValuePair<string, UISurface> pair in surfaces)
            {
                if (pair.Value == null)
                {
                    deadSurfaceIds.Add(
                        pair.Key);
                }
            }

            for (int index = 0;
                 index < deadSurfaceIds.Count;
                 index++)
            {
                surfaces.Remove(
                    deadSurfaceIds[index]);
            }

            List<string> deadExternalIds =
                new List<string>();

            foreach (KeyValuePair<string, UISurface> pair in externalModalViews)
            {
                if (pair.Value == null)
                {
                    deadExternalIds.Add(
                        pair.Key);
                }
            }

            for (int index = 0;
                 index < deadExternalIds.Count;
                 index++)
            {
                externalModalViews.Remove(
                    deadExternalIds[index]);
            }

            List<UISurface> deadResponseSurfaces =
                new List<UISurface>();

            foreach (UISurface surface in
                     responseApplicationStateBySurface.Keys)
            {
                if (surface == null)
                {
                    deadResponseSurfaces.Add(
                        surface);
                }
            }

            for (int index = 0;
                 index < deadResponseSurfaces.Count;
                 index++)
            {
                responseApplicationStateBySurface.Remove(
                    deadResponseSurfaces[index]);
            }
        }

        private UISurface ResolveExternalModalView(
            string modalId)
        {
            string normalized =
                modalId == null
                    ? string.Empty
                    : modalId.Trim();

            return externalModalViews.TryGetValue(
                    normalized,
                    out UISurface view)
                ? view
                : null;
        }

        private string RegisterRuntimeModalSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return "Runtime Modal surface is missing.";
            }

            string id =
                surface.SurfaceId;

            if (string.IsNullOrWhiteSpace(id))
            {
                return "Runtime Modal surface has an empty stable ID.";
            }

            if (surface.Role != UISurfaceRole.Modal)
            {
                return
                    "Runtime Modal surface '" +
                    id +
                    "' is not configured with Modal role.";
            }

            if (surfaces.TryGetValue(
                    id,
                    out UISurface existing))
            {
                return existing == surface
                    ? string.Empty
                    : "A different registered surface already uses stable ID '" +
                        id +
                        "'.";
            }

            surfaces.Add(
                id,
                surface);

            surface.SetVisible(
                false);

            return string.Empty;
        }

        private void UnregisterRuntimeModalSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            if (surfaces.TryGetValue(
                    surface.SurfaceId,
                    out UISurface registered) &&
                registered == surface)
            {
                surfaces.Remove(
                    surface.SurfaceId);
            }

            responseApplicationStateBySurface.Remove(
                surface);
        }

        private UISurface ResolveExternalScreenView(
            string screenId)
        {
            string normalized =
                screenId == null
                    ? string.Empty
                    : screenId.Trim();

            return externalScreenViews.TryGetValue(
                    normalized,
                    out UISurface view)
                ? view
                : null;
        }

        private string RegisterRuntimeScreenSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return "Runtime Screen surface is missing.";
            }

            string id =
                surface.SurfaceId;

            if (string.IsNullOrWhiteSpace(id))
            {
                return "Runtime Screen surface has an empty stable ID.";
            }

            if (surface.Role != UISurfaceRole.Screen)
            {
                return
                    "Runtime Screen surface '" +
                    id +
                    "' is not configured with Screen role.";
            }

            if (surfaces.TryGetValue(
                    id,
                    out UISurface existing))
            {
                return existing == surface
                    ? string.Empty
                    : "A different registered surface already uses stable ID '" +
                        id +
                        "'.";
            }

            surfaces.Add(
                id,
                surface);

            surface.SetVisible(false);

            return string.Empty;
        }

        private void UnregisterRuntimeScreenSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            if (surfaces.TryGetValue(
                    surface.SurfaceId,
                    out UISurface registered) &&
                registered == surface)
            {
                surfaces.Remove(
                    surface.SurfaceId);
            }

            responseApplicationStateBySurface.Remove(
                surface);
        }

        private void ActivateScreenEntry(
            UIScreenEntry entry)
        {
            if (entry == null ||
                entry.View == null)
            {
                return;
            }

            entry.View.SetScreenSuspended(
                false,
                entry.Definition.SuspensionVisibility);

            ActivateSurface(
                entry.View);
        }

        private void SuspendScreenEntry(
            UIScreenEntry entry)
        {
            if (entry == null ||
                entry.View == null)
            {
                return;
            }

            selectionCoordinator.ClearSelectionForSurface(
                entry.View);

            entry.View.SetScreenSuspended(
                true,
                entry.Definition.SuspensionVisibility);

            ApplyCurrentContext(
                entry.View);
        }

        private void ResumeScreenEntry(
            UIScreenEntry entry)
        {
            if (entry == null ||
                entry.View == null)
            {
                return;
            }

            entry.View.SetScreenSuspended(
                false,
                entry.Definition.SuspensionVisibility);

            ActivateSurface(
                entry.View);
        }

        private void CloseScreenEntry(
            UIScreenEntry entry)
        {
            if (entry == null ||
                entry.View == null)
            {
                return;
            }

            entry.View.SetScreenSuspended(
                false,
                entry.Definition.SuspensionVisibility);

            DeactivateSurface(
                entry.View);
        }

        private UISurfaceOperationResult ToSurfaceOperationResult(
            UIScreenHandle handle)
        {
            if (handle == null)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    message: "Screen operation did not produce a handle.");
            }

            if (!handle.IsCompleted)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.InvalidDefinition,
                    surfaceId: handle.Request.ScreenId,
                    scopeId: handle.Request.ScopeId,
                    message: "Screen operation did not settle synchronously.");
            }

            UIScreenOperationResult result =
                handle.Result;

            if (result.Status ==
                    UIScreenOperationStatus.Succeeded ||
                result.Status ==
                    UIScreenOperationStatus.NoChange)
            {
                return UISurfaceOperationResult.Success(
                    result.ScreenId,
                    result.ScopeId,
                    result.Message);
            }

            if (result.Status ==
                    UIScreenOperationStatus.BlockedByModal)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.BlockedByModal,
                    surfaceId: result.ScreenId,
                    scopeId: result.ScopeId,
                    message: result.Message);
            }

            return new UISurfaceOperationResult(
                UISurfaceOperationStatus.InvalidDefinition,
                surfaceId: result.ScreenId,
                scopeId: result.ScopeId,
                message: result.Message);
        }

        private void TryClaimAuthority()
        {
            if (active == null ||
                active == this)
            {
                active = this;
                IsAuthoritative = true;
                return;
            }

            IsAuthoritative = false;
        }

        private void ActivateSurface(
            UISurface surface)
        {
            RecordDirectVisibilityIntent(
                surface,
                true);
            surface.SetVisible(true);
            selectionCoordinator.ApplyOpenSelection(
                surface,
                inputModality);
            ApplyCurrentContext(
                surface);
        }

        private void DeactivateSurface(
            UISurface surface)
        {
            selectionCoordinator.ClearSelectionForSurface(
                surface);
            RecordDirectVisibilityIntent(
                surface,
                false);
            surface.SetVisible(false);
        }

        private void ApplyCurrentContext(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            UISurfaceContextResponse response =
                surface.ResolveContextResponse(
                    contextState);

            ApplyResolvedVisibility(
                surface,
                response.Visibility);
            ApplyResolvedInteraction(
                surface,
                response.Interaction);

            selectionCoordinator.ApplyContextSelection(
                surface,
                response.Selection);
        }

        private void ApplyResolvedVisibility(
            UISurface surface,
            UISurfaceVisibilityIntent intent)
        {
            SurfaceResponseApplicationState state =
                GetResponseApplicationState(surface);

            if (intent == UISurfaceVisibilityIntent.NoChange)
            {
                if (!state.VisibilityControlled)
                {
                    return;
                }

                surface.SetVisible(
                    state.VisibilityBaseline);
                state.VisibilityControlled = false;
                return;
            }

            if (!state.VisibilityControlled)
            {
                state.VisibilityBaseline =
                    surface.RequestedVisibility;
                state.VisibilityControlled = true;
            }

            if (intent == UISurfaceVisibilityIntent.Visible)
            {
                ApplyContextVisible(surface);
                return;
            }

            selectionCoordinator.ClearSelectionForSurface(
                surface);
            surface.SetVisible(false);
        }

        private void ApplyResolvedInteraction(
            UISurface surface,
            UISurfaceInteractionIntent intent)
        {
            SurfaceResponseApplicationState state =
                GetResponseApplicationState(surface);

            if (intent == UISurfaceInteractionIntent.NoChange)
            {
                if (!state.InteractionControlled)
                {
                    return;
                }

                surface.SetInteractable(
                    state.InteractionBaseline);
                state.InteractionControlled = false;
                return;
            }

            if (!state.InteractionControlled)
            {
                state.InteractionBaseline =
                    surface.RequestedInteractability;
            }

            bool applied =
                surface.SetInteractable(
                    intent == UISurfaceInteractionIntent.Interactable);
            if (applied)
            {
                state.InteractionControlled = true;
            }
        }

        private void RecordDirectVisibilityIntent(
            UISurface surface,
            bool visible)
        {
            if (surface == null ||
                !responseApplicationStateBySurface.TryGetValue(
                    surface,
                    out SurfaceResponseApplicationState state) ||
                !state.VisibilityControlled)
            {
                return;
            }

            state.VisibilityBaseline = visible;
        }

        private SurfaceResponseApplicationState GetResponseApplicationState(
            UISurface surface)
        {
            if (!responseApplicationStateBySurface.TryGetValue(
                    surface,
                    out SurfaceResponseApplicationState state))
            {
                state =
                    new SurfaceResponseApplicationState();
                responseApplicationStateBySurface.Add(
                    surface,
                    state);
            }

            return state;
        }

        private void ApplyContextVisible(
            UISurface surface)
        {
            if (surface.Role != UISurfaceRole.Screen)
            {
                surface.SetVisible(true);
                return;
            }

            string currentId =
                GetCurrentScreenId(
                    surface.NavigationScopeId);

            if (string.Equals(
                    currentId,
                    surface.SurfaceId,
                    StringComparison.Ordinal))
            {
                surface.SetVisible(true);
            }
        }

        private sealed class SurfaceResponseApplicationState
        {
            public bool VisibilityControlled;
            public bool VisibilityBaseline;
            public bool InteractionControlled;
            public bool InteractionBaseline;
        }

        private UISurfaceOperationResult ResolveSurface(
            string requestedId,
            out UISurface surface)
        {
            surface = null;
            if (!IsAuthoritative)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotAuthoritative,
                    message: "Only the authoritative Looking Glass root may operate surfaces.");
            }
            if (!IsInitialized)
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.NotInitialized,
                    message: "Looking Glass is not initialized.");
            }

            string id =
                requestedId == null
                    ? string.Empty
                    : requestedId.Trim();
            if (string.IsNullOrWhiteSpace(id) ||
                !surfaces.TryGetValue(
                    id,
                    out surface))
            {
                return new UISurfaceOperationResult(
                    UISurfaceOperationStatus.UnknownSurface,
                    surfaceId: id,
                    message: "No registered UI surface matches the requested stable ID.");
            }

            return UISurfaceOperationResult.Success(
                id);
        }
    }
}
