//----- StartupStepPolicyAndExecutorContractTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class
        StartupStepPolicyAndExecutorContractTests
    {
        private static readonly FieldInfo
            PolicyRequirementField =
                typeof(StartupStepPolicy).GetField(
                    "requirement",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            PolicyFailureActionField =
                typeof(StartupStepPolicy).GetField(
                    "failureAction",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            PolicyTimeoutSecondsField =
                typeof(StartupStepPolicy).GetField(
                    "timeoutSeconds",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            PolicyCancellationField =
                typeof(StartupStepPolicy).GetField(
                    "cancellation",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceSchemaVersionField =
                typeof(StartupSequence).GetField(
                    "schemaVersion",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                PolicyRequirementField,
                Is.Not.Null);

            Assert.That(
                PolicyFailureActionField,
                Is.Not.Null);

            Assert.That(
                PolicyTimeoutSecondsField,
                Is.Not.Null);

            Assert.That(
                PolicyCancellationField,
                Is.Not.Null);

            Assert.That(
                SequenceSchemaVersionField,
                Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = createdAssets.Count - 1;
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
        public void FailureActionContainsOnlyApprovedMvpValues()
        {
            StartupStepFailureAction[] values =
                (StartupStepFailureAction[])
                Enum.GetValues(
                    typeof(StartupStepFailureAction));

            Assert.That(
                values,
                Is.EqualTo(
                    new[]
                    {
                        StartupStepFailureAction
                            .BlockLaunch,
                        StartupStepFailureAction
                            .ContinueWithWarning
                    }));
        }

        [Test]
        public void RequiredPresetIsRequired()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.RequiredBlocking;

            Assert.That(
                policy.IsRequired,
                Is.True);

            Assert.That(
                policy.IsOptional,
                Is.False);
        }

        [Test]
        public void RequiredPresetBlocksLaunch()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.RequiredBlocking;

            Assert.That(
                policy.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .BlockLaunch));
        }

        [Test]
        public void RequiredPresetHasNoTimeout()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.RequiredBlocking;

            Assert.That(
                policy.TimeoutSeconds,
                Is.EqualTo(0f));

            Assert.That(
                policy.HasTimeout,
                Is.False);
        }

        [Test]
        public void RequiredPresetSupportsCancellation()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.RequiredBlocking;

            Assert.That(
                policy.SupportsCancellation,
                Is.True);

            Assert.That(
                policy.IsValid,
                Is.True);
        }

        [Test]
        public void OptionalPresetIsOptional()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.OptionalWarning;

            Assert.That(
                policy.IsOptional,
                Is.True);

            Assert.That(
                policy.IsRequired,
                Is.False);
        }

        [Test]
        public void OptionalPresetContinuesWithWarning()
        {
            StartupStepPolicy policy =
                StartupStepPolicy.OptionalWarning;

            Assert.That(
                policy.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .ContinueWithWarning));

            Assert.That(
                policy.IsValid,
                Is.True);
        }

        [Test]
        public void PositiveTimeoutIsEnabledAndPreserved()
        {
            StartupStepPolicy policy =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .BlockLaunch,
                    12.5f,
                    true);

            Assert.That(
                policy.TimeoutSeconds,
                Is.EqualTo(12.5f));

            Assert.That(
                policy.HasTimeout,
                Is.True);

            Assert.That(
                policy.HasValidTimeout,
                Is.True);
        }

        [Test]
        public void ZeroTimeoutIsDisabled()
        {
            StartupStepPolicy policy =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .BlockLaunch,
                    0f,
                    true);

            Assert.That(
                policy.TimeoutSeconds,
                Is.EqualTo(0f));

            Assert.That(
                policy.HasTimeout,
                Is.False);

            Assert.That(
                policy.HasValidTimeout,
                Is.True);
        }

        [Test]
        public void NegativeTimeoutIsInvalidWithoutRepair()
        {
            StartupStepPolicy policy =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .BlockLaunch,
                    -3f,
                    true);

            Assert.That(
                policy.HasValidTimeout,
                Is.False);

            Assert.That(
                policy.IsValid,
                Is.False);

            Assert.That(
                policy.TimeoutSeconds,
                Is.EqualTo(-3f));
        }

        [Test]
        public void NonFiniteTimeoutIsInvalidWithoutRepair()
        {
            StartupStepPolicy nanPolicy =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .BlockLaunch,
                    float.NaN,
                    true);

            StartupStepPolicy infinityPolicy =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .BlockLaunch,
                    float.PositiveInfinity,
                    true);

            Assert.That(
                nanPolicy.HasValidTimeout,
                Is.False);

            Assert.That(
                float.IsNaN(
                    nanPolicy.TimeoutSeconds),
                Is.True);

            Assert.That(
                infinityPolicy.HasValidTimeout,
                Is.False);

            Assert.That(
                float.IsPositiveInfinity(
                    infinityPolicy.TimeoutSeconds),
                Is.True);
        }

        [Test]
        public void UndefinedFailureActionIsInvalidWithoutRewrite()
        {
            StartupStepFailureAction undefined =
                (StartupStepFailureAction)999;

            StartupStepPolicy policy =
                CreatePolicy(
                    true,
                    undefined,
                    0f,
                    true);

            Assert.That(
                policy.HasValidFailureAction,
                Is.False);

            Assert.That(
                policy.IsValid,
                Is.False);

            Assert.That(
                policy.FailureAction,
                Is.EqualTo(undefined));
        }

        [Test]
        public void DeterminateProgressPreservesValues()
        {
            StartupStepProgress progress =
                StartupStepProgress.Determinate(
                    0.45f,
                    "Loading catalogs");

            Assert.That(
                progress.IsIndeterminate,
                Is.False);

            Assert.That(
                progress.Progress01,
                Is.EqualTo(0.45f));

            Assert.That(
                progress.Message,
                Is.EqualTo("Loading catalogs"));
        }

        [Test]
        public void DeterminateProgressAcceptsInclusiveBoundaries()
        {
            StartupStepProgress zero =
                StartupStepProgress.Determinate(0f);

            StartupStepProgress one =
                StartupStepProgress.Determinate(1f);

            Assert.That(
                zero.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                one.Progress01,
                Is.EqualTo(1f));
        }

        [Test]
        public void IndeterminateProgressDoesNotInventPercentage()
        {
            StartupStepProgress progress =
                StartupStepProgress.Indeterminate(
                    "Waiting for platform");

            Assert.That(
                progress.IsIndeterminate,
                Is.True);

            Assert.That(
                progress.Progress01,
                Is.EqualTo(0f));

            Assert.That(
                progress.Message,
                Is.EqualTo("Waiting for platform"));
        }

        [Test]
        public void DeterminateProgressBelowRangeIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    StartupStepProgress.Determinate(
                        -0.01f));
        }

        [Test]
        public void DeterminateProgressAboveRangeIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    StartupStepProgress.Determinate(
                        1.01f));
        }

        [Test]
        public void ProgressMessageIsNormalized()
        {
            StartupStepProgress trimmed =
                StartupStepProgress.Determinate(
                    0.5f,
                    "  Preparing world  ");

            StartupStepProgress blank =
                StartupStepProgress.Indeterminate(
                    "   ");

            Assert.That(
                trimmed.Message,
                Is.EqualTo("Preparing world"));

            Assert.That(
                blank.Message,
                Is.Empty);
        }

        [Test]
        public void ContextPreservesIdentityData()
        {
            RecordingProgressReporter reporter =
                new RecordingProgressReporter();

            StartupStepContext context =
                CreateContext(
                    reporter,
                    CancellationToken.None);

            Assert.That(
                context.LaunchMode,
                Is.EqualTo(
                    LaunchMode.CanonicalBoot));

            Assert.That(
                context.ConfigurationId,
                Is.EqualTo("configuration-id"));

            Assert.That(
                context.SequenceId,
                Is.EqualTo("sequence-id"));

            Assert.That(
                context.EntryId,
                Is.EqualTo("entry-id"));

            Assert.That(
                context.StepId,
                Is.EqualTo("step-id"));
        }

        [Test]
        public void ContextPreservesStepIndexAndCount()
        {
            RecordingProgressReporter reporter =
                new RecordingProgressReporter();

            StartupStepContext context =
                new StartupStepContext(
                    LaunchMode.CanonicalBoot,
                    "configuration-id",
                    "sequence-id",
                    "entry-id",
                    "step-id",
                    2,
                    5,
                    CancellationToken.None,
                    reporter);

            Assert.That(
                context.StepIndex,
                Is.EqualTo(2));

            Assert.That(
                context.StepCount,
                Is.EqualTo(5));
        }

        [Test]
        public void ContextPreservesCancellationToken()
        {
            RecordingProgressReporter reporter =
                new RecordingProgressReporter();

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                StartupStepContext context =
                    CreateContext(
                        reporter,
                        source.Token);

                Assert.That(
                    context.CancellationToken,
                    Is.EqualTo(source.Token));
            }
        }

        [Test]
        public void ContextProgressReporterReceivesValue()
        {
            RecordingProgressReporter reporter =
                new RecordingProgressReporter();

            StartupStepContext context =
                CreateContext(
                    reporter,
                    CancellationToken.None);

            StartupStepProgress progress =
                StartupStepProgress.Determinate(
                    0.75f,
                    "Almost ready");

            context.ProgressReporter.Report(
                progress);

            Assert.That(
                reporter.ReportCount,
                Is.EqualTo(1));

            Assert.That(
                reporter.Latest.Progress01,
                Is.EqualTo(0.75f));

            Assert.That(
                reporter.Latest.Message,
                Is.EqualTo("Almost ready"));
        }

        [Test]
        public void ContextRejectsNullProgressReporter()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new StartupStepContext(
                        LaunchMode.CanonicalBoot,
                        "configuration-id",
                        "sequence-id",
                        "entry-id",
                        "step-id",
                        0,
                        1,
                        CancellationToken.None,
                        null));
        }

        [Test]
        public void ExecutorMethodReturnsApprovedAwaitableType()
        {
            MethodInfo method =
                typeof(IStartupStepExecutor).GetMethod(
                    nameof(
                        IStartupStepExecutor
                            .ExecuteAsync));

            Assert.That(
                method,
                Is.Not.Null);

            Assert.That(
                method.ReturnType,
                Is.EqualTo(
                    typeof(
                        Awaitable<
                            StartupStepResult>)));
        }

        [Test]
        public void DefinitionFactoryProducesExecutor()
        {
            TestStartupStepDefinition definition =
                CreateDefinition();

            IStartupStepExecutor executor =
                definition.CreateExecutor();

            Assert.That(
                executor,
                Is.Not.Null);
        }

        [Test]
        public void RepeatedFactoryCallsProduceDistinctExecutors()
        {
            TestStartupStepDefinition definition =
                CreateDefinition();

            IStartupStepExecutor first =
                definition.CreateExecutor();

            IStartupStepExecutor second =
                definition.CreateExecutor();

            Assert.That(
                second,
                Is.Not.SameAs(first));
        }

        [Test]
        public void NewEntryUsesSafeDefaultPolicy()
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            Assert.That(
                entry.IsEnabled,
                Is.True);

            Assert.That(
                entry.Policy.IsRequired,
                Is.True);

            Assert.That(
                entry.Policy.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .BlockLaunch));

            Assert.That(
                entry.Policy.HasTimeout,
                Is.False);

            Assert.That(
                entry.Policy.SupportsCancellation,
                Is.True);

            Assert.That(
                entry.Policy.IsValid,
                Is.True);
        }

        [Test]
        public void SequenceSchemaIsTwoAndOlderValueIsUnsupported()
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<
                    StartupSequence>();

            createdAssets.Add(sequence);

            Assert.That(
                StartupSequence.CurrentSchemaVersion,
                Is.EqualTo(2));

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(2));

            Assert.That(
                sequence.HasSupportedSchema,
                Is.True);

            SequenceSchemaVersionField.SetValue(
                sequence,
                1);

            Assert.That(
                sequence.HasSupportedSchema,
                Is.False);

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(1));
        }

        private TestStartupStepDefinition
            CreateDefinition()
        {
            TestStartupStepDefinition definition =
                ScriptableObject.CreateInstance<
                    TestStartupStepDefinition>();

            createdAssets.Add(definition);

            return definition;
        }

        private static StartupStepPolicy CreatePolicy(
            bool isRequired,
            StartupStepFailureAction failureAction,
            float timeoutSeconds,
            bool supportsCancellation)
        {
            object boxed =
                StartupStepPolicy.RequiredBlocking;

            object requirementValue =
                Enum.ToObject(
                    PolicyRequirementField.FieldType,
                    isRequired
                        ? 0
                        : 1);

            object cancellationValue =
                Enum.ToObject(
                    PolicyCancellationField.FieldType,
                    supportsCancellation
                        ? 0
                        : 1);

            PolicyRequirementField.SetValue(
                boxed,
                requirementValue);

            PolicyFailureActionField.SetValue(
                boxed,
                failureAction);

            PolicyTimeoutSecondsField.SetValue(
                boxed,
                timeoutSeconds);

            PolicyCancellationField.SetValue(
                boxed,
                cancellationValue);

            return (StartupStepPolicy)boxed;
        }

        private static StartupStepContext CreateContext(
            IStartupStepProgressReporter reporter,
            CancellationToken cancellationToken)
        {
            return new StartupStepContext(
                LaunchMode.CanonicalBoot,
                "configuration-id",
                "sequence-id",
                "entry-id",
                "step-id",
                0,
                1,
                cancellationToken,
                reporter);
        }

        private sealed class RecordingProgressReporter :
            IStartupStepProgressReporter
        {
            public int ReportCount
            {
                get;
                private set;
            }

            public StartupStepProgress Latest
            {
                get;
                private set;
            }

            public void Report(
                StartupStepProgress progress)
            {
                ReportCount++;
                Latest = progress;
            }
        }
    }
}

//----- StartupStepPolicyAndExecutorContractTests.cs END -----
