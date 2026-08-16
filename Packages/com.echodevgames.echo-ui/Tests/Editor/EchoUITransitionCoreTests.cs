using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUITransitionCoreTests
    {
        private sealed class CustomImmediateDriver : IUITransitionDriver
        {
            public string DriverId => "test-custom";
            public bool SupportsCancellation => true;
            public Awaitable<UITransitionResult> ExecuteAsync(
                UITransitionRequest request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                AwaitableCompletionSource<UITransitionResult> completion =
                    new AwaitableCompletionSource<UITransitionResult>();

                completion.SetResult(
                    UITransitionResult.ForRequest(
                        request,
                        UITransitionStatus.Completed,
                        message: "custom"));

                return completion.Awaitable;
            }
            public void ForceFinalState(UITransitionRequest request) { }
        }

        [Test]
        public void DefaultProfileUsesImmediateDriversAndHardBound()
        {
            UITransitionProfile profile = UITransitionProfile.CreateDefault();
            Assert.That(profile.EnterDriverId, Is.EqualTo(UITransitionDriverIds.Immediate));
            Assert.That(profile.ExitDriverId, Is.EqualTo(UITransitionDriverIds.Immediate));
            Assert.That(profile.HardTimeoutSeconds, Is.GreaterThan(0f));
        }

        [Test]
        public void OperationIdsAreValueStable()
        {
            Assert.That(new UITransitionOperationId(7), Is.EqualTo(new UITransitionOperationId(7)));
            Assert.That(new UITransitionOperationId(7), Is.Not.EqualTo(new UITransitionOperationId(8)));
        }

        [Test]
        public void CoordinatorRegistersExactlyTwoBuiltInDrivers()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            Assert.That(coordinator.RegisteredDriverCount, Is.EqualTo(2));
        }

        [Test]
        public void BuiltInDriversCannotBeUnregistered()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            Assert.That(coordinator.UnregisterDriver(UITransitionDriverIds.Immediate), Is.False);
            Assert.That(coordinator.UnregisterDriver(UITransitionDriverIds.CanvasGroupFade), Is.False);
        }

        [Test]
        public void CustomDriverCanBeRegisteredByStableId()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            Assert.That(coordinator.RegisterDriver(new CustomImmediateDriver()), Is.True);
            Assert.That(coordinator.TryGetDriver("test-custom", out IUITransitionDriver driver), Is.True);
            Assert.That(driver.DriverId, Is.EqualTo("test-custom"));
        }

        [Test]
        public void DuplicateDriverRegistrationIsRejected()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            Assert.That(coordinator.RegisterDriver(new CustomImmediateDriver()), Is.True);
            Assert.That(coordinator.RegisterDriver(new CustomImmediateDriver()), Is.False);
        }

        [Test]
        public void RootDefaultResolvesImmediateEnter()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Enter);
            Assert.That(policy.DriverId, Is.EqualTo(UITransitionDriverIds.Immediate));
            Assert.That(policy.DurationSeconds, Is.EqualTo(0f));
        }

        [Test]
        public void DefinitionProfileOverridesRootDefault()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionProfile definition = new UITransitionProfile(
                "definition", "test-enter", "test-exit", 0.75f, 0.5f, hardTimeoutSeconds: 3f);
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Enter, definition);
            Assert.That(policy.ProfileId, Is.EqualTo("definition"));
            Assert.That(policy.DriverId, Is.EqualTo("test-enter"));
            Assert.That(policy.DurationSeconds, Is.EqualTo(0.75f));
            Assert.That(policy.HardTimeoutSeconds, Is.EqualTo(3f));
        }

        [Test]
        public void ExitPolicyUsesIndependentExitDriverAndTiming()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionProfile definition = new UITransitionProfile(
                "definition", "enter-a", "exit-b", 0.1f, 0.9f);
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Exit, definition);
            Assert.That(policy.DriverId, Is.EqualTo("exit-b"));
            Assert.That(policy.DurationSeconds, Is.EqualTo(0.9f));
        }

        [Test]
        public void TransientOverrideWinsWithoutMutatingDefinition()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionProfile definition = new UITransitionProfile("definition", "enter-a", "exit-a", 1f, 1f);
            UITransitionProfile transient = new UITransitionProfile("runtime", "enter-b", "exit-b", 0.25f, 0.25f);
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Enter, definition, transient);
            Assert.That(policy.DriverId, Is.EqualTo("enter-b"));
            Assert.That(policy.DurationSeconds, Is.EqualTo(0.25f));
            Assert.That(definition.EnterDriverId, Is.EqualTo("enter-a"));
            Assert.That(definition.EnterDurationSeconds, Is.EqualTo(1f));
        }

        [Test]
        public void ReducedMotionCanSubstituteImmediate()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionProfile profile = new UITransitionProfile(
                "animated", "canvas-group-fade", "canvas-group-fade", 1f, 1f,
                hardTimeoutSeconds: 4f,
                reducedMotionMode: UITransitionReducedMotionMode.UseReplacement,
                reducedMotionDriverId: UITransitionDriverIds.Immediate);
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Enter, profile, reducedMotion: true);
            Assert.That(policy.ReducedMotionApplied, Is.True);
            Assert.That(policy.DriverId, Is.EqualTo(UITransitionDriverIds.Immediate));
            Assert.That(policy.DurationSeconds, Is.EqualTo(0f));
        }

        [Test]
        public void KeepAuthoredReducedMotionModePreservesDriver()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            UITransitionProfile profile = new UITransitionProfile(
                "animated", "canvas-group-fade", "canvas-group-fade", 1f, 1f,
                reducedMotionMode: UITransitionReducedMotionMode.KeepAuthored,
                reducedMotionDriverId: UITransitionDriverIds.Immediate);
            UITransitionResolvedPolicy policy = coordinator.ResolvePolicy(UITransitionDirection.Enter, profile, reducedMotion: true);
            Assert.That(policy.ReducedMotionApplied, Is.False);
            Assert.That(policy.DriverId, Is.EqualTo(UITransitionDriverIds.CanvasGroupFade));
        }

        [Test]
        public void ScreenDefinitionSnapshotsTransitionProfileAtConstruction()
        {
            UITransitionProfile profile =
                new UITransitionProfile(
                    "screen-profile",
                    "a",
                    "b",
                    0.2f,
                    0.3f);

            UIScreenDefinition definition =
                new UIScreenDefinition(
                    "screen",
                    "frontend",
                    "screen",
                    UIScreenOwnershipMode.ExternalOwned,
                    UIScreenSuspensionVisibility.Hidden,
                    transitionProfile: profile);

            Assert.That(
                definition.TransitionProfile.ProfileId,
                Is.EqualTo("screen-profile"));

            Assert.That(
                definition.TransitionProfile,
                Is.Not.SameAs(profile));
        }

        [Test]
        public void ModalDefinitionSnapshotsTransitionProfileAtConstruction()
        {
            UITransitionProfile profile =
                new UITransitionProfile(
                    "modal-profile",
                    "a",
                    "b");

            UIModalDefinition definition =
                new UIModalDefinition(
                    "modal",
                    "modal",
                    UIScreenOwnershipMode.ExternalOwned,
                    transitionProfile: profile);

            Assert.That(
                definition.TransitionProfile.ProfileId,
                Is.EqualTo("modal-profile"));

            Assert.That(
                definition.TransitionProfile,
                Is.Not.SameAs(profile));
        }

        [Test]
        public void TransitionProfileCurveIsDefensivelyCopied()
        {
            AnimationCurve source = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            UITransitionProfile profile = new UITransitionProfile("curve", "a", "b", enterCurve: source);
            AnimationCurve first = profile.EnterCurve;
            AnimationCurve second = profile.EnterCurve;
            Assert.That(first, Is.Not.SameAs(source));
            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(first.keys.Length, Is.EqualTo(source.keys.Length));
        }

        [Test]
        public void CoordinatorShutdownRejectsNewDriverRegistration()
        {
            UITransitionCoordinator coordinator = new UITransitionCoordinator();
            coordinator.Shutdown();
            Assert.That(coordinator.IsValid, Is.False);
            Assert.That(coordinator.RegisterDriver(new CustomImmediateDriver()), Is.False);
        }

        [UnityTest]
        public IEnumerator ImmediateEnterForcesVisibleAlpha()
        {
            async Awaitable Run()
            {
                GameObject go = new GameObject("transition-test", typeof(CanvasGroup), typeof(UISurface));
                try
                {
                    CanvasGroup group = go.GetComponent<CanvasGroup>();
                    group.alpha = 0.2f;
                    UITransitionCoordinator coordinator = new UITransitionCoordinator();
                    UITransitionResult result = await coordinator.ExecuteAsync(go.GetComponent<UISurface>(), UITransitionDirection.Enter);
                    Assert.That(result.Status, Is.EqualTo(UITransitionStatus.Completed));
                    Assert.That(group.alpha, Is.EqualTo(1f));
                }
                finally { UnityEngine.Object.DestroyImmediate(go); }
            }
            return Run();
        }

        [UnityTest]
        public IEnumerator ImmediateExitForcesHiddenAlpha()
        {
            async Awaitable Run()
            {
                GameObject go = new GameObject("transition-test", typeof(CanvasGroup), typeof(UISurface));
                try
                {
                    CanvasGroup group = go.GetComponent<CanvasGroup>();
                    group.alpha = 1f;
                    UITransitionCoordinator coordinator = new UITransitionCoordinator();
                    UITransitionResult result = await coordinator.ExecuteAsync(go.GetComponent<UISurface>(), UITransitionDirection.Exit);
                    Assert.That(result.Status, Is.EqualTo(UITransitionStatus.Completed));
                    Assert.That(group.alpha, Is.EqualTo(0f));
                }
                finally { UnityEngine.Object.DestroyImmediate(go); }
            }
            return Run();
        }

        [UnityTest]
        public IEnumerator SuccessiveExecutionsReceiveFreshOperationIds()
        {
            async Awaitable Run()
            {
                GameObject go = new GameObject("transition-test", typeof(CanvasGroup), typeof(UISurface));
                try
                {
                    UITransitionCoordinator coordinator = new UITransitionCoordinator();
                    UITransitionResult first = await coordinator.ExecuteAsync(go.GetComponent<UISurface>(), UITransitionDirection.Enter);
                    UITransitionResult second = await coordinator.ExecuteAsync(go.GetComponent<UISurface>(), UITransitionDirection.Exit);
                    Assert.That(first.OperationId.IsValid, Is.True);
                    Assert.That(second.OperationId.IsValid, Is.True);
                    Assert.That(first.OperationId, Is.Not.EqualTo(second.OperationId));
                    Assert.That(second.Generation, Is.GreaterThan(first.Generation));
                }
                finally { UnityEngine.Object.DestroyImmediate(go); }
            }
            return Run();
        }

        [UnityTest]
        public IEnumerator ZeroDurationCanvasGroupFadeCompletesDeterministically()
        {
            async Awaitable Run()
            {
                GameObject go = new GameObject("transition-test", typeof(CanvasGroup), typeof(UISurface));
                try
                {
                    UITransitionCoordinator coordinator = new UITransitionCoordinator();
                    UITransitionProfile fade = new UITransitionProfile(
                        "fade", UITransitionDriverIds.CanvasGroupFade, UITransitionDriverIds.CanvasGroupFade, 0f, 0f);
                    UITransitionResult result = await coordinator.ExecuteAsync(
                        go.GetComponent<UISurface>(), UITransitionDirection.Enter, fade);
                    Assert.That(result.Status, Is.EqualTo(UITransitionStatus.Completed));
                    Assert.That(go.GetComponent<CanvasGroup>().alpha, Is.EqualTo(1f));
                }
                finally { UnityEngine.Object.DestroyImmediate(go); }
            }
            return Run();
        }

        [UnityTest]
        public IEnumerator SupersedingTokenCancelledFadeDoesNotRecancelReleasedAwaitable()
        {
            async Awaitable Run()
            {
                GameObject go =
                    new GameObject(
                        "transition-supersession-test",
                        typeof(CanvasGroup),
                        typeof(UISurface));

                UITransitionCoordinator coordinator =
                    new UITransitionCoordinator();

                try
                {
                    UISurface surface =
                        go.GetComponent<UISurface>();

                    UITransitionProfile slowFade =
                        new UITransitionProfile(
                            "slow-fade",
                            UITransitionDriverIds.CanvasGroupFade,
                            UITransitionDriverIds.CanvasGroupFade,
                            1.25f,
                            1.25f,
                            hardTimeoutSeconds: 3f);

                    Awaitable<UITransitionResult> firstAwaitable =
                        coordinator.ExecuteAsync(
                            surface,
                            UITransitionDirection.Enter,
                            slowFade);

                    await Awaitable.NextFrameAsync();

                    UITransitionResult second =
                        await coordinator.ExecuteAsync(
                            surface,
                            UITransitionDirection.Enter);

                    UITransitionResult first =
                        await firstAwaitable;

                    Assert.That(
                        first.Status,
                        Is.EqualTo(UITransitionStatus.Stale));

                    Assert.That(
                        second.Status,
                        Is.EqualTo(UITransitionStatus.Completed));

                    Assert.That(
                        coordinator.ActiveCount,
                        Is.EqualTo(0));

                    Assert.That(
                        go.GetComponent<CanvasGroup>().alpha,
                        Is.EqualTo(1f));
                }
                finally
                {
                    coordinator.Shutdown();
                    UnityEngine.Object.DestroyImmediate(go);
                }
            }

            return Run();
        }
    }
}
