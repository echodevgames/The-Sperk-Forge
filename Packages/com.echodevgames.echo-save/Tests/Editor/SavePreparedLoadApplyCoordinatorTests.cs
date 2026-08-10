
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePreparedLoadApplyCoordinatorTests
    {
        private FakePreparedLoadClock clock;
        private SavePreparedLoadStore store;
        private SaveParticipantRegistry registry;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            clock =
                new FakePreparedLoadClock(
                    new DateTimeOffset(
                        2026,
                        8,
                        10,
                        5,
                        0,
                        0,
                        TimeSpan.Zero));

            store =
                ParticipantApplyTestSupport.Store(
                    clock);

            registry =
                new SaveParticipantRegistry();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [TearDown]
        public void TearDown()
        {
            store.Dispose();
        }

        [Test]
        public void PreflightFailPolicyLeavesHandleLiveAndInvokesZeroCallbacks()
        {
            ParticipantApplyTestParticipant blocker =
                new ParticipantApplyTestParticipant(
                    "com.example.blocker",
                    SaveMissingPayloadPolicy.Fail);

            registry.Register(
                blocker);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.PreflightRejected));

            Assert.That(
                result.MutationBegan,
                Is.False);

            Assert.That(
                result.HandleConsumed,
                Is.False);

            Assert.That(
                handle.IsValid,
                Is.True);

            Assert.That(
                blocker.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void PreflightDefaultCapabilityMissingLeavesHandleLive()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.defaults",
                    SaveMissingPayloadPolicy.InitializeDefault));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.PreflightRejected));

            Assert.That(
                handle.IsValid,
                Is.True);
        }

        [Test]
        public void SuccessfulCoordinatorApplyConsumesHandleAndRejectsReplay()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            SavePreparedLoadApplyCoordinator coordinator =
                Coordinator();

            SavePreparedLoadApplyResult first =
                coordinator.Apply(
                    handle);

            SavePreparedLoadApplyResult replay =
                coordinator.Apply(
                    handle);

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                first.HandleConsumed,
                Is.True);

            Assert.That(
                replay.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.HandleUnavailable));

            Assert.That(
                participant.ApplyCalls,
                Is.EqualTo(1));
        }

        [Test]
        public void UnknownOnlyHandleCreatesNoParticipantCallbacks()
        {
            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateUnknownOnlyHandle(
                    store,
                    slotId,
                    generationId);

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.MutationBegan,
                Is.False);

            Assert.That(
                result.Entries,
                Is.Empty);

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Consumed));
        }

        [Test]
        public void MixedIgnoreAndPreparedApplyReportsBothWithoutHiddenDefault()
        {
            ParticipantApplyTestParticipant optional =
                new ParticipantApplyTestParticipant(
                    "com.example.alpha",
                    SaveMissingPayloadPolicy.Ignore);

            ParticipantApplyTestParticipant prepared =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta");

            registry.Register(
                optional);

            registry.Register(
                prepared);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"));

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Entries.Count,
                Is.EqualTo(2));

            Assert.That(
                result.Entries[0].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome.Ignored));

            Assert.That(
                result.Entries[1].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome.Applied));

            Assert.That(
                optional.ApplyCalls,
                Is.Zero);

            Assert.That(
                prepared.ApplyCalls,
                Is.EqualTo(1));
        }

        [Test]
        public void CallbackFailureConsumesHandleAndPreservesSourceIdentityInReport()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory")
                {
                    ApplyResult =
                        SaveParticipantApplyResult.Failure(
                            "failed")
                };

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.ParticipantFailed));

            Assert.That(
                result.SourceSlotId,
                Is.EqualTo(
                    slotId));

            Assert.That(
                result.SourceGenerationId,
                Is.EqualTo(
                    generationId));

            Assert.That(
                result.HandleConsumed,
                Is.True);
        }

        [Test]
        public void ExpiredHandleRejectsWithoutParticipantMutation()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            clock.Advance(
                TimeSpan.FromMinutes(6));

            SavePreparedLoadApplyResult result =
                Coordinator().Apply(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.HandleUnavailable));

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        private SavePreparedLoadApplyCoordinator Coordinator() =>
            new SavePreparedLoadApplyCoordinator(
                store,
                registry);
    }
}
