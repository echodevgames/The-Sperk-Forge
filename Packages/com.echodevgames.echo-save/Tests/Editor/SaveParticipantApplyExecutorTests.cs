
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantApplyExecutorTests
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
                        4,
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
        public void PreparedStateAppliesExactlyOnceAndConsumesHandle()
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
                        "com.example.inventory",
                        42));

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                participant.ApplyCalls,
                Is.EqualTo(1));

            Assert.That(
                ((ParticipantApplyTestState)
                    participant.LastAppliedState).Value,
                Is.EqualTo(42));

            Assert.That(
                result.MutationBegan,
                Is.True);

            Assert.That(
                result.HandleConsumed,
                Is.True);

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Consumed));
        }

        [Test]
        public void DefaultInitializationNeverCallsApplyNull()
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

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                participant.InitializeDefaultCalls,
                Is.EqualTo(1));

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);

            Assert.That(
                participant.LastAppliedState,
                Is.Null);

            Assert.That(
                result.Entries[0].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome
                        .DefaultInitialized));
        }

        [Test]
        public void IgnoreInvokesNoCallbackAndIsReported()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.optional",
                    SaveMissingPayloadPolicy.Ignore);

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.MutationBegan,
                Is.False);

            Assert.That(
                result.HandleConsumed,
                Is.True);

            Assert.That(
                participant.ApplyCalls,
                Is.Zero);

            Assert.That(
                result.Entries[0].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome.Ignored));
        }

        [Test]
        public void ApplyFailureStopsLaterMutatingActions()
        {
            ParticipantApplyTestParticipant first =
                new ParticipantApplyTestParticipant(
                    "com.example.alpha")
                {
                    ApplyResult =
                        SaveParticipantApplyResult.Failure(
                            "nope",
                            "TEST-APPLY")
                };

            ParticipantApplyTestParticipant second =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta");

            registry.Register(
                first);

            registry.Register(
                second);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.alpha"),
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"));

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.ParticipantFailed));

            Assert.That(
                first.ApplyCalls,
                Is.EqualTo(1));

            Assert.That(
                second.ApplyCalls,
                Is.Zero);

            Assert.That(
                result.Entries[0].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome.Failed));

            Assert.That(
                result.Entries[1].Outcome,
                Is.EqualTo(
                    SaveParticipantApplyOutcome.NotAttempted));

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Consumed));
        }

        [Test]
        public void DefaultFailureStopsLaterMutatingActions()
        {
            DefaultableParticipantApplyTestParticipant first =
                new DefaultableParticipantApplyTestParticipant(
                    "com.example.alpha")
                {
                    DefaultResult =
                        SaveParticipantApplyResult.Failure(
                            "default-failed",
                            "TEST-DEFAULT")
                };

            ParticipantApplyTestParticipant second =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta");

            registry.Register(
                first);

            registry.Register(
                second);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"));

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.ParticipantFailed));

            Assert.That(
                first.InitializeDefaultCalls,
                Is.EqualTo(1));

            Assert.That(
                second.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void ApplyExceptionBecomesStructuredFailure()
        {
            ParticipantApplyTestParticipant participant =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory")
                {
                    ThrowOnApply =
                        true
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
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.ParticipantException));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    EchoSaveDiagnosticCodes
                        .PreparedApplyParticipantException));

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Consumed));
        }

        [Test]
        public void DefaultExceptionBecomesStructuredFailure()
        {
            DefaultableParticipantApplyTestParticipant participant =
                new DefaultableParticipantApplyTestParticipant(
                    "com.example.defaults")
                {
                    ThrowOnDefault =
                        true
                };

            registry.Register(
                participant);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId);

            Assert.That(
                ExecutePlanned(
                    handle)
                    .Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.ParticipantException));
        }

        [Test]
        public void RegistrationReplacementBeforeExecutionLeavesHandleLive()
        {
            ParticipantApplyTestParticipant first =
                new ParticipantApplyTestParticipant(
                    "com.example.inventory");

            SaveParticipantRegistration registration =
                registry.Register(
                    first)
                    .Registration;

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.inventory"));

            SaveParticipantApplyPlan plan =
                Planner().Plan(
                    handle)
                    .Plan;

            registration.Dispose();

            registry.Register(
                new ParticipantApplyTestParticipant(
                    "com.example.inventory"));

            SavePreparedLoadApplyResult result =
                Executor().Execute(
                    plan,
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.RegistryChanged));

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
                first.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void RegistrationChangeAfterEarlierMutationConsumesHandle()
        {
            ParticipantApplyTestParticipant first =
                new ParticipantApplyTestParticipant(
                    "com.example.alpha");

            ParticipantApplyTestParticipant second =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta");

            registry.Register(
                first);

            SaveParticipantRegistration secondRegistration =
                registry.Register(
                    second)
                    .Registration;

            first.BeforeApply =
                () =>
                    secondRegistration.Dispose();

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.alpha"),
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"));

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SavePreparedLoadApplyStatus.RegistryChanged));

            Assert.That(
                result.MutationBegan,
                Is.True);

            Assert.That(
                result.HandleConsumed,
                Is.True);

            Assert.That(
                first.ApplyCalls,
                Is.EqualTo(1));

            Assert.That(
                second.ApplyCalls,
                Is.Zero);

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Consumed));
        }

        [Test]
        public void SuccessfulReportOrderMatchesDeterministicPlan()
        {
            ParticipantApplyTestParticipant zeta =
                new ParticipantApplyTestParticipant(
                    "com.example.zeta");

            ParticipantApplyTestParticipant alpha =
                new ParticipantApplyTestParticipant(
                    "com.example.alpha");

            registry.Register(
                zeta);

            registry.Register(
                alpha);

            PreparedSaveLoad handle =
                ParticipantApplyTestSupport.CreateHandle(
                    store,
                    slotId,
                    generationId,
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.zeta"),
                    ParticipantApplyTestSupport.PreparedEntry(
                        "com.example.alpha"));

            SavePreparedLoadApplyResult result =
                ExecutePlanned(
                    handle);

            Assert.That(
                result.Entries[0].ParticipantId.Value,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                result.Entries[1].ParticipantId.Value,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        private SavePreparedLoadApplyResult ExecutePlanned(
            PreparedSaveLoad handle)
        {
            SaveParticipantApplyPlanResult plan =
                Planner().Plan(
                    handle);

            Assert.That(
                plan.Succeeded,
                Is.True);

            return Executor().Execute(
                plan.Plan,
                handle);
        }

        private SaveParticipantApplyPlanner Planner() =>
            new SaveParticipantApplyPlanner(
                store,
                registry);

        private SaveParticipantApplyExecutor Executor() =>
            new SaveParticipantApplyExecutor(
                store,
                registry);
    }
}
