
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicLoadCompositionTests
    {
        [Test]
        public void PrepareLoadBeforeReadyReportsServiceNotReady()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PreparedLoadCreationResult result =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                SaveSlotId.NewId()));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        PreparedLoadCreationStatus
                            .ServiceNotReady));

                Assert.That(result.Handle, Is.Null);
            }
        }

        [Test]
        public void MissingExplicitSlotDoesNotAutoRecover()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                PreparedLoadCreationResult result =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                SaveSlotId.NewId()));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        PreparedLoadCreationStatus
                            .SourceUnavailable));

                Assert.That(result.Handle, Is.Null);
            }
        }

        [Test]
        public void PrepareLoadCreatesBoundedHandleWithoutParticipantMutation()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.prepare",
                        value: 17);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                participant.Value = 99;

                int appliesBefore =
                    participant.ApplyCalls;

                int mutationsBefore =
                    env.Storage.Backend.MutationCount;

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);
                Assert.That(prepared.Handle, Is.Not.Null);
                Assert.That(prepared.Handle.IsValid, Is.True);
                Assert.That(
                    prepared.Handle.SourceSlotId,
                    Is.EqualTo(slot.SlotId));
                Assert.That(
                    prepared.Handle.PreparedParticipantCount,
                    Is.EqualTo(1));
                Assert.That(
                    participant.ApplyCalls,
                    Is.EqualTo(appliesBefore));
                Assert.That(participant.Value, Is.EqualTo(99));
                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(mutationsBefore));

                prepared.Handle.Dispose();
                registration.Dispose();
            }
        }

        [Test]
        public void PrepareLoadPreservesUnclaimedPayloadAsOpaqueHandleTruth()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.unknown",
                        value: 21);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                registration.Dispose();

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);
                Assert.That(
                    prepared.Handle.PreparedParticipantCount,
                    Is.Zero);
                Assert.That(
                    prepared.Handle.UnknownPayloadCount,
                    Is.EqualTo(1));

                prepared.Handle.Dispose();
            }
        }

        [Test]
        public void ApplyPreparedLoadRestoresSavedParticipantState()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.apply",
                        value: 31);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                participant.Value = 900;

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);

                int storageMutationsBefore =
                    env.Storage.Backend.MutationCount;

                SavePreparedLoadApplyResult applied =
                    env.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(applied.Succeeded, Is.True);
                Assert.That(applied.MutationBegan, Is.True);
                Assert.That(applied.HandleConsumed, Is.True);
                Assert.That(participant.Value, Is.EqualTo(31));
                Assert.That(participant.ApplyCalls, Is.EqualTo(1));
                Assert.That(
                    env.Storage.Backend.MutationCount,
                    Is.EqualTo(storageMutationsBefore));
                Assert.That(
                    prepared.Handle.State,
                    Is.EqualTo(
                        PreparedLoadState.Consumed));

                registration.Dispose();
            }
        }

        [Test]
        public void ApplyPreparedLoadBusyDoesNotConsumeHandleOrQueue()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.applybusy",
                        value: 41);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                participant.Value = 500;

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SavePreparedLoadApplyResult busy =
                    env.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(
                    busy.Status,
                    Is.EqualTo(
                        SavePreparedLoadApplyStatus.Busy));
                Assert.That(busy.MutationBegan, Is.False);
                Assert.That(busy.HandleConsumed, Is.False);
                Assert.That(prepared.Handle.IsValid, Is.True);
                Assert.That(participant.ApplyCalls, Is.Zero);

                lease.Dispose();

                SavePreparedLoadApplyResult retry =
                    env.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(retry.Succeeded, Is.True);
                Assert.That(participant.Value, Is.EqualTo(41));

                registration.Dispose();
            }
        }

        [Test]
        public void DisposedPreparedHandleRejectsBeforeMutation()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.disposed",
                        value: 51);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);

                prepared.Handle.Dispose();

                SavePreparedLoadApplyResult result =
                    env.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SavePreparedLoadApplyStatus
                            .HandleUnavailable));
                Assert.That(result.MutationBegan, Is.False);
                Assert.That(participant.ApplyCalls, Is.Zero);

                registration.Dispose();
            }
        }

        [Test]
        public void ForeignPreparedHandleRejectsBeforeMutation()
        {
            using (PublicRuntimeFacadeTestEnvironment owner =
                new PublicRuntimeFacadeTestEnvironment())
            using (PublicRuntimeFacadeTestEnvironment foreign =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant ownerParticipant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.foreign",
                        value: 61);

                SaveParticipantRegistration ownerRegistration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        owner,
                        ownerParticipant,
                        out ownerRegistration);

                PreparedLoadCreationResult prepared =
                    owner.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);
                Assert.That(foreign.Initialize().Succeeded, Is.True);

                PublicRuntimeFacadeParticipant foreignParticipant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.foreign",
                        value: 999);

                SaveParticipantRegistrationResult foreignRegistration =
                    foreign.Service.RegisterParticipant(
                        foreignParticipant);

                Assert.That(foreignRegistration.Succeeded, Is.True);

                SavePreparedLoadApplyResult result =
                    foreign.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SavePreparedLoadApplyStatus
                            .HandleUnavailable));
                Assert.That(result.MutationBegan, Is.False);
                Assert.That(foreignParticipant.ApplyCalls, Is.Zero);
                Assert.That(prepared.Handle.IsValid, Is.True);

                prepared.Handle.Dispose();
                ownerRegistration.Dispose();
                foreignRegistration.Registration.Dispose();
            }
        }

        [Test]
        public void RegistryChangeAfterPrepareRejectsBeforeAnyParticipantMutation()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant first =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.registry.a",
                        value: 71);

                PublicRuntimeFacadeParticipant second =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.registry.b",
                        value: 72);

                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveParticipantRegistrationResult firstRegistration =
                    env.Service.RegisterParticipant(first);

                SaveParticipantRegistrationResult secondRegistration =
                    env.Service.RegisterParticipant(second);

                Assert.That(firstRegistration.Succeeded, Is.True);
                Assert.That(secondRegistration.Succeeded, Is.True);

                SaveSlotCreateResult slot =
                    env.CreateAndSelect();

                Assert.That(env.SaveCurrent().Succeeded, Is.True);

                first.Value = 700;
                second.Value = 720;

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);

                secondRegistration.Registration.Dispose();

                SavePreparedLoadApplyResult result =
                    env.Service
                        .ApplyPreparedLoadSynchronouslyForTesting(
                            prepared.Handle);

                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.MutationBegan, Is.False);
                Assert.That(first.ApplyCalls, Is.Zero);
                Assert.That(second.ApplyCalls, Is.Zero);
                Assert.That(first.Value, Is.EqualTo(700));
                Assert.That(second.Value, Is.EqualTo(720));

                prepared.Handle.Dispose();
                firstRegistration.Registration.Dispose();
            }
        }

        [Test]
        public void ConvenienceLoadPreparesAndAppliesInCurrentScene()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.convenience",
                        value: 81);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                participant.Value = 810;

                SaveLoadResult result =
                    env.Service
                        .LoadAndApplySynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.PreparationSucceeded, Is.True);
                Assert.That(result.ApplyAttempted, Is.True);
                Assert.That(result.MutationBegan, Is.True);
                Assert.That(result.HandleConsumed, Is.True);
                Assert.That(result.ApplyResult, Is.Not.Null);
                Assert.That(result.ApplyResult.Succeeded, Is.True);
                Assert.That(participant.Value, Is.EqualTo(81));

                registration.Dispose();
            }
        }

        [Test]
        public void ConveniencePreparationFailurePerformsNoApply()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.nosource",
                        value: 91);

                SaveParticipantRegistrationResult registration =
                    env.Service.RegisterParticipant(
                        participant);

                Assert.That(registration.Succeeded, Is.True);

                SaveLoadResult result =
                    env.Service
                        .LoadAndApplySynchronouslyForTesting(
                            new SaveLoadRequest(
                                SaveSlotId.NewId()));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveLoadStatus.PreparationFailed));
                Assert.That(result.PreparationSucceeded, Is.False);
                Assert.That(result.ApplyAttempted, Is.False);
                Assert.That(result.MutationBegan, Is.False);
                Assert.That(participant.ApplyCalls, Is.Zero);

                registration.Registration.Dispose();
            }
        }

        [Test]
        public void ConvenienceApplyPreflightFailureReleasesHandleWithoutMutation()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveSlotCreateResult slot =
                    env.CreateSlot(
                        "R1 Empty Source");

                Assert.That(slot.Succeeded, Is.True);

                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.preflight.missing",
                        value: 95,
                        missingPayloadPolicy:
                            SaveMissingPayloadPolicy.Fail);

                SaveParticipantRegistrationResult registration =
                    env.Service.RegisterParticipant(
                        participant);

                Assert.That(registration.Succeeded, Is.True);

                SaveLoadResult result =
                    env.Service
                        .LoadAndApplySynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveLoadStatus.ApplyFailed));
                Assert.That(result.PreparationSucceeded, Is.True);
                Assert.That(result.ApplyAttempted, Is.True);
                Assert.That(result.MutationBegan, Is.False);
                Assert.That(result.HandleConsumed, Is.False);
                Assert.That(participant.ApplyCalls, Is.Zero);
                Assert.That(
                    env.Service
                        .PreparedLoadStoreForTesting
                        .LiveCount,
                    Is.Zero);

                registration.Registration.Dispose();
            }
        }

        [Test]
        public void ConvenienceApplyFailureReportsTruthAfterMutationBegins()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                PublicRuntimeFacadeParticipant first =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.partial.a",
                        value: 101);

                PublicRuntimeFacadeParticipant second =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.partial.z",
                        value: 102);

                SaveParticipantRegistrationResult firstRegistration =
                    env.Service.RegisterParticipant(first);

                SaveParticipantRegistrationResult secondRegistration =
                    env.Service.RegisterParticipant(second);

                Assert.That(firstRegistration.Succeeded, Is.True);
                Assert.That(secondRegistration.Succeeded, Is.True);

                SaveSlotCreateResult slot =
                    env.CreateAndSelect();

                Assert.That(env.SaveCurrent().Succeeded, Is.True);

                first.Value = 1001;
                second.Value = 1002;
                second.FailApply = true;

                SaveLoadResult result =
                    env.Service
                        .LoadAndApplySynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveLoadStatus.ApplyFailed));
                Assert.That(result.PreparationSucceeded, Is.True);
                Assert.That(result.ApplyAttempted, Is.True);
                Assert.That(result.MutationBegan, Is.True);
                Assert.That(result.HandleConsumed, Is.True);
                Assert.That(first.ApplyCalls, Is.EqualTo(1));
                Assert.That(second.ApplyCalls, Is.EqualTo(1));
                Assert.That(first.Value, Is.EqualTo(101));
                Assert.That(second.Value, Is.EqualTo(1002));

                firstRegistration.Registration.Dispose();
                secondRegistration.Registration.Dispose();
            }
        }

        [Test]
        public void ShutdownInvalidatesOutstandingPreparedHandle()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.handle.shutdown",
                        value: 111);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                PreparedLoadCreationResult prepared =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(prepared.Succeeded, Is.True);
                Assert.That(prepared.Handle.IsValid, Is.True);

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);

                Assert.That(prepared.Handle.IsValid, Is.False);
                Assert.That(
                    prepared.Handle.State,
                    Is.EqualTo(
                        PreparedLoadState.OwnerInvalidated));
                Assert.That(registration.IsActive, Is.False);
            }
        }

        [Test]
        public void PrepareAndConvenienceLoadReturnBusyWithoutQueues()
        {
            using (PublicRuntimeFacadeTestEnvironment env =
                new PublicRuntimeFacadeTestEnvironment())
            {
                PublicRuntimeFacadeParticipant participant =
                    new PublicRuntimeFacadeParticipant(
                        "com.example.r1.loadbusy",
                        value: 121);

                SaveParticipantRegistration registration;
                SaveSlotCreateResult slot =
                    CreateSavedSource(
                        env,
                        participant,
                        out registration);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                PreparedLoadCreationResult prepareBusy =
                    env.Service
                        .PrepareLoadSynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                SaveLoadResult convenienceBusy =
                    env.Service
                        .LoadAndApplySynchronouslyForTesting(
                            new SaveLoadRequest(
                                slot.SlotId));

                Assert.That(
                    prepareBusy.Status,
                    Is.EqualTo(
                        PreparedLoadCreationStatus.Busy));

                Assert.That(
                    convenienceBusy.Status,
                    Is.EqualTo(
                        SaveLoadStatus.Busy));

                Assert.That(participant.ApplyCalls, Is.Zero);

                lease.Dispose();
                registration.Dispose();
            }
        }

        private static SaveSlotCreateResult CreateSavedSource(
            PublicRuntimeFacadeTestEnvironment env,
            PublicRuntimeFacadeParticipant participant,
            out SaveParticipantRegistration registration)
        {
            Assert.That(
                env.Initialize().Succeeded,
                Is.True);

            SaveParticipantRegistrationResult registered =
                env.Service.RegisterParticipant(
                    participant);

            Assert.That(registered.Succeeded, Is.True);

            registration =
                registered.Registration;

            SaveSlotCreateResult slot =
                env.CreateAndSelect();

            SaveOperationResult saved =
                env.SaveCurrent();

            Assert.That(saved.Succeeded, Is.True);
            Assert.That(saved.GenerationPublished, Is.True);
            Assert.That(saved.HeadPublished, Is.True);
            Assert.That(saved.CatalogReconciled, Is.True);

            return slot;
        }
    }
}
