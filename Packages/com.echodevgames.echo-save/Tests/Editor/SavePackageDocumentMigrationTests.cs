using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePackageDocumentMigrationTests
    {
        private static readonly SavePackageDocumentVersion HistoricalA =
            new SavePackageDocumentVersion(
                0,
                0,
                0);

        private static readonly SavePackageDocumentVersion HistoricalB =
            new SavePackageDocumentVersion(
                0,
                5,
                0);

        [Test]
        public void CurrentDocumentExecutesZeroMigrationSteps()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep unused =
                new VersionRewriteStep(
                    "tests.head.legacy-to-current",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current);

            SavePackageDocumentReader reader =
                Reader(
                    unused);

            string source =
                CurrentHeadJson();

            SavePackageDocumentReadResult result =
                reader.ReadCurrent(
                    source,
                    SaveDocumentKinds.HeadPointer,
                    out SaveHeadPointer head);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.WasMigrated, Is.False);
            Assert.That(result.Provenance.Count, Is.Zero);
            Assert.That(unused.Calls, Is.Zero);
            Assert.That(head, Is.Not.Null);
        }

        [Test]
        public void OneStepChainMigratesInMemoryAndPreservesSourceText()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.head.b-to-current",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current);

            SavePackageDocumentReader reader =
                Reader(
                    step);

            string source =
                HeadJson(
                    HistoricalB);

            string before =
                new string(
                    source.ToCharArray());

            SavePackageDocumentReadResult result =
                reader.ReadCurrent(
                    source,
                    SaveDocumentKinds.HeadPointer,
                    out SaveHeadPointer head);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.WasMigrated, Is.True);
            Assert.That(result.Provenance.Count, Is.EqualTo(1));
            Assert.That(result.Provenance[0].StepId, Is.EqualTo(step.StepId));
            Assert.That(step.Calls, Is.EqualTo(1));
            Assert.That(source, Is.EqualTo(before));
            Assert.That(head.formatMajor, Is.EqualTo(current.Major));
            Assert.That(head.formatMinor, Is.EqualTo(current.Minor));
            Assert.That(head.formatRevision, Is.EqualTo(current.Revision));
        }

        [Test]
        public void MultiStepContiguousChainMigratesInDeterministicOrder()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            List<string> trace =
                new List<string>();

            VersionRewriteStep first =
                new VersionRewriteStep(
                    "tests.head.a-to-b",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalA,
                    HistoricalB,
                    trace);

            VersionRewriteStep second =
                new VersionRewriteStep(
                    "tests.head.b-to-current",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    trace);

            SavePackageDocumentReadResult result =
                Reader(
                        first,
                        second)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalA),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Provenance.Count, Is.EqualTo(2));
            Assert.That(trace, Is.EqualTo(new[]
            {
                first.StepId,
                second.StepId
            }));
            Assert.That(head.formatMajor, Is.EqualTo(current.Major));
        }

        [Test]
        public void MissingFirstStepBlocksAndLeavesSourceUnchanged()
        {
            string source =
                HeadJson(
                    HistoricalA);

            string before =
                new string(
                    source.ToCharArray());

            SavePackageDocumentReadResult result =
                Reader()
                    .ReadCurrent(
                        source,
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationUnavailable));
            Assert.That(result.IsUnsupported, Is.True);
            Assert.That(head, Is.Null);
            Assert.That(source, Is.EqualTo(before));
        }

        [Test]
        public void MissingMiddleStepBlocksCompleteChain()
        {
            VersionRewriteStep first =
                new VersionRewriteStep(
                    "tests.head.a-to-b",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalA,
                    HistoricalB);

            SavePackageDocumentReadResult result =
                Reader(
                        first)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalA),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationUnavailable));
            Assert.That(result.IsUnsupported, Is.True);
            Assert.That(first.Calls, Is.Zero);
            Assert.That(head, Is.Null);
        }

        [Test]
        public void DuplicateOutboundEdgeInvalidatesRegistry()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            SavePackageDocumentMigrationRegistry registry =
                new SavePackageDocumentMigrationRegistry(
                    new ISavePackageDocumentMigrationStep[]
                    {
                        new VersionRewriteStep(
                            "tests.duplicate.one",
                            SaveDocumentKinds.HeadPointer,
                            HistoricalB,
                            current),
                        new VersionRewriteStep(
                            "tests.duplicate.two",
                            SaveDocumentKinds.HeadPointer,
                            HistoricalB,
                            current)
                    });

            Assert.That(registry.IsValid, Is.False);
            Assert.That(registry.DiagnosticCode, Is.EqualTo(
                SavePackageDocumentMigrationDiagnosticCodes.DuplicateEdge));

            SavePackageDocumentReadResult result =
                new SavePackageDocumentReader(
                        new UnityJsonSaveSerializer(),
                        registry)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationUnavailable));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void DowngradeEdgeIsRejectedBeforeItCanFormLoop()
        {
            VersionRewriteStep downgrade =
                new VersionRewriteStep(
                    "tests.loop.downgrade",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    HistoricalA);

            SavePackageDocumentMigrationRegistry registry =
                new SavePackageDocumentMigrationRegistry(
                    new[]
                    {
                        downgrade
                    });

            Assert.That(registry.IsValid, Is.False);
            Assert.That(registry.DiagnosticCode, Is.EqualTo(
                SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid));
            Assert.That(downgrade.Calls, Is.Zero);
        }

        [Test]
        public void PlannerEnforcesConfiguredStepBound()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            SavePackageDocumentMigrationRegistry registry =
                Registry(
                    new VersionRewriteStep(
                        "tests.bound.a-to-b",
                        SaveDocumentKinds.HeadPointer,
                        HistoricalA,
                        HistoricalB),
                    new VersionRewriteStep(
                        "tests.bound.b-to-current",
                        SaveDocumentKinds.HeadPointer,
                        HistoricalB,
                        current));

            SavePackageDocumentMigrationPlanResult result =
                registry.TryBuildPlan(
                    SaveDocumentKinds.HeadPointer,
                    HistoricalA,
                    current,
                    1,
                    out SavePackageDocumentMigrationPlan plan);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentMigrationPlanStatus.StepLimitExceeded));
            Assert.That(plan, Is.Null);
        }

        [Test]
        public void WrongKindOutputIsRejected()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.bad-kind",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    outputDocumentKind:
                        SaveDocumentKinds.Manifest);

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationFailed));
            Assert.That(result.DiagnosticCode, Is.EqualTo(
                SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void WrongTargetVersionOutputIsRejected()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.bad-target",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    outputVersionOverride:
                        HistoricalB);

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationFailed));
            Assert.That(result.DiagnosticCode, Is.EqualTo(
                SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void EmptyStepOutputIsRejected()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.empty-output",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    returnEmpty:
                        true);

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationFailed));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void StepFailureResultBlocksAndPreservesSource()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.reported-failure",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    reportFailure:
                        true);

            string source =
                HeadJson(
                    HistoricalB);
            string before =
                new string(
                    source.ToCharArray());

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        source,
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationFailed));
            Assert.That(result.DiagnosticCode, Is.EqualTo(
                "ESV-TEST-PKG-STEP"));
            Assert.That(source, Is.EqualTo(before));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void StepExceptionIsConvertedToStructuredFailure()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.throw",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current,
                    throwOnMigrate:
                        true);

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.MigrationFailed));
            Assert.That(result.DiagnosticCode, Is.EqualTo(
                SavePackageDocumentMigrationDiagnosticCodes.StepFailed));
            Assert.That(result.Message, Does.Contain("InvalidOperationException"));
            Assert.That(head, Is.Null);
        }

        [Test]
        public void NewerPackageVersionIsRefusedWithoutExecutingSteps()
        {
            SavePackageDocumentVersion newer =
                new SavePackageDocumentVersion(
                    2,
                    0,
                    0);

            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep oldStep =
                new VersionRewriteStep(
                    "tests.old-only",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current);

            SavePackageDocumentReadResult result =
                Reader(
                        oldStep)
                    .ReadCurrent(
                        HeadJson(
                            newer),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.UnsupportedVersion));
            Assert.That(result.IsUnsupported, Is.True);
            Assert.That(oldStep.Calls, Is.Zero);
            Assert.That(head, Is.Null);
        }

        [Test]
        public void MalformedVersionProbeFailsClosedBeforeMigration()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            VersionRewriteStep step =
                new VersionRewriteStep(
                    "tests.malformed-probe",
                    SaveDocumentKinds.HeadPointer,
                    HistoricalB,
                    current);

            string malformed =
                "{\"documentKind\":\"echosave.head\",\"formatMajor\":0}";

            SavePackageDocumentReadResult result =
                Reader(
                        step)
                    .ReadCurrent(
                        malformed,
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Status, Is.EqualTo(
                SavePackageDocumentReadStatus.InvalidDocument));
            Assert.That(step.Calls, Is.Zero);
            Assert.That(head, Is.Null);
        }

        [Test]
        public void MigratedDocumentPassesExistingExactCurrentValidator()
        {
            SavePackageDocumentVersion current =
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer);

            SavePackageDocumentReadResult result =
                Reader(
                        new VersionRewriteStep(
                            "tests.validate-current",
                            SaveDocumentKinds.HeadPointer,
                            HistoricalB,
                            current))
                    .ReadCurrent(
                        HeadJson(
                            HistoricalB),
                        SaveDocumentKinds.HeadPointer,
                        out SaveHeadPointer head);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(
                SavePackageDocumentValidator
                    .ValidateCurrent(
                        head)
                    .Succeeded,
                Is.True);
        }

        [Test]
        public void ExistingCurrentValidatorRemainsStrictForHistoricalDto()
        {
            SaveHeadPointer historical =
                new SaveHeadPointer
                {
                    formatMajor = HistoricalB.Major,
                    formatMinor = HistoricalB.Minor,
                    formatRevision = HistoricalB.Revision
                };

            SaveSerializerResult result =
                SavePackageDocumentValidator
                    .ValidateCurrent(
                        historical);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Status, Is.EqualTo(
                SaveSerializerStatus.UnsupportedDocumentVersion));
        }

        [Test]
        public void CurrentGenerationReaderMigratesHistoricalHeadWithoutParticipantCallbacksOrStorageMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    new DateTime(
                        2026,
                        8,
                        11,
                        12,
                        0,
                        0,
                        DateTimeKind.Utc),
                    1);

                byte[] historicalHead =
                    RewriteHeadVersion(
                        env,
                        HistoricalB);

                byte[] sourceBefore =
                    (byte[])historicalHead.Clone();

                CountingParticipant participant =
                    new CountingParticipant();
                SaveParticipantRegistry participantRegistry =
                    new SaveParticipantRegistry();

                Assert.That(
                    participantRegistry.Register(
                        participant)
                        .Succeeded,
                    Is.True);

                SavePackageDocumentReader packageReader =
                    Reader(
                        new VersionRewriteStep(
                            "tests.reader.head",
                            SaveDocumentKinds.HeadPointer,
                            HistoricalB,
                            CurrentVersion(
                                SaveDocumentKinds.HeadPointer)));

                SaveCurrentGenerationReader reader =
                    new SaveCurrentGenerationReader(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        env.Integrity,
                        participantRegistry,
                        new SaveUnknownPayloadStore(),
                        packageReader);

                SaveCurrentGenerationReadResult result =
                    reader.ReadCurrent(
                        env.SlotId);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(participant.CaptureCalls, Is.Zero);
                Assert.That(participant.ApplyCalls, Is.Zero);
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
                Assert.That(env.ReadHeadBytes(), Is.EqualTo(sourceBefore));
            }
        }

        [Test]
        public void StoredPayloadIntegrityIsCheckedAgainstHistoricalSourceBytesAfterInMemoryMigration()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        new DateTime(
                            2026,
                            8,
                            11,
                            12,
                            10,
                            0,
                            DateTimeKind.Utc),
                        1);

                byte[] historicalPayload =
                    RewritePayloadVersionAndManifestChecksum(
                        env,
                        generation,
                        HistoricalB);

                SavePackageDocumentReader packageReader =
                    Reader(
                        new VersionRewriteStep(
                            "tests.reader.payload",
                            SaveDocumentKinds.Payload,
                            HistoricalB,
                            CurrentVersion(
                                SaveDocumentKinds.Payload)));

                SaveCurrentGenerationReader reader =
                    new SaveCurrentGenerationReader(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        env.Integrity,
                        new SaveParticipantRegistry(),
                        new SaveUnknownPayloadStore(),
                        packageReader);

                SaveCurrentGenerationReadResult result =
                    reader.ReadCurrent(
                        env.SlotId);

                Assert.That(result.Succeeded, Is.True);
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
                Assert.That(
                    ReadGenerationPayload(
                        env,
                        generation),
                    Is.EqualTo(
                        historicalPayload));
            }
        }

        [Test]
        public void CatalogMigratesHistoricalManifestInMemoryWithoutMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        new DateTime(
                            2026,
                            8,
                            11,
                            12,
                            20,
                            0,
                            DateTimeKind.Utc),
                        1);

                byte[] historicalManifest =
                    RewriteManifestVersion(
                        env,
                        generation,
                        HistoricalB);

                SavePackageDocumentReader packageReader =
                    Reader(
                        new VersionRewriteStep(
                            "tests.catalog.manifest",
                            SaveDocumentKinds.Manifest,
                            HistoricalB,
                            CurrentVersion(
                                SaveDocumentKinds.Manifest)));

                SaveSlotCatalogScanner scanner =
                    new SaveSlotCatalogScanner(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        512,
                        packageReader);

                SaveSlotCatalogRefreshResult result =
                    scanner.Scan();

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Snapshot.TryGetEntry(
                    env.SlotId,
                    out SaveSlotCatalogEntry entry), Is.True);
                Assert.That(entry.Health, Is.EqualTo(
                    SaveSlotHealth.Healthy));
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
                Assert.That(
                    ReadGenerationManifest(
                        env,
                        generation),
                    Is.EqualTo(
                        historicalManifest));
            }
        }

        [Test]
        public void CatalogReportsUnmigratableHistoricalManifestAsUnsupportedWithoutMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        new DateTime(
                            2026,
                            8,
                            11,
                            12,
                            30,
                            0,
                            DateTimeKind.Utc),
                        1);

                RewriteManifestVersion(
                    env,
                    generation,
                    HistoricalB);

                SaveSlotCatalogScanner scanner =
                    new SaveSlotCatalogScanner(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        512,
                        Reader());

                SaveSlotCatalogRefreshResult result =
                    scanner.Scan();

                Assert.That(result.Snapshot.TryGetEntry(
                    env.SlotId,
                    out SaveSlotCatalogEntry entry), Is.True);
                Assert.That(entry.Health, Is.EqualTo(
                    SaveSlotHealth.UnsupportedManifest));
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
            }
        }

        [Test]
        public void RecoveryPlanningMigratesHistoricalManifestInMemoryWithoutMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        new DateTime(
                            2026,
                            8,
                            11,
                            12,
                            40,
                            0,
                            DateTimeKind.Utc),
                        1);

                byte[] historicalManifest =
                    RewriteManifestVersion(
                        env,
                        generation,
                        HistoricalB);

                SavePackageDocumentReader packageReader =
                    Reader(
                        new VersionRewriteStep(
                            "tests.recovery.manifest",
                            SaveDocumentKinds.Manifest,
                            HistoricalB,
                            CurrentVersion(
                                SaveDocumentKinds.Manifest)));

                SaveRecoveryPlanBuilder builder =
                    new SaveRecoveryPlanBuilder(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        env.Integrity,
                        packageReader);

                SaveRecoveryPlan plan =
                    builder.Build(
                        env.SlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(plan.HeadCondition, Is.EqualTo(
                    SaveRecoveryHeadCondition.Healthy));
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
                Assert.That(
                    ReadGenerationManifest(
                        env,
                        generation),
                    Is.EqualTo(
                        historicalManifest));
            }
        }

        [Test]
        public void RecoveryPlanningRejectsUnmigratableHistoricalManifestWithoutMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        new DateTime(
                            2026,
                            8,
                            11,
                            12,
                            45,
                            0,
                            DateTimeKind.Utc),
                        1);

                byte[] historicalManifest =
                    RewriteManifestVersion(
                        env,
                        generation,
                        HistoricalB);

                SaveRecoveryPlanBuilder builder =
                    new SaveRecoveryPlanBuilder(
                        env.ReadOnlyBackend,
                        env.Serializer,
                        env.Integrity,
                        Reader());

                SaveRecoveryPlan plan =
                    builder.Build(
                        env.SlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(plan.RecoveryRequired, Is.True);
                Assert.That(plan.HasPreferredCandidate, Is.False);
                Assert.That(env.ReadOnlyBackend.MutationCalls, Is.Zero);
                Assert.That(
                    ReadGenerationManifest(
                        env,
                        generation),
                    Is.EqualTo(
                        historicalManifest));
            }
        }

        [Test]
        public void ProductionRegistryIsEmptyUntilARealPackageFormatBumpExists()
        {
            SavePackageDocumentMigrationRegistry registry =
                SavePackageDocumentMigrationRegistry
                    .CreateProduction();

            Assert.That(registry.IsValid, Is.True);
            Assert.That(registry.Count, Is.Zero);
            Assert.That(
                CurrentVersion(
                    SaveDocumentKinds.Envelope),
                Is.EqualTo(
                    new SavePackageDocumentVersion(
                        1,
                        0,
                        0)));
            Assert.That(
                CurrentVersion(
                    SaveDocumentKinds.Manifest),
                Is.EqualTo(
                    new SavePackageDocumentVersion(
                        1,
                        0,
                        0)));
            Assert.That(
                CurrentVersion(
                    SaveDocumentKinds.Payload),
                Is.EqualTo(
                    new SavePackageDocumentVersion(
                        1,
                        0,
                        0)));
            Assert.That(
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer),
                Is.EqualTo(
                    new SavePackageDocumentVersion(
                        1,
                        0,
                        0)));
        }

        private static SavePackageDocumentReader Reader(
            params ISavePackageDocumentMigrationStep[] steps) =>
            new SavePackageDocumentReader(
                new UnityJsonSaveSerializer(),
                Registry(
                    steps));

        private static SavePackageDocumentMigrationRegistry Registry(
            params ISavePackageDocumentMigrationStep[] steps) =>
            new SavePackageDocumentMigrationRegistry(
                steps ??
                Array.Empty<ISavePackageDocumentMigrationStep>());

        private static SavePackageDocumentVersion CurrentVersion(
            string documentKind)
        {
            Assert.That(
                SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion version),
                Is.True);

            return version;
        }

        private static string CurrentHeadJson() =>
            HeadJson(
                CurrentVersion(
                    SaveDocumentKinds.HeadPointer));

        private static string HeadJson(
            SavePackageDocumentVersion version)
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    formatMajor = version.Major,
                    formatMinor = version.Minor,
                    formatRevision = version.Revision,
                    slotId = SaveSlotId.NewId().Value,
                    currentGenerationId = SaveGenerationId.NewId().Value,
                    previousGenerationId = string.Empty,
                    updateSequence = 1
                };

            return JsonUtility.ToJson(
                head,
                false);
        }

        private static byte[] RewriteHeadVersion(
            SaveRecoveryTestEnvironment env,
            SavePackageDocumentVersion version)
        {
            string source =
                Encoding.UTF8.GetString(
                    env.ReadHeadBytes());

            string historical =
                RewriteVersion(
                    source,
                    CurrentVersion(
                        SaveDocumentKinds.HeadPointer),
                    version);

            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    historical);

            env.RestoreHeadBytes(
                bytes);

            return bytes;
        }

        private static byte[] RewriteManifestVersion(
            SaveRecoveryTestEnvironment env,
            SaveGenerationId generation,
            SavePackageDocumentVersion version)
        {
            SaveGenerationStorageKeys.TryCreate(
                env.SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                env.Local.Read(
                    keys.GenerationManifest);

            Assert.That(read.Succeeded, Is.True);

            string historical =
                RewriteVersion(
                    Encoding.UTF8.GetString(
                        read.Data),
                    CurrentVersion(
                        SaveDocumentKinds.Manifest),
                    version);

            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    historical);

            Assert.That(
                env.Local.Delete(
                    keys.GenerationManifest)
                    .Succeeded,
                Is.True);
            Assert.That(
                env.Local.WriteNew(
                    keys.GenerationManifest,
                    bytes)
                    .Succeeded,
                Is.True);

            return bytes;
        }

        private static byte[] RewritePayloadVersionAndManifestChecksum(
            SaveRecoveryTestEnvironment env,
            SaveGenerationId generation,
            SavePackageDocumentVersion version)
        {
            SaveGenerationStorageKeys.TryCreate(
                env.SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult payloadRead =
                env.Local.Read(
                    keys.GenerationPayload);
            SaveStorageReadResult manifestRead =
                env.Local.Read(
                    keys.GenerationManifest);

            Assert.That(payloadRead.Succeeded, Is.True);
            Assert.That(manifestRead.Succeeded, Is.True);

            string historicalPayloadText =
                RewriteVersion(
                    Encoding.UTF8.GetString(
                        payloadRead.Data),
                    CurrentVersion(
                        SaveDocumentKinds.Payload),
                    version);

            byte[] historicalPayload =
                Encoding.UTF8.GetBytes(
                    historicalPayloadText);

            Assert.That(
                env.Integrity.Calculate(
                    historicalPayload,
                    out string checksum)
                    .Succeeded,
                Is.True);

            Assert.That(
                env.Serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest)
                    .Succeeded,
                Is.True);

            manifest.payloadByteLength =
                historicalPayload.LongLength;
            manifest.payloadChecksum =
                checksum;

            Assert.That(
                env.Serializer.Serialize(
                    manifest,
                    out string manifestJson)
                    .Succeeded,
                Is.True);

            Assert.That(
                env.Local.Delete(
                    keys.GenerationPayload)
                    .Succeeded,
                Is.True);
            Assert.That(
                env.Local.Delete(
                    keys.GenerationManifest)
                    .Succeeded,
                Is.True);
            Assert.That(
                env.Local.WriteNew(
                    keys.GenerationPayload,
                    historicalPayload)
                    .Succeeded,
                Is.True);
            Assert.That(
                env.Local.WriteNew(
                    keys.GenerationManifest,
                    Encoding.UTF8.GetBytes(
                        manifestJson))
                    .Succeeded,
                Is.True);

            return historicalPayload;
        }

        private static byte[] ReadGenerationManifest(
            SaveRecoveryTestEnvironment env,
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                env.SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                env.Local.Read(
                    keys.GenerationManifest);

            Assert.That(read.Succeeded, Is.True);
            return read.Data;
        }

        private static byte[] ReadGenerationPayload(
            SaveRecoveryTestEnvironment env,
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                env.SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                env.Local.Read(
                    keys.GenerationPayload);

            Assert.That(read.Succeeded, Is.True);
            return read.Data;
        }

        private static string RewriteVersion(
            string serializedDocument,
            SavePackageDocumentVersion source,
            SavePackageDocumentVersion target)
        {
            string result =
                ReplaceIntegerField(
                    serializedDocument,
                    "formatMajor",
                    source.Major,
                    target.Major);

            result =
                ReplaceIntegerField(
                    result,
                    "formatMinor",
                    source.Minor,
                    target.Minor);

            return ReplaceIntegerField(
                result,
                "formatRevision",
                source.Revision,
                target.Revision);
        }

        private static string ReplaceDocumentKind(
            string serializedDocument,
            string sourceKind,
            string targetKind)
        {
            string source =
                "\"documentKind\":\"" +
                sourceKind +
                "\"";
            string target =
                "\"documentKind\":\"" +
                targetKind +
                "\"";

            int index =
                serializedDocument.IndexOf(
                    source,
                    StringComparison.Ordinal);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    "Fixture could not locate documentKind.");
            }

            return
                serializedDocument.Substring(
                    0,
                    index) +
                target +
                serializedDocument.Substring(
                    index + source.Length);
        }

        private static string ReplaceIntegerField(
            string serializedDocument,
            string fieldName,
            int expected,
            int replacement)
        {
            string marker =
                "\"" +
                fieldName +
                "\"";

            int markerIndex =
                serializedDocument.IndexOf(
                    marker,
                    StringComparison.Ordinal);

            if (markerIndex < 0)
            {
                throw new InvalidOperationException(
                    "Fixture could not locate " +
                    fieldName +
                    ".");
            }

            int colon =
                serializedDocument.IndexOf(
                    ':',
                    markerIndex + marker.Length);

            if (colon < 0)
            {
                throw new InvalidOperationException(
                    "Fixture could not locate numeric separator for " +
                    fieldName +
                    ".");
            }

            int start =
                colon + 1;
            while (start < serializedDocument.Length &&
                   char.IsWhiteSpace(
                       serializedDocument[start]))
            {
                start++;
            }

            int end = start;
            if (end < serializedDocument.Length &&
                serializedDocument[end] == '-')
            {
                end++;
            }

            while (end < serializedDocument.Length &&
                   char.IsDigit(
                       serializedDocument[end]))
            {
                end++;
            }

            string observed =
                serializedDocument.Substring(
                    start,
                    end - start);

            if (!string.Equals(
                    observed,
                    expected.ToString(
                        CultureInfo.InvariantCulture),
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Fixture version field " +
                    fieldName +
                    " did not contain the expected source value.");
            }

            return
                serializedDocument.Substring(
                    0,
                    start) +
                replacement.ToString(
                    CultureInfo.InvariantCulture) +
                serializedDocument.Substring(
                    end);
        }

        private sealed class VersionRewriteStep :
            ISavePackageDocumentMigrationStep
        {
            private readonly List<string> trace;
            private readonly string outputDocumentKind;
            private readonly SavePackageDocumentVersion? outputVersionOverride;
            private readonly bool returnEmpty;
            private readonly bool reportFailure;
            private readonly bool throwOnMigrate;

            internal VersionRewriteStep(
                string stepId,
                string documentKind,
                SavePackageDocumentVersion sourceVersion,
                SavePackageDocumentVersion targetVersion,
                List<string> trace = null,
                string outputDocumentKind = null,
                SavePackageDocumentVersion? outputVersionOverride = null,
                bool returnEmpty = false,
                bool reportFailure = false,
                bool throwOnMigrate = false)
            {
                StepId = stepId;
                DocumentKind = documentKind;
                SourceVersion = sourceVersion;
                TargetVersion = targetVersion;
                this.trace = trace;
                this.outputDocumentKind = outputDocumentKind;
                this.outputVersionOverride = outputVersionOverride;
                this.returnEmpty = returnEmpty;
                this.reportFailure = reportFailure;
                this.throwOnMigrate = throwOnMigrate;
            }

            public string StepId { get; }

            public string DocumentKind { get; }

            public SavePackageDocumentVersion SourceVersion { get; }

            public SavePackageDocumentVersion TargetVersion { get; }

            internal int Calls { get; private set; }

            public SavePackageDocumentMigrationStepResult Migrate(
                string serializedDocument)
            {
                Calls++;
                trace?.Add(
                    StepId);

                if (throwOnMigrate)
                {
                    throw new InvalidOperationException(
                        "Injected package-document migration exception.");
                }

                if (reportFailure)
                {
                    return SavePackageDocumentMigrationStepResult.Failure(
                        "ESV-TEST-PKG-STEP",
                        "Injected package-document migration failure.");
                }

                if (returnEmpty)
                {
                    return SavePackageDocumentMigrationStepResult.Success(
                        string.Empty);
                }

                SavePackageDocumentVersion outputVersion =
                    outputVersionOverride ??
                    TargetVersion;

                string output =
                    RewriteVersion(
                        serializedDocument,
                        SourceVersion,
                        outputVersion);

                if (!string.IsNullOrEmpty(
                        outputDocumentKind) &&
                    !string.Equals(
                        outputDocumentKind,
                        DocumentKind,
                        StringComparison.Ordinal))
                {
                    output =
                        ReplaceDocumentKind(
                            output,
                            DocumentKind,
                            outputDocumentKind);
                }

                return SavePackageDocumentMigrationStepResult.Success(
                    output);
            }
        }

        private sealed class CountingParticipant :
            ISaveParticipant
        {
            public SaveParticipantDescriptor Descriptor { get; } =
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        "com.example.r3-order"),
                    1,
                    SaveParticipantCriticality.Optional,
                    SaveMissingPayloadPolicy.Ignore,
                    default);

            internal int CaptureCalls { get; private set; }

            internal int ApplyCalls { get; private set; }

            public SaveParticipantCaptureResult Capture()
            {
                CaptureCalls++;
                return SaveParticipantCaptureResult.Failure(
                    "R3 package-document migration tests forbid participant capture.");
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;
                return SaveParticipantApplyResult.Failure(
                    "R3 package-document migration tests forbid participant apply.");
            }
        }
    }
}
