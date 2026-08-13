using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Thin uGUI Button adapter for Looking Glass navigation/surface requests.
    /// It does not own input maps or game-domain commands.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class UINavigationButton : MonoBehaviour
    {
        [SerializeField]
        private EchoUIRoot rootOverride;

        [SerializeField]
        private UINavigationAction action =
            UINavigationAction.NavigateTo;

        [SerializeField]
        private string targetSurfaceId =
            string.Empty;

        [SerializeField]
        private string targetScopeId =
            string.Empty;

        private Button button;

        private void Awake()
        {
            button =
                GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button == null)
            {
                button =
                    GetComponent<Button>();
            }

            button.onClick.AddListener(
                HandleClicked);
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(
                    HandleClicked);
            }
        }

        private void HandleClicked()
        {
            EchoUIRoot root =
                rootOverride != null
                    ? rootOverride
                    : EchoUIRoot.Active;

            if (root == null)
            {
                return;
            }

            switch (action)
            {
                case UINavigationAction.NavigateTo:
                    root.NavigateTo(
                        targetSurfaceId);
                    break;

                case UINavigationAction.Back:
                    root.Back(
                        targetScopeId);
                    break;

                case UINavigationAction.OpenSurface:
                    root.OpenSurface(
                        targetSurfaceId);
                    break;

                case UINavigationAction.CloseSurface:
                    root.CloseSurface(
                        targetSurfaceId);
                    break;

                case UINavigationAction.ToggleSurface:
                    root.ToggleSurface(
                        targetSurfaceId);
                    break;
            }
        }
    }
}
