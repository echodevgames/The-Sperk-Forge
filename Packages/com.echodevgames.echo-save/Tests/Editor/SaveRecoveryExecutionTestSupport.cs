
using System;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SaveRecoveryExecutionTestBackend :
        ISaveStoragePublicationBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly ISaveStorageBackend storage;
        private readonly ISaveStoragePublicationBackend publication;
        private readonly ISaveStorageDiscoveryBackend discovery;

        private int failDiscoveryAfterHeadCount =
            int.MaxValue;

        internal SaveRecoveryExecutionTestBackend(
            ISaveStorageBackend storage)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            publication =
                storage as
                    ISaveStoragePublicationBackend ??
                throw new ArgumentException(
                    "Recovery execution tests require publication capability.",
                    nameof(storage));

            discovery =
                storage as
                    ISaveStorageDiscoveryBackend ??
                throw new ArgumentException(
                    "Recovery execution tests require discovery capability.",
                    nameof(storage));
        }

        internal int MutationCalls { get; private set; }

        internal int HeadPublicationCalls { get; private set; }

        internal bool FailNextHeadPublication { get; set; }

        internal void FailCatalogDiscoveryAfterNextHead()
        {
            failDiscoveryAfterHeadCount =
                HeadPublicationCalls + 1;
        }

        public SaveStorageBackendId Id =>
            storage.Id;

        public string RootPath =>
            storage.RootPath;

        public SaveStoragePublicationCapabilities
            PublicationCapabilities =>
            publication.PublicationCapabilities;

        public SaveStorageResult Initialize() =>
            storage.Initialize();

        public SaveStorageResult Exists(
            SaveStorageKey key,
            out bool exists) =>
            storage.Exists(
                key,
                out exists);

        public SaveStorageReadResult Read(
            SaveStorageKey key) =>
            storage.Read(
                key);

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            MutationCalls++;

            return storage.WriteNew(
                key,
                data);
        }

        public SaveStorageResult Delete(
            SaveStorageKey key)
        {
            MutationCalls++;

            return storage.Delete(
                key);
        }

        public SaveStorageResult PublishNewTree(
            SaveStorageKey sourceDirectoryKey,
            SaveStorageKey destinationDirectoryKey)
        {
            MutationCalls++;

            return publication.PublishNewTree(
                sourceDirectoryKey,
                destinationDirectoryKey);
        }

        public SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data)
        {
            HeadPublicationCalls++;

            if (FailNextHeadPublication &&
                key.Value.EndsWith(
                    "/head.json",
                    StringComparison.Ordinal))
            {
                FailNextHeadPublication =
                    false;

                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    "ESV-TEST-REC-HEAD",
                    "Injected recovery head-publication failure.");
            }

            MutationCalls++;

            return publication.PublishCurrentObject(
                key,
                data);
        }

        public SaveStorageDiscoveryResult
            DiscoverChildDirectories(
                SaveStorageKey parentKey,
                int maxChildren)
        {
            if (HeadPublicationCalls >=
                failDiscoveryAfterHeadCount)
            {
                return new SaveStorageDiscoveryResult(
                    SaveStorageDiscoveryStatus.Failed,
                    "ESV-TEST-REC-CATALOG",
                    "Injected post-recovery catalog discovery failure.",
                    Array.Empty<string>());
            }

            return discovery
                .DiscoverChildDirectories(
                    parentKey,
                    maxChildren);
        }

        public SaveStorageResult Shutdown() =>
            storage.Shutdown();
    }
}
