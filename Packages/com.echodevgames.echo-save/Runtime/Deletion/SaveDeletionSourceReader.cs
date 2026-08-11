
using System;
using System.Text;

namespace EchoDevGames.EchoSave
{
    internal enum SaveDeletionSourceStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        HeadUnavailable = 2,
        SourceInvalid = 3,
        SourceStale = 4
    }

    internal sealed class SaveDeletionSourceSnapshot
    {
        internal SaveDeletionSourceSnapshot(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            long headUpdateSequence,
            string provenanceFingerprint,
            string displayName)
        {
            SlotId = slotId;
            GenerationId = generationId;
            HeadUpdateSequence = headUpdateSequence;
            ProvenanceFingerprint =
                provenanceFingerprint ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
        }

        internal SaveSlotId SlotId { get; }
        internal SaveGenerationId GenerationId { get; }
        internal long HeadUpdateSequence { get; }
        internal string ProvenanceFingerprint { get; }
        internal string DisplayName { get; }
    }

    internal sealed class SaveDeletionSourceReadResult
    {
        internal SaveDeletionSourceReadResult(
            SaveDeletionSourceStatus status,
            string diagnosticCode,
            string message,
            SaveDeletionSourceSnapshot snapshot)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot = snapshot;
        }

        internal SaveDeletionSourceStatus Status { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
        internal SaveDeletionSourceSnapshot Snapshot { get; }

        internal bool Succeeded =>
            Status == SaveDeletionSourceStatus.Succeeded &&
            Snapshot != null;

        internal static SaveDeletionSourceReadResult Failure(
            SaveDeletionSourceStatus status,
            string diagnosticCode,
            string message) =>
            new SaveDeletionSourceReadResult(
                status,
                diagnosticCode,
                message,
                null);
    }

    internal interface ISaveDeletionSourceReader
    {
        SaveDeletionSourceReadResult Read(
            SaveSlotId slotId);

        SaveDeletionSourceReadResult Revalidate(
            SaveDeletionSourceSnapshot snapshot);
    }

    internal sealed class SaveDeletionSourceReader :
        ISaveDeletionSourceReader
    {
        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly IIntegrityProvider integrity;

        internal SaveDeletionSourceReader(
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

        public SaveDeletionSourceReadResult Read(
            SaveSlotId slotId)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveDeletionSourceStatus.InvalidRequest,
                    "Chronicle deletion planning requires one valid technical slot identity.");
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
                    SaveDeletionSourceStatus.InvalidRequest,
                    headKeyResult.Message,
                    headKeyResult.DiagnosticCode);
            }

            SaveStorageReadResult headRead =
                storage.Read(headKey);

            if (!headRead.Succeeded)
            {
                return Failure(
                    SaveDeletionSourceStatus.HeadUnavailable,
                    "Chronicle deletion planning could not read the authoritative source head. " +
                    headRead.Result.Message,
                    headRead.Result.DiagnosticCode);
            }

            SaveSerializerResult headDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(headRead.Data),
                    out SaveHeadPointer head);

            if (!headDeserialize.Succeeded ||
                head == null)
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceInvalid,
                    "Chronicle deletion planning could not deserialize the authoritative source head.");
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
                    SaveDeletionSourceStatus.SourceInvalid,
                    "Chronicle deletion planning requires one structurally valid source head matching the requested slot.",
                    headValidation.DiagnosticCode);
            }

            SaveStorageResult generationKeys =
                SaveGenerationStorageKeys.TryCreate(
                    validatedSlot,
                    currentGeneration,
                    out SaveGenerationStorageKeys keys);

            if (!generationKeys.Succeeded)
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceInvalid,
                    generationKeys.Message,
                    generationKeys.DiagnosticCode);
            }

            SaveStorageReadResult manifestRead =
                storage.Read(
                    keys.GenerationManifest);

            if (!manifestRead.Succeeded)
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceInvalid,
                    "Chronicle deletion planning could not read the current source manifest. " +
                    manifestRead.Result.Message,
                    manifestRead.Result.DiagnosticCode);
            }

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(manifestRead.Data),
                    out SaveManifest manifest);

            if (!manifestDeserialize.Succeeded ||
                manifest == null ||
                manifest.commitState != SaveGenerationCommitState.Committed ||
                !string.Equals(
                    manifest.slotId,
                    validatedSlot.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    currentGeneration.Value,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceInvalid,
                    "Chronicle deletion planning requires one current committed manifest matching the source head.");
            }

            SaveIntegrityResult headFingerprint =
                integrity.Calculate(
                    headRead.Data,
                    out string headHash);

            SaveIntegrityResult manifestFingerprint =
                integrity.Calculate(
                    manifestRead.Data,
                    out string manifestHash);

            if (!headFingerprint.Succeeded ||
                !manifestFingerprint.Succeeded)
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceInvalid,
                    "Chronicle deletion planning could not fingerprint the lightweight source provenance.");
            }

            return new SaveDeletionSourceReadResult(
                SaveDeletionSourceStatus.Succeeded,
                string.Empty,
                "Chronicle deletion source provenance is valid.",
                new SaveDeletionSourceSnapshot(
                    validatedSlot,
                    currentGeneration,
                    head.updateSequence,
                    headHash + "|" + manifestHash,
                    manifest.displayName));
        }

        public SaveDeletionSourceReadResult Revalidate(
            SaveDeletionSourceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return Failure(
                    SaveDeletionSourceStatus.InvalidRequest,
                    "Chronicle deletion source revalidation requires one prepared source snapshot.");
            }

            SaveDeletionSourceReadResult fresh =
                Read(snapshot.SlotId);

            if (!fresh.Succeeded)
            {
                return fresh;
            }

            SaveDeletionSourceSnapshot current =
                fresh.Snapshot;

            if (current.GenerationId != snapshot.GenerationId ||
                current.HeadUpdateSequence != snapshot.HeadUpdateSequence ||
                !string.Equals(
                    current.ProvenanceFingerprint,
                    snapshot.ProvenanceFingerprint,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveDeletionSourceStatus.SourceStale,
                    "Chronicle deletion source changed after the deletion plan was prepared.");
            }

            return fresh;
        }

        private static SaveDeletionSourceReadResult Failure(
            SaveDeletionSourceStatus status,
            string message,
            string diagnosticCode = "") =>
            SaveDeletionSourceReadResult.Failure(
                status,
                diagnosticCode,
                message);
    }
}
