using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifTargetTests
    {
        private readonly List<UnityEngine.Object> created =
            new List<UnityEngine.Object>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < created.Count; i++)
                if (created[i] != null) UnityEngine.Object.DestroyImmediate(created[i]);
            created.Clear();
        }

        [Test]
        public void RegistrationImmediatelyAppliesCurrentMotif()
        {
            UIMotifService service = CreateService();
            RecordingTarget target = new RecordingTarget();

            UIMotifRegistrationHandle handle = service.RegisterTarget(target);

            Assert.That(handle.Result.Status, Is.EqualTo(UIMotifRegistrationStatus.Registered));
            Assert.That(target.Applications, Is.EqualTo(1));
            Assert.That(target.LastMotifId.Value, Is.EqualTo("motif.first"));
            Assert.That(service.RegisteredTargetCount, Is.EqualTo(1));
        }

        [Test]
        public void SwitchAppliesTargetsInRegistrationOrder()
        {
            UIMotifService service = CreateService();
            List<int> order = new List<int>();
            service.RegisterTarget(new RecordingTarget(() => order.Add(1)));
            service.RegisterTarget(new RecordingTarget(() => order.Add(2)));
            order.Clear();

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.second"));

            Assert.That(order, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(result.AppliedTargetCount, Is.EqualTo(2));
            Assert.That(result.FailedTargetCount, Is.Zero);
        }

        [Test]
        public void KeepLocalBindingIsReportedAndPreservedByTarget()
        {
            UIMotifService service = CreateService();
            RecordingTarget target = new RecordingTarget
            {
                BindingMode = UIMotifBindingMode.KeepLocal,
                LocalColor = Color.magenta
            };

            UIMotifRegistrationHandle handle = service.RegisterTarget(target);

            Assert.That(handle.Result.ApplyResult.Status, Is.EqualTo(UIMotifTargetApplyStatus.Partial));
            Assert.That(handle.Result.ApplyResult.KeptLocalBindingCount, Is.EqualTo(1));
            Assert.That(target.LocalColor, Is.EqualTo(Color.magenta));
        }

        [Test]
        public void TargetFailureDoesNotBlockHealthyTargetsOrCommittedTruth()
        {
            UIMotifService service = CreateService();
            RecordingTarget failing = new RecordingTarget { ReturnFailure = true };
            RecordingTarget healthy = new RecordingTarget();
            service.RegisterTarget(failing);
            service.RegisterTarget(healthy);

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.second"));

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Applied));
            Assert.That(result.AppliedTargetCount, Is.EqualTo(1));
            Assert.That(result.FailedTargetCount, Is.EqualTo(1));
            Assert.That(healthy.LastMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
        }

        [Test]
        public void TargetExceptionIsLoggedAndIsolated()
        {
            UIMotifService service = CreateService();
            RecordingTarget throwing = new RecordingTarget { Throw = true };
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: motif-target");

            UIMotifRegistrationHandle handle = service.RegisterTarget(throwing);

            Assert.That(handle.Result.Status, Is.EqualTo(UIMotifRegistrationStatus.RegisteredWithApplyFailure));
            Assert.That(handle.Result.ApplyResult.Status, Is.EqualTo(UIMotifTargetApplyStatus.Failed));
            Assert.That(service.RegisteredTargetCount, Is.EqualTo(1));
        }

        [Test]
        public void ReleaseIsGenerationSafeAndIdempotent()
        {
            UIMotifService service = CreateService();
            RecordingTarget target = new RecordingTarget();
            UIMotifRegistrationHandle first = service.RegisterTarget(target);
            UIMotifRegistrationHandle second = service.RegisterTarget(target);

            UIMotifRegistrationReleaseResult released = first.Release();
            UIMotifRegistrationReleaseResult repeated = first.Release();
            service.Switch(new UIMotifId("motif.second"));

            Assert.That(second.Generation, Is.GreaterThan(first.Generation));
            Assert.That(released.Status, Is.EqualTo(UIMotifRegistrationReleaseStatus.Released));
            Assert.That(repeated.Status, Is.EqualTo(UIMotifRegistrationReleaseStatus.AlreadyReleased));
            Assert.That(service.RegisteredTargetCount, Is.EqualTo(1));
            Assert.That(target.Applications, Is.EqualTo(3));
        }

        [Test]
        public void DestroyedOwnerIsPrunedAndHandleBecomesStale()
        {
            UIMotifService service = CreateService();
            GameObject owner = Track(new GameObject("Motif owner"));
            UIMotifRegistrationHandle handle =
                service.RegisterTarget(new RecordingTarget(), owner);
            UnityEngine.Object.DestroyImmediate(owner);

            int removed = service.RefreshDestroyedTargets();
            UIMotifRegistrationReleaseResult released = handle.Release();

            Assert.That(removed, Is.EqualTo(1));
            Assert.That(service.RegisteredTargetCount, Is.Zero);
            Assert.That(released.Status, Is.EqualTo(UIMotifRegistrationReleaseStatus.Stale));
        }

        [Test]
        public void DestroyedUnityTargetIsPrunedBeforeNextApplication()
        {
            UIMotifService service = CreateService();
            UnityRecordingTarget target = Track(ScriptableObject.CreateInstance<UnityRecordingTarget>());
            service.RegisterTarget(target);
            UnityEngine.Object.DestroyImmediate(target);

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.second"));

            Assert.That(result.AppliedTargetCount, Is.Zero);
            Assert.That(result.FailedTargetCount, Is.Zero);
            Assert.That(service.RegisteredTargetCount, Is.Zero);
        }

        [Test]
        public void InvalidTargetsAreRejectedWithoutRegistration()
        {
            UIMotifService service = CreateService();
            UIMotifRegistrationHandle handle = service.RegisterTarget(null);

            Assert.That(handle.Result.Status, Is.EqualTo(UIMotifRegistrationStatus.InvalidTarget));
            Assert.That(handle.Result.Succeeded, Is.False);
            Assert.That(service.RegisteredTargetCount, Is.Zero);
        }

        [Test]
        public void RegistrationApplyReentryIsRejected()
        {
            UIMotifService service = CreateService();
            UIMotifSwitchResult nested = default;
            RecordingTarget target = new RecordingTarget(
                () => nested = service.Switch(new UIMotifId("motif.second")));

            UIMotifRegistrationHandle handle = service.RegisterTarget(target);

            Assert.That(handle.Result.Succeeded, Is.True);
            Assert.That(nested.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
        }

        [Test]
        public void ShutdownClearsRegistrationsAndInvalidatesHandles()
        {
            UIMotifService service = CreateService();
            UIMotifRegistrationHandle handle =
                service.RegisterTarget(new RecordingTarget());

            service.Shutdown();
            UIMotifRegistrationReleaseResult release = handle.Release();
            UIMotifRegistrationHandle rejected =
                service.RegisterTarget(new RecordingTarget());

            Assert.That(service.RegisteredTargetCount, Is.Zero);
            Assert.That(release.Status, Is.EqualTo(UIMotifRegistrationReleaseStatus.Shutdown));
            Assert.That(rejected.Result.Status, Is.EqualTo(UIMotifRegistrationStatus.Shutdown));
        }

        private UIMotifService CreateService()
        {
            UIMotifDefinition first = Track(UIMotifDefinition.CreateTransient(
                "motif.first",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.red) }));
            UIMotifDefinition second = Track(UIMotifDefinition.CreateTransient(
                "motif.second",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.blue) }));
            UIMotifCatalog catalog = Track(UIMotifCatalog.CreateTransient(
                "motif.first", "motif.second", new[] { first, second }));
            return new UIMotifService(catalog.CreateSnapshot(4, 4).Snapshot);
        }

        private T Track<T>(T value) where T : UnityEngine.Object
        {
            created.Add(value);
            return value;
        }

        private sealed class RecordingTarget : IUIMotifTarget
        {
            private readonly Action applied;

            public RecordingTarget(Action applied = null)
            {
                this.applied = applied;
            }

            public int Applications { get; private set; }
            public UIMotifId LastMotifId { get; private set; }
            public bool ReturnFailure { get; set; }
            public bool Throw { get; set; }
            public UIMotifBindingMode BindingMode { get; set; }
            public Color LocalColor { get; set; }

            public UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot)
            {
                if (Throw) throw new InvalidOperationException("motif-target");
                Applications++;
                LastMotifId = snapshot.MotifId;
                applied?.Invoke();
                if (ReturnFailure)
                    return new UIMotifTargetApplyResult(
                        UIMotifTargetApplyStatus.Failed,
                        failedBindingCount: 1);
                if (BindingMode == UIMotifBindingMode.KeepLocal)
                    return new UIMotifTargetApplyResult(
                        UIMotifTargetApplyStatus.Partial,
                        keptLocalBindingCount: 1);
                snapshot.TryGetColor(new UIMotifTokenId("color.surface"), out Color value);
                LocalColor = value;
                return new UIMotifTargetApplyResult(
                    UIMotifTargetApplyStatus.Applied,
                    appliedBindingCount: 1);
            }
        }

        public sealed class UnityRecordingTarget : ScriptableObject, IUIMotifTarget
        {
            public UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot) =>
                new UIMotifTargetApplyResult(UIMotifTargetApplyStatus.Applied);
        }
    }
}
