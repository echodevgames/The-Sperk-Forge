
using System;
using System.IO;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SlotDeletionTestBackend :
        ISaveStoragePublicationBackend,
        ISaveStorageDiscoveryBackend,
        ISaveStorageTreeDeletionBackend
    {
        private readonly LocalFileSaveStorageBackend inner;
        private bool failNextTrashMove;
        private bool failCatalogAfterTrashMove;
        private bool failNextTrashRetentionDelete;

        internal SlotDeletionTestBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(nameof(inner));
        }

        internal int MutationCount { get; private set; }
        internal int TrashMoveCount { get; private set; }
        internal int TrashRetentionDeleteCount { get; private set; }

        internal void FailNextTrashMove() =>
            failNextTrashMove = true;

        internal void FailLiveCatalogAfterTrashMove() =>
            failCatalogAfterTrashMove = true;

        internal void FailNextTrashRetentionDelete() =>
            failNextTrashRetentionDelete = true;

        public SaveStorageBackendId Id => inner.Id;
        public string RootPath => inner.RootPath;

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
            inner.Read(key);

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            MutationCount++;

            return inner.WriteNew(
                key,
                data);
        }

        public SaveStorageResult Delete(
            SaveStorageKey key)
        {
            MutationCount++;

            return inner.Delete(
                key);
        }

        public SaveStorageResult PublishNewTree(
            SaveStorageKey sourceDirectoryKey,
            SaveStorageKey destinationDirectoryKey)
        {
            MutationCount++;

            bool trashMove =
                sourceDirectoryKey.Value.StartsWith(
                    "slots/",
                    StringComparison.Ordinal) &&
                destinationDirectoryKey.Value.StartsWith(
                    "trash/",
                    StringComparison.Ordinal);

            if (trashMove &&
                failNextTrashMove)
            {
                failNextTrashMove = false;

                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-M410-MOVE",
                    "Injected M4-10 trash move failure.");
            }

            SaveStorageResult result =
                inner.PublishNewTree(
                    sourceDirectoryKey,
                    destinationDirectoryKey);

            if (trashMove &&
                result.Succeeded)
            {
                TrashMoveCount++;
            }

            return result;
        }

        public SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data)
        {
            MutationCount++;

            return inner.PublishCurrentObject(
                key,
                data);
        }

        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren)
        {
            if (failCatalogAfterTrashMove &&
                TrashMoveCount > 0 &&
                string.Equals(
                    parentKey.Value,
                    "slots",
                    StringComparison.Ordinal))
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.Failed,
                    "ESV-TEST-M410-CATALOG",
                    "Injected M4-10 post-delete live-catalog discovery failure.",
                    Array.Empty<string>());
            }

            return inner.DiscoverChildDirectories(
                parentKey,
                maxChildren);
        }

        public SaveStorageResult DeleteTree(
            SaveStorageKey directoryKey)
        {
            MutationCount++;

            bool trashDelete =
                directoryKey.Value.StartsWith(
                    "trash/",
                    StringComparison.Ordinal);

            if (trashDelete &&
                failNextTrashRetentionDelete)
            {
                failNextTrashRetentionDelete = false;

                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-M410-RETENTION",
                    "Injected M4-10 trash retention failure.");
            }

            SaveStorageResult result =
                inner.DeleteTree(
                    directoryKey);

            if (trashDelete &&
                result.Succeeded)
            {
                TrashRetentionDeleteCount++;
            }

            return result;
        }

        public SaveStorageResult Shutdown() =>
            inner.Shutdown();
    }

    internal sealed class SaveSlotDeletionTestEnvironment :
        IDisposable
    {
        private readonly string sandboxParent;

        internal SaveSlotDeletionTestEnvironment(
            DateTimeOffset? startUtc = null)
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M4-10-" +
                    Guid.NewGuid().ToString("N"));

            string root =
                Path.Combine(
                    sandboxParent,
                    "Chronicle");

            Local =
                new LocalFileSaveStorageBackend(
                    root);

            if (!Local.Initialize().Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not initialize M4-10 local storage.");
            }

            Backend =
                new SlotDeletionTestBackend(
                    Local);

            Serializer =
                new UnityJsonSaveSerializer();

            Integrity =
                new Sha256IntegrityProvider();

            Catalog =
                new SaveSlotCatalog(
                    Backend,
                    Serializer,
                    64);

            Publication =
                new SaveGenerationPublicationCoordinator(
                    Backend,
                    Serializer,
                    Integrity);

            Clock =
                startUtc ??
                new DateTimeOffset(
                    2026,
                    8,
                    11,
                    1,
                    0,
                    0,
                    TimeSpan.Zero);

            SessionId =
                Guid.NewGuid().ToString("N");
        }

        internal LocalFileSaveStorageBackend Local { get; }
        internal SlotDeletionTestBackend Backend { get; }
        internal UnityJsonSaveSerializer Serializer { get; }
        internal Sha256IntegrityProvider Integrity { get; }
        internal SaveSlotCatalog Catalog { get; }
        internal SaveGenerationPublicationCoordinator Publication { get; }
        internal DateTimeOffset Clock { get; set; }
        internal string SessionId { get; }

        internal CreatedSource CreateSource(
            string displayName = "Delete Me")
        {
            SaveSlotId slotId =
                SaveSlotId.NewId();

            const string serialized =
                "{\"score\":10,\"zone\":\"delete\"}";

            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    serialized);

            SaveIntegrityResult integrity =
                Integrity.Calculate(
                    bytes,
                    out string checksum);

            if (!integrity.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not calculate M4-10 test checksum.");
            }

            SavePayloadEntry payload =
                new SavePayloadEntry
                {
                    participantId = "delete.example",
                    participantSchemaVersion = 1,
                    serializerId = UnityJsonSaveSerializer.StableId,
                    required = true,
                    serializedPayload = serialized,
                    byteProviderReference = string.Empty,
                    byteLength = bytes.LongLength,
                    checksum = checksum,
                    flags = 0
                };

            SavePayloadInventoryEntry inventory =
                new SavePayloadInventoryEntry
                {
                    participantId = payload.participantId,
                    participantSchemaVersion = payload.participantSchemaVersion,
                    serializerId = payload.serializerId,
                    required = payload.required,
                    byteLength = payload.byteLength,
                    checksum = payload.checksum,
                    flags = payload.flags
                };

            SaveGenerationPublicationResult publication =
                Publication.PublishInitialStoredTransportGeneration(
                    slotId,
                    "com.example.m410",
                    "1.0.0",
                    "m410-source",
                    displayName,
                    new[] { payload },
                    new[] { inventory },
                    "participant");

            if (!publication.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not create M4-10 source slot. " +
                    publication.DiagnosticCode +
                    " " +
                    publication.Message);
            }

            SaveSlotCatalogRefreshResult refresh =
                Catalog.Refresh();

            if (!refresh.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not refresh M4-10 source catalog.");
            }

            return new CreatedSource(
                slotId,
                publication.GenerationId);
        }

        internal SaveSlotDeletionCoordinator Coordinator(
            string sessionId = null,
            TimeSpan? planLifetime = null,
            int maxTrashRecords = 4,
            ISaveDeletionSourceReader sourceReader = null,
            Func<string> trashTokenFactory = null) =>
            new SaveSlotDeletionCoordinator(
                Catalog,
                Backend,
                Serializer,
                Integrity,
                sessionId ?? SessionId,
                () => Clock,
                planLifetime ?? TimeSpan.FromMinutes(5),
                64,
                64,
                maxTrashRecords,
                4,
                trashTokenFactory,
                sourceReader);

        internal bool LiveSlotDirectoryExists(
            SaveSlotId slotId) =>
            Directory.Exists(
                Path.Combine(
                    Local.RootPath,
                    "slots",
                    slotId.Value));

        internal int TrashRecordCount()
        {
            string path =
                Path.Combine(
                    Local.RootPath,
                    "trash");

            return Directory.Exists(path)
                ? Directory.GetDirectories(path).Length
                : 0;
        }

        internal bool TrashRecordExists(
            string trashRecordId) =>
            Directory.Exists(
                Path.Combine(
                    Local.RootPath,
                    "trash",
                    trashRecordId,
                    "slot"));

        public void Dispose()
        {
            Backend.Shutdown();

            if (Directory.Exists(
                    sandboxParent))
            {
                Directory.Delete(
                    sandboxParent,
                    true);
            }
        }

        internal readonly struct CreatedSource
        {
            internal CreatedSource(
                SaveSlotId slotId,
                SaveGenerationId generationId)
            {
                SlotId = slotId;
                GenerationId = generationId;
            }

            internal SaveSlotId SlotId { get; }
            internal SaveGenerationId GenerationId { get; }
        }
    }

    internal sealed class StaleDeletionSourceReader :
        ISaveDeletionSourceReader
    {
        private readonly ISaveDeletionSourceReader inner;

        internal StaleDeletionSourceReader(
            ISaveDeletionSourceReader inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(nameof(inner));
        }

        public SaveDeletionSourceReadResult Read(
            SaveSlotId slotId) =>
            inner.Read(slotId);

        public SaveDeletionSourceReadResult Revalidate(
            SaveDeletionSourceSnapshot snapshot) =>
            SaveDeletionSourceReadResult.Failure(
                SaveDeletionSourceStatus.SourceStale,
                EchoSaveDiagnosticCodes.DeleteConfirmSourceStale,
                "Injected stale M4-10 deletion source.");
    }
}
