
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantApplyPlannerTests
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
                        3,
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
        public void PlannerOrdersActionsByCurrentCanonicalRegistryOrder()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.zeta"));

            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.alpha"));

            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.middle"));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"),
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.alpha"),
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.middle"));

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Plan.Steps[0].ParticipantId.Value,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                result.Plan.Steps[1].ParticipantId.Value,
                Is.EqualTo(
                    "com.example.middle"));

            Assert.That(
                result.Plan.Steps[2].ParticipantId.Value,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void PlanningInvokesZeroApplyOrDefaultCallbacks()
        {
            DefaultableParticipantApplyTestParticipant participant =
                new DefaultableParticipantApplyTestParticipant(
                    "com.example.defaults");

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            Assert.That(
                Planner().Plan(
                    handle)
                    .Succeeded,
                Is.True);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);

            Assert.That(
                participant.InitializeDefaultCalls,
                Is.Zero);
        }

        [Test]
        public void PreparedOwnerMissingRejectsBeforeMutation()
        {
            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .ParticipantUnavailable));

            Assert.That(
                handle.IsValid,
                Is.True);
        }

        [Test]
        public void PreparedTypeMismatchRejects()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory")
                {
                    DetachedStateType =
                        typeof(string)
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

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .StateIncompatible));

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void PreparedSchemaMismatchRejects()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.inventory",
                    SaveMissingPayloadPolicy.Ignore,
                    2));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory",
                        schemaVersion: 1));

            Assert.That(
                Planner().Plan(
                    handle)
                    .Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .StateIncompatible));
        }

        [Test]
        public void DuplicatePreparedCanonicalParticipantRejects()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.inventory"));

            SavePreparedParticipantEntry first =
                ParticipantApplyTestSupport.PreparedEntry(
                    "com.example.inventory",
                    1);

            SavePreparedParticipantEntry second =
                ParticipantApplyTestSupport.PreparedEntry(
                    "com.example.inventory",
                    2);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    first,
                    second);

            Assert.That(
                Planner().Plan(
                    handle)
                    .Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .DuplicatePreparedParticipant));
        }

        [Test]
        public void MissingPayloadFailBlocksWholePlan()
        {
            ParticipantApplyTestParticipant prepared =
                new ParticipantApplyTestParticipant(
                    "com.example.alpha");

            ParticipantApplyTestParticipant blocker =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta",
                    SaveMissingPayloadPolicy.Fail);

            registry.Register(
                prepared);

            registry.Register(
                blocker);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.alpha"));

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .MissingPayloadBlocked));

            Assert.That(
                prepared.ApplyCalls,
                Is.Zero);

            Assert.That(
                blocker.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void MissingDefaultCapabilityRejectsWholePlan()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.defaults",
                    SaveMissingPayloadPolicy.InitializeDefault);

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .DefaultCapabilityMissing));

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void MissingIgnoreProducesIgnoreAction()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.optional",
                    SaveMissingPayloadPolicy.Ignore));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Plan.Count,
                Is.EqualTo(1));

            Assert.That(
                result.Plan.Steps[0].Action,
                Is.EqualTo(
                    SaveParticipantApplyActionKind.Ignore));
        }

        [Test]
        public void MissingDefaultProducesExplicitDefaultAction()
        {
            registry.Register(
                new DefaultableParticipantApplyTestParticipant(
                    "com.example.defaults"));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Plan.Steps[0].Action,
                Is.EqualTo(
                    SaveParticipantApplyActionKind
                        .InitializeDefault));

            Assert.That(
                result.Plan.Steps[0].DetachedState,
                Is.Null);
        }

        [Test]
        public void PreparedPayloadProducesExplicitApplyAction()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.inventory"));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            SaveParticipantApplyPlanResult result =
                Planner().Plan(
                    handle);

            Assert.That(
                result.Plan.Steps[0].Action,
                Is.EqualTo(
                    SaveParticipantApplyActionKind
                        .ApplyPreparedState));

            Assert.That(
                result.Plan.Steps[0].DetachedState,
                Is.Not.Null);
        }

        [Test]
        public void ExpiredHandleRejectsPreflight()
        {
            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.inventory"));

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            clock.Advance(
                TimeSpan.FromMinutes(6));

            Assert.That(
                Planner().Plan(
                    handle)
                    .Status,
                Is.EqualTo(
                    SaveParticipantApplyPlanStatus
                        .HandleUnavailable));
        }

        private SaveParticipantApplyPlanner Planner() =>
            new SaveParticipantApplyPlanner(
                store,
                registry);
    }
}
