using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Per-surface input-aware selection policy.
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

        public UISurfaceSelectionPolicy()
        {
        }

        public UISurfaceSelectionPolicy(
            UISelectionOpenBehavior pointerOpenBehavior,
            UISelectionOpenBehavior navigationOpenBehavior,
            GameObject defaultSelectionTarget)
        {
            this.pointerOpenBehavior = pointerOpenBehavior;
            this.navigationOpenBehavior = navigationOpenBehavior;
            this.defaultSelectionTarget = defaultSelectionTarget;
        }

        public UISelectionOpenBehavior PointerOpenBehavior =>
            pointerOpenBehavior;

        public UISelectionOpenBehavior NavigationOpenBehavior =>
            navigationOpenBehavior;

        public GameObject DefaultSelectionTarget =>
            defaultSelectionTarget;

        public UISelectionOpenBehavior GetOpenBehavior(
            UIInputModality modality) =>
            modality == UIInputModality.Navigation
                ? navigationOpenBehavior
                : pointerOpenBehavior;
    }
}
