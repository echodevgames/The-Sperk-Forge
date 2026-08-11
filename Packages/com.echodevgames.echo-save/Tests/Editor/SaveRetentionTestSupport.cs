
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SaveRetentionTestEnvironment :
        IDisposable
    {
        private readonly string sandboxParent;

        internal SaveRetentionTestEnvironment()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M4-06-" +
                    Guid.NewGuid().ToString("N"));

            Local =
                new LocalFileSaveStorageBackend(
                    Path.Combine(
                        sandboxParent,
                        "Chronicle"));

            if (!Local.Initialize().Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not initialize M4-06 test storage.");
            }

            Serializer =
                new UnityJsonSaveSerializer();

            SlotId =
                SaveSlotId.NewId();
        }

        internal LocalFileSaveStorageBackend Local { get; }

        internal UnityJsonSaveSerializer Serializer { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId Generation(
            int ordinal,
            SaveGenerationCommitState commitState =
                SaveGenerationCommitState.Committed,
            SaveSlotId? manifestSlotOverride = null,
            string rawManifestOverride = null)
        {
            DateTime utc =
                new DateTime(
                    2026,
                    8,
                    10,
                    12,
                    ordinal,
                    0,
                    DateTimeKind.Utc);

            SaveGenerationId generation =
                SaveGenerationId.CreateForTesting(
                    utc,
                    ordinal + 1,
                    GuidFromOrdinal(
                        ordinal + 1));

            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            string serialized;

            if (rawManifestOverride != null)
            {
                serialized =
                    rawManifestOverride;
            }
            else
            {
                SaveManifest manifest =
                    new SaveManifest
                    {
                        slotId =
                            (manifestSlotOverride ??
                             SlotId).Value,
                        generationId =
                            generation.Value,
                        createdUtc =
                            utc.ToString(
                                "O",
                                CultureInfo.InvariantCulture),
                        updatedUtc =
                            utc.ToString(
                                "O",
                                CultureInfo.InvariantCulture),
                        saveKind =
                            "participant",
                        projectId =
                            "com.example.retention",
                        projectVersion =
                            "1.0.0",
                        buildId =
                            "retention",
                        displayName =
                            "Retention",
                        commitState =
                            commitState
                    };

                SaveSerializerResult result =
                    Serializer.Serialize(
                        manifest,
                        out serialized);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        result.Message);
                }
            }

            SaveStorageResult write =
                Local.WriteNew(
                    keys.GenerationManifest,
                    Encoding.UTF8.GetBytes(
                        serialized));

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.Message);
            }

            return generation;
        }

        internal void WriteHead(
            SaveGenerationId current,
            SaveGenerationId previous = default)
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId =
                        SlotId.Value,
                    currentGenerationId =
                        current.Value,
                    previousGenerationId =
                        previous.Value ??
                        string.Empty,
                    updateSequence =
                        1
                };

            SaveSerializerResult serialized =
                Serializer.Serialize(
                    head,
                    out string json);

            if (!serialized.Succeeded)
            {
                throw new InvalidOperationException(
                    serialized.Message);
            }

            SaveStorageKey.TryCreate(
                "slots/" +
                SlotId.Value +
                "/head.json",
                out SaveStorageKey key);

            SaveStorageResult write =
                Local.WriteNew(
                    key,
                    Encoding.UTF8.GetBytes(
                        json));

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.Message);
            }
        }

        internal bool GenerationExists(
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            return Local.Read(
                    keys.GenerationManifest)
                .Succeeded;
        }

        internal void CreateNonCanonicalGenerationChild(
            string name)
        {
            Directory.CreateDirectory(
                Path.Combine(
                    Local.RootPath,
                    "slots",
                    SlotId.Value,
                    "generations",
                    name));
        }

        internal SaveGenerationRetentionCoordinator Coordinator(
            ISaveStorageBackend backend = null,
            int discoveryLimit =
                SaveGenerationRetentionCoordinator
                    .DefaultDiscoveryLimit) =>
            new SaveGenerationRetentionCoordinator(
                backend ?? Local,
                Serializer,
                discoveryLimit);

        public void Dispose()
        {
            Local.Shutdown();

            if (Directory.Exists(
                    sandboxParent))
            {
                Directory.Delete(
                    sandboxParent,
                    true);
            }
        }

        private static Guid GuidFromOrdinal(
            int ordinal)
        {
            byte[] bytes =
                new byte[16];

            bytes[15] =
                (byte)ordinal;

            return new Guid(
                bytes);
        }
    }

    internal sealed class DiscoveryOnlyRetentionBackend :
        ISaveStorageBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly LocalFileSaveStorageBackend inner;

        internal DiscoveryOnlyRetentionBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner;
        }

        public SaveStorageBackendId Id => inner.Id;
        public string RootPath => inner.RootPath;
        public SaveStorageResult Initialize() => inner.Initialize();
        public SaveStorageResult Exists(SaveStorageKey key, out bool exists) =>
            inner.Exists(key, out exists);
        public SaveStorageReadResult Read(SaveStorageKey key) => inner.Read(key);
        public SaveStorageResult WriteNew(SaveStorageKey key, byte[] data) =>
            inner.WriteNew(key, data);
        public SaveStorageResult Delete(SaveStorageKey key) => inner.Delete(key);
        public SaveStorageResult Shutdown() => inner.Shutdown();
        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren) =>
            inner.DiscoverChildDirectories(
                parentKey,
                maxChildren);
    }

    internal sealed class FailingTreeDeletionRetentionBackend :
        ISaveStorageBackend,
        ISaveStorageDiscoveryBackend,
        ISaveStorageTreeDeletionBackend
    {
        private readonly LocalFileSaveStorageBackend inner;

        internal FailingTreeDeletionRetentionBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner;
        }

        internal int DeleteTreeCalls { get; private set; }

        public SaveStorageBackendId Id => inner.Id;
        public string RootPath => inner.RootPath;
        public SaveStorageResult Initialize() => inner.Initialize();
        public SaveStorageResult Exists(SaveStorageKey key, out bool exists) =>
            inner.Exists(key, out exists);
        public SaveStorageReadResult Read(SaveStorageKey key) => inner.Read(key);
        public SaveStorageResult WriteNew(SaveStorageKey key, byte[] data) =>
            inner.WriteNew(key, data);
        public SaveStorageResult Delete(SaveStorageKey key) => inner.Delete(key);
        public SaveStorageResult Shutdown() => inner.Shutdown();
        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren) =>
            inner.DiscoverChildDirectories(
                parentKey,
                maxChildren);

        public SaveStorageResult DeleteTree(
            SaveStorageKey directoryKey)
        {
            DeleteTreeCalls++;

            return new SaveStorageResult(
                SaveStorageStatus.Failed,
                "ESV-TEST-RETENTION-DELETE",
                "Injected retention tree-delete failure.");
        }
    }
}
