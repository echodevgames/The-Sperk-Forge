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

        public static EchoUIRoot Active =>
            active;

        public bool IsAuthoritative { get; private set; }

        public bool IsInitialized { get; private set; }

        public int RegisteredSurfaceCount =>
            surfaces.Count;

        public UIInputModality InputModality =>
            inputModality;

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

        private void OnDestroy()
        {
            if (active == this)
            {
                active = null;
            }

            IsAuthoritative = false;
            IsInitialized = false;
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

            if (surface.Role == UISurfaceRole.Screen)
            {
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

            return currentScreenByScope.TryGetValue(
                    scopeId.Trim(),
                    out string surfaceId)
                ? surfaceId
                : string.Empty;
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
                    surface.IsVisible;
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
                    surface.IsInteractable;
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

            if (currentScreenByScope.TryGetValue(
                    surface.NavigationScopeId,
                    out string currentId) &&
                string.Equals(
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
