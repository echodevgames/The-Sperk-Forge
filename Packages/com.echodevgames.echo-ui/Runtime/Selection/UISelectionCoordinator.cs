using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Event-driven EventSystem selection adapter for Looking Glass surfaces.
    /// It consumes externally supplied modality and never detects devices itself.
    /// </summary>
    public sealed class UISelectionCoordinator
    {
        public static GameObject CurrentSelectedObject
        {
            get
            {
                EventSystem eventSystem =
                    ResolveEventSystem();
                return eventSystem != null
                    ? eventSystem.currentSelectedGameObject
                    : null;
            }
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

            UISelectionOpenBehavior behavior =
                surface.SelectionPolicy.GetOpenBehavior(
                    modality);

            switch (behavior)
            {
                case UISelectionOpenBehavior.ClearSelection:
                    ClearSelection();
                    break;

                case UISelectionOpenBehavior.SelectDefault:
                    SelectDefault(surface);
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
                    ClearSelection();
                    break;

                case UISurfaceSelectionIntent.SelectDefault:
                    SelectDefault(surface);
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
            if (selected == null)
            {
                return;
            }

            Transform selectedTransform =
                selected.transform;
            if (selected == surface.gameObject ||
                selectedTransform.IsChildOf(
                    surface.transform))
            {
                eventSystem.SetSelectedGameObject(
                    null);
            }
        }

        private static void ClearSelection()
        {
            EventSystem eventSystem =
                ResolveEventSystem();
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(
                    null);
            }
        }

        private static void SelectDefault(
            UISurface surface)
        {
            if (surface == null ||
                surface.SelectionPolicy == null)
            {
                return;
            }

            EventSystem eventSystem =
                ResolveEventSystem();
            if (eventSystem == null)
            {
                return;
            }

            GameObject target =
                surface.SelectionPolicy.DefaultSelectionTarget;
            if (!IsValidTarget(
                    surface,
                    target))
            {
                return;
            }

            eventSystem.SetSelectedGameObject(
                target);
        }

        private static EventSystem ResolveEventSystem()
        {
            EventSystem current =
                EventSystem.current;
            if (current != null &&
                current.isActiveAndEnabled)
            {
                return current;
            }

            return UnityEngine.Object
                .FindFirstObjectByType<EventSystem>();
        }

        private static bool IsValidTarget(
            UISurface surface,
            GameObject target)
        {
            if (surface == null ||
                target == null ||
                !target.activeInHierarchy)
            {
                return false;
            }

            return target == surface.gameObject ||
                target.transform.IsChildOf(
                    surface.transform);
        }
    }
}
