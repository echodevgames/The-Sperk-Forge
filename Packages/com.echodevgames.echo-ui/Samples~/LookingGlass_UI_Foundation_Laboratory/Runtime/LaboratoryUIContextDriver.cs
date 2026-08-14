using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Sample-owned Looking Glass proof console.
    /// M1 controls simulate external context/input truth.
    /// M2 controls exercise the authoritative Screen lifecycle using scene-authored
    /// layer hosts plus bounded sample-owned Screen definitions.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LaboratoryUIContextDriver : MonoBehaviour
    {
        private const string MainMenuId = "main-menu";
        private const string SettingsId = "settings";
        private const string DefaultWindowId = "default-window";
        private const string RootOwnedId = "lab-root-owned";
        private const string ExternalOwnedId = "lab-external-owned";
        private const string FrontendScopeId = "frontend";
        private const string PrimaryLayerId = "primary-ui";
        private const string FloatingLayerId = "floating-lab";
        private const string PauseContextId = "pause";
        private const string CinematicContextId = "cinematic";

        [SerializeField]
        private EchoUIRoot rootOverride;

        private readonly List<string> operationLog =
            new List<string>();

        private EchoUIRoot root;
        private UISurface mainMenu;
        private UISurface settings;
        private UISurface defaultWindow;
        private UISurface rootOwnedTemplate;
        private UISurface externalOwnedView;
        private GameObject settingsButton;
        private Vector2 scroll;
        private int selectedTab;
        private bool m2InitializationAttempted;
        private bool m2Ready;
        private bool externalRegistered;
        private string m2Message = "Waiting for Looking Glass surface initialization...";
        private string fifoObserved = "<not run>";

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

            UISurface[] allSceneSurfaces =
                Resources.FindObjectsOfTypeAll<UISurface>();
            for (int index = 0; index < allSceneSurfaces.Length; index++)
            {
                UISurface candidate = allSceneSurfaces[index];
                if (candidate == null ||
                    candidate.gameObject.scene != gameObject.scene)
                {
                    continue;
                }

                if (candidate.SurfaceId == RootOwnedId &&
                    !candidate.transform.IsChildOf(transform))
                {
                    rootOwnedTemplate = candidate;
                }
                else if (candidate.SurfaceId == ExternalOwnedId &&
                         !candidate.transform.IsChildOf(transform))
                {
                    externalOwnedView = candidate;
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

        private IEnumerator Start()
        {
            const int maxFrames = 120;
            int frames = 0;

            while (root != null &&
                   !root.IsInitialized &&
                   frames < maxFrames)
            {
                frames++;
                yield return null;
            }

            InitializeM2Proof();
        }

        private void OnGUI()
        {
            const float width = 470f;
            const float margin = 20f;
            float height = Mathf.Min(Screen.height - (margin * 2f), 820f);
            float left = Mathf.Max(
                margin,
                Screen.width - width - margin);

            GUILayout.BeginArea(
                new Rect(left, margin, width, height),
                "LOOKING GLASS LABORATORY-OWNED PROOF CONSOLE",
                GUI.skin.window);

            selectedTab = GUILayout.Toolbar(
                selectedTab,
                new[] { "M2 Screen Lifecycle", "M1 Retained Proof" });

            scroll = GUILayout.BeginScrollView(scroll);

            if (root == null)
            {
                GUILayout.Label("EchoUIRoot not found. The proof console cannot run.");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            if (selectedTab == 0)
            {
                DrawM2Console();
            }
            else
            {
                DrawM1Console();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawM2Console()
        {
            GUILayout.Label("EUI-M2-01: project-defined layers + authoritative Screen lifecycle + strict FIFO operations.");
            GUILayout.Label("This console is evidence tooling only; it does not become production UI architecture.");
            GUILayout.Space(8f);

            DrawM2State();
            GUILayout.Space(8f);

            if (!m2Ready)
            {
                GUILayout.Label("M2 proof is not ready: " + m2Message);
                if (!m2InitializationAttempted &&
                    GUILayout.Button("Initialize M2 Proof"))
                {
                    InitializeM2Proof();
                }
                return;
            }

            GUILayout.Label("SceneOwned lifecycle / suspension");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Push Settings"))
            {
                LogHandle("Push settings", root.PushScreen(SettingsId));
            }
            if (GUILayout.Button("Back: frontend"))
            {
                LogHandle("Back frontend", root.BackScreen(FrontendScopeId));
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("• main-menu suspension = Visible (must remain visible but non-interactable)");
            GUILayout.Label("• settings suspension = Hidden (must hide when another Screen is pushed over it)");

            GUILayout.Space(8f);
            GUILayout.Label("RootOwned / ExternalOwned");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Push RootOwned"))
            {
                LogHandle("Push RootOwned", root.PushScreen(RootOwnedId));
            }
            if (GUILayout.Button("Close RootOwned"))
            {
                LogHandle("Close RootOwned", root.CloseScreen(RootOwnedId, FrontendScopeId));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Push ExternalOwned"))
            {
                EnsureExternalRegistered();
                LogHandle("Push ExternalOwned", root.PushScreen(ExternalOwnedId));
            }
            if (GUILayout.Button("Close ExternalOwned"))
            {
                LogHandle("Close ExternalOwned", root.CloseScreen(ExternalOwnedId, FrontendScopeId));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("Replace / Reset");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Replace Top -> ExternalOwned"))
            {
                EnsureExternalRegistered();
                int before = root.GetScreenHistoryDepth(FrontendScopeId);
                UIScreenHandle handle = root.ReplaceScreen(ExternalOwnedId);
                int after = root.GetScreenHistoryDepth(FrontendScopeId);
                LogHandle("Replace ExternalOwned [depth " + before + " -> " + after + "]", handle);
            }
            if (GUILayout.Button("Reset -> Main Menu"))
            {
                LogHandle("Reset main-menu", root.ResetScreen(MainMenuId));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("Strict FIFO visible sequence");
            if (GUILayout.Button("Run Rapid FIFO: Settings -> RootOwned -> Back"))
            {
                RunRapidFifoProof();
            }
            GUILayout.Label("Expected: Push settings -> Push lab-root-owned -> Back frontend");
            GUILayout.Label("Observed: " + fifoObserved);

            GUILayout.Space(8f);
            if (GUILayout.Button("Reset Complete Laboratory Proof State"))
            {
                ResetProofState(clearOperationLog: true);
            }

            GUILayout.Space(10f);
            GUILayout.Label("Recent structural operation settlements");
            if (operationLog.Count == 0)
            {
                GUILayout.Label("<none>");
            }
            else
            {
                for (int index = 0; index < operationLog.Count; index++)
                {
                    GUILayout.Label(operationLog[index]);
                }
            }
        }

        private void DrawM2State()
        {
            GUILayout.Label("M2 lifecycle initialized: " + root.IsScreenLifecycleInitialized);
            GUILayout.Label("Proof readiness: " + (m2Ready ? "READY" : "NOT READY"));
            GUILayout.Label("Status: " + m2Message);
            GUILayout.Label("Resolved layers: " + ResolvedLayerSummary());
            GUILayout.Label("Current frontend Screen: " + root.GetCurrentScreenId(FrontendScopeId));
            GUILayout.Label("History depth: " + root.GetScreenHistoryDepth(FrontendScopeId));
            GUILayout.Label("Queue depth: " + root.ScreenOperationQueueDepth);
            GUILayout.Label("Current ownership: " + OwnershipLabel(root.GetCurrentScreenId(FrontendScopeId)));
            GUILayout.Label("main-menu: " + SurfaceState(mainMenu));
            GUILayout.Label("settings: " + SurfaceState(settings));
            GUILayout.Label("RootOwned template alive: " + YesNo(rootOwnedTemplate != null));
            GUILayout.Label("RootOwned runtime instance: " + RuntimeRootOwnedState());
            GUILayout.Label("ExternalOwned supplied object alive: " + YesNo(externalOwnedView != null));
            GUILayout.Label("ExternalOwned active: " + YesNo(externalOwnedView != null && externalOwnedView.gameObject.activeSelf));
            GUILayout.Label("ExternalOwned registered: " + YesNo(externalRegistered));
        }

        private void DrawM1Console()
        {
            GUILayout.Label("Retained M1 proof: external context truth, input-aware selection, independent Window, normal Back.");
            GUILayout.Space(8f);

            DrawM1State();
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

            if (GUILayout.Button("Reset Complete Laboratory Proof State"))
            {
                ResetProofState(clearOperationLog: true);
            }

            GUILayout.Space(10f);
            GUILayout.Label("Retained authored proof configuration");
            GUILayout.Label("• default-window order: cinematic first, pause second");
            GUILayout.Label("• cinematic controls visibility: Hidden");
            GUILayout.Label("• pause controls visibility: Visible + interaction: NonInteractable");
            GUILayout.Label("• settings carries a pause rule but Allow External Context is OFF");
            GUILayout.Label("• main-menu has no context rule");
            GUILayout.Label("• pointer opening clears selection");
            GUILayout.Label("• default-window navigation opening selects Button_DefaultClose");
            GUILayout.Label("• settings navigation opening is explicitly unselected");
        }

        private void DrawM1State()
        {
            GUILayout.Label("Initialized: " + root.IsInitialized);
            GUILayout.Label("Modality: " + root.InputModality);
            GUILayout.Label("pause: " + OnOff(root.IsContextActive(PauseContextId)));
            GUILayout.Label("cinematic: " + OnOff(root.IsContextActive(CinematicContextId)));
            GUILayout.Label("Current frontend screen: " + root.GetCurrentScreenId(FrontendScopeId));
            GUILayout.Label("main-menu: " + SurfaceState(mainMenu));
            GUILayout.Label("settings: " + SurfaceState(settings) + " [external participation OFF]");
            GUILayout.Label("default-window: " + SurfaceState(defaultWindow));

            GameObject selected = UISelectionCoordinator.CurrentSelectedObject;
            GUILayout.Label("EventSystem selected: " + (selected != null ? selected.name : "<none>"));
        }

        private void InitializeM2Proof()
        {
            if (m2InitializationAttempted || root == null)
            {
                return;
            }

            m2InitializationAttempted = true;

            if (!root.IsInitialized)
            {
                m2Message = "Surface foundation is not initialized.";
                return;
            }

            if (mainMenu == null ||
                settings == null ||
                rootOwnedTemplate == null ||
                externalOwnedView == null)
            {
                m2Message = "Required scene-authored proof surfaces/templates are missing.";
                return;
            }

            UILayerHost[] layerHosts =
                GetComponentsInChildren<UILayerHost>(true);

            List<UIScreenDefinition> definitions =
                new List<UIScreenDefinition>
                {
                    new UIScreenDefinition(
                        MainMenuId,
                        FrontendScopeId,
                        PrimaryLayerId,
                        UIScreenOwnershipMode.SceneOwned,
                        UIScreenSuspensionVisibility.Visible,
                        sceneOwnedView: mainMenu,
                        displayLabel: "Main Menu",
                        allowClose: false),
                    new UIScreenDefinition(
                        SettingsId,
                        FrontendScopeId,
                        PrimaryLayerId,
                        UIScreenOwnershipMode.SceneOwned,
                        UIScreenSuspensionVisibility.Hidden,
                        sceneOwnedView: settings,
                        displayLabel: "Settings",
                        allowClose: true),
                    new UIScreenDefinition(
                        RootOwnedId,
                        FrontendScopeId,
                        FloatingLayerId,
                        UIScreenOwnershipMode.RootOwned,
                        UIScreenSuspensionVisibility.Visible,
                        rootOwnedPrefab: rootOwnedTemplate.gameObject,
                        displayLabel: "Laboratory RootOwned",
                        allowClose: true),
                    new UIScreenDefinition(
                        ExternalOwnedId,
                        FrontendScopeId,
                        FloatingLayerId,
                        UIScreenOwnershipMode.ExternalOwned,
                        UIScreenSuspensionVisibility.Hidden,
                        displayLabel: "Laboratory ExternalOwned",
                        allowClose: true)
                };

            UISurfaceOperationResult result =
                root.InitializeScreenLifecycle(
                    layerHosts,
                    definitions,
                    null,
                    8);

            if (!result.Succeeded)
            {
                m2Message = "Lifecycle initialization failed: " + result.Message;
                return;
            }

            UISurfaceOperationResult externalResult =
                root.RegisterExternalScreenView(
                    ExternalOwnedId,
                    externalOwnedView);

            externalRegistered = externalResult.Succeeded;
            if (!externalRegistered)
            {
                m2Message = "ExternalOwned registration failed: " + externalResult.Message;
                return;
            }

            m2Ready = true;
            m2Message = "READY. Authored custom layers and all three ownership modes resolved.";
            ResetProofState(clearOperationLog: true);
        }

        private void EnsureExternalRegistered()
        {
            if (externalRegistered ||
                root == null ||
                externalOwnedView == null ||
                !root.IsScreenLifecycleInitialized)
            {
                return;
            }

            UISurfaceOperationResult result =
                root.RegisterExternalScreenView(
                    ExternalOwnedId,
                    externalOwnedView);

            externalRegistered = result.Succeeded;
            m2Message = externalRegistered
                ? "ExternalOwned supplied object registered."
                : "ExternalOwned registration failed: " + result.Message;
        }

        private void RunRapidFifoProof()
        {
            UIScreenHandle reset =
                root.ResetScreen(MainMenuId);
            LogHandle("FIFO setup reset", reset);

            UIScreenHandle first =
                root.PushScreen(SettingsId);
            UIScreenHandle second =
                root.PushScreen(RootOwnedId);
            UIScreenHandle third =
                root.BackScreen(FrontendScopeId);

            LogHandle("FIFO #1 Push settings", first);
            LogHandle("FIFO #2 Push RootOwned", second);
            LogHandle("FIFO #3 Back frontend", third);

            fifoObserved =
                HandleSettlement(first) +
                " -> " +
                HandleSettlement(second) +
                " -> " +
                HandleSettlement(third);
        }

        private void LogHandle(string label, UIScreenHandle handle)
        {
            string line =
                label + ": " +
                HandleSettlement(handle);

            operationLog.Insert(0, line);
            while (operationLog.Count > 10)
            {
                operationLog.RemoveAt(operationLog.Count - 1);
            }
        }

        private static string HandleSettlement(UIScreenHandle handle)
        {
            if (handle == null)
            {
                return "<no handle>";
            }

            string status = handle.IsCompleted
                ? handle.Result.Status.ToString()
                : "Pending";

            return "seq=" + handle.Request.Sequence +
                   " " + handle.Request.Kind +
                   " " + status;
        }

        private string ResolvedLayerSummary()
        {
            IReadOnlyList<UILayerHost> hosts =
                root.GetResolvedScreenLayerHosts();

            if (hosts == null || hosts.Count == 0)
            {
                return "<none>";
            }

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < hosts.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(" -> ");
                }

                UILayerHost host = hosts[index];
                builder.Append('#');
                builder.Append(index);
                builder.Append(' ');
                builder.Append(host.LayerId.Value);
                builder.Append(" (order ");
                builder.Append(host.Order);
                builder.Append(')');
            }

            return builder.ToString();
        }

        private string RuntimeRootOwnedState()
        {
            UISurface runtime = FindRuntimeRootOwnedSurface();
            return runtime == null
                ? "<released>"
                : SurfaceState(runtime);
        }

        private UISurface FindRuntimeRootOwnedSurface()
        {
            if (root == null)
            {
                return null;
            }

            UISurface[] candidates =
                Resources.FindObjectsOfTypeAll<UISurface>();

            for (int index = 0; index < candidates.Length; index++)
            {
                UISurface candidate = candidates[index];
                if (candidate == null ||
                    candidate == rootOwnedTemplate ||
                    candidate.gameObject.scene != gameObject.scene ||
                    candidate.SurfaceId != RootOwnedId)
                {
                    continue;
                }

                if (candidate.transform.IsChildOf(root.transform))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string OwnershipLabel(string screenId)
        {
            switch (screenId)
            {
                case MainMenuId:
                case SettingsId:
                    return UIScreenOwnershipMode.SceneOwned.ToString();
                case RootOwnedId:
                    return UIScreenOwnershipMode.RootOwned.ToString();
                case ExternalOwnedId:
                    return UIScreenOwnershipMode.ExternalOwned.ToString();
                default:
                    return "<none>";
            }
        }

        private string ContextButtonLabel(string id) =>
            "Toggle " + id + " (currently " + OnOff(root.IsContextActive(id)) + ")";

        private static string OnOff(bool value) =>
            value ? "ON" : "OFF";

        private static string YesNo(bool value) =>
            value ? "YES" : "NO";

        private static string SurfaceState(UISurface surface)
        {
            if (surface == null)
            {
                return "<missing>";
            }

            return "visible=" + surface.IsVisible +
                   ", interactable=" + surface.IsInteractable;
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

        private void ResetProofState(bool clearOperationLog)
        {
            root.SetContextActive(PauseContextId, false);
            root.SetContextActive(CinematicContextId, false);
            root.SetInputModality(UIInputModality.Pointer);
            root.CloseSurface(DefaultWindowId);

            if (root.IsScreenLifecycleInitialized)
            {
                root.ResetScreen(MainMenuId);
            }
            else if (!string.Equals(
                         root.GetCurrentScreenId(FrontendScopeId),
                         MainMenuId,
                         StringComparison.Ordinal))
            {
                root.NavigateTo(MainMenuId);
            }

            ClearSelection();
            fifoObserved = "<not run>";

            if (clearOperationLog)
            {
                operationLog.Clear();
            }
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
