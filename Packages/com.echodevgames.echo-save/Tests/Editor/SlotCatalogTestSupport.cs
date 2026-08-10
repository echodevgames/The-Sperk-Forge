
using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SlotCatalogFakeStorageBackend :
        ISaveStorageBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly Dictionary<string, byte[]> objects =
            new Dictionary<string, byte[]>(
                StringComparer.Ordinal);

        private string[] childNames =
            Array.Empty<string>();

        internal bool DiscoveryFails { get; set; }

        internal bool DiscoveryLimitExceeded { get; set; }

        internal string FailReadKey { get; set; }

        internal int WriteCount { get; private set; }

        internal List<string> ReadKeys { get; } =
            new List<string>();

        public SaveStorageBackendId Id =>
            new SaveStorageBackendId(
                "tests.slot-catalog");

        public string RootPath =>
            "memory://slot-catalog";

        public SaveStorageResult Initialize() =>
            SaveStorageResult.Success(
                "ready");

        public SaveStorageResult Exists(
            SaveStorageKey key,
            out bool exists)
        {
            exists =
                objects.ContainsKey(
                    key.Value);

            return SaveStorageResult.Success(
                "checked");
        }

        public SaveStorageReadResult Read(
            SaveStorageKey key)
        {
            ReadKeys.Add(
                key.Value);

            if (string.Equals(
                    FailReadKey,
                    key.Value,
                    StringComparison.Ordinal))
            {
                return new SaveStorageReadResult(
                    new SaveStorageResult(
                        SaveStorageStatus.Failed,
                        "TEST-READ",
                        "simulated read failure"),
                    null);
            }

            if (!objects.TryGetValue(
                    key.Value,
                    out byte[] data))
            {
                return new SaveStorageReadResult(
                    new SaveStorageResult(
                        SaveStorageStatus.NotFound,
                        EchoSaveDiagnosticCodes.StorageNotFound,
                        "missing"),
                    null);
            }

            return new SaveStorageReadResult(
                SaveStorageResult.Success(
                    "read"),
                data);
        }

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            WriteCount++;

            objects[key.Value] =
                data == null
                    ? Array.Empty<byte>()
                    : (byte[])data.Clone();

            return SaveStorageResult.Success(
                "written");
        }

        public SaveStorageResult Delete(
            SaveStorageKey key)
        {
            objects.Remove(
                key.Value);

            return SaveStorageResult.Success(
                "deleted");
        }

        public SaveStorageResult Shutdown() =>
            SaveStorageResult.Success(
                "stopped");

        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren)
        {
            if (DiscoveryLimitExceeded)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.LimitExceeded,
                    EchoSaveDiagnosticCodes.StorageDiscoveryLimitExceeded,
                    "simulated limit",
                    Array.Empty<string>());
            }

            if (DiscoveryFails)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.Failed,
                    EchoSaveDiagnosticCodes.StorageDiscoveryFailed,
                    "simulated discovery failure",
                    Array.Empty<string>());
            }

            if (childNames == null)
            {
                return SaveStorageDiscoveryResult.ParentNotFound(
                    "missing");
            }

            if (maxChildren <= 0)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes.StorageDiscoveryInvalidRequest,
                    "invalid",
                    Array.Empty<string>());
            }

            if (childNames.Length >
                maxChildren)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.LimitExceeded,
                    EchoSaveDiagnosticCodes.StorageDiscoveryLimitExceeded,
                    "limit",
                    Array.Empty<string>());
            }

            return SaveStorageDiscoveryResult.Success(
                childNames,
                "discovered");
        }

        internal void SetChildren(
            params string[] children)
        {
            childNames =
                children == null
                    ? null
                    : (string[])children.Clone();
        }

        internal void Put(
            string key,
            string text)
        {
            objects[key] =
                Encoding.UTF8.GetBytes(
                    text ?? string.Empty);
        }

        internal void Remove(
            string key)
        {
            objects.Remove(
                key);
        }
    }

    internal static class SlotCatalogTestSupport
    {
        private static readonly UnityJsonSaveSerializer Serializer =
            new UnityJsonSaveSerializer();

        internal static SaveSlotId Slot(
            int value)
        {
            string text =
                $"aaaaaaaa-bbbb-cccc-dddd-{value.ToString("000000000000")}";

            if (!SaveSlotId.TryParse(
                    text,
                    out SaveSlotId slotId))
            {
                throw new InvalidOperationException(
                    "Test slot ID is invalid.");
            }

            return slotId;
        }

        internal static SaveGenerationId Generation(
            int value)
        {
            _ = value;

            return SaveGenerationId.NewId();
        }

        internal static string HeadKey(
            SaveSlotId slotId) =>
            "slots/" +
            slotId.Value +
            "/head.json";

        internal static string ManifestKey(
            SaveSlotId slotId,
            SaveGenerationId generationId) =>
            "slots/" +
            slotId.Value +
            "/generations/" +
            generationId.Value +
            "/manifest.json";

        internal static string PayloadKey(
            SaveSlotId slotId,
            SaveGenerationId generationId) =>
            "slots/" +
            slotId.Value +
            "/generations/" +
            generationId.Value +
            "/payload.json";

        internal static void PutHealthy(
            SlotCatalogFakeStorageBackend backend,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            string displayName = "Save",
            string updatedUtc = "2026-08-10T10:00:00.0000000+00:00")
        {
            SaveHeadPointer head =
                new SaveHeadPointer
                {
                    slotId =
                        slotId.Value,
                    currentGenerationId =
                        generationId.Value,
                    previousGenerationId =
                        string.Empty,
                    updateSequence =
                        1
                };

            SaveManifest manifest =
                new SaveManifest
                {
                    slotId =
                        slotId.Value,
                    generationId =
                        generationId.Value,
                    createdUtc =
                        "2026-08-10T09:00:00.0000000+00:00",
                    updatedUtc =
                        updatedUtc,
                    saveKind =
                        "manual",
                    projectId =
                        "test-project",
                    projectVersion =
                        "1.0.0",
                    buildId =
                        "build-1",
                    displayName =
                        displayName,
                    payloadByteLength =
                        123,
                    payloadEntries =
                        new[]
                        {
                            new SavePayloadInventoryEntry
                            {
                                participantId =
                                    "com.example.inventory",
                                participantSchemaVersion =
                                    1,
                                serializerId =
                                    UnityJsonSaveSerializer.StableId,
                                required =
                                    true,
                                byteLength =
                                    10,
                                checksum =
                                    new string(
                                        'a',
                                        64),
                                flags =
                                    0
                            }
                        },
                    commitState =
                        SaveGenerationCommitState.Committed
                };

            Serializer.Serialize(
                head,
                out string headJson);

            Serializer.Serialize(
                manifest,
                out string manifestJson);

            backend.Put(
                HeadKey(
                    slotId),
                headJson);

            backend.Put(
                ManifestKey(
                    slotId,
                    generationId),
                manifestJson);

            backend.Put(
                PayloadKey(
                    slotId,
                    generationId),
                "{\"mustNotBeRead\":true}");
        }

        internal static SaveSlotCatalog CreateCatalog(
            SlotCatalogFakeStorageBackend backend,
            int maxScanEntries = 32) =>
            new SaveSlotCatalog(
                backend,
                new UnityJsonSaveSerializer(),
                maxScanEntries);
    }
}
