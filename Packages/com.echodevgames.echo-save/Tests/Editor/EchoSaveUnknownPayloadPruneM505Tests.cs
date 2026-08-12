
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveUnknownPayloadPruneM505Tests
    {
        [Test]
        public void PrepareAndConfirm_PrunesExactlyOneUnknownIntoNewGeneration_WithoutRewritingSource()
        {
            string root =
                TempRoot();

            LocalFileSaveStorageBackend storage =
                new LocalFileSaveStorageBackend(
                    root);

            Assert.That(
                storage.Initialize().Succeeded,
                Is.True);

            UnityJsonSaveSerializer serializer =
                new UnityJsonSaveSerializer();

            Sha256IntegrityProvider integrity =
                new Sha256IntegrityProvider();

            SaveParticipantRegistry registry =
                new SaveParticipantRegistry();

            SaveUnknownPayloadStore unknown =
                new SaveUnknownPayloadStore();

            SaveSlotCatalog catalog =
                new SaveSlotCatalog(
                    storage,
                    serializer,
                    64);

            SaveGenerationPublicationCoordinator publication =
                new SaveGenerationPublicationCoordinator(
                    storage,
                    serializer,
                    integrity);

            SaveGenerationRetentionCoordinator retention =
                new SaveGenerationRetentionCoordinator(
                    storage,
                    serializer,
                    64);

            SaveSlotId slotId =
                SaveSlotId.NewId();

            try
            {
                Assert.That(
                    registry.Register(
                            new TestParticipant(
                                "com.example.known"))
                        .Succeeded,
                    Is.True);

                SavePayloadEntry[] entries =
                    Entries(
                        integrity,
                        "com.example.known",
                        "com.example.unknown-a",
                        "com.example.unknown-b");

                SaveGenerationPublicationResult initial =
                    publication
                        .PublishInitialStoredTransportGeneration(
                            slotId,
                            "project",
                            "1",
                            "build",
                            "M5-05",
                            entries,
                            Inventory(
                                entries),
                            "transport");

                Assert.That(
                    initial.Succeeded,
                    Is.True,
                    initial.Message);

                Assert.That(
                    catalog.Refresh().Succeeded,
                    Is.True);

                SaveUnknownPayloadPruneCoordinator coordinator =
                    Coordinator(
                        catalog,
                        storage,
                        serializer,
                        integrity,
                        registry,
                        unknown,
                        publication,
                        retention);

                SaveStorageReadResult sourceBefore =
                    ReadPayload(
                        storage,
                        slotId,
                        initial.GenerationId);

                SaveUnknownPayloadPrunePlan plan =
                    coordinator.Prepare(
                        slotId,
                        new[]
                        {
                            new SaveParticipantId(
                                "com.example.unknown-a")
                        });

                Assert.That(
                    plan.Succeeded,
                    Is.True,
                    plan.Message);

                SaveStorageReadResult sourceAfterPreview =
                    ReadPayload(
                        storage,
                        slotId,
                        initial.GenerationId);

                CollectionAssert.AreEqual(
                    sourceBefore.Data,
                    sourceAfterPreview.Data,
                    "Prune Preview must perform zero durable writes.");

                SaveUnknownPayloadPruneResult result =
                    coordinator.Confirm(
                        plan);

                Assert.That(
                    result.Succeeded,
                    Is.True,
                    result.Message);

                Assert.That(
                    result.PublishedGenerationId,
                    Is.Not.EqualTo(
                        initial.GenerationId));

                SaveStorageReadResult sourceAfterConfirm =
                    ReadPayload(
                        storage,
                        slotId,
                        initial.GenerationId);

                CollectionAssert.AreEqual(
                    sourceBefore.Data,
                    sourceAfterConfirm.Data,
                    "The source generation must remain byte-immutable.");

                SavePayloadDocument current =
                    ReadPayloadDocument(
                        storage,
                        serializer,
                        slotId,
                        result.PublishedGenerationId);

                Assert.That(
                    Contains(
                        current,
                        "com.example.known"),
                    Is.True);

                Assert.That(
                    Contains(
                        current,
                        "com.example.unknown-b"),
                    Is.True);

                Assert.That(
                    Contains(
                        current,
                        "com.example.unknown-a"),
                    Is.False);

                SaveUnknownPayloadSnapshot snapshot =
                    unknown.GetSnapshot();

                Assert.That(
                    snapshot.Count,
                    Is.EqualTo(1));

                Assert.That(
                    snapshot.TryGet(
                        new SaveParticipantId(
                            "com.example.unknown-b"),
                        out SavePayloadEntry remaining),
                    Is.True);

                Assert.That(
                    remaining.serializedPayload,
                    Is.EqualTo(
                        PayloadFor(
                            "com.example.unknown-b")));
            }
            finally
            {
                storage.Shutdown();

                if (Directory.Exists(
                        root))
                {
                    Directory.Delete(
                        root,
                        true);
                }
            }
        }

        [Test]
        public void Confirm_RejectsRequestedIdThatBecomesClaimedAfterPreview()
        {
            using (PruneFixture fixture =
                   PruneFixture.Create())
            {
                SaveUnknownPayloadPrunePlan plan =
                    fixture.Coordinator.Prepare(
                        fixture.SlotId,
                        new[]
                        {
                            new SaveParticipantId(
                                "com.example.unknown-a")
                        });

                Assert.That(
                    plan.Succeeded,
                    Is.True);

                Assert.That(
                    fixture.Registry.Register(
                            new TestParticipant(
                                "com.example.unknown-a"))
                        .Succeeded,
                    Is.True);

                SaveUnknownPayloadPruneResult result =
                    fixture.Coordinator.Confirm(
                        plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveUnknownPayloadPruneStatus
                            .RequestedIdClaimed));

                Assert.That(
                    CurrentGeneration(
                        fixture.Catalog,
                        fixture.SlotId),
                    Is.EqualTo(
                        fixture.InitialGeneration));
            }
        }

        [Test]
        public void Confirm_RejectsPlanWhenCurrentGenerationChangesAfterPreview()
        {
            using (PruneFixture fixture =
                   PruneFixture.Create())
            {
                SaveUnknownPayloadPrunePlan plan =
                    fixture.Coordinator.Prepare(
                        fixture.SlotId,
                        new[]
                        {
                            new SaveParticipantId(
                                "com.example.unknown-a")
                        });

                Assert.That(
                    plan.Succeeded,
                    Is.True);

                SaveGenerationPublicationResult intervening =
                    fixture.Publication
                        .PublishStoredTransportGeneration(
                            fixture.SlotId,
                            "project",
                            "1",
                            "build",
                            "M5-05",
                            fixture.Entries,
                            Inventory(
                                fixture.Entries),
                            "transport",
                            fixture.InitialGeneration);

                Assert.That(
                    intervening.Succeeded,
                    Is.True,
                    intervening.Message);

                SaveUnknownPayloadPruneResult result =
                    fixture.Coordinator.Confirm(
                        plan);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveUnknownPayloadPruneStatus
                            .SourceStale));
            }
        }

        private static SaveUnknownPayloadPruneCoordinator Coordinator(
            SaveSlotCatalog catalog,
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity,
            SaveParticipantRegistry registry,
            SaveUnknownPayloadStore unknown,
            SaveGenerationPublicationCoordinator publication,
            SaveGenerationRetentionCoordinator retention) =>
            new SaveUnknownPayloadPruneCoordinator(
                catalog,
                storage,
                serializer,
                integrity,
                registry,
                unknown,
                publication,
                retention,
                SaveRetentionPolicy.Default,
                "m505-test-session",
                () => DateTimeOffset.UtcNow,
                TimeSpan.FromMinutes(5));

        private static SavePayloadEntry[] Entries(
            IIntegrityProvider integrity,
            params string[] ids)
        {
            Array.Sort(
                ids,
                StringComparer.Ordinal);

            SavePayloadEntry[] entries =
                new SavePayloadEntry[
                    ids.Length];

            for (int i = 0;
                 i < ids.Length;
                 i++)
            {
                string payload =
                    PayloadFor(
                        ids[i]);

                byte[] bytes =
                    Encoding.UTF8.GetBytes(
                        payload);

                Assert.That(
                    integrity.Calculate(
                            bytes,
                            out string checksum)
                        .Succeeded,
                    Is.True);

                entries[i] =
                    new SavePayloadEntry
                    {
                        participantId =
                            ids[i],
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
                    };
            }

            return entries;
        }

        private static SavePayloadInventoryEntry[] Inventory(
            SavePayloadEntry[] source)
        {
            SavePayloadInventoryEntry[] inventory =
                new SavePayloadInventoryEntry[
                    source.Length];

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SavePayloadEntry entry =
                    source[i];

                inventory[i] =
                    new SavePayloadInventoryEntry
                    {
                        participantId =
                            entry.participantId,
                        participantSchemaVersion =
                            entry.participantSchemaVersion,
                        serializerId =
                            entry.serializerId,
                        required =
                            entry.required,
                        byteLength =
                            entry.byteLength,
                        checksum =
                            entry.checksum,
                        flags =
                            entry.flags
                    };
            }

            return inventory;
        }

        private static SaveStorageReadResult ReadPayload(
            ISaveStorageBackend storage,
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            Assert.That(
                SaveGenerationStorageKeys.TryCreate(
                        slotId,
                        generationId,
                        out SaveGenerationStorageKeys keys)
                    .Succeeded,
                Is.True);

            return storage.Read(
                keys.GenerationPayload);
        }

        private static SavePayloadDocument ReadPayloadDocument(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveStorageReadResult read =
                ReadPayload(
                    storage,
                    slotId,
                    generationId);

            Assert.That(
                read.Succeeded,
                Is.True);

            Assert.That(
                serializer.Deserialize(
                        Encoding.UTF8.GetString(
                            read.Data),
                        out SavePayloadDocument document)
                    .Succeeded,
                Is.True);

            return document;
        }

        private static bool Contains(
            SavePayloadDocument document,
            string participantId)
        {
            SavePayloadEntry[] entries =
                document.entries ??
                Array.Empty<SavePayloadEntry>();

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                if (string.Equals(
                        entries[i].participantId,
                        participantId,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static SaveGenerationId CurrentGeneration(
            SaveSlotCatalog catalog,
            SaveSlotId slotId)
        {
            SaveSlotCatalogRefreshResult refresh =
                catalog.Refresh();

            Assert.That(
                refresh.Succeeded,
                Is.True);

            Assert.That(
                refresh.Snapshot.TryGetEntry(
                    slotId,
                    out SaveSlotCatalogEntry entry),
                Is.True);

            return entry.CurrentGenerationId;
        }

        private static string PayloadFor(
            string id) =>
            "{\"id\":\"" +
            id +
            "\"}";

        private static string TempRoot() =>
            Path.Combine(
                Path.GetTempPath(),
                "EchoSave-M505-Prune-" +
                Guid.NewGuid().ToString("N"));

        private sealed class TestParticipant :
            ISaveParticipant
        {
            internal TestParticipant(
                string id)
            {
                Descriptor =
                    new SaveParticipantDescriptor(
                        new SaveParticipantId(
                            id),
                        1,
                        SaveParticipantCriticality.Optional,
                        SaveMissingPayloadPolicy.Ignore,
                        default);
            }

            public SaveParticipantDescriptor Descriptor { get; }

            public SaveParticipantCaptureResult Capture() =>
                SaveParticipantCaptureResult.Failure(
                    "M5-05 prune tests do not invoke capture.");

            public SaveParticipantApplyResult Apply(
                object detachedState) =>
                SaveParticipantApplyResult.Failure(
                    "M5-05 prune tests do not invoke apply.");
        }

        private sealed class PruneFixture :
            IDisposable
        {
            private PruneFixture()
            {
            }

            internal string Root { get; private set; }

            internal LocalFileSaveStorageBackend Storage { get; private set; }

            internal UnityJsonSaveSerializer Serializer { get; private set; }

            internal Sha256IntegrityProvider Integrity { get; private set; }

            internal SaveParticipantRegistry Registry { get; private set; }

            internal SaveUnknownPayloadStore Unknown { get; private set; }

            internal SaveSlotCatalog Catalog { get; private set; }

            internal SaveGenerationPublicationCoordinator Publication
            {
                get;
                private set;
            }

            internal SaveGenerationRetentionCoordinator Retention
            {
                get;
                private set;
            }

            internal SaveUnknownPayloadPruneCoordinator Coordinator
            {
                get;
                private set;
            }

            internal SaveSlotId SlotId { get; private set; }

            internal SaveGenerationId InitialGeneration { get; private set; }

            internal SavePayloadEntry[] Entries { get; private set; }

            internal static PruneFixture Create()
            {
                PruneFixture fixture =
                    new PruneFixture();

                fixture.Root =
                    TempRoot();

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

                fixture.Registry =
                    new SaveParticipantRegistry();

                fixture.Unknown =
                    new SaveUnknownPayloadStore();

                fixture.Catalog =
                    new SaveSlotCatalog(
                        fixture.Storage,
                        fixture.Serializer,
                        64);

                fixture.Publication =
                    new SaveGenerationPublicationCoordinator(
                        fixture.Storage,
                        fixture.Serializer,
                        fixture.Integrity);

                fixture.Retention =
                    new SaveGenerationRetentionCoordinator(
                        fixture.Storage,
                        fixture.Serializer,
                        64);

                fixture.SlotId =
                    SaveSlotId.NewId();

                fixture.Entries =
                    Entries(
                        fixture.Integrity,
                        "com.example.unknown-a",
                        "com.example.unknown-b");

                SaveGenerationPublicationResult initial =
                    fixture.Publication
                        .PublishInitialStoredTransportGeneration(
                            fixture.SlotId,
                            "project",
                            "1",
                            "build",
                            "M5-05",
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

                Assert.That(
                    fixture.Catalog.Refresh().Succeeded,
                    Is.True);

                fixture.Coordinator =
                    Coordinator(
                        fixture.Catalog,
                        fixture.Storage,
                        fixture.Serializer,
                        fixture.Integrity,
                        fixture.Registry,
                        fixture.Unknown,
                        fixture.Publication,
                        fixture.Retention);

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
    }
}
