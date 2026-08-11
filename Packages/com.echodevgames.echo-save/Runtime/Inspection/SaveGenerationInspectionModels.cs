using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    public enum SaveGenerationInspectionStatus
    {
        Healthy = 0,
        InvalidGenerationId = 1,
        MissingManifest = 2,
        InvalidManifest = 3,
        UnsupportedManifest = 4,
        IdentityMismatch = 5,
        BackendReadFailure = 6
    }

    public enum SaveGenerationInspectionSnapshotStatus
    {
        Succeeded = 0,
        SucceededEmpty = 1,
        InvalidSlot = 2,
        RootMissing = 3,
        DiscoveryFailed = 4,
        DiscoveryLimitExceeded = 5,
        SessionClosed = 6
    }

    /// <summary>
    /// Read-only metadata for one immutable committed-generation candidate.
    /// </summary>
    public sealed class SaveGenerationInspectionEntry
    {
        internal SaveGenerationInspectionEntry(
            string generationId,
            bool isCurrentHead,
            SaveGenerationInspectionStatus status,
            string sourceManifestVersion,
            string currentManifestVersion,
            bool wasMigratedInMemory,
            string commitState,
            string createdUtc,
            string updatedUtc,
            string displayName,
            string saveKind,
            string projectId,
            string projectVersion,
            string buildId,
            int participantCount,
            long payloadByteLength,
            string diagnosticCode,
            string message)
        {
            GenerationId = generationId ?? string.Empty;
            IsCurrentHead = isCurrentHead;
            Status = status;
            SourceManifestVersion = sourceManifestVersion ?? string.Empty;
            CurrentManifestVersion = currentManifestVersion ?? string.Empty;
            WasMigratedInMemory = wasMigratedInMemory;
            CommitState = commitState ?? string.Empty;
            CreatedUtc = createdUtc ?? string.Empty;
            UpdatedUtc = updatedUtc ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
            SaveKind = saveKind ?? string.Empty;
            ProjectId = projectId ?? string.Empty;
            ProjectVersion = projectVersion ?? string.Empty;
            BuildId = buildId ?? string.Empty;
            ParticipantCount = participantCount;
            PayloadByteLength = payloadByteLength;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string GenerationId { get; }

        public bool IsCurrentHead { get; }

        public SaveGenerationInspectionStatus Status { get; }

        public string SourceManifestVersion { get; }

        public string CurrentManifestVersion { get; }

        public bool WasMigratedInMemory { get; }

        public string CommitState { get; }

        public string CreatedUtc { get; }

        public string UpdatedUtc { get; }

        public string DisplayName { get; }

        public string SaveKind { get; }

        public string ProjectId { get; }

        public string ProjectVersion { get; }

        public string BuildId { get; }

        public int ParticipantCount { get; }

        public long PayloadByteLength { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool IsHealthy =>
            Status == SaveGenerationInspectionStatus.Healthy;
    }

    /// <summary>
    /// Copied deterministic generation-inspection result for one slot.
    /// </summary>
    public sealed class SaveGenerationInspectionSnapshot
    {
        private readonly ReadOnlyCollection<SaveGenerationInspectionEntry> entries;

        internal SaveGenerationInspectionSnapshot(
            SaveGenerationInspectionSnapshotStatus status,
            SaveSlotId slotId,
            string currentGenerationId,
            SaveGenerationInspectionEntry[] entries,
            string diagnosticCode,
            string message)
        {
            Status = status;
            SlotId = slotId;
            CurrentGenerationId = currentGenerationId ?? string.Empty;

            SaveGenerationInspectionEntry[] copy =
                entries == null
                    ? Array.Empty<SaveGenerationInspectionEntry>()
                    : (SaveGenerationInspectionEntry[])entries.Clone();

            this.entries = Array.AsReadOnly(copy);
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public SaveGenerationInspectionSnapshotStatus Status { get; }

        public SaveSlotId SlotId { get; }

        public string CurrentGenerationId { get; }

        public IReadOnlyList<SaveGenerationInspectionEntry> Entries =>
            entries;

        public int Count =>
            entries.Count;

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == SaveGenerationInspectionSnapshotStatus.Succeeded ||
            Status == SaveGenerationInspectionSnapshotStatus.SucceededEmpty ||
            Status == SaveGenerationInspectionSnapshotStatus.RootMissing;
    }
}
