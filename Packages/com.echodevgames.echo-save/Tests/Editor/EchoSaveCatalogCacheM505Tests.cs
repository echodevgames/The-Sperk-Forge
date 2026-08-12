
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveCatalogCacheM505Tests
    {
        [Test]
        public void PreviewMissing_IsZeroWrite_AndExplicitRebuildCreatesOnlyCache()
        {
            using (CacheFixture fixture =
                   CacheFixture.Create())
            {
                SaveCatalogCacheCoordinator cache =
                    fixture.CreateCoordinator();

                SaveStorageKey.TryCreate(
                    SaveCatalogCacheCoordinator.CacheFileName,
                    out SaveStorageKey cacheKey);

                fixture.Storage.Exists(
                    cacheKey,
                    out bool existsBefore);

                Assert.That(
                    existsBefore,
                    Is.False);

                SaveCatalogCachePreview preview =
                    cache.Preview();

                Assert.That(
                    preview.State,
                    Is.EqualTo(
                        SaveCatalogCacheState.Missing));

                Assert.That(
                    preview.DurableSnapshot.Count,
                    Is.EqualTo(1));

                fixture.Storage.Exists(
                    cacheKey,
                    out bool existsAfterPreview);

                Assert.That(
                    existsAfterPreview,
                    Is.False,
                    "Cache Preview must perform zero writes.");

                string[] before =
                    Directory.GetFiles(
                        fixture.Root,
                        "*",
                        SearchOption.AllDirectories);

                SaveCatalogCacheRebuildResult rebuilt =
                    cache.Rebuild();

                Assert.That(
                    rebuilt.Succeeded,
                    Is.True,
                    rebuilt.Message);

                string[] after =
                    Directory.GetFiles(
                        fixture.Root,
                        "*",
                        SearchOption.AllDirectories);

                Assert.That(
                    after.Length,
                    Is.EqualTo(
                        before.Length + 1));

                SaveCatalogCachePreview valid =
                    cache.Preview();

                Assert.That(
                    valid.State,
                    Is.EqualTo(
                        SaveCatalogCacheState.Valid));

                Assert.That(
                    valid.CacheFingerprint,
                    Is.EqualTo(
                        valid.DurableFingerprint));
            }
        }

        [Test]
        public void DurableGenerationChange_MarksCacheStale_UntilRebuilt()
        {
            using (CacheFixture fixture =
                   CacheFixture.Create())
            {
                SaveCatalogCacheCoordinator cache =
                    fixture.CreateCoordinator();

                Assert.That(
                    cache.Rebuild().Succeeded,
                    Is.True);

                SaveGenerationPublicationResult second =
                    fixture.Publication
                        .PublishStoredTransportGeneration(
                            fixture.SlotId,
                            "project",
                            "1",
                            "build",
                            "Cache",
                            fixture.Entries,
                            Inventory(
                                fixture.Entries),
                            "transport",
                            fixture.InitialGeneration);

                Assert.That(
                    second.Succeeded,
                    Is.True,
                    second.Message);

                Assert.That(
                    cache.Preview().State,
                    Is.EqualTo(
                        SaveCatalogCacheState.Stale));

                Assert.That(
                    cache.Rebuild().Succeeded,
                    Is.True);

                Assert.That(
                    cache.Preview().State,
                    Is.EqualTo(
                        SaveCatalogCacheState.Valid));
            }
        }

        [Test]
        public void CorruptCache_DoesNotMaskDurableCatalogTruth()
        {
            using (CacheFixture fixture =
                   CacheFixture.Create())
            {
                SaveCatalogCacheCoordinator cache =
                    fixture.CreateCoordinator();

                Assert.That(
                    cache.Rebuild().Succeeded,
                    Is.True);

                SaveStorageKey.TryCreate(
                    SaveCatalogCacheCoordinator.CacheFileName,
                    out SaveStorageKey key);

                Assert.That(
                    ((ISaveStoragePublicationBackend)
                        fixture.Storage)
                        .PublishCurrentObject(
                            key,
                            Encoding.UTF8.GetBytes(
                                "{not-json"))
                        .Succeeded,
                    Is.True);

                Assert.That(
                    cache.Preview().State,
                    Is.EqualTo(
                        SaveCatalogCacheState.Corrupt));

                SaveSlotCatalog catalog =
                    new SaveSlotCatalog(
                        fixture.Storage,
                        fixture.Serializer,
                        64,
                        true);

                SaveSlotCatalogRefreshResult refresh =
                    catalog.Refresh();

                Assert.That(
                    refresh.Succeeded,
                    Is.True,
                    refresh.Message);

                Assert.That(
                    refresh.Snapshot.Count,
                    Is.EqualTo(1));

                Assert.That(
                    refresh.Snapshot.TryGetEntry(
                        fixture.SlotId,
                        out SaveSlotCatalogEntry entry),
                    Is.True);

                Assert.That(
                    entry.Health,
                    Is.EqualTo(
                        SaveSlotHealth.Healthy));

                Assert.That(
                    refresh.CacheMaintenanceStatus,
                    Is.EqualTo(
                        SaveCatalogCacheMaintenanceStatus.Rebuilt));
            }
        }

        [Test]
        public void CachePublicationFailure_LeavesSuccessfulDurableRefreshTruthful()
        {
            using (CacheFixture fixture =
                   CacheFixture.Create())
            {
                RejectCachePublicationBackend storage =
                    new RejectCachePublicationBackend(
                        fixture.Storage);

                SaveSlotCatalog catalog =
                    new SaveSlotCatalog(
                        storage,
                        fixture.Serializer,
                        64,
                        true);

                SaveSlotCatalogRefreshResult refresh =
                    catalog.Refresh();

                Assert.That(
                    refresh.Succeeded,
                    Is.True,
                    refresh.Message);

                Assert.That(
                    refresh.Snapshot.Count,
                    Is.EqualTo(1));

                Assert.That(
                    refresh.CacheMaintenanceStatus,
                    Is.EqualTo(
                        SaveCatalogCacheMaintenanceStatus
                            .RebuildFailed));

                Assert.That(
                    refresh.CacheMaintenanceFailed,
                    Is.True);
            }
        }

        private static SavePayloadEntry[] OneEntry(
            IIntegrityProvider integrity)
        {
            string payload =
                "{\"value\":1}";

            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    payload);

            Assert.That(
                integrity.Calculate(
                        bytes,
                        out string checksum)
                    .Succeeded,
                Is.True);

            return new[]
            {
                new SavePayloadEntry
                {
                    participantId =
                        "com.example.cache",
                    participantSchemaVersion =
                        1,
                    serializerId =
                        UnityJsonSaveSerializer.StableId,
                    required =
                        false,
                    serializedPayload =
                        payload,
                    byteProviderReference =
                        string.Empty,
                    byteLength =
                        bytes.LongLength,
                    checksum =
                        checksum,
                    flags =
                        0
                }
            };
        }

        private static SavePayloadInventoryEntry[] Inventory(
            SavePayloadEntry[] entries) =>
            new[]
            {
                new SavePayloadInventoryEntry
                {
                    participantId =
                        entries[0].participantId,
                    participantSchemaVersion =
                        entries[0].participantSchemaVersion,
                    serializerId =
                        entries[0].serializerId,
                    required =
                        entries[0].required,
                    byteLength =
                        entries[0].byteLength,
                    checksum =
                        entries[0].checksum,
                    flags =
                        entries[0].flags
                }
            };

        private sealed class CacheFixture :
            IDisposable
        {
            private CacheFixture()
            {
            }

            internal string Root { get; private set; }

            internal LocalFileSaveStorageBackend Storage { get; private set; }

            internal UnityJsonSaveSerializer Serializer { get; private set; }

            internal Sha256IntegrityProvider Integrity { get; private set; }

            internal SaveGenerationPublicationCoordinator Publication
            {
                get;
                private set;
            }

            internal SaveSlotId SlotId { get; private set; }

            internal SaveGenerationId InitialGeneration { get; private set; }

            internal SavePayloadEntry[] Entries { get; private set; }

            internal SaveCatalogCacheCoordinator CreateCoordinator() =>
                new SaveCatalogCacheCoordinator(
                    Storage,
                    Serializer,
                    64);

            internal static CacheFixture Create()
            {
                CacheFixture fixture =
                    new CacheFixture();

                fixture.Root =
                    Path.Combine(
                        Path.GetTempPath(),
                        "EchoSave-M505-Cache-" +
                        Guid.NewGuid().ToString("N"));

                fixture.Storage =
                    new LocalFileSaveStorageBackend(
                        fixture.Root);

                Assert.That(
                    fixture.Storage.Initialize().Succeeded,
                    Is.True);

                fixture.Serializer =
                    new UnityJsonSaveSerializer();

                fixture.Integrity =
                    new Sha256IntegrityProvider();

                fixture.Publication =
                    new SaveGenerationPublicationCoordinator(
                        fixture.Storage,
                        fixture.Serializer,
                        fixture.Integrity);

                fixture.SlotId =
                    SaveSlotId.NewId();

                fixture.Entries =
                    OneEntry(
                        fixture.Integrity);

                SaveGenerationPublicationResult initial =
                    fixture.Publication
                        .PublishInitialStoredTransportGeneration(
                            fixture.SlotId,
                            "project",
                            "1",
                            "build",
                            "Cache",
                            fixture.Entries,
                            Inventory(
                                fixture.Entries),
                            "transport");

                Assert.That(
                    initial.Succeeded,
                    Is.True,
                    initial.Message);

                fixture.InitialGeneration =
                    initial.GenerationId;

                return fixture;
            }

            public void Dispose()
            {
                Storage?.Shutdown();

                if (!string.IsNullOrEmpty(
                        Root) &&
                    Directory.Exists(
                        Root))
                {
                    Directory.Delete(
                        Root,
                        true);
                }
            }
        }

        private sealed class RejectCachePublicationBackend :
            ISaveStoragePublicationBackend,
            ISaveStorageDiscoveryBackend
        {
            private readonly LocalFileSaveStorageBackend inner;

            internal RejectCachePublicationBackend(
                LocalFileSaveStorageBackend inner)
            {
                this.inner =
                    inner;
            }

            public SaveStorageBackendId Id =>
                inner.Id;

            public string RootPath =>
                inner.RootPath;

            public SaveStoragePublicationCapabilities
                PublicationCapabilities =>
                inner.PublicationCapabilities;

            public SaveStorageResult Initialize() =>
                inner.Initialize();

            public SaveStorageResult Exists(
                SaveStorageKey key,
                out bool exists) =>
                inner.Exists(
                    key,
                    out exists);

            public SaveStorageReadResult Read(
                SaveStorageKey key) =>
                inner.Read(
                    key);

            public SaveStorageResult WriteNew(
                SaveStorageKey key,
                byte[] data) =>
                inner.WriteNew(
                    key,
                    data);

            public SaveStorageResult Delete(
                SaveStorageKey key) =>
                inner.Delete(
                    key);

            public SaveStorageResult Shutdown() =>
                inner.Shutdown();

            public SaveStorageResult PublishNewTree(
                SaveStorageKey sourceDirectoryKey,
                SaveStorageKey destinationDirectoryKey) =>
                inner.PublishNewTree(
                    sourceDirectoryKey,
                    destinationDirectoryKey);

            public SaveStorageResult PublishCurrentObject(
                SaveStorageKey key,
                byte[] data)
            {
                if (string.Equals(
                        key.Value,
                        SaveCatalogCacheCoordinator.CacheFileName,
                        StringComparison.Ordinal))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.Failed,
                        "TEST-CACHE-WRITE",
                        "The test backend intentionally rejects derived cache publication.");
                }

                return inner.PublishCurrentObject(
                    key,
                    data);
            }

            public SaveStorageDiscoveryResult
                DiscoverChildDirectories(
                    SaveStorageKey parentKey,
                    int maxChildren) =>
                inner.DiscoverChildDirectories(
                    parentKey,
                    maxChildren);
        }
    }
}
