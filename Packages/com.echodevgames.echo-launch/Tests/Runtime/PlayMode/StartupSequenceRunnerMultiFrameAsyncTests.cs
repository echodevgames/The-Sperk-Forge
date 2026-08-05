//----- StartupSequenceRunnerMultiFrameAsyncTests.cs START -----

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
    /// ScriptableObject definition used only by the FL-M3-04 Play Mode
    /// proof. Every factory call creates a fresh executor whose work can
    /// genuinely span Unity frames.
    /// </summary>
    internal sealed class MultiFrameAsyncTestDefinition :
        StartupStepDefinition
    {
        private MultiFrameAsyncTestExecutor lastExecutor;

        internal int FramesToWait
        {
            get;
            set;
        }

        internal int LateProgressDelayFrames
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

        internal int FactoryObservedFrame
        {
            get;
            private set;
        } = -1;

        internal int StartFrame =>
            lastExecutor == null
                ? -1
                : lastExecutor.StartFrame;

        internal int SettlementFrame =>
            lastExecutor == null
                ? -1
                : lastExecutor.SettlementFrame;

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
            FactoryObservedFrame =
                Time.frameCount;

            lastExecutor =
                new MultiFrameAsyncTestExecutor(
                    this);

            return lastExecutor;
        }

        internal void RecordExecution()
        {
            ExecutionCallCount++;
        }
    }

    /// <summary>
    /// Production-shaped executor that uses Unity Awaitable frame yields,
    /// the package cancellation token, and the package progress reporter.
    /// </summary>
    internal sealed class MultiFrameAsyncTestExecutor :
        IStartupStepExecutor
    {
        private readonly MultiFrameAsyncTestDefinition
            definition;

        internal MultiFrameAsyncTestExecutor(
            MultiFrameAsyncTestDefinition definition)
        {
            this.definition =
                definition ??
                throw new ArgumentNullException(
                    nameof(definition));
        }

        internal int StartFrame
        {
            get;
            private set;
        } = -1;

        internal int SettlementFrame
        {
            get;
            private set;
        } = -1;

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
            StartFrame = Time.frameCount;

            try
            {
                for (int frameIndex = 0;
                     frameIndex <
                     definition.FramesToWait;
                     frameIndex++)
                {
                    await Awaitable.NextFrameAsync(
                        context.CancellationToken);

                    FramesCompleted++;

                    float progress01 =
                        definition.FramesToWait == 0
                            ? 1f
                            : (float)FramesCompleted /
                              definition.FramesToWait;

                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            progress01,
                            $"Frame {FramesCompleted} of {definition.FramesToWait}"));
                }

                return StartupStepResult.Success(
                    "Multi-frame startup work completed.");
            }
            catch (OperationCanceledException)
            {
                CancellationObserved = true;

                for (int delayIndex = 0;
                     delayIndex <
                     definition.LateProgressDelayFrames;
                     delayIndex++)
                {
                    await Awaitable.NextFrameAsync(
                        CancellationToken.None);
                }

                if (definition
                    .LateProgressDelayFrames > 0)
                {
                    context.ProgressReporter.Report(
                        StartupStepProgress.Determinate(
                            0.99f,
                            "Late cancellation progress"));
                }

                throw;
            }
            finally
            {
                Settled = true;
                SettlementFrame =
                    Time.frameCount;
            }
        }
    }

    /// <summary>
    /// FL-M3-04 proof that the runner handles genuine frame-spanning work
    /// and caller cancellation without involving EchoLaunchRoot or scenes.
    /// </summary>
    public sealed class
        StartupSequenceRunnerMultiFrameAsyncTests
    {
        private const int MaximumFramesToWait = 60;

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

        [UnityTest]
        public IEnumerator
            MultiFrameExecutionPreservesProgressTimingAndAuthoredOrder()
        {
            MultiFrameAsyncTestDefinition first =
                CreateDefinition(
                    "Three Frame Step",
                    3);

            MultiFrameAsyncTestDefinition second =
                CreateDefinition(
                    "Immediate Step",
                    0);

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
                        .RequiredBlocking);

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

            string firstStepId =
                first.StepId;

            int runStartFrame =
                Time.frameCount;

            StartupSequenceRunner runner =
                new StartupSequenceRunner();

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    runner.RunAsync(
                            LaunchMode.CanonicalBoot,
                            configuration,
                            CancellationToken.None)
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.False,
                "A three-frame executor must not complete synchronously.");

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
                "The multi-frame startup sequence did not settle within the bounded Play Mode proof window.");

            StartupSequenceRunResult result =
                awaiter.GetResult();

            StartupStepExecution firstExecution =
                result.GetExecution(0);

            Assert.That(
                first.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                first.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                first.FramesCompleted,
                Is.EqualTo(3));

            Assert.That(
                first.Settled,
                Is.True);

            Assert.That(
                first.SettlementFrame -
                first.StartFrame,
                Is.GreaterThanOrEqualTo(3));

            Assert.That(
                Time.frameCount -
                runStartFrame,
                Is.GreaterThanOrEqualTo(3));

            Assert.That(
                firstExecution.Result.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));

            Assert.That(
                firstExecution.LatestProgress
                    .IsIndeterminate,
                Is.False);

            Assert.That(
                firstExecution.LatestProgress
                    .Progress01,
                Is.EqualTo(1f));

            Assert.That(
                firstExecution.LatestProgress
                    .Message,
                Is.EqualTo("Frame 3 of 3"));

            Assert.That(
                firstExecution.Timing
                    .ElapsedSeconds,
                Is.GreaterThan(0d));

            Assert.That(
                firstExecution.Timing.TimedOut,
                Is.False);

            Assert.That(
                second.FactoryCallCount,
                Is.EqualTo(1));

            Assert.That(
                second.ExecutionCallCount,
                Is.EqualTo(1));

            Assert.That(
                second.FactoryObservedFrame,
                Is.GreaterThanOrEqualTo(
                    first.SettlementFrame));

            Assert.That(
                result.AttemptedExecutionCount,
                Is.EqualTo(2));

            Assert.That(
                result.UnvisitedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.WasStoppedEarly,
                Is.False);

            Assert.That(
                result.WasCancelled,
                Is.False);

            AssertAuthoredStateUnchanged(
                configuration,
                configurationId,
                sequence,
                sequenceId,
                firstEntry,
                firstEntryId,
                first,
                firstStepId);
        }

        [UnityTest]
        public IEnumerator
            CallerCancellationReturnsAfterSettlementAndLeavesLaterStepUnvisited()
        {
            MultiFrameAsyncTestDefinition first =
                CreateDefinition(
                    "Cancellable Step",
                    20);

            first.LateProgressDelayFrames = 2;

            MultiFrameAsyncTestDefinition later =
                CreateDefinition(
                    "Later Step",
                    0);

            StartupStepPolicy cancellationPolicy =
                CreatePolicy(
                    StartupStepFailureAction
                        .ContinueWithWarning,
                    5f,
                    true);

            StartupSequenceEntry firstEntry =
                CreateEntry(
                    first,
                    true,
                    cancellationPolicy);

            StartupSequenceEntry laterEntry =
                CreateEntry(
                    later,
                    true,
                    StartupStepPolicy
                        .RequiredBlocking);

            StartupSequence sequence =
                CreateSequence(
                    firstEntry,
                    laterEntry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            string configurationId =
                configuration.ConfigurationId;

            string sequenceId =
                sequence.SequenceId;

            string firstEntryId =
                firstEntry.EntryId;

            string firstStepId =
                first.StepId;

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                StartupSequenceRunner runner =
                    new StartupSequenceRunner();

                Awaitable<
                    StartupSequenceRunResult>.Awaiter
                    awaiter =
                        runner.RunAsync(
                                LaunchMode.CanonicalBoot,
                                configuration,
                                source.Token)
                            .GetAwaiter();

                int preCancellationWait = 0;

                while (first.FramesCompleted < 1 &&
                       !awaiter.IsCompleted &&
                       preCancellationWait <
                       MaximumFramesToWait)
                {
                    preCancellationWait++;
                    yield return null;
                }

                Assert.That(
                    first.FramesCompleted,
                    Is.GreaterThanOrEqualTo(1),
                    "The executor must report accepted progress before caller cancellation is requested.");

                Assert.That(
                    awaiter.IsCompleted,
                    Is.False);

                int acceptedFrames =
                    first.FramesCompleted;

                float acceptedProgress =
                    (float)acceptedFrames /
                    first.FramesToWait;

                string acceptedMessage =
                    $"Frame {acceptedFrames} of {first.FramesToWait}";

                int cancellationFrame =
                    Time.frameCount;

                source.Cancel();

                int postCancellationWait = 0;

                while (!awaiter.IsCompleted &&
                       postCancellationWait <
                       MaximumFramesToWait)
                {
                    postCancellationWait++;
                    yield return null;
                }

                Assert.That(
                    awaiter.IsCompleted,
                    Is.True,
                    "The runner did not return after the cancelled executor settled.");

                StartupSequenceRunResult result =
                    awaiter.GetResult();

                StartupStepExecution execution =
                    result.GetExecution(0);

                Assert.That(
                    first.CancellationObserved,
                    Is.True);

                Assert.That(
                    first.Settled,
                    Is.True);

                Assert.That(
                    first.SettlementFrame -
                    cancellationFrame,
                    Is.GreaterThanOrEqualTo(2));

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
                    execution.LatestProgress
                        .Progress01,
                    Is.EqualTo(acceptedProgress));

                Assert.That(
                    execution.LatestProgress
                        .Message,
                    Is.EqualTo(acceptedMessage));

                Assert.That(
                    execution.LatestProgress
                        .Message,
                    Is.Not.EqualTo(
                        "Late cancellation progress"));

                Assert.That(
                    execution.Timing.TimedOut,
                    Is.False);

                Assert.That(
                    execution.Timing
                        .CancellationRequested,
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
                    result.UnvisitedEntryCount,
                    Is.EqualTo(1));

                Assert.That(
                    later.FactoryCallCount,
                    Is.EqualTo(0));

                Assert.That(
                    later.ExecutionCallCount,
                    Is.EqualTo(0));

                Assert.That(
                    firstEntry.Policy.FailureAction,
                    Is.EqualTo(
                        StartupStepFailureAction
                            .ContinueWithWarning));

                Assert.That(
                    execution.Result.Status,
                    Is.Not.EqualTo(
                        StartupStepStatus.Warning));

                AssertAuthoredStateUnchanged(
                    configuration,
                    configurationId,
                    sequence,
                    sequenceId,
                    firstEntry,
                    firstEntryId,
                    first,
                    firstStepId);
            }
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

        private MultiFrameAsyncTestDefinition
            CreateDefinition(
                string displayName,
                int framesToWait)
        {
            if (framesToWait < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(framesToWait));
            }

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

        private static void AssertAuthoredStateUnchanged(
            EchoLaunchConfiguration configuration,
            string configurationId,
            StartupSequence sequence,
            string sequenceId,
            StartupSequenceEntry entry,
            string entryId,
            StartupStepDefinition definition,
            string stepId)
        {
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
                definition.StepId,
                Is.EqualTo(stepId));
        }
    }
}

//----- StartupSequenceRunnerMultiFrameAsyncTests.cs END -----
