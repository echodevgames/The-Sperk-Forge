
using System;
using System.Collections.Generic;
using System.IO;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal enum SlotCreationFaultPoint
    {
        None = 0,
        CandidatePayloadWrite = 1,
        CandidateManifestWrite = 2,
        CandidatePayloadReadCorruption = 3,
        GenerationPublication = 4,
        PublishedPayloadReadCorruption = 5,
        HeadPublication = 6
    }

    internal sealed class SlotCreationTestStorageBackend :
        ISaveStoragePublicationBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly LocalFileSaveStorageBackend inner;
        private bool headPublished;

        internal SlotCreationTestStorageBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(
                    nameof(inner));
        }

        internal SlotCreationFaultPoint Fault { get; set; }

        internal bool FailDiscoveryAfterHeadPublication
        {
            get;
            set;
        }

        internal int MutationCount { get; private set; }

        internal bool HeadPublished =>
            headPublished;

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
            SaveStorageKey key)
        {
            SaveStorageReadResult result =
                inner.Read(
                    key);

            bool corruptCandidate =
                Fault ==
                    SlotCreationFaultPoint
                        .CandidatePayloadReadCorruption &&
                result.Succeeded &&
                key.Value.Contains(
                    "/incomplete/") &&
                key.Value.EndsWith(
                    "/payload.json",
                    StringComparison.Ordinal);

            bool corruptPublished =
                Fault ==
                    SlotCreationFaultPoint
                        .PublishedPayloadReadCorruption &&
                result.Succeeded &&
                key.Value.Contains(
                    "/generations/") &&
                key.Value.EndsWith(
                    "/payload.json",
                    StringComparison.Ordinal);

            if (!corruptCandidate &&
                !corruptPublished)
            {
                return result;
            }

            byte[] corrupted =
                result.Data;

            if (corrupted.Length > 0)
            {
                corrupted[
                    corrupted.Length - 1] =
                    (byte)'!';
            }

            return new SaveStorageReadResult(
                SaveStorageResult.Success(
                    "Fault-injected corrupted Chronicle payload read."),
                corrupted);
        }

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            if (Fault ==
                    SlotCreationFaultPoint.CandidatePayloadWrite &&
                key.Value.Contains(
                    "/incomplete/") &&
                key.Value.EndsWith(
                    "/payload.json",
                    StringComparison.Ordinal))
            {
                return InjectedFailure(
                    "Candidate payload write fault.");
            }

            if (Fault ==
                    SlotCreationFaultPoint.CandidateManifestWrite &&
                key.Value.Contains(
                    "/incomplete/") &&
                key.Value.EndsWith(
                    "/manifest.json",
                    StringComparison.Ordinal))
            {
                return InjectedFailure(
                    "Candidate manifest write fault.");
            }

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
            if (Fault ==
                SlotCreationFaultPoint.GenerationPublication)
            {
                return InjectedFailure(
                    "Generation publication fault.");
            }

            MutationCount++;

            return inner.PublishNewTree(
                sourceDirectoryKey,
                destinationDirectoryKey);
        }

        public SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data)
        {
            if (Fault ==
                SlotCreationFaultPoint.HeadPublication)
            {
                return InjectedFailure(
                    "Head publication fault.");
            }

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
                headPublished =
                    true;
            }

            return result;
        }

        public SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren)
        {
            if (FailDiscoveryAfterHeadPublication &&
                headPublished)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.Failed,
                    "ESV-TEST-DISCOVERY",
                    "Fault-injected post-publication catalog discovery failure.",
                    Array.Empty<string>());
            }

            return inner.DiscoverChildDirectories(
                parentKey,
                maxChildren);
        }

        public SaveStorageResult Shutdown() =>
            inner.Shutdown();

        private static SaveStorageResult InjectedFailure(
            string message) =>
            new SaveStorageResult(
                SaveStorageStatus.Failed,
                "ESV-TEST-FAULT",
                message);
    }

    internal sealed class SlotCreationTestEnvironment :
        IDisposable
    {
        private readonly string sandboxParent;

        internal SlotCreationTestEnvironment(
            int catalogScanLimit = 64)
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M4-02-" +
                    Guid.NewGuid()
                        .ToString("N"));

            string root =
                Path.Combine(
                    sandboxParent,
                    "Chronicle");

            Local =
                new LocalFileSaveStorageBackend(
                    root);

            if (!Local.Initialize()
                    .Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not initialize Chronicle test storage.");
            }

            Backend =
                new SlotCreationTestStorageBackend(
                    Local);

            Serializer =
                new UnityJsonSaveSerializer();

            Integrity =
                new Sha256IntegrityProvider();

            Catalog =
                new SaveSlotCatalog(
                    Backend,
                    Serializer,
                    catalogScanLimit);
        }

        internal LocalFileSaveStorageBackend Local { get; }

        internal SlotCreationTestStorageBackend Backend { get; }

        internal UnityJsonSaveSerializer Serializer { get; }

        internal Sha256IntegrityProvider Integrity { get; }

        internal SaveSlotCatalog Catalog { get; }

        internal SaveGenerationPublicationCoordinator
            CreatePublicationCoordinator(
                Func<SaveGenerationId> generationFactory = null) =>
            generationFactory == null
                ? new SaveGenerationPublicationCoordinator(
                    Backend,
                    Serializer,
                    Integrity)
                : new SaveGenerationPublicationCoordinator(
                    Backend,
                    Serializer,
                    Integrity,
                    SystemSaveClock.Instance,
                    generationFactory);

        internal SaveTechnicalSlotCreationCoordinator
            CreateSlotCoordinator(
                int capacity,
                int maxIdAttempts,
                Func<SaveSlotId> slotIdFactory) =>
            new SaveTechnicalSlotCreationCoordinator(
                Catalog,
                CreatePublicationCoordinator(),
                capacity,
                maxIdAttempts,
                slotIdFactory);

        internal static SaveTechnicalSlotCreateRequest Request(
            string displayName = "Manual Save",
            string projectId = "com.example.game",
            string projectVersion = "1.0.0",
            string buildId = "build-a") =>
            new SaveTechnicalSlotCreateRequest(
                displayName,
                projectId,
                projectVersion,
                buildId);

        internal void CreateRawSlotDirectory(
            string childName)
        {
            Directory.CreateDirectory(
                Path.Combine(
                    Local.RootPath,
                    "slots",
                    childName));
        }

        internal SaveStorageReadResult ReadHead(
            SaveSlotId slotId)
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                slotId.Value +
                "/head.json",
                out SaveStorageKey key);

            return Local.Read(
                key);
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

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected Chronicle manifest was not readable.");
            }

            SaveSerializerResult result =
                Serializer.Deserialize(
                    System.Text.Encoding.UTF8.GetString(
                        read.Data),
                    out SaveManifest manifest);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected Chronicle manifest was not deserializable.");
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

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected Chronicle payload was not readable.");
            }

            SaveSerializerResult result =
                Serializer.Deserialize(
                    System.Text.Encoding.UTF8.GetString(
                        read.Data),
                    out SavePayloadDocument payload);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Expected Chronicle payload was not deserializable.");
            }

            return payload;
        }

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
    }
}
