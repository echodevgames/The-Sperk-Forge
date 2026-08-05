//----- StartupStepPolicyApplicationTests.cs START -----

using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class StartupStepPolicyApplicationTests
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
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void DecisionRejectsNullOriginalResult()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new StartupStepPolicyDecision(
                        null,
                        StartupStepResult.Success(),
                        true));
        }

        [Test]
        public void DecisionRejectsNullEffectiveResult()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new StartupStepPolicyDecision(
                        StartupStepResult.Success(),
                        null,
                        true));
        }

        [Test]
        public void PreservedDecisionUsesSameResult()
        {
            StartupStepResult result =
                StartupStepResult.Success(
                    "Ready");

            StartupStepPolicyDecision decision =
                new StartupStepPolicyDecision(
                    result,
                    result,
                    true);

            Assert.That(
                decision.OriginalResult,
                Is.SameAs(result));

            Assert.That(
                decision.EffectiveResult,
                Is.SameAs(result));

            Assert.That(
                decision.ShouldContinue,
                Is.True);

            Assert.That(
                decision.StopsTraversal,
                Is.False);

            Assert.That(
                decision.WasConverted,
                Is.False);
        }

        [Test]
        public void ConvertedDecisionUsesDifferentResult()
        {
            StartupStepResult original =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-POLICY-001",
                        "Original");

            StartupStepResult effective =
                StartupStepResult.Warning(
                    original.Code,
                    original.Message,
                    original.Details);

            StartupStepPolicyDecision decision =
                new StartupStepPolicyDecision(
                    original,
                    effective,
                    true);

            Assert.That(
                decision.OriginalResult,
                Is.SameAs(original));

            Assert.That(
                decision.EffectiveResult,
                Is.SameAs(effective));

            Assert.That(
                decision.WasConverted,
                Is.True);
        }

        [Test]
        public void SuccessIsPreservedAndContinues()
        {
            StartupStepResult result =
                StartupStepResult.Success(
                    "Ready");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    result);

            AssertPreservedContinuation(
                decision,
                result);
        }

        [Test]
        public void WarningIsPreservedAndContinues()
        {
            StartupStepResult result =
                StartupStepResult.Warning(
                    "ELAUNCH-POLICY-WARN",
                    "Warning");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    result);

            AssertPreservedContinuation(
                decision,
                result);
        }

        [Test]
        public void SkippedIsPreservedAndContinues()
        {
            StartupStepResult result =
                StartupStepResult.Skipped(
                    "Not needed");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    result);

            AssertPreservedContinuation(
                decision,
                result);
        }

        [Test]
        public void RecoverableFailureWithContinueBecomesWarning()
        {
            StartupStepResult original =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-POLICY-RECOVER",
                        "Recoverable");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .OptionalWarning,
                    original);

            AssertConvertedWarning(
                decision,
                original);
        }

        [Test]
        public void BlockingFailureWithContinueBecomesWarning()
        {
            StartupStepResult original =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-POLICY-BLOCK",
                        "Blocking");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .OptionalWarning,
                    original);

            AssertConvertedWarning(
                decision,
                original);
        }

        [Test]
        public void RecoverableFailureWithBlockBecomesBlocking()
        {
            StartupStepResult original =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-POLICY-RECOVER",
                        "Recoverable");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    original);

            Assert.That(
                decision.EffectiveResult,
                Is.Not.SameAs(original));

            Assert.That(
                decision.EffectiveResult.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                decision.ShouldContinue,
                Is.False);

            Assert.That(
                decision.StopsTraversal,
                Is.True);

            Assert.That(
                decision.WasConverted,
                Is.True);
        }

        [Test]
        public void BlockingFailureWithBlockIsPreserved()
        {
            StartupStepResult result =
                StartupStepResult
                    .BlockingFailure(
                        "ELAUNCH-POLICY-BLOCK",
                        "Blocking");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    result);

            Assert.That(
                decision.OriginalResult,
                Is.SameAs(result));

            Assert.That(
                decision.EffectiveResult,
                Is.SameAs(result));

            Assert.That(
                decision.ShouldContinue,
                Is.False);

            Assert.That(
                decision.WasConverted,
                Is.False);
        }

        [Test]
        public void TimedOutWithContinueBecomesWarning()
        {
            StartupStepResult original =
                StartupStepResult.TimedOut(
                    "ELAUNCH-STEP-003",
                    "Timed out");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .OptionalWarning,
                    original);

            AssertConvertedWarning(
                decision,
                original);
        }

        [Test]
        public void TimedOutWithBlockBecomesBlocking()
        {
            StartupStepResult original =
                StartupStepResult.TimedOut(
                    "ELAUNCH-STEP-003",
                    "Timed out");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .RequiredBlocking,
                    original);

            Assert.That(
                decision.EffectiveResult.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                decision.ShouldContinue,
                Is.False);

            Assert.That(
                decision.WasConverted,
                Is.True);
        }

        [Test]
        public void CancelledIsPreservedAndStops()
        {
            StartupStepResult result =
                StartupStepResult.Cancelled(
                    "ELAUNCH-POLICY-CANCEL",
                    "Cancelled");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .OptionalWarning,
                    result);

            Assert.That(
                decision.OriginalResult,
                Is.SameAs(result));

            Assert.That(
                decision.EffectiveResult,
                Is.SameAs(result));

            Assert.That(
                decision.ShouldContinue,
                Is.False);

            Assert.That(
                decision.StopsTraversal,
                Is.True);

            Assert.That(
                decision.WasConverted,
                Is.False);
        }

        [Test]
        public void ConvertedResultPreservesDiagnosticText()
        {
            StartupStepResult original =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-POLICY-TEXT",
                        "  Original message  ",
                        "  Original details  ");

            StartupStepPolicyDecision decision =
                StartupStepPolicyEvaluator.Evaluate(
                    StartupStepPolicy
                        .OptionalWarning,
                    original);

            Assert.That(
                decision.EffectiveResult.Code,
                Is.EqualTo(original.Code));

            Assert.That(
                decision.EffectiveResult.Message,
                Is.EqualTo(original.Message));

            Assert.That(
                decision.EffectiveResult.Details,
                Is.EqualTo(original.Details));
        }

        [Test]
        public void FailureActionOverridesUnusualRequirementIntent()
        {
            StartupStepPolicy requiredContinue =
                CreatePolicy(
                    true,
                    StartupStepFailureAction
                        .ContinueWithWarning);

            StartupStepPolicy optionalBlock =
                CreatePolicy(
                    false,
                    StartupStepFailureAction
                        .BlockLaunch);

            StartupStepResult failure =
                StartupStepResult
                    .RecoverableFailure(
                        "ELAUNCH-POLICY-PAIR",
                        "Failure");

            StartupStepPolicyDecision
                requiredDecision =
                    StartupStepPolicyEvaluator
                        .Evaluate(
                            requiredContinue,
                            failure);

            StartupStepPolicyDecision
                optionalDecision =
                    StartupStepPolicyEvaluator
                        .Evaluate(
                            optionalBlock,
                            failure);

            Assert.That(
                requiredContinue.IsRequired,
                Is.True);

            Assert.That(
                requiredDecision
                    .EffectiveResult.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                requiredDecision.ShouldContinue,
                Is.True);

            Assert.That(
                optionalBlock.IsOptional,
                Is.True);

            Assert.That(
                optionalDecision
                    .EffectiveResult.Status,
                Is.EqualTo(
                    StartupStepStatus
                        .BlockingFailure));

            Assert.That(
                optionalDecision.ShouldContinue,
                Is.False);
        }

        private static void AssertPreservedContinuation(
            StartupStepPolicyDecision decision,
            StartupStepResult result)
        {
            Assert.That(
                decision.OriginalResult,
                Is.SameAs(result));

            Assert.That(
                decision.EffectiveResult,
                Is.SameAs(result));

            Assert.That(
                decision.ShouldContinue,
                Is.True);

            Assert.That(
                decision.StopsTraversal,
                Is.False);

            Assert.That(
                decision.WasConverted,
                Is.False);
        }

        private static void AssertConvertedWarning(
            StartupStepPolicyDecision decision,
            StartupStepResult original)
        {
            Assert.That(
                decision.OriginalResult,
                Is.SameAs(original));

            Assert.That(
                decision.EffectiveResult,
                Is.Not.SameAs(original));

            Assert.That(
                decision.EffectiveResult.Status,
                Is.EqualTo(
                    StartupStepStatus.Warning));

            Assert.That(
                decision.EffectiveResult.Code,
                Is.EqualTo(original.Code));

            Assert.That(
                decision.EffectiveResult.Message,
                Is.EqualTo(original.Message));

            Assert.That(
                decision.EffectiveResult.Details,
                Is.EqualTo(original.Details));

            Assert.That(
                decision.ShouldContinue,
                Is.True);

            Assert.That(
                decision.StopsTraversal,
                Is.False);

            Assert.That(
                decision.WasConverted,
                Is.True);
        }

        private static StartupStepPolicy CreatePolicy(
            bool isRequired,
            StartupStepFailureAction failureAction)
        {
            object boxed =
                StartupStepPolicy.RequiredBlocking;

            PolicyRequirementField.SetValue(
                boxed,
                Enum.ToObject(
                    PolicyRequirementField.FieldType,
                    isRequired
                        ? 0
                        : 1));

            PolicyFailureActionField.SetValue(
                boxed,
                failureAction);

            PolicyTimeoutSecondsField.SetValue(
                boxed,
                0f);

            PolicyCancellationField.SetValue(
                boxed,
                Enum.ToObject(
                    PolicyCancellationField.FieldType,
                    0));

            return (StartupStepPolicy)boxed;
        }
    }
}

//----- StartupStepPolicyApplicationTests.cs END -----
