using System;
using UnityEngine;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Sample-owned EUI-M4-02 manual proof controller.
    /// It exercises the public notification contract without adding project
    /// presentation policy to the package runtime.
    /// </summary>
    internal sealed class LaboratoryNotificationProof :
        IDisposable
    {
        private readonly struct StructuralSnapshot
        {
            public StructuralSnapshot(
                EchoUIRoot root)
            {
                ScreenId =
                    root.GetCurrentScreenId(
                        FrontendScopeId);

                ModalCount =
                    root.ActiveModalCount;

                WindowVisible =
                    root.IsSurfaceVisible(
                        DefaultWindowId);

                HudRegions =
                    root.HudRegionCount;

                HudWidgets =
                    root.ActiveHudWidgetCount;

                HudLeases =
                    root.ActiveHudVisibilityLeaseCount;

                Transitions =
                    root.ActiveTransitionCount;
            }

            public string ScreenId { get; }

            public int ModalCount { get; }

            public bool WindowVisible { get; }

            public int HudRegions { get; }

            public int HudWidgets { get; }

            public int HudLeases { get; }

            public int Transitions { get; }

            public bool Matches(
                EchoUIRoot root) =>
                string.Equals(
                    ScreenId,
                    root.GetCurrentScreenId(
                        FrontendScopeId),
                    StringComparison.Ordinal) &&
                ModalCount ==
                    root.ActiveModalCount &&
                WindowVisible ==
                    root.IsSurfaceVisible(
                        DefaultWindowId) &&
                HudRegions ==
                    root.HudRegionCount &&
                HudWidgets ==
                    root.ActiveHudWidgetCount &&
                HudLeases ==
                    root.ActiveHudVisibilityLeaseCount &&
                Transitions ==
                    root.ActiveTransitionCount;
        }

        private const string PrimaryChannelId =
            "notification.primary";

        private const string SecondaryChannelId =
            "notification.secondary";

        private const string PriorityChannelId =
            "notification.priority";

        private const string FrontendScopeId =
            "frontend";

        private const string DefaultWindowId =
            "default-window";

        private readonly EchoUIRoot root;

        private readonly LaboratoryNotificationPresenter presenter;

        private GameObject ownerProbe;

        private GameObject presentationProbe;

        private bool ready;

        private bool busy;

        private bool timeScaleOverrideActive;

        private float timeScaleBeforeOverride;

        private string message =
            "M4-02 notification proof infrastructure has not initialized yet.";

        private string observed =
            "<not run>";

        private string performanceEvidence =
            "<not run>";

        public LaboratoryNotificationProof(
            EchoUIRoot root,
            LaboratoryNotificationPresenter presenter)
        {
            this.root = root;
            this.presenter = presenter;
        }

        public void Initialize()
        {
            if (root == null ||
                presenter == null ||
                !root.IsInitialized ||
                !root.IsNotificationLifecycleInitialized)
            {
                message =
                    "M4-02 notification lifecycle or sample presenter is unavailable.";
                return;
            }

            presenter.Initialize(root);

            bool attached =
                root.SetNotificationPresenter(
                    presenter);

            bool definitionsReady =
                DefinitionsReady(
                    out string definitionSummary);

            ready =
                attached &&
                definitionsReady;

            message =
                ready
                    ? "M4-02 proof READY. Click Prepare M4-02 Baseline before a fresh acceptance run."
                    : "M4-02 proof is not ready: " +
                      definitionSummary;
        }

        public void Dispose()
        {
            RestoreTimeScale();
            CleanupOwnedObjects();

            if (root != null)
            {
                root.SetNotificationPresenter(null);
            }

            if (presenter != null)
            {
                presenter.Clear();
            }
        }

        public void DrawConsole()
        {
            GUILayout.Label(
                "EUI-M4-02: bounded notification channels, deterministic priority/FIFO promotion, coalescing, overflow, unscaled/manual lifetime, cleanup, and a replaceable sample presenter.");

            GUILayout.Label(
                "Plain cards render at bottom-center. Looking Glass owns lifecycle truth only; projects own content and final visuals.");

            GUILayout.Space(8f);

            DrawState();

            GUILayout.Space(8f);

            bool priorEnabled =
                GUI.enabled;

            GUI.enabled =
                !busy;

            if (GUILayout.Button(
                    "Prepare M4-02 Baseline"))
            {
                PrepareBaseline();
            }

            GUILayout.Space(10f);
            GUILayout.Label(
                "1. Independent channels, bounded visible state, priority promotion, FIFO ties, and no visible preemption");

            if (GUILayout.Button(
                    "Run Check 1: Channels + Priority/FIFO"))
            {
                RunCheck1();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "2. Visible and pending coalescing, fresh generations, stale handles, and visible lifetime restart");

            if (GUILayout.Button(
                    "Run Check 2: Coalescing + Lifetime Restart"))
            {
                RunCheck2();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "3. RejectNewest, DropOldestPending, and ReplaceLowestPriorityPending without unrelated mutation");

            if (GUILayout.Button(
                    "Run Check 3: Overflow Policies"))
            {
                RunCheck3();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "4. Automatic unscaled expiry while Time.timeScale is zero; manual entry remains until dismissal");

            if (GUILayout.Button(
                    "Run Check 4: Paused Automatic + Manual"))
            {
                RunCheck4();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "5. Owner/presentation loss, stale generation safety, exact reset, and fresh post-reset generation");

            if (GUILayout.Button(
                    "Run Check 5: Loss + Stale + Reset"))
            {
                RunCheck5();
            }

            GUILayout.Space(8f);
            GUILayout.Label(
                "6. 180-frame idle notification/presenter quiescence and unchanged structural UI truth");

            if (GUILayout.Button(
                    "Run Check 6: Idle Notification Probe"))
            {
                RunCheck6();
            }

            GUI.enabled =
                priorEnabled;

            GUILayout.Space(12f);
            GUILayout.Label(
                "7. Retained smoke: visit M4-01 HUD, M3-02 Transitions, M3-01 Focus, M2-02 Modals, M2-01 Screens, and M1 Retained.");

            GUILayout.Label(
                "No M4-02 check may change Screen history, Modal order, Window/HUD state, gameplay input, persistent data, or project domain truth.");

            GUILayout.Space(12f);
            GUILayout.Label(
                "Latest M4-02 observation");

            GUILayout.TextArea(
                observed,
                GUILayout.MinHeight(140f));

            GUILayout.Space(8f);
            GUILayout.Label(
                "Performance evidence: " +
                performanceEvidence);
        }

        private void DrawState()
        {
            GUILayout.Label(
                "Notification lifecycle initialized: " +
                root.IsNotificationLifecycleInitialized);

            GUILayout.Label(
                "Channels / visible / pending: " +
                root.NotificationChannelCount +
                " / " +
                root.VisibleNotificationCount +
                " / " +
                root.PendingNotificationCount);

            GUILayout.Label(
                "notification.primary: " +
                SnapshotSummary(
                    PrimaryChannelId));

            GUILayout.Label(
                "notification.secondary: " +
                SnapshotSummary(
                    SecondaryChannelId));

            GUILayout.Label(
                "notification.priority: " +
                SnapshotSummary(
                    PriorityChannelId));

            GUILayout.Label(
                "Presenter channels / visible / apply count: " +
                (presenter == null
                    ? "0 / 0 / 0"
                    : presenter.ChannelCount +
                      " / " +
                      presenter.TotalVisibleCount +
                      " / " +
                      presenter.ApplyCount));

            GUILayout.Label(
                "Status: " +
                message);

            GUILayout.Label(
                "Busy: " +
                YesNo(
                    busy));
        }

        private async void PrepareBaseline()
        {
            if (busy)
            {
                return;
            }

            busy = true;

            try
            {
                RestoreTimeScale();
                CleanupOwnedObjects();

                int settled =
                    root.ResetNotifications();

                await WaitFramesAsync(3);

                bool definitionsReady =
                    DefinitionsReady(
                        out string definitionSummary);

                bool presenterReady =
                    presenter.HasChannel(
                        PrimaryChannelId) &&
                    presenter.HasChannel(
                        SecondaryChannelId) &&
                    presenter.HasChannel(
                        PriorityChannelId) &&
                    presenter.TotalVisibleCount == 0;

                bool passed =
                    definitionsReady &&
                    presenterReady &&
                    root.VisibleNotificationCount == 0 &&
                    root.PendingNotificationCount == 0;

                presenter.ResetEvidenceCounter();

                observed =
                    (passed
                        ? "M4-02 BASELINE READY"
                        : "M4-02 BASELINE NOT READY") +
                    ". settled=" +
                    settled +
                    ", definitions=" +
                    definitionSummary +
                    ", presenterChannels=" +
                    presenter.ChannelCount +
                    ", presenterVisible=" +
                    presenter.TotalVisibleCount +
                    ", visible/pending=" +
                    root.VisibleNotificationCount +
                    "/" +
                    root.PendingNotificationCount +
                    ".";

                message =
                    passed
                        ? "M4-02 baseline READY."
                        : "M4-02 baseline did not resolve expected empty channel truth.";
            }
            catch (Exception exception)
            {
                SetException(
                    "Prepare baseline",
                    exception);
            }
            finally
            {
                busy = false;
            }
        }

        private void RunCheck1()
        {
            if (!CanRunCheck())
            {
                return;
            }

            root.ResetNotifications();

            StructuralSnapshot structure =
                new StructuralSnapshot(root);

            UINotificationHandle secondary =
                Admit(
                    SecondaryChannelId,
                    "Secondary retained",
                    priority: 50);

            UINotificationHandle visibleA =
                Admit(
                    PrimaryChannelId,
                    "Visible A",
                    priority: 0);

            UINotificationHandle visibleB =
                Admit(
                    PrimaryChannelId,
                    "Visible B",
                    priority: 100);

            UINotificationHandle lower =
                Admit(
                    PrimaryChannelId,
                    "Pending lower",
                    priority: 1);

            UINotificationHandle fifoFirst =
                Admit(
                    PrimaryChannelId,
                    "Pending FIFO first",
                    priority: 10);

            UINotificationHandle fifoSecond =
                Admit(
                    PrimaryChannelId,
                    "Pending FIFO second",
                    priority: 10);

            bool admitted =
                secondary.Accepted &&
                visibleA.Accepted &&
                visibleB.Accepted &&
                lower.Accepted &&
                fifoFirst.Accepted &&
                fifoSecond.Accepted;

            bool noPreemption =
                HasState(
                    visibleA,
                    UINotificationEntryState.Visible) &&
                HasState(
                    visibleB,
                    UINotificationEntryState.Visible) &&
                HasState(
                    lower,
                    UINotificationEntryState.Pending) &&
                HasState(
                    fifoFirst,
                    UINotificationEntryState.Pending) &&
                HasState(
                    fifoSecond,
                    UINotificationEntryState.Pending);

            UINotificationOperationResult firstDismiss =
                root.DismissNotification(
                    visibleA);

            bool firstFifoPromoted =
                HasState(
                    fifoFirst,
                    UINotificationEntryState.Visible) &&
                HasState(
                    fifoSecond,
                    UINotificationEntryState.Pending);

            UINotificationOperationResult secondDismiss =
                root.DismissNotification(
                    visibleB);

            bool secondFifoPromoted =
                HasState(
                    fifoSecond,
                    UINotificationEntryState.Visible) &&
                HasState(
                    lower,
                    UINotificationEntryState.Pending);

            bool independent =
                HasState(
                    secondary,
                    UINotificationEntryState.Visible) &&
                presenter.VisibleCount(
                    SecondaryChannelId) == 1 &&
                presenter.VisibleCount(
                    PrimaryChannelId) == 2;

            bool structuralUnchanged =
                structure.Matches(root);

            bool passed =
                admitted &&
                noPreemption &&
                firstDismiss.Succeeded &&
                firstFifoPromoted &&
                secondDismiss.Succeeded &&
                secondFifoPromoted &&
                independent &&
                structuralUnchanged;

            observed =
                (passed ? "CHECK 1 PASS" : "CHECK 1 FAIL") +
                " channels + priority/FIFO. admitted=" +
                YesNo(
                    admitted) +
                ", noPreemption=" +
                YesNo(
                    noPreemption) +
                ", firstPromotion=" +
                HandleLabel(
                    fifoFirst) +
                ", secondPromotion=" +
                HandleLabel(
                    fifoSecond) +
                ", lower=" +
                HandleLabel(
                    lower) +
                ", secondaryIndependent=" +
                YesNo(
                    independent) +
                ", structuralUnchanged=" +
                YesNo(
                    structuralUnchanged) +
                ".";
        }

        private async void RunCheck2()
        {
            if (!CanRunCheck())
            {
                return;
            }

            busy = true;

            try
            {
                root.ResetNotifications();

                StructuralSnapshot structure =
                    new StructuralSnapshot(root);

                UINotificationHandle visiblePrior =
                    Admit(
                        PrimaryChannelId,
                        "Visible coalesce prior",
                        priority: 4,
                        lifetimeMode:
                            UINotificationLifetimeMode.Automatic,
                        durationSeconds: 0.6f,
                        coalescingKey:
                            "lab.visible");

                await WaitRealtimeSecondsAsync(
                    0.25d);

                UINotificationHandle visibleReplacement =
                    Admit(
                        PrimaryChannelId,
                        "Visible coalesce replacement",
                        priority: 8,
                        lifetimeMode:
                            UINotificationLifetimeMode.Automatic,
                        durationSeconds: 0.6f,
                        coalescingKey:
                            "lab.visible");

                UINotificationOperationResult staleDismiss =
                    root.DismissNotification(
                        visiblePrior);

                UINotificationHandle blocker =
                    Admit(
                        SecondaryChannelId,
                        "Pending blocker");

                UINotificationHandle pendingPrior =
                    Admit(
                        SecondaryChannelId,
                        "Pending coalesce prior",
                        priority: 3,
                        coalescingKey:
                            "lab.pending");

                UINotificationHandle pendingReplacement =
                    Admit(
                        SecondaryChannelId,
                        "Pending coalesce replacement",
                        priority: 9,
                        coalescingKey:
                            "lab.pending");

                bool coalesced =
                    visiblePrior.IsCompleted &&
                    visiblePrior.Result.Outcome ==
                        UINotificationOutcome.Superseded &&
                    visibleReplacement.Admission.Status ==
                        UINotificationAdmissionStatus.Coalesced &&
                    staleDismiss.Status ==
                        UINotificationOperationStatus.Stale &&
                    pendingPrior.IsCompleted &&
                    pendingPrior.Result.Outcome ==
                        UINotificationOutcome.Superseded &&
                    pendingReplacement.Admission.Status ==
                        UINotificationAdmissionStatus.Coalesced &&
                    HasState(
                        pendingReplacement,
                        UINotificationEntryState.Pending);

                UINotificationOperationResult blockerDismiss =
                    root.DismissNotification(
                        blocker);

                bool pendingPromoted =
                    blockerDismiss.Succeeded &&
                    HasState(
                        pendingReplacement,
                        UINotificationEntryState.Visible);

                await WaitRealtimeSecondsAsync(
                    0.3d);

                bool lifetimeRestarted =
                    HasState(
                        visibleReplacement,
                        UINotificationEntryState.Visible) &&
                    presenter.Contains(
                        visibleReplacement);

                await WaitRealtimeSecondsAsync(
                    0.4d);

                bool replacementExpired =
                    visibleReplacement.IsCompleted &&
                    visibleReplacement.Result.Outcome ==
                        UINotificationOutcome.Expired;

                bool structuralUnchanged =
                    structure.Matches(root);

                bool passed =
                    coalesced &&
                    pendingPromoted &&
                    lifetimeRestarted &&
                    replacementExpired &&
                    structuralUnchanged;

                observed =
                    (passed ? "CHECK 2 PASS" : "CHECK 2 FAIL") +
                    " coalescing + lifetime restart. visiblePrior=" +
                    OutcomeLabel(
                        visiblePrior) +
                    ", visibleReplacementAdmission=" +
                    visibleReplacement.Admission.Status +
                    ", staleDismiss=" +
                    staleDismiss.Status +
                    ", pendingPrior=" +
                    OutcomeLabel(
                        pendingPrior) +
                    ", pendingReplacement=" +
                    HandleLabel(
                        pendingReplacement) +
                    ", lifetimeRestarted=" +
                    YesNo(
                        lifetimeRestarted) +
                    ", replacementExpired=" +
                    YesNo(
                        replacementExpired) +
                    ", structuralUnchanged=" +
                    YesNo(
                        structuralUnchanged) +
                    ".";
            }
            catch (Exception exception)
            {
                SetException(
                    "Check 2",
                    exception);
            }
            finally
            {
                busy = false;
            }
        }

        private void RunCheck3()
        {
            if (!CanRunCheck())
            {
                return;
            }

            root.ResetNotifications();

            StructuralSnapshot structure =
                new StructuralSnapshot(root);

            Admit(
                PrimaryChannelId,
                "Reject visible A");

            Admit(
                PrimaryChannelId,
                "Reject visible B");

            Admit(
                PrimaryChannelId,
                "Reject pending A");

            Admit(
                PrimaryChannelId,
                "Reject pending B");

            Admit(
                PrimaryChannelId,
                "Reject pending C");

            UINotificationHandle rejectedNewest =
                Admit(
                    PrimaryChannelId,
                    "Reject newest");

            Admit(
                SecondaryChannelId,
                "Drop visible");

            UINotificationHandle dropVictim =
                Admit(
                    SecondaryChannelId,
                    "Drop oldest pending");

            Admit(
                SecondaryChannelId,
                "Drop retained pending");

            UINotificationHandle dropReplacement =
                Admit(
                    SecondaryChannelId,
                    "Drop replacement");

            Admit(
                PriorityChannelId,
                "Priority visible");

            UINotificationHandle priorityVictim =
                Admit(
                    PriorityChannelId,
                    "Priority low",
                    priority: 1);

            Admit(
                PriorityChannelId,
                "Priority middle",
                priority: 5);

            UINotificationHandle priorityReplacement =
                Admit(
                    PriorityChannelId,
                    "Priority high",
                    priority: 10);

            UINotificationHandle insufficient =
                Admit(
                    PriorityChannelId,
                    "Priority equal-lowest",
                    priority: 5);

            bool rejectPolicy =
                !rejectedNewest.Accepted &&
                rejectedNewest.Admission.Status ==
                    UINotificationAdmissionStatus.CapacityExceeded;

            bool dropPolicy =
                dropReplacement.Accepted &&
                dropVictim.IsCompleted &&
                dropVictim.Result.Outcome ==
                    UINotificationOutcome.OverflowEvicted;

            bool priorityPolicy =
                priorityReplacement.Accepted &&
                priorityVictim.IsCompleted &&
                priorityVictim.Result.Outcome ==
                    UINotificationOutcome.OverflowEvicted &&
                !insufficient.Accepted &&
                insufficient.Admission.Status ==
                    UINotificationAdmissionStatus.InsufficientPriority;

            bool boundsStable =
                SnapshotCounts(
                    PrimaryChannelId,
                    visible: 2,
                    pending: 3) &&
                SnapshotCounts(
                    SecondaryChannelId,
                    visible: 1,
                    pending: 2) &&
                SnapshotCounts(
                    PriorityChannelId,
                    visible: 1,
                    pending: 2);

            bool structuralUnchanged =
                structure.Matches(root);

            bool passed =
                rejectPolicy &&
                dropPolicy &&
                priorityPolicy &&
                boundsStable &&
                structuralUnchanged;

            observed =
                (passed ? "CHECK 3 PASS" : "CHECK 3 FAIL") +
                " overflow policies. rejectNewest=" +
                rejectedNewest.Admission.Status +
                ", dropVictim=" +
                OutcomeLabel(
                    dropVictim) +
                ", dropReplacement=" +
                dropReplacement.Admission.Status +
                ", priorityVictim=" +
                OutcomeLabel(
                    priorityVictim) +
                ", priorityReplacement=" +
                priorityReplacement.Admission.Status +
                ", insufficient=" +
                insufficient.Admission.Status +
                ", boundsStable=" +
                YesNo(
                    boundsStable) +
                ", structuralUnchanged=" +
                YesNo(
                    structuralUnchanged) +
                ".";
        }

        private async void RunCheck4()
        {
            if (!CanRunCheck())
            {
                return;
            }

            busy = true;

            StructuralSnapshot structure =
                default;

            bool paused =
                false;

            bool automaticExpired =
                false;

            bool manualRetained =
                false;

            UINotificationOperationResult manualDismiss =
                default;

            try
            {
                root.ResetNotifications();

                structure =
                    new StructuralSnapshot(root);

                timeScaleBeforeOverride =
                    Time.timeScale;

                timeScaleOverrideActive =
                    true;

                Time.timeScale = 0f;

                paused =
                    Time.timeScale == 0f;

                UINotificationHandle automatic =
                    Admit(
                        PrimaryChannelId,
                        "Automatic while paused",
                        lifetimeMode:
                            UINotificationLifetimeMode.Automatic,
                        durationSeconds: 0.25f);

                UINotificationHandle manual =
                    Admit(
                        SecondaryChannelId,
                        "Manual while paused",
                        lifetimeMode:
                            UINotificationLifetimeMode.Manual);

                await WaitRealtimeSecondsAsync(
                    0.35d);

                automaticExpired =
                    automatic.IsCompleted &&
                    automatic.Result.Outcome ==
                        UINotificationOutcome.Expired;

                manualRetained =
                    HasState(
                        manual,
                        UINotificationEntryState.Visible) &&
                    presenter.Contains(
                        manual);

                manualDismiss =
                    root.DismissNotification(
                        manual);
            }
            catch (Exception exception)
            {
                SetException(
                    "Check 4",
                    exception);

                return;
            }
            finally
            {
                RestoreTimeScale();
                busy = false;
            }

            bool structuralUnchanged =
                structure.Matches(root);

            bool passed =
                paused &&
                automaticExpired &&
                manualRetained &&
                manualDismiss.Succeeded &&
                structuralUnchanged;

            observed =
                (passed ? "CHECK 4 PASS" : "CHECK 4 FAIL") +
                " unscaled automatic + manual lifetime. paused=" +
                YesNo(
                    paused) +
                ", automaticExpired=" +
                YesNo(
                    automaticExpired) +
                ", manualRetained=" +
                YesNo(
                    manualRetained) +
                ", manualDismiss=" +
                manualDismiss.Status +
                ", timeScaleRestored=" +
                Time.timeScale +
                ", structuralUnchanged=" +
                YesNo(
                    structuralUnchanged) +
                ".";
        }

        private async void RunCheck5()
        {
            if (!CanRunCheck())
            {
                return;
            }

            busy = true;

            try
            {
                root.ResetNotifications();
                CleanupOwnedObjects();

                StructuralSnapshot structure =
                    new StructuralSnapshot(root);

                ownerProbe =
                    new GameObject(
                        "M4_02_NotificationOwner");

                UINotificationHandle ownerLost =
                    Admit(
                        PrimaryChannelId,
                        "Owner-bound visible",
                        owner: ownerProbe);

                UINotificationHandle retained =
                    Admit(
                        PrimaryChannelId,
                        "Retained visible");

                UINotificationHandle promoted =
                    Admit(
                        PrimaryChannelId,
                        "Promoted after owner loss",
                        priority: 10);

                presentationProbe =
                    new GameObject(
                        "M4_02_PresentationProbe");

                UINotificationHandle presentationLost =
                    Admit(
                        SecondaryChannelId,
                        "Destroyed presentation",
                        presentation:
                            presentationProbe);

                UnityEngine.Object.Destroy(
                    ownerProbe);

                UnityEngine.Object.Destroy(
                    presentationProbe);

                ownerProbe = null;
                presentationProbe = null;

                await WaitFramesAsync(5);

                bool lossSettled =
                    ownerLost.IsCompleted &&
                    ownerLost.Result.Outcome ==
                        UINotificationOutcome.OwnerLost &&
                    presentationLost.IsCompleted &&
                    presentationLost.Result.Outcome ==
                        UINotificationOutcome.PresentationLost &&
                    HasState(
                        promoted,
                        UINotificationEntryState.Visible);

                UINotificationHandle stalePrior =
                    Admit(
                        PriorityChannelId,
                        "Stale prior",
                        coalescingKey:
                            "lab.stale");

                UINotificationHandle staleReplacement =
                    Admit(
                        PriorityChannelId,
                        "Stale replacement",
                        coalescingKey:
                            "lab.stale");

                UINotificationOperationResult staleDismiss =
                    root.DismissNotification(
                        stalePrior);

                bool replacementSurvived =
                    staleDismiss.Status ==
                        UINotificationOperationStatus.Stale &&
                    presenter.Contains(
                        staleReplacement);

                int resetCount =
                    root.ResetNotifications();

                bool resetSettled =
                    resetCount == 3 &&
                    retained.Result.Outcome ==
                        UINotificationOutcome.Reset &&
                    promoted.Result.Outcome ==
                        UINotificationOutcome.Reset &&
                    staleReplacement.Result.Outcome ==
                        UINotificationOutcome.Reset;

                UINotificationHandle fresh =
                    Admit(
                        PrimaryChannelId,
                        "Fresh after reset");

                bool freshGeneration =
                    fresh.Accepted &&
                    fresh.Generation >
                        staleReplacement.Generation;

                UINotificationOperationResult freshDismiss =
                    root.DismissNotification(
                        fresh);

                bool finalBaseline =
                    freshDismiss.Succeeded &&
                    root.VisibleNotificationCount == 0 &&
                    root.PendingNotificationCount == 0 &&
                    presenter.TotalVisibleCount == 0;

                bool structuralUnchanged =
                    structure.Matches(root);

                bool passed =
                    lossSettled &&
                    replacementSurvived &&
                    resetSettled &&
                    freshGeneration &&
                    finalBaseline &&
                    structuralUnchanged;

                observed =
                    (passed ? "CHECK 5 PASS" : "CHECK 5 FAIL") +
                    " owner/presentation loss + stale/reset. owner=" +
                    OutcomeLabel(
                        ownerLost) +
                    ", presentation=" +
                    OutcomeLabel(
                        presentationLost) +
                    ", promoted=" +
                    HandleLabel(
                        promoted) +
                    ", staleDismiss=" +
                    staleDismiss.Status +
                    ", replacementSurvived=" +
                    YesNo(
                        replacementSurvived) +
                    ", resetCount=" +
                    resetCount +
                    ", freshGeneration=" +
                    YesNo(
                        freshGeneration) +
                    ", finalBaseline=" +
                    YesNo(
                        finalBaseline) +
                    ", structuralUnchanged=" +
                    YesNo(
                        structuralUnchanged) +
                    ".";
            }
            catch (Exception exception)
            {
                SetException(
                    "Check 5",
                    exception);
            }
            finally
            {
                busy = false;
            }
        }

        private async void RunCheck6()
        {
            if (!CanRunCheck())
            {
                return;
            }

            busy = true;

            try
            {
                RestoreTimeScale();
                CleanupOwnedObjects();
                root.ResetNotifications();

                await WaitFramesAsync(3);

                bool primaryBeforeFound =
                    root.TryGetNotificationChannelSnapshot(
                        PrimaryChannelId,
                        out UINotificationChannelSnapshot primaryBefore);

                bool secondaryBeforeFound =
                    root.TryGetNotificationChannelSnapshot(
                        SecondaryChannelId,
                        out UINotificationChannelSnapshot secondaryBefore);

                bool priorityBeforeFound =
                    root.TryGetNotificationChannelSnapshot(
                        PriorityChannelId,
                        out UINotificationChannelSnapshot priorityBefore);

                int channelsBefore =
                    root.NotificationChannelCount;

                int visibleBefore =
                    root.VisibleNotificationCount;

                int pendingBefore =
                    root.PendingNotificationCount;

                int presenterVisibleBefore =
                    presenter.TotalVisibleCount;

                int presenterApplyBefore =
                    presenter.ApplyCount;

                StructuralSnapshot structure =
                    new StructuralSnapshot(root);

                const int sampleFrames = 180;

                await WaitFramesAsync(
                    sampleFrames);

                bool primaryAfterFound =
                    root.TryGetNotificationChannelSnapshot(
                        PrimaryChannelId,
                        out UINotificationChannelSnapshot primaryAfter);

                bool secondaryAfterFound =
                    root.TryGetNotificationChannelSnapshot(
                        SecondaryChannelId,
                        out UINotificationChannelSnapshot secondaryAfter);

                bool priorityAfterFound =
                    root.TryGetNotificationChannelSnapshot(
                        PriorityChannelId,
                        out UINotificationChannelSnapshot priorityAfter);

                bool snapshotsStable =
                    primaryBeforeFound &&
                    primaryAfterFound &&
                    secondaryBeforeFound &&
                    secondaryAfterFound &&
                    priorityBeforeFound &&
                    priorityAfterFound &&
                    SnapshotsEqual(
                        primaryBefore,
                        primaryAfter) &&
                    SnapshotsEqual(
                        secondaryBefore,
                        secondaryAfter) &&
                    SnapshotsEqual(
                        priorityBefore,
                        priorityAfter);

                bool countsStable =
                    channelsBefore ==
                        root.NotificationChannelCount &&
                    visibleBefore ==
                        root.VisibleNotificationCount &&
                    pendingBefore ==
                        root.PendingNotificationCount &&
                    presenterVisibleBefore ==
                        presenter.TotalVisibleCount &&
                    presenterApplyBefore ==
                        presenter.ApplyCount;

                bool structuralUnchanged =
                    structure.Matches(root);

                bool passed =
                    snapshotsStable &&
                    countsStable &&
                    structuralUnchanged;

                performanceEvidence =
                    (passed ? "PASS" : "FAIL") +
                    " 180 idle frames. channels=" +
                    channelsBefore +
                    "->" +
                    root.NotificationChannelCount +
                    ", visible=" +
                    visibleBefore +
                    "->" +
                    root.VisibleNotificationCount +
                    ", pending=" +
                    pendingBefore +
                    "->" +
                    root.PendingNotificationCount +
                    ", presenterVisible=" +
                    presenterVisibleBefore +
                    "->" +
                    presenter.TotalVisibleCount +
                    ", presenterApply=" +
                    presenterApplyBefore +
                    "->" +
                    presenter.ApplyCount +
                    ", snapshotsStable=" +
                    YesNo(
                        snapshotsStable) +
                    ", structuralUnchanged=" +
                    YesNo(
                        structuralUnchanged) +
                    ".";

                observed =
                    (passed ? "CHECK 6 PASS. " : "CHECK 6 FAIL. ") +
                    performanceEvidence;
            }
            catch (Exception exception)
            {
                SetException(
                    "Check 6",
                    exception);
            }
            finally
            {
                busy = false;
            }
        }

        private bool CanRunCheck()
        {
            if (busy)
            {
                return false;
            }

            string summary =
                string.Empty;

            if (!ready ||
                root == null ||
                presenter == null ||
                !root.IsInitialized ||
                !root.IsNotificationLifecycleInitialized ||
                !DefinitionsReady(
                    out summary))
            {
                observed =
                    "M4-02 proof is not ready. Prepare the baseline first. " +
                    summary;

                return false;
            }

            return true;
        }

        private bool DefinitionsReady(
            out string summary)
        {
            bool primaryFound =
                root.TryGetNotificationChannelDefinition(
                    PrimaryChannelId,
                    out UINotificationChannelDefinition primary);

            bool secondaryFound =
                root.TryGetNotificationChannelDefinition(
                    SecondaryChannelId,
                    out UINotificationChannelDefinition secondary);

            bool priorityFound =
                root.TryGetNotificationChannelDefinition(
                    PriorityChannelId,
                    out UINotificationChannelDefinition priority);

            bool valid =
                root.NotificationChannelCount == 3 &&
                primaryFound &&
                secondaryFound &&
                priorityFound &&
                primary.VisibleCapacity == 2 &&
                primary.PendingCapacity == 3 &&
                primary.OverflowPolicy ==
                    UINotificationOverflowPolicy.RejectNewest &&
                secondary.VisibleCapacity == 1 &&
                secondary.PendingCapacity == 2 &&
                secondary.OverflowPolicy ==
                    UINotificationOverflowPolicy.DropOldestPending &&
                priority.VisibleCapacity == 1 &&
                priority.PendingCapacity == 2 &&
                priority.OverflowPolicy ==
                    UINotificationOverflowPolicy.ReplaceLowestPriorityPending &&
                presenter.HasChannel(
                    PrimaryChannelId) &&
                presenter.HasChannel(
                    SecondaryChannelId) &&
                presenter.HasChannel(
                    PriorityChannelId);

            summary =
                "channels=" +
                root.NotificationChannelCount +
                ", primary=" +
                DefinitionSummary(
                    primaryFound,
                    primary) +
                ", secondary=" +
                DefinitionSummary(
                    secondaryFound,
                    secondary) +
                ", priority=" +
                DefinitionSummary(
                    priorityFound,
                    priority) +
                ", presenterChannels=" +
                presenter.ChannelCount;

            return valid;
        }

        private UINotificationHandle Admit(
            string channelId,
            string title,
            int priority = 0,
            UINotificationLifetimeMode lifetimeMode =
                UINotificationLifetimeMode.Manual,
            float durationSeconds = 0f,
            string coalescingKey = "",
            UnityEngine.Object owner = null,
            object presentation = null)
        {
            object payload =
                presentation ??
                new LaboratoryNotificationCard(
                    title,
                    "Laboratory proof payload",
                    ChannelColor(
                        channelId));

            return root.AdmitNotification(
                new UINotificationRequest(
                    channelId,
                    payload,
                    priority,
                    lifetimeMode,
                    durationSeconds,
                    coalescingKey,
                    owner,
                    title));
        }

        private bool HasState(
            UINotificationHandle handle,
            UINotificationEntryState expected) =>
            root.TryGetNotificationEntryState(
                handle,
                out UINotificationEntryState state) &&
            state == expected;

        private bool SnapshotCounts(
            string channelId,
            int visible,
            int pending) =>
            root.TryGetNotificationChannelSnapshot(
                channelId,
                out UINotificationChannelSnapshot snapshot) &&
            snapshot.VisibleCount == visible &&
            snapshot.PendingCount == pending;

        private string SnapshotSummary(
            string channelId)
        {
            if (!root.TryGetNotificationChannelSnapshot(
                    channelId,
                    out UINotificationChannelSnapshot snapshot))
            {
                return "<missing>";
            }

            return
                "visible=" +
                snapshot.VisibleCount +
                "/" +
                snapshot.VisibleCapacity +
                ", pending=" +
                snapshot.PendingCount +
                "/" +
                snapshot.PendingCapacity +
                ", overflow=" +
                snapshot.OverflowPolicy +
                ", presenterVisible=" +
                presenter.VisibleCount(
                    channelId);
        }

        private static string DefinitionSummary(
            bool found,
            UINotificationChannelDefinition definition)
        {
            if (!found ||
                definition == null)
            {
                return "<missing>";
            }

            return
                definition.VisibleCapacity +
                "/" +
                definition.PendingCapacity +
                "/" +
                definition.OverflowPolicy;
        }

        private static string HandleLabel(
            UINotificationHandle handle)
        {
            if (handle == null)
            {
                return "<null>";
            }

            if (handle.IsCompleted)
            {
                return
                    "generation " +
                    handle.Generation +
                    " " +
                    handle.Result.Outcome;
            }

            return
                "generation " +
                handle.Generation +
                " live";
        }

        private static string OutcomeLabel(
            UINotificationHandle handle)
        {
            if (handle == null)
            {
                return "<null>";
            }

            return handle.IsCompleted
                ? handle.Result.Outcome.ToString()
                : "Live";
        }

        private static bool SnapshotsEqual(
            UINotificationChannelSnapshot left,
            UINotificationChannelSnapshot right) =>
            left.ChannelId ==
                right.ChannelId &&
            left.VisibleCapacity ==
                right.VisibleCapacity &&
            left.PendingCapacity ==
                right.PendingCapacity &&
            left.VisibleCount ==
                right.VisibleCount &&
            left.PendingCount ==
                right.PendingCount &&
            left.OverflowPolicy ==
                right.OverflowPolicy;

        private static Color ChannelColor(
            string channelId)
        {
            if (string.Equals(
                    channelId,
                    SecondaryChannelId,
                    StringComparison.Ordinal))
            {
                return new Color32(
                    235,
                    80,
                    200,
                    255);
            }

            if (string.Equals(
                    channelId,
                    PriorityChannelId,
                    StringComparison.Ordinal))
            {
                return new Color32(
                    250,
                    170,
                    45,
                    255);
            }

            return new Color32(
                50,
                205,
                225,
                255);
        }

        private void CleanupOwnedObjects()
        {
            DestroyOwnedObject(
                ref ownerProbe);

            DestroyOwnedObject(
                ref presentationProbe);
        }

        private static void DestroyOwnedObject(
            ref GameObject value)
        {
            if (value != null)
            {
                UnityEngine.Object.Destroy(
                    value);
            }

            value = null;
        }

        private void RestoreTimeScale()
        {
            if (!timeScaleOverrideActive)
            {
                return;
            }

            Time.timeScale =
                timeScaleBeforeOverride;

            timeScaleOverrideActive = false;
        }

        private static async Awaitable WaitFramesAsync(
            int frameCount)
        {
            for (int frame = 0;
                 frame < frameCount;
                 frame++)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        private static async Awaitable WaitRealtimeSecondsAsync(
            double durationSeconds)
        {
            double deadline =
                Time.realtimeSinceStartupAsDouble +
                durationSeconds;

            while (Time.realtimeSinceStartupAsDouble <
                   deadline)
            {
                await Awaitable.NextFrameAsync();
            }

            await Awaitable.NextFrameAsync();
        }

        private void SetException(
            string operation,
            Exception exception)
        {
            observed =
                operation +
                " FAIL with " +
                exception.GetType().Name +
                ": " +
                exception.Message;

            Debug.LogException(exception);
        }

        private static string YesNo(
            bool value) =>
            value
                ? "YES"
                : "NO";
    }
}
