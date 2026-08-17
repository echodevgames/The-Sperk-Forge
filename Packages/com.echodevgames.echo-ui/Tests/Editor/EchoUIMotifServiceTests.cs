using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifServiceTests
    {
        private readonly List<UnityEngine.Object> created = new List<UnityEngine.Object>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < created.Count; i++)
                if (created[i] != null) UnityEngine.Object.DestroyImmediate(created[i]);
            created.Clear();
        }

        [Test]
        public void ConstructionCommitsDefaultWithoutPublishing()
        {
            UIMotifService service = CreateService();
            int events = 0;
            service.Changed += _ => events++;

            Assert.That(service.IsValid, Is.True);
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
            Assert.That(service.Revision, Is.EqualTo(1));
            Assert.That(events, Is.Zero);
        }

        [Test]
        public void NullCatalogCreatesUnavailableService()
        {
            UIMotifService service = new UIMotifService(null);
            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.first"));

            Assert.That(service.IsValid, Is.False);
            Assert.That(service.GetSnapshot().State, Is.EqualTo(UIMotifServiceState.Unavailable));
            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(result.Revision, Is.Zero);
        }

        [Test]
        public void ValidSwitchCommitsThenPublishesOnce()
        {
            UIMotifService service = CreateService();
            UIMotifServiceSnapshot observed = default;
            int events = 0;
            service.Changed += value => { observed = value; events++; };

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.second"));

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Applied));
            Assert.That(result.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(result.Revision, Is.EqualTo(2));
            Assert.That(events, Is.EqualTo(1));
            Assert.That(observed.EffectiveMotifId, Is.EqualTo(service.EffectiveMotifId));
            Assert.That(observed.Revision, Is.EqualTo(service.Revision));
        }

        [Test]
        public void UnknownSwitchAppliesFallback()
        {
            UIMotifService service = CreateService();
            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.missing"));

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.FallbackApplied));
            Assert.That(result.RequestedMotifId.Value, Is.EqualTo("motif.missing"));
            Assert.That(result.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
        }

        [Test]
        public void MissingFallbackRejectsWithoutMutationOrEvent()
        {
            UIMotifService service = CreateService(fallbackId: "");
            int events = 0;
            service.Changed += _ => events++;

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.missing"));

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
            Assert.That(service.Revision, Is.EqualTo(1));
            Assert.That(events, Is.Zero);
        }

        [Test]
        public void ReapplyingEffectiveMotifIsSilentAndUnchanged()
        {
            UIMotifService service = CreateService();
            int events = 0;
            service.Changed += _ => events++;

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.first"));

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Unchanged));
            Assert.That(result.Revision, Is.EqualTo(1));
            Assert.That(events, Is.Zero);
        }

        [Test]
        public void ResetReturnsToDefaultAndSecondResetIsSilent()
        {
            UIMotifService service = CreateService();
            service.Switch(new UIMotifId("motif.second"));
            int events = 0;
            service.Changed += _ => events++;

            UIMotifSwitchResult first = service.Reset();
            UIMotifSwitchResult second = service.Reset();

            Assert.That(first.Status, Is.EqualTo(UIMotifSwitchStatus.Applied));
            Assert.That(second.Status, Is.EqualTo(UIMotifSwitchStatus.Unchanged));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
            Assert.That(events, Is.EqualTo(1));
        }

        [Test]
        public void ListenerFailureIsIsolatedAfterCommittedTruth()
        {
            UIMotifService service = CreateService();
            int healthyCalls = 0;
            service.Changed += _ => throw new InvalidOperationException("motif-observer");
            service.Changed += _ => healthyCalls++;
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: motif-observer");

            UIMotifSwitchResult result = service.Switch(new UIMotifId("motif.second"));

            Assert.That(result.Succeeded, Is.True);
            Assert.That(healthyCalls, Is.EqualTo(1));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
        }

        [Test]
        public void ListenerReentryIsRejectedWithoutChangingCommittedTruth()
        {
            UIMotifService service = CreateService();
            UIMotifSwitchResult nested = default;
            service.Changed += _ => nested = service.Switch(new UIMotifId("motif.first"));

            UIMotifSwitchResult outer = service.Switch(new UIMotifId("motif.second"));

            Assert.That(outer.Status, Is.EqualTo(UIMotifSwitchStatus.Applied));
            Assert.That(nested.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(service.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(service.Revision, Is.EqualTo(2));
        }

        [Test]
        public void ShutdownPublishesFinalInvalidationExactlyOnce()
        {
            UIMotifService service = CreateService();
            int events = 0;
            UIMotifServiceSnapshot observed = default;
            service.Changed += value => { events++; observed = value; };

            bool first = service.Shutdown();
            bool second = service.Shutdown();

            Assert.That(first, Is.True);
            Assert.That(second, Is.False);
            Assert.That(events, Is.EqualTo(1));
            Assert.That(observed.State, Is.EqualTo(UIMotifServiceState.Shutdown));
            Assert.That(observed.EffectiveMotifId.IsEmpty, Is.True);
            Assert.That(service.IsValid, Is.False);
        }

        [Test]
        public void ShutdownPermanentlyRejectsSwitchAndReset()
        {
            UIMotifService service = CreateService();
            service.Shutdown();

            UIMotifSwitchResult switched = service.Switch(new UIMotifId("motif.second"));
            UIMotifSwitchResult reset = service.Reset();

            Assert.That(switched.Status, Is.EqualTo(UIMotifSwitchStatus.Shutdown));
            Assert.That(reset.Status, Is.EqualTo(UIMotifSwitchStatus.Shutdown));
            Assert.That(switched.Succeeded, Is.False);
            Assert.That(service.EffectiveMotif, Is.Null);
        }

        private UIMotifService CreateService(string fallbackId = "motif.second")
        {
            UIMotifDefinition first = Track(UIMotifDefinition.CreateTransient("motif.first",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.red) }));
            UIMotifDefinition second = Track(UIMotifDefinition.CreateTransient("motif.second",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.blue) }));
            UIMotifCatalog catalog = Track(UIMotifCatalog.CreateTransient(
                "motif.first", fallbackId, new[] { first, second }));
            return new UIMotifService(catalog.CreateSnapshot(4, 4).Snapshot);
        }

        private T Track<T>(T value) where T : UnityEngine.Object
        {
            created.Add(value);
            return value;
        }
    }
}
