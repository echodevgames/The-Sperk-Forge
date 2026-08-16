using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Authored Screen definition. Runtime lifecycle state is stored separately.
    /// </summary>
    [Serializable]
    public sealed class UIScreenDefinition
    {
        [SerializeField]
        private string screenId = "screen";

        [SerializeField]
        private string displayLabel = string.Empty;

        [SerializeField]
        private string navigationScopeId = "default";

        [SerializeField]
        private string targetLayerId = "screen";

        [SerializeField]
        private UIScreenOwnershipMode ownershipMode =
            UIScreenOwnershipMode.SceneOwned;

        [SerializeField]
        private UIScreenSuspensionVisibility suspensionVisibility =
            UIScreenSuspensionVisibility.Hidden;

        [SerializeField]
        private bool allowClose = true;

        [SerializeField]
        private GameObject rootOwnedPrefab;

        [SerializeField]
        private UISurface sceneOwnedView;

        [SerializeField]
        private UITransitionProfile transitionProfile;

        public UIScreenDefinition()
        {
        }

        public UIScreenDefinition(
            string screenId,
            string navigationScopeId,
            string targetLayerId,
            UIScreenOwnershipMode ownershipMode,
            UIScreenSuspensionVisibility suspensionVisibility,
            GameObject rootOwnedPrefab = null,
            UISurface sceneOwnedView = null,
            string displayLabel = "",
            bool allowClose = true,
            UITransitionProfile transitionProfile = null)
        {
            this.screenId = screenId ?? string.Empty;
            this.navigationScopeId = navigationScopeId ?? string.Empty;
            this.targetLayerId = targetLayerId ?? string.Empty;
            this.ownershipMode = ownershipMode;
            this.suspensionVisibility = suspensionVisibility;
            this.allowClose = allowClose;
            this.rootOwnedPrefab = rootOwnedPrefab;
            this.sceneOwnedView = sceneOwnedView;
            this.displayLabel = displayLabel ?? string.Empty;
            this.transitionProfile = transitionProfile == null ? null : transitionProfile.Snapshot();
        }

        public string ScreenId =>
            screenId == null
                ? string.Empty
                : screenId.Trim();

        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(displayLabel)
                ? ScreenId
                : displayLabel.Trim();

        public string NavigationScopeId =>
            navigationScopeId == null
                ? string.Empty
                : navigationScopeId.Trim();

        public UILayerId TargetLayerId =>
            new UILayerId(targetLayerId);

        public UIScreenOwnershipMode OwnershipMode =>
            ownershipMode;

        public UIScreenSuspensionVisibility SuspensionVisibility =>
            suspensionVisibility;

        public bool AllowClose =>
            allowClose;

        public GameObject RootOwnedPrefab =>
            rootOwnedPrefab;

        public UISurface SceneOwnedView =>
            sceneOwnedView;

        public UITransitionProfile TransitionProfile =>
            transitionProfile;

        internal UIScreenDefinition Snapshot() =>
            new UIScreenDefinition(
                ScreenId,
                NavigationScopeId,
                TargetLayerId.Value,
                OwnershipMode,
                SuspensionVisibility,
                RootOwnedPrefab,
                SceneOwnedView,
                DisplayLabel,
                AllowClose,
                transitionProfile == null ? null : transitionProfile.Snapshot());
    }
}
