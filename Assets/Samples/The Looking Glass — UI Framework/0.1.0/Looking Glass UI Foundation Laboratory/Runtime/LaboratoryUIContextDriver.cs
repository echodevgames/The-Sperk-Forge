using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Sample-owned Looking Glass proof console.
    /// M1 controls simulate external context/input truth.
    /// M2 controls exercise the authoritative Screen/Modal lifecycle using scene-authored
    /// definitions. M3-01 adds sample-owned EventSystem/focus proof infrastructure.
    /// M3-02 adds sample-owned transition/failure/performance proof infrastructure.
    /// M4-01 adds sample-owned named HUD-region, widget-lease, visibility, and owner-loss proof.
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
        private const string M302FailureDriverId = "lab-transition-failure";
        private const string M302NeverDriverId = "lab-transition-never";
        private const string M401StatusRegionId = "hud.status";
        private const string M401UtilityRegionId = "hud.utility";
        private const string M401HealthWidgetId = "hud.status.health";
        private const string M401ObjectiveWidgetId = "hud.status.objective";
        private const string M401UtilityWidgetId = "hud.utility.signal";
        private const string M401StaleWidgetId = "hud.status.stale-probe";


        private sealed class LaboratoryFailureTransitionDriver :
            IUITransitionDriver
        {
            public string DriverId =>
                M302FailureDriverId;

            public bool SupportsCancellation =>
                true;

            public Awaitable<UITransitionResult> ExecuteAsync(
                UITransitionRequest request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                throw new InvalidOperationException(
                    "Laboratory-injected transition failure.");
            }

            public void ForceFinalState(
                UITransitionRequest request)
            {
                ApplyTerminalAlpha(
                    request);
            }
        }

        private sealed class LaboratoryNeverTransitionDriver :
            IUITransitionDriver
        {
            private readonly List<AwaitableCompletionSource<UITransitionResult>>
                pending =
                    new List<AwaitableCompletionSource<UITransitionResult>>();

            public string DriverId =>
                M302NeverDriverId;

            public bool SupportsCancellation =>
                false;

            public Awaitable<UITransitionResult> ExecuteAsync(
                UITransitionRequest request,
                CancellationToken cancellationToken)
            {
                AwaitableCompletionSource<UITransitionResult> completion =
                    new AwaitableCompletionSource<UITransitionResult>();

                pending.Add(
                    completion);

                return completion.Awaitable;
            }

            public void ForceFinalState(
                UITransitionRequest request)
            {
                ApplyTerminalAlpha(
                    request);
            }

            public void Clear() =>
                pending.Clear();
        }

        private static void ApplyTerminalAlpha(
            UITransitionRequest request)
        {
            if (request == null ||
                request.Surface == null)
            {
                return;
            }

            CanvasGroup group =
                request.Surface.GetComponent<CanvasGroup>();

            if (group != null)
            {
                group.alpha =
                    request.Direction ==
                        UITransitionDirection.Enter
                        ? 1f
                        : 0f;
            }
        }

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
        private LaboratoryFailureTransitionDriver m302FailureDriver;
        private LaboratoryNeverTransitionDriver m302NeverDriver;
        private UITransitionProfile m302FadeProfile;
        private UITransitionProfile m302FailureProfile;
        private UITransitionProfile m302TimeoutProfile;
        private UITransitionProfile m302ImmediateProfile;
        private UITransitionProfile m302SlowFadeProfile;
        private UITransitionResult m302StaleFirstResult;
        private bool m302StaleFirstDone;
        private bool m302Busy;
        private string m302Message =
            "M3-02 proof infrastructure has not initialized yet.";
        private string m302Observed =
            "<not run>";
        private string m302PerformanceEvidence =
            "<not run>";
        private LaboratoryNotificationPresenter m402Presenter;
        private LaboratoryNotificationProof m402Proof;
        private UIHudRegionHost m401StatusRegion;
        private UIHudRegionHost m401UtilityRegion;
        private UISurface m401HealthWidget;
        private UISurface m401ObjectiveWidget;
        private UISurface m401UtilityWidget;
        private UISurface m401StaleWidget;
        private UIHudWidgetHandle m401HealthHandle;
        private UIHudWidgetHandle m401ObjectiveHandle;
        private UIHudWidgetHandle m401UtilityHandle;
        private UIHudWidgetHandle m401ProbeHandle;
        private UIHudWidgetHandle m401ReplacementHandle;
        private UIHudVisibilityLease m401HideLease;
        private UIHudVisibilityLease m401ShowLease;
        private UIHudVisibilityLease m401OwnerHideLease;
        private GameObject m401ProbeOwner;
        private GameObject m401ReplacementOwner;
        private GameObject m401VisibilityOwner;
        private bool m401Busy;
        private string m401Message =
            "M4-01 HUD proof infrastructure has not initialized yet.";
        private string m401Observed =
            "<not run>";
        private string m401PerformanceEvidence =
            "<not run>";

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
                    case M401HealthWidgetId:
                        m401HealthWidget = surface;
                        break;
                    case M401ObjectiveWidgetId:
                        m401ObjectiveWidget = surface;
                        break;
                    case M401UtilityWidgetId:
                        m401UtilityWidget = surface;
                        break;
                    case M401StaleWidgetId:
                        m401StaleWidget = surface;
                        break;
                }
            }

            UIHudRegionHost[] hudRegions =
                GetComponentsInChildren<UIHudRegionHost>(true);
            for (int index = 0; index < hudRegions.Length; index++)
            {
                UIHudRegionHost region = hudRegions[index];
                if (region.RegionId.Value == M401StatusRegionId)
                {
                    m401StatusRegion = region;
                }
                else if (region.RegionId.Value == M401UtilityRegionId)
                {
                    m401UtilityRegion = region;
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

            InitializeM402ProofInfrastructure();
            InitializeM401ProofInfrastructure();
            InitializeM302ProofInfrastructure();
            InitializeM2Proof();

            sceneEventSystem =
                ResolveEventSystem();

            EnsureM3FocusTargets();

            m3Message =
                "M3 proof READY. Use Prepare M3 Baseline before a fresh acceptance run.";

            m302Message =
                root != null &&
                root.IsTransitionLifecycleInitialized
                    ? "M3-02 proof READY. Click Prepare M3-02 Baseline before running checks."
                    : "M3-02 transition lifecycle is not initialized.";
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
                    "M4-02 Notifications",
                    "M4-01 HUD",
                    "M3-02 Transitions",
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
                if (m402Proof == null)
                {
                    GUILayout.Label(
                        "M4-02 notification proof infrastructure is unavailable.");
                }
                else
                {
                    m402Proof.DrawConsole();
                }
            }
            else if (selectedTab == 1)
            {
                DrawM401HudConsole();
            }
            else if (selectedTab == 2)
            {
                DrawM302TransitionConsole();
            }
            else if (selectedTab == 3)
            {
                DrawM301FocusConsole();
            }
            else if (selectedTab == 4)
            {
                DrawM202ModalConsole();
            }
            else if (selectedTab == 5)
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

        private void OnDestroy()
        {
            if (m402Proof != null)
            {
                m402Proof.Dispose();
                m402Proof = null;
            }
        }

        private void InitializeM402ProofInfrastructure()
        {
            if (root == null ||
                !root.IsInitialized ||
                !root.IsNotificationLifecycleInitialized)
            {
                return;
            }

            m402Presenter =
                GetComponent<LaboratoryNotificationPresenter>();

            if (m402Presenter == null)
            {
                m402Presenter =
                    gameObject.AddComponent<
                        LaboratoryNotificationPresenter>();
            }

            m402Proof =
                new LaboratoryNotificationProof(
                    root,
                    m402Presenter);

            m402Proof.Initialize();
        }


        private void InitializeM401ProofInfrastructure()
        {
            if (root == null ||
                !root.IsInitialized ||
                !root.IsHudLifecycleInitialized)
            {
                m401Message =
                    "M4-01 HUD lifecycle is not initialized.";
                return;
            }

            if (m401StatusRegion == null ||
                m401UtilityRegion == null ||
                m401HealthWidget == null ||
                m401ObjectiveWidget == null ||
                m401UtilityWidget == null ||
                m401StaleWidget == null)
            {
                m401Message =
                    "Required M4-01 scene-authored regions/widgets are missing.";
                return;
            }

            bool baselineReady =
                EnsureM401BaselineWidgets();

            m401Message =
                baselineReady
                    ? "M4-01 HUD proof READY. Click Prepare M4-01 Baseline before a fresh acceptance run."
                    : "M4-01 baseline widget registration failed. Inspect the latest observation.";
        }

        private void DrawM401HudConsole()
        {
            GUILayout.Label(
                "EUI-M4-01: named HUD regions, generation-safe widget/visibility leases, deterministic visibility, and owner-loss cleanup.");
            GUILayout.Label(
                "The cyan/blue panels at top-left are hud.status widgets; the magenta panel at bottom-left is hud.utility. HUD remains presentation-only.");
            GUILayout.Space(8f);

            DrawM401State();
            GUILayout.Space(8f);

            bool previousEnabled =
                GUI.enabled;

            GUI.enabled =
                !m401Busy;

            if (GUILayout.Button(
                    "Prepare M4-01 Baseline"))
            {
                PrepareM401Baseline();
            }

            GUILayout.Space(10f);
            GUILayout.Label(
                "1. Discover two named regions and three registered widgets");
            if (GUILayout.Button(
                    "Run Check 1: Named Regions + Multiple Widgets"))
            {
                RunM401Check1();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "2. Hide then override-show hud.status; release the older hide lease first");
            if (GUILayout.Button(
                    "Run Check 2: Overlapping Visibility Leases"))
            {
                RunM401Check2();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "3. Destroy widget/visibility owners, re-register, then prove the old handle is stale");
            if (GUILayout.Button(
                    "Run Check 3: Owner Loss + Stale Handle"))
            {
                RunM401Check3();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "4. Duplicate widget and full-region admission reject without mutation");
            if (GUILayout.Button(
                    "Run Check 4: Duplicate + Capacity"))
            {
                RunM401Check4();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "5. 180-frame idle HUD-state quiescence");
            if (GUILayout.Button(
                    "Run Check 5: Idle HUD Probe"))
            {
                StartCoroutine(
                    RunM401Check5());
            }

            GUI.enabled =
                previousEnabled;

            GUILayout.Space(12f);
            GUILayout.Label(
                "6. Retained smoke: visit M3-02 Transitions, M3-01 Focus, M2-02 Modals, M2-01 Screens, and M1 Retained.");
            GUILayout.Label(
                "No M4-01 check may change Screen history, Modal order, Window state, gameplay input, pause/time scale, persistence, or domain truth.");

            GUILayout.Space(12f);
            GUILayout.Label(
                "Latest M4-01 observation");
            GUILayout.TextArea(
                m401Observed,
                GUILayout.MinHeight(120f));

            GUILayout.Space(8f);
            GUILayout.Label(
                "Performance evidence: " +
                m401PerformanceEvidence);
        }

        private void DrawM401State()
        {
            GUILayout.Label(
                "HUD lifecycle initialized: " +
                root.IsHudLifecycleInitialized);
            GUILayout.Label(
                "Regions/widgets/visibility leases: " +
                root.HudRegionCount +
                " / " +
                root.ActiveHudWidgetCount +
                " / " +
                root.ActiveHudVisibilityLeaseCount);
            GUILayout.Label(
                "hud.status: " +
                M401SnapshotSummary(
                    M401StatusRegionId));
            GUILayout.Label(
                "hud.utility: " +
                M401SnapshotSummary(
                    M401UtilityRegionId));
            GUILayout.Label(
                "Status: " +
                m401Message);
            GUILayout.Label(
                "Busy: " +
                YesNo(
                    m401Busy));
        }

        private async void PrepareM401Baseline()
        {
            if (m401Busy)
            {
                return;
            }

            m401Busy = true;

            try
            {
                ReleaseM401TemporaryState();
                await WaitM401FramesAsync(3);

                bool ready =
                    EnsureM401BaselineWidgets();

                bool statusFound =
                    root.TryGetHudRegionSnapshot(
                        M401StatusRegionId,
                        out UIHudRegionSnapshot status);
                bool utilityFound =
                    root.TryGetHudRegionSnapshot(
                        M401UtilityRegionId,
                        out UIHudRegionSnapshot utility);

                bool passed =
                    ready &&
                    statusFound &&
                    utilityFound &&
                    status.WidgetCount == 2 &&
                    utility.WidgetCount == 1 &&
                    status.VisibilityLeaseCount == 0 &&
                    utility.VisibilityLeaseCount == 0 &&
                    status.EffectiveVisibility &&
                    utility.EffectiveVisibility;

                m401Observed =
                    (passed
                        ? "M4-01 BASELINE READY"
                        : "M4-01 BASELINE NOT READY") +
                    ". regions=" +
                    root.HudRegionCount +
                    ", widgets=" +
                    root.ActiveHudWidgetCount +
                    ", leases=" +
                    root.ActiveHudVisibilityLeaseCount +
                    ", status=" +
                    M401SnapshotSummary(
                        M401StatusRegionId) +
                    ", utility=" +
                    M401SnapshotSummary(
                        M401UtilityRegionId) +
                    ".";

                m401Message =
                    passed
                        ? "M4-01 baseline READY."
                        : "M4-01 baseline did not resolve the expected authored state.";
            }
            catch (Exception exception)
            {
                SetM401Exception(
                    "Prepare baseline",
                    exception);
            }
            finally
            {
                m401Busy = false;
            }
        }

        private void RunM401Check1()
        {
            if (!CanRunM401Check())
            {
                return;
            }

            bool statusFound =
                root.TryGetHudRegionSnapshot(
                    M401StatusRegionId,
                    out UIHudRegionSnapshot status);
            bool utilityFound =
                root.TryGetHudRegionSnapshot(
                    M401UtilityRegionId,
                    out UIHudRegionSnapshot utility);

            bool passed =
                root.HudRegionCount == 2 &&
                root.ActiveHudWidgetCount == 3 &&
                statusFound &&
                utilityFound &&
                status.WidgetCount == 2 &&
                utility.WidgetCount == 1 &&
                status.OwnershipMode ==
                    UIHudOwnershipMode.SceneOwned &&
                utility.OwnershipMode ==
                    UIHudOwnershipMode.SceneOwned &&
                status.EffectiveVisibility &&
                utility.EffectiveVisibility;

            m401Observed =
                (passed ? "CHECK 1 PASS" : "CHECK 1 FAIL") +
                " named regions + multiple widgets. status=" +
                M401SnapshotSummary(
                    M401StatusRegionId) +
                ", utility=" +
                M401SnapshotSummary(
                    M401UtilityRegionId) +
                ".";
        }

        private async void RunM401Check2()
        {
            if (!CanRunM401Check())
            {
                return;
            }

            m401Busy = true;

            string screenBefore =
                root.GetCurrentScreenId(
                    FrontendScopeId);
            int modalBefore =
                root.ActiveModalCount;
            bool windowBefore =
                defaultWindow != null &&
                defaultWindow.gameObject.activeSelf;

            try
            {
                ReleaseM401VisibilityLease(
                    ref m401HideLease);
                ReleaseM401VisibilityLease(
                    ref m401ShowLease);

                m401HideLease =
                    root.RequestHudVisibility(
                        M401StatusRegionId,
                        "lab.cinematic-hide",
                        false,
                        priority: 10);

                bool hidden =
                    TryGetM401Status(
                        out UIHudRegionSnapshot afterHide) &&
                    !afterHide.EffectiveVisibility &&
                    afterHide.VisibilityLeaseCount == 1;

                await WaitM401FramesAsync(45);

                m401ShowLease =
                    root.RequestHudVisibility(
                        M401StatusRegionId,
                        "lab.accessibility-show",
                        true,
                        priority: 20);

                bool shownByWinner =
                    TryGetM401Status(
                        out UIHudRegionSnapshot afterShow) &&
                    afterShow.EffectiveVisibility &&
                    afterShow.VisibilityLeaseCount == 2;

                await WaitM401FramesAsync(45);

                UIHudOperationResult hideRelease =
                    m401HideLease.Release();
                m401HideLease = null;

                bool stayedVisible =
                    TryGetM401Status(
                        out UIHudRegionSnapshot afterOlderRelease) &&
                    afterOlderRelease.EffectiveVisibility &&
                    afterOlderRelease.VisibilityLeaseCount == 1;

                await WaitM401FramesAsync(30);

                UIHudOperationResult showRelease =
                    m401ShowLease.Release();
                m401ShowLease = null;

                bool restoredBaseline =
                    TryGetM401Status(
                        out UIHudRegionSnapshot afterFinalRelease) &&
                    afterFinalRelease.EffectiveVisibility &&
                    afterFinalRelease.VisibilityLeaseCount == 0;

                bool structuralUnchanged =
                    M401StructuralStateMatches(
                        screenBefore,
                        modalBefore,
                        windowBefore);

                bool passed =
                    m401HideLease == null &&
                    m401ShowLease == null &&
                    hidden &&
                    shownByWinner &&
                    hideRelease.Succeeded &&
                    stayedVisible &&
                    showRelease.Succeeded &&
                    restoredBaseline &&
                    structuralUnchanged;

                m401Observed =
                    (passed ? "CHECK 2 PASS" : "CHECK 2 FAIL") +
                    " overlapping visibility/out-of-order release. hidden=" +
                    YesNo(
                        hidden) +
                    ", higherShow=" +
                    YesNo(
                        shownByWinner) +
                    ", olderHideRelease=" +
                    hideRelease.Status +
                    ", stayedVisible=" +
                    YesNo(
                        stayedVisible) +
                    ", finalShowRelease=" +
                    showRelease.Status +
                    ", baselineVisible=" +
                    YesNo(
                        restoredBaseline) +
                    ", structuralUnchanged=" +
                    YesNo(
                        structuralUnchanged) +
                    ".";
            }
            catch (Exception exception)
            {
                SetM401Exception(
                    "Check 2",
                    exception);
            }
            finally
            {
                m401Busy = false;
            }
        }

        private async void RunM401Check3()
        {
            if (!CanRunM401Check())
            {
                return;
            }

            m401Busy = true;

            try
            {
                ReleaseM401ProbeState();

                m401ProbeOwner =
                    new GameObject(
                        "M4_01_OldWidgetOwner");
                m401ProbeHandle =
                    root.RegisterHudWidget(
                        M401StatusRegionId,
                        M401StaleWidgetId,
                        m401StaleWidget,
                        m401ProbeOwner,
                        order: 99);

                m401VisibilityOwner =
                    new GameObject(
                        "M4_01_VisibilityOwner");
                m401OwnerHideLease =
                    root.RequestHudVisibility(
                        M401StatusRegionId,
                        "lab.owner-loss-hide",
                        false,
                        priority: 100,
                        owner: m401VisibilityOwner);

                bool admitted =
                    m401ProbeHandle.Accepted &&
                    m401OwnerHideLease.Accepted &&
                    TryGetM401Status(
                        out UIHudRegionSnapshot admittedSnapshot) &&
                    admittedSnapshot.WidgetCount == 3 &&
                    admittedSnapshot.VisibilityLeaseCount == 1 &&
                    !admittedSnapshot.EffectiveVisibility;

                Destroy(
                    m401ProbeOwner);
                Destroy(
                    m401VisibilityOwner);
                m401ProbeOwner = null;
                m401VisibilityOwner = null;

                await WaitM401FramesAsync(5);

                bool ownerLossCleaned =
                    TryGetM401Status(
                        out UIHudRegionSnapshot cleanedSnapshot) &&
                    cleanedSnapshot.WidgetCount == 2 &&
                    cleanedSnapshot.VisibilityLeaseCount == 0 &&
                    cleanedSnapshot.EffectiveVisibility;

                m401ReplacementOwner =
                    new GameObject(
                        "M4_01_ReplacementWidgetOwner");
                m401ReplacementHandle =
                    root.RegisterHudWidget(
                        M401StatusRegionId,
                        M401StaleWidgetId,
                        m401StaleWidget,
                        m401ReplacementOwner,
                        order: 99);

                UIHudOperationResult staleRelease =
                    m401ProbeHandle.Release();
                m401ProbeHandle = null;
                m401OwnerHideLease = null;

                bool replacementSurvived =
                    m401ReplacementHandle.Accepted &&
                    staleRelease.Status ==
                        UIHudOperationStatus.Stale &&
                    TryGetM401Status(
                        out UIHudRegionSnapshot afterStaleRelease) &&
                    afterStaleRelease.WidgetCount == 3;

                UIHudOperationResult replacementRelease =
                    m401ReplacementHandle.Release();
                m401ReplacementHandle = null;

                Destroy(
                    m401ReplacementOwner);
                m401ReplacementOwner = null;

                bool finalBaseline =
                    replacementRelease.Succeeded &&
                    TryGetM401Status(
                        out UIHudRegionSnapshot finalSnapshot) &&
                    finalSnapshot.WidgetCount == 2 &&
                    finalSnapshot.VisibilityLeaseCount == 0 &&
                    finalSnapshot.EffectiveVisibility;

                bool passed =
                    admitted &&
                    ownerLossCleaned &&
                    replacementSurvived &&
                    finalBaseline;

                m401Observed =
                    (passed ? "CHECK 3 PASS" : "CHECK 3 FAIL") +
                    " owner loss + stale generation. admitted=" +
                    YesNo(
                        admitted) +
                    ", ownerLossCleaned=" +
                    YesNo(
                        ownerLossCleaned) +
                    ", staleRelease=" +
                    staleRelease.Status +
                    ", replacementSurvived=" +
                    YesNo(
                        replacementSurvived) +
                    ", finalBaseline=" +
                    YesNo(
                        finalBaseline) +
                    ".";
            }
            catch (Exception exception)
            {
                SetM401Exception(
                    "Check 3",
                    exception);
            }
            finally
            {
                m401Busy = false;
            }
        }

        private void RunM401Check4()
        {
            if (!CanRunM401Check())
            {
                return;
            }

            int widgetsBefore =
                root.ActiveHudWidgetCount;

            UIHudWidgetHandle duplicate =
                root.RegisterHudWidget(
                    M401StatusRegionId,
                    M401HealthWidgetId,
                    m401HealthWidget,
                    order: 0);

            UIHudWidgetHandle overflow =
                root.RegisterHudWidget(
                    M401UtilityRegionId,
                    M401StaleWidgetId,
                    m401StaleWidget,
                    order: 100);

            bool snapshotsStable =
                TryGetM401Status(
                    out UIHudRegionSnapshot status) &&
                root.TryGetHudRegionSnapshot(
                    M401UtilityRegionId,
                    out UIHudRegionSnapshot utility) &&
                status.WidgetCount == 2 &&
                utility.WidgetCount == 1 &&
                root.ActiveHudWidgetCount ==
                    widgetsBefore;

            bool passed =
                !duplicate.Accepted &&
                duplicate.LastResult.Status ==
                    UIHudOperationStatus.Duplicate &&
                !overflow.Accepted &&
                overflow.LastResult.Status ==
                    UIHudOperationStatus.CapacityExceeded &&
                snapshotsStable;

            m401Observed =
                (passed ? "CHECK 4 PASS" : "CHECK 4 FAIL") +
                " rejection without mutation. duplicate=" +
                duplicate.LastResult.Status +
                ", overflow=" +
                overflow.LastResult.Status +
                ", widgets=" +
                root.ActiveHudWidgetCount +
                ", snapshotsStable=" +
                YesNo(
                    snapshotsStable) +
                ".";
        }

        private IEnumerator RunM401Check5()
        {
            if (!CanRunM401Check())
            {
                yield break;
            }

            m401Busy = true;

            int regionsBefore =
                root.HudRegionCount;
            int widgetsBefore =
                root.ActiveHudWidgetCount;
            int leasesBefore =
                root.ActiveHudVisibilityLeaseCount;
            int transitionsBefore =
                root.ActiveTransitionCount;
            string screenBefore =
                root.GetCurrentScreenId(
                    FrontendScopeId);
            int modalBefore =
                root.ActiveModalCount;
            bool windowBefore =
                defaultWindow != null &&
                defaultWindow.gameObject.activeSelf;

            bool statusBeforeFound =
                TryGetM401Status(
                    out UIHudRegionSnapshot statusBefore);
            bool utilityBeforeFound =
                root.TryGetHudRegionSnapshot(
                    M401UtilityRegionId,
                    out UIHudRegionSnapshot utilityBefore);

            const int sampleFrames = 180;
            for (int frame = 0;
                 frame < sampleFrames;
                 frame++)
            {
                yield return null;
            }

            bool statusAfterFound =
                TryGetM401Status(
                    out UIHudRegionSnapshot statusAfter);
            bool utilityAfterFound =
                root.TryGetHudRegionSnapshot(
                    M401UtilityRegionId,
                    out UIHudRegionSnapshot utilityAfter);

            bool snapshotsStable =
                statusBeforeFound &&
                statusAfterFound &&
                utilityBeforeFound &&
                utilityAfterFound &&
                M401SnapshotsEqual(
                    statusBefore,
                    statusAfter) &&
                M401SnapshotsEqual(
                    utilityBefore,
                    utilityAfter);

            bool countsStable =
                root.HudRegionCount ==
                    regionsBefore &&
                root.ActiveHudWidgetCount ==
                    widgetsBefore &&
                root.ActiveHudVisibilityLeaseCount ==
                    leasesBefore &&
                root.ActiveTransitionCount ==
                    transitionsBefore;

            bool structuralUnchanged =
                M401StructuralStateMatches(
                    screenBefore,
                    modalBefore,
                    windowBefore);

            bool passed =
                snapshotsStable &&
                countsStable &&
                structuralUnchanged;

            m401PerformanceEvidence =
                (passed ? "PASS" : "FAIL") +
                " 180 idle frames. regions=" +
                regionsBefore +
                "->" +
                root.HudRegionCount +
                ", widgets=" +
                widgetsBefore +
                "->" +
                root.ActiveHudWidgetCount +
                ", leases=" +
                leasesBefore +
                "->" +
                root.ActiveHudVisibilityLeaseCount +
                ", transitions=" +
                transitionsBefore +
                "->" +
                root.ActiveTransitionCount +
                ", snapshotsStable=" +
                YesNo(
                    snapshotsStable) +
                ", structuralUnchanged=" +
                YesNo(
                    structuralUnchanged) +
                ".";

            m401Observed =
                (passed ? "CHECK 5 PASS. " : "CHECK 5 FAIL. ") +
                m401PerformanceEvidence;

            m401Busy = false;
        }

        private bool CanRunM401Check()
        {
            if (m401Busy)
            {
                return false;
            }

            if (root == null ||
                !root.IsInitialized ||
                !root.IsHudLifecycleInitialized ||
                !EnsureM401BaselineWidgets())
            {
                m401Observed =
                    "M4-01 proof is not ready. Click Prepare M4-01 Baseline first.";
                return false;
            }

            return true;
        }

        private bool EnsureM401BaselineWidgets()
        {
            if (root == null ||
                !root.IsHudLifecycleInitialized ||
                m401HealthWidget == null ||
                m401ObjectiveWidget == null ||
                m401UtilityWidget == null)
            {
                return false;
            }

            if (m401HealthHandle == null ||
                !m401HealthHandle.Accepted ||
                m401HealthHandle.IsReleased)
            {
                m401HealthHandle =
                    root.RegisterHudWidget(
                        M401StatusRegionId,
                        M401HealthWidgetId,
                        m401HealthWidget,
                        order: 0);
            }

            if (m401ObjectiveHandle == null ||
                !m401ObjectiveHandle.Accepted ||
                m401ObjectiveHandle.IsReleased)
            {
                m401ObjectiveHandle =
                    root.RegisterHudWidget(
                        M401StatusRegionId,
                        M401ObjectiveWidgetId,
                        m401ObjectiveWidget,
                        order: 10);
            }

            if (m401UtilityHandle == null ||
                !m401UtilityHandle.Accepted ||
                m401UtilityHandle.IsReleased)
            {
                m401UtilityHandle =
                    root.RegisterHudWidget(
                        M401UtilityRegionId,
                        M401UtilityWidgetId,
                        m401UtilityWidget,
                        order: 0);
            }

            bool statusFound =
                TryGetM401Status(
                    out UIHudRegionSnapshot status);
            bool utilityFound =
                root.TryGetHudRegionSnapshot(
                    M401UtilityRegionId,
                    out UIHudRegionSnapshot utility);

            return m401HealthHandle.Accepted &&
                m401ObjectiveHandle.Accepted &&
                m401UtilityHandle.Accepted &&
                statusFound &&
                utilityFound &&
                status.WidgetCount == 2 &&
                utility.WidgetCount == 1;
        }

        private void ReleaseM401TemporaryState()
        {
            ReleaseM401VisibilityLease(
                ref m401HideLease);
            ReleaseM401VisibilityLease(
                ref m401ShowLease);
            ReleaseM401ProbeState();
        }

        private void ReleaseM401ProbeState()
        {
            ReleaseM401VisibilityLease(
                ref m401OwnerHideLease);
            ReleaseM401WidgetHandle(
                ref m401ProbeHandle);
            ReleaseM401WidgetHandle(
                ref m401ReplacementHandle);

            DestroyM401Owner(
                ref m401ProbeOwner);
            DestroyM401Owner(
                ref m401ReplacementOwner);
            DestroyM401Owner(
                ref m401VisibilityOwner);
        }

        private static void ReleaseM401VisibilityLease(
            ref UIHudVisibilityLease lease)
        {
            if (lease != null &&
                !lease.IsReleased)
            {
                lease.Release();
            }

            lease = null;
        }

        private static void ReleaseM401WidgetHandle(
            ref UIHudWidgetHandle handle)
        {
            if (handle != null &&
                !handle.IsReleased)
            {
                handle.Release();
            }

            handle = null;
        }

        private void DestroyM401Owner(
            ref GameObject owner)
        {
            if (owner != null)
            {
                Destroy(
                    owner);
            }

            owner = null;
        }

        private bool TryGetM401Status(
            out UIHudRegionSnapshot snapshot) =>
            root.TryGetHudRegionSnapshot(
                M401StatusRegionId,
                out snapshot);

        private string M401SnapshotSummary(
            string regionId)
        {
            if (!root.TryGetHudRegionSnapshot(
                    regionId,
                    out UIHudRegionSnapshot snapshot))
            {
                return "<missing>";
            }

            return "generation=" +
                   snapshot.Generation +
                   ", visible=" +
                   YesNo(
                       snapshot.EffectiveVisibility) +
                   ", widgets=" +
                   snapshot.WidgetCount +
                   ", leases=" +
                   snapshot.VisibilityLeaseCount +
                   ", ownership=" +
                   snapshot.OwnershipMode;
        }

        private bool M401StructuralStateMatches(
            string screenId,
            int modalCount,
            bool windowOpen) =>
            string.Equals(
                root.GetCurrentScreenId(
                    FrontendScopeId),
                screenId,
                StringComparison.Ordinal) &&
            root.ActiveModalCount ==
                modalCount &&
            (defaultWindow != null &&
             defaultWindow.gameObject.activeSelf) ==
                windowOpen;

        private static bool M401SnapshotsEqual(
            UIHudRegionSnapshot first,
            UIHudRegionSnapshot second) =>
            first.RegionId.Equals(
                second.RegionId) &&
            first.Generation ==
                second.Generation &&
            first.EffectiveVisibility ==
                second.EffectiveVisibility &&
            first.WidgetCount ==
                second.WidgetCount &&
            first.VisibilityLeaseCount ==
                second.VisibilityLeaseCount &&
            first.OwnershipMode ==
                second.OwnershipMode;

        private static async Awaitable WaitM401FramesAsync(
            int frameCount)
        {
            for (int frame = 0;
                 frame < frameCount;
                 frame++)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private void SetM401Exception(
            string check,
            Exception exception)
        {
            m401Observed =
                check +
                " EXCEPTION: " +
                exception.GetType().Name +
                " - " +
                exception.Message;
            m401Message =
                "M4-01 proof encountered an exception.";
        }

        private void InitializeM302ProofInfrastructure()
        {
            if (root == null ||
                m302FailureDriver != null)
            {
                return;
            }

            m302FadeProfile =
                new UITransitionProfile(
                    "lab-m3-02-fade",
                    UITransitionDriverIds.CanvasGroupFade,
                    UITransitionDriverIds.CanvasGroupFade,
                    0.45f,
                    0.35f,
                    AnimationCurve.EaseInOut(
                        0f,
                        0f,
                        1f,
                        1f),
                    AnimationCurve.EaseInOut(
                        0f,
                        0f,
                        1f,
                        1f),
                    2f,
                    UITransitionReducedMotionMode.UseReplacement,
                    UITransitionDriverIds.Immediate);

            m302ImmediateProfile =
                new UITransitionProfile(
                    "lab-m3-02-immediate",
                    UITransitionDriverIds.Immediate,
                    UITransitionDriverIds.Immediate,
                    0f,
                    0f,
                    hardTimeoutSeconds: 1f,
                    reducedMotionMode:
                        UITransitionReducedMotionMode.UseReplacement,
                    reducedMotionDriverId:
                        UITransitionDriverIds.Immediate);

            m302FailureProfile =
                new UITransitionProfile(
                    "lab-m3-02-failure",
                    M302FailureDriverId,
                    M302FailureDriverId,
                    0f,
                    0f,
                    hardTimeoutSeconds: 0.5f,
                    reducedMotionMode:
                        UITransitionReducedMotionMode.UseReplacement,
                    reducedMotionDriverId:
                        UITransitionDriverIds.Immediate);

            m302TimeoutProfile =
                new UITransitionProfile(
                    "lab-m3-02-timeout",
                    M302NeverDriverId,
                    M302NeverDriverId,
                    0f,
                    0f,
                    hardTimeoutSeconds: 0.20f,
                    reducedMotionMode:
                        UITransitionReducedMotionMode.UseReplacement,
                    reducedMotionDriverId:
                        UITransitionDriverIds.Immediate);

            m302SlowFadeProfile =
                new UITransitionProfile(
                    "lab-m3-02-slow-fade",
                    UITransitionDriverIds.CanvasGroupFade,
                    UITransitionDriverIds.CanvasGroupFade,
                    1.25f,
                    1.25f,
                    AnimationCurve.Linear(
                        0f,
                        0f,
                        1f,
                        1f),
                    AnimationCurve.Linear(
                        0f,
                        0f,
                        1f,
                        1f),
                    3f,
                    UITransitionReducedMotionMode.UseReplacement,
                    UITransitionDriverIds.Immediate);

            m302FailureDriver =
                new LaboratoryFailureTransitionDriver();

            m302NeverDriver =
                new LaboratoryNeverTransitionDriver();

            bool failureRegistered =
                root.RegisterTransitionDriver(
                    m302FailureDriver);

            bool neverRegistered =
                root.RegisterTransitionDriver(
                    m302NeverDriver);

            m302Message =
                "M3-02 custom proof drivers: failure=" +
                (failureRegistered ? "registered" : "already/unavailable") +
                ", never=" +
                (neverRegistered ? "registered" : "already/unavailable") +
                ".";
        }

        private void DrawM302TransitionConsole()
        {
            GUILayout.Label(
                "EUI-M3-02: authoritative view transitions, replaceable drivers, deterministic recovery, exact-once Modal exit, and reduced-motion substitution.");
            GUILayout.Label(
                "The console is sample-owned evidence tooling. Transition drivers own presentation only; gameplay, pause, input maps, cursor, audio, persistence, and scene travel remain project-owned.");
            GUILayout.Space(8f);

            DrawM302State();
            GUILayout.Space(8f);

            bool previousEnabled =
                GUI.enabled;

            GUI.enabled =
                !m302Busy;

            if (GUILayout.Button(
                    "Prepare M3-02 Baseline"))
            {
                PrepareM302Baseline();
            }

            GUILayout.Space(10f);
            GUILayout.Label(
                "1. Immediate Screen enter/exit through the root default");
            if (GUILayout.Button(
                    "Run Check 1: Immediate RootOwned Screen"))
            {
                RunM302Check1();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "2. Definition-profile Screen CanvasGroup fade");
            if (GUILayout.Button(
                    "Run Check 2: Fade ExternalOwned Screen In + Out"))
            {
                RunM302Check2();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "3. Independent Window fade from a transient operation override");
            if (GUILayout.Button(
                    "Run Check 3: Fade Default Window In + Out"))
            {
                RunM302Check3();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "4. Blocking Modal fade + first-terminal-wins while exit is pending");
            if (GUILayout.Button(
                    "Run Check 4: External Modal Exact-Once Fade"))
            {
                RunM302Check4();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "5. Root default -> definition -> transient policy layering");
            if (GUILayout.Button(
                    "Run Check 5: Inspect Effective Policy Layers"))
            {
                RunM302Check5();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "6. Failed enter rolls back independent Window admission");
            if (GUILayout.Button(
                    "Run Check 6: Inject Enter Failure"))
            {
                RunM302Check6();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "7. Failed exit force-closes independent Window");
            if (GUILayout.Button(
                    "Run Check 7: Inject Exit Failure"))
            {
                RunM302Check7();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "8. Never-completing driver hits hard timeout and cleans up");
            if (GUILayout.Button(
                    "Run Check 8: Inject Hard Timeout"))
            {
                RunM302Check8();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "9. Superseded transition settles Stale and cannot rewind newer truth");
            if (GUILayout.Button(
                    "Run Check 9: Supersede Slow Fade"))
            {
                RunM302Check9();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "10. Reduced-motion substitution replaces fade with Immediate");
            if (GUILayout.Button(
                    "Run Check 10: Reduced Motion -> Immediate"))
            {
                RunM302Check10();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "11. CanvasGroup fade uses unscaled time while Time.timeScale = 0");
            if (GUILayout.Button(
                    "Run Check 11: Paused-Time Fade"))
            {
                RunM302Check11();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "12. M3-01 navigation focus remains valid after a transition");
            if (GUILayout.Button(
                    "Run Check 12: Focus After Window Fade"))
            {
                RunM302Check12();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "13. 180-frame idle transition-coordinator quiescence");
            if (GUILayout.Button(
                    "Run Check 13: Idle Transition Probe"))
            {
                StartCoroutine(
                    RunM302Check13());
            }

            GUI.enabled =
                previousEnabled;

            GUILayout.Space(12f);
            GUILayout.Label(
                "14. Retained smoke: visit M3-01 Focus, M2-02 Modals, M2-01 Screens, and M1 Retained after checks 1-13.");
            GUILayout.Label(
                "No M3-02 check should change project gameplay/input ownership or activate the future Window manager.");

            GUILayout.Space(12f);
            GUILayout.Label(
                "Latest M3-02 observation");
            GUILayout.TextArea(
                m302Observed,
                GUILayout.MinHeight(110f));

            GUILayout.Space(8f);
            GUILayout.Label(
                "Performance evidence: " +
                m302PerformanceEvidence);
        }

        private void DrawM302State()
        {
            GUILayout.Label(
                "Transition lifecycle initialized: " +
                root.IsTransitionLifecycleInitialized);
            GUILayout.Label(
                "Active transitions: " +
                root.ActiveTransitionCount);
            GUILayout.Label(
                "Reduced motion: " +
                OnOff(
                    root.ReducedMotionTransitions));
            GUILayout.Label(
                "Current frontend Screen: " +
                root.GetCurrentScreenId(
                    FrontendScopeId));
            GUILayout.Label(
                "Screen queue depth: " +
                root.ScreenOperationQueueDepth);
            GUILayout.Label(
                "Deferred Screen queue depth: " +
                root.DeferredScreenOperationQueueDepth);
            GUILayout.Label(
                "default-window: " +
                SurfaceStateWithAlpha(
                    defaultWindow));
            GUILayout.Label(
                "ExternalOwned Screen: " +
                SurfaceStateWithAlpha(
                    externalOwnedView));
            GUILayout.Label(
                "ExternalOwned Modal: " +
                SurfaceStateWithAlpha(
                    externalModalView));
            GUILayout.Label(
                "Status: " +
                m302Message);
            GUILayout.Label(
                "Busy: " +
                YesNo(
                    m302Busy));
        }

        private async void PrepareM302Baseline()
        {
            if (m302Busy)
            {
                return;
            }

            m302Busy = true;

            try
            {
                InitializeM302ProofInfrastructure();

                if (!m2Ready)
                {
                    InitializeM2Proof();
                }

                if (!modalReady)
                {
                    InitializeModalProof(
                        UIModalScreenMutationPolicy.Reject);
                }

                if (modalReady)
                {
                    ResetModalProofState();

                    await WaitForM302ConditionAsync(
                        () =>
                            !root.HasBlockingModal,
                        240);
                }

                EnsureExternalRegistered();
                EnsureExternalModalRegistered();

                root.SetReducedMotionTransitions(
                    false);

                root.SetContextActive(
                    PauseContextId,
                    false);

                root.SetContextActive(
                    CinematicContextId,
                    false);

                root.SetInputModality(
                    UIInputModality.Pointer);

                root.CloseSurface(
                    DefaultWindowId);

                UIScreenHandle reset =
                    root.ResetScreen(
                        MainMenuId);

                bool resetSettled =
                    await WaitForM302ScreenHandleAsync(
                        reset,
                        240);

                ClearSelection();
                m302NeverDriver?.Clear();

                m302Observed =
                    "BASELINE " +
                    (resetSettled ? "READY" : "PARTIAL") +
                    ": current=" +
                    root.GetCurrentScreenId(
                        FrontendScopeId) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ", modalPolicy=" +
                    (modalReady
                        ? activeModalScreenPolicy.ToString()
                        : "<not initialized>") +
                    ", rootDefault=Immediate, ExternalOwned Screen definition=CanvasGroup Fade, transient Window profile=CanvasGroup Fade.";

                m302Message =
                    resetSettled
                        ? "M3-02 baseline READY."
                        : "M3-02 baseline reset did not settle inside the bounded wait.";
            }
            catch (Exception exception)
            {
                m302Observed =
                    "BASELINE ERROR: " +
                    exception.GetType().Name +
                    " | " +
                    exception.Message;
                m302Message =
                    "M3-02 baseline preparation failed.";
            }
            finally
            {
                Time.timeScale =
                    1f;
                root.SetReducedMotionTransitions(
                    false);
                m302Busy = false;
            }
        }

        private async void RunM302Check1()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                UIScreenHandle reset =
                    root.ResetScreen(
                        MainMenuId);

                await WaitForM302ScreenHandleAsync(
                    reset,
                    240);

                UIScreenHandle push =
                    root.PushScreen(
                        RootOwnedId);

                bool pushSettled =
                    await WaitForM302ScreenHandleAsync(
                        push,
                        240);

                UIScreenHandle back =
                    root.BackScreen(
                        FrontendScopeId);

                bool backSettled =
                    await WaitForM302ScreenHandleAsync(
                        back,
                        240);

                bool pass =
                    pushSettled &&
                    backSettled &&
                    push != null &&
                    push.IsCompleted &&
                    push.Result.Status ==
                        UIScreenOperationStatus.Succeeded &&
                    back != null &&
                    back.IsCompleted &&
                    back.Result.Status ==
                        UIScreenOperationStatus.Succeeded &&
                    string.Equals(
                        root.GetCurrentScreenId(
                            FrontendScopeId),
                        MainMenuId,
                        StringComparison.Ordinal) &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 1 " +
                    (pass ? "PASS" : "FAIL") +
                    " root-default Immediate Screen lifecycle. push=" +
                    ScreenHandleSettlement(
                        push) +
                    ", back=" +
                    ScreenHandleSettlement(
                        back) +
                    ", final=" +
                    root.GetCurrentScreenId(
                        FrontendScopeId) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 1",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check2()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                EnsureExternalRegistered();

                UIScreenHandle reset =
                    root.ResetScreen(
                        MainMenuId);

                await WaitForM302ScreenHandleAsync(
                    reset,
                    240);

                double started =
                    Time.realtimeSinceStartupAsDouble;

                UIScreenHandle push =
                    root.PushScreen(
                        ExternalOwnedId);

                bool pushSettled =
                    await WaitForM302ScreenHandleAsync(
                        push,
                        360);

                double enterElapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                await WaitM302FramesAsync(
                    18);

                started =
                    Time.realtimeSinceStartupAsDouble;

                UIScreenHandle back =
                    root.BackScreen(
                        FrontendScopeId);

                bool backSettled =
                    await WaitForM302ScreenHandleAsync(
                        back,
                        360);

                double exitElapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                bool pass =
                    pushSettled &&
                    backSettled &&
                    push != null &&
                    push.IsCompleted &&
                    push.Result.Status ==
                        UIScreenOperationStatus.Succeeded &&
                    back != null &&
                    back.IsCompleted &&
                    back.Result.Status ==
                        UIScreenOperationStatus.Succeeded &&
                    enterElapsed >= 0.30d &&
                    exitElapsed >= 0.20d &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 2 " +
                    (pass ? "PASS" : "FAIL") +
                    " ExternalOwned Screen definition fade. enterElapsed=" +
                    enterElapsed.ToString("0.000") +
                    "s, exitElapsed=" +
                    exitElapsed.ToString("0.000") +
                    "s, externalAlpha=" +
                    CanvasAlpha(
                        externalOwnedView) +
                    ", final=" +
                    root.GetCurrentScreenId(
                        FrontendScopeId) +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 2",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check3()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.CloseSurface(
                    DefaultWindowId);

                UISurfaceOperationResult open =
                    await root.OpenSurfaceAsync(
                        DefaultWindowId,
                        m302FadeProfile);

                double openAlpha =
                    NumericCanvasAlpha(
                        defaultWindow);

                await WaitM302FramesAsync(
                    18);

                UISurfaceOperationResult close =
                    await root.CloseSurfaceAsync(
                        DefaultWindowId,
                        m302FadeProfile);

                bool pass =
                    open.Succeeded &&
                    close.Succeeded &&
                    openAlpha >= 0.99d &&
                    defaultWindow != null &&
                    !defaultWindow.IsVisible &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 3 " +
                    (pass ? "PASS" : "FAIL") +
                    " transient Window fade. open=" +
                    open.Status +
                    ", close=" +
                    close.Status +
                    ", openAlpha=" +
                    openAlpha.ToString("0.00") +
                    ", finalVisible=" +
                    YesNo(
                        defaultWindow != null &&
                        defaultWindow.IsVisible) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 3",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check4()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                if (!modalReady)
                {
                    InitializeModalProof(
                        UIModalScreenMutationPolicy.Reject);
                }

                EnsureExternalModalRegistered();

                UIModalHandle handle =
                    root.OpenModal(
                        ExternalModalId);

                externalModalHandle =
                    handle;

                bool entered =
                    await WaitForM302ConditionAsync(
                        () =>
                            handle == null ||
                            handle.IsCompleted ||
                            (externalModalView != null &&
                             externalModalView.IsInteractable),
                        360);

                if (!entered ||
                    handle == null ||
                    handle.IsCompleted)
                {
                    m302Observed =
                        "CHECK 4 FAIL ExternalOwned Modal did not reach interactive post-enter state. handle=" +
                        ModalHandleSettlement(
                            handle) +
                        ".";
                    return;
                }

                UIModalCompletionAttemptResult first =
                    root.CompleteModal(
                        handle,
                        ConfirmResultId);

                bool lowerBlockedDuringExit =
                    mainMenu != null &&
                    !mainMenu.IsInteractable;

                bool pendingAfterClaim =
                    !handle.IsCompleted;

                UIModalCompletionAttemptResult second =
                    root.CompleteModal(
                        handle,
                        CancelResultId);

                bool settled =
                    await WaitForM302ConditionAsync(
                        () =>
                            handle.IsCompleted,
                        360);

                bool pass =
                    settled &&
                    first.Status ==
                        UIModalCompletionStatus.Succeeded &&
                    second.Status ==
                        UIModalCompletionStatus.AlreadyCompleted &&
                    pendingAfterClaim &&
                    lowerBlockedDuringExit &&
                    handle.Result.Outcome ==
                        UIModalOutcome.Completed &&
                    handle.Result.ResultId.Value ==
                        ConfirmResultId &&
                    root.ActiveModalCount == 0 &&
                    mainMenu != null &&
                    mainMenu.IsInteractable;

                m302Observed =
                    "CHECK 4 " +
                    (pass ? "PASS" : "FAIL") +
                    " Modal exact-once across fade exit. first=" +
                    first.Status +
                    ", second=" +
                    second.Status +
                    ", pendingAfterFirst=" +
                    YesNo(
                        pendingAfterClaim) +
                    ", lowerBlockedDuringExit=" +
                    YesNo(
                        lowerBlockedDuringExit) +
                    ", terminal=" +
                    (handle.IsCompleted
                        ? handle.Result.Outcome +
                          "/" +
                          handle.Result.ResultId.Value
                        : "<pending>") +
                    ", activeModals=" +
                    root.ActiveModalCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 4",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private void RunM302Check5()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            UITransitionResolvedPolicy rootDefault =
                root.ResolveTransitionPolicy(
                    DefaultWindowId,
                    UITransitionDirection.Enter);

            UITransitionResolvedPolicy transient =
                root.ResolveTransitionPolicy(
                    DefaultWindowId,
                    UITransitionDirection.Enter,
                    m302FadeProfile);

            bool pass =
                rootDefault != null &&
                transient != null &&
                rootDefault.DriverId ==
                    UITransitionDriverIds.Immediate &&
                transient.DriverId ==
                    UITransitionDriverIds.CanvasGroupFade &&
                m302FadeProfile.EnterDriverId ==
                    UITransitionDriverIds.CanvasGroupFade;

            m302Observed =
                "CHECK 5 " +
                (pass ? "PASS" : "FAIL") +
                " policy layers. root/default-window driver=" +
                (rootDefault == null
                    ? "<missing>"
                    : rootDefault.DriverId) +
                ", ExternalOwned Screen definition profile=" +
                m302FadeProfile.ProfileId +
                "/" +
                m302FadeProfile.EnterDriverId +
                ", transient/default-window driver=" +
                (transient == null
                    ? "<missing>"
                    : transient.DriverId) +
                ". Runtime override did not mutate authored sample assets.";
        }

        private async void RunM302Check6()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.CloseSurface(
                    DefaultWindowId);

                UISurfaceOperationResult result =
                    await root.OpenSurfaceAsync(
                        DefaultWindowId,
                        m302FailureProfile);

                bool pass =
                    !result.Succeeded &&
                    result.Status ==
                        UISurfaceOperationStatus.TransitionFailed &&
                    defaultWindow != null &&
                    !defaultWindow.IsVisible &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 6 " +
                    (pass ? "PASS" : "FAIL") +
                    " enter failure rollback. result=" +
                    result.Status +
                    ", visibleAfter=" +
                    YesNo(
                        defaultWindow != null &&
                        defaultWindow.IsVisible) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ", message=" +
                    result.Message +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 6",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check7()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.OpenSurface(
                    DefaultWindowId);

                UISurfaceOperationResult result =
                    await root.CloseSurfaceAsync(
                        DefaultWindowId,
                        m302FailureProfile);

                bool pass =
                    !result.Succeeded &&
                    result.Status ==
                        UISurfaceOperationStatus.TransitionFailed &&
                    defaultWindow != null &&
                    !defaultWindow.IsVisible &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 7 " +
                    (pass ? "PASS" : "FAIL") +
                    " exit failure force-close. result=" +
                    result.Status +
                    ", visibleAfter=" +
                    YesNo(
                        defaultWindow != null &&
                        defaultWindow.IsVisible) +
                    ", alphaAfter=" +
                    CanvasAlpha(
                        defaultWindow) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 7",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check8()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                m302NeverDriver?.Clear();

                root.CloseSurface(
                    DefaultWindowId);

                double started =
                    Time.realtimeSinceStartupAsDouble;

                UISurfaceOperationResult result =
                    await root.OpenSurfaceAsync(
                        DefaultWindowId,
                        m302TimeoutProfile);

                double elapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                bool pass =
                    !result.Succeeded &&
                    result.Status ==
                        UISurfaceOperationStatus.TransitionFailed &&
                    elapsed >= 0.15d &&
                    defaultWindow != null &&
                    !defaultWindow.IsVisible &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 8 " +
                    (pass ? "PASS" : "FAIL") +
                    " hard timeout recovery. result=" +
                    result.Status +
                    ", elapsed=" +
                    elapsed.ToString("0.000") +
                    "s, visibleAfter=" +
                    YesNo(
                        defaultWindow != null &&
                        defaultWindow.IsVisible) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ", message=" +
                    result.Message +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 8",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void RunM302Check9()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.OpenSurface(
                    DefaultWindowId);

                m302StaleFirstDone =
                    false;

                BeginM302SlowTransition();

                await Awaitable.NextFrameAsync();

                UITransitionResult second =
                    await root.RunSurfaceTransitionAsync(
                        DefaultWindowId,
                        UITransitionDirection.Enter,
                        m302ImmediateProfile);

                bool firstSettled =
                    await WaitForM302ConditionAsync(
                        () =>
                            m302StaleFirstDone,
                        240);

                bool pass =
                    firstSettled &&
                    m302StaleFirstResult.Status ==
                        UITransitionStatus.Stale &&
                    second.Status ==
                        UITransitionStatus.Completed &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 9 " +
                    (pass ? "PASS" : "FAIL") +
                    " stale completion rejection. first=" +
                    (firstSettled
                        ? m302StaleFirstResult.Status.ToString()
                        : "<pending>") +
                    ", second=" +
                    second.Status +
                    ", finalAlpha=" +
                    CanvasAlpha(
                        defaultWindow) +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 9",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private async void BeginM302SlowTransition()
        {
            try
            {
                m302StaleFirstResult =
                    await root.RunSurfaceTransitionAsync(
                        DefaultWindowId,
                        UITransitionDirection.Enter,
                        m302SlowFadeProfile);
            }
            catch (Exception exception)
            {
                m302StaleFirstResult =
                    new UITransitionResult(
                        UITransitionStatus.Failed,
                        default(UITransitionOperationId),
                        0,
                        DefaultWindowId,
                        UITransitionDirection.Enter,
                        UITransitionDriverIds.CanvasGroupFade,
                        m302SlowFadeProfile == null
                            ? string.Empty
                            : m302SlowFadeProfile.ProfileId,
                        0d,
                        exception.Message);
            }
            finally
            {
                m302StaleFirstDone =
                    true;
            }
        }

        private async void RunM302Check10()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.OpenSurface(
                    DefaultWindowId);

                root.SetReducedMotionTransitions(
                    true);

                UITransitionResult result =
                    await root.RunSurfaceTransitionAsync(
                        DefaultWindowId,
                        UITransitionDirection.Enter,
                        m302FadeProfile);

                bool pass =
                    result.Succeeded &&
                    result.DriverId ==
                        UITransitionDriverIds.Immediate &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 10 " +
                    (pass ? "PASS" : "FAIL") +
                    " reduced-motion substitution. requested=" +
                    UITransitionDriverIds.CanvasGroupFade +
                    ", effectiveDriver=" +
                    result.DriverId +
                    ", profile=" +
                    result.ProfileId +
                    ", elapsed=" +
                    result.ElapsedSeconds.ToString("0.000") +
                    "s.";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 10",
                    exception);
            }
            finally
            {
                root.SetReducedMotionTransitions(
                    false);
                m302Busy = false;
            }
        }

        private async void RunM302Check11()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            float previousTimeScale =
                Time.timeScale;

            try
            {
                root.CloseSurface(
                    DefaultWindowId);

                Time.timeScale =
                    0f;

                double started =
                    Time.realtimeSinceStartupAsDouble;

                UISurfaceOperationResult open =
                    await root.OpenSurfaceAsync(
                        DefaultWindowId,
                        m302FadeProfile);

                double openElapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                UISurfaceOperationResult close =
                    await root.CloseSurfaceAsync(
                        DefaultWindowId,
                        m302FadeProfile);

                bool pass =
                    open.Succeeded &&
                    close.Succeeded &&
                    openElapsed >= 0.30d &&
                    Time.timeScale == 0f &&
                    root.ActiveTransitionCount == 0;

                m302Observed =
                    "CHECK 11 " +
                    (pass ? "PASS" : "FAIL") +
                    " unscaled fade while Time.timeScale=0. open=" +
                    open.Status +
                    ", close=" +
                    close.Status +
                    ", realtimeOpenElapsed=" +
                    openElapsed.ToString("0.000") +
                    "s, timeScaleDuring=" +
                    Time.timeScale.ToString("0.0") +
                    ", activeTransitions=" +
                    root.ActiveTransitionCount +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 11",
                    exception);
            }
            finally
            {
                Time.timeScale =
                    previousTimeScale;
                m302Busy = false;
            }
        }

        private async void RunM302Check12()
        {
            if (!CanRunM302Check())
            {
                return;
            }

            m302Busy = true;

            try
            {
                root.CloseSurface(
                    DefaultWindowId);

                root.SetInputModality(
                    UIInputModality.Navigation);

                ClearSelection();

                UISurfaceOperationResult open =
                    await root.OpenSurfaceAsync(
                        DefaultWindowId,
                        m302FadeProfile);

                string selected =
                    SelectedName();

                bool pass =
                    open.Succeeded &&
                    string.Equals(
                        selected,
                        ObjectName(
                            defaultWindowCloseButton),
                        StringComparison.Ordinal);

                await root.CloseSurfaceAsync(
                    DefaultWindowId,
                    m302FadeProfile);

                m302Observed =
                    "CHECK 12 " +
                    (pass ? "PASS" : "FAIL") +
                    " retained M3-01 focus after transition. open=" +
                    open.Status +
                    ", selectedAfterEnter=" +
                    selected +
                    ", expected=" +
                    ObjectName(
                        defaultWindowCloseButton) +
                    ", focusGeneration=" +
                    root.FocusGeneration +
                    ".";
            }
            catch (Exception exception)
            {
                SetM302Exception(
                    "CHECK 12",
                    exception);
            }
            finally
            {
                m302Busy = false;
            }
        }

        private IEnumerator RunM302Check13()
        {
            if (!CanRunM302Check())
            {
                yield break;
            }

            m302Busy = true;

            int activeStart =
                root.ActiveTransitionCount;
            int queueStart =
                root.ScreenOperationQueueDepth;
            int deferredStart =
                root.DeferredScreenOperationQueueDepth;
            long focusStart =
                root.FocusGeneration;
            int maxActive =
                activeStart;
            int maxQueue =
                queueStart;
            int maxDeferred =
                deferredStart;
            double started =
                Time.realtimeSinceStartupAsDouble;

            const int frames =
                180;

            for (int frame = 0;
                 frame < frames;
                 frame++)
            {
                maxActive =
                    Mathf.Max(
                        maxActive,
                        root.ActiveTransitionCount);

                maxQueue =
                    Mathf.Max(
                        maxQueue,
                        root.ScreenOperationQueueDepth);

                maxDeferred =
                    Mathf.Max(
                        maxDeferred,
                        root.DeferredScreenOperationQueueDepth);

                yield return null;
            }

            double elapsed =
                Time.realtimeSinceStartupAsDouble -
                started;

            bool pass =
                activeStart == 0 &&
                queueStart == 0 &&
                deferredStart == 0 &&
                maxActive == 0 &&
                maxQueue == 0 &&
                maxDeferred == 0 &&
                root.ActiveTransitionCount == 0 &&
                root.ScreenOperationQueueDepth == 0 &&
                root.DeferredScreenOperationQueueDepth == 0;

            m302PerformanceEvidence =
                (pass ? "PASS" : "FAIL") +
                " 180 idle frames / " +
                elapsed.ToString("0.000") +
                "s, active max=" +
                maxActive +
                ", screenQueue max=" +
                maxQueue +
                ", deferredQueue max=" +
                maxDeferred +
                ", focusGeneration " +
                focusStart +
                " -> " +
                root.FocusGeneration +
                ".";

            m302Observed =
                "CHECK 13 " +
                m302PerformanceEvidence;

            m302Busy = false;
        }

        private bool CanRunM302Check()
        {
            if (m302Busy)
            {
                return false;
            }

            if (root == null ||
                !root.IsInitialized ||
                !root.IsTransitionLifecycleInitialized ||
                !m2Ready ||
                m302FadeProfile == null)
            {
                m302Observed =
                    "M3-02 proof is not ready. Click Prepare M3-02 Baseline first.";
                return false;
            }

            return true;
        }

        private static async Awaitable<bool> WaitForM302ScreenHandleAsync(
            UIScreenHandle handle,
            int frameLimit)
        {
            if (handle == null)
            {
                return false;
            }

            for (int frame = 0;
                 frame < frameLimit;
                 frame++)
            {
                if (handle.IsCompleted)
                {
                    return true;
                }

                await Awaitable.NextFrameAsync();
            }

            return handle.IsCompleted;
        }

        private static async Awaitable<bool> WaitForM302ConditionAsync(
            Func<bool> predicate,
            int frameLimit)
        {
            if (predicate == null)
            {
                return false;
            }

            for (int frame = 0;
                 frame < frameLimit;
                 frame++)
            {
                if (predicate())
                {
                    return true;
                }

                await Awaitable.NextFrameAsync();
            }

            return predicate();
        }

        private static async Awaitable WaitM302FramesAsync(
            int frameCount)
        {
            for (int frame = 0;
                 frame < frameCount;
                 frame++)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private void SetM302Exception(
            string check,
            Exception exception)
        {
            Debug.LogException(
                exception,
                this);

            m302Observed =
                check +
                " ERROR: " +
                exception.GetType().Name +
                " | " +
                exception.Message +
                " | Full stack trace logged to the Unity Console.";
        }

        private static string ModalHandleSettlement(
            UIModalHandle handle)
        {
            if (handle == null)
            {
                return "<no handle>";
            }

            if (!handle.IsCompleted)
            {
                return "accepted=" +
                    handle.Accepted +
                    ", pending gen=" +
                    handle.Generation;
            }

            return "accepted=" +
                handle.Accepted +
                ", outcome=" +
                handle.Result.Outcome +
                (handle.Result.IsSemanticCompletion
                    ? ", resultId=" +
                      handle.Result.ResultId.Value
                    : ", abort=" +
                      handle.Result.AbortReason);
        }

        private static string SurfaceStateWithAlpha(
            UISurface surface)
        {
            return SurfaceState(
                       surface) +
                   ", alpha=" +
                   CanvasAlpha(
                       surface);
        }

        private static string CanvasAlpha(
            UISurface surface)
        {
            if (surface == null)
            {
                return "<missing>";
            }

            CanvasGroup group =
                surface.GetComponent<CanvasGroup>();

            return group == null
                ? "<no CanvasGroup>"
                : group.alpha.ToString("0.00");
        }

        private static double NumericCanvasAlpha(
            UISurface surface)
        {
            if (surface == null)
            {
                return -1d;
            }

            CanvasGroup group =
                surface.GetComponent<CanvasGroup>();

            return group == null
                ? -1d
                : group.alpha;
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
                                CancelResultId),
                        transitionProfile: m302FadeProfile)
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
                        allowClose: true,
                        transitionProfile: m302FadeProfile)
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
