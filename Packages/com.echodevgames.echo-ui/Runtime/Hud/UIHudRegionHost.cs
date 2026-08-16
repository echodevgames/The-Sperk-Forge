using UnityEngine;

namespace EchoDevGames.EchoUI
{
    [DisallowMultipleComponent]
    public sealed class UIHudRegionHost : MonoBehaviour
    {
        [SerializeField]
        private UIHudRegionDefinition definition =
            new UIHudRegionDefinition();

        [SerializeField]
        private Transform contentRoot;

        private UIHudRegionDefinition runtimeDefinition;
        private Transform runtimeContentRoot;
        private bool snapshotCaptured;

        public UIHudRegionDefinition Definition =>
            snapshotCaptured ? runtimeDefinition : definition;

        public UIHudRegionId RegionId =>
            Definition == null
                ? new UIHudRegionId(string.Empty)
                : Definition.RegionId;

        public Transform ContentRoot =>
            snapshotCaptured
                ? runtimeContentRoot
                : contentRoot != null
                    ? contentRoot
                    : transform;

        public bool IsVisible => gameObject.activeSelf;

        internal void CaptureRuntimeSnapshot()
        {
            runtimeDefinition =
                definition == null
                    ? null
                    : definition.Snapshot();

            runtimeContentRoot =
                contentRoot != null
                    ? contentRoot
                    : transform;

            snapshotCaptured = true;
        }

        internal void SetVisible(bool visible)
        {
            if (gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }
    }
}
