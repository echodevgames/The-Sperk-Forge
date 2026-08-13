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

        internal void SetVisible(
            bool visible)
        {
            if (gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }
    }
}
