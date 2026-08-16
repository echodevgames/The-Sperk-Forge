using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Declares one project-authored UI surface using a stable ID and behavioral role.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UISurface : MonoBehaviour
    {
        [SerializeField]
        private string surfaceId = "surface";

        [SerializeField]
        private string displayLabel = string.Empty;

        [SerializeField]
        private UISurfaceRole role =
            UISurfaceRole.Screen;

        [SerializeField]
        private string navigationScopeId =
            "default";

        [SerializeField]
        private bool startVisible;

        [Header("External Context")]
        [SerializeField]
        private bool allowExternalContext = true;

        [SerializeField]
        private CanvasGroup interactionGroup;

        [SerializeField]
        private List<UISurfaceContextRule> contextRules =
            new List<UISurfaceContextRule>();

        [Header("Selection")]
        [SerializeField]
        private UISurfaceSelectionPolicy selectionPolicy =
            new UISurfaceSelectionPolicy();

        [Header("M3 View Transitions")]
        [SerializeField]
        private UITransitionProfile transitionProfile;

        private UISurfaceRuntimeOverride runtimeOverride =
            UISurfaceRuntimeOverride.None;

        private bool screenSuspended;
        private UIScreenSuspensionVisibility screenSuspensionVisibility =
            UIScreenSuspensionVisibility.Preserve;

        private bool modalInteractionBlocked;
        private bool hasModalBlocksRaycastsBaseline;
        private bool modalBlocksRaycastsBaseline;

        private bool hasRequestedVisibility;
        private bool requestedVisibility;

        private bool hasRequestedInteractability;
        private bool requestedInteractability;

        public string SurfaceId =>
            surfaceId == null
                ? string.Empty
                : surfaceId.Trim();

        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(displayLabel)
                ? SurfaceId
                : displayLabel.Trim();

        public UISurfaceRole Role =>
            role;

        public string NavigationScopeId =>
            navigationScopeId == null
                ? string.Empty
                : navigationScopeId.Trim();

        public bool StartVisible =>
            startVisible;

        public bool IsVisible =>
            gameObject.activeSelf;

        internal bool RequestedVisibility =>
            hasRequestedVisibility
                ? requestedVisibility
                : gameObject.activeSelf;

        internal bool RequestedInteractability
        {
            get
            {
                if (hasRequestedInteractability)
                {
                    return requestedInteractability;
                }

                CanvasGroup group =
                    ResolveInteractionGroup();

                return group == null ||
                    group.interactable;
            }
        }

        internal bool IsScreenSuspended =>
            screenSuspended;

        internal bool IsModalInteractionBlocked =>
            modalInteractionBlocked;

        public bool AllowExternalContext =>
            allowExternalContext;

        public IReadOnlyList<UISurfaceContextRule> ContextRules =>
            contextRules;

        public UISurfaceSelectionPolicy SelectionPolicy =>
            selectionPolicy;

        public UITransitionProfile TransitionProfile =>
            transitionProfile;

        internal CanvasGroup TransitionCanvasGroup =>
            ResolveInteractionGroup();

        public UISurfaceRuntimeOverride RuntimeOverride =>
            runtimeOverride;

        public bool IsInteractable
        {
            get
            {
                CanvasGroup group =
                    ResolveInteractionGroup();
                return group == null ||
                    group.interactable;
            }
        }

        internal void SetVisible(
            bool visible)
        {
            requestedVisibility =
                visible;
            hasRequestedVisibility =
                true;

            ApplyLifecycleVisibilityOverlay();
        }

        internal bool SetInteractable(
            bool interactable)
        {
            CanvasGroup group =
                ResolveInteractionGroup();
            if (group == null)
            {
                return false;
            }

            requestedInteractability =
                interactable;
            hasRequestedInteractability =
                true;

            ApplyLifecycleInteractionOverlay(
                group);
            return true;
        }

        internal void SetScreenSuspended(
            bool suspended,
            UIScreenSuspensionVisibility visibility)
        {
            CanvasGroup group =
                ResolveInteractionGroup();

            if (suspended &&
                !screenSuspended &&
                group != null &&
                !hasRequestedInteractability)
            {
                requestedInteractability =
                    group.interactable;
                hasRequestedInteractability =
                    true;
            }

            screenSuspended =
                suspended;
            screenSuspensionVisibility =
                visibility;

            ApplyLifecycleVisibilityOverlay();

            if (group != null)
            {
                ApplyLifecycleInteractionOverlay(
                    group);
            }
        }

        internal bool SetModalInteractionBlocked(
            bool blocked)
        {
            CanvasGroup group =
                ResolveInteractionGroup();

            if (group == null)
            {
                modalInteractionBlocked =
                    blocked;
                return false;
            }

            if (blocked &&
                !modalInteractionBlocked)
            {
                if (!hasRequestedInteractability)
                {
                    requestedInteractability =
                        group.interactable;
                    hasRequestedInteractability =
                        true;
                }

                modalBlocksRaycastsBaseline =
                    group.blocksRaycasts;
                hasModalBlocksRaycastsBaseline =
                    true;
            }

            modalInteractionBlocked =
                blocked;

            ApplyLifecycleInteractionOverlay(
                group);

            return true;
        }

        internal UISurfaceContextResponse ResolveContextResponse(
            UIContextState contextState)
        {
            return UISurfaceContextResolver.Resolve(
                allowExternalContext
                    ? contextRules
                    : null,
                allowExternalContext
                    ? contextState
                    : null,
                runtimeOverride);
        }

        internal void SetRuntimeOverride(
            UISurfaceRuntimeOverride value)
        {
            runtimeOverride = value;
        }

        internal void ClearRuntimeOverride()
        {
            runtimeOverride =
                UISurfaceRuntimeOverride.None;
        }

        private void ApplyLifecycleVisibilityOverlay()
        {
            bool baseVisibility =
                hasRequestedVisibility
                    ? requestedVisibility
                    : gameObject.activeSelf;

            bool effective =
                baseVisibility;

            if (screenSuspended)
            {
                switch (screenSuspensionVisibility)
                {
                    case UIScreenSuspensionVisibility.Hidden:
                        effective = false;
                        break;

                    case UIScreenSuspensionVisibility.Visible:
                        effective = true;
                        break;

                    case UIScreenSuspensionVisibility.Preserve:
                        effective = baseVisibility;
                        break;
                }
            }

            if (gameObject.activeSelf != effective)
            {
                gameObject.SetActive(
                    effective);
            }
        }

        private void ApplyLifecycleInteractionOverlay(
            CanvasGroup group)
        {
            bool baseInteractable =
                hasRequestedInteractability
                    ? requestedInteractability
                    : group.interactable;

            bool lifecycleBlocked =
                screenSuspended ||
                modalInteractionBlocked;

            group.interactable =
                lifecycleBlocked
                    ? false
                    : baseInteractable;

            if (modalInteractionBlocked)
            {
                group.blocksRaycasts =
                    false;
            }
            else if (hasModalBlocksRaycastsBaseline)
            {
                group.blocksRaycasts =
                    modalBlocksRaycastsBaseline;

                hasModalBlocksRaycastsBaseline =
                    false;
            }
        }

        private CanvasGroup ResolveInteractionGroup()
        {
            return interactionGroup != null
                ? interactionGroup
                : GetComponent<CanvasGroup>();
        }
    }
}
