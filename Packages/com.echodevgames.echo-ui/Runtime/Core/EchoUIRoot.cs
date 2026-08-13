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

        public static EchoUIRoot Active =>
            active;

        public bool IsAuthoritative { get; private set; }

        public bool IsInitialized { get; private set; }

        public int RegisteredSurfaceCount =>
            surfaces.Count;

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
                    target.SetVisible(true);

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
                    currentSurface.SetVisible(false);
                }
            }

            target.SetVisible(true);
            currentScreenByScope[scopeId] =
                target.SurfaceId;

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
                    current.SetVisible(false);
                }

                previous.SetVisible(true);
                currentScreenByScope[normalizedScope] =
                    previous.SurfaceId;

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

            surface.SetVisible(true);

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

            surface.SetVisible(false);

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

            surface.SetVisible(
                !surface.IsVisible);

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
