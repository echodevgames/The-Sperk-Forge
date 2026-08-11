
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicSlotDeletionServiceTests
    {
        [Test]
        public void PublicServiceExposesTwoStepDeletionAndNoOneStepDelete()
        {
            MethodInfo prepare =
                typeof(IEchoSaveService).GetMethod(
                    "PrepareDeleteSlotAsync",
                    new[]
                    {
                        typeof(SaveSlotId)
                    });

            MethodInfo confirm =
                typeof(IEchoSaveService).GetMethod(
                    "ConfirmDeleteSlotAsync",
                    new[]
                    {
                        typeof(SaveDeletionPlan)
                    });

            Assert.That(prepare, Is.Not.Null);
            Assert.That(
                prepare.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<SaveDeletionPlan>)));

            Assert.That(confirm, Is.Not.Null);
            Assert.That(
                confirm.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<SaveSlotDeleteResult>)));

            Assert.That(
                typeof(IEchoSaveService).GetMethod("DeleteSlotAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService).GetMethod("TrashSlotAsync"),
                Is.Null);
        }

        [Test]
        public void PrepareDeleteBeforeReadyReportsServiceNotReady()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveDeletionPlan plan =
                    env.Service
                        .PrepareDeleteSlotSynchronouslyForTesting(
                            SaveSlotId.NewId());

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveDeletionPlanStatus.ServiceNotReady));
            }
        }

        [Test]
        public void ConfirmDeleteBeforeReadyReportsServiceNotReady()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveSlotDeleteResult result =
                    env.Service
                        .ConfirmDeleteSlotSynchronouslyForTesting(
                            null);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDeleteStatus.ServiceNotReady));
            }
        }

        [Test]
        public void PrepareDeleteDoesNotUseMutatingAdmission()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveDeletionPlan plan =
                    env.Service
                        .PrepareDeleteSlotSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(plan.Succeeded, Is.True);

                lease.Dispose();
            }
        }

        [Test]
        public void ConfirmDeleteOverlappingMutationReturnsBusyAndNeverQueues()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveDeletionPlan plan =
                    env.Service
                        .PrepareDeleteSlotSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(plan.Succeeded, Is.True);

                Assert.That(
                    env.Service.SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveSlotDeleteResult result =
                    env.Service
                        .ConfirmDeleteSlotSynchronouslyForTesting(
                            plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(SaveSlotDeleteStatus.Busy));

                lease.Dispose();
            }
        }

        [Test]
        public void ConfirmDeleteAfterShutdownReportsAdmissionClosed()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveDeletionPlan plan =
                    env.Service
                        .PrepareDeleteSlotSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(env.Service.ShutdownCore().Succeeded, Is.True);

                SaveSlotDeleteResult result =
                    env.Service
                        .ConfirmDeleteSlotSynchronouslyForTesting(
                            plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveSlotDeleteStatus.AdmissionClosed));
            }
        }

        [Test]
        public void PrepareDeleteNeverExecutesManualSaveTransaction()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(env.Initialize().Succeeded, Is.True);

                SaveDeletionPlan plan =
                    env.Service
                        .PrepareDeleteSlotSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(env.Executor.Calls, Is.Zero);
            }
        }
    }
}
