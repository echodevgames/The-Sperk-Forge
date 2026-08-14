using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Sample-owned EUI-M1-02 proof console. It simulates external context and input
    /// modality only; it is not a production context provider or input detector.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LaboratoryUIContextDriver : MonoBehaviour
    {
        private const string MainMenuId = "main-menu";
        private const string SettingsId = "settings";
        private const string DefaultWindowId = "default-window";
        private const string FrontendScopeId = "frontend";
        private const string PauseContextId = "pause";
        private const string CinematicContextId = "cinematic";

        [SerializeField]
        private EchoUIRoot rootOverride;

        private EchoUIRoot root;
        private UISurface mainMenu;
        private UISurface settings;
        private UISurface defaultWindow;
        private GameObject settingsButton;
        private Vector2 scroll;

        private void Awake()
        {
            root = rootOverride != null
                ? rootOverride
                : GetComponent<EchoUIRoot>();

            UISurface[] surfaces =
                GetComponentsInChildren<UISurface>(true);
            for (int index = 0; index < surfaces.Length; index++)
            {
                UISurface surface = surfaces[index];
                switch (surface.SurfaceId)
                {
                    case MainMenuId:
                        mainMenu = surface;
                        break;
                    case SettingsId:
                        settings = surface;
                        break;
                    case DefaultWindowId:
                        defaultWindow = surface;
                        break;
                }
            }

            UnityEngine.UI.Button[] buttons =
                GetComponentsInChildren<UnityEngine.UI.Button>(true);
            for (int index = 0; index < buttons.Length; index++)
            {
                if (string.Equals(
                        buttons[index].gameObject.name,
                        "Button_Settings",
                        StringComparison.Ordinal))
                {
                    settingsButton = buttons[index].gameObject;
                    break;
                }
            }
        }

        private void OnGUI()
        {
            const float width = 430f;
            const float margin = 20f;
            float height = Mathf.Min(Screen.height - (margin * 2f), 760f);
            float left = Mathf.Max(
                margin,
                Screen.width - width - margin);

            // Foundry Laboratory convention: proof/debug consoles reserve the
            // top-right safe zone and grow downward in rows from there.
            GUILayout.BeginArea(
                new Rect(left, margin, width, height),
                "EUI-M1-02 LABORATORY-OWNED SIMULATION",
                GUI.skin.window);

            scroll = GUILayout.BeginScrollView(scroll);

            if (root == null)
            {
                GUILayout.Label("EchoUIRoot not found. The proof console cannot run.");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label("This console simulates external truth. Looking Glass does not own pause, cinematic, or input detection.");
            GUILayout.Space(8f);

            DrawState();
            GUILayout.Space(8f);

            GUILayout.Label("External contexts");
            if (GUILayout.Button(ContextButtonLabel(PauseContextId)))
            {
                ToggleContext(PauseContextId);
            }
            if (GUILayout.Button(ContextButtonLabel(CinematicContextId)))
            {
                ToggleContext(CinematicContextId);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Externally supplied input modality");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pointer"))
            {
                root.SetInputModality(UIInputModality.Pointer);
                ClearSelection();
            }
            if (GUILayout.Button("Navigation / Controller"))
            {
                root.SetInputModality(UIInputModality.Navigation);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("Surface operations");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Default Window"))
            {
                root.OpenSurface(DefaultWindowId);
            }
            if (GUILayout.Button("Close Default Window"))
            {
                root.CloseSurface(DefaultWindowId);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Navigate Settings"))
            {
                root.NavigateTo(SettingsId);
            }
            if (GUILayout.Button("Back: frontend"))
            {
                root.Back(FrontendScopeId);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle Default Window"))
            {
                root.ToggleSurface(DefaultWindowId);
            }
            if (GUILayout.Button("Prime Prior Selection: Button_Settings"))
            {
                PrimeSettingsSelection();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset Proof State"))
            {
                ResetProofState();
            }

            GUILayout.Space(10f);
            GUILayout.Label("Authored proof configuration");
            GUILayout.Label("• default-window order: cinematic first, pause second");
            GUILayout.Label("• cinematic controls visibility: Hidden");
            GUILayout.Label("• pause controls visibility: Visible + interaction: NonInteractable");
            GUILayout.Label("• settings carries a pause rule but Allow External Context is OFF");
            GUILayout.Label("• main-menu has no context rule");
            GUILayout.Label("• pointer opening clears selection");
            GUILayout.Label("• default-window navigation opening selects itself");
            GUILayout.Label("• settings navigation opening is explicitly unselected");

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawState()
        {
            GUILayout.Label($"Initialized: {root.IsInitialized}");
            GUILayout.Label($"Modality: {root.InputModality}");
            GUILayout.Label($"pause: {OnOff(root.IsContextActive(PauseContextId))}");
            GUILayout.Label($"cinematic: {OnOff(root.IsContextActive(CinematicContextId))}");
            GUILayout.Label($"Current frontend screen: {root.GetCurrentScreenId(FrontendScopeId)}");
            GUILayout.Label($"main-menu: {SurfaceState(mainMenu)}");
            GUILayout.Label($"settings: {SurfaceState(settings)} [external participation OFF]");
            GUILayout.Label($"default-window: {SurfaceState(defaultWindow)}");

            GameObject selected = UISelectionCoordinator.CurrentSelectedObject;
            GUILayout.Label($"EventSystem selected: {(selected != null ? selected.name : "<none>")}");
        }

        private string ContextButtonLabel(string id) =>
            $"Toggle {id} (currently {OnOff(root.IsContextActive(id))})";

        private static string OnOff(bool value) =>
            value ? "ON" : "OFF";

        private static string SurfaceState(UISurface surface)
        {
            if (surface == null)
            {
                return "<missing>";
            }

            return $"visible={surface.IsVisible}, interactable={surface.IsInteractable}";
        }

        private void ToggleContext(string id)
        {
            root.SetContextActive(
                id,
                !root.IsContextActive(id));
        }

        private void PrimeSettingsSelection()
        {
            EventSystem eventSystem = ResolveEventSystem();
            if (eventSystem != null &&
                settingsButton != null &&
                settingsButton.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(settingsButton);
            }
        }

        private void ResetProofState()
        {
            root.SetContextActive(PauseContextId, false);
            root.SetContextActive(CinematicContextId, false);
            root.SetInputModality(UIInputModality.Pointer);
            root.CloseSurface(DefaultWindowId);
            if (!string.Equals(
                    root.GetCurrentScreenId(FrontendScopeId),
                    MainMenuId,
                    StringComparison.Ordinal))
            {
                root.NavigateTo(MainMenuId);
            }
            ClearSelection();
        }

        private static void ClearSelection()
        {
            EventSystem eventSystem = ResolveEventSystem();
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(null);
            }
        }

        private static EventSystem ResolveEventSystem()
        {
            EventSystem current = EventSystem.current;
            if (current != null && current.isActiveAndEnabled)
            {
                return current;
            }

            return UnityEngine.Object.FindFirstObjectByType<EventSystem>();
        }
    }
}
