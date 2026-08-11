
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicRecoveryExecutionServiceTests
    {
        [Test]
        public void PublicServiceExposesExplicitRecoveryExecutionOperation()
        {
            MethodInfo method =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "ExecuteRecoveryAsync",
                        new[]
                        {
                            typeof(SaveRecoveryPlan),
                            typeof(SaveRecoveryCandidate)
                        });

            Assert.That(method, Is.Not.Null);
            Assert.That(
                method.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<
                        SaveRecoveryResult>)));
        }

        [Test]
        public void RecoveryExecutionBeforeReadyReportsServiceNotReady()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveRecoveryResult result =
                    env.Service
                        .ExecuteRecoverySynchronouslyForTesting(
                            null,
                            default);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .ServiceNotReady));
                Assert.That(result.HeadPublished, Is.False);
            }
        }

        [Test]
        public void OverlappingRecoveryExecutionReturnsBusyAndNeverQueues()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveRecoveryPlan plan =
                    MakeActiveSlotRecoverable(
                        env);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveRecoveryResult result =
                    env.Service
                        .ExecuteRecoverySynchronouslyForTesting(
                            plan,
                            plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus.Busy));
                Assert.That(result.HeadPublished, Is.False);

                lease.Dispose();
            }
        }

        [Test]
        public void RecoveryExecutionAfterShutdownReportsAdmissionClosed()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveRecoveryPlan plan =
                    MakeActiveSlotRecoverable(
                        env);

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);

                SaveRecoveryResult result =
                    env.Service
                        .ExecuteRecoverySynchronouslyForTesting(
                            plan,
                            plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .AdmissionClosed));
                Assert.That(result.HeadPublished, Is.False);
            }
        }

        [Test]
        public void SuccessfulPublicRecoveryPreservesActiveSlotSelection()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveSlotId activeBefore =
                    env.Service
                        .SlotCatalogForTesting
                        .ActiveSlotId;

                SaveRecoveryPlan plan =
                    MakeActiveSlotRecoverable(
                        env);

                SaveRecoveryResult result =
                    env.Service
                        .ExecuteRecoverySynchronouslyForTesting(
                            plan,
                            plan.PreferredCandidate);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    env.Service
                        .SlotCatalogForTesting
                        .HasActiveSlot,
                    Is.True);
                Assert.That(
                    env.Service
                        .SlotCatalogForTesting
                        .ActiveSlotId,
                    Is.EqualTo(
                        activeBefore));
            }
        }

        [Test]
        public void SuccessfulPublicRecoveryDoesNotApplyParticipants()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveRecoveryPlan plan =
                    MakeActiveSlotRecoverable(
                        env);

                int executorCallsBefore =
                    env.Executor.Calls;

                SaveRecoveryResult result =
                    env.Service
                        .ExecuteRecoverySynchronouslyForTesting(
                            plan,
                            plan.PreferredCandidate);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        executorCallsBefore));
            }
        }

        private static SaveRecoveryPlan MakeActiveSlotRecoverable(
            AutosaveServiceTestEnvironment env)
        {
            SaveGenerationPublicationResult second =
                env.Storage
                    .CreatePublicationCoordinator()
                    .PublishEmptyTransportGeneration(
                        env.ActiveSlotId,
                        "com.example.autosave",
                        "1.0.0",
                        "recovery-second",
                        "Autosave Test Slot");

            Assert.That(second.Succeeded, Is.True);

            SaveGenerationStorageKeys.TryCreate(
                env.ActiveSlotId,
                second.GenerationId,
                out SaveGenerationStorageKeys keys);

            Assert.That(
                env.Storage.Local.Delete(
                    keys.Head)
                    .Succeeded,
                Is.True);

            SaveRecoveryPlan plan =
                env.Service
                    .BuildRecoveryPlanSynchronouslyForTesting(
                        env.ActiveSlotId);

            Assert.That(
                plan.Status,
                Is.EqualTo(
                    SaveRecoveryPlanStatus
                        .RecoveryAvailable));

            return plan;
        }
    }
}
