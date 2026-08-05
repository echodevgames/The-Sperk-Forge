//----- StartupSequenceRunnerTimeoutTests.cs START -----

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
    internal sealed class TimeoutTestClock :
        ILaunchClock
    {
        private readonly Queue<double>
            queuedTimes =
                new Queue<double>();

        internal TimeoutTestClock(
            double initialSeconds,
            double secondsPerTick = 1d)
        {
            CurrentSeconds = initialSeconds;
            SecondsPerTick = secondsPerTick;
        }

        internal double CurrentSeconds
        {
            get;
            private set;
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

        internal Action<int> OnTick
        {
            get;
            set;
        }

        internal void QueueTime(
            double seconds)
        {
            queuedTimes.Enqueue(seconds);
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

            CurrentSeconds =
                queuedTimes.Count > 0
                    ? queuedTimes.Dequeue()
                    : CurrentSeconds +
                      SecondsPerTick;

            OnTick?.Invoke(TickCount);
        }
#pragma warning restore CS1998
    }

    internal sealed class TimeoutTestDefinition :
        StartupStepDefinition
    {
        private TimeoutTestExecutor lastExecutor;

        public StartupStepResult ImmediateResult
        {
            get;
            set;
        } =
            StartupStepResult.Success();

        public bool CompleteImmediately
        {
            get;
            set;
        } = true;

        public bool ObserveCancellation
        {
            get;
            set;
        }

        public Func<int> TickReader
        {
            get;
            set;
        }

        public int FactoryObservedTick
        {
            get;
            private set;
        } = -1;

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

        public int CancellationObservationCount =>
            lastExecutor == null
                ? 0
                : lastExecutor
                    .CancellationObservationCount;

        public StartupStepContext LastContext =>
            lastExecutor?.Context;

        public override IStartupStepExecutor
            CreateExecutor()
        {
            FactoryCallCount++;

            if (TickReader != null)
            {
                FactoryObservedTick =
                    TickReader();
            }

            lastExecutor =
                new TimeoutTestExecutor(this);

            return lastExecutor;
        }

        internal void RecordExecution()
        {
            ExecutionCallCount++;
        }

        internal void Complete(
            StartupStepResult result)
        {
            lastExecutor.Complete(result);
        }

        internal void Fail(
            Exception exception)
        {
            lastExecutor.Fail(exception);
        }

        internal void Report(
            StartupStepProgress progress)
        {
            LastContext.ProgressReporter.Report(
                progress);
        }
    }

    internal sealed class TimeoutTestExecutor :
        IStartupStepExecutor
    {
        private readonly TimeoutTestDefinition
            definition;

        private readonly AwaitableCompletionSource<
            StartupStepResult>
            completionSource =
                new AwaitableCompletionSource<
                    StartupStepResult>();

        private bool settled;

        internal TimeoutTestExecutor(
            TimeoutTestDefinition definition)
        {
            this.definition =
                definition ??
                throw new ArgumentNullException(
                    nameof(definition));
        }

        internal StartupStepContext Context
        {
            get;
            private set;
        }

        internal int CancellationObservationCount
        {
            get;
            private set;
        }

        public Awaitable<StartupStepResult>
            ExecuteAsync(
                StartupStepContext context)
        {
            Context = context;
            definition.RecordExecution();

            if (definition.CompleteImmediately)
            {
                return CompleteImmediatelyAsync(
                    definition.ImmediateResult);
            }

            if (definition.ObserveCancellation)
            {
                context.CancellationToken.Register(
                    ObserveCancellation);
            }

            return completionSource.Awaitable;
        }

#pragma warning disable CS1998
        private static async Awaitable<
            StartupStepResult>
            CompleteImmediatelyAsync(
                StartupStepResult result)
        {
            return result;
        }
#pragma warning restore CS1998

        internal void Complete(
            StartupStepResult result)
        {
            if (settled)
            {
                return;
            }

            settled = true;
            completionSource.SetResult(
                result);
        }

        internal void Fail(
            Exception exception)
        {
            if (settled)
            {
                return;
            }

            settled = true;
            completionSource.SetException(
                exception);
        }

        private void ObserveCancellation()
        {
            CancellationObservationCount++;

            if (settled)
            {
                return;
            }

            settled = true;
            completionSource.SetCanceled();
        }
    }

    public sealed class StartupSequenceRunnerTimeoutTests
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
        public void ZeroTimeoutLeavesDelayedResultUnchanged()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed("No Timeout");

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        definition.Complete(
                            StartupStepResult.Success(
                                "Ready"));
                    }
                };

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        0f,
                        true))
                    .GetExecution(0);

            Assert.That(
                execution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));

            Assert.That(
                execution.Timing.HasTimeout,
                Is.False);

            Assert.That(
                execution.Timing.ElapsedSeconds,
                Is.EqualTo(3d));
        }

        [Test]
        public void CompletionBeforeDeadlineWins()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Before Deadline");

            clock.OnTick =
                tick =>
                {
                    if (tick == 2)
                    {
                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        5f,
                        true))
                    .GetExecution(0);

            Assert.That(
                execution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));

            Assert.That(
                execution.Timing.TimedOut,
                Is.False);
        }

        [Test]
        public void CompletionObservableAtDeadlineWins()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Boundary");

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        3f,
                        true))
                    .GetExecution(0);

            Assert.That(
                execution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));

            Assert.That(
                execution.Timing.ElapsedSeconds,
                Is.EqualTo(3d));

            Assert.That(
                execution.Timing.TimedOut,
                Is.False);
        }

        [Test]
        public void DeadlineCrossingProducesTimeout()
        {
            StartupSequenceRunResult result =
                RunBlockingTimeout();

            Assert.That(
                result.GetExecution(0)
                    .Timing.TimedOut,
                Is.True);

            Assert.That(
                result.GetExecution(0)
                    .Result.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                result.WasStoppedEarly,
                Is.True);
        }

        [Test]
        public void TimeoutUsesStableDiagnosticCode()
        {
            StartupStepResult result =
                RunBlockingTimeout()
                    .GetExecution(0)
                    .Result;

            Assert.That(
                result.Code,
                Is.EqualTo(
                    "ELAUNCH-STEP-003"));

            Assert.That(
                result.Message,
                Is.EqualTo(
                    "The startup step exceeded its configured timeout."));
        }

        [Test]
        public void TimeoutDetailsContainMeasuredFacts()
        {
            StartupStepResult result =
                RunBlockingTimeout()
                    .GetExecution(0)
                    .Result;

            Assert.That(
                result.Details,
                Does.Contain(
                    "TimeoutSeconds: 2"));

            Assert.That(
                result.Details,
                Does.Contain(
                    "ElapsedSeconds: 3"));

            Assert.That(
                result.Details,
                Does.Contain(
                    "CancellationRequested: False"));
        }

        [Test]
        public void SupportedTimeoutRequestsCancellationOnce()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Cancellable");

            definition.ObserveCancellation = true;

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        2f,
                        true))
                    .GetExecution(0);

            Assert.That(
                definition
                    .CancellationObservationCount,
                Is.EqualTo(1));

            Assert.That(
                execution.Timing
                    .CancellationRequested,
                Is.True);

            Assert.That(
                execution.Timing
                    .SettlementSeconds,
                Is.EqualTo(2d));
        }

        [Test]
        public void UnsupportedTimeoutDoesNotRequestCancellation()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Unsupported");

            definition.ObserveCancellation = true;

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        2f,
                        false))
                    .GetExecution(0);

            Assert.That(
                definition
                    .CancellationObservationCount,
                Is.EqualTo(0));

            Assert.That(
                execution.Timing
                    .CancellationRequested,
                Is.False);
        }

        [Test]
        public void LateSuccessCannotReplaceTimeout()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Late Success");

            clock.OnTick =
                tick =>
                {
                    if (tick == 4)
                    {
                        definition.Complete(
                            StartupStepResult.Success(
                                "Too late"));
                    }
                };

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        2f,
                        false))
                    .GetExecution(0);

            Assert.That(
                execution.Result.Code,
                Is.EqualTo(
                    "ELAUNCH-STEP-003"));

            Assert.That(
                execution.Timing.ElapsedSeconds,
                Is.EqualTo(4d));
        }

        [Test]
        public void LateFailureCannotReplaceTimeout()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Late Failure");

            clock.OnTick =
                tick =>
                {
                    if (tick == 4)
                    {
                        definition.Fail(
                            new InvalidOperationException(
                                "late failure"));
                    }
                };

            StartupStepResult result =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        2f,
                        false))
                    .GetExecution(0)
                    .Result;

            Assert.That(
                result.Code,
                Is.EqualTo(
                    "ELAUNCH-STEP-003"));

            Assert.That(
                result.Details,
                Does.Not.Contain(
                    "late failure"));
        }

        [Test]
        public void TimeoutCancellationExceptionBecomesTimeout()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Cancellation");

            definition.ObserveCancellation = true;

            StartupStepExecution execution =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        1f,
                        true))
                    .GetExecution(0);

            Assert.That(
                execution.Result.Code,
                Is.EqualTo(
                    "ELAUNCH-STEP-003"));

            Assert.That(
                execution.Timing.TimedOut,
                Is.True);
        }

        [Test]
        public void CallerCancellationReturnsStructuredOutcome()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Caller Cancellation");

            definition.ObserveCancellation = true;

            StartupSequenceRunResult result;

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                clock.OnTick =
                    tick =>
                    {
                        if (tick == 1)
                        {
                            source.Cancel();
                        }
                    };

                result =
                    RunSingle(
                        clock,
                        definition,
                        CreatePolicy(
                            StartupStepFailureAction
                                .ContinueWithWarning,
                            5f,
                            true),
                        source.Token);
            }

            StartupStepExecution execution =
                result.GetExecution(0);

            Assert.That(
                definition.CancellationObservationCount,
                Is.EqualTo(1));

            Assert.That(
                execution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Cancelled));

            Assert.That(
                execution.Result.Code,
                Is.EqualTo(
                    "ELAUNCH-STEP-005"));

            Assert.That(
                execution.Result.Message,
                Is.EqualTo(
                    "Startup-sequence execution was cancelled by the caller."));

            Assert.That(
                execution.Result.Details,
                Does.Contain(
                    "ExecutorCompletedWithoutException: False"));

            Assert.That(
                execution.Timing.TimedOut,
                Is.False);

            Assert.That(
                result.WasCancelled,
                Is.True);

            Assert.That(
                result.WasStoppedEarly,
                Is.True);

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(0));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(1));

            Assert.That(
                execution.Result.Status,
                Is.Not.EqualTo(
                    StartupStepStatus.Warning));
        }
        [Test]
        public void ContinueWithWarningTimeoutRunsLaterStep()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition first =
                CreateDelayed(
                    "Optional Timeout");

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        first.Complete(
                            StartupStepResult.Success());
                    }
                };

            TimeoutTestDefinition later =
                CreateImmediate("Later");

            StartupSequenceRunResult result =
                Run(
                    clock,
                    CancellationToken.None,
                    CreateEntry(
                        first,
                        true,
                        CreatePolicy(
                            StartupStepFailureAction
                                .ContinueWithWarning,
                            2f,
                            false)),
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
                result.WasStoppedEarly,
                Is.False);
        }

        [Test]
        public void BlockLaunchTimeoutLeavesLaterStepUnvisited()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition first =
                CreateDelayed(
                    "Blocking Timeout");

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        first.Complete(
                            StartupStepResult.Success());
                    }
                };

            TimeoutTestDefinition later =
                CreateImmediate("Later");

            StartupSequenceRunResult result =
                Run(
                    clock,
                    CancellationToken.None,
                    CreateEntry(
                        first,
                        true,
                        CreatePolicy(
                            StartupStepFailureAction
                                .BlockLaunch,
                            2f,
                            false)),
                    CreateEntry(
                        later,
                        true,
                        StartupStepPolicy
                            .RequiredBlocking));

            Assert.That(
                later.FactoryCallCount,
                Is.EqualTo(0));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.StoppingAuthoredEntryIndex,
                Is.EqualTo(0));
        }

        [Test]
        public void LateProgressIsIgnoredAfterTimeout()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Progress Timeout");

            clock.OnTick =
                tick =>
                {
                    if (tick == 1)
                    {
                        definition.Report(
                            StartupStepProgress
                                .Determinate(
                                    0.25f,
                                    "Before timeout"));
                    }

                    if (tick == 3)
                    {
                        definition.Report(
                            StartupStepProgress
                                .Determinate(
                                    0.9f,
                                    "Late progress"));

                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            StartupStepProgress progress =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .BlockLaunch,
                        2f,
                        false))
                    .GetExecution(0)
                    .LatestProgress;

            Assert.That(
                progress.Progress01,
                Is.EqualTo(0.25f));

            Assert.That(
                progress.Message,
                Is.EqualTo(
                    "Before timeout"));
        }

        [Test]
        public void BackwardClockBecomesBlockingContractFailure()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(2d);

            clock.QueueTime(1d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Backward Clock");

            definition.ObserveCancellation = true;

            StartupSequenceRunResult result =
                RunSingle(
                    clock,
                    definition,
                    CreatePolicy(
                        StartupStepFailureAction
                            .ContinueWithWarning,
                        5f,
                        true));

            StartupStepResult stepResult =
                result.GetExecution(0)
                    .Result;

            Assert.That(
                stepResult.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                stepResult.Code,
                Is.EqualTo(
                    StartupStepExceptionConverter
                        .DiagnosticCode));

            Assert.That(
                stepResult.Details,
                Does.Contain("moved backward"));
        }

        [Test]
        public void TimeoutRunDoesNotMutateAuthoredAssets()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Immutable");

            StartupStepPolicy policy =
                CreatePolicy(
                    StartupStepFailureAction
                        .ContinueWithWarning,
                    2.5f,
                    false);

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    true,
                    policy);

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

            clock.OnTick =
                tick =>
                {
                    if (tick == 4)
                    {
                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            RunConfiguration(
                clock,
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
                sequence.GetEntry(0),
                Is.SameAs(entry));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(entryId));

            Assert.That(
                entry.Policy.TimeoutSeconds,
                Is.EqualTo(2.5f));

            Assert.That(
                entry.Policy.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .ContinueWithWarning));

            Assert.That(
                entry.Policy.SupportsCancellation,
                Is.False);

            Assert.That(
                definition.StepId,
                Is.EqualTo(stepId));
        }

        [Test]
        public void ContinueTimeoutWaitsForSettlementBeforeLaterFactory()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition first =
                CreateDelayed(
                    "Slow Timeout");

            clock.OnTick =
                tick =>
                {
                    if (tick == 4)
                    {
                        first.Complete(
                            StartupStepResult.Success());
                    }
                };

            TimeoutTestDefinition later =
                CreateImmediate("Later");

            later.TickReader =
                () => clock.TickCount;

            Run(
                clock,
                CancellationToken.None,
                CreateEntry(
                    first,
                    true,
                    CreatePolicy(
                        StartupStepFailureAction
                            .ContinueWithWarning,
                        2f,
                        false)),
                CreateEntry(
                    later,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking));

            Assert.That(
                later.FactoryObservedTick,
                Is.EqualTo(4));

            Assert.That(
                first.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                later.ExecutionCallCount,
                Is.EqualTo(1));
        }

        private StartupSequenceRunResult
            RunBlockingTimeout()
        {
            TimeoutTestClock clock =
                new TimeoutTestClock(0d);

            TimeoutTestDefinition definition =
                CreateDelayed(
                    "Blocking Evidence");

            clock.OnTick =
                tick =>
                {
                    if (tick == 3)
                    {
                        definition.Complete(
                            StartupStepResult.Success());
                    }
                };

            return RunSingle(
                clock,
                definition,
                CreatePolicy(
                    StartupStepFailureAction
                        .BlockLaunch,
                    2f,
                    false));
        }

        private StartupSequenceRunResult RunSingle(
            TimeoutTestClock clock,
            TimeoutTestDefinition definition,
            StartupStepPolicy policy,
            CancellationToken cancellationToken =
                default)
        {
            return Run(
                clock,
                cancellationToken,
                CreateEntry(
                    definition,
                    true,
                    policy));
        }

        private StartupSequenceRunResult Run(
            TimeoutTestClock clock,
            CancellationToken cancellationToken,
            params StartupSequenceEntry[] entries)
        {
            return RunConfiguration(
                clock,
                CreateConfiguration(
                    CreateSequence(entries)),
                cancellationToken);
        }

        private static StartupSequenceRunResult
            RunConfiguration(
                ILaunchClock clock,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken)
        {
            StartupSequenceRunner runner =
                new StartupSequenceRunner(clock);

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
                "Manual-clock timeout tests must settle synchronously.");

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

        private TimeoutTestDefinition CreateDelayed(
            string displayName)
        {
            TimeoutTestDefinition definition =
                CreateDefinition(displayName);

            definition.CompleteImmediately =
                false;

            return definition;
        }

        private TimeoutTestDefinition CreateImmediate(
            string displayName)
        {
            return CreateDefinition(displayName);
        }

        private TimeoutTestDefinition CreateDefinition(
            string displayName)
        {
            TimeoutTestDefinition definition =
                ScriptableObject.CreateInstance<
                    TimeoutTestDefinition>();

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

        private static StartupStepPolicy CreatePolicy(
            StartupStepFailureAction failureAction,
            float timeoutSeconds,
            bool supportsCancellation)
        {
            object boxed =
                StartupStepPolicy.RequiredBlocking;

            PolicyRequirementField.SetValue(
                boxed,
                Enum.ToObject(
                    PolicyRequirementField.FieldType,
                    0));

            PolicyFailureActionField.SetValue(
                boxed,
                failureAction);

            PolicyTimeoutSecondsField.SetValue(
                boxed,
                timeoutSeconds);

            PolicyCancellationField.SetValue(
                boxed,
                Enum.ToObject(
                    PolicyCancellationField.FieldType,
                    supportsCancellation
                        ? 0
                        : 1));

            return (StartupStepPolicy)boxed;
        }
    }
}

//----- StartupSequenceRunnerTimeoutTests.cs END -----
