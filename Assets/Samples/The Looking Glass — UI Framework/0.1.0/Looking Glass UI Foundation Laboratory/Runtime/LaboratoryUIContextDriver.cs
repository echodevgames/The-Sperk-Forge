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
    /// M2 controls exercise the authoritative Screen/Modal lifecycle using scene-authored
    /// definitions. M3 controls add sample-owned EventSystem/focus proof infrastructure.
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
        private const string SceneModalId = "lab-modal-confirm";
        private const string RootModalId = "lab-modal-root";
        private const string ExternalModalId = "lab-modal-external";
        private const string ConfirmResultId = "confirm";
        private const string CancelResultId = "cancel";

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
        private UISurface sceneModalView;
        private UISurface rootOwnedModalTemplate;
        private UISurface externalModalView;
        private GameObject settingsButton;
        private GameObject settingsBackButton;
        private GameObject defaultWindowCloseButton;
        private EventSystem sceneEventSystem;
        private GameObject m3ExtraEventSystemObject;
        private GameObject m3MainMenuAlternateTarget;
        private GameObject m3SettingsAlternateTarget;
        private GameObject m3WindowAlternateTarget;
        private Vector2 scroll;
        private int selectedTab;
        private bool m2InitializationAttempted;
        private bool m2Ready;
        private bool externalRegistered;
        private bool modalReady;
        private bool externalModalRegistered;
        private UIModalScreenMutationPolicy activeModalScreenPolicy =
            UIModalScreenMutationPolicy.Reject;
        private UIModalHandle sceneModalHandle;
        private UIModalHandle rootModalHandle;
        private UIModalHandle externalModalHandle;
        private UIScreenHandle modalScreenRequestA;
        private UIScreenHandle modalScreenRequestB;
        private int externalProjectActionCount;
        private string modalMessage =
            "Choose Reject or Defer initialization after M2 Screen lifecycle becomes READY.";
        private string modalAttemptSummary = "<none>";
        private string modalTerminalSummary = "<none>";
        private string deferredObserved = "<not run>";
        private string m2Message = "Waiting for Looking Glass surface initialization...";
        private string fifoObserved = "<not run>";
        private string m3Message =
            "M3 proof controls are sample-owned simulation infrastructure. Prepare a baseline, then run checks 1-12.";
        private string m3Observed = "<not run>";
        private string m3PerformanceEvidence = "<not run>";
        private bool m3PerformanceRunning;

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
                    case SceneModalId:
                        sceneModalView = surface;
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
                else if (candidate.SurfaceId == RootModalId &&
                         !candidate.transform.IsChildOf(transform))
                {
                    rootOwnedModalTemplate = candidate;
                }
                else if (candidate.SurfaceId == ExternalModalId &&
                         !candidate.transform.IsChildOf(transform))
                {
                    externalModalView = candidate;
                }
            }

            UnityEngine.UI.Button[] buttons =
                GetComponentsInChildren<UnityEngine.UI.Button>(true);
            for (int index = 0; index < buttons.Length; index++)
            {
                string buttonName =
                    buttons[index].gameObject.name;

                if (string.Equals(
                        buttonName,
                        "Button_Settings",
                        StringComparison.Ordinal) ||
                    string.Equals(
                        buttonName,
                        "Button_SettingsOpen",
                        StringComparison.Ordinal))
                {
                    settingsButton =
                        buttons[index].gameObject;
                }
                else if (string.Equals(
                             buttonName,
                             "Button_Back",
                             StringComparison.Ordinal))
                {
                    settingsBackButton =
                        buttons[index].gameObject;
                }
                else if (string.Equals(
                             buttonName,
                             "Button_DefaultClose",
                             StringComparison.Ordinal))
                {
                    defaultWindowCloseButton =
                        buttons[index].gameObject;
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

            sceneEventSystem =
                ResolveEventSystem();

            EnsureM3FocusTargets();

            m3Message =
                "M3 proof READY. Use Prepare M3 Baseline before a fresh acceptance run.";
        }

        private void OnGUI()
        {
            Color previousContentColor =
                GUI.contentColor;
            GUI.contentColor =
                new Color32(255, 45, 214, 255);

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
                new[]
                {
                    "M3-01 Focus",
                    "M2-02 Modals",
                    "M2-01 Screens",
                    "M1 Retained"
                });

            scroll = GUILayout.BeginScrollView(scroll);

            if (root == null)
            {
                GUILayout.Label("EchoUIRoot not found. The proof console cannot run.");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                GUI.contentColor =
                    previousContentColor;
                return;
            }

            if (selectedTab == 0)
            {
                DrawM301FocusConsole();
            }
            else if (selectedTab == 1)
            {
                DrawM202ModalConsole();
            }
            else if (selectedTab == 2)
            {
                DrawM2Console();
            }
            else
            {
                DrawM1Console();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUI.contentColor =
                previousContentColor;
        }


        private void DrawM301FocusConsole()
        {
            GUILayout.Label("EUI-M3-01: EventSystem coordination, focus memory/restoration, Modal focus containment, and explicit revalidation.");
            GUILayout.Label("These controls are Laboratory proof infrastructure only. Looking Glass still does not own project input actions, gameplay state, or device detection.");
            GUILayout.Space(8f);

            DrawM301State();
            GUILayout.Space(8f);

            if (GUILayout.Button("Prepare M3 Baseline"))
            {
                PrepareM3Baseline();
            }

            GUILayout.Space(10f);
            GUILayout.Label("1. AdoptAssigned");
            if (GUILayout.Button("Run Check 1: Adopt Assigned Scene EventSystem"))
            {
                RunM3Check1();
            }

            GUILayout.Space(8f);
            GUILayout.Label("2. Distinct AdoptExisting / CreateIfMissing / RequireExternal rules");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("2A AdoptExisting"))
            {
                RunM3Check2AdoptExisting();
            }
            if (GUILayout.Button("2B CreateIfMissing"))
            {
                RunM3Check2CreateIfMissing();
            }
            if (GUILayout.Button("2C RequireExternal"))
            {
                RunM3Check2RequireExternal();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("3. Multiple EventSystems degrade without deletion");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Run Check 3: Create Ambiguity"))
            {
                RunM3Check3();
            }
            if (GUILayout.Button("Cleanup / Restore One EventSystem"))
            {
                PrepareM3Baseline();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("4. Blocking Modal remembers/restores lower focus");
            if (GUILayout.Button("Run Check 4: Modal Restore"))
            {
                RunM3Check4();
            }

            GUILayout.Space(8f);
            GUILayout.Label("5. Screen Back restores prior Screen focus");
            if (GUILayout.Button("Run Check 5: Screen Back Restore"))
            {
                RunM3Check5();
            }

            GUILayout.Space(8f);
            GUILayout.Label("6. Fresh reopen ignores old session focus");
            if (GUILayout.Button("Run Check 6: Fresh Reopen"))
            {
                RunM3Check6();
            }

            GUILayout.Space(8f);
            GUILayout.Label("7. Remember-this-session reuses stable-surface focus");
            if (GUILayout.Button("Run Check 7: Session Reopen"))
            {
                RunM3Check7();
            }

            GUILayout.Space(8f);
            GUILayout.Label("8. Invalid remembered target falls through");
            if (GUILayout.Button("Run Check 8: Invalidate Remembered Target"))
            {
                RunM3Check8();
            }

            GUILayout.Space(8f);
            GUILayout.Label("9. Pointer policy may remain <none> without jitter");
            if (!m3PerformanceRunning &&
                GUILayout.Button("Run Check 9: Pointer <none> / 60-Frame Stability"))
            {
                StartCoroutine(
                    RunM3Check9());
            }

            GUILayout.Space(8f);
            GUILayout.Label("10. Navigation/controller policy establishes default");
            if (GUILayout.Button("Run Check 10: Navigation Default"))
            {
                RunM3Check10();
            }

            GUILayout.Space(8f);
            GUILayout.Label("11. Blocking Modal focus cannot escape lower UI");
            if (GUILayout.Button("Run Check 11: Modal Containment"))
            {
                RunM3Check11();
            }

            GUILayout.Space(8f);
            GUILayout.Label("12. Explicit revalidation repairs invalid dynamic focus + retained smoke");
            if (GUILayout.Button("Run Check 12: Revalidation + Smoke"))
            {
                RunM3Check12();
            }

            GUILayout.Space(12f);
            GUILayout.Label("Bounded performance evidence");
            if (!m3PerformanceRunning &&
                GUILayout.Button("Run 180-Frame Idle Focus Probe"))
            {
                StartCoroutine(
                    RunM3PerformanceProbe());
            }

            GUILayout.Label("Performance evidence: " + m3PerformanceEvidence);

            GUILayout.Space(12f);
            GUILayout.Label("Latest M3 observation");
            GUILayout.TextArea(
                m3Observed,
                GUILayout.MinHeight(90f));
        }

        private void DrawM301State()
        {
            EventSystem current =
                ResolveEventSystem();

            GUILayout.Label("Focus coordination: " + root.EventSystemCoordinationStatus);
            GUILayout.Label("Focus generation: " + root.FocusGeneration);
            GUILayout.Label("Active EventSystems: " + CountActiveEventSystems());
            GUILayout.Label("Resolved EventSystem: " + ObjectName(current));
            GUILayout.Label("Selected object: " + SelectedName());
            GUILayout.Label("Current frontend Screen: " + root.GetCurrentScreenId(FrontendScopeId));
            GUILayout.Label("Active blocking Modals: " + root.ActiveModalCount);
            GUILayout.Label(
                "default-window reopen policy: " +
                (defaultWindow != null && defaultWindow.SelectionPolicy != null
                    ? defaultWindow.SelectionPolicy.ReopenBehavior.ToString()
                    : "<missing>"));
            GUILayout.Label("Status: " + m3Message);
        }

        private void PrepareM3Baseline()
        {
            if (root == null ||
                !root.IsInitialized)
            {
                m3Message =
                    "Cannot prepare M3 baseline because EchoUIRoot is not initialized.";
                return;
            }

            if (!m2Ready)
            {
                InitializeM2Proof();
            }

            if (modalReady)
            {
                ResetModalProofState();
            }

            root.SetContextActive(
                PauseContextId,
                false);

            root.SetContextActive(
                CinematicContextId,
                false);

            CleanupM3ExtraEventSystems();
            EnsureSceneEventSystemActive();

            UISurfaceOperationResult coordination =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            root.SetInputModality(
                UIInputModality.Pointer);

            root.CloseSurface(
                DefaultWindowId);

            if (root.IsScreenLifecycleInitialized)
            {
                root.ResetScreen(
                    MainMenuId);
            }

            EnsureM3FocusTargets();
            ClearSelection();

            m3Message =
                coordination.Succeeded
                    ? "M3 baseline READY with one adopted scene EventSystem."
                    : "M3 baseline coordination failed: " + coordination.Message;

            m3Observed =
                "Baseline: status=" + root.EventSystemCoordinationStatus +
                ", EventSystems=" + CountActiveEventSystems() +
                ", selected=" + SelectedName();
        }

        private void RunM3Check1()
        {
            PrepareM3Baseline();

            UISurfaceOperationResult result =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.AdoptAssigned,
                    sceneEventSystem);

            m3Observed =
                "CHECK 1 expected Ready + assigned scene EventSystem. Observed: " +
                "status=" + root.EventSystemCoordinationStatus +
                ", assignedAlive=" + YesNo(sceneEventSystem != null && sceneEventSystem.isActiveAndEnabled) +
                ", EventSystems=" + CountActiveEventSystems() +
                ", message=" + result.Message;
        }

        private void RunM3Check2AdoptExisting()
        {
            PrepareM3Baseline();

            UISurfaceOperationResult result =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            m3Observed =
                "CHECK 2A expected Ready by adopting the one existing scene EventSystem. Observed: " +
                "status=" + root.EventSystemCoordinationStatus +
                ", EventSystems=" + CountActiveEventSystems() +
                ", message=" + result.Message;
        }

        private void RunM3Check2CreateIfMissing()
        {
            PrepareM3Baseline();

            if (sceneEventSystem != null)
            {
                sceneEventSystem.gameObject.SetActive(false);
            }

            CleanupRootCreatedEventSystem();

            UISurfaceOperationResult result =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.CreateIfMissing,
                    null);

            EventSystem created =
                FindRootCreatedEventSystem();

            m3Observed =
                "CHECK 2B expected Ready + one Looking Glass-created EventSystem because none existed. Observed: " +
                "status=" + root.EventSystemCoordinationStatus +
                ", created=" + ObjectName(created) +
                ", activeEventSystems=" + CountActiveEventSystems() +
                ", sceneEventSystemActive=" + YesNo(sceneEventSystem != null && sceneEventSystem.isActiveAndEnabled) +
                ", message=" + result.Message +
                ". Click Prepare M3 Baseline before the next check.";
        }

        private void RunM3Check2RequireExternal()
        {
            PrepareM3Baseline();

            if (sceneEventSystem != null)
            {
                sceneEventSystem.gameObject.SetActive(false);
            }

            CleanupRootCreatedEventSystem();

            UISurfaceOperationResult result =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.RequireExternal,
                    null);

            m3Observed =
                "CHECK 2C expected Missing/degraded and zero created EventSystems. Observed: " +
                "status=" + root.EventSystemCoordinationStatus +
                ", activeEventSystems=" + CountActiveEventSystems() +
                ", created=" + ObjectName(FindRootCreatedEventSystem()) +
                ", operationSucceeded=" + result.Succeeded +
                ", message=" + result.Message +
                ". Click Prepare M3 Baseline before the next check.";
        }

        private void RunM3Check3()
        {
            PrepareM3Baseline();

            m3ExtraEventSystemObject =
                new GameObject(
                    "M3 Laboratory Extra EventSystem");

            m3ExtraEventSystemObject.AddComponent<EventSystem>();

            UISurfaceOperationResult result =
                root.InitializeFocusLifecycle(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            m3Observed =
                "CHECK 3 expected Ambiguous/degraded, both EventSystems still alive, and no arbitrary adoption. Observed: " +
                "status=" + root.EventSystemCoordinationStatus +
                ", activeEventSystems=" + CountActiveEventSystems() +
                ", sceneAlive=" + YesNo(sceneEventSystem != null) +
                ", extraAlive=" + YesNo(m3ExtraEventSystemObject != null) +
                ", operationSucceeded=" + result.Succeeded +
                ", message=" + result.Message;
        }

        private void RunM3Check4()
        {
            PrepareM3Baseline();
            EnsureM3ModalReady();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.ResetScreen(
                MainMenuId);

            UIFocusRequestResult prime =
                root.RequestFocus(
                    MainMenuId,
                    m3MainMenuAlternateTarget,
                    root.FocusGeneration);

            string before =
                SelectedName();

            sceneModalHandle =
                root.OpenModal(
                    SceneModalId);

            root.RequestFocus(
                SceneModalId,
                sceneModalView != null
                    ? sceneModalView.gameObject
                    : null,
                root.FocusGeneration);

            string during =
                SelectedName();

            UIModalCompletionAttemptResult completion =
                root.CompleteModal(
                    sceneModalHandle,
                    ConfirmResultId);

            string after =
                SelectedName();

            m3Observed =
                "CHECK 4 expected lower focus remembered, Modal owns focus while open, then lower focus restored. " +
                "Observed before=" + before +
                ", during=" + during +
                ", after=" + after +
                ", prime=" + prime.Status +
                ", completion=" + completion.Status +
                ", expectedAfter=" + ObjectName(m3MainMenuAlternateTarget);
        }

        private void RunM3Check5()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.ResetScreen(
                MainMenuId);

            root.RequestFocus(
                MainMenuId,
                m3MainMenuAlternateTarget,
                root.FocusGeneration);

            string before =
                SelectedName();

            root.PushScreen(
                SettingsId);

            string settingsSelection =
                SelectedName();

            root.BackScreen(
                FrontendScopeId);

            string after =
                SelectedName();

            m3Observed =
                "CHECK 5 expected Back to expose main-menu and restore its remembered target. " +
                "Observed before=" + before +
                ", settings=" + settingsSelection +
                ", currentScreen=" + root.GetCurrentScreenId(FrontendScopeId) +
                ", after=" + after +
                ", expectedAfter=" + ObjectName(m3MainMenuAlternateTarget);
        }

        private void RunM3Check6()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.PushScreen(
                SettingsId);

            root.RequestFocus(
                SettingsId,
                m3SettingsAlternateTarget,
                root.FocusGeneration);

            string primed =
                SelectedName();

            root.BackScreen(
                FrontendScopeId);

            root.PushScreen(
                SettingsId);

            string reopened =
                SelectedName();

            m3Observed =
                "CHECK 6 expected Fresh reopen to ignore the old alternate target and use Settings' authored opening policy. " +
                "Observed primed=" + primed +
                ", reopened=" + reopened +
                ", oldAlternate=" + ObjectName(m3SettingsAlternateTarget) +
                ", ignoredOldMemory=" + YesNo(
                    !string.Equals(
                        reopened,
                        ObjectName(m3SettingsAlternateTarget),
                        StringComparison.Ordinal));
        }

        private void RunM3Check7()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.OpenSurface(
                DefaultWindowId);

            root.RequestFocus(
                DefaultWindowId,
                m3WindowAlternateTarget,
                root.FocusGeneration);

            string primed =
                SelectedName();

            root.CloseSurface(
                DefaultWindowId);

            root.OpenSurface(
                DefaultWindowId);

            string reopened =
                SelectedName();

            bool restored =
                string.Equals(
                    reopened,
                    ObjectName(m3WindowAlternateTarget),
                    StringComparison.Ordinal);

            m3Observed =
                "CHECK 7 " + (restored ? "PASS" : "FAIL") +
                " expected RememberThisSession to restore the alternate stable-surface target. " +
                "Observed policy=" +
                (defaultWindow != null && defaultWindow.SelectionPolicy != null
                    ? defaultWindow.SelectionPolicy.ReopenBehavior.ToString()
                    : "<missing>") +
                ", primed=" + primed +
                ", reopened=" + reopened +
                ", expected=" + ObjectName(m3WindowAlternateTarget);
        }

        private void RunM3Check8()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.OpenSurface(
                DefaultWindowId);

            root.RequestFocus(
                DefaultWindowId,
                m3WindowAlternateTarget,
                root.FocusGeneration);

            root.CloseSurface(
                DefaultWindowId);

            if (m3WindowAlternateTarget != null)
            {
                m3WindowAlternateTarget.SetActive(false);
            }

            root.OpenSurface(
                DefaultWindowId);

            string reopened =
                SelectedName();

            if (m3WindowAlternateTarget != null)
            {
                m3WindowAlternateTarget.SetActive(true);
            }

            m3Observed =
                "CHECK 8 expected invalid remembered target to fall through to Button_DefaultClose or legal <none>. " +
                "Observed reopened=" + reopened +
                ", authoredDefault=" + ObjectName(defaultWindowCloseButton);
        }

        private IEnumerator RunM3Check9()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Pointer);

            root.CloseSurface(
                DefaultWindowId);

            root.OpenSurface(
                DefaultWindowId);

            string initial =
                SelectedName();

            long generationBefore =
                root.FocusGeneration;

            m3PerformanceRunning = true;

            for (int frame = 0;
                 frame < 60;
                 frame++)
            {
                yield return null;
            }

            long generationAfter =
                root.FocusGeneration;

            string after =
                SelectedName();

            m3PerformanceRunning = false;

            m3Observed =
                "CHECK 9 expected pointer-opened default-window to remain <none> without idle focus jitter. " +
                "Observed initial=" + initial +
                ", after60Frames=" + after +
                ", generation=" + generationBefore + " -> " + generationAfter +
                ", stable=" + YesNo(
                    string.Equals(initial, after, StringComparison.Ordinal) &&
                    generationBefore == generationAfter);
        }

        private void RunM3Check10()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.CloseSurface(
                DefaultWindowId);

            root.OpenSurface(
                DefaultWindowId);

            m3Observed =
                "CHECK 10 expected Navigation/controller policy to select Button_DefaultClose. " +
                "Observed selected=" + SelectedName() +
                ", expected=" + ObjectName(defaultWindowCloseButton);
        }

        private void RunM3Check11()
        {
            PrepareM3Baseline();
            EnsureM3ModalReady();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.ResetScreen(
                MainMenuId);

            root.RequestFocus(
                MainMenuId,
                m3MainMenuAlternateTarget,
                root.FocusGeneration);

            sceneModalHandle =
                root.OpenModal(
                    SceneModalId);

            root.RequestFocus(
                SceneModalId,
                sceneModalView != null
                    ? sceneModalView.gameObject
                    : null,
                root.FocusGeneration);

            string legalModalFocus =
                SelectedName();

            EventSystem eventSystem =
                ResolveEventSystem();

            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(
                    m3MainMenuAlternateTarget);
            }

            string forcedIllegal =
                SelectedName();

            UIFocusRequestResult revalidation =
                root.RevalidateFocus(
                    root.FocusGeneration);

            string repaired =
                SelectedName();

            m3Observed =
                "CHECK 11 expected forced lower-UI focus to be repaired back inside the top Modal or to legal <none>. " +
                "Observed legalModalFocus=" + legalModalFocus +
                ", forcedLower=" + forcedIllegal +
                ", repaired=" + repaired +
                ", revalidation=" + revalidation.Status +
                ", escapedLowerAfterRepair=" + YesNo(
                    string.Equals(
                        repaired,
                        ObjectName(m3MainMenuAlternateTarget),
                        StringComparison.Ordinal));
        }

        private void RunM3Check12()
        {
            PrepareM3Baseline();
            EnsureM3ModalReady();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.OpenSurface(
                DefaultWindowId);

            root.RequestFocus(
                DefaultWindowId,
                m3WindowAlternateTarget,
                root.FocusGeneration);

            if (m3WindowAlternateTarget != null)
            {
                m3WindowAlternateTarget.SetActive(false);
            }

            UIFocusRequestResult repair =
                root.RevalidateFocus(
                    root.FocusGeneration);

            string repaired =
                SelectedName();

            if (m3WindowAlternateTarget != null)
            {
                m3WindowAlternateTarget.SetActive(true);
            }

            root.CloseSurface(
                DefaultWindowId);

            UIScreenHandle push =
                root.PushScreen(
                    SettingsId);

            UIScreenHandle back =
                root.BackScreen(
                    FrontendScopeId);

            root.SetContextActive(
                PauseContextId,
                true);

            bool pauseObserved =
                root.IsContextActive(
                    PauseContextId);

            root.SetContextActive(
                PauseContextId,
                false);

            UIModalHandle modal =
                root.OpenModal(
                    SceneModalId);

            UIModalCompletionAttemptResult first =
                root.CompleteModal(
                    modal,
                    ConfirmResultId);

            UIModalCompletionAttemptResult second =
                root.CompleteModal(
                    modal,
                    ConfirmResultId);

            bool smoke =
                push != null &&
                push.Accepted &&
                back != null &&
                back.Accepted &&
                pauseObserved &&
                first.Status ==
                    UIModalCompletionStatus.Succeeded &&
                second.Status !=
                    UIModalCompletionStatus.Succeeded &&
                string.Equals(
                    root.GetCurrentScreenId(
                        FrontendScopeId),
                    MainMenuId,
                    StringComparison.Ordinal);

            m3Observed =
                "CHECK 12 retainedSmoke=" + (smoke ? "PASS" : "FAIL") +
                ", repairedFocus=" + repaired +
                ", expectedFallback=" + ObjectName(defaultWindowCloseButton) +
                ", revalidation=" + repair.Status +
                ", finalScreen=" + root.GetCurrentScreenId(FrontendScopeId) +
                ". Details: pushAccepted=" + (push != null && push.Accepted) +
                ", backAccepted=" + (back != null && back.Accepted) +
                ", pauseObserved=" + pauseObserved +
                ", firstModalCompletion=" + first.Status +
                ", secondModalCompletion=" + second.Status;
        }

        private IEnumerator RunM3PerformanceProbe()
        {
            PrepareM3Baseline();

            root.SetInputModality(
                UIInputModality.Navigation);

            root.OpenSurface(
                DefaultWindowId);

            long before =
                root.FocusGeneration;

            int startFrame =
                Time.frameCount;

            float startTime =
                Time.realtimeSinceStartup;

            m3PerformanceRunning = true;

            for (int frame = 0;
                 frame < 180;
                 frame++)
            {
                yield return null;
            }

            long after =
                root.FocusGeneration;

            float elapsed =
                Time.realtimeSinceStartup -
                startTime;

            float revalidationStart =
                Time.realtimeSinceStartup;

            UIFocusRequestResult explicitProbe =
                root.RevalidateFocus(
                    root.FocusGeneration);

            float revalidationMilliseconds =
                (Time.realtimeSinceStartup -
                 revalidationStart) *
                1000f;

            m3PerformanceRunning = false;

            m3PerformanceEvidence =
                "Idle " + (Time.frameCount - startFrame) +
                " frames / " + elapsed.ToString("0.000") +
                "s: focus generation " + before + " -> " + after +
                " (" + (before == after ? "STABLE" : "CHANGED") + "). " +
                "Explicit revalidation=" + explicitProbe.Status +
                ", synchronous sample timing=" +
                revalidationMilliseconds.ToString("0.###") + " ms. " +
                "Together with the focused no-Update/LateUpdate automated test, this is the bounded Laboratory evidence for event-driven idle behavior.";
        }

        private void EnsureM3ModalReady()
        {
            if (!modalReady)
            {
                InitializeModalProof(
                    UIModalScreenMutationPolicy.Reject);
            }
            else
            {
                ResetModalProofState();
            }
        }

        private void EnsureM3FocusTargets()
        {
            m3MainMenuAlternateTarget =
                EnsureFocusTarget(
                    mainMenu,
                    m3MainMenuAlternateTarget,
                    "M3_MainMenu_RememberedTarget");

            m3SettingsAlternateTarget =
                EnsureFocusTarget(
                    settings,
                    m3SettingsAlternateTarget,
                    "M3_Settings_FreshTarget");

            m3WindowAlternateTarget =
                EnsureFocusTarget(
                    defaultWindow,
                    m3WindowAlternateTarget,
                    "M3_DefaultWindow_SessionTarget");
        }

        private static GameObject EnsureFocusTarget(
            UISurface surface,
            GameObject existing,
            string name)
        {
            if (existing != null)
            {
                return existing;
            }

            if (surface == null)
            {
                return null;
            }

            Transform child =
                surface.transform.Find(
                    name);

            if (child != null)
            {
                child.gameObject.SetActive(true);
                return child.gameObject;
            }

            GameObject target =
                new GameObject(
                    name);

            target.transform.SetParent(
                surface.transform,
                false);

            return target;
        }

        private void CleanupM3ExtraEventSystems()
        {
            if (m3ExtraEventSystemObject != null)
            {
                m3ExtraEventSystemObject.SetActive(false);
                Destroy(
                    m3ExtraEventSystemObject);

                m3ExtraEventSystemObject = null;
            }

            CleanupRootCreatedEventSystem();
        }

        private void CleanupRootCreatedEventSystem()
        {
            EventSystem created =
                FindRootCreatedEventSystem();

            if (created == null)
            {
                return;
            }

            created.gameObject.SetActive(false);
            Destroy(
                created.gameObject);
        }

        private EventSystem FindRootCreatedEventSystem()
        {
            EventSystem[] systems =
                UnityEngine.Object.FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.InstanceID);

            for (int index = 0;
                 index < systems.Length;
                 index++)
            {
                EventSystem candidate =
                    systems[index];

                if (candidate == null ||
                    !candidate.isActiveAndEnabled ||
                    !string.Equals(
                        candidate.gameObject.name,
                        "EchoUI EventSystem",
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (root == null ||
                    candidate.transform.IsChildOf(
                        root.transform))
                {
                    return candidate;
                }
            }

            return null;
        }

        private void EnsureSceneEventSystemActive()
        {
            if (sceneEventSystem == null)
            {
                EventSystem[] systems =
                    UnityEngine.Object.FindObjectsByType<EventSystem>(
                        FindObjectsInactive.Include,
                        FindObjectsSortMode.InstanceID);

                for (int index = 0;
                     index < systems.Length;
                     index++)
                {
                    if (systems[index] == null ||
                        string.Equals(
                            systems[index].gameObject.name,
                            "EchoUI EventSystem",
                            StringComparison.Ordinal) ||
                        string.Equals(
                            systems[index].gameObject.name,
                            "M3 Laboratory Extra EventSystem",
                            StringComparison.Ordinal))
                    {
                        continue;
                    }

                    sceneEventSystem =
                        systems[index];

                    break;
                }
            }

            if (sceneEventSystem != null)
            {
                sceneEventSystem.gameObject.SetActive(true);
            }
        }

        private static int CountActiveEventSystems()
        {
            return UnityEngine.Object.FindObjectsByType<EventSystem>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.InstanceID).Length;
        }

        private static string SelectedName()
        {
            EventSystem eventSystem =
                ResolveEventSystem();

            return eventSystem != null &&
                   eventSystem.currentSelectedGameObject != null
                ? eventSystem.currentSelectedGameObject.name
                : "<none>";
        }

        private static string ObjectName(
            UnityEngine.Object value)
        {
            return value != null
                ? value.name
                : "<none>";
        }

        private void DrawM202ModalConsole()
        {
            GUILayout.Label("EUI-M2-02: blocking Modal lifecycle, exact-once results, UI-scoped blocking, and explicit Screen mutation policy.");
            GUILayout.Label("Independent Windows and gameplay input remain outside blocking-Modal ownership.");
            GUILayout.Space(8f);

            DrawM202State();
            GUILayout.Space(8f);

            if (!m2Ready)
            {
                GUILayout.Label("M2-01 Screen lifecycle must be READY first: " + m2Message);
                return;
            }

            if (!modalReady)
            {
                GUILayout.Label("Initialize one policy per Play Mode session.");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Initialize: Reject"))
                {
                    InitializeModalProof(
                        UIModalScreenMutationPolicy.Reject);
                }
                if (GUILayout.Button("Initialize: Defer"))
                {
                    InitializeModalProof(
                        UIModalScreenMutationPolicy.DeferUntilModalStackClears);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Use Reject for checks 1-9/11-12. Restart Play Mode and choose Defer for check 10.");
                return;
            }

            GUILayout.Label("Open / complete / exact once");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Scene Confirm"))
            {
                sceneModalHandle =
                    root.OpenModal(
                        SceneModalId);
                TrackModalHandle(
                    "Open Scene Confirm",
                    sceneModalHandle);
            }
            if (GUILayout.Button("Complete: confirm"))
            {
                CompleteTrackedModal(
                    sceneModalHandle,
                    ConfirmResultId,
                    "Complete scene -> confirm");
            }
            if (GUILayout.Button("Complete Again: cancel"))
            {
                CompleteTrackedModal(
                    sceneModalHandle,
                    CancelResultId,
                    "Repeat scene -> cancel");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Nested top-only / out-of-order lower cleanup");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Root Modal (Back Disabled)"))
            {
                rootModalHandle =
                    root.OpenModal(
                        RootModalId);
                TrackModalHandle(
                    "Open Root Modal",
                    rootModalHandle);
            }
            if (GUILayout.Button("Abort Lower Scene Handle"))
            {
                AbortTrackedModal(
                    sceneModalHandle,
                    UIModalAbortReason.ExplicitAbort,
                    "Abort lower scene");
            }
            if (GUILayout.Button("Complete Root: confirm"))
            {
                CompleteTrackedModal(
                    rootModalHandle,
                    ConfirmResultId,
                    "Complete root -> confirm");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Back policy");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Back on Top Modal"))
            {
                UIModalCompletionAttemptResult result =
                    root.HandleModalBack();
                modalAttemptSummary =
                    "Back: " +
                    result.Status +
                    " | " +
                    result.Message;
                CaptureCompletedHandleSummaries();
            }
            if (GUILayout.Button("Open Dismissible Scene"))
            {
                sceneModalHandle =
                    root.OpenModal(
                        SceneModalId);
                TrackModalHandle(
                    "Open dismissible Scene",
                    sceneModalHandle);
            }
            if (GUILayout.Button("Open Non-dismissible Root"))
            {
                rootModalHandle =
                    root.OpenModal(
                        RootModalId);
                TrackModalHandle(
                    "Open non-dismissible Root",
                    rootModalHandle);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Structural abort / ExternalOwned");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open External Modal"))
            {
                EnsureExternalModalRegistered();
                externalModalHandle =
                    root.OpenModal(
                        ExternalModalId);
                TrackModalHandle(
                    "Open External Modal",
                    externalModalHandle);
            }
            if (GUILayout.Button("Simulate External Owner Loss"))
            {
                UISurfaceOperationResult result =
                    root.UnregisterExternalModalView(
                        ExternalModalId);
                externalModalRegistered = false;
                modalAttemptSummary =
                    "Owner loss: " +
                    result.Status +
                    " | " +
                    result.Message;
                CaptureCompletedHandleSummaries();
            }
            if (GUILayout.Button("Complete External: confirm"))
            {
                CompleteTrackedModal(
                    externalModalHandle,
                    ConfirmResultId,
                    "Complete external -> confirm");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Screen mutation while Modal is active");
            if (activeModalScreenPolicy ==
                UIModalScreenMutationPolicy.Reject)
            {
                if (GUILayout.Button("Request Push Settings (expect BlockedByModal)"))
                {
                    modalScreenRequestA =
                        root.PushScreen(
                            SettingsId);
                    deferredObserved =
                        ScreenHandleSettlement(
                            modalScreenRequestA);
                }
            }
            else
            {
                if (GUILayout.Button("Queue Deferred: Settings -> RootOwned Screen"))
                {
                    modalScreenRequestA =
                        root.PushScreen(
                            SettingsId);
                    modalScreenRequestB =
                        root.PushScreen(
                            RootOwnedId);

                    deferredObserved =
                        "Before settle: " +
                        ScreenHandleSettlement(
                            modalScreenRequestA) +
                        " -> " +
                        ScreenHandleSettlement(
                            modalScreenRequestB);
                }
            }
            GUILayout.Label("Screen mutation observed: " + deferredObserved);

            GUILayout.Space(6f);
            GUILayout.Label("Project/gameplay separation simulator");
            if (GUILayout.Button("Trigger External Project Action (+1)"))
            {
                externalProjectActionCount++;
                modalMessage =
                    "External project action executed. Looking Glass did not own/freeze it.";
            }
            GUILayout.Label("External project action count: " + externalProjectActionCount);

            GUILayout.Space(8f);
            if (GUILayout.Button("Reset M2-02 Proof State"))
            {
                ResetModalProofState();
            }

            GUILayout.Space(10f);
            GUILayout.Label("Latest Modal attempt");
            GUILayout.Label(modalAttemptSummary);
            GUILayout.Label("Latest terminal result");
            GUILayout.Label(modalTerminalSummary);
        }

        private void DrawM202State()
        {
            GUILayout.Label("M2 Screen lifecycle ready: " + m2Ready);
            GUILayout.Label("Modal lifecycle initialized: " + root.IsModalLifecycleInitialized);
            GUILayout.Label("Modal proof readiness: " + (modalReady ? "READY" : "NOT READY"));
            GUILayout.Label("Modal policy: " + (modalReady ? activeModalScreenPolicy.ToString() : "<not selected>"));
            GUILayout.Label("Status: " + modalMessage);
            GUILayout.Label("Active Modal count: " + root.ActiveModalCount);
            GUILayout.Label("Top Modal: " + (string.IsNullOrWhiteSpace(root.TopModalId) ? "<none>" : root.TopModalId));
            GUILayout.Label("Deferred Screen queue depth: " + root.DeferredScreenOperationQueueDepth);
            GUILayout.Label("Current frontend Screen: " + root.GetCurrentScreenId(FrontendScopeId));
            GUILayout.Label("History depth: " + root.GetScreenHistoryDepth(FrontendScopeId));
            GUILayout.Label("main-menu: " + SurfaceStateWithRaycast(mainMenu));
            GUILayout.Label("default-window: " + SurfaceStateWithRaycast(defaultWindow));
            GUILayout.Label("SceneOwned Modal: " + SurfaceStateWithRaycast(sceneModalView));
            GUILayout.Label("RootOwned Modal template alive: " + YesNo(rootOwnedModalTemplate != null));
            GUILayout.Label("RootOwned Modal runtime: " + RuntimeRootOwnedModalState());
            GUILayout.Label("ExternalOwned Modal object alive: " + YesNo(externalModalView != null));
            GUILayout.Label("ExternalOwned Modal active: " + YesNo(externalModalView != null && externalModalView.gameObject.activeSelf));
            GUILayout.Label("ExternalOwned Modal registered: " + YesNo(externalModalRegistered));
        }

        private void InitializeModalProof(
            UIModalScreenMutationPolicy policy)
        {
            if (modalReady ||
                root == null ||
                !m2Ready)
            {
                return;
            }

            if (sceneModalView == null ||
                rootOwnedModalTemplate == null ||
                externalModalView == null)
            {
                modalMessage =
                    "Required M2-02 SceneOwned/RootOwned/ExternalOwned Modal proof views are missing.";
                return;
            }

            List<UIModalDefinition> definitions =
                new List<UIModalDefinition>
                {
                    new UIModalDefinition(
                        SceneModalId,
                        FloatingLayerId,
                        UIScreenOwnershipMode.SceneOwned,
                        sceneOwnedView: sceneModalView,
                        backPolicy:
                            new UIModalBackPolicy(
                                UIModalBackBehavior.CompleteWithResultId,
                                CancelResultId)),
                    new UIModalDefinition(
                        RootModalId,
                        FloatingLayerId,
                        UIScreenOwnershipMode.RootOwned,
                        rootOwnedPrefab: rootOwnedModalTemplate.gameObject,
                        backPolicy:
                            new UIModalBackPolicy(
                                UIModalBackBehavior.Disabled)),
                    new UIModalDefinition(
                        ExternalModalId,
                        FloatingLayerId,
                        UIScreenOwnershipMode.ExternalOwned,
                        backPolicy:
                            new UIModalBackPolicy(
                                UIModalBackBehavior.CompleteWithResultId,
                                CancelResultId))
                };

            UISurfaceOperationResult result =
                root.InitializeModalLifecycle(
                    definitions,
                    null,
                    8,
                    policy,
                    8);

            if (!result.Succeeded)
            {
                modalMessage =
                    "Modal lifecycle initialization failed: " +
                    result.Status +
                    " | " +
                    result.Message;
                return;
            }

            activeModalScreenPolicy =
                policy;

            UISurfaceOperationResult externalResult =
                root.RegisterExternalModalView(
                    ExternalModalId,
                    externalModalView);

            externalModalRegistered =
                externalResult.Succeeded;

            if (!externalModalRegistered)
            {
                modalMessage =
                    "ExternalOwned Modal registration failed: " +
                    externalResult.Status +
                    " | " +
                    externalResult.Message;
                return;
            }

            modalReady = true;
            modalMessage =
                "READY. Modal lifecycle initialized with " +
                policy +
                ".";
            ResetModalProofState();
        }

        private void EnsureExternalModalRegistered()
        {
            if (!modalReady ||
                externalModalRegistered ||
                externalModalView == null)
            {
                return;
            }

            UISurfaceOperationResult result =
                root.RegisterExternalModalView(
                    ExternalModalId,
                    externalModalView);

            externalModalRegistered =
                result.Succeeded;

            modalAttemptSummary =
                "External Modal registration: " +
                result.Status +
                " | " +
                result.Message;
        }

        private void CompleteTrackedModal(
            UIModalHandle handle,
            string resultId,
            string label)
        {
            UIModalCompletionAttemptResult result =
                root.CompleteModal(
                    handle,
                    resultId);

            modalAttemptSummary =
                label +
                ": " +
                result.Status +
                " | " +
                result.Message;

            CaptureCompletedHandleSummaries();
            RefreshDeferredObservation();
        }

        private void AbortTrackedModal(
            UIModalHandle handle,
            UIModalAbortReason reason,
            string label)
        {
            UIModalCompletionAttemptResult result =
                root.AbortModal(
                    handle,
                    reason);

            modalAttemptSummary =
                label +
                ": " +
                result.Status +
                " | " +
                result.Message;

            CaptureCompletedHandleSummaries();
            RefreshDeferredObservation();
        }

        private void TrackModalHandle(
            string label,
            UIModalHandle handle)
        {
            if (handle == null)
            {
                modalAttemptSummary =
                    label + ": <no handle>";
                return;
            }

            modalAttemptSummary =
                label +
                ": accepted=" +
                handle.Accepted +
                ", generation=" +
                handle.Generation +
                ", completed=" +
                handle.IsCompleted;

            CaptureCompletedHandleSummaries();
        }

        private void CaptureCompletedHandleSummaries()
        {
            UIModalHandle[] handles =
            {
                sceneModalHandle,
                rootModalHandle,
                externalModalHandle
            };

            for (int index = handles.Length - 1;
                 index >= 0;
                 index--)
            {
                UIModalHandle handle =
                    handles[index];

                if (handle == null ||
                    !handle.IsCompleted)
                {
                    continue;
                }

                UIModalResult result =
                    handle.Result;

                modalTerminalSummary =
                    result.ModalId.Value +
                    " gen=" +
                    result.Generation +
                    " outcome=" +
                    result.Outcome +
                    (result.IsSemanticCompletion
                        ? " resultId=" + result.ResultId.Value
                        : " abortReason=" + result.AbortReason);

                return;
            }
        }

        private void RefreshDeferredObservation()
        {
            if (modalScreenRequestA == null &&
                modalScreenRequestB == null)
            {
                return;
            }

            deferredObserved =
                ScreenHandleSettlement(
                    modalScreenRequestA) +
                (modalScreenRequestB == null
                    ? string.Empty
                    : " -> " +
                      ScreenHandleSettlement(
                          modalScreenRequestB)) +
                " | current=" +
                root.GetCurrentScreenId(
                    FrontendScopeId) +
                " | depth=" +
                root.GetScreenHistoryDepth(
                    FrontendScopeId);
        }

        private static string ScreenHandleSettlement(
            UIScreenHandle handle)
        {
            if (handle == null)
            {
                return "<none>";
            }

            if (!handle.IsCompleted)
            {
                return "seq=" +
                       handle.Request.Sequence +
                       " " +
                       handle.Request.Kind +
                       " Pending";
            }

            return "seq=" +
                   handle.Request.Sequence +
                   " " +
                   handle.Request.Kind +
                   " " +
                   handle.Result.Status;
        }

        private string RuntimeRootOwnedModalState()
        {
            UISurface runtime =
                FindRuntimeRootOwnedModalSurface();

            return runtime == null
                ? "<released>"
                : SurfaceStateWithRaycast(
                    runtime);
        }

        private UISurface FindRuntimeRootOwnedModalSurface()
        {
            if (root == null)
            {
                return null;
            }

            UISurface[] candidates =
                Resources.FindObjectsOfTypeAll<UISurface>();

            for (int index = 0;
                 index < candidates.Length;
                 index++)
            {
                UISurface candidate =
                    candidates[index];

                if (candidate == null ||
                    candidate == rootOwnedModalTemplate ||
                    candidate.gameObject.scene != gameObject.scene ||
                    candidate.SurfaceId != RootModalId)
                {
                    continue;
                }

                if (candidate.transform.IsChildOf(
                        root.transform))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string SurfaceStateWithRaycast(
            UISurface surface)
        {
            if (surface == null)
            {
                return "<missing>";
            }

            CanvasGroup group =
                surface.GetComponent<CanvasGroup>();

            return SurfaceState(
                       surface) +
                   ", blocksRaycasts=" +
                   (group == null
                       ? "<no CanvasGroup>"
                       : group.blocksRaycasts.ToString());
        }

        private void ResetModalProofState()
        {
            AbortIfLive(
                sceneModalHandle);
            AbortIfLive(
                rootModalHandle);
            AbortIfLive(
                externalModalHandle);

            sceneModalHandle = null;
            rootModalHandle = null;
            externalModalHandle = null;
            modalScreenRequestA = null;
            modalScreenRequestB = null;
            deferredObserved = "<not run>";
            modalAttemptSummary = "<none>";
            modalTerminalSummary = "<none>";

            EnsureExternalModalRegistered();

            if (root.IsScreenLifecycleInitialized &&
                !root.HasBlockingModal)
            {
                root.ResetScreen(
                    MainMenuId);
            }

            root.CloseSurface(
                DefaultWindowId);

            externalProjectActionCount = 0;
            modalMessage =
                modalReady
                    ? "READY. Proof state reset."
                    : modalMessage;
        }

        private void AbortIfLive(
            UIModalHandle handle)
        {
            if (handle == null ||
                !handle.Accepted ||
                handle.IsCompleted)
            {
                return;
            }

            root.AbortModal(
                handle,
                UIModalAbortReason.ExplicitAbort);
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
            if (modalReady)
            {
                ResetModalProofState();
            }

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
