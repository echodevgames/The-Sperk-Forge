//----- EchoLaunchRootSplashLifecycleTests.cs START -----

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class FLM404ManualClock :
        ILaunchClock
    {
        internal FLM404ManualClock(
            double initialSeconds = 0d,
            double secondsPerTick = 0.25d)
        {
            CurrentSeconds =
                initialSeconds;

            SecondsPerTick =
                secondsPerTick;
        }

        internal double CurrentSeconds
        {
            get;
            set;
        }

        internal double SecondsPerTick
        {
            get;
            set;
        }

        internal int TickCount
        {
            get;
            private set;
        }

        public double NowSeconds =>
            CurrentSeconds;

#pragma warning disable CS1998
        public async Awaitable NextTickAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            TickCount++;

            CurrentSeconds +=
                SecondsPerTick;
        }
#pragma warning restore CS1998
    }

    internal sealed class FLM404Presenter :
        ILaunchStatusPresenter,
        IImageSplashPresenter
    {
        internal readonly List<
            SplashPresentationFrame> Frames =
                new List<
                    SplashPresentationFrame>();

        internal readonly List<string> Order =
            new List<string>();

        internal bool ThrowOnSplashFrame
        {
            get;
            set;
        }

        internal bool RequestSkipWhenAllowed
        {
            get;
            set;
        }

        internal Action<SplashPresentationFrame>
            FrameObserved
        {
            get;
            set;
        }

        internal int BindCount
        {
            get;
            private set;
        }

        internal int ClearCount
        {
            get;
            private set;
        }

        internal int TerminalCount
        {
            get;
            private set;
        }

        public event Action SkipRequested;

        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
            BindCount++;
        }

        public void Present(
            LaunchProgressSnapshot snapshot)
        {
        }

        public void PresentTerminal(
            LaunchReport report)
        {
            TerminalCount++;
        }

        public void Unbind()
        {
        }

        public void PresentSplash(
            SplashPresentationFrame frame)
        {
            if (ThrowOnSplashFrame)
            {
                throw new InvalidOperationException(
                    "Controlled splash presenter failure.");
            }

            Frames.Add(frame);
            Order.Add("splash");

            FrameObserved?.Invoke(frame);

            if (RequestSkipWhenAllowed &&
                frame.CanSkipNow)
            {
                SkipRequested?.Invoke();
            }
        }

        public void ClearSplash()
        {
            ClearCount++;
            Order.Add("splash-clear");
        }
    }

    internal sealed class FLM404StatusOnlyPresenter :
        ILaunchStatusPresenter
    {
        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
        }

        public void Present(
            LaunchProgressSnapshot snapshot)
        {
        }

        public void PresentTerminal(
            LaunchReport report)
        {
        }

        public void Unbind()
        {
        }
    }

    internal sealed class FLM404StepDefinition :
        StartupStepDefinition
    {
        internal List<string> Order
        {
            get;
            set;
        }

        internal int FactoryCallCount
        {
            get;
            private set;
        }

        internal int ExecutionCallCount
        {
            get;
            private set;
        }

        public override IStartupStepExecutor
            CreateExecutor()
        {
            FactoryCallCount++;

            return new FLM404StepExecutor(
                this);
        }

        internal void RecordExecution()
        {
            ExecutionCallCount++;
            Order?.Add("step");
        }
    }

    internal sealed class FLM404StepExecutor :
        IStartupStepExecutor
    {
        private readonly FLM404StepDefinition
            definition;

        internal FLM404StepExecutor(
            FLM404StepDefinition definition)
        {
            this.definition =
                definition ??
                throw new ArgumentNullException(
                    nameof(definition));
        }

#pragma warning disable CS1998
        public async Awaitable<StartupStepResult>
            ExecuteAsync(
                StartupStepContext context)
        {
            context.CancellationToken
                .ThrowIfCancellationRequested();

            definition.RecordExecution();

            return StartupStepResult.Success(
                "FL-M4-04 startup step completed.");
        }
#pragma warning restore CS1998
    }

    internal sealed class FLM404DestinationLoader :
        IInitialDestinationLoader,
        IInitialDestinationPreflightValidator
    {
        internal List<string> Order
        {
            get;
            set;
        }

        internal int ValidationCallCount
        {
            get;
            private set;
        }

        internal int LoadCallCount
        {
            get;
            private set;
        }

        public bool TryValidate(
            LaunchDestination destination,
            out string failureMessage)
        {
            ValidationCallCount++;
            failureMessage =
                string.Empty;
            return true;
        }

#pragma warning disable CS1998
        public async Awaitable<
            InitialDestinationLoadResult>
            LoadAsync(
                LaunchDestination destination,
                IProgress<float> progress,
                CancellationToken cancellationToken)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            LoadCallCount++;
            Order?.Add("destination");
            progress.Report(1f);

            return InitialDestinationLoadResult
                .Success(
                    destination.DestinationId,
                    "FL-M4-04 destination activated.");
        }
#pragma warning restore CS1998
    }

    public sealed class
        EchoLaunchRootSplashLifecycleTests
    {
        private const int MaximumFramesToWait = 30;

        private const string SequenceId =
            "11111111111111111111111111111111";

        private const string FirstEntryId =
            "22222222222222222222222222222222";

        private static readonly FieldInfo
            RootConfigurationField =
                typeof(EchoLaunchRoot).GetField(
                    "configuration",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            RootLaunchModeField =
                typeof(EchoLaunchRoot).GetField(
                    "launchMode",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationSequenceField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "startupSequence",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationDestinationField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "initialDestination",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationSplashField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "splashSequence",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationReducedMotionField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "useReducedMotionForSplash",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationDisplayNameField =
                typeof(LaunchDestination)
                    .GetField(
                        "displayName",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationScenePathField =
                typeof(LaunchDestination)
                    .GetField(
                        "scenePath",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceEntriesField =
                typeof(StartupSequence).GetField(
                    "entries",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryActivationField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "activation",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryDefinitionField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "stepDefinition",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryPolicyField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "policy",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionDisplayNameField =
                typeof(StartupStepDefinition)
                    .GetField(
                        "displayName",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private readonly List<GameObject>
            createdObjects =
                new List<GameObject>();

        private readonly List<Object>
            createdAssets =
                new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                RootConfigurationField,
                Is.Not.Null);

            Assert.That(
                RootLaunchModeField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSequenceField,
                Is.Not.Null);

            Assert.That(
                ConfigurationDestinationField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSplashField,
                Is.Not.Null);

            Assert.That(
                ConfigurationReducedMotionField,
                Is.Not.Null);

            Assert.That(
                DestinationDisplayNameField,
                Is.Not.Null);

            Assert.That(
                DestinationScenePathField,
                Is.Not.Null);

            Assert.That(
                SequenceEntriesField,
                Is.Not.Null);

            Assert.That(
                EntryActivationField,
                Is.Not.Null);

            Assert.That(
                EntryDefinitionField,
                Is.Not.Null);

            Assert.That(
                EntryPolicyField,
                Is.Not.Null);

            Assert.That(
                DefinitionDisplayNameField,
                Is.Not.Null);

            LaunchAuthorityClaim.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            for (int index =
                     createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target =
                    createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(
                        target);
                }
            }

            createdObjects.Clear();

            for (int index =
                     createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object asset =
                    createdAssets[index];

                if (asset != null)
                {
                    Object.DestroyImmediate(
                        asset);
                }
            }

            createdAssets.Clear();

            LaunchAuthorityClaim.Reset();

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void SplashDiagnosticCodesAreStable()
        {
            Assert.That(
                EchoLaunchRoot
                    .SplashPreflightDiagnosticCode,
                Is.EqualTo(
                    "ELAUNCH-SPLASH-001"));

            Assert.That(
                EchoLaunchRoot
                    .SplashPlaybackDiagnosticCode,
                Is.EqualTo(
                    "ELAUNCH-SPLASH-002"));

            Assert.That(
                EchoLaunchRoot
                    .SplashPresenterUnavailableDiagnosticCode,
                Is.EqualTo(
                    "ELAUNCH-SPLASH-003"));
        }

        [Test]
        public void ConfigurationExposesAssignedSplashSequence()
        {
            SplashSequence splash =
                CreateSplashSequence();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateStepDefinition(),
                    splash);

            Assert.That(
                configuration.SplashSequence,
                Is.SameAs(splash));
        }

        [Test]
        public void ConfigurationExposesReducedMotionDefault()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateStepDefinition(),
                    CreateSplashSequence(),
                    true);

            Assert.That(
                configuration
                    .UseReducedMotionForSplash,
                Is.True);
        }

        [Test]
        public void RootExposesAssignedSplashSequence()
        {
            SplashSequence splash =
                CreateSplashSequence();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        splash));

            Assert.That(
                root.SplashSequence,
                Is.SameAs(splash));
        }

        [Test]
        public void NullSplashRunsStepAndDestination()
        {
            List<string> order =
                new List<string>();

            FLM404StepDefinition definition =
                CreateStepDefinition(order);

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader
                {
                    Order = order,
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        null),
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.LastSplashPlaybackResult,
                Is.Null);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [Test]
        public void EmptySplashRunsStepAndDestination()
        {
            FLM404StepDefinition definition =
                CreateStepDefinition();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence()),
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.LastSplashPlaybackResult,
                Is.Not.Null);

            Assert.That(
                root.LastSplashPlaybackResult
                    .PresentedEntryCount,
                Is.EqualTo(0));
        }

        [Test]
        public void InvalidStartupSequenceBlocksBeforeSplash()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateStepDefinition(),
                    CreateSplashSequence(
                        CreateSplashEntry(
                            hold: 0.5d)));

            ConfigurationSequenceField.SetValue(
                configuration,
                null);

            EchoLaunchRoot root =
                CreateRoot(
                    configuration,
                    presenter: presenter,
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                presenter.Frames,
                Is.Empty);

            Assert.That(
                loader.ValidationCallCount,
                Is.EqualTo(0));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    StartupSequencePreflight
                        .ConfigurationDiagnosticCode));
        }

        [Test]
        public void InvalidSequenceIdentityBlocksBeforeStepAndDestination()
        {
            SplashSequence splash =
                CreateSplashSequence(
                    CreateSplashEntry());

            splash.SetIdentityForTesting(
                "INVALID",
                SplashSequence
                    .CurrentSchemaVersion);

            AssertSplashPreflightBlocks(splash);
        }

        [Test]
        public void UnsupportedSplashSchemaBlocksBeforeStepAndDestination()
        {
            SplashSequence splash =
                CreateSplashSequence(
                    CreateSplashEntry());

            splash.SetIdentityForTesting(
                SequenceId,
                SplashSequence
                    .CurrentSchemaVersion +
                1);

            AssertSplashPreflightBlocks(splash);
        }

        [Test]
        public void NullSplashEntryBlocksBeforeStepAndDestination()
        {
            SplashSequence splash =
                CreateSplashSequence(
                    (SplashEntry)null);

            AssertSplashPreflightBlocks(splash);
        }

        [Test]
        public void MissingSplashImageBlocksBeforeStepAndDestination()
        {
            SplashEntry entry =
                new SplashEntry(
                    FirstEntryId,
                    null,
                    "Missing Image",
                    0d,
                    0d,
                    0d,
                    0d,
                    SplashSkipPolicy
                        .Disallowed);

            SplashSequence splash =
                CreateSplashSequence(entry);

            AssertSplashPreflightBlocks(splash);
        }

        [Test]
        public void DuplicateSplashEntryIdBlocksBeforeStepAndDestination()
        {
            SplashSequence splash =
                CreateSplashSequence(
                    CreateSplashEntry(
                        FirstEntryId),
                    CreateSplashEntry(
                        FirstEntryId));

            AssertSplashPreflightBlocks(splash);
        }

        [Test]
        public void AssignedSplashPresentsBeforeStep()
        {
            List<string> order =
                new List<string>();

            FLM404Presenter presenter =
                new FLM404Presenter();

            presenter.Order.Clear();

            FLM404StepDefinition definition =
                CreateStepDefinition(order);

            presenter.FrameObserved =
                frame =>
                {
                    if (!order.Contains(
                            "splash"))
                    {
                        order.Add("splash");
                    }
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter: presenter);

            StartImmediate(root);

            Assert.That(
                order.IndexOf("splash"),
                Is.GreaterThanOrEqualTo(0));

            Assert.That(
                order.IndexOf("step"),
                Is.GreaterThan(
                    order.IndexOf("splash")));
        }

        [Test]
        public void SplashClearsBeforeStep()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            FLM404StepDefinition definition =
                CreateStepDefinition(
                    presenter.Order);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter: presenter);

            StartImmediate(root);

            Assert.That(
                presenter.Order.IndexOf(
                    "splash-clear"),
                Is.GreaterThanOrEqualTo(0));

            Assert.That(
                presenter.Order.IndexOf(
                    "step"),
                Is.GreaterThan(
                    presenter.Order.IndexOf(
                        "splash-clear")));
        }

        [Test]
        public void StartupStepCompletesBeforeDestinationLoad()
        {
            List<string> order =
                new List<string>();

            FLM404StepDefinition definition =
                CreateStepDefinition(order);

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader
                {
                    Order = order,
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.25d))),
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                order.IndexOf("step"),
                Is.GreaterThanOrEqualTo(0));

            Assert.That(
                order.IndexOf("destination"),
                Is.GreaterThan(
                    order.IndexOf("step")));
        }

        [Test]
        public void ReducedMotionRemovesFadeFrames()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                fadeIn: 1d,
                                hold: 0.5d,
                                fadeOut: 1d)),
                        true),
                    presenter: presenter);

            StartImmediate(root);

            Assert.That(
                presenter.Frames,
                Is.Not.Empty);

            foreach (
                SplashPresentationFrame frame
                in presenter.Frames)
            {
                Assert.That(
                    frame.ReducedMotion,
                    Is.True);

                Assert.That(
                    frame.Phase,
                    Is.EqualTo(
                        SplashPlaybackPhase
                            .Hold));
            }
        }

        [Test]
        public void MissingVisualPresenterWarnsAndContinuesHeadless()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-SPLASH-003] " +
                "A splash sequence is configured, but the active status presenter does not implement IImageSplashPresenter. " +
                "First Light will preserve authored splash timing through the headless presenter.");

            FLM404StepDefinition definition =
                CreateStepDefinition();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter:
                        new FLM404StatusOnlyPresenter(),
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [Test]
        public void PresenterSkipRequestShortensSplash()
        {
            FLM404Presenter presenter =
                new FLM404Presenter
                {
                    RequestSkipWhenAllowed = true,
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 5d,
                                minimum: 0.5d,
                                skipPolicy:
                                    SplashSkipPolicy
                                        .AfterMinimumDisplay))),
                    presenter: presenter);

            StartImmediate(root);

            Assert.That(
                root.LastSplashPlaybackResult,
                Is.Not.Null);

            Assert.That(
                root.LastSplashPlaybackResult
                    .SkippedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                root.LastSplashPlaybackResult
                    .ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    0.5d));

            Assert.That(
                root.LastSplashPlaybackResult
                    .ElapsedSeconds,
                Is.LessThan(5d));
        }

        [Test]
        public void SuccessfulReportElapsedIncludesSplashTime()
        {
            FLM404ManualClock clock =
                new FLM404ManualClock(
                    0d,
                    0.25d);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 1d))),
                    clock: clock);

            StartImmediate(root);

            Assert.That(
                root.LastReport,
                Is.Not.Null);

            Assert.That(
                root.LastReport.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    1d));
        }

        [Test]
        public void RootRetainsSuccessfulSplashPlaybackResult()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))));

            StartImmediate(root);

            Assert.That(
                root.LastSplashPlaybackResult,
                Is.Not.Null);

            Assert.That(
                root.LastSplashPlaybackResult
                    .PresentedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                root.LastSplashPlaybackResult
                    .SequenceId,
                Is.EqualTo(SequenceId));
        }

        [Test]
        public void PresenterFailureBlocksStepAndDestination()
        {
            FLM404Presenter presenter =
                new FLM404Presenter
                {
                    ThrowOnSplashFrame = true,
                };

            FLM404StepDefinition definition =
                CreateStepDefinition();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter: presenter,
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .SplashPlaybackDiagnosticCode));
        }

        [Test]
        public void CancellationDuringSplashInterruptsExactlyOnce()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            FLM404StepDefinition definition =
                CreateStepDefinition();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 5d))),
                    presenter: presenter,
                    loader: loader);

            int interruptedCount = 0;

            root.LaunchInterrupted +=
                report =>
                {
                    interruptedCount++;
                };

            bool cancellationRequested = false;

            presenter.FrameObserved =
                frame =>
                {
                    if (cancellationRequested)
                    {
                        return;
                    }

                    cancellationRequested = true;

                    root.CancelLaunch(
                        "Cancelled during splash.");
                };

            StartImmediate(root);

            Assert.That(
                interruptedCount,
                Is.EqualTo(1));

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Interrupted));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .LifecycleDiagnosticCode));
        }

        [Test]
        public void DuplicateRootDoesNotPresentAnotherSplash()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateStepDefinition(),
                    CreateSplashSequence(
                        CreateSplashEntry(
                            hold: 0.5d)));

            EchoLaunchRoot first =
                CreateRoot(
                    configuration,
                    presenter: presenter);

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot(
                    configuration,
                    name:
                        "Duplicate FL-M4-04 Root");

            StartImmediate(first);

            Assert.That(
                duplicate.WasRejectedAsDuplicate,
                Is.True);

            Assert.That(
                presenter.Frames,
                Is.Not.Empty);

            Assert.That(
                first.IsAuthoritative,
                Is.True);
        }

        [Test]
        public void DirectSceneModeUsesSameSplashContract()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter: presenter,
                    mode:
                        LaunchMode
                            .DirectSceneDevelopment);

            StartImmediate(root);

            Assert.That(
                presenter.Frames,
                Is.Not.Empty);

            Assert.That(
                root.LastReport.LaunchMode,
                Is.EqualTo(
                    LaunchMode
                        .DirectSceneDevelopment));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [Test]
        public void ConfigurationRemainsImmutableAcrossSplashLaunch()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateStepDefinition(),
                    CreateSplashSequence(
                        CreateSplashEntry(
                            hold: 0.5d)),
                    true);

            string originalId =
                configuration.ConfigurationId;

            int originalSchema =
                configuration.SchemaVersion;

            SplashSequence originalSplash =
                configuration.SplashSequence;

            bool originalReducedMotion =
                configuration
                    .UseReducedMotionForSplash;

            StartImmediate(
                CreateRoot(configuration));

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(originalId));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(originalSchema));

            Assert.That(
                configuration.SplashSequence,
                Is.SameAs(originalSplash));

            Assert.That(
                configuration
                    .UseReducedMotionForSplash,
                Is.EqualTo(
                    originalReducedMotion));
        }

        [Test]
        public void SplashSequenceRemainsImmutableAcrossRootLaunch()
        {
            SplashEntry entry =
                CreateSplashEntry(
                    hold: 0.5d);

            SplashSequence splash =
                CreateSplashSequence(entry);

            string originalSequenceId =
                splash.SequenceId;

            int originalSchema =
                splash.SchemaVersion;

            string originalEntryId =
                entry.EntryId;

            double originalHold =
                entry.HoldSeconds;

            StartImmediate(
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        splash)));

            Assert.That(
                splash.SequenceId,
                Is.EqualTo(
                    originalSequenceId));

            Assert.That(
                splash.SchemaVersion,
                Is.EqualTo(
                    originalSchema));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(
                    originalEntryId));

            Assert.That(
                entry.HoldSeconds,
                Is.EqualTo(
                    originalHold));
        }

        [Test]
        public void LaunchReportSchemaRemainsTwo()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.25d))));

            StartImmediate(root);

            Assert.That(
                LaunchReport.CurrentSchemaVersion,
                Is.EqualTo(2));

            Assert.That(
                root.LastReport.ReportSchemaVersion,
                Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator AutomaticStartUsesSplashPath()
        {
            FLM404Presenter presenter =
                new FLM404Presenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateStepDefinition(),
                        CreateSplashSequence(
                            CreateSplashEntry(
                                hold: 0.5d))),
                    presenter: presenter,
                    automatic: true);

            int waitedFrames = 0;

            while (root.State !=
                       LaunchStatus.Completed &&
                   root.State !=
                       LaunchStatus.Failed &&
                   root.State !=
                       LaunchStatus.Interrupted &&
                   waitedFrames <
                       MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                presenter.Frames,
                Is.Not.Empty);

            Assert.That(
                root.LastSplashPlaybackResult,
                Is.Not.Null);
        }

        private void AssertSplashPreflightBlocks(
            SplashSequence splash)
        {
            FLM404StepDefinition definition =
                CreateStepDefinition();

            FLM404DestinationLoader loader =
                new FLM404DestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        definition,
                        splash),
                    loader: loader);

            StartImmediate(root);

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(0));

            Assert.That(
                loader.ValidationCallCount,
                Is.EqualTo(0));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport,
                Is.Not.Null);

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .SplashPreflightDiagnosticCode));
        }

        private EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            ILaunchStatusPresenter presenter = null,
            FLM404DestinationLoader loader = null,
            FLM404ManualClock clock = null,
            LaunchMode mode =
                LaunchMode.CanonicalBoot,
            bool automatic = false,
            string name =
                "FL-M4-04 Root")
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);

            target.SetActive(false);

            EchoLaunchRoot root =
                target.AddComponent<
                    EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            RootLaunchModeField.SetValue(
                root,
                mode);

            target.SetActive(true);

            if (!root.IsAuthoritative)
            {
                return root;
            }

            if (!automatic)
            {
                root.SetAutomaticStartForTesting(
                    false);
            }

            root.SetLaunchClockForTesting(
                clock ??
                new FLM404ManualClock());

            root.SetInitialDestinationLoaderForTesting(
                loader ??
                new FLM404DestinationLoader());

            root.SetStatusPresenterForTesting(
                presenter ??
                new FLM404Presenter());

            return root;
        }

        private EchoLaunchConfiguration
            CreateConfiguration(
                FLM404StepDefinition definition,
                SplashSequence splash,
                bool reducedMotion = false)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(configuration);

            ConfigurationSequenceField.SetValue(
                configuration,
                CreateStartupSequence(
                    CreateStartupEntry(
                        definition)));

            ConfigurationDestinationField.SetValue(
                configuration,
                CreateDestination());

            ConfigurationSplashField.SetValue(
                configuration,
                splash);

            ConfigurationReducedMotionField.SetValue(
                configuration,
                reducedMotion);

            return configuration;
        }

        private LaunchDestination CreateDestination()
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<
                    LaunchDestination>();

            createdAssets.Add(destination);

            DestinationDisplayNameField.SetValue(
                destination,
                "FL-M4-04 Destination");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/FL-M4-04-Destination.unity");

            return destination;
        }

        private StartupSequence CreateStartupSequence(
            params StartupSequenceEntry[] entries)
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<
                    StartupSequence>();

            createdAssets.Add(sequence);

            SequenceEntriesField.SetValue(
                sequence,
                new List<StartupSequenceEntry>(
                    entries));

            return sequence;
        }

        private FLM404StepDefinition
            CreateStepDefinition(
                List<string> order = null)
        {
            FLM404StepDefinition definition =
                ScriptableObject.CreateInstance<
                    FLM404StepDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                "FL-M4-04 Step");

            definition.Order =
                order;

            return definition;
        }

        private static StartupSequenceEntry
            CreateStartupEntry(
                StartupStepDefinition definition)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                StartupStepPolicy
                    .RequiredBlocking);

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField
                        .FieldType,
                    0));

            return entry;
        }

        private SplashSequence
            CreateSplashSequence(
                params SplashEntry[] entries)
        {
            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(sequence);

            sequence.SetIdentityForTesting(
                SequenceId,
                SplashSequence
                    .CurrentSchemaVersion);

            sequence.SetEntriesForTesting(
                entries);

            return sequence;
        }

        private SplashEntry CreateSplashEntry(
            string entryId = FirstEntryId,
            double fadeIn = 0d,
            double hold = 0d,
            double fadeOut = 0d,
            double minimum = 0d,
            SplashSkipPolicy skipPolicy =
                SplashSkipPolicy
                    .AfterMinimumDisplay)
        {
            return new SplashEntry(
                entryId,
                CreateSprite(),
                "FL-M4-04 Splash",
                fadeIn,
                hold,
                fadeOut,
                minimum,
                skipPolicy);
        }

        private Sprite CreateSprite()
        {
            Texture2D texture =
                new Texture2D(
                    2,
                    2);

            createdAssets.Add(texture);

            Sprite sprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        2f,
                        2f),
                    new Vector2(
                        0.5f,
                        0.5f));

            createdAssets.Add(sprite);

            return sprite;
        }

        private static StartupSequenceRunResult
            StartImmediate(
                EchoLaunchRoot root)
        {
            Awaitable<
                StartupSequenceRunResult>.Awaiter
                    awaiter =
                        root.StartLaunchAsync()
                            .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The deterministic FL-M4-04 root fixture did not settle synchronously.");

            return awaiter.GetResult();
        }

        private static void ExpectDuplicateWarning()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-ROOT-001] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.");
        }
    }
}

//----- EchoLaunchRootSplashLifecycleTests.cs END -----
