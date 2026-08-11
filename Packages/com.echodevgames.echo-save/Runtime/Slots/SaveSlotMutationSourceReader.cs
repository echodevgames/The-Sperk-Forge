
using System;
using System.Text;

namespace EchoDevGames.EchoSave
{
    internal enum SaveSlotMutationSourceStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        HeadUnavailable = 2,
        SourceInvalid = 3,
        SourceStale = 4
    }

    internal sealed class SaveSlotMutationSourceReadResult
    {
        internal SaveSlotMutationSourceReadResult(
            SaveSlotMutationSourceStatus status,
            string diagnosticCode,
            string message,
            SaveSlotMutationSourceSnapshot snapshot)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot = snapshot;
        }

        internal SaveSlotMutationSourceStatus Status { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
        internal SaveSlotMutationSourceSnapshot Snapshot { get; }

        internal bool Succeeded =>
            Status == SaveSlotMutationSourceStatus.Succeeded &&
            Snapshot != null;

        internal static SaveSlotMutationSourceReadResult Failure(
            SaveSlotMutationSourceStatus status,
            string diagnosticCode,
            string message) =>
            new SaveSlotMutationSourceReadResult(
                status,
                diagnosticCode,
                message,
                null);
    }

    internal sealed class SaveSlotMutationSourceSnapshot
    {
        private readonly SavePayloadEntry[] payloadEntries;
        private readonly SavePayloadInventoryEntry[] inventoryEntries;

        internal SaveSlotMutationSourceSnapshot(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            long headUpdateSequence,
            string provenanceFingerprint,
            string saveKind,
            string projectId,
            string projectVersion,
            string buildId,
            string displayName,
            SavePayloadEntry[] payloadEntries,
            SavePayloadInventoryEntry[] inventoryEntries)
        {
            SlotId = slotId;
            GenerationId = generationId;
            HeadUpdateSequence = headUpdateSequence;
            ProvenanceFingerprint = provenanceFingerprint ?? string.Empty;
            SaveKind = saveKind ?? string.Empty;
            ProjectId = projectId ?? string.Empty;
            ProjectVersion = projectVersion ?? string.Empty;
            BuildId = buildId ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
            this.payloadEntries = ClonePayloadEntries(payloadEntries);
            this.inventoryEntries = CloneInventoryEntries(inventoryEntries);
        }

        internal SaveSlotId SlotId { get; }
        internal SaveGenerationId GenerationId { get; }
        internal long HeadUpdateSequence { get; }
        internal string ProvenanceFingerprint { get; }
        internal string SaveKind { get; }
        internal string ProjectId { get; }
        internal string ProjectVersion { get; }
        internal string BuildId { get; }
        internal string DisplayName { get; }

        internal SavePayloadEntry[] CopyPayloadEntries() =>
            ClonePayloadEntries(payloadEntries);

        internal SavePayloadInventoryEntry[] CopyInventoryEntries() =>
            CloneInventoryEntries(inventoryEntries);

        private static SavePayloadEntry[] ClonePayloadEntries(
            SavePayloadEntry[] source)
        {
            SavePayloadEntry[] safe =
                source ?? Array.Empty<SavePayloadEntry>();

            SavePayloadEntry[] copy =
                new SavePayloadEntry[safe.Length];

            for (int i = 0; i < safe.Length; i++)
            {
                SavePayloadEntry entry = safe[i];
                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadEntry
                        {
                            participantId = entry.participantId,
                            participantSchemaVersion = entry.participantSchemaVersion,
                            serializerId = entry.serializerId,
                            required = entry.required,
                            serializedPayload = entry.serializedPayload,
                            byteProviderReference = entry.byteProviderReference,
                            byteLength = entry.byteLength,
                            checksum = entry.checksum,
                            flags = entry.flags
                        };
            }

            return copy;
        }

        private static SavePayloadInventoryEntry[] CloneInventoryEntries(
            SavePayloadInventoryEntry[] source)
        {
            SavePayloadInventoryEntry[] safe =
                source ?? Array.Empty<SavePayloadInventoryEntry>();

            SavePayloadInventoryEntry[] copy =
                new SavePayloadInventoryEntry[safe.Length];

            for (int i = 0; i < safe.Length; i++)
            {
                SavePayloadInventoryEntry entry = safe[i];
                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadInventoryEntry
                        {
                            participantId = entry.participantId,
                            participantSchemaVersion = entry.participantSchemaVersion,
                            serializerId = entry.serializerId,
                            required = entry.required,
                            byteLength = entry.byteLength,
                            checksum = entry.checksum,
                            flags = entry.flags
                        };
            }

            return copy;
        }
    }

    internal interface ISaveSlotMutationSourceReader
    {
        SaveSlotMutationSourceReadResult Read(
            SaveSlotId slotId);

        SaveSlotMutationSourceReadResult Revalidate(
            SaveSlotMutationSourceSnapshot snapshot);
    }

    /// <summary>
    /// Provider-neutral, read-only source verification for M4-09 slot mutation.
    /// </summary>
    internal sealed class SaveSlotMutationSourceReader :
        ISaveSlotMutationSourceReader
    {
        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly IIntegrityProvider integrity;

        internal SaveSlotMutationSourceReader(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(nameof(storage));

            this.serializer =
                serializer ??
                throw new ArgumentNullException(nameof(serializer));

            this.integrity =
                integrity ??
                throw new ArgumentNullException(nameof(integrity));
        }

        public SaveSlotMutationSourceReadResult Read(
            SaveSlotId slotId)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveSlotMutationSourceStatus.InvalidRequest,
                    "Chronicle slot mutation requires one valid source slot identity.");
            }

            SaveStorageResult headKeyResult =
                SaveStorageKey.TryCreate(
                    "slots/" +
                    validatedSlot.Value +
                    "/head.json",
                    out SaveStorageKey headKey);

            if (!headKeyResult.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.InvalidRequest,
                    headKeyResult.Message,
                    headKeyResult.DiagnosticCode);
            }

            SaveStorageReadResult headRead =
                storage.Read(headKey);

            if (!headRead.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.HeadUnavailable,
                    "Chronicle slot mutation could not read one authoritative source head. " +
                    headRead.Result.Message,
                    headRead.Result.DiagnosticCode);
            }

            byte[] headBytes = headRead.Data;

            SaveSerializerResult headDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(headBytes),
                    out SaveHeadPointer head);

            if (!headDeserialize.Succeeded ||
                head == null)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    "Chronicle slot mutation source head could not be deserialized.");
            }

            SaveDocumentValidationResult headValidation =
                SaveCommitDocumentValidator.ValidateHead(head);

            if (!headValidation.Succeeded ||
                !string.Equals(
                    head.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId currentGeneration))
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    "Chronicle slot mutation requires one structurally valid source head matching the requested slot.",
                    headValidation.DiagnosticCode);
            }

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    validatedSlot,
                    currentGeneration,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    keysResult.Message,
                    keysResult.DiagnosticCode);
            }

            SaveStorageReadResult manifestRead =
                storage.Read(keys.GenerationManifest);

            SaveStorageReadResult payloadRead =
                storage.Read(keys.GenerationPayload);

            if (!manifestRead.Succeeded ||
                !payloadRead.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    "Chronicle slot mutation could not read the source current generation completely.");
            }

            byte[] manifestBytes = manifestRead.Data;
            byte[] payloadBytes = payloadRead.Data;

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(manifestBytes),
                    out SaveManifest manifest);

            SaveSerializerResult payloadDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(payloadBytes),
                    out SavePayloadDocument payload);

            if (!manifestDeserialize.Succeeded ||
                !payloadDeserialize.Succeeded ||
                manifest == null ||
                payload == null ||
                !string.Equals(
                    manifest.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    currentGeneration.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.generationId,
                    currentGeneration.Value,
                    StringComparison.Ordinal) ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    "Chronicle slot mutation source documents do not describe one matching committed current generation.");
            }

            SaveDocumentValidationResult documentValidation =
                SaveCommitDocumentValidator.ValidateManifestAndPayload(
                    manifest,
                    payload,
                    payloadBytes,
                    integrity);

            if (!documentValidation.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    documentValidation.Message,
                    documentValidation.DiagnosticCode);
            }

            SavePayloadEntry[] payloadEntries =
                payload.entries ??
                Array.Empty<SavePayloadEntry>();

            SavePayloadInventoryEntry[] inventoryEntries =
                manifest.payloadEntries ??
                Array.Empty<SavePayloadInventoryEntry>();

            SaveDocumentValidationResult entryValidation =
                SaveParticipantPublicationBatchValidator.ValidateStoredEntries(
                    payloadEntries,
                    inventoryEntries,
                    integrity);

            if (!entryValidation.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    entryValidation.Message,
                    entryValidation.DiagnosticCode);
            }

            SaveIntegrityResult headFingerprint =
                integrity.Calculate(
                    headBytes,
                    out string headHash);

            SaveIntegrityResult manifestFingerprint =
                integrity.Calculate(
                    manifestBytes,
                    out string manifestHash);

            SaveIntegrityResult payloadFingerprint =
                integrity.Calculate(
                    payloadBytes,
                    out string payloadHash);

            if (!headFingerprint.Succeeded ||
                !manifestFingerprint.Succeeded ||
                !payloadFingerprint.Succeeded)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceInvalid,
                    "Chronicle slot mutation could not fingerprint the verified source evidence.");
            }

            string fingerprint =
                headHash +
                "|" +
                manifestHash +
                "|" +
                payloadHash;

            return new SaveSlotMutationSourceReadResult(
                SaveSlotMutationSourceStatus.Succeeded,
                string.Empty,
                "Chronicle slot mutation source is one fully verified current generation.",
                new SaveSlotMutationSourceSnapshot(
                    validatedSlot,
                    currentGeneration,
                    head.updateSequence,
                    fingerprint,
                    manifest.saveKind,
                    manifest.projectId,
                    manifest.projectVersion,
                    manifest.buildId,
                    manifest.displayName,
                    payloadEntries,
                    inventoryEntries));
        }

        public SaveSlotMutationSourceReadResult Revalidate(
            SaveSlotMutationSourceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return Failure(
                    SaveSlotMutationSourceStatus.InvalidRequest,
                    "Chronicle slot mutation source revalidation requires one prior verified snapshot.");
            }

            SaveSlotMutationSourceReadResult fresh =
                Read(snapshot.SlotId);

            if (!fresh.Succeeded)
            {
                return fresh;
            }

            SaveSlotMutationSourceSnapshot current =
                fresh.Snapshot;

            if (current.GenerationId != snapshot.GenerationId ||
                current.HeadUpdateSequence != snapshot.HeadUpdateSequence ||
                !string.Equals(
                    current.ProvenanceFingerprint,
                    snapshot.ProvenanceFingerprint,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveSlotMutationSourceStatus.SourceStale,
                    "Chronicle slot mutation source changed after preflight. Refresh source state before retrying.");
            }

            return fresh;
        }

        private static SaveSlotMutationSourceReadResult Failure(
            SaveSlotMutationSourceStatus status,
            string message,
            string diagnosticCode = "") =>
            SaveSlotMutationSourceReadResult.Failure(
                status,
                diagnosticCode,
                message);
    }
}
