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
                DrawM202ModalConsole();
            }
            else if (selectedTab == 1)
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
