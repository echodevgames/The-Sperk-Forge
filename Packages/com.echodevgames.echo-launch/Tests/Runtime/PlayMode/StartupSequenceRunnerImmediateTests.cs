//----- StartupSequenceRunnerImmediateTests.cs START -----

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
    internal sealed class ImmediateRunnerTestDefinition :
        StartupStepDefinition
    {
        private readonly List<IStartupStepExecutor>
            createdExecutors =
                new List<IStartupStepExecutor>();

        private readonly List<StartupStepContext>
            receivedContexts =
                new List<StartupStepContext>();

        public StartupStepResult ResultToReturn
        {
            get;
            set;
        } =
            StartupStepResult.Success();

        public bool ShouldReportProgress
        {
            get;
            set;
        }

        public StartupStepProgress ProgressToReport
        {
            get;
            set;
        } =
            StartupStepProgress.Indeterminate();

        public bool ReturnNullExecutor
        {
            get;
            set;
        }

        public List<int> InvocationOrder
        {
            get;
            set;
        }

        public int InvocationToken
        {
            get;
            set;
        }

        public int FactoryCallCount
        {
            get;
            private set;
        }

        public int ExecutionCallCount
        {
            get;
            private set;
        }

        public int CreatedExecutorCount =>
            createdExecutors.Count;

        public int ReceivedContextCount =>
            receivedContexts.Count;

        public IStartupStepExecutor GetCreatedExecutor(
            int index)
        {
            return createdExecutors[index];
        }

        public StartupStepContext GetReceivedContext(
            int index)
        {
            return receivedContexts[index];
        }

        public override IStartupStepExecutor
            CreateExecutor()
        {
            FactoryCallCount++;

            if (ReturnNullExecutor)
            {
                return null;
            }

            ImmediateRunnerTestExecutor executor =
                new ImmediateRunnerTestExecutor(
                    this);

            createdExecutors.Add(executor);

            return executor;
        }

        internal void RecordExecution(
            StartupStepContext context)
        {
            ExecutionCallCount++;
            receivedContexts.Add(context);

            InvocationOrder?.Add(
                InvocationToken);
        }
    }

    internal sealed class ImmediateRunnerTestExecutor :
        IStartupStepExecutor
    {
        private readonly ImmediateRunnerTestDefinition
            definition;

        internal ImmediateRunnerTestExecutor(
            ImmediateRunnerTestDefinition definition)
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
            definition.RecordExecution(context);

            if (definition.ShouldReportProgress)
            {
                context.ProgressReporter.Report(
                    definition.ProgressToReport);
            }

            return definition.ResultToReturn;
        }
#pragma warning restore CS1998
    }

    public sealed class
        StartupSequenceRunnerImmediateTests
    {
        private static readonly FieldInfo
            ConfigurationSequenceField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "startupSequence",
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

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                ConfigurationSequenceField,
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
        public void NullConfigurationIsRejected()
        {
            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            Assert.Throws<ArgumentNullException>(
                () =>
                    RunImmediate(
                        runner,
                        null,
                        CancellationToken.None));
        }

        [Test]
        public void MissingSequenceIsRejected()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(null);

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            Assert.Throws<InvalidOperationException>(
                () =>
                    RunImmediate(
                        runner,
                        configuration,
                        CancellationToken.None));
        }

        [Test]
        public void EmptySequenceReturnsValidEmptyResult()
        {
            StartupSequence sequence =
                CreateSequence();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    CancellationToken.None);

            Assert.That(
                result.AuthoredEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.DisabledEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(0));

            Assert.That(
                result.HasWarnings,
                Is.False);

            Assert.That(
                result.HasFailures,
                Is.False);

            Assert.That(
                result.HasBlockingFailures,
                Is.False);
        }

        [Test]
        public void DisabledEntryCreatesNoExecutor()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Disabled Step");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    false,
                    StartupStepPolicy
                        .RequiredBlocking);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(entry));

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
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

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(0));
        }

        [Test]
        public void EnabledEntryCreatesAndExecutesOnce()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Enabled Step");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(entry));

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    CancellationToken.None);

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                result.GetExecution(0).IsComplete,
                Is.True);
        }

        [Test]
        public void RepeatedRunsCreateFreshExecutors()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Fresh Executor Step");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            definition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            StartupSequenceRunResult first =
                RunImmediate(
                    runner,
                    configuration,
                    CancellationToken.None);

            StartupSequenceRunResult second =
                RunImmediate(
                    runner,
                    configuration,
                    CancellationToken.None);

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(2));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(2));

            Assert.That(
                definition.CreatedExecutorCount,
                Is.EqualTo(2));

            Assert.That(
                definition.GetCreatedExecutor(1),
                Is.Not.SameAs(
                    definition.GetCreatedExecutor(0)));

            Assert.That(
                second.GetExecution(0).Executor,
                Is.Not.SameAs(
                    first.GetExecution(0).Executor));
        }

        [Test]
        public void ContextPreservesStableIdentities()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Identity Step");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequence sequence =
                CreateSequence(entry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            RunImmediate(
                new StartupSequenceRunner(),
                configuration,
                CancellationToken.None);

            StartupStepContext context =
                definition.GetReceivedContext(0);

            Assert.That(
                context.ConfigurationId,
                Is.EqualTo(
                    configuration.ConfigurationId));

            Assert.That(
                context.SequenceId,
                Is.EqualTo(sequence.SequenceId));

            Assert.That(
                context.EntryId,
                Is.EqualTo(entry.EntryId));

            Assert.That(
                context.StepId,
                Is.EqualTo(definition.StepId));
        }

        [Test]
        public void ContextUsesAuthoredIndexAndFullCount()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Middle Step");

            StartupSequenceEntry firstDisabled =
                CreateEntry(
                    CreateDefinition(
                        "First Disabled"),
                    false,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequenceEntry middleEnabled =
                CreateEntry(
                    definition,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequenceEntry lastDisabled =
                CreateEntry(
                    CreateDefinition(
                        "Last Disabled"),
                    false,
                    StartupStepPolicy
                        .RequiredBlocking);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        firstDisabled,
                        middleEnabled,
                        lastDisabled));

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    CancellationToken.None);

            StartupStepContext context =
                definition.GetReceivedContext(0);

            Assert.That(
                context.StepIndex,
                Is.EqualTo(1));

            Assert.That(
                context.StepCount,
                Is.EqualTo(3));

            Assert.That(
                result.DisabledEntryCount,
                Is.EqualTo(2));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ContextReceivesLinkedCancellationToken()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Cancellation Step");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            definition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    source.Token);

                CancellationToken contextToken =
                    definition
                        .GetReceivedContext(0)
                        .CancellationToken;

                Assert.That(
                    contextToken.CanBeCanceled,
                    Is.True);

                Assert.That(
                    contextToken
                        .IsCancellationRequested,
                    Is.False);

                Assert.That(
                    contextToken,
                    Is.Not.EqualTo(source.Token));
            }
        }

        [Test]
        public void ImmediateProgressIsCapturedByExecution()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Progress Step");

            definition.ShouldReportProgress = true;

            definition.ProgressToReport =
                StartupStepProgress.Determinate(
                    0.7f,
                    "Immediate progress");

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            definition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    CancellationToken.None);

            StartupStepProgress progress =
                result.GetExecution(0)
                    .LatestProgress;

            Assert.That(
                progress.IsIndeterminate,
                Is.False);

            Assert.That(
                progress.Progress01,
                Is.EqualTo(0.7f));

            Assert.That(
                progress.Message,
                Is.EqualTo(
                    "Immediate progress"));
        }

        [Test]
        public void SuccessResultIsPreserved()
        {
            StartupStepResult expected =
                StartupStepResult.Success(
                    "Ready");

            StartupSequenceRunResult result =
                RunSingleResult(expected);

            Assert.That(
                result.GetExecution(0).Result,
                Is.SameAs(expected));

            Assert.That(
                result.GetExecution(0).Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));

            Assert.That(
                result.HasWarnings,
                Is.False);

            Assert.That(
                result.HasFailures,
                Is.False);
        }

        [Test]
        public void WarningResultIsPreserved()
        {
            StartupStepResult expected =
                StartupStepResult.Warning(
                    "ELAUNCH-TEST-WARN",
                    "Warning result");

            StartupSequenceRunResult result =
                RunSingleResult(expected);

            Assert.That(
                result.GetExecution(0).Result,
                Is.SameAs(expected));

            Assert.That(
                result.HasWarnings,
                Is.True);

            Assert.That(
                result.HasFailures,
                Is.False);

            Assert.That(
                result.HasBlockingFailures,
                Is.False);
        }

        [Test]
        public void RecoverableFailureWithBlockingPolicyStops()
        {
            StartupStepResult original =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-TEST-RECOVER",
                        "Recoverable result");

            StartupSequenceRunResult result =
                RunSingleResult(original);

            StartupStepResult effective =
                result.GetExecution(0).Result;

            Assert.That(
                effective,
                Is.Not.SameAs(original));

            Assert.That(
                effective.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                effective.Code,
                Is.EqualTo(original.Code));

            Assert.That(
                effective.Message,
                Is.EqualTo(original.Message));

            Assert.That(
                result.HasFailures,
                Is.True);

            Assert.That(
                result.HasBlockingFailures,
                Is.True);

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(0));
        }

        [Test]
        public void BlockingFailureResultIsPreserved()
        {
            StartupStepResult expected =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-TEST-BLOCK",
                        "Blocking result");

            StartupSequenceRunResult result =
                RunSingleResult(expected);

            Assert.That(
                result.GetExecution(0).Result,
                Is.SameAs(expected));

            Assert.That(
                result.HasFailures,
                Is.True);

            Assert.That(
                result.HasBlockingFailures,
                Is.True);

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(0));
        }

        [Test]
        public void EnabledEntriesExecuteInAuthoredOrder()
        {
            List<int> invocationOrder =
                new List<int>();

            ImmediateRunnerTestDefinition first =
                CreateDefinition("First");

            first.InvocationOrder =
                invocationOrder;

            first.InvocationToken = 1;

            ImmediateRunnerTestDefinition second =
                CreateDefinition("Second");

            second.InvocationOrder =
                invocationOrder;

            second.InvocationToken = 2;

            ImmediateRunnerTestDefinition third =
                CreateDefinition("Third");

            third.InvocationOrder =
                invocationOrder;

            third.InvocationToken = 3;

            StartupSequenceEntry firstEntry =
                CreateEntry(
                    first,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequenceEntry secondEntry =
                CreateEntry(
                    second,
                    true,
                    StartupStepPolicy
                        .OptionalWarning);

            StartupSequenceEntry thirdEntry =
                CreateEntry(
                    third,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    CreateConfiguration(
                        CreateSequence(
                            firstEntry,
                            secondEntry,
                            thirdEntry)),
                    CancellationToken.None);

            Assert.That(
                invocationOrder,
                Is.EqualTo(
                    new[] { 1, 2, 3 }));

            Assert.That(
                result.GetExecution(0).EntryId,
                Is.EqualTo(firstEntry.EntryId));

            Assert.That(
                result.GetExecution(1).EntryId,
                Is.EqualTo(secondEntry.EntryId));

            Assert.That(
                result.GetExecution(2).EntryId,
                Is.EqualTo(thirdEntry.EntryId));
        }

        [Test]
        public void RunnerStopsAfterBlockingResult()
        {
            ImmediateRunnerTestDefinition blocking =
                CreateDefinition(
                    "Blocking First");

            blocking.ResultToReturn =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-TEST-BLOCK-FIRST",
                        "First blocks");

            ImmediateRunnerTestDefinition later =
                CreateDefinition(
                    "Later Success");

            later.ResultToReturn =
                StartupStepResult.Success(
                    "Later must not run");

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(
                                blocking,
                                true,
                                StartupStepPolicy
                                    .RequiredBlocking),
                            CreateEntry(
                                later,
                                true,
                                StartupStepPolicy
                                    .RequiredBlocking))),
                    CancellationToken.None);

            Assert.That(
                blocking.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                later.ExecutionCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(0));

            Assert.That(
                result.HasBlockingFailures,
                Is.True);
        }

        [Test]
        public void NullExecutorBecomesBlockingContractResult()
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Null Executor");

            definition.ReturnNullExecutor = true;

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            definition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking)));

            StartupSequenceRunResult result =
                RunImmediate(
                    new StartupSequenceRunner(),
                    configuration,
                    CancellationToken.None);

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                execution.HasExecutor,
                Is.False);

            Assert.That(
                execution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                execution.Result.Code,
                Is.EqualTo(
                    StartupStepExceptionConverter
                        .DiagnosticCode));
        }

        [Test]
        public void RunnerDoesNotMutateAuthoredDefinitions()
        {
            ImmediateRunnerTestDefinition first =
                CreateDefinition(
                    "Immutable First");

            ImmediateRunnerTestDefinition second =
                CreateDefinition(
                    "Immutable Second");

            StartupStepPolicy firstPolicy =
                StartupStepPolicy
                    .OptionalWarning;

            StartupStepPolicy secondPolicy =
                StartupStepPolicy
                    .RequiredBlocking;

            StartupSequenceEntry firstEntry =
                CreateEntry(
                    first,
                    true,
                    firstPolicy);

            StartupSequenceEntry secondEntry =
                CreateEntry(
                    second,
                    true,
                    secondPolicy);

            StartupSequence sequence =
                CreateSequence(
                    firstEntry,
                    secondEntry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            string configurationId =
                configuration.ConfigurationId;

            string sequenceId =
                sequence.SequenceId;

            string firstEntryId =
                firstEntry.EntryId;

            string secondEntryId =
                secondEntry.EntryId;

            string firstStepId =
                first.StepId;

            string secondStepId =
                second.StepId;

            RunImmediate(
                new StartupSequenceRunner(),
                configuration,
                CancellationToken.None);

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
                sequence.EntryCount,
                Is.EqualTo(2));

            Assert.That(
                sequence.GetEntry(0),
                Is.SameAs(firstEntry));

            Assert.That(
                sequence.GetEntry(1),
                Is.SameAs(secondEntry));

            Assert.That(
                firstEntry.EntryId,
                Is.EqualTo(firstEntryId));

            Assert.That(
                secondEntry.EntryId,
                Is.EqualTo(secondEntryId));

            Assert.That(
                firstEntry.StepDefinition,
                Is.SameAs(first));

            Assert.That(
                secondEntry.StepDefinition,
                Is.SameAs(second));

            Assert.That(
                firstEntry.Policy.IsOptional,
                Is.EqualTo(
                    firstPolicy.IsOptional));

            Assert.That(
                firstEntry.Policy.FailureAction,
                Is.EqualTo(
                    firstPolicy.FailureAction));

            Assert.That(
                secondEntry.Policy.IsRequired,
                Is.EqualTo(
                    secondPolicy.IsRequired));

            Assert.That(
                secondEntry.Policy.FailureAction,
                Is.EqualTo(
                    secondPolicy.FailureAction));

            Assert.That(
                first.StepId,
                Is.EqualTo(firstStepId));

            Assert.That(
                second.StepId,
                Is.EqualTo(secondStepId));
        }

        private StartupSequenceRunResult
            RunSingleResult(
                StartupStepResult terminalResult)
        {
            ImmediateRunnerTestDefinition definition =
                CreateDefinition(
                    "Result Step");

            definition.ResultToReturn =
                terminalResult;

            return RunImmediate(
                new StartupSequenceRunner(),
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(
                            definition,
                            true,
                            StartupStepPolicy
                                .RequiredBlocking))),
                CancellationToken.None);
        }

        private static StartupSequenceRunResult
            RunImmediate(
                StartupSequenceRunner runner,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken)
        {
            Awaitable<
                StartupSequenceRunResult>.Awaiter
                awaiter =
                    runner.RunAsync(
                            LaunchMode
                                .CanonicalBoot,
                            configuration,
                            cancellationToken)
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "FL-M3-01 immediate test executors must complete synchronously.");

            return awaiter.GetResult();
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
            CreateDefinition(
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

            object activation =
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled
                        ? 0
                        : 1);

            EntryActivationField.SetValue(
                entry,
                activation);

            return entry;
        }
    }
}

//----- StartupSequenceRunnerImmediateTests.cs END -----
