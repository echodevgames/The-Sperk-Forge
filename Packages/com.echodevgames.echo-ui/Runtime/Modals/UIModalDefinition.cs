using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable-at-runtime project-authored blocking modal definition.
    /// </summary>
    [Serializable]
    public sealed class UIModalDefinition
    {
        [SerializeField]
        private string modalId = "modal";

        [SerializeField]
        private string targetLayerId = "modal";

        [SerializeField]
        private UIScreenOwnershipMode ownershipMode =
            UIScreenOwnershipMode.SceneOwned;

        [SerializeField]
        private GameObject rootOwnedPrefab;

        [SerializeField]
        private UISurface sceneOwnedView;

        [SerializeField]
        private UIModalBackPolicy backPolicy =
            new UIModalBackPolicy();

        [SerializeField]
        private UITransitionProfile transitionProfile;

        public UIModalDefinition()
        {
        }

        public UIModalDefinition(
            string modalId,
            string targetLayerId,
            UIScreenOwnershipMode ownershipMode,
            GameObject rootOwnedPrefab = null,
            UISurface sceneOwnedView = null,
            UIModalBackPolicy backPolicy = null,
            UITransitionProfile transitionProfile = null)
        {
            this.modalId = modalId ?? string.Empty;
            this.targetLayerId = targetLayerId ?? string.Empty;
            this.ownershipMode = ownershipMode;
            this.rootOwnedPrefab = rootOwnedPrefab;
            this.sceneOwnedView = sceneOwnedView;
            this.backPolicy =
                backPolicy == null
                    ? new UIModalBackPolicy()
                    : backPolicy;
            this.transitionProfile = transitionProfile == null ? null : transitionProfile.Snapshot();
        }

        public UIModalId ModalId =>
            new UIModalId(modalId);

        public UILayerId TargetLayerId =>
            new UILayerId(targetLayerId);

        public UIScreenOwnershipMode OwnershipMode =>
            ownershipMode;

        public GameObject RootOwnedPrefab =>
            rootOwnedPrefab;

        public UISurface SceneOwnedView =>
            sceneOwnedView;

        public UIModalBackPolicy BackPolicy =>
            backPolicy ?? new UIModalBackPolicy();

        public UITransitionProfile TransitionProfile =>
            transitionProfile;

        internal UIModalDefinition Snapshot() =>
            new UIModalDefinition(
                ModalId.Value,
                TargetLayerId.Value,
                ownershipMode,
                rootOwnedPrefab,
                sceneOwnedView,
                BackPolicy.Snapshot(),
                transitionProfile == null ? null : transitionProfile.Snapshot());
    }
}
