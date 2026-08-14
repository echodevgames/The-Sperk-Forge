using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Scene/project-owned host for one stable Looking Glass layer.
    /// Looking Glass never assumes a fixed layer count or reserved layer name.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UILayerHost : MonoBehaviour
    {
        [SerializeField]
        private UILayerDefinition definition =
            new UILayerDefinition();

        [SerializeField]
        private Transform contentRoot;

        private UILayerDefinition runtimeDefinition;
        private Transform runtimeContentRoot;
        private bool runtimeSnapshotCaptured;

        public UILayerDefinition Definition =>
            runtimeSnapshotCaptured
                ? runtimeDefinition
                : definition;

        public UILayerId LayerId =>
            Definition == null
                ? new UILayerId(string.Empty)
                : Definition.LayerId;

        public int Order =>
            Definition == null
                ? 0
                : Definition.Order;

        public Transform ContentRoot =>
            runtimeSnapshotCaptured
                ? runtimeContentRoot
                : contentRoot != null
                    ? contentRoot
                    : transform;

        internal UILayerDefinition SnapshotDefinition() =>
            definition == null
                ? null
                : definition.Snapshot();

        internal void CaptureRuntimeSnapshot()
        {
            runtimeDefinition =
                SnapshotDefinition();

            runtimeContentRoot =
                contentRoot != null
                    ? contentRoot
                    : transform;

            runtimeSnapshotCaptured =
                true;
        }
    }
}
