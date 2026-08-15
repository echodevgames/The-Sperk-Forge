using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Per-surface input-aware selection/focus policy.
    /// </summary>
    [Serializable]
    public sealed class UISurfaceSelectionPolicy
    {
        [SerializeField]
        private UISelectionOpenBehavior pointerOpenBehavior =
            UISelectionOpenBehavior.ClearSelection;

        [SerializeField]
        private UISelectionOpenBehavior navigationOpenBehavior =
            UISelectionOpenBehavior.SelectDefault;

        [SerializeField]
        private GameObject defaultSelectionTarget;

        [SerializeField]
        private UIFocusReopenBehavior reopenBehavior =
            UIFocusReopenBehavior.Fresh;

        [SerializeField]
        private bool restoreFocusWhenReexposed = true;

        public UISurfaceSelectionPolicy()
        {
        }

        public UISurfaceSelectionPolicy(
            UISelectionOpenBehavior pointerOpenBehavior,
            UISelectionOpenBehavior navigationOpenBehavior,
            GameObject defaultSelectionTarget)
            : this(
                pointerOpenBehavior,
                navigationOpenBehavior,
                defaultSelectionTarget,
                UIFocusReopenBehavior.Fresh,
                true)
        {
        }

        public UISurfaceSelectionPolicy(
            UISelectionOpenBehavior pointerOpenBehavior,
            UISelectionOpenBehavior navigationOpenBehavior,
            GameObject defaultSelectionTarget,
            UIFocusReopenBehavior reopenBehavior,
            bool restoreFocusWhenReexposed)
        {
            this.pointerOpenBehavior =
                pointerOpenBehavior;

            this.navigationOpenBehavior =
                navigationOpenBehavior;

            this.defaultSelectionTarget =
                defaultSelectionTarget;

            this.reopenBehavior =
                reopenBehavior;

            this.restoreFocusWhenReexposed =
                restoreFocusWhenReexposed;
        }

        public UISelectionOpenBehavior PointerOpenBehavior =>
            pointerOpenBehavior;

        public UISelectionOpenBehavior NavigationOpenBehavior =>
            navigationOpenBehavior;

        public GameObject DefaultSelectionTarget =>
            defaultSelectionTarget;

        public UIFocusReopenBehavior ReopenBehavior =>
            reopenBehavior;

        public bool RestoreFocusWhenReexposed =>
            restoreFocusWhenReexposed;

        public UISelectionOpenBehavior GetOpenBehavior(
            UIInputModality modality) =>
            modality == UIInputModality.Navigation
                ? navigationOpenBehavior
                : pointerOpenBehavior;
    }
}
