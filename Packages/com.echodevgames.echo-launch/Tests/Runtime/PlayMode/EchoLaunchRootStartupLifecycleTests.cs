//----- EchoLaunchRootStartupLifecycleTests.cs START -----

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
    /// <summary>
    /// ScriptableObject definition used only by the FL-M3-06 root-owned
    /// lifecycle proof.
    /// </summary>
    internal sealed class RootLifecycleTestDefinition :
        StartupStepDefinition
    {
        private RootLifecycleTestExecutor lastExecutor;

        internal StartupStepResult ResultToReturn
        {
            get;
            set;
        } = StartupStepResult.Success(
            "Root lifecycle test step completed.");

        internal int FramesToWait
        {
            get;
            set;
        }

        internal int CancellationSettlementDelayFrames
        {
            get;
            set;
        }

        internal bool ReportImmediateProgress
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

        internal int FramesCompleted =>
            lastExecutor == null
                ? 0
                : lastExecutor.FramesCompleted;

        internal bool CancellationObserved =>
            lastExecutor != null &&
            lastExecutor.CancellationObserved;

        internal bool Settled =>
            lastExecutor != null &&
            lastExecutor.Settled;

        public override IStartupStepExecutor
            CreateExecutor()
        {
            FactoryCallCount++;

            lastExecutor =
                new RootLifecycleTestExecutor(
                    this);

            return lastExecutor;
        }

        internal void RecordExecution()
        {
            ExecutionCallCount++;
        }
    }

    /// <summary>
    /// Runtime-only executor used to prove immediate work, progress,
    /// multi-frame work, cooperative cancellation, and delayed settlement.
    /// </summary>
    internal sealed class RootLifecycleTestExecutor :
        IStartupStepExecutor
    {
        private readonly RootLifecycleTestDefinition
            definition;

        internal RootLifecycleTestExecutor(
            RootLifecycleTestDefinition definition)
        {
            this.definition =
                definition ??
                throw new ArgumentNullException(
                    nameof(definition));
        }

        internal int FramesCompleted
        {
            get;
            private set;
        }

        internal bool CancellationObserved
        {
            get;
            private set;
        }

        internal bool Settled
        {
            get;
            private set;
        }

        public async Awaitable<StartupStepResult>
            ExecuteAsync(
                StartupStepContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            definition.RecordExecution();

            try
            {
                if (definition.ReportImmediateProgress)
                {
                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            0.5f,
                            "Halfway through root-owned startup."));
                }

                for (int frameIndex = 0;
                     frameIndex <
                     definition.FramesToWait;
                     frameIndex++)
                {
                    await Awaitable.NextFrameAsync(
                        context.CancellationToken);

                    FramesCompleted++;

                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            (float)FramesCompleted /
                            definition.FramesToWait,
                            $"Root frame {FramesCompleted} of {definition.FramesToWait}."));
                }

                return definition.ResultToReturn;
            }
            catch (OperationCanceledException)
            {
                CancellationObserved = true;

                for (int delayIndex = 0;
                     delayIndex <
                     definition
                         .CancellationSettlementDelayFrames;
                     delayIndex++)
                {
                    await Awaitable.NextFrameAsync(
                        CancellationToken.None);
                }

                throw;
            }
            finally
            {
                Settled = true;
            }
        }
    }

    /// <summary>
    /// FL-M3-06 proof that the authoritative root owns one explicit startup
    /// run, translates runner observations into launch lifecycle snapshots,
    /// cooperatively cancels active work, and completes one injected initial
    /// destination handoff.
    /// </summary>
    public sealed class
        EchoLaunchRootStartupLifecycleTests
    {
        private const int MaximumFramesToWait = 90;

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
            ConfigurationIdField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "configurationId",
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
                ConfigurationIdField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSequenceField,
                Is.Not.Null);

            Assert.That(
                ConfigurationDestinationField,
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
        public void AwakeClaimsAuthorityBeforeAutomaticStart()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence()),
                    LaunchMode.CanonicalBoot,
                    "Explicit Start Root");

            Assert.That(
                root.IsAuthoritative,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                root.IsLaunchActive,
                Is.False);

            Assert.That(
                root.LastSequenceRunResult,
                Is.Null);
        }

        [Test]
        public void EmptySequenceAdvancesToCompleted()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence()));

            StartupSequenceRunResult result =
                StartImmediate(root);

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result.AuthoredEntryCount,
                Is.EqualTo(0));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.Progress.Progress01,
                Is.EqualTo(1f));

            Assert.That(
                root.Progress.ActiveStepIndex,
                Is.EqualTo(-1));

            Assert.That(
                root.LastSequenceRunResult,
                Is.SameAs(result));
        }

        [Test]
        public void SuccessfulStepPublishesApprovedStateOrder()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Ordered Success");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            List<LaunchStatus> states =
                new List<LaunchStatus>();

            root.LaunchStateChanged +=
                change => states.Add(
                    change.CurrentState);

            StartImmediate(root);

            CollectionAssert.AreEqual(
                new[]
                {
                    LaunchStatus.Validating,
                    LaunchStatus.Running,
                    LaunchStatus.Transitioning,
                    LaunchStatus.Completed
                },
                states);
        }

        [Test]
        public void RootPublishesStepStartProgressAndCompletion()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Progress Step");

            definition.ReportImmediateProgress =
                true;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            List<LaunchProgressSnapshot> snapshots =
                new List<LaunchProgressSnapshot>();

            root.LaunchProgressChanged +=
                change => snapshots.Add(
                    change.Current);

            StartImmediate(root);

            Assert.That(
                snapshots.Exists(
                    snapshot =>
                        snapshot.Status ==
                            LaunchStatus.Running &&
                        snapshot.ActiveStepId ==
                            definition.StepId &&
                        snapshot.IsProgressIndeterminate),
                Is.True,
                "The root must publish a step-start snapshot before executor progress.");

            Assert.That(
                snapshots.Exists(
                    snapshot =>
                        snapshot.Status ==
                            LaunchStatus.Running &&
                        snapshot.ActiveStepId ==
                            definition.StepId &&
                        snapshot.Progress01 == 0.5f &&
                        snapshot.Message ==
                            "Halfway through root-owned startup."),
                Is.True);

            Assert.That(
                snapshots.Exists(
                    snapshot =>
                        snapshot.Status ==
                            LaunchStatus.Running &&
                        snapshot.ActiveStepId ==
                            definition.StepId &&
                        snapshot.Progress01 == 1f &&
                        snapshot.LastResult != null &&
                        snapshot.LastResult.Status ==
                            StartupStepStatus.Succeeded),
                Is.True,
                "The root must publish the immutable terminal step result before sequence handoff.");
        }

        [Test]
        public void WarningRunAdvancesToCompleted()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Warning Step");

            definition.ResultToReturn =
                StartupStepResult.Warning(
                    "ELAUNCH-TEST-WARN",
                    "Startup completed with an advisory.");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(
                                definition,
                                StartupStepPolicy
                                    .OptionalWarning))));

            StartupSequenceRunResult result =
                StartImmediate(root);

            Assert.That(
                result.HasWarnings,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.Progress.LastResult.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded),
                "The final lifecycle snapshot must describe the successful destination activation.");

            Assert.That(
                root.LastReport,
                Is.Not.Null);

            Assert.That(
                root.LastReport.WarningCount,
                Is.EqualTo(1));

            Assert.That(
                root.LastReport.GetStepReport(0).Status,
                Is.EqualTo(
                    StartupStepStatus.Warning),
                "The completed report must preserve the startup warning even though the destination handoff succeeded.");
        }

        [Test]
        public void BlockingRunAdvancesToFailed()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Blocking Step");

            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "ELAUNCH-TEST-BLOCK",
                    "The root-owned startup step blocked launch.");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            StartupSequenceRunResult result =
                StartImmediate(root);

            Assert.That(
                result.HasBlockingFailures,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.Progress.LastResult.Code,
                Is.EqualTo(
                    "ELAUNCH-TEST-BLOCK"));
        }

        [Test]
        public void InvalidConfigurationPreflightFailsBeforeFactory()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Never Created");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(definition)));

            ConfigurationIdField.SetValue(
                configuration,
                "invalid");

            EchoLaunchRoot root =
                CreateRoot(configuration);

            StartupSequenceRunResult result =
                StartImmediate(root);

            Assert.That(
                result,
                Is.Null);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.Progress.LastResult.Code,
                Is.EqualTo(
                    StartupSequencePreflight
                        .ConfigurationDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));
        }

        [Test]
        public void MissingConfigurationAdvancesToFailed()
        {
            EchoLaunchRoot root =
                CreateRoot(null);

            StartupSequenceRunResult result =
                StartImmediate(root);

            Assert.That(
                result,
                Is.Null);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.Progress.LastResult.Code,
                Is.EqualTo(
                    StartupSequencePreflight
                        .ConfigurationDiagnosticCode));
        }

        [Test]
        public void CancelBeforeLaunchReturnsFalse()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence()));

            Assert.That(
                root.CancelLaunch(
                    "No active launch."),
                Is.False);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));
        }

        [UnityTest]
        public IEnumerator
            ActiveCancellationWaitsForSettlementAndInterrupts()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Cancellable Step",
                    framesToWait: 20);

            definition
                .CancellationSettlementDelayFrames = 2;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                awaiter);

            Assert.That(
                root.CancelLaunch(
                    "User requested a safe stop."),
                Is.True);

            Assert.That(
                awaiter.IsCompleted,
                Is.False,
                "The root must wait for cooperative executor settlement.");

            yield return WaitForCompletion(
                awaiter);

            StartupSequenceRunResult result =
                awaiter.GetResult();

            Assert.That(
                definition.CancellationObserved,
                Is.True);

            Assert.That(
                definition.Settled,
                Is.True);

            Assert.That(
                result.WasCancelled,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Interrupted));

            Assert.That(
                root.Progress.Message,
                Is.EqualTo(
                    "User requested a safe stop."));

            Assert.That(
                root.IsLaunchActive,
                Is.False);
        }

        [UnityTest]
        public IEnumerator
            BlankCancellationReasonUsesStableDefault()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Blank Cancellation",
                    framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                awaiter);

            Assert.That(
                root.CancelLaunch("   "),
                Is.True);

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                root.Progress.Message,
                Is.EqualTo(
                    "Launch cancellation requested."));
        }

        [UnityTest]
        public IEnumerator
            RepeatedCancellationRequestIsRejected()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Repeated Cancellation",
                    framesToWait: 20);

            definition
                .CancellationSettlementDelayFrames = 2;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                awaiter);

            Assert.That(
                root.CancelLaunch("First request."),
                Is.True);

            Assert.That(
                root.CancelLaunch("Second request."),
                Is.False);

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                root.Progress.Message,
                Is.EqualTo("First request."));
        }

        [UnityTest]
        public IEnumerator
            ConcurrentRootStartIsRejectedWithoutSecondFactory()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Active Root Run",
                    framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter
                activeAwaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                activeAwaiter);

            Awaitable<StartupSequenceRunResult>.Awaiter
                rejectedAwaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            Assert.That(
                rejectedAwaiter.IsCompleted,
                Is.True);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () => rejectedAwaiter.GetResult());

            Assert.That(
                exception.Message,
                Does.Contain(
                    EchoLaunchRoot
                        .StartGateDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.CancelLaunch(
                    "End re-entry proof."),
                Is.True);

            yield return WaitForCompletion(
                activeAwaiter);

            activeAwaiter.GetResult();
        }

        [Test]
        public void SettledRootSessionCannotRestart()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Single Session");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            StartImmediate(root);

            InvalidOperationException exception =
                GetImmediateFailure(
                    root.StartLaunchAsync());

            Assert.That(
                exception.Message,
                Does.Contain(
                    EchoLaunchRoot
                        .StartGateDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void FailedRootSessionCannotRestart()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Terminal Failure");

            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "ELAUNCH-TEST-FAIL",
                    "Terminal failure.");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            InvalidOperationException exception =
                GetImmediateFailure(
                    root.StartLaunchAsync());

            Assert.That(
                exception.Message,
                Does.Contain(
                    EchoLaunchRoot
                        .StartGateDiagnosticCode));
        }

        [Test]
        public void DuplicateRootCannotStartOrCancel()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence());

            EchoLaunchRoot authority =
                CreateRoot(
                    configuration,
                    LaunchMode.CanonicalBoot,
                    "Authority");

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot(
                    configuration,
                    LaunchMode.CanonicalBoot,
                    "Duplicate");

            Assert.That(
                duplicate.WasRejectedAsDuplicate,
                Is.True);

            Assert.That(
                duplicate.CancelLaunch(
                    "Duplicate request."),
                Is.False);

            InvalidOperationException exception =
                GetImmediateFailure(
                    duplicate.StartLaunchAsync());

            Assert.That(
                exception.Message,
                Does.Contain(
                    EchoLaunchRoot
                        .StartGateDiagnosticCode));

            Assert.That(
                authority.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));
        }

        [UnityTest]
        public IEnumerator
            DestroyingActiveRootCancelsAndSuppressesLatePublication()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Destruction Cancellation",
                    framesToWait: 30);

            definition
                .CancellationSettlementDelayFrames = 2;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            int progressCalls = 0;

            root.LaunchProgressChanged +=
                _ => progressCalls++;

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                awaiter);

            int callsBeforeDestroy =
                progressCalls;

            GameObject target =
                root.gameObject;

            Object.DestroyImmediate(target);

            yield return WaitForCompletion(
                awaiter);

            StartupSequenceRunResult result =
                awaiter.GetResult();

            Assert.That(
                definition.CancellationObserved,
                Is.True);

            Assert.That(
                definition.Settled,
                Is.True);

            Assert.That(
                result.WasCancelled,
                Is.True);

            Assert.That(
                EchoLaunchRoot.Current,
                Is.Null);

            Assert.That(
                progressCalls,
                Is.EqualTo(callsBeforeDestroy),
                "Destroyed roots must not publish late progress or terminal events.");
        }

        [Test]
        public void SuccessfulRunPublishesCompletedAfterDestination()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(
                                CreateDefinition(
                                    "Pending Destination")))));

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.LastReport,
                Is.Not.Null);

            Assert.That(
                root.LastReport.FinalStatus,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.Progress.Message,
                Does.Contain(
                    "activated"));
        }

        [Test]
        public void DirectSceneModeIsPreservedAcrossRun()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence()),
                    LaunchMode.DirectSceneDevelopment,
                    "Direct Scene Root");

            StartImmediate(root);

            Assert.That(
                root.Progress.Mode,
                Is.EqualTo(
                    LaunchMode.DirectSceneDevelopment));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [Test]
        public void RootOwnedRunDoesNotMutateAuthoredAssets()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Immutable Root Step");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    StartupStepPolicy
                        .OptionalWarning);

            StartupSequence sequence =
                CreateSequence(entry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            string configurationId =
                configuration.ConfigurationId;

            string sequenceId =
                sequence.SequenceId;

            string entryId =
                entry.EntryId;

            string stepId =
                definition.StepId;

            EchoLaunchRoot root =
                CreateRoot(configuration);

            StartImmediate(root);

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(configurationId));

            Assert.That(
                configuration.StartupSequence,
                Is.SameAs(sequence));

            Assert.That(
                sequence.SequenceId,
                Is.EqualTo(sequenceId));

            Assert.That(
                sequence.GetEntry(0),
                Is.SameAs(entry));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(entryId));

            Assert.That(
                entry.StepDefinition,
                Is.SameAs(definition));

            Assert.That(
                definition.StepId,
                Is.EqualTo(stepId));
        }

        [Test]
        public void RunnerReplacementIsRejectedAfterLifecycleAdvances()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence()));

            StartImmediate(root);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () => root.SetSequenceRunnerForTesting(
                        new StartupSequenceRunner()));

            Assert.That(
                exception.Message,
                Does.Contain(
                    EchoLaunchRoot
                        .StartGateDiagnosticCode));
        }

        [Test]
        public void PreflightFailureClearsActiveGate()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence());

            ConfigurationIdField.SetValue(
                configuration,
                "invalid");

            EchoLaunchRoot root =
                CreateRoot(configuration);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.IsLaunchActive,
                Is.False);
        }

        [UnityTest]
        public IEnumerator
            CancellationPublishesInterruptedExactlyOnce()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Single Interrupted Event",
                    framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition))));

            int interruptedCalls = 0;

            root.LaunchStateChanged +=
                change =>
                {
                    if (change.CurrentState ==
                        LaunchStatus.Interrupted)
                    {
                        interruptedCalls++;
                    }
                };

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForFirstFrame(
                definition,
                awaiter);

            root.CancelLaunch(
                "Interrupt once.");

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                interruptedCalls,
                Is.EqualTo(1));
        }

        private static IEnumerator WaitForFirstFrame(
            RootLifecycleTestDefinition definition,
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter)
        {
            int waitedFrames = 0;

            while (definition.FramesCompleted < 1 &&
                   !awaiter.IsCompleted &&
                   waitedFrames <
                   MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                definition.FramesCompleted,
                Is.GreaterThanOrEqualTo(1),
                "The root-owned executor did not begin inside the bounded Play Mode window.");

            Assert.That(
                awaiter.IsCompleted,
                Is.False);
        }

        private static IEnumerator WaitForCompletion(
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter)
        {
            int waitedFrames = 0;

            while (!awaiter.IsCompleted &&
                   waitedFrames <
                   MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The root-owned launch did not settle inside the bounded Play Mode window.");
        }

        private static StartupSequenceRunResult
            StartImmediate(
                EchoLaunchRoot root)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The immediate root-owned fixture must settle synchronously.");

            return awaiter.GetResult();
        }

        private static InvalidOperationException
            GetImmediateFailure(
                Awaitable<StartupSequenceRunResult>
                    awaitable)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    awaitable.GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True);

            return Assert.Throws<InvalidOperationException>(
                () => awaiter.GetResult());
        }

        private EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            LaunchMode mode =
                LaunchMode.CanonicalBoot,
            string name = "EchoLaunch Root")
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);
            target.SetActive(false);

            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            RootLaunchModeField.SetValue(
                root,
                mode);

            target.SetActive(true);

            if (root.IsAuthoritative)
            {
                root.SetAutomaticStartForTesting(
                    false);
            }

            if (root.IsAuthoritative &&
                configuration != null)
            {
                root.SetInitialDestinationLoaderForTesting(
                    ImmediateSuccessInitialDestinationLoader
                        .Shared);
            }

            return root;
        }

        private EchoLaunchConfiguration
            CreateConfiguration(
                StartupSequence sequence)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(configuration);

            ConfigurationSequenceField.SetValue(
                configuration,
                sequence);

            ConfigurationDestinationField.SetValue(
                configuration,
                CreateDestination());

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
                "Lifecycle Destination");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/LifecycleDestination.unity");

            return destination;
        }

        private StartupSequence CreateSequence(
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

        private RootLifecycleTestDefinition
            CreateDefinition(
                string displayName,
                int framesToWait = 0)
        {
            RootLifecycleTestDefinition definition =
                ScriptableObject.CreateInstance<
                    RootLifecycleTestDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                displayName);

            definition.FramesToWait =
                framesToWait;

            return definition;
        }

        private static StartupSequenceEntry CreateEntry(
            StartupStepDefinition definition,
            StartupStepPolicy? policy = null,
            bool enabled = true)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                policy ??
                StartupStepPolicy.RequiredBlocking);

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled
                        ? 0
                        : 1));

            return entry;
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

//----- EchoLaunchRootStartupLifecycleTests.cs END -----
