
using System;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class SaveRecoveryTestEnvironment :
        IDisposable
    {
        private readonly string sandboxParent;

        internal SaveRecoveryTestEnvironment()
        {
            sandboxParent =
                System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "EchoSave-M4-07-" +
                    Guid.NewGuid().ToString("N"));

            Local =
                new LocalFileSaveStorageBackend(
                    System.IO.Path.Combine(
                        sandboxParent,
                        "Chronicle"));

            SaveStorageResult initialized =
                Local.Initialize();

            if (!initialized.Succeeded)
            {
                throw new InvalidOperationException(
                    initialized.ToString());
            }

            Serializer =
                new UnityJsonSaveSerializer();

            Integrity =
                new Sha256IntegrityProvider();

            SlotId =
                SaveSlotId.NewId();

            ReadOnlyBackend =
                new RecoveryReadOnlyCountingBackend(
                    Local);
        }

        internal LocalFileSaveStorageBackend Local { get; }

        internal RecoveryReadOnlyCountingBackend ReadOnlyBackend { get; }

        internal UnityJsonSaveSerializer Serializer { get; }

        internal Sha256IntegrityProvider Integrity { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId PublishGeneration(
            DateTime utc,
            long sequence)
        {
            SaveGenerationId generation =
                SaveGenerationId.CreateForTesting(
                    utc,
                    sequence,
                    GuidFromOrdinal(
                        (int)sequence));

            SaveGenerationPublicationCoordinator publication =
                new SaveGenerationPublicationCoordinator(
                    Local,
                    Serializer,
                    Integrity,
                    new FixedRecoveryClock(
                        utc),
                    () => generation);

            SaveGenerationPublicationResult result =
                publication.PublishEmptyTransportGeneration(
                    SlotId,
                    "com.example.recovery",
                    "1.0.0",
                    "recovery-" +
                    sequence,
                    "Recovery Test");

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not publish recovery fixture generation. " +
                    result.DiagnosticCode +
                    " " +
                    result.Message);
            }

            return generation;
        }

        internal SaveRecoveryPlanBuilder Builder(
            int discoveryLimit =
                SaveRecoveryPlanBuilder
                    .DefaultDiscoveryLimit) =>
            new SaveRecoveryPlanBuilder(
                ReadOnlyBackend,
                Serializer,
                Integrity,
                discoveryLimit);

        internal void DeleteHead()
        {
            SaveStorageKey head =
                HeadKey();

            SaveStorageResult result =
                Local.Delete(
                    head);

            if (!result.Succeeded &&
                result.Status !=
                    SaveStorageStatus.NotFound)
            {
                throw new InvalidOperationException(
                    result.ToString());
            }
        }

        internal byte[] ReadHeadBytes()
        {
            SaveStorageReadResult read =
                Local.Read(
                    HeadKey());

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    read.Result.ToString());
            }

            return read.Data;
        }

        internal void RestoreHeadBytes(
            byte[] bytes)
        {
            SaveStorageKey head =
                HeadKey();

            Local.Delete(
                head);

            SaveStorageResult write =
                Local.WriteNew(
                    head,
                    bytes);

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.ToString());
            }
        }

        internal void CorruptHead()
        {
            SaveStorageKey head =
                HeadKey();

            Local.Delete(
                head);

            SaveStorageResult write =
                Local.WriteNew(
                    head,
                    Encoding.UTF8.GetBytes(
                        "{ definitely-not-a-head }"));

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.ToString());
            }
        }

        internal void MakeHeadUnsupported()
        {
            SaveHeadPointer head =
                ReadHead();

            head.formatMajor =
                SaveDocumentVersions.HeadPointerMajor +
                1;

            RewriteHeadUnchecked(
                head);
        }

        internal void PointHeadAtMissingGeneration(
            SaveGenerationId missing)
        {
            SaveHeadPointer head =
                ReadHead();

            head.currentGenerationId =
                missing.Value;

            RewriteHead(
                head);
        }

        internal void CorruptPayloadChecksum(
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult payloadRead =
                Local.Read(
                    keys.GenerationPayload);

            SaveStorageReadResult manifestRead =
                Local.Read(
                    keys.GenerationManifest);

            if (!payloadRead.Succeeded ||
                !manifestRead.Succeeded)
            {
                throw new InvalidOperationException(
                    "Recovery fixture generation was not readable.");
            }

            byte[] original =
                payloadRead.Data;

            byte[] changed =
                new byte[
                    original.Length + 1];

            Buffer.BlockCopy(
                original,
                0,
                changed,
                0,
                original.Length);

            changed[
                changed.Length - 1] =
                    (byte)' ';

            SaveSerializerResult deserialized =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest);

            if (!deserialized.Succeeded)
            {
                throw new InvalidOperationException(
                    deserialized.Message);
            }

            manifest.payloadByteLength =
                changed.LongLength;

            RewriteGenerationDocuments(
                keys,
                changed,
                Serialize(
                    manifest));
        }

        internal void MakeInventoryMismatch(
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult payloadRead =
                Local.Read(
                    keys.GenerationPayload);

            SaveStorageReadResult manifestRead =
                Local.Read(
                    keys.GenerationManifest);

            Serializer.Deserialize(
                Encoding.UTF8.GetString(
                    payloadRead.Data),
                out SavePayloadDocument payload);

            Serializer.Deserialize(
                Encoding.UTF8.GetString(
                    manifestRead.Data),
                out SaveManifest manifest);

            payload.entries =
                new[]
                {
                    new SavePayloadEntry()
                };

            byte[] changedPayload =
                Encoding.UTF8.GetBytes(
                    Serialize(
                        payload));

            SaveIntegrityResult checksum =
                Integrity.Calculate(
                    changedPayload,
                    out string payloadChecksum);

            if (!checksum.Succeeded)
            {
                throw new InvalidOperationException(
                    checksum.Message);
            }

            manifest.payloadByteLength =
                changedPayload.LongLength;

            manifest.payloadChecksum =
                payloadChecksum;

            RewriteGenerationDocuments(
                keys,
                changedPayload,
                Serialize(
                    manifest));
        }

        internal void MakeManifestUnsupported(
            SaveGenerationId generation)
        {
            MutateManifestUnchecked(
                generation,
                manifest =>
                    manifest.formatMajor =
                        SaveDocumentVersions.ManifestMajor +
                        1);
        }

        internal void MakeGenerationUncommitted(
            SaveGenerationId generation)
        {
            MutateManifest(
                generation,
                manifest =>
                    manifest.commitState =
                        SaveGenerationCommitState.Candidate);
        }

        internal void DeleteGenerationPayload(
            SaveGenerationId generation)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            Local.Delete(
                keys.GenerationPayload);
        }

        internal void CreateNonCanonicalChild(
            string name)
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                SlotId.Value +
                "/generations/" +
                name +
                "/marker.bin",
                out SaveStorageKey marker);

            SaveStorageResult write =
                Local.WriteNew(
                    marker,
                    new byte[]
                    {
                        1
                    });

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.ToString());
            }
        }

        internal SaveHeadPointer ReadHead()
        {
            SaveStorageReadResult read =
                Local.Read(
                    HeadKey());

            if (!read.Succeeded)
            {
                throw new InvalidOperationException(
                    read.Result.ToString());
            }

            SaveSerializerResult deserialized =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head);

            if (!deserialized.Succeeded)
            {
                throw new InvalidOperationException(
                    deserialized.Message);
            }

            return head;
        }

        public void Dispose()
        {
            Local.Shutdown();

            if (System.IO.Directory.Exists(
                    sandboxParent))
            {
                System.IO.Directory.Delete(
                    sandboxParent,
                    true);
            }
        }

        private void MutateManifest(
            SaveGenerationId generation,
            Action<SaveManifest> mutate)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationManifest);

            Serializer.Deserialize(
                Encoding.UTF8.GetString(
                    read.Data),
                out SaveManifest manifest);

            mutate(
                manifest);

            Local.Delete(
                keys.GenerationManifest);

            SaveStorageResult write =
                Local.WriteNew(
                    keys.GenerationManifest,
                    Encoding.UTF8.GetBytes(
                        Serialize(
                            manifest)));

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.ToString());
            }
        }

        private void MutateManifestUnchecked(
            SaveGenerationId generation,
            Action<SaveManifest> mutate)
        {
            SaveGenerationStorageKeys.TryCreate(
                SlotId,
                generation,
                out SaveGenerationStorageKeys keys);

            SaveStorageReadResult read =
                Local.Read(
                    keys.GenerationManifest);

            SaveSerializerResult deserialized =
                Serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveManifest manifest);

            if (!deserialized.Succeeded)
            {
                throw new InvalidOperationException(
                    deserialized.Message);
            }

            mutate(
                manifest);

            Local.Delete(
                keys.GenerationManifest);

            SaveStorageResult write =
                Local.WriteNew(
                    keys.GenerationManifest,
                    Encoding.UTF8.GetBytes(
                        SerializeUnchecked(
                            manifest)));

            if (!write.Succeeded)
            {
                throw new InvalidOperationException(
                    write.ToString());
            }
        }


        private void RewriteGenerationDocuments(
            SaveGenerationStorageKeys keys,
            byte[] payloadBytes,
            string manifestJson)
        {
            Local.Delete(
                keys.GenerationPayload);

            Local.Delete(
                keys.GenerationManifest);

            SaveStorageResult payloadWrite =
                Local.WriteNew(
                    keys.GenerationPayload,
                    payloadBytes);

            SaveStorageResult manifestWrite =
                Local.WriteNew(
                    keys.GenerationManifest,
                    Encoding.UTF8.GetBytes(
                        manifestJson));

            if (!payloadWrite.Succeeded ||
                !manifestWrite.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not rewrite recovery fixture generation documents.");
            }
        }

        private void RewriteHead(
            SaveHeadPointer head)
        {
            RestoreHeadBytes(
                Encoding.UTF8.GetBytes(
                    Serialize(
                        head)));
        }

        private void RewriteHeadUnchecked(
            SaveHeadPointer head)
        {
            RestoreHeadBytes(
                Encoding.UTF8.GetBytes(
                    SerializeUnchecked(
                        head)));
        }


        private static string SerializeUnchecked<T>(
            T value)
        {
            string json =
                UnityEngine.JsonUtility.ToJson(
                    value,
                    false);

            if (string.IsNullOrWhiteSpace(
                    json))
            {
                throw new InvalidOperationException(
                    "Unity JsonUtility returned no unsupported-version recovery fixture data.");
            }

            return json;
        }


        private string Serialize<T>(
            T value)
        {
            SaveSerializerResult result =
                Serializer.Serialize(
                    value,
                    out string json);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            return json;
        }

        private SaveStorageKey HeadKey()
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                SlotId.Value +
                "/head.json",
                out SaveStorageKey key);

            return key;
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

        private sealed class FixedRecoveryClock :
            ISaveClock
        {
            internal FixedRecoveryClock(
                DateTime utcNow)
            {
                UtcNow =
                    utcNow.Kind ==
                        DateTimeKind.Utc
                        ? utcNow
                        : utcNow.ToUniversalTime();
            }

            public DateTime UtcNow { get; }

            public double MonotonicSeconds =>
                0d;
        }
    }

    internal sealed class RecoveryReadOnlyCountingBackend :
        ISaveStorageBackend,
        ISaveStorageDiscoveryBackend
    {
        private readonly LocalFileSaveStorageBackend inner;

        internal RecoveryReadOnlyCountingBackend(
            LocalFileSaveStorageBackend inner)
        {
            this.inner =
                inner ??
                throw new ArgumentNullException(
                    nameof(inner));
        }

        internal int MutationCalls { get; private set; }

        internal bool FailReads { get; set; }

        internal bool FailDiscovery { get; set; }

        public SaveStorageBackendId Id =>
            inner.Id;

        public string RootPath =>
            inner.RootPath;

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
            if (FailReads)
            {
                return new SaveStorageReadResult(
                    new SaveStorageResult(
                        SaveStorageStatus.Failed,
                        "ESV-TEST-REC-READ",
                        "Injected recovery read failure."),
                    null);
            }

            return inner.Read(
                key);
        }

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            MutationCalls++;

            return inner.WriteNew(
                key,
                data);
        }

        public SaveStorageResult Delete(
            SaveStorageKey key)
        {
            MutationCalls++;

            return inner.Delete(
                key);
        }

        public SaveStorageResult Shutdown() =>
            inner.Shutdown();

        public SaveStorageDiscoveryResult
            DiscoverChildDirectories(
                SaveStorageKey parentKey,
                int maxChildren)
        {
            if (FailDiscovery)
            {
                return inner
                    .DiscoverChildDirectories(
                        parentKey,
                        0);
            }

            return inner
                .DiscoverChildDirectories(
                    parentKey,
                    maxChildren);
        }
    }
}
