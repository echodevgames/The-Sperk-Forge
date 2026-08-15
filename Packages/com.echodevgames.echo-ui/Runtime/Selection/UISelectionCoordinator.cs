using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Event-driven EventSystem focus adapter for Looking Glass surfaces.
    /// It consumes externally supplied modality and never detects devices or owns input actions.
    /// </summary>
    public sealed class UISelectionCoordinator
    {
        private readonly Dictionary<UISurface, GameObject> liveMemory =
            new Dictionary<UISurface, GameObject>();

        private readonly Dictionary<string, GameObject> sessionMemory =
            new Dictionary<string, GameObject>(
                StringComparer.Ordinal);

        private readonly HashSet<UISurface> suspended =
            new HashSet<UISurface>();

        private readonly Stack<UISurface> modalReturnSurfaceStack =
            new Stack<UISurface>();

        private EventSystem coordinatedEventSystem;
        private bool eventSystemCoordinationConfigured;
        private GameObject globalFallback;
        private UISurface lastFocusedSurface;
        private int previousModalCount;

        public long Generation { get; private set; }

        public EventSystem CoordinatedEventSystem =>
            ResolveEventSystem();

        public static GameObject CurrentSelectedObject
        {
            get
            {
                EventSystem eventSystem =
                    ResolveLegacyEventSystem();

                return eventSystem != null
                    ? eventSystem.currentSelectedGameObject
                    : null;
            }
        }

        public void ConfigureEventSystem(
            EventSystem eventSystem,
            bool coordinationConfigured = true)
        {
            coordinatedEventSystem =
                eventSystem;

            eventSystemCoordinationConfigured =
                coordinationConfigured;

            BumpGeneration();
        }

        public void SetGlobalFallback(
            GameObject fallback)
        {
            globalFallback = fallback;
            BumpGeneration();
        }

        public void Reset()
        {
            liveMemory.Clear();
            sessionMemory.Clear();
            suspended.Clear();
            modalReturnSurfaceStack.Clear();

            coordinatedEventSystem = null;
            eventSystemCoordinationConfigured = false;
            globalFallback = null;
            lastFocusedSurface = null;
            previousModalCount = 0;
            BumpGeneration();
        }

        public void ApplyOpenSelection(
            UISurface surface,
            UIInputModality modality)
        {
            if (surface == null ||
                surface.SelectionPolicy == null)
            {
                return;
            }

            CaptureCurrentFocus();

            if (surface.Role ==
                UISurfaceRole.Modal)
            {
                modalReturnSurfaceStack.Push(
                    lastFocusedSurface);
            }

            bool wasSuspended =
                suspended.Remove(
                    surface);

            BumpGeneration();

            if (wasSuspended &&
                surface.SelectionPolicy.RestoreFocusWhenReexposed &&
                TryResolveRemembered(
                    surface,
                    useSessionMemory: false,
                    out GameObject liveTarget,
                    out UIFocusResolutionSource liveSource))
            {
                ApplyResolvedTarget(
                    surface,
                    liveTarget,
                    liveSource,
                    "Restored live-entry focus.");
                return;
            }

            if (surface.SelectionPolicy.ReopenBehavior ==
                    UIFocusReopenBehavior.RememberThisSession &&
                TryResolveRemembered(
                    surface,
                    useSessionMemory: true,
                    out GameObject sessionTarget,
                    out UIFocusResolutionSource sessionSource))
            {
                ApplyResolvedTarget(
                    surface,
                    sessionTarget,
                    sessionSource,
                    "Restored root-session focus.");
                return;
            }

            UISelectionOpenBehavior behavior =
                surface.SelectionPolicy.GetOpenBehavior(
                    modality);

            switch (behavior)
            {
                case UISelectionOpenBehavior.ClearSelection:
                    ClearSelection();
                    break;

                case UISelectionOpenBehavior.SelectDefault:
                    ResolveAndApply(
                        surface,
                        null,
                        includeLiveMemory: false,
                        includeSessionMemory: false,
                        allowNoFocus: true,
                        "Applied authored opening focus policy.");
                    break;
            }
        }

        public void ApplyContextSelection(
            UISurface surface,
            UISurfaceSelectionIntent intent)
        {
            switch (intent)
            {
                case UISurfaceSelectionIntent.ClearSelection:
                    ClearSelectionForSurface(
                        surface);
                    break;

                case UISurfaceSelectionIntent.SelectDefault:
                    ResolveAndApply(
                        surface,
                        null,
                        includeLiveMemory: false,
                        includeSessionMemory: false,
                        allowNoFocus: true,
                        "Applied context-driven focus policy.");
                    break;
            }
        }

        public void ClearSelectionForSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return;
            }

            GameObject selected =
                eventSystem.currentSelectedGameObject;

            if (!BelongsToSurface(
                    surface,
                    selected))
            {
                return;
            }

            Remember(
                surface,
                selected,
                includeSessionMemory: false);

            eventSystem.SetSelectedGameObject(
                null);

            BumpGeneration();
        }

        public void SuspendSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            CaptureSurfaceCurrentFocus(
                surface);

            suspended.Add(
                surface);

            ClearSelectedObjectIfOwnedBy(
                surface);

            BumpGeneration();
        }

        public void CloseSurface(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            CaptureSurfaceCurrentFocus(
                surface);

            if (surface.SelectionPolicy != null &&
                surface.SelectionPolicy.ReopenBehavior ==
                    UIFocusReopenBehavior.RememberThisSession &&
                liveMemory.TryGetValue(
                    surface,
                    out GameObject remembered) &&
                IsRememberableTarget(
                    surface,
                    remembered))
            {
                sessionMemory[surface.SurfaceId] =
                    remembered;
            }

            liveMemory.Remove(
                surface);

            suspended.Remove(
                surface);

            ClearSelectedObjectIfOwnedBy(
                surface);

            BumpGeneration();
        }

        public void RememberCurrentFocus()
        {
            CaptureCurrentFocus();
        }

        public bool TryGetRememberedFocus(
            UISurface surface,
            out GameObject target)
        {
            target = null;
            if (surface == null ||
                !liveMemory.TryGetValue(
                    surface,
                    out GameObject remembered) ||
                !IsValidTarget(
                    surface,
                    remembered))
            {
                return false;
            }

            target = remembered;
            return true;
        }

        public bool TryGetSessionRememberedFocus(
            string surfaceId,
            out GameObject target)
        {
            target = null;

            string normalized =
                Normalize(
                    surfaceId);

            return !string.IsNullOrWhiteSpace(
                    normalized) &&
                sessionMemory.TryGetValue(
                    normalized,
                    out target) &&
                target != null &&
                target.activeInHierarchy;
        }

        public UIFocusRequestResult RequestFocus(
            UISurface surface,
            GameObject explicitTarget,
            UIInputModality modality,
            long expectedGeneration)
        {
            if (expectedGeneration >= 0 &&
                expectedGeneration != Generation)
            {
                return new UIFocusRequestResult(
                    UIFocusRequestStatus.Stale,
                    UIFocusResolutionSource.None,
                    GetCurrentSelectedObject(),
                    Generation,
                    "Focus request generation is stale and was rejected.");
            }

            if (surface == null)
            {
                return UIFocusRequestResult.Unavailable(
                    Generation,
                    "Focus request surface is missing.");
            }

            BumpGeneration();

            return ResolveAndApply(
                surface,
                explicitTarget,
                includeLiveMemory: true,
                includeSessionMemory:
                    surface.SelectionPolicy != null &&
                    surface.SelectionPolicy.ReopenBehavior ==
                        UIFocusReopenBehavior.RememberThisSession,
                allowNoFocus: true,
                "Explicit focus request resolved.");
        }

        public UIFocusRequestResult Revalidate(
            UIInputModality modality,
            UISurface topModal,
            IEnumerable<UISurface> surfaces,
            long expectedGeneration)
        {
            if (expectedGeneration >= 0 &&
                expectedGeneration != Generation)
            {
                return new UIFocusRequestResult(
                    UIFocusRequestStatus.Stale,
                    UIFocusResolutionSource.None,
                    GetCurrentSelectedObject(),
                    Generation,
                    "Focus revalidation generation is stale and was rejected.");
            }

            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return UIFocusRequestResult.Unavailable(
                    Generation,
                    "No coordinated EventSystem is available.");
            }

            GameObject current =
                eventSystem.currentSelectedGameObject;

            UISurface owner =
                FindOwningSurface(
                    current);

            if (topModal != null)
            {
                if (owner == topModal &&
                    IsValidTarget(
                        topModal,
                        current))
                {
                    return new UIFocusRequestResult(
                        UIFocusRequestStatus.NoChange,
                        UIFocusResolutionSource.None,
                        current,
                        Generation,
                        "Current focus is already legal inside the top Modal.");
                }

                BumpGeneration();

                return ResolveAndApply(
                    topModal,
                    null,
                    includeLiveMemory: true,
                    includeSessionMemory: false,
                    allowNoFocus: true,
                    "Blocking Modal containment repaired focus.");
            }

            if (owner != null &&
                IsValidTarget(
                    owner,
                    current))
            {
                Remember(
                    owner,
                    current,
                    includeSessionMemory: false);

                return new UIFocusRequestResult(
                    UIFocusRequestStatus.NoChange,
                    UIFocusResolutionSource.None,
                    current,
                    Generation,
                    "Current focus remains valid.");
            }

            UISurface candidate =
                lastFocusedSurface;

            if (!IsEligibleSurface(
                    candidate))
            {
                candidate =
                    FindDeterministicEligibleSurface(
                        surfaces);
            }

            if (candidate == null)
            {
                eventSystem.SetSelectedGameObject(
                    null);

                BumpGeneration();

                return new UIFocusRequestResult(
                    UIFocusRequestStatus.NoFocus,
                    UIFocusResolutionSource.None,
                    null,
                    Generation,
                    "No eligible focus surface remains.");
            }

            BumpGeneration();

            return ResolveAndApply(
                candidate,
                null,
                includeLiveMemory: true,
                includeSessionMemory:
                    candidate.SelectionPolicy != null &&
                    candidate.SelectionPolicy.ReopenBehavior ==
                        UIFocusReopenBehavior.RememberThisSession,
                allowNoFocus: true,
                "Explicit focus revalidation completed.");
        }

        public void ApplyModalStackChanged(
            UISurface topModal,
            int activeModalCount,
            UIInputModality modality,
            IEnumerable<UISurface> surfaces)
        {
            int normalizedCount =
                activeModalCount < 0
                    ? 0
                    : activeModalCount;

            if (normalizedCount < previousModalCount)
            {
                int closedCount =
                    previousModalCount -
                    normalizedCount;

                UISurface restoreSurface = null;
                for (int index = 0;
                     index < closedCount;
                     index++)
                {
                    if (modalReturnSurfaceStack.Count > 0)
                    {
                        restoreSurface =
                            modalReturnSurfaceStack.Pop();
                    }
                }

                BumpGeneration();

                if (topModal != null)
                {
                    restoreSurface = topModal;
                }

                if (restoreSurface != null &&
                    restoreSurface.SelectionPolicy != null &&
                    restoreSurface.SelectionPolicy.RestoreFocusWhenReexposed &&
                    IsEligibleSurface(
                        restoreSurface))
                {
                    ResolveAndApply(
                        restoreSurface,
                        null,
                        includeLiveMemory: true,
                        includeSessionMemory:
                            restoreSurface.SelectionPolicy.ReopenBehavior ==
                                UIFocusReopenBehavior.RememberThisSession,
                        allowNoFocus: true,
                        "Modal completion restored newly exposed focus.");
                }
                else if (topModal == null)
                {
                    Revalidate(
                        modality,
                        null,
                        surfaces,
                        Generation);
                }
            }

            previousModalCount =
                normalizedCount;
        }

        public void OnInputModalityChanged(
            UIInputModality modality,
            IEnumerable<UISurface> surfaces)
        {
            if (modality !=
                UIInputModality.Navigation)
            {
                return;
            }

            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null ||
                eventSystem.currentSelectedGameObject != null)
            {
                return;
            }

            UISurface modal =
                FindTopEligibleModal(
                    surfaces);

            UISurface candidate =
                modal != null
                    ? modal
                    : lastFocusedSurface;

            if (!IsEligibleSurface(
                    candidate))
            {
                return;
            }

            BumpGeneration();

            ResolveAndApply(
                candidate,
                null,
                includeLiveMemory: true,
                includeSessionMemory:
                    candidate.SelectionPolicy != null &&
                    candidate.SelectionPolicy.ReopenBehavior ==
                        UIFocusReopenBehavior.RememberThisSession,
                allowNoFocus: true,
                "Navigation modality established eligible focus.");
        }

        private UIFocusRequestResult ResolveAndApply(
            UISurface surface,
            GameObject explicitTarget,
            bool includeLiveMemory,
            bool includeSessionMemory,
            bool allowNoFocus,
            string message)
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return UIFocusRequestResult.Unavailable(
                    Generation,
                    "No coordinated EventSystem is available.");
            }

            if (!IsEligibleSurface(
                    surface))
            {
                eventSystem.SetSelectedGameObject(
                    null);

                return new UIFocusRequestResult(
                    UIFocusRequestStatus.Blocked,
                    UIFocusResolutionSource.None,
                    null,
                    Generation,
                    "Requested focus surface is not currently eligible.");
            }

            if (IsValidTarget(
                    surface,
                    explicitTarget))
            {
                return ApplyResolvedTarget(
                    surface,
                    explicitTarget,
                    UIFocusResolutionSource.Explicit,
                    message);
            }

            if (includeLiveMemory &&
                liveMemory.TryGetValue(
                    surface,
                    out GameObject liveTarget) &&
                IsValidTarget(
                    surface,
                    liveTarget))
            {
                return ApplyResolvedTarget(
                    surface,
                    liveTarget,
                    UIFocusResolutionSource.LiveMemory,
                    message);
            }

            if (includeSessionMemory &&
                sessionMemory.TryGetValue(
                    surface.SurfaceId,
                    out GameObject sessionTarget) &&
                IsValidTarget(
                    surface,
                    sessionTarget))
            {
                return ApplyResolvedTarget(
                    surface,
                    sessionTarget,
                    UIFocusResolutionSource.SessionMemory,
                    message);
            }

            GameObject authoredDefault =
                surface.SelectionPolicy != null
                    ? surface.SelectionPolicy.DefaultSelectionTarget
                    : null;

            if (IsValidTarget(
                    surface,
                    authoredDefault))
            {
                return ApplyResolvedTarget(
                    surface,
                    authoredDefault,
                    UIFocusResolutionSource.AuthoredDefault,
                    message);
            }

            GameObject resolved =
                ResolveEntryTarget(
                    surface);

            if (IsValidTarget(
                    surface,
                    resolved))
            {
                return ApplyResolvedTarget(
                    surface,
                    resolved,
                    UIFocusResolutionSource.EntryResolver,
                    message);
            }

            if (IsValidTarget(
                    surface,
                    globalFallback))
            {
                return ApplyResolvedTarget(
                    surface,
                    globalFallback,
                    UIFocusResolutionSource.GlobalFallback,
                    message);
            }

            if (allowNoFocus)
            {
                eventSystem.SetSelectedGameObject(
                    null);

                return new UIFocusRequestResult(
                    UIFocusRequestStatus.NoFocus,
                    UIFocusResolutionSource.None,
                    null,
                    Generation,
                    "Focus resolution legally ended with no selected object.");
            }

            return new UIFocusRequestResult(
                UIFocusRequestStatus.Unavailable,
                UIFocusResolutionSource.None,
                null,
                Generation,
                "No legal focus target could be resolved.");
        }

        private UIFocusRequestResult ApplyResolvedTarget(
            UISurface surface,
            GameObject target,
            UIFocusResolutionSource source,
            string message)
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null ||
                !IsValidTarget(
                    surface,
                    target))
            {
                return UIFocusRequestResult.Unavailable(
                    Generation,
                    "Resolved focus target became unavailable.");
            }

            eventSystem.SetSelectedGameObject(
                target);

            Remember(
                surface,
                target,
                includeSessionMemory:
                    surface.SelectionPolicy != null &&
                    surface.SelectionPolicy.ReopenBehavior ==
                        UIFocusReopenBehavior.RememberThisSession);

            lastFocusedSurface =
                surface;

            return new UIFocusRequestResult(
                UIFocusRequestStatus.Succeeded,
                source,
                target,
                Generation,
                message);
        }

        private bool TryResolveRemembered(
            UISurface surface,
            bool useSessionMemory,
            out GameObject target,
            out UIFocusResolutionSource source)
        {
            target = null;
            source =
                UIFocusResolutionSource.None;

            Dictionary<string, GameObject> session =
                sessionMemory;

            if (useSessionMemory)
            {
                if (!session.TryGetValue(
                        surface.SurfaceId,
                        out target) ||
                    !IsValidTarget(
                        surface,
                        target))
                {
                    target = null;
                    return false;
                }

                source =
                    UIFocusResolutionSource.SessionMemory;

                return true;
            }

            if (!liveMemory.TryGetValue(
                    surface,
                    out target) ||
                !IsValidTarget(
                    surface,
                    target))
            {
                target = null;
                return false;
            }

            source =
                UIFocusResolutionSource.LiveMemory;

            return true;
        }

        private void CaptureCurrentFocus()
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return;
            }

            GameObject selected =
                eventSystem.currentSelectedGameObject;

            UISurface owner =
                FindOwningSurface(
                    selected);

            if (owner == null ||
                !IsValidTarget(
                    owner,
                    selected))
            {
                return;
            }

            Remember(
                owner,
                selected,
                includeSessionMemory: false);

            lastFocusedSurface =
                owner;
        }

        private void CaptureSurfaceCurrentFocus(
            UISurface surface)
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return;
            }

            GameObject selected =
                eventSystem.currentSelectedGameObject;

            if (!IsRememberableTarget(
                    surface,
                    selected))
            {
                return;
            }

            Remember(
                surface,
                selected,
                includeSessionMemory: false);
        }

        private void Remember(
            UISurface surface,
            GameObject target,
            bool includeSessionMemory)
        {
            if (!IsRememberableTarget(
                    surface,
                    target))
            {
                return;
            }

            liveMemory[surface] =
                target;

            if (includeSessionMemory &&
                surface.SelectionPolicy != null &&
                surface.SelectionPolicy.ReopenBehavior ==
                    UIFocusReopenBehavior.RememberThisSession)
            {
                sessionMemory[surface.SurfaceId] =
                    target;
            }
        }

        private void ClearSelectedObjectIfOwnedBy(
            UISurface surface)
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem == null)
            {
                return;
            }

            if (BelongsToSurface(
                    surface,
                    eventSystem.currentSelectedGameObject))
            {
                eventSystem.SetSelectedGameObject(
                    null);
            }
        }

        private void ClearSelection()
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(
                    null);
            }
        }

        private EventSystem ResolveEventSystem()
        {
            if (eventSystemCoordinationConfigured)
            {
                return coordinatedEventSystem != null &&
                    coordinatedEventSystem.isActiveAndEnabled
                        ? coordinatedEventSystem
                        : null;
            }

            return ResolveLegacyEventSystem();
        }

        private static EventSystem ResolveLegacyEventSystem()
        {
            EventSystem current =
                EventSystem.current;

            if (current != null &&
                current.isActiveAndEnabled)
            {
                return current;
            }

            EventSystem[] systems =
                UnityEngine.Object
                .FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.InstanceID);

            EventSystem resolved = null;
            for (int index = 0;
                 index < systems.Length;
                 index++)
            {
                EventSystem candidate =
                    systems[index];

                if (candidate == null ||
                    !candidate.isActiveAndEnabled)
                {
                    continue;
                }

                if (resolved != null)
                {
                    return null;
                }

                resolved = candidate;
            }

            return resolved;
        }

        private static GameObject ResolveEntryTarget(
            UISurface surface)
        {
            if (surface == null)
            {
                return null;
            }

            MonoBehaviour[] components =
                surface.GetComponents<MonoBehaviour>();

            for (int index = 0;
                 index < components.Length;
                 index++)
            {
                if (components[index] is
                    IUIFocusTargetResolver resolver)
                {
                    GameObject resolved =
                        resolver.ResolveFocusTarget(
                            surface);

                    if (resolved != null)
                    {
                        return resolved;
                    }
                }
            }

            return null;
        }

        private static UISurface FindOwningSurface(
            GameObject target)
        {
            return target != null
                ? target.GetComponentInParent<UISurface>(
                    true)
                : null;
        }

        private static UISurface FindTopEligibleModal(
            IEnumerable<UISurface> surfaces)
        {
            if (surfaces == null)
            {
                return null;
            }

            UISurface resolved = null;
            foreach (UISurface surface in surfaces)
            {
                if (surface == null ||
                    surface.Role != UISurfaceRole.Modal ||
                    !IsEligibleSurface(
                        surface))
                {
                    continue;
                }

                if (resolved != null)
                {
                    return null;
                }

                resolved = surface;
            }

            return resolved;
        }

        private static UISurface FindDeterministicEligibleSurface(
            IEnumerable<UISurface> surfaces)
        {
            if (surfaces == null)
            {
                return null;
            }

            UISurface result = null;
            foreach (UISurface surface in surfaces)
            {
                if (!IsEligibleSurface(
                        surface))
                {
                    continue;
                }

                if (result == null ||
                    string.CompareOrdinal(
                        surface.SurfaceId,
                        result.SurfaceId) < 0)
                {
                    result = surface;
                }
            }

            return result;
        }

        private static bool IsEligibleSurface(
            UISurface surface)
        {
            return surface != null &&
                surface.gameObject.activeInHierarchy &&
                surface.IsVisible &&
                surface.IsInteractable &&
                !surface.IsScreenSuspended &&
                !surface.IsModalInteractionBlocked;
        }

        private static bool IsRememberableTarget(
            UISurface surface,
            GameObject target)
        {
            return surface != null &&
                target != null &&
                BelongsToSurface(
                    surface,
                    target);
        }

        private static bool IsValidTarget(
            UISurface surface,
            GameObject target)
        {
            if (!IsEligibleSurface(
                    surface) ||
                target == null ||
                !target.activeInHierarchy ||
                !BelongsToSurface(
                    surface,
                    target))
            {
                return false;
            }

            Selectable selectable =
                target.GetComponent<Selectable>();

            return selectable == null ||
                selectable.IsInteractable();
        }

        private static bool BelongsToSurface(
            UISurface surface,
            GameObject target)
        {
            if (surface == null ||
                target == null)
            {
                return false;
            }

            return target == surface.gameObject ||
                target.transform.IsChildOf(
                    surface.transform);
        }

        private GameObject GetCurrentSelectedObject()
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            return eventSystem != null
                ? eventSystem.currentSelectedGameObject
                : null;
        }

        private void BumpGeneration()
        {
            if (Generation == long.MaxValue)
            {
                Generation = 1;
                return;
            }

            Generation++;
        }

        private static string Normalize(
            string value) =>
            value == null
                ? string.Empty
                : value.Trim();
    }
}
