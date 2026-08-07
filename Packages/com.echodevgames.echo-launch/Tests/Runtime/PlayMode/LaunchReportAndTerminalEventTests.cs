//----- LaunchReportAndTerminalEventTests.cs START -----

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    /// <summary>
    /// FL-M3-07 proof that failed and interrupted root-owned launch attempts
    /// finalize immutable reports and publish one matching terminal event only
    /// after authoritative lifecycle state and LastReport are accepted.
    /// </summary>
    public sealed class LaunchReportAndTerminalEventTests
    {
        private const int MaximumFramesToWait = 90;

        private static readonly FieldInfo RootConfigurationField =
            typeof(EchoLaunchRoot).GetField(
                "configuration",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo RootLaunchModeField =
            typeof(EchoLaunchRoot).GetField(
                "launchMode",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo ConfigurationIdField =
            typeof(EchoLaunchConfiguration).GetField(
                "configurationId",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo ConfigurationSequenceField =
            typeof(EchoLaunchConfiguration).GetField(
                "startupSequence",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo ConfigurationDestinationField =
            typeof(EchoLaunchConfiguration).GetField(
                "initialDestination",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo DestinationDisplayNameField =
            typeof(LaunchDestination).GetField(
                "displayName",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo DestinationScenePathField =
            typeof(LaunchDestination).GetField(
                "scenePath",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo SequenceEntriesField =
            typeof(StartupSequence).GetField(
                "entries",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo EntryActivationField =
            typeof(StartupSequenceEntry).GetField(
                "activation",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo EntryDefinitionField =
            typeof(StartupSequenceEntry).GetField(
                "stepDefinition",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo EntryPolicyField =
            typeof(StartupSequenceEntry).GetField(
                "policy",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo DefinitionDisplayNameField =
            typeof(StartupStepDefinition).GetField(
                "displayName",
                BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly List<GameObject> createdObjects =
            new List<GameObject>();

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(RootConfigurationField, Is.Not.Null);
            Assert.That(RootLaunchModeField, Is.Not.Null);
            Assert.That(ConfigurationIdField, Is.Not.Null);
            Assert.That(ConfigurationSequenceField, Is.Not.Null);
            Assert.That(ConfigurationDestinationField, Is.Not.Null);
            Assert.That(DestinationDisplayNameField, Is.Not.Null);
            Assert.That(DestinationScenePathField, Is.Not.Null);
            Assert.That(SequenceEntriesField, Is.Not.Null);
            Assert.That(EntryActivationField, Is.Not.Null);
            Assert.That(EntryDefinitionField, Is.Not.Null);
            Assert.That(EntryPolicyField, Is.Not.Null);
            Assert.That(DefinitionDisplayNameField, Is.Not.Null);

            LaunchAuthorityClaim.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target = createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            for (int index = createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object target = createdAssets[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            createdObjects.Clear();
            createdAssets.Clear();
            LaunchAuthorityClaim.Reset();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void LastReportIsNullBeforeLaunch()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateConfiguration(CreateSequence()));

            Assert.That(root.LastReport, Is.Null);
            Assert.That(root.HasPendingLaunchReport, Is.False);
        }

        [Test]
        public void MissingConfigurationFinalizesFailedReport()
        {
            EchoLaunchRoot root = CreateRoot(null);

            StartImmediate(root);

            LaunchReport report = root.LastReport;

            Assert.That(root.State, Is.EqualTo(LaunchStatus.Failed));
            Assert.That(report, Is.Not.Null);
            Assert.That(report.FinalStatus, Is.EqualTo(LaunchStatus.Failed));
            Assert.That(report.FinalResult.Code, Is.EqualTo("ELAUNCH-CFG-001"));
            Assert.That(report.AuthoredEntryCount, Is.Zero);
            Assert.That(report.StepReportCount, Is.Zero);
            Assert.That(report.FailureCount, Is.EqualTo(1));
            Assert.That(report.BlockingFailureCount, Is.EqualTo(1));
            Assert.That(report.WasCancelled, Is.False);
        }

        [Test]
        public void InvalidPreflightFinalizesBeforeExecutorFactory()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Preflight Sentinel");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(CreateEntry(definition)));

            ConfigurationIdField.SetValue(configuration, "invalid");

            EchoLaunchRoot root = CreateRoot(configuration);
            StartImmediate(root);

            Assert.That(definition.FactoryCallCount, Is.Zero);
            Assert.That(root.LastReport, Is.Not.Null);
            Assert.That(root.LastReport.FinalResult.Code, Is.EqualTo("ELAUNCH-CFG-001"));
            Assert.That(root.LastReport.AuthoredEntryCount, Is.EqualTo(1));
            Assert.That(root.LastReport.UnvisitedEntryCount, Is.EqualTo(1));
        }

        [Test]
        public void BlockingStepReportCopiesTerminalExecutionValues()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Copied Blocking Step");

            definition.ReportImmediateProgress = true;
            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "REPORT-BLOCK-001",
                    "Copied blocking result.",
                    "Copied details.");

            StartupStepPolicy policy =
                StartupStepPolicy.RequiredBlocking;

            StartupSequenceEntry entry =
                CreateEntry(definition, policy);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(entry)));

            StartImmediate(root);

            LaunchReport report = root.LastReport;
            LaunchStepReport step = report.GetStepReport(0);

            Assert.That(step.EntryId, Is.EqualTo(entry.EntryId));
            Assert.That(step.StepId, Is.EqualTo(definition.StepId));
            Assert.That(step.StepDisplayName, Is.EqualTo("Copied Blocking Step"));
            Assert.That(step.StepIndex, Is.Zero);
            Assert.That(step.StepCount, Is.EqualTo(1));
            Assert.That(step.Policy.IsRequired, Is.EqualTo(policy.IsRequired));
            Assert.That(step.Policy.FailureAction, Is.EqualTo(policy.FailureAction));
            Assert.That(step.Status, Is.EqualTo(StartupStepStatus.BlockingFailure));
            Assert.That(step.Result.Code, Is.EqualTo("REPORT-BLOCK-001"));
            Assert.That(step.Result.Message, Is.EqualTo("Copied blocking result."));
            Assert.That(step.Result.Details, Is.EqualTo("Copied details."));
            Assert.That(step.Progress.Progress01, Is.EqualTo(0.5f));
            Assert.That(step.Progress.Message, Does.Contain("Halfway"));
            Assert.That(step.StartSeconds, Is.GreaterThanOrEqualTo(0d));
            Assert.That(step.SettlementSeconds, Is.GreaterThanOrEqualTo(step.StartSeconds));
            Assert.That(step.ElapsedSeconds, Is.GreaterThanOrEqualTo(0d));
        }

        [Test]
        public void ReportAccountingPreservesWarningDisabledFailureAndUnvisitedEntries()
        {
            RootLifecycleTestDefinition warning =
                CreateDefinition("Warning Step");
            warning.ResultToReturn =
                StartupStepResult.Warning(
                    "REPORT-WARN-001",
                    "Warning retained.");

            RootLifecycleTestDefinition disabled =
                CreateDefinition("Disabled Step");

            RootLifecycleTestDefinition blocking =
                CreateDefinition("Blocking Step");
            blocking.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "REPORT-BLOCK-002",
                    "Traversal stopped.");

            RootLifecycleTestDefinition unvisited =
                CreateDefinition("Unvisited Step");

            StartupSequence sequence =
                CreateSequence(
                    CreateEntry(warning, StartupStepPolicy.OptionalWarning),
                    CreateEntry(disabled, enabled: false),
                    CreateEntry(blocking),
                    CreateEntry(unvisited));

            EchoLaunchRoot root =
                CreateRoot(CreateConfiguration(sequence));

            StartImmediate(root);

            LaunchReport report = root.LastReport;

            Assert.That(report.AuthoredEntryCount, Is.EqualTo(4));
            Assert.That(report.AttemptedStepCount, Is.EqualTo(2));
            Assert.That(report.DisabledEntryCount, Is.EqualTo(1));
            Assert.That(report.UnvisitedEntryCount, Is.EqualTo(1));
            Assert.That(report.WarningCount, Is.EqualTo(1));
            Assert.That(report.FailureCount, Is.EqualTo(1));
            Assert.That(report.BlockingFailureCount, Is.EqualTo(1));
            Assert.That(report.GetStepReport(0).StepId, Is.EqualTo(warning.StepId));
            Assert.That(report.GetStepReport(1).StepId, Is.EqualTo(blocking.StepId));
            Assert.That(disabled.FactoryCallCount, Is.Zero);
            Assert.That(unvisited.FactoryCallCount, Is.Zero);
        }

        [Test]
        public void FailedEventObservesAcceptedStateAndStoredReport()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateBlockingConfiguration("Event Failure"));

            LaunchReport payload = null;
            LaunchStatus observedState = LaunchStatus.None;
            bool observedStoredIdentity = false;

            root.LaunchFailed += report =>
            {
                payload = report;
                observedState = root.State;
                observedStoredIdentity =
                    ReferenceEquals(root.LastReport, report);
            };

            StartImmediate(root);

            Assert.That(payload, Is.Not.Null);
            Assert.That(observedState, Is.EqualTo(LaunchStatus.Failed));
            Assert.That(observedStoredIdentity, Is.True);
            Assert.That(payload, Is.SameAs(root.LastReport));
        }

        [Test]
        public void FailedEventFiresExactlyOnceAndInterruptedDoesNotFire()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateBlockingConfiguration("Single Failure"));

            int failedCalls = 0;
            int interruptedCalls = 0;

            root.LaunchFailed += _ => failedCalls++;
            root.LaunchInterrupted += _ => interruptedCalls++;

            StartImmediate(root);

            Assert.That(failedCalls, Is.EqualTo(1));
            Assert.That(interruptedCalls, Is.Zero);
        }

        [Test]
        public void FailedListenerFailureDoesNotPreventLaterListener()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateBlockingConfiguration("Listener Failure"));

            int laterCalls = 0;

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    @"\[ELAUNCH-EVENT-001\].*'LaunchFailed'.*report boom",
                    RegexOptions.Singleline));

            root.LaunchFailed += _ =>
                throw new InvalidOperationException("report boom");
            root.LaunchFailed += _ => laterCalls++;

            StartImmediate(root);

            Assert.That(laterCalls, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator InterruptedReportFinalizesAfterExecutorSettlement()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Settled Cancellation", framesToWait: 20);
            definition.CancellationSettlementDelayFrames = 2;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);

            Assert.That(root.LastReport, Is.Null);
            Assert.That(root.CancelLaunch("Cancel after settlement."), Is.True);
            Assert.That(root.LastReport, Is.Null);

            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(definition.CancellationObserved, Is.True);
            Assert.That(definition.Settled, Is.True);
            Assert.That(root.State, Is.EqualTo(LaunchStatus.Interrupted));
            Assert.That(root.LastReport, Is.Not.Null);
            Assert.That(root.LastReport.WasCancelled, Is.True);
            Assert.That(root.LastReport.FinalResult.Message, Is.EqualTo("Cancel after settlement."));
        }

        [UnityTest]
        public IEnumerator InterruptedEventObservesAcceptedStateAndStoredReport()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Interrupted Event", framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            LaunchReport payload = null;
            LaunchStatus observedState = LaunchStatus.None;
            bool observedIdentity = false;

            root.LaunchInterrupted += report =>
            {
                payload = report;
                observedState = root.State;
                observedIdentity = ReferenceEquals(root.LastReport, report);
            };

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);
            root.CancelLaunch("Observe interruption.");
            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(payload, Is.Not.Null);
            Assert.That(observedState, Is.EqualTo(LaunchStatus.Interrupted));
            Assert.That(observedIdentity, Is.True);
        }

        [UnityTest]
        public IEnumerator InterruptedEventFiresExactlyOnceAndFailedDoesNotFire()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Single Interruption", framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            int interruptedCalls = 0;
            int failedCalls = 0;
            root.LaunchInterrupted += _ => interruptedCalls++;
            root.LaunchFailed += _ => failedCalls++;

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);
            root.CancelLaunch("Interrupt exactly once.");
            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(interruptedCalls, Is.EqualTo(1));
            Assert.That(failedCalls, Is.Zero);
        }

        [UnityTest]
        public IEnumerator BlankCancellationReasonIsRecordedInReport()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Blank Reason", framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);
            root.CancelLaunch("   ");
            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(
                root.LastReport.FinalResult.Message,
                Is.EqualTo("Launch cancellation requested."));
        }

        [UnityTest]
        public IEnumerator InterruptedListenerFailureDoesNotPreventLaterListener()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Interrupted Listener", framesToWait: 20);

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            int laterCalls = 0;

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    @"\[ELAUNCH-EVENT-001\].*'LaunchInterrupted'.*interrupt boom",
                    RegexOptions.Singleline));

            root.LaunchInterrupted += _ =>
                throw new InvalidOperationException("interrupt boom");
            root.LaunchInterrupted += _ => laterCalls++;

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);
            root.CancelLaunch("Listener interruption.");
            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(laterCalls, Is.EqualTo(1));
        }

        [Test]
        public void SuccessfulHandoffFinalizesCompletedReportAndEvent()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(CreateDefinition("Success")))));

            int failedCalls = 0;
            int interruptedCalls = 0;
            int completedCalls = 0;
            LaunchReport completedReport = null;

            root.LaunchFailed += _ => failedCalls++;
            root.LaunchInterrupted += _ => interruptedCalls++;
            root.LaunchCompleted += report =>
            {
                completedCalls++;
                completedReport = report;
            };

            StartImmediate(root);

            Assert.That(root.State, Is.EqualTo(LaunchStatus.Completed));
            Assert.That(root.LastReport, Is.Not.Null);
            Assert.That(root.LastReport.FinalStatus, Is.EqualTo(LaunchStatus.Completed));
            Assert.That(root.HasPendingLaunchReport, Is.False);
            Assert.That(completedCalls, Is.EqualTo(1));
            Assert.That(completedReport, Is.SameAs(root.LastReport));
            Assert.That(failedCalls, Is.Zero);
            Assert.That(interruptedCalls, Is.Zero);
        }

        [Test]
        public void DuplicateRootExposesNoReportAndPublishesNoTerminalEvent()
        {
            EchoLaunchRoot authority =
                CreateRoot(CreateConfiguration(CreateSequence()));

            ExpectDuplicateWarning();

            EchoLaunchRoot duplicate =
                CreateRoot(
                    CreateConfiguration(CreateSequence()),
                    name: "Duplicate Report Root");

            int calls = 0;
            duplicate.LaunchFailed += _ => calls++;
            duplicate.LaunchInterrupted += _ => calls++;
            duplicate.LaunchCompleted += _ => calls++;

            Assert.That(authority.IsAuthoritative, Is.True);
            Assert.That(duplicate.IsAuthoritative, Is.False);
            Assert.That(duplicate.LastReport, Is.Null);
            Assert.That(duplicate.CancelLaunch("No authority."), Is.False);
            Assert.That(calls, Is.Zero);
        }

        [UnityTest]
        public IEnumerator DestroyedRootPublishesNoLateTerminalReportEvent()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Destroyed Report", framesToWait: 20);
            definition.CancellationSettlementDelayFrames = 2;

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));

            int terminalCalls = 0;
            root.LaunchFailed += _ => terminalCalls++;
            root.LaunchInterrupted += _ => terminalCalls++;
            root.LaunchCompleted += _ => terminalCalls++;

            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            yield return WaitForFirstFrame(definition, awaiter);

            GameObject target = root.gameObject;
            Object.DestroyImmediate(target);

            yield return WaitForCompletion(awaiter);
            awaiter.GetResult();

            Assert.That(definition.CancellationObserved, Is.True);
            Assert.That(definition.Settled, Is.True);
            Assert.That(terminalCalls, Is.Zero);
            Assert.That(EchoLaunchRoot.Current, Is.Null);
        }

        [Test]
        public void ReportSchemaAndPackageVersionAreStable()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateBlockingConfiguration("Schema Report"));

            StartImmediate(root);

            Assert.That(root.LastReport.ReportSchemaVersion, Is.EqualTo(2));
            Assert.That(root.LastReport.PackageVersion, Is.EqualTo("0.1.0-beta.1"));
            Assert.That(LaunchReport.CurrentSchemaVersion, Is.EqualTo(2));
            Assert.That(LaunchReport.CurrentPackageVersion, Is.EqualTo("0.1.0-beta.1"));
        }

        [Test]
        public void PublicReportPropertiesExposeNoPublicSetters()
        {
            AssertPublicPropertiesAreReadOnly(typeof(LaunchReport));
            AssertPublicPropertiesAreReadOnly(typeof(LaunchStepReport));

            Assert.That(
                typeof(LaunchReport).GetProperty("StepReports"),
                Is.Null,
                "The public report must not expose a mutable step collection.");
        }

        [Test]
        public void GetStepReportRejectsInvalidIndex()
        {
            EchoLaunchRoot root =
                CreateRoot(CreateBlockingConfiguration("Index Report"));

            StartImmediate(root);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => root.LastReport.GetStepReport(-1));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => root.LastReport.GetStepReport(1));
        }

        [Test]
        public void BuilderRejectsSecondFinalization()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Builder Step");
            StartupSequenceEntry entry = CreateEntry(definition);
            StartupSequence sequence = CreateSequence(entry);
            EchoLaunchConfiguration configuration = CreateConfiguration(sequence);
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    "BUILDER-001",
                    "Builder failure.");

            StartupStepExecution execution =
                CreateCompletedExecution(entry, 0, 1, result);

            StartupSequenceRunResult runResult =
                new StartupSequenceRunResult(
                    1,
                    0,
                    new[] { execution });

            LaunchReportBuilder builder =
                new LaunchReportBuilder(
                    LaunchMode.CanonicalBoot,
                    configuration,
                    1d);

            builder.RecordSequenceValidated(sequence);
            builder.RecordStepCompleted(execution);

            LaunchReport report =
                builder.FinalizeReport(
                    LaunchStatus.Failed,
                    runResult,
                    result,
                    2d);

            Assert.That(report, Is.Not.Null);
            Assert.That(builder.IsFinalized, Is.True);

            Assert.Throws<InvalidOperationException>(
                () => builder.FinalizeReport(
                    LaunchStatus.Failed,
                    runResult,
                    result,
                    3d));
        }

        [Test]
        public void LaunchReportRejectsNonterminalSuccessStatus()
        {
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    "REPORT-INVALID-001",
                    "Invalid final status.");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LaunchReport(
                    LaunchMode.CanonicalBoot,
                    "configuration",
                    "sequence",
                    LaunchStatus.Transitioning,
                    1d,
                    2d,
                    0,
                    0,
                    0,
                    false,
                    result,
                    Array.Empty<LaunchStepReport>()));
        }

        [Test]
        public void LaunchReportDefensivelyCopiesStepList()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Defensive Copy");
            StartupSequenceEntry entry = CreateEntry(definition);
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    "COPY-001",
                    "Copy source.");
            StartupStepExecution execution =
                CreateCompletedExecution(entry, 0, 1, result);
            LaunchStepReport stepReport =
                new LaunchStepReport(execution);
            List<LaunchStepReport> source =
                new List<LaunchStepReport> { stepReport };

            LaunchReport report =
                new LaunchReport(
                    LaunchMode.CanonicalBoot,
                    "configuration",
                    "sequence",
                    LaunchStatus.Failed,
                    1d,
                    2d,
                    1,
                    0,
                    0,
                    false,
                    result,
                    source);

            source.Clear();

            Assert.That(report.StepReportCount, Is.EqualTo(1));
            Assert.That(report.GetStepReport(0), Is.SameAs(stepReport));
        }

        [Test]
        public void FinalizedReportRemainsReadableAfterRootAndAssetsAreDestroyed()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Durable Session Copy");
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(CreateEntry(definition))));
            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "DURABLE-001",
                    "Durable report.");

            StartImmediate(root);
            LaunchReport report = root.LastReport;
            string stepId = report.GetStepReport(0).StepId;

            Object.DestroyImmediate(root.gameObject);
            Object.DestroyImmediate(definition);

            Assert.That(report.FinalResult.Code, Is.EqualTo("DURABLE-001"));
            Assert.That(report.GetStepReport(0).StepId, Is.EqualTo(stepId));
            Assert.That(report.GetStepReport(0).StepDisplayName, Is.EqualTo("Durable Session Copy"));
        }

        [Test]
        public void FailedReportDoesNotMutateAuthoredAssets()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition("Immutable Report Step");
            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "IMMUTABLE-001",
                    "Immutable failure.");

            StartupSequenceEntry entry = CreateEntry(definition);
            StartupSequence sequence = CreateSequence(entry);
            EchoLaunchConfiguration configuration = CreateConfiguration(sequence);

            string configurationId = configuration.ConfigurationId;
            string sequenceId = sequence.SequenceId;
            string entryId = entry.EntryId;
            string stepId = definition.StepId;

            EchoLaunchRoot root = CreateRoot(configuration);
            StartImmediate(root);

            Assert.That(configuration.ConfigurationId, Is.EqualTo(configurationId));
            Assert.That(configuration.StartupSequence, Is.SameAs(sequence));
            Assert.That(sequence.SequenceId, Is.EqualTo(sequenceId));
            Assert.That(sequence.GetEntry(0), Is.SameAs(entry));
            Assert.That(entry.EntryId, Is.EqualTo(entryId));
            Assert.That(entry.StepDefinition, Is.SameAs(definition));
            Assert.That(definition.StepId, Is.EqualTo(stepId));
        }

        [Test]
        public void LaunchReportRejectsInconsistentAccountingAndTiming()
        {
            StartupStepResult result =
                StartupStepResult.BlockingFailure(
                    "REPORT-INVALID-002",
                    "Invalid accounting.");

            Assert.Throws<ArgumentException>(
                () => new LaunchReport(
                    LaunchMode.CanonicalBoot,
                    "configuration",
                    "sequence",
                    LaunchStatus.Failed,
                    1d,
                    2d,
                    1,
                    0,
                    0,
                    false,
                    result,
                    Array.Empty<LaunchStepReport>()));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new LaunchReport(
                    LaunchMode.CanonicalBoot,
                    "configuration",
                    "sequence",
                    LaunchStatus.Failed,
                    2d,
                    1d,
                    0,
                    0,
                    0,
                    false,
                    result,
                    Array.Empty<LaunchStepReport>()));
        }

        private EchoLaunchConfiguration CreateBlockingConfiguration(
            string displayName)
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(displayName);

            definition.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "REPORT-FAIL-001",
                    "Launch report failure.");

            return CreateConfiguration(
                CreateSequence(CreateEntry(definition)));
        }

        private static StartupStepExecution CreateCompletedExecution(
            StartupSequenceEntry entry,
            int stepIndex,
            int stepCount,
            StartupStepResult result)
        {
            IStartupStepExecutor executor =
                entry.StepDefinition.CreateExecutor();

            StartupStepExecution execution =
                new StartupStepExecution(
                    entry,
                    stepIndex,
                    stepCount,
                    executor);

            execution.Begin();
            execution.Report(
                StartupStepProgress.Determinate(
                    0.75f,
                    "Copied progress."));
            execution.Complete(
                result,
                new StartupStepTiming(
                    1d,
                    2d,
                    0d,
                    false,
                    false));

            return execution;
        }

        private static void AssertPublicPropertiesAreReadOnly(Type type)
        {
            PropertyInfo[] properties =
                type.GetProperties(
                    BindingFlags.Instance |
                    BindingFlags.Public);

            Assert.That(properties.Length, Is.GreaterThan(0));

            foreach (PropertyInfo property in properties)
            {
                Assert.That(
                    property.SetMethod,
                    Is.Null,
                    $"{type.Name}.{property.Name} must not expose a public setter.");
            }
        }

        private static IEnumerator WaitForFirstFrame(
            RootLifecycleTestDefinition definition,
            Awaitable<StartupSequenceRunResult>.Awaiter awaiter)
        {
            int waitedFrames = 0;

            while (definition.FramesCompleted < 1 &&
                   !awaiter.IsCompleted &&
                   waitedFrames < MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(definition.FramesCompleted, Is.GreaterThanOrEqualTo(1));
            Assert.That(awaiter.IsCompleted, Is.False);
        }

        private static IEnumerator WaitForCompletion(
            Awaitable<StartupSequenceRunResult>.Awaiter awaiter)
        {
            int waitedFrames = 0;

            while (!awaiter.IsCompleted &&
                   waitedFrames < MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(awaiter.IsCompleted, Is.True);
        }

        private static StartupSequenceRunResult StartImmediate(
            EchoLaunchRoot root)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter awaiter =
                root.StartLaunchAsync().GetAwaiter();

            Assert.That(awaiter.IsCompleted, Is.True);
            return awaiter.GetResult();
        }

        private EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            LaunchMode mode = LaunchMode.CanonicalBoot,
            string name = "EchoLaunch Report Root")
        {
            GameObject target = new GameObject(name);
            createdObjects.Add(target);
            target.SetActive(false);

            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(root, configuration);
            RootLaunchModeField.SetValue(root, mode);
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
                    ImmediateSuccessInitialDestinationLoader.Shared);
            }

            return root;
        }

        private EchoLaunchConfiguration CreateConfiguration(
            StartupSequence sequence)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<EchoLaunchConfiguration>();

            createdAssets.Add(configuration);
            ConfigurationSequenceField.SetValue(configuration, sequence);
            ConfigurationDestinationField.SetValue(
                configuration,
                CreateDestination());
            return configuration;
        }

        private LaunchDestination CreateDestination()
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<LaunchDestination>();

            createdAssets.Add(destination);
            DestinationDisplayNameField.SetValue(
                destination,
                "Report Destination");
            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/ReportDestination.unity");
            return destination;
        }

        private StartupSequence CreateSequence(
            params StartupSequenceEntry[] entries)
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<StartupSequence>();

            createdAssets.Add(sequence);
            SequenceEntriesField.SetValue(
                sequence,
                new List<StartupSequenceEntry>(entries));
            return sequence;
        }

        private RootLifecycleTestDefinition CreateDefinition(
            string displayName,
            int framesToWait = 0)
        {
            RootLifecycleTestDefinition definition =
                ScriptableObject.CreateInstance<RootLifecycleTestDefinition>();

            createdAssets.Add(definition);
            DefinitionDisplayNameField.SetValue(definition, displayName);
            definition.FramesToWait = framesToWait;
            return definition;
        }

        private static StartupSequenceEntry CreateEntry(
            StartupStepDefinition definition,
            StartupStepPolicy? policy = null,
            bool enabled = true)
        {
            StartupSequenceEntry entry = new StartupSequenceEntry();

            EntryDefinitionField.SetValue(entry, definition);
            EntryPolicyField.SetValue(
                entry,
                policy ?? StartupStepPolicy.RequiredBlocking);
            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled ? 0 : 1));

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

//----- LaunchReportAndTerminalEventTests.cs END -----
