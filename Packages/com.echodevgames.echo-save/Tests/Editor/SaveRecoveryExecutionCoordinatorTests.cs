
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRecoveryExecutionCoordinatorTests
    {
        [Test]
        public void MissingHeadRecoveryPublishesSelectedCandidateAndReconcilesCatalog()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        fixture.Plan.PreferredCandidate);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.CatalogReconciled, Is.True);

                SaveHeadPointer head =
                    ReadHead(
                        env,
                        fixture.SlotId);

                Assert.That(
                    head.currentGenerationId,
                    Is.EqualTo(
                        fixture.Plan
                            .PreferredCandidate
                            .GenerationId
                            .Value));
                Assert.That(head.previousGenerationId, Is.Empty);

                Assert.That(
                    fixture.Catalog.Snapshot
                        .TryGetEntry(
                            fixture.SlotId,
                            out SaveSlotCatalogEntry entry),
                    Is.True);
                Assert.That(entry.IsSelectable, Is.True);
                Assert.That(
                    entry.CurrentGenerationId,
                    Is.EqualTo(
                        fixture.Plan
                            .PreferredCandidate
                            .GenerationId));
            }
        }

        [Test]
        public void ExplicitNonPreferredVerifiedCandidateMayBeSelected()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                Assert.That(
                    fixture.Plan.Candidates.Count,
                    Is.EqualTo(2));

                SaveRecoveryCandidate selected =
                    fixture.Plan.Candidates[1];

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        selected);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    result.SelectedGenerationId,
                    Is.EqualTo(
                        selected.GenerationId));

                Assert.That(
                    ReadHead(
                        env,
                        fixture.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        selected.GenerationId.Value));
            }
        }

        [Test]
        public void StalePlanRejectsBeforeMutationWhenSourceChanges()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                SaveGenerationPublicationResult changed =
                    env.CreatePublicationCoordinator()
                        .PublishEmptyTransportGeneration(
                            fixture.SlotId,
                            "com.example.recovery",
                            "1.0.0",
                            "stale-change",
                            "Changed Source");

                Assert.That(changed.Succeeded, Is.True);

                int mutationsBefore =
                    fixture.Backend.MutationCalls;

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        fixture.Plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .StalePlan));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    fixture.Backend.MutationCalls,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void CandidateNotInSuppliedPlanRejectsBeforeMutation()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                SaveRecoveryCandidate foreign =
                    new SaveRecoveryCandidate(
                        SaveGenerationId.NewId(),
                        "2026-08-10T00:00:00.0000000Z",
                        "transport",
                        "com.example.recovery",
                        "1.0.0",
                        "foreign");

                int mutationsBefore =
                    fixture.Backend.MutationCalls;

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        foreign);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .CandidateInvalid));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    fixture.Backend.MutationCalls,
                    Is.EqualTo(
                        mutationsBefore));
            }
        }

        [Test]
        public void CandidateCorruptedAfterPlanningRejectsBeforeHeadPublication()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId first =
                    env.PublishGeneration(
                        DateTime.UtcNow.AddMinutes(-2),
                        1);

                SaveGenerationId selected =
                    env.PublishGeneration(
                        DateTime.UtcNow.AddMinutes(-1),
                        2);

                env.DeleteHead();

                SaveRecoveryExecutionTestBackend backend =
                    new SaveRecoveryExecutionTestBackend(
                        env.Local);

                SaveSlotCatalog catalog =
                    new SaveSlotCatalog(
                        backend,
                        env.Serializer,
                        64);

                SaveRecoveryPlanBuilder builder =
                    new SaveRecoveryPlanBuilder(
                        backend,
                        env.Serializer,
                        env.Integrity,
                        64);

                SaveRecoveryPlan plan =
                    builder.Build(
                        env.SlotId);

                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(selected));

                env.CorruptPayloadChecksum(
                    selected);

                int mutationsBefore =
                    backend.MutationCalls;

                SaveRecoveryResult result =
                    new SaveRecoveryExecutionCoordinator(
                        backend,
                        env.Serializer,
                        builder,
                        catalog)
                    .Execute(
                        plan,
                        plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .StalePlan));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(
                    backend.MutationCalls,
                    Is.EqualTo(
                        mutationsBefore));

                Assert.That(first.Value, Is.Not.Empty);
            }
        }

        [Test]
        public void HeadPublicationFailureNeverFabricatesRecoveryCommit()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                fixture.Backend.FailNextHeadPublication =
                    true;

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        fixture.Plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .HeadPublicationFailed));
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(result.CatalogReconciled, Is.False);
            }
        }

        [Test]
        public void CatalogFailureAfterHeadPublicationPreservesCommittedTruth()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                fixture.Backend
                    .FailCatalogDiscoveryAfterNextHead();

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        fixture.Plan.PreferredCandidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .CatalogReconciliationFailed));
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(result.RecoveryCommitted, Is.True);
                Assert.That(result.CatalogReconciled, Is.False);

                SaveHeadPointer head =
                    ReadHead(
                        env,
                        fixture.SlotId);

                Assert.That(
                    head.currentGenerationId,
                    Is.EqualTo(
                        fixture.Plan
                            .PreferredCandidate
                            .GenerationId
                            .Value));
            }
        }

        [Test]
        public void SelectedGenerationBytesRemainImmutableAcrossRecovery()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                RecoveryFixture fixture =
                    CreateRecoverableMissingHead(
                        env);

                SaveGenerationId generation =
                    fixture.Plan
                        .PreferredCandidate
                        .GenerationId;

                SaveGenerationStorageKeys.TryCreate(
                    fixture.SlotId,
                    generation,
                    out SaveGenerationStorageKeys keys);

                byte[] payloadBefore =
                    env.Local.Read(
                        keys.GenerationPayload)
                    .Data;

                byte[] manifestBefore =
                    env.Local.Read(
                        keys.GenerationManifest)
                    .Data;

                SaveRecoveryResult result =
                    fixture.Executor.Execute(
                        fixture.Plan,
                        fixture.Plan.PreferredCandidate);

                Assert.That(result.Succeeded, Is.True);

                CollectionAssert.AreEqual(
                    payloadBefore,
                    env.Local.Read(
                        keys.GenerationPayload)
                    .Data);

                CollectionAssert.AreEqual(
                    manifestBefore,
                    env.Local.Read(
                        keys.GenerationManifest)
                    .Data);
            }
        }

        [Test]
        public void RecoveryNotRequiredPlanCannotExecute()
        {
            using (SlotCreationTestEnvironment env =
                new SlotCreationTestEnvironment())
            {
                SaveTechnicalSlotCreateResult created =
                    CreateSlot(
                        env);

                SaveRecoveryExecutionTestBackend backend =
                    new SaveRecoveryExecutionTestBackend(
                        env.Backend);

                SaveSlotCatalog catalog =
                    new SaveSlotCatalog(
                        backend,
                        env.Serializer,
                        64);

                SaveRecoveryPlanBuilder builder =
                    new SaveRecoveryPlanBuilder(
                        backend,
                        env.Serializer,
                        env.Integrity,
                        64);

                SaveRecoveryPlan plan =
                    builder.Build(
                        created.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .RecoveryNotRequired));

                SaveRecoveryCandidate candidate =
                    new SaveRecoveryCandidate(
                        created.GenerationId,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty);

                SaveRecoveryResult result =
                    new SaveRecoveryExecutionCoordinator(
                        backend,
                        env.Serializer,
                        builder,
                        catalog)
                    .Execute(
                        plan,
                        candidate);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .InvalidRequest));
                Assert.That(backend.MutationCalls, Is.Zero);
            }
        }

        [Test]
        public void NoValidCandidatePlanCannotExecute()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        DateTime.UtcNow.AddMinutes(-1),
                        1);

                env.CorruptPayloadChecksum(
                    generation);

                env.DeleteHead();

                SaveRecoveryExecutionTestBackend backend =
                    new SaveRecoveryExecutionTestBackend(
                        env.Local);

                SaveRecoveryPlanBuilder builder =
                    new SaveRecoveryPlanBuilder(
                        backend,
                        env.Serializer,
                        env.Integrity,
                        64);

                SaveRecoveryPlan plan =
                    builder.Build(
                        env.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .NoValidCandidate));

                SaveRecoveryResult result =
                    new SaveRecoveryExecutionCoordinator(
                        backend,
                        env.Serializer,
                        builder,
                        new SaveSlotCatalog(
                            backend,
                            env.Serializer,
                            64))
                    .Execute(
                        plan,
                        default);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRecoveryExecutionStatus
                            .InvalidRequest));
                Assert.That(backend.MutationCalls, Is.Zero);
            }
        }

        private static RecoveryFixture CreateRecoverableMissingHead(
            SlotCreationTestEnvironment env)
        {
            SaveTechnicalSlotCreateResult created =
                CreateSlot(
                    env);

            SaveGenerationPublicationResult second =
                env.CreatePublicationCoordinator()
                    .PublishEmptyTransportGeneration(
                        created.SlotId,
                        "com.example.recovery",
                        "1.0.0",
                        "second",
                        "Recovery Test");

            Assert.That(second.Succeeded, Is.True);

            SaveGenerationStorageKeys.TryCreate(
                created.SlotId,
                second.GenerationId,
                out SaveGenerationStorageKeys keys);

            Assert.That(
                env.Local.Delete(
                    keys.Head)
                    .Succeeded,
                Is.True);

            SaveRecoveryExecutionTestBackend backend =
                new SaveRecoveryExecutionTestBackend(
                    env.Backend);

            SaveSlotCatalog catalog =
                new SaveSlotCatalog(
                    backend,
                    env.Serializer,
                    64);

            SaveRecoveryPlanBuilder builder =
                new SaveRecoveryPlanBuilder(
                    backend,
                    env.Serializer,
                    env.Integrity,
                    64);

            SaveRecoveryPlan plan =
                builder.Build(
                    created.SlotId);

            Assert.That(
                plan.Status,
                Is.EqualTo(
                    SaveRecoveryPlanStatus
                        .RecoveryAvailable));
            Assert.That(plan.Candidates.Count, Is.EqualTo(2));

            return new RecoveryFixture(
                created.SlotId,
                backend,
                catalog,
                plan,
                new SaveRecoveryExecutionCoordinator(
                    backend,
                    env.Serializer,
                    builder,
                    catalog));
        }

        private static SaveTechnicalSlotCreateResult CreateSlot(
            SlotCreationTestEnvironment env)
        {
            SaveTechnicalSlotCreateResult created =
                env.CreateSlotCoordinator(
                        8,
                        4,
                        SaveSlotId.NewId)
                    .Create(
                        SlotCreationTestEnvironment.Request(
                            "Recovery Execution",
                            "com.example.recovery",
                            "1.0.0",
                            "initial"));

            Assert.That(created.Succeeded, Is.True);

            return created;
        }

        private static SaveHeadPointer ReadHead(
            SlotCreationTestEnvironment env,
            SaveSlotId slotId)
        {
            SaveStorageReadResult read =
                env.ReadHead(
                    slotId);

            Assert.That(read.Succeeded, Is.True);

            SaveSerializerResult parsed =
                env.Serializer.Deserialize(
                    System.Text.Encoding.UTF8
                        .GetString(
                            read.Data),
                    out SaveHeadPointer head);

            Assert.That(parsed.Succeeded, Is.True);

            return head;
        }

        private sealed class RecoveryFixture
        {
            internal RecoveryFixture(
                SaveSlotId slotId,
                SaveRecoveryExecutionTestBackend backend,
                SaveSlotCatalog catalog,
                SaveRecoveryPlan plan,
                SaveRecoveryExecutionCoordinator executor)
            {
                SlotId =
                    slotId;

                Backend =
                    backend;

                Catalog =
                    catalog;

                Plan =
                    plan;

                Executor =
                    executor;
            }

            internal SaveSlotId SlotId { get; }

            internal SaveRecoveryExecutionTestBackend Backend { get; }

            internal SaveSlotCatalog Catalog { get; }

            internal SaveRecoveryPlan Plan { get; }

            internal SaveRecoveryExecutionCoordinator Executor { get; }
        }
    }
}
