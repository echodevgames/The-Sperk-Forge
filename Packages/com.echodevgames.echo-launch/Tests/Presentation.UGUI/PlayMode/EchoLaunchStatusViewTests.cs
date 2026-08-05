//----- EchoLaunchStatusViewTests.cs START -----

using System;
using System.Reflection;
using EchoDevGames.EchoLaunch;
using EchoDevGames.EchoLaunch.Presentation.UGUI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoLaunch.Tests.Presentation.UGUI
{
    public sealed class EchoLaunchStatusViewTests
    {
        private const string DestinationId =
            "0123456789abcdef0123456789abcdef";

        [Test]
        public void ViewImplementsNeutralPresenterContract()
        {
            using ViewRig rig =
                new ViewRig();

            Assert.That(
                rig.View,
                Is.InstanceOf<
                    ILaunchStatusPresenter>());
        }

        [Test]
        public void BindShowsViewAndAuthorityCopy()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus
                        .AuthorityClaimed,
                    "Authority claimed.",
                    0f,
                    true));

            Assert.That(
                rig.View.IsBound,
                Is.True);

            Assert.That(
                rig.View.IsVisible,
                Is.True);

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Preparing launch."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "Authority claimed."));
        }

        [Test]
        public void DeterminateRunningSnapshotShowsSliderAndPercent()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Loading settings.",
                    0.42f,
                    false));

            Assert.That(
                rig.View
                    .IsShowingDeterminateProgress,
                Is.True);

            Assert.That(
                rig.View
                    .IsShowingIndeterminateProgress,
                Is.False);

            Assert.That(
                rig.Slider.value,
                Is.EqualTo(0.42f)
                    .Within(0.0001f));

            Assert.That(
                rig.ProgressText.text,
                Is.EqualTo("42%"));
        }

        [Test]
        public void IndeterminateRunningSnapshotShowsWorkingSurface()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Connecting services.",
                    0.15f,
                    true));

            Assert.That(
                rig.View
                    .IsShowingDeterminateProgress,
                Is.False);

            Assert.That(
                rig.View
                    .IsShowingIndeterminateProgress,
                Is.True);

            Assert.That(
                rig.ProgressText.text,
                Is.EqualTo("Working..."));
        }

        [Test]
        public void RunningSnapshotShowsStepPositionAndIdentity()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                new LaunchProgressSnapshot(
                    LaunchMode.CanonicalBoot,
                    LaunchStatus.Running,
                    "initialize-settings",
                    1,
                    4,
                    0.5f,
                    false,
                    "Loading settings.",
                    2.25d,
                    null));

            Assert.That(
                rig.StepText.text,
                Is.EqualTo(
                    "Step 2 of 4 - initialize-settings"));

            Assert.That(
                rig.ElapsedText.text,
                Is.EqualTo(
                    "Elapsed 2.3s"));
        }

        [Test]
        public void WarningResultUsesWarningCopyAndDiagnosticDetail()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                new LaunchProgressSnapshot(
                    LaunchMode.CanonicalBoot,
                    LaunchStatus.Running,
                    "optional-service",
                    0,
                    1,
                    1f,
                    false,
                    "Continuing.",
                    1d,
                    StartupStepResult.Warning(
                        "TEST-WARN",
                        "Optional service unavailable.")));

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Continuing with a warning."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "[TEST-WARN] Optional service unavailable."));
        }

        [Test]
        public void TransitioningSnapshotUsesLoadingCopy()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Loading destination.",
                    0.75f,
                    false));

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Loading destination."));
        }

        [Test]
        public void CompletedReportShowsDestinationAndFullProgress()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Activating destination.",
                    0.9f,
                    false));

            LaunchReport report =
                CreateCompletedReport();

            rig.View.PresentTerminal(
                report);

            Assert.That(
                rig.View.LastReport,
                Is.SameAs(report));

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Launch complete."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "Destination activated."));

            Assert.That(
                rig.StepText.text,
                Is.EqualTo(
                    "First Light Destination"));

            Assert.That(
                rig.Slider.value,
                Is.EqualTo(1f));

            Assert.That(
                rig.ProgressText.text,
                Is.EqualTo("100%"));
        }

        [Test]
        public void FailedReportShowsDiagnosticCodeAndMessage()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Starting systems.",
                    0.3f,
                    false));

            rig.View.PresentTerminal(
                CreateFailedReport());

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Launch blocked."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "[TEST-FAIL] Required launch work failed."));

            Assert.That(
                rig.ProgressText.text,
                Is.EqualTo("30%"));
        }

        [Test]
        public void InterruptedReportShowsCancellationMessage()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Loading destination.",
                    0.6f,
                    true));

            rig.View.PresentTerminal(
                CreateInterruptedReport());

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Launch interrupted."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "[TEST-CANCEL] Launch cancelled."));

            Assert.That(
                rig.View
                    .IsShowingIndeterminateProgress,
                Is.True);
        }

        [Test]
        public void PresentBeforeBindDoesNothing()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Present(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Should not render.",
                    0f,
                    true));

            Assert.That(
                rig.View.IsBound,
                Is.False);

            Assert.That(
                rig.StateText.text,
                Is.Empty);

            Assert.That(
                rig.MessageText.text,
                Is.Empty);
        }

        [Test]
        public void TerminalBeforeBindDoesNothing()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.PresentTerminal(
                CreateCompletedReport());

            Assert.That(
                rig.View.LastReport,
                Is.Null);

            Assert.That(
                rig.StateText.text,
                Is.Empty);
        }

        [Test]
        public void NullTerminalReportThrows()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating.",
                    0f,
                    true));

            Assert.Throws<
                ArgumentNullException>(
                () =>
                    rig.View
                        .PresentTerminal(
                            null));
        }

        [Test]
        public void UnbindHidesView()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating.",
                    0f,
                    true));

            rig.View.Unbind();

            Assert.That(
                rig.View.IsBound,
                Is.False);

            Assert.That(
                rig.View.IsVisible,
                Is.False);
        }

        [Test]
        public void ClearOnUnbindClearsRenderedState()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View
                .ConfigureBehaviorForTesting(
                    true,
                    true,
                    true);

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Running,
                    "Running.",
                    0.5f,
                    false));

            rig.View.Unbind();

            Assert.That(
                rig.StateText.text,
                Is.Empty);

            Assert.That(
                rig.MessageText.text,
                Is.Empty);

            Assert.That(
                rig.ProgressText.text,
                Is.Empty);

            Assert.That(
                rig.View.LastSnapshot.Status,
                Is.EqualTo(
                    LaunchStatus.None));
        }

        [Test]
        public void RebindAfterUnbindResetsTerminalReport()
        {
            using ViewRig rig =
                new ViewRig();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Transitioning,
                    "Loading.",
                    0.8f,
                    false));

            rig.View.PresentTerminal(
                CreateCompletedReport());

            rig.View.Unbind();

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Second launch.",
                    0f,
                    true));

            Assert.That(
                rig.View.LastReport,
                Is.Null);

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Validating launch."));
        }

        [Test]
        public void MissingOptionalReferencesRemainSafe()
        {
            GameObject target =
                new GameObject(
                    "Minimal Status View");

            try
            {
                EchoLaunchStatusView view =
                    target.AddComponent<
                        EchoLaunchStatusView>();

                Assert.DoesNotThrow(
                    () =>
                        view.Bind(
                            CreateSnapshot(
                                LaunchStatus
                                    .Validating,
                                "Validating.",
                                0f,
                                true)));

                Assert.DoesNotThrow(
                    () =>
                        view.PresentTerminal(
                            CreateFailedReport()));

                Assert.DoesNotThrow(
                    view.Unbind);
            }
            finally
            {
                UnityEngine.Object
                    .DestroyImmediate(
                        target);
            }
        }

        [Test]
        public void SerializedStateCopyCanBeReplaced()
        {
            using ViewRig rig =
                new ViewRig();

            SetPrivateField(
                rig.View,
                "validatingText",
                "Checking the gate.");

            rig.View.Bind(
                CreateSnapshot(
                    LaunchStatus.Validating,
                    "Validating.",
                    0f,
                    true));

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Checking the gate."));
        }

        private static LaunchProgressSnapshot
            CreateSnapshot(
                LaunchStatus status,
                string message,
                float progress01,
                bool isIndeterminate)
        {
            return new LaunchProgressSnapshot(
                LaunchMode.CanonicalBoot,
                status,
                string.Empty,
                -1,
                0,
                progress01,
                isIndeterminate,
                message,
                0d,
                null);
        }

        private static LaunchReport
            CreateCompletedReport()
        {
            return new LaunchReport(
                LaunchMode.CanonicalBoot,
                "configuration",
                "sequence",
                DestinationId,
                "First Light Destination",
                LaunchStatus.Completed,
                0d,
                2d,
                0,
                0,
                0,
                false,
                StartupStepResult.Success(
                    "Destination activated."),
                Array.Empty<
                    LaunchStepReport>());
        }

        private static LaunchReport
            CreateFailedReport()
        {
            return new LaunchReport(
                LaunchMode.CanonicalBoot,
                "configuration",
                "sequence",
                string.Empty,
                string.Empty,
                LaunchStatus.Failed,
                0d,
                1d,
                0,
                0,
                0,
                false,
                StartupStepResult
                    .BlockingFailure(
                        "TEST-FAIL",
                        "Required launch work failed."),
                Array.Empty<
                    LaunchStepReport>());
        }

        private static LaunchReport
            CreateInterruptedReport()
        {
            return new LaunchReport(
                LaunchMode.CanonicalBoot,
                "configuration",
                "sequence",
                string.Empty,
                string.Empty,
                LaunchStatus.Interrupted,
                0d,
                1d,
                0,
                0,
                0,
                true,
                StartupStepResult.Cancelled(
                    "TEST-CANCEL",
                    "Launch cancelled."),
                Array.Empty<
                    LaunchStepReport>());
        }

        private static void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field =
                target.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                field,
                Is.Not.Null,
                $"Missing field {fieldName}.");

            field.SetValue(
                target,
                value);
        }

        private sealed class ViewRig :
            IDisposable
        {
            public ViewRig()
            {
                Root =
                    new GameObject(
                        "Echo Launch Status View",
                        typeof(RectTransform),
                        typeof(CanvasGroup));

                CanvasGroup =
                    Root.GetComponent<
                        CanvasGroup>();

                View =
                    Root.AddComponent<
                        EchoLaunchStatusView>();

                StateText =
                    CreateText(
                        "State");

                MessageText =
                    CreateText(
                        "Message");

                StepText =
                    CreateText(
                        "Step");

                ProgressText =
                    CreateText(
                        "Progress");

                ElapsedText =
                    CreateText(
                        "Elapsed");

                DeterminateRoot =
                    CreateChild(
                        "Determinate Progress");

                Slider =
                    DeterminateRoot
                        .AddComponent<Slider>();

                IndeterminateRoot =
                    CreateChild(
                        "Indeterminate Progress");

                View.ConfigureForTesting(
                    CanvasGroup,
                    StateText,
                    MessageText,
                    StepText,
                    ProgressText,
                    ElapsedText,
                    Slider,
                    DeterminateRoot,
                    IndeterminateRoot);
            }

            public GameObject Root
            {
                get;
            }

            public CanvasGroup CanvasGroup
            {
                get;
            }

            public EchoLaunchStatusView View
            {
                get;
            }

            public Text StateText
            {
                get;
            }

            public Text MessageText
            {
                get;
            }

            public Text StepText
            {
                get;
            }

            public Text ProgressText
            {
                get;
            }

            public Text ElapsedText
            {
                get;
            }

            public Slider Slider
            {
                get;
            }

            public GameObject DeterminateRoot
            {
                get;
            }

            public GameObject IndeterminateRoot
            {
                get;
            }

            public void Dispose()
            {
                UnityEngine.Object
                    .DestroyImmediate(
                        Root);
            }

            private Text CreateText(
                string objectName)
            {
                GameObject child =
                    CreateChild(
                        objectName);

                return child
                    .AddComponent<Text>();
            }

            private GameObject CreateChild(
                string objectName)
            {
                GameObject child =
                    new GameObject(
                        objectName,
                        typeof(RectTransform));

                child.transform.SetParent(
                    Root.transform,
                    false);

                return child;
            }
        }
    }
}

//----- EchoLaunchStatusViewTests.cs END -----
