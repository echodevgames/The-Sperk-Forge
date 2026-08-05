//----- StartupSequenceRunnerPreflightAndReentryTests.cs START -----

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
    /// FL-M3-05 proof that the runner validates the complete authored sequence
    /// before executor creation and rejects concurrent use of one runner
    /// instance.
    /// </summary>
    public sealed class
        StartupSequenceRunnerPreflightAndReentryTests
    {
        private const int MaximumFramesToWait = 60;

        private static readonly FieldInfo
            ConfigurationIdField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "configurationId",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationSchemaField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "schemaVersion",
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
            SequenceIdField =
                typeof(StartupSequence)
                    .GetField(
                        "sequenceId",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceSchemaField =
                typeof(StartupSequence)
                    .GetField(
                        "schemaVersion",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceEntriesField =
                typeof(StartupSequence)
                    .GetField(
                        "entries",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryIdField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "entryId",
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
            DefinitionIdField =
                typeof(StartupStepDefinition)
                    .GetField(
                        "stepId",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionSchemaField =
                typeof(StartupStepDefinition)
                    .GetField(
                        "schemaVersion",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionDisplayNameField =
                typeof(StartupStepDefinition)
                    .GetField(
                        "displayName",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            PolicyFailureActionField =
                typeof(StartupStepPolicy)
                    .GetField(
                        "failureAction",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                ConfigurationIdField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSchemaField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSequenceField,
                Is.Not.Null);

            Assert.That(
                SequenceIdField,
                Is.Not.Null);

            Assert.That(
                SequenceSchemaField,
                Is.Not.Null);

            Assert.That(
                SequenceEntriesField,
                Is.Not.Null);

            Assert.That(
                EntryIdField,
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
                DefinitionIdField,
                Is.Not.Null);

            Assert.That(
                DefinitionSchemaField,
                Is.Not.Null);

            Assert.That(
                DefinitionDisplayNameField,
                Is.Not.Null);

            Assert.That(
                PolicyFailureActionField,
                Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            for (int index =
                     createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object asset =
                    createdAssets[index];

                if (asset != null)
                {
                    Object.DestroyImmediate(asset);
                }
            }

            createdAssets.Clear();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void UnknownLaunchModeIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition;
            EchoLaunchConfiguration configuration =
                CreateValidConfiguration(
                    out definition);

            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    RunImmediate(
                        new StartupSequenceRunner(),
                        LaunchMode.Unknown,
                        configuration,
                        CancellationToken.None));

            AssertNoFactory(definition);
        }

        [Test]
        public void NullConfigurationIsRejected()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    RunImmediate(
                        new StartupSequenceRunner(),
                        LaunchMode.CanonicalBoot,
                        null,
                        CancellationToken.None));
        }

        [Test]
        public void InvalidConfigurationIdentityIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition;
            EchoLaunchConfiguration configuration =
                CreateValidConfiguration(
                    out definition);

            ConfigurationIdField.SetValue(
                configuration,
                "invalid");

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .ConfigurationDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void UnsupportedConfigurationSchemaIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition;
            EchoLaunchConfiguration configuration =
                CreateValidConfiguration(
                    out definition);

            ConfigurationSchemaField.SetValue(
                configuration,
                EchoLaunchConfiguration
                    .CurrentSchemaVersion + 1);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .ConfigurationDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void MissingSequenceIsRejectedBeforeFactory()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(null);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .ConfigurationDiagnosticCode);
        }

        [Test]
        public void InvalidSequenceIdentityIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition;
            EchoLaunchConfiguration configuration =
                CreateValidConfiguration(
                    out definition);

            SequenceIdField.SetValue(
                configuration.StartupSequence,
                "invalid");

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void UnsupportedSequenceSchemaIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition;
            EchoLaunchConfiguration configuration =
                CreateValidConfiguration(
                    out definition);

            SequenceSchemaField.SetValue(
                configuration.StartupSequence,
                StartupSequence.CurrentSchemaVersion + 1);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void NullEntryIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition later =
                CreateImmediateDefinition(
                    "Later");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        null,
                        CreateEntry(
                            later,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(later);
        }

        [Test]
        public void InvalidEntryIdentityIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Invalid Entry Identity");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy.RequiredBlocking);

            EntryIdField.SetValue(
                entry,
                "invalid");

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(entry)),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void UndefinedActivationIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Undefined Activation");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy.RequiredBlocking);

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    99));

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(entry)),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void DuplicateEntryIdentityIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition first =
                CreateImmediateDefinition(
                    "First Entry");

            ImmediateRunnerTestDefinition second =
                CreateImmediateDefinition(
                    "Second Entry");

            StartupSequenceEntry firstEntry =
                CreateEntry(
                    first,
                    true,
                    StartupStepPolicy.RequiredBlocking);

            StartupSequenceEntry secondEntry =
                CreateEntry(
                    second,
                    true,
                    StartupStepPolicy.RequiredBlocking);

            EntryIdField.SetValue(
                secondEntry,
                firstEntry.EntryId);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(
                                    firstEntry,
                                    secondEntry)),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .SequenceDiagnosticCode);

            AssertNoFactory(first);
            AssertNoFactory(second);
        }

        [Test]
        public void EnabledMissingDefinitionIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition later =
                CreateImmediateDefinition(
                    "Later Definition");

            StartupSequenceEntry missing =
                CreateEntry(
                    null,
                    true,
                    StartupStepPolicy.RequiredBlocking);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(
                                    missing,
                                    CreateEntry(
                                        later,
                                        true,
                                        StartupStepPolicy
                                            .RequiredBlocking))),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .StepDiagnosticCode);

            AssertNoFactory(later);
        }

        [Test]
        public void InvalidStepIdentityIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Invalid Step Identity");

            DefinitionIdField.SetValue(
                definition,
                "invalid");

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(
                                    CreateEntry(
                                        definition,
                                        true,
                                        StartupStepPolicy
                                            .RequiredBlocking))),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .StepDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void UnsupportedStepSchemaIsRejectedBeforeFactory()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Unsupported Step Schema");

            DefinitionSchemaField.SetValue(
                definition,
                StartupStepDefinition
                    .CurrentSchemaVersion + 1);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(
                                    CreateEntry(
                                        definition,
                                        true,
                                        StartupStepPolicy
                                            .RequiredBlocking))),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .StepDiagnosticCode);

            AssertNoFactory(definition);
        }

        [Test]
        public void DuplicateStepIdentityIsRejectedBeforeAnyFactory()
        {
            ImmediateRunnerTestDefinition first =
                CreateImmediateDefinition(
                    "First Step");

            ImmediateRunnerTestDefinition second =
                CreateImmediateDefinition(
                    "Second Step");

            DefinitionIdField.SetValue(
                second,
                first.StepId);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        RunImmediate(
                            new StartupSequenceRunner(),
                            LaunchMode.CanonicalBoot,
                            CreateConfiguration(
                                CreateSequence(
                                    CreateEntry(
                                        first,
                                        true,
                                        StartupStepPolicy
                                            .RequiredBlocking),
                                    CreateEntry(
                                        second,
                                        true,
                                        StartupStepPolicy
                                            .RequiredBlocking))),
                            CancellationToken.None));

            AssertDiagnostic(
                exception,
                StartupSequencePreflight
                    .DuplicateStepDiagnosticCode);

            AssertNoFactory(first);
            AssertNoFactory(second);
        }

        [Test]
        public void InvalidPolicyBecomesPreStartBlockingResultWithoutFactory()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Invalid Policy");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    CreateInvalidPolicy());

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    LaunchMode.CanonicalBoot,
                    CreateConfiguration(
                        CreateSequence(entry)),
                    CancellationToken.None);

            AssertNoFactory(definition);

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                result.HasBlockingFailures,
                Is.True);

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                execution.HasExecutor,
                Is.False);

            Assert.That(
                execution.Result.Code,
                Is.EqualTo(
                    StartupStepExceptionConverter
                        .DiagnosticCode));

            Assert.That(
                execution.Result.Details,
                Does.Contain("InvalidPolicy"));
        }

        [Test]
        public void DisabledEntryWithoutDefinitionRemainsValid()
        {
            StartupSequenceEntry disabled =
                CreateEntry(
                    null,
                    false,
                    StartupStepPolicy.RequiredBlocking);

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    LaunchMode.CanonicalBoot,
                    CreateConfiguration(
                        CreateSequence(disabled)),
                    CancellationToken.None);

            Assert.That(
                result.AuthoredEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.DisabledEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(0));
        }

        [Test]
        public void EmptySequenceRemainsValid()
        {
            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    LaunchMode.CanonicalBoot,
                    CreateConfiguration(
                        CreateSequence()),
                    CancellationToken.None);

            Assert.That(
                result.AuthoredEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(0));

            Assert.That(
                result.WasStoppedEarly,
                Is.False);
        }

        [Test]
        public void SuccessfulPreflightDoesNotMutateAuthoredAssets()
        {
            ImmediateRunnerTestDefinition definition =
                CreateImmediateDefinition(
                    "Immutable Preflight");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy.OptionalWarning);

            StartupSequence sequence =
                CreateSequence(entry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            string configurationId =
                configuration.ConfigurationId;

            int configurationSchema =
                configuration.SchemaVersion;

            string sequenceId =
                sequence.SequenceId;

            int sequenceSchema =
                sequence.SchemaVersion;

            string entryId =
                entry.EntryId;

            string stepId =
                definition.StepId;

            int stepSchema =
                definition.SchemaVersion;

            StartupStepPolicy policy =
                entry.Policy;

            RunImmediate(
                new StartupSequenceRunner(),
                LaunchMode.CanonicalBoot,
                configuration,
                CancellationToken.None);

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(configurationId));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(configurationSchema));

            Assert.That(
                configuration.StartupSequence,
                Is.SameAs(sequence));

            Assert.That(
                sequence.SequenceId,
                Is.EqualTo(sequenceId));

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(sequenceSchema));

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
                entry.Policy.FailureAction,
                Is.EqualTo(policy.FailureAction));

            Assert.That(
                definition.StepId,
                Is.EqualTo(stepId));

            Assert.That(
                definition.SchemaVersion,
                Is.EqualTo(stepSchema));
        }

        [UnityTest]
        public IEnumerator
            ConcurrentReentryRejectsBeforeSecondFactoryAndRunnerCanBeReused()
        {
            MultiFrameAsyncTestDefinition activeDefinition =
                CreateMultiFrameDefinition(
                    "Active Run",
                    3);

            ImmediateRunnerTestDefinition secondDefinition =
                CreateImmediateDefinition(
                    "Rejected Concurrent Run");

            EchoLaunchConfiguration activeConfiguration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            activeDefinition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            EchoLaunchConfiguration secondConfiguration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            secondDefinition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            Awaitable<StartupSequenceRunResult>.Awaiter
                activeAwaiter =
                    runner.RunAsync(
                            LaunchMode.CanonicalBoot,
                            activeConfiguration,
                            CancellationToken.None)
                        .GetAwaiter();

            Assert.That(
                activeAwaiter.IsCompleted,
                Is.False);

            Awaitable<StartupSequenceRunResult>.Awaiter
                rejectedAwaiter =
                    runner.RunAsync(
                            LaunchMode.CanonicalBoot,
                            secondConfiguration,
                            CancellationToken.None)
                        .GetAwaiter();

            Assert.That(
                rejectedAwaiter.IsCompleted,
                Is.True);

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () => rejectedAwaiter.GetResult());

            Assert.That(
                exception.Message,
                Does.Contain("ELAUNCH-RUN-001"));

            AssertNoFactory(secondDefinition);

            yield return WaitForCompletion(
                activeAwaiter);

            StartupSequenceRunResult firstResult =
                activeAwaiter.GetResult();

            Assert.That(
                firstResult.AttemptedExecutionCount,
                Is.EqualTo(1));

            StartupSequenceRunResult reusedResult =
                RunImmediate(
                    runner,
                    LaunchMode.CanonicalBoot,
                    secondConfiguration,
                    CancellationToken.None);

            Assert.That(
                reusedResult.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                secondDefinition.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                secondDefinition.ExecutionCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void GateReleasesAfterPreflightRejection()
        {
            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            EchoLaunchConfiguration invalid =
                CreateConfiguration(
                    CreateSequence());

            ConfigurationIdField.SetValue(
                invalid,
                "invalid");

            Assert.Throws<InvalidOperationException>(
                () =>
                    RunImmediate(
                        runner,
                        LaunchMode.CanonicalBoot,
                        invalid,
                        CancellationToken.None));

            ImmediateRunnerTestDefinition validDefinition;
            EchoLaunchConfiguration valid =
                CreateValidConfiguration(
                    out validDefinition);

            StartupSequenceRunResult result =
                RunImmediate(
                    runner,
                    LaunchMode.CanonicalBoot,
                    valid,
                    CancellationToken.None);

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                validDefinition.FactoryCallCount,
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator
            GateReleasesAfterStructuredCallerCancellation()
        {
            MultiFrameAsyncTestDefinition cancellingDefinition =
                CreateMultiFrameDefinition(
                    "Cancelling Run",
                    10);

            EchoLaunchConfiguration cancellingConfiguration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            cancellingDefinition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                Awaitable<StartupSequenceRunResult>.Awaiter
                    awaiter =
                        runner.RunAsync(
                                LaunchMode.CanonicalBoot,
                                cancellingConfiguration,
                                source.Token)
                            .GetAwaiter();

                Assert.That(
                    awaiter.IsCompleted,
                    Is.False);

                yield return null;
                source.Cancel();

                yield return WaitForCompletion(
                    awaiter);

                StartupSequenceRunResult cancelled =
                    awaiter.GetResult();

                Assert.That(
                    cancelled.WasCancelled,
                    Is.True);
            }

            ImmediateRunnerTestDefinition validDefinition;
            EchoLaunchConfiguration validConfiguration =
                CreateValidConfiguration(
                    out validDefinition);

            StartupSequenceRunResult reused =
                RunImmediate(
                    runner,
                    LaunchMode.CanonicalBoot,
                    validConfiguration,
                    CancellationToken.None);

            Assert.That(
                reused.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                validDefinition.FactoryCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void GateReleasesAfterBlockingTraversal()
        {
            ImmediateRunnerTestDefinition blocking =
                CreateImmediateDefinition(
                    "Blocking Run");

            blocking.ResultToReturn =
                StartupStepResult.BlockingFailure(
                    "ELAUNCH-TEST-BLOCK",
                    "Blocking result");

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            StartupSequenceRunResult blocked =
                RunImmediate(
                    runner,
                    LaunchMode.CanonicalBoot,
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(
                                blocking,
                                true,
                                StartupStepPolicy
                                    .RequiredBlocking))),
                    CancellationToken.None);

            Assert.That(
                blocked.HasBlockingFailures,
                Is.True);

            ImmediateRunnerTestDefinition validDefinition;
            EchoLaunchConfiguration validConfiguration =
                CreateValidConfiguration(
                    out validDefinition);

            StartupSequenceRunResult reused =
                RunImmediate(
                    runner,
                    LaunchMode.CanonicalBoot,
                    validConfiguration,
                    CancellationToken.None);

            Assert.That(
                reused.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                validDefinition.FactoryCallCount,
                Is.EqualTo(1));
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
                "The runner did not settle inside the bounded Play Mode proof window.");
        }

        private static void AssertNoFactory(
            ImmediateRunnerTestDefinition definition)
        {
            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(0));
        }

        private static void AssertDiagnostic(
            InvalidOperationException exception,
            string diagnosticCode)
        {
            Assert.That(
                exception,
                Is.Not.Null);

            Assert.That(
                exception.Message,
                Does.Contain(diagnosticCode));
        }

        private static StartupSequenceRunResult
            RunImmediate(
                StartupSequenceRunner runner,
                LaunchMode launchMode,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    runner.RunAsync(
                            launchMode,
                            configuration,
                            cancellationToken)
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The immediate fixture must settle synchronously.");

            return awaiter.GetResult();
        }

        private EchoLaunchConfiguration
            CreateValidConfiguration(
                out ImmediateRunnerTestDefinition
                    definition)
        {
            definition =
                CreateImmediateDefinition(
                    "Valid Step");

            return CreateConfiguration(
                CreateSequence(
                    CreateEntry(
                        definition,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking)));
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

            return configuration;
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

        private ImmediateRunnerTestDefinition
            CreateImmediateDefinition(
                string displayName)
        {
            ImmediateRunnerTestDefinition definition =
                ScriptableObject.CreateInstance<
                    ImmediateRunnerTestDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                displayName);

            return definition;
        }

        private MultiFrameAsyncTestDefinition
            CreateMultiFrameDefinition(
                string displayName,
                int framesToWait)
        {
            MultiFrameAsyncTestDefinition definition =
                ScriptableObject.CreateInstance<
                    MultiFrameAsyncTestDefinition>();

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
            bool enabled,
            StartupStepPolicy policy)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                policy);

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled
                        ? 0
                        : 1));

            return entry;
        }

        private static StartupStepPolicy
            CreateInvalidPolicy()
        {
            object boxedPolicy =
                StartupStepPolicy.RequiredBlocking;

            PolicyFailureActionField.SetValue(
                boxedPolicy,
                Enum.ToObject(
                    PolicyFailureActionField.FieldType,
                    99));

            return (StartupStepPolicy)boxedPolicy;
        }
    }
}

//----- StartupSequenceRunnerPreflightAndReentryTests.cs END -----
