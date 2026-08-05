//----- StartupSequenceRunnerPolicyAndExceptionTests.cs START -----

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
    internal sealed class PolicyRunnerTestDefinition :
        StartupStepDefinition
    {
        public StartupStepResult ResultToReturn
        {
            get;
            set;
        } =
            StartupStepResult.Success();

        public Exception FactoryException
        {
            get;
            set;
        }

        public Exception ExecutionException
        {
            get;
            set;
        }

        public bool ReturnNullExecutor
        {
            get;
            set;
        }

        public bool ReturnNullResult
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

        public override IStartupStepExecutor
            CreateExecutor()
        {
            FactoryCallCount++;

            if (FactoryException != null)
            {
                throw FactoryException;
            }

            if (ReturnNullExecutor)
            {
                return null;
            }

            return new PolicyRunnerTestExecutor(
                this);
        }

        internal void RecordExecution()
        {
            ExecutionCallCount++;
        }
    }

    internal sealed class PolicyRunnerTestExecutor :
        IStartupStepExecutor
    {
        private readonly PolicyRunnerTestDefinition
            definition;

        internal PolicyRunnerTestExecutor(
            PolicyRunnerTestDefinition definition)
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
            definition.RecordExecution();

            if (definition.ExecutionException != null)
            {
                throw definition.ExecutionException;
            }

            if (definition.ReturnNullResult)
            {
                return null;
            }

            return definition.ResultToReturn;
        }
#pragma warning restore CS1998
    }

    public sealed class
        StartupSequenceRunnerPolicyAndExceptionTests
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
        public void FactoryExceptionBecomesBlockingDiagnostic()
        {
            PolicyRunnerTestDefinition definition =
                CreateDefinition(
                    "Factory Failure");

            definition.FactoryException =
                new InvalidOperationException(
                    "factory boom");

            StartupSequenceRunResult result =
                RunSingle(
                    definition,
                    StartupStepPolicy
                        .OptionalWarning);

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                execution.HasExecutor,
                Is.False);

            AssertBlockingDiagnostic(
                execution.Result);

            Assert.That(
                execution.Result.Details,
                Does.Contain(
                    typeof(
                        InvalidOperationException)
                        .FullName));

            Assert.That(
                execution.Result.Details,
                Does.Contain("factory boom"));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);
        }

        [Test]
        public void NullExecutorBecomesBlockingDiagnostic()
        {
            PolicyRunnerTestDefinition definition =
                CreateDefinition(
                    "Null Executor");

            definition.ReturnNullExecutor = true;

            StartupSequenceRunResult result =
                RunSingle(
                    definition,
                    StartupStepPolicy
                        .OptionalWarning);

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                execution.HasExecutor,
                Is.False);

            AssertBlockingDiagnostic(
                execution.Result);

            Assert.That(
                execution.Result.Details,
                Is.EqualTo(
                    "ContractFailure: NullExecutor"));
        }

        [Test]
        public void FactoryFailurePreventsLaterFactory()
        {
            PolicyRunnerTestDefinition failing =
                CreateDefinition(
                    "Factory Failure");

            failing.FactoryException =
                new InvalidOperationException(
                    "factory stops");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        failing,
                        true,
                        StartupStepPolicy
                            .OptionalWarning),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                failing.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ExecutorExceptionWithContinueBecomesWarning()
        {
            PolicyRunnerTestDefinition failing =
                CreateDefinition(
                    "Execution Warning");

            failing.ExecutionException =
                new InvalidOperationException(
                    "execution boom");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        failing,
                        true,
                        StartupStepPolicy
                            .OptionalWarning),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            StartupStepResult effective =
                result.GetExecution(0).Result;

            Assert.That(
                effective.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                effective.Code,
                Is.EqualTo(
                    StartupStepExceptionConverter
                        .DiagnosticCode));

            Assert.That(
                later.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                result.WasStoppedEarly,
                Is.False);
        }

        [Test]
        public void ExecutorExceptionWithBlockStops()
        {
            PolicyRunnerTestDefinition failing =
                CreateDefinition(
                    "Execution Blocking");

            failing.ExecutionException =
                new InvalidOperationException(
                    "execution blocks");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        failing,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            AssertBlockingDiagnostic(
                result.GetExecution(0).Result);

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(1));
        }

        [Test]
        public void NullExecutorResultBecomesBlockingDiagnostic()
        {
            PolicyRunnerTestDefinition definition =
                CreateDefinition(
                    "Null Result");

            definition.ReturnNullResult = true;

            StartupSequenceRunResult result =
                RunSingle(
                    definition,
                    StartupStepPolicy
                        .OptionalWarning);

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                execution.HasExecutor,
                Is.True);

            AssertBlockingDiagnostic(
                execution.Result);

            Assert.That(
                execution.Result.Details,
                Is.EqualTo(
                    "ContractFailure: NullResult"));
        }

        [Test]
        public void ExceptionDetailsExcludeStackTrace()
        {
            InvalidOperationException exception =
                new InvalidOperationException(
                    "  sanitized message  ");

            StartupStepResult result =
                StartupStepExceptionConverter
                    .Convert(
                        StartupStepExceptionPhase
                            .ExecutorExecution,
                        exception);

            Assert.That(
                result.Details,
                Does.Contain(
                    typeof(
                        InvalidOperationException)
                        .FullName));

            Assert.That(
                result.Details,
                Does.Contain(
                    "ExceptionMessage: sanitized message"));

            Assert.That(
                result.Details,
                Does.Not.Contain(" at "));

            Assert.That(
                result.Details,
                Does.Not.Contain(
                    Environment.NewLine +
                    Environment.NewLine));
        }

        [Test]
        public void CancellationExceptionIsNotConverted()
        {
            PolicyRunnerTestDefinition definition =
                CreateDefinition(
                    "Cancellation");

            definition.ExecutionException =
                new OperationCanceledException(
                    "cancelled");

            Assert.Throws<OperationCanceledException>(
                () =>
                    RunSingle(
                        definition,
                        StartupStepPolicy
                            .OptionalWarning));

            Assert.That(
                definition.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                definition.ExecutionCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ReturnedRecoverableWithContinueWarns()
        {
            PolicyRunnerTestDefinition first =
                CreateDefinition(
                    "Recoverable Continue");

            first.ResultToReturn =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-RUN-RECOVER",
                        "Recoverable");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        first,
                        true,
                        StartupStepPolicy
                            .OptionalWarning),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.GetExecution(0)
                    .Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                later.ExecutionCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ReturnedBlockingWithContinueWarns()
        {
            PolicyRunnerTestDefinition first =
                CreateDefinition(
                    "Blocking Continue");

            first.ResultToReturn =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-RUN-BLOCK",
                        "Blocking");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        first,
                        true,
                        StartupStepPolicy
                            .OptionalWarning),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.GetExecution(0)
                    .Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                later.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                result.HasBlockingFailures,
                Is.False);
        }

        [Test]
        public void ReturnedRecoverableWithBlockStops()
        {
            PolicyRunnerTestDefinition first =
                CreateDefinition(
                    "Recoverable Block");

            first.ResultToReturn =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-RUN-RECOVER",
                        "Recoverable");

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        first,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.GetExecution(0)
                    .Result.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);
        }

        [Test]
        public void ReturnedBlockingWithBlockStops()
        {
            PolicyRunnerTestDefinition first =
                CreateDefinition(
                    "Blocking Block");

            StartupStepResult blocking =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-RUN-BLOCK",
                        "Blocking");

            first.ResultToReturn =
                blocking;

            PolicyRunnerTestDefinition later =
                CreateDefinition(
                    "Later");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        first,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.GetExecution(0).Result,
                Is.SameAs(blocking));

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);
        }

        [Test]
        public void EarlyStopAccountingBalancesAuthoredEntries()
        {
            PolicyRunnerTestDefinition disabled =
                CreateDefinition(
                    "Disabled");

            PolicyRunnerTestDefinition success =
                CreateDefinition(
                    "Success");

            PolicyRunnerTestDefinition blocking =
                CreateDefinition(
                    "Blocking");

            blocking.ResultToReturn =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-RUN-STOP",
                        "Stop");

            PolicyRunnerTestDefinition unvisited =
                CreateDefinition(
                    "Unvisited");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        disabled,
                        false,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        success,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        blocking,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        unvisited,
                        false,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.AuthoredEntryCount,
                Is.EqualTo(4));

            Assert.That(
                result.DisabledEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(2));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.AttemptedExecutionCount +
                result.DisabledEntryCount +
                result.UnvisitedEntryCount,
                Is.EqualTo(
                    result.AuthoredEntryCount));

            Assert.That(
                unvisited.FactoryCallCount,
                Is.EqualTo(0));
        }

        [Test]
        public void StoppingAuthoredIndexIsRecorded()
        {
            PolicyRunnerTestDefinition disabled =
                CreateDefinition(
                    "Disabled");

            PolicyRunnerTestDefinition success =
                CreateDefinition(
                    "Success");

            PolicyRunnerTestDefinition blocking =
                CreateDefinition(
                    "Blocking");

            blocking.ResultToReturn =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-RUN-INDEX",
                        "Stop");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        disabled,
                        false,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        success,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        blocking,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(2));

            Assert.That(
                result.GetExecution(1).StepIndex,
                Is.EqualTo(2));
        }

        [Test]
        public void CompleteTraversalHasNoUnvisitedEntries()
        {
            PolicyRunnerTestDefinition success =
                CreateDefinition(
                    "Success");

            PolicyRunnerTestDefinition warning =
                CreateDefinition(
                    "Warning");

            warning.ResultToReturn =
                StartupStepResult.Warning(
                    "ELAUNCH-RUN-WARN",
                    "Warning");

            StartupSequenceRunResult result =
                Run(
                    CreateEntry(
                        success,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking),
                    CreateEntry(
                        warning,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                result.WasStoppedEarly,
                Is.False);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(-1));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(2));
        }

        [Test]
        public void RunnerDoesNotMutateAuthoredAssets()
        {
            PolicyRunnerTestDefinition first =
                CreateDefinition(
                    "First");

            first.ExecutionException =
                new InvalidOperationException(
                    "convert me");

            PolicyRunnerTestDefinition second =
                CreateDefinition(
                    "Second");

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

            RunConfiguration(configuration);

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
                firstEntry.Policy.IsOptional,
                Is.EqualTo(firstPolicy.IsOptional));

            Assert.That(
                firstEntry.Policy.FailureAction,
                Is.EqualTo(
                    firstPolicy.FailureAction));

            Assert.That(
                secondEntry.Policy.IsRequired,
                Is.EqualTo(secondPolicy.IsRequired));

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

        private StartupSequenceRunResult RunSingle(
            PolicyRunnerTestDefinition definition,
            StartupStepPolicy policy)
        {
            return Run(
                CreateEntry(
                    definition,
                    true,
                    policy));
        }

        private StartupSequenceRunResult Run(
            params StartupSequenceEntry[] entries)
        {
            return RunConfiguration(
                CreateConfiguration(
                    CreateSequence(entries)));
        }

        private static StartupSequenceRunResult
            RunConfiguration(
                EchoLaunchConfiguration configuration)
        {
            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            Awaitable<
                StartupSequenceRunResult>.Awaiter
                awaiter =
                    runner.RunAsync(
                            LaunchMode
                                .CanonicalBoot,
                            configuration,
                            CancellationToken.None)
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "FL-M3-02 policy tests use immediate executors.");

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

        private PolicyRunnerTestDefinition
            CreateDefinition(
                string displayName)
        {
            PolicyRunnerTestDefinition definition =
                ScriptableObject.CreateInstance<
                    PolicyRunnerTestDefinition>();

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

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled
                        ? 0
                        : 1));

            return entry;
        }

        private static void AssertBlockingDiagnostic(
            StartupStepResult result)
        {
            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                result.Code,
                Is.EqualTo(
                    StartupStepExceptionConverter
                        .DiagnosticCode));
        }
    }
}

//----- StartupSequenceRunnerPolicyAndExceptionTests.cs END -----
