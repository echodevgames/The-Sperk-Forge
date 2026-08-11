
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicRecoveryPlanServiceTests
    {
        [Test]
        public void PublicServiceExposesBoundedReadOnlyRecoveryPlanOperation()
        {
            MethodInfo method =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "BuildRecoveryPlanAsync",
                        new[]
                        {
                            typeof(SaveSlotId)
                        });

            Assert.That(method, Is.Not.Null);
            Assert.That(
                method.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<
                        SaveRecoveryPlan>)));
        }

        [Test]
        public void RecoveryPlanBeforeReadyRejectsWithoutStorageInspection()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveRecoveryPlan plan =
                    env.Service
                        .BuildRecoveryPlanSynchronouslyForTesting(
                            SaveSlotId.NewId());

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .ServiceNotReady));
            }
        }

        [Test]
        public void HealthyCreatedSlotBuildsRecoveryNotRequiredPlan()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveRecoveryPlan plan =
                    env.Service
                        .BuildRecoveryPlanSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .RecoveryNotRequired));
                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Healthy));
            }
        }

        [Test]
        public void RecoveryPlanningDoesNotAcquireMutatingAdmission()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveRecoveryPlan plan =
                    env.Service
                        .BuildRecoveryPlanSynchronouslyForTesting(
                            env.ActiveSlotId);

                Assert.That(plan.Succeeded, Is.True);

                lease.Dispose();
            }
        }

        [Test]
        public void ExplicitSlotRecoveryPlanningDoesNotRequireActiveSelection()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        selectActiveSlot: false)
                        .Succeeded,
                    Is.True);

                SaveTechnicalSlotCreationCoordinator creation =
                    new SaveTechnicalSlotCreationCoordinator(
                        env.Service.SlotCatalogForTesting,
                        env.Storage.Backend,
                        env.Storage.Serializer,
                        env.Storage.Integrity,
                        8,
                        4);

                SaveTechnicalSlotCreateResult created =
                    creation.Create(
                        SlotCreationTestEnvironment.Request(
                            "Recovery Explicit Slot",
                            "com.example.recovery",
                            "1.0.0",
                            "initial"));

                Assert.That(created.Succeeded, Is.True);

                SaveRecoveryPlan plan =
                    env.Service
                        .BuildRecoveryPlanSynchronouslyForTesting(
                            created.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .RecoveryNotRequired));
            }
        }

        [Test]
        public void RecoveryPlanRemainsPayloadFreeWhileServiceAddsExplicitExecution()
        {
            PropertyInfo[] properties =
                typeof(SaveRecoveryPlan)
                    .GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public);

            Assert.That(
                System.Array.Exists(
                    properties,
                    property =>
                        property.Name ==
                            "Path" ||
                        property.Name ==
                            "Execute" ||
                        property.Name ==
                            "Catalog"),
                Is.False);

            MethodInfo execute =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "ExecuteRecoveryAsync",
                        new[]
                        {
                            typeof(SaveRecoveryPlan),
                            typeof(SaveRecoveryCandidate)
                        });

            Assert.That(execute, Is.Not.Null);
            Assert.That(
                execute.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<
                        SaveRecoveryResult>)));
        }
    }
}
