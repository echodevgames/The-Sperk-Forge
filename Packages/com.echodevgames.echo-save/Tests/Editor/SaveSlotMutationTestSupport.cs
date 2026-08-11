
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SlotMutationTestBackend :
        ISaveStoragePublicationBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly LocalFileSaveStorageBackend inner;
        private int failCatalogDiscoveryAtHeadCount =
            int.MaxValue;

        internal SlotMutationTestBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(nameof(inner));
        }

        internal int MutationCount { get; private set; }
        internal int HeadPublicationCount { get; private set; }

        internal void FailCatalogDiscoveryAfterNextHead()
        {
            failCatalogDiscoveryAtHeadCount =
                HeadPublicationCount + 1;
        }

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
            return inner.Delete(key);
        }

        public SaveStorageResult PublishNewTree(
            SaveStorageKey sourceDirectoryKey,
            SaveStorageKey destinationDirectoryKey)
        {
            MutationCount++;
            return inner.PublishNewTree(
                sourceDirectoryKey,
                destinationDirectoryKey);
        }

        public SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data)
        {
            MutationCount++;

            SaveStorageResult result =
                inner.PublishCurrentObject(
                    key,
                    data);

            if (result.Succeeded &&
                key.Value.EndsWith(
                    "/head.json",
                    StringComparison.Ordinal))
            {
                HeadPublicationCount++;
            }

            return result;
        }

        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren)
        {
            if (string.Equals(
                    parentKey.Value,
                    "slots",
                    StringComparison.Ordinal) &&
                HeadPublicationCount >=
                    failCatalogDiscoveryAtHeadCount)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.Failed,
                    "ESV-TEST-M409-CATALOG",
                    "Injected post-publication catalog discovery failure.",
                    Array.Empty<string>());
            }

            return inner.DiscoverChildDirectories(
                parentKey,
                maxChildren);
        }

        public SaveStorageResult Shutdown() =>
            inner.Shutdown();
    }

    internal sealed class SaveSlotMutationTestEnvironment :
        IDisposable
    {
        private readonly string sandboxParent;

        internal SaveSlotMutationTestEnvironment()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M4-09-" +
                    Guid.NewGuid().ToString("N"));

            string root =
                Path.Combine(
                    sandboxParent,
                    "Chronicle");

            Local =
                new LocalFileSaveStorageBackend(root);

            if (!Local.Initialize().Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not initialize the M4-09 Chronicle test storage.");
            }

            Backend =
                new SlotMutationTestBackend(Local);

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

            SourceReader =
                new SaveSlotMutationSourceReader(
                    Backend,
                    Serializer,
                    Integrity);

            Retention =
                new SaveGenerationRetentionCoordinator(
                    Local,
                    Serializer,
                    64);
        }

        internal LocalFileSaveStorageBackend Local { get; }
        internal SlotMutationTestBackend Backend { get; }
        internal UnityJsonSaveSerializer Serializer { get; }
        internal Sha256IntegrityProvider Integrity { get; }
        internal SaveSlotCatalog Catalog { get; }
        internal SaveGenerationPublicationCoordinator Publication { get; }
        internal SaveSlotMutationSourceReader SourceReader { get; }
        internal SaveGenerationRetentionCoordinator Retention { get; }

        internal CreatedSource CreateSource(
            string displayName = "Original Slot")
        {
            SaveSlotId slotId =
                SaveSlotId.NewId();

            CreateStoredEntries(
                out SavePayloadEntry[] payloadEntries,
                out SavePayloadInventoryEntry[] inventoryEntries);

            SaveGenerationPublicationResult publication =
                Publication.PublishInitialStoredTransportGeneration(
                    slotId,
                    "com.example.m409",
                    "1.0.0",
                    "m409-source",
                    displayName,
                    payloadEntries,
                    inventoryEntries,
                    "participant");

            if (!publication.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not publish M4-09 source slot. " +
                    publication.DiagnosticCode +
                    " " +
                    publication.Message);
            }

            SaveSlotCatalogRefreshResult refresh =
                Catalog.Refresh();

            if (!refresh.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not refresh M4-09 source catalog.");
            }

            return new CreatedSource(
                slotId,
                publication.GenerationId);
        }

        internal SaveSlotMutationCoordinator Coordinator(
            int capacity = 8,
            int maxIdAttempts = 4,
            Func<SaveSlotId> slotIdFactory = null,
            ISaveSlotMutationSourceReader sourceReader = null) =>
            new SaveSlotMutationCoordinator(
                Catalog,
                sourceReader ?? SourceReader,
                Publication,
                Retention,
                SaveRetentionPolicy.Default,
                capacity,
                maxIdAttempts,
                slotIdFactory ?? SaveSlotId.NewId);

        internal SaveHeadPointer ReadHead(
            SaveSlotId slotId)
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                slotId.Value +
                "/head.json",
                out SaveStorageKey key);

            SaveStorageReadResult read =
                Local.Read(key);

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 head was unreadable.");
            }

            SaveSerializerResult parsed =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(read.Data),
                    out SaveHeadPointer head);

            if (!parsed.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 head was not deserializable.");
            }

            return head;
        }

        internal SaveManifest ReadManifest(
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationManifest);

            SaveSerializerResult parsed =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(read.Data),
                    out SaveManifest manifest);

            if (!read.Succeeded ||
                !parsed.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 manifest was unavailable.");
            }

            return manifest;
        }

        internal SavePayloadDocument ReadPayload(
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationPayload);

            SaveSerializerResult parsed =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(read.Data),
                    out SavePayloadDocument payload);

            if (!read.Succeeded ||
                !parsed.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 payload was unavailable.");
            }

            return payload;
        }

        internal byte[] ReadRawPayload(
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationPayload);

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 raw payload was unavailable.");
            }

            return read.Data;
        }

        internal byte[] ReadRawManifest(
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationManifest);

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected M4-09 raw manifest was unavailable.");
            }

            return read.Data;
        }

        internal int CountCommittedGenerations(
            SaveSlotId slotId)
        {
            string path =
                Path.Combine(
                    Local.RootPath,
                    "slots",
                    slotId.Value,
                    "generations");

            return Directory.Exists(path)
                ? Directory.GetDirectories(path).Length
                : 0;
        }

        internal void CorruptCurrentPayload(
            SaveSlotId slotId)
        {
            SaveHeadPointer head =
                ReadHead(slotId);

            if (!SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId generationId))
            {
                throw new InvalidOperationException(
                    "Expected a canonical current generation.");
            }

            SaveGenerationStorageKeys.TryCreate(
                slotId,
                generationId,
                out SaveGenerationStorageKeys keys);

            string path =
                Path.Combine(
                    Local.RootPath,
                    keys.GenerationPayload.Value.Replace(
                        '/',
                        Path.DirectorySeparatorChar));

            File.AppendAllText(
                path,
                "!");
        }

        private void CreateStoredEntries(
            out SavePayloadEntry[] payloadEntries,
            out SavePayloadInventoryEntry[] inventoryEntries)
        {
            const string serialized =
                "{\"score\":7,\"zone\":\"m409\"}";

            byte[] bytes =
                Encoding.UTF8.GetBytes(serialized);

            SaveIntegrityResult checksumResult =
                Integrity.Calculate(
                    bytes,
                    out string checksum);

            if (!checksumResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not calculate M4-09 participant checksum.");
            }

            SavePayloadEntry payload =
                new SavePayloadEntry
                {
                    participantId = "example.state",
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

            payloadEntries =
                new[]
                {
                    payload
                };

            inventoryEntries =
                new[]
                {
                    inventory
                };
        }

        public void Dispose()
        {
            Backend.Shutdown();

            if (Directory.Exists(sandboxParent))
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

    internal sealed class StaleSlotMutationSourceReader :
        ISaveSlotMutationSourceReader
    {
        private readonly ISaveSlotMutationSourceReader inner;

        internal StaleSlotMutationSourceReader(
            ISaveSlotMutationSourceReader inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(nameof(inner));
        }

        public SaveSlotMutationSourceReadResult Read(
            SaveSlotId slotId) =>
            inner.Read(slotId);

        public SaveSlotMutationSourceReadResult Revalidate(
            SaveSlotMutationSourceSnapshot snapshot) =>
            SaveSlotMutationSourceReadResult.Failure(
                SaveSlotMutationSourceStatus.SourceStale,
                EchoSaveDiagnosticCodes.SlotDuplicateSourceStale,
                "Injected stale M4-09 source revalidation.");
    }
}
