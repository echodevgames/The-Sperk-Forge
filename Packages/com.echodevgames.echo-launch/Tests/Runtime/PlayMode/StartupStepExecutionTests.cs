//----- StartupStepExecutionTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class ExecutionStateTestExecutor :
        IStartupStepExecutor
    {
        public Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context)
        {
            throw new NotSupportedException(
                "StartupStepExecutionTests do not invoke executors.");
        }
    }

    internal sealed class ExecutionStateTestDefinition :
        StartupStepDefinition
    {
        public override IStartupStepExecutor
            CreateExecutor()
        {
            return new ExecutionStateTestExecutor();
        }
    }

    public sealed class StartupStepExecutionTests
    {
        private static readonly FieldInfo
            EntryStepDefinitionField =
                typeof(StartupSequenceEntry).GetField(
                    "stepDefinition",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryPolicyField =
                typeof(StartupSequenceEntry).GetField(
                    "policy",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionDisplayNameField =
                typeof(StartupStepDefinition).GetField(
                    "displayName",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                EntryStepDefinitionField,
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
        public void ConstructionCopiesAuthoredMetadata()
        {
            ExecutionStateTestDefinition definition =
                CreateDefinition(
                    "Initialize Services");

            StartupStepPolicy policy =
                StartupStepPolicy.OptionalWarning;

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    policy);

            StartupStepExecution execution =
                new StartupStepExecution(
                    entry,
                    2,
                    5,
                    new ExecutionStateTestExecutor());

            Assert.That(
                execution.EntryId,
                Is.EqualTo(entry.EntryId));

            Assert.That(
                execution.StepId,
                Is.EqualTo(definition.StepId));

            Assert.That(
                execution.StepDisplayName,
                Is.EqualTo("Initialize Services"));

            Assert.That(
                execution.StepIndex,
                Is.EqualTo(2));

            Assert.That(
                execution.StepCount,
                Is.EqualTo(5));

            Assert.That(
                execution.Policy.IsOptional,
                Is.True);

            Assert.That(
                execution.Policy.FailureAction,
                Is.EqualTo(
                    StartupStepFailureAction
                        .ContinueWithWarning));

            Assert.That(
                execution.Executor,
                Is.Not.Null);
        }

        [Test]
        public void NewExecutionStartsNotStarted()
        {
            StartupStepExecution execution =
                CreateExecution();

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.NotStarted));

            Assert.That(
                execution.Result,
                Is.Null);

            Assert.That(
                execution.IsComplete,
                Is.False);

            Assert.That(
                execution.LatestProgress
                    .IsIndeterminate,
                Is.True);
        }

        [Test]
        public void BeginMovesExecutionToRunning()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.Running));

            Assert.That(
                execution.IsComplete,
                Is.False);
        }

        [Test]
        public void BeginTwiceIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            Assert.Throws<InvalidOperationException>(
                () => execution.Begin());

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.Running));
        }

        [Test]
        public void ProgressBeforeBeginIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            Assert.Throws<InvalidOperationException>(
                () =>
                    execution.Report(
                        StartupStepProgress
                            .Determinate(0.25f)));

            Assert.That(
                execution.LatestProgress
                    .IsIndeterminate,
                Is.True);
        }

        [Test]
        public void ProgressWhileRunningIsCaptured()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            StartupStepProgress progress =
                StartupStepProgress.Determinate(
                    0.6f,
                    "Preparing systems");

            execution.Report(progress);

            Assert.That(
                execution.LatestProgress
                    .IsIndeterminate,
                Is.False);

            Assert.That(
                execution.LatestProgress.Progress01,
                Is.EqualTo(0.6f));

            Assert.That(
                execution.LatestProgress.Message,
                Is.EqualTo(
                    "Preparing systems"));
        }

        [Test]
        public void CompleteBeforeBeginIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            StartupStepResult result =
                StartupStepResult.Success(
                    "Ready");

            Assert.Throws<InvalidOperationException>(
                () => execution.Complete(result));

            Assert.That(
                execution.Result,
                Is.Null);
        }

        [Test]
        public void NullCompletionResultIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            Assert.Throws<ArgumentNullException>(
                () => execution.Complete(null));

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.Running));

            Assert.That(
                execution.Result,
                Is.Null);
        }

        [Test]
        public void TerminalCompletionCapturesStatusAndResult()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            StartupStepResult result =
                StartupStepResult.Warning(
                    "ELAUNCH-TEST-001",
                    "Completed with warning");

            execution.Complete(result);

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                execution.Result,
                Is.SameAs(result));

            Assert.That(
                execution.IsComplete,
                Is.True);
        }

        [Test]
        public void CompleteTwiceIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();

            StartupStepResult first =
                StartupStepResult.Success();

            execution.Complete(first);

            Assert.Throws<InvalidOperationException>(
                () =>
                    execution.Complete(
                        StartupStepResult.Success()));

            Assert.That(
                execution.Result,
                Is.SameAs(first));
        }

        [Test]
        public void ProgressAfterCompletionIsRejected()
        {
            StartupStepExecution execution =
                CreateExecution();

            execution.Begin();
            execution.Complete(
                StartupStepResult.Success());

            Assert.Throws<InvalidOperationException>(
                () =>
                    execution.Report(
                        StartupStepProgress
                            .Determinate(1f)));

            Assert.That(
                execution.Status,
                Is.EqualTo(
                    StartupStepStatus.Succeeded));
        }

        [Test]
        public void InvalidConstructionIsRejectedWithoutAssetMutation()
        {
            ExecutionStateTestDefinition definition =
                CreateDefinition(
                    "Stable Definition");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    StartupStepPolicy
                        .RequiredBlocking);

            string originalEntryId =
                entry.EntryId;

            string originalStepId =
                definition.StepId;

            StartupStepPolicy originalPolicy =
                entry.Policy;

            Assert.Throws<ArgumentNullException>(
                () =>
                    new StartupStepExecution(
                        null,
                        0,
                        1,
                        new ExecutionStateTestExecutor()));

            Assert.Throws<ArgumentNullException>(
                () =>
                    new StartupStepExecution(
                        entry,
                        0,
                        1,
                        null));

            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new StartupStepExecution(
                        entry,
                        -1,
                        1,
                        new ExecutionStateTestExecutor()));

            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new StartupStepExecution(
                        entry,
                        1,
                        1,
                        new ExecutionStateTestExecutor()));

            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new StartupStepExecution(
                        entry,
                        0,
                        0,
                        new ExecutionStateTestExecutor()));

            StartupSequenceEntry missingDefinition =
                new StartupSequenceEntry();

            Assert.Throws<ArgumentException>(
                () =>
                    new StartupStepExecution(
                        missingDefinition,
                        0,
                        1,
                        new ExecutionStateTestExecutor()));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(originalEntryId));

            Assert.That(
                definition.StepId,
                Is.EqualTo(originalStepId));

            Assert.That(
                entry.Policy.IsRequired,
                Is.EqualTo(
                    originalPolicy.IsRequired));

            Assert.That(
                entry.Policy.FailureAction,
                Is.EqualTo(
                    originalPolicy.FailureAction));
        }

        private StartupStepExecution CreateExecution()
        {
            ExecutionStateTestDefinition definition =
                CreateDefinition(
                    "Execution Test Step");

            StartupSequenceEntry entry =
                CreateEntry(
                    definition,
                    StartupStepPolicy
                        .RequiredBlocking);

            return new StartupStepExecution(
                entry,
                0,
                1,
                new ExecutionStateTestExecutor());
        }

        private ExecutionStateTestDefinition
            CreateDefinition(
                string displayName)
        {
            ExecutionStateTestDefinition definition =
                ScriptableObject.CreateInstance<
                    ExecutionStateTestDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                displayName);

            return definition;
        }

        private static StartupSequenceEntry CreateEntry(
            StartupStepDefinition definition,
            StartupStepPolicy policy)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryStepDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                policy);

            return entry;
        }
    }
}

//----- StartupStepExecutionTests.cs END -----
