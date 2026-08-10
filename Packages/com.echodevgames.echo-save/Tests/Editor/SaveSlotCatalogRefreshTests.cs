
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotCatalogRefreshTests
    {
        [Test]
        public void MissingSlotsRootProducesSuccessfulEmptyCatalog()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            backend.SetChildren(
                null);

            SaveSlotCatalogRefreshResult result =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSlotCatalogRefreshStatus.SucceededEmpty));

            Assert.That(
                result.Snapshot.Count,
                Is.Zero);
        }

        [Test]
        public void InvalidChildNamesDoNotBecomeSlots()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId valid =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                "not-a-slot",
                valid.Value,
                "AAAAAAAA-BBBB-CCCC-DDDD-000000000002");

            SlotCatalogTestSupport.PutHealthy(
                backend,
                valid,
                generation);

            SaveSlotCatalogRefreshResult result =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh();

            Assert.That(
                result.Snapshot.Count,
                Is.EqualTo(1));

            Assert.That(
                result.Snapshot.Entries[0].SlotId,
                Is.EqualTo(
                    valid));
        }

        [Test]
        public void ProviderEnumerationOrderDoesNotAffectCatalogOrder()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId first =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveSlotId second =
                SlotCatalogTestSupport.Slot(
                    2);

            backend.SetChildren(
                second.Value,
                first.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                first,
                SlotCatalogTestSupport.Generation(
                    1));

            SlotCatalogTestSupport.PutHealthy(
                backend,
                second,
                SlotCatalogTestSupport.Generation(
                    2));

            SaveSlotCatalogSnapshot snapshot =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot;

            Assert.That(
                snapshot.Entries[0].SlotId,
                Is.EqualTo(
                    first));

            Assert.That(
                snapshot.Entries[1].SlotId,
                Is.EqualTo(
                    second));
        }

        [Test]
        public void HealthyCatalogReadsHeadAndManifestButNeverPayload()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation,
                "Player Display Name");

            SaveSlotCatalogRefreshResult result =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh();

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Snapshot.Entries[0].Health,
                Is.EqualTo(
                    SaveSlotHealth.Healthy));

            Assert.That(
                result.Snapshot.Entries[0].DisplayName,
                Is.EqualTo(
                    "Player Display Name"));

            Assert.That(
                backend.ReadKeys,
                Does.Contain(
                    SlotCatalogTestSupport.HeadKey(
                        slot)));

            Assert.That(
                backend.ReadKeys,
                Does.Contain(
                    SlotCatalogTestSupport.ManifestKey(
                        slot,
                        generation)));

            Assert.That(
                backend.ReadKeys,
                Does.Not.Contain(
                    SlotCatalogTestSupport.PayloadKey(
                        slot,
                        generation)));
        }

        [Test]
        public void MissingHeadProducesDegradedNonSelectableEntry()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SaveSlotCatalogEntry entry =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0];

            Assert.That(
                entry.Health,
                Is.EqualTo(
                    SaveSlotHealth.MissingHead));

            Assert.That(
                entry.IsSelectable,
                Is.False);
        }

        [Test]
        public void InvalidHeadProducesDegradedEntry()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            backend.Put(
                SlotCatalogTestSupport.HeadKey(
                    slot),
                "{broken");

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.InvalidHead));
        }

        [Test]
        public void UnsupportedHeadProducesDistinctHealth()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            UnityJsonSaveSerializer serializer =
                new UnityJsonSaveSerializer();

            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId =
                        slot.Value,
                    currentGenerationId =
                        generation.Value,
                    previousGenerationId =
                        string.Empty,
                    updateSequence =
                        1
                };

            Assert.That(
                serializer.Serialize(
                    head,
                    out string headJson)
                    .Succeeded,
                Is.True);

            string unsupportedHeadJson =
                headJson.Replace(
                    "\"formatMajor\":" +
                    head.formatMajor,
                    "\"formatMajor\":999");

            Assert.That(
                unsupportedHeadJson,
                Is.Not.EqualTo(
                    headJson));

            backend.Put(
                SlotCatalogTestSupport.HeadKey(
                    slot),
                unsupportedHeadJson);

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.UnsupportedHead));
        }

        [Test]
        public void MissingManifestProducesDegradedEntry()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation);

            backend.Remove(
                SlotCatalogTestSupport.ManifestKey(
                    slot,
                    generation));

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.MissingManifest));
        }

        [Test]
        public void InvalidManifestProducesDegradedEntry()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation);

            backend.Put(
                SlotCatalogTestSupport.ManifestKey(
                    slot,
                    generation),
                "{broken");

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.InvalidManifest));
        }

        [Test]
        public void UnsupportedManifestProducesDistinctHealth()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation);

            UnityJsonSaveSerializer serializer =
                new UnityJsonSaveSerializer();

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId =
                        slot.Value,
                    generationId =
                        generation.Value,
                    createdUtc =
                        "2026-08-10T09:00:00.0000000+00:00",
                    updatedUtc =
                        "2026-08-10T10:00:00.0000000+00:00",
                    saveKind =
                        "manual",
                    projectId =
                        "test-project",
                    projectVersion =
                        "1.0.0",
                    buildId =
                        "build-1",
                    displayName =
                        "Save",
                    payloadByteLength =
                        0,
                    payloadEntries =
                        Array.Empty<SavePayloadInventoryEntry>(),
                    commitState =
                        SaveGenerationCommitState.Committed
                };

            Assert.That(
                serializer.Serialize(
                    manifest,
                    out string manifestJson)
                    .Succeeded,
                Is.True);

            string unsupportedManifestJson =
                manifestJson.Replace(
                    "\"formatMajor\":" +
                    manifest.formatMajor,
                    "\"formatMajor\":999");

            Assert.That(
                unsupportedManifestJson,
                Is.Not.EqualTo(
                    manifestJson));

            backend.Put(
                SlotCatalogTestSupport.ManifestKey(
                    slot,
                    generation),
                unsupportedManifestJson);

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.UnsupportedManifest));
        }

        [Test]
        public void DisplayNameNeverBecomesStorageKey()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            const string displayName =
                "../PLAYER DISPLAY NAME";

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation,
                displayName);

            SaveSlotCatalogEntry entry =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0];

            Assert.That(
                entry.DisplayName,
                Is.EqualTo(
                    displayName));

            Assert.That(
                backend.ReadKeys,
                Has.None.Contains(
                    displayName));
        }

        [Test]
        public void HeadManifestIdentityMismatchProducesDegradedEntry()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveSlotId wrongSlot =
                SlotCatalogTestSupport.Slot(
                    2);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation);

            UnityJsonSaveSerializer serializer =
                new UnityJsonSaveSerializer();

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId =
                        wrongSlot.Value,
                    generationId =
                        generation.Value,
                    commitState =
                        SaveGenerationCommitState.Committed
                };

            serializer.Serialize(
                manifest,
                out string json);

            backend.Put(
                SlotCatalogTestSupport.ManifestKey(
                    slot,
                    generation),
                json);

            Assert.That(
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0]
                    .Health,
                Is.EqualTo(
                    SaveSlotHealth.IdentityMismatch));
        }

        [Test]
        public void OneDegradedSlotDoesNotEraseHealthySlot()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId healthy =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveSlotId degraded =
                SlotCatalogTestSupport.Slot(
                    2);

            backend.SetChildren(
                degraded.Value,
                healthy.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                healthy,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalogRefreshResult result =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSlotCatalogRefreshStatus.SucceededWithDegradedSlots));

            Assert.That(
                result.Snapshot.Count,
                Is.EqualTo(2));

            Assert.That(
                result.Snapshot.HealthyCount,
                Is.EqualTo(1));

            Assert.That(
                result.Snapshot.DegradedCount,
                Is.EqualTo(1));
        }

        [Test]
        public void DiscoveryFailurePreservesPreviousSnapshot()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            SaveSlotCatalogSnapshot first =
                catalog.Refresh()
                    .Snapshot;

            backend.DiscoveryFails =
                true;

            SaveSlotCatalogRefreshResult failed =
                catalog.Refresh();

            Assert.That(
                failed.Succeeded,
                Is.False);

            Assert.That(
                failed.Snapshot,
                Is.SameAs(
                    first));

            Assert.That(
                catalog.Snapshot,
                Is.SameAs(
                    first));
        }

        [Test]
        public void ScanLimitFailurePreservesPreviousSnapshot()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            SaveSlotCatalogSnapshot first =
                catalog.Refresh()
                    .Snapshot;

            backend.DiscoveryLimitExceeded =
                true;

            SaveSlotCatalogRefreshResult failed =
                catalog.Refresh();

            Assert.That(
                failed.Status,
                Is.EqualTo(
                    SaveSlotCatalogRefreshStatus.ScanLimitExceeded));

            Assert.That(
                failed.Snapshot,
                Is.SameAs(
                    first));
        }

        [Test]
        public void BackendReadFailureIsRepresentedWithoutPathLeak()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            backend.FailReadKey =
                SlotCatalogTestSupport.HeadKey(
                    slot);

            SaveSlotCatalogEntry entry =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot
                    .Entries[0];

            Assert.That(
                entry.Health,
                Is.EqualTo(
                    SaveSlotHealth.BackendReadFailure));

            Assert.That(
                entry.Message,
                Does.Not.Contain(
                    backend.RootPath));
        }
    }
}
