
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Lightweight payload-free metadata reconstructed from one slot head and
    /// its current immutable-generation manifest.
    /// </summary>
    public sealed class SaveSlotCatalogEntry
    {
        internal SaveSlotCatalogEntry(
            SaveSlotId slotId,
            SaveGenerationId currentGenerationId,
            SaveSlotHealth health,
            string diagnosticCode,
            string message,
            string createdUtc,
            string updatedUtc,
            string displayName,
            string saveKind,
            string projectId,
            string projectVersion,
            string buildId,
            int participantCount,
            long payloadByteLength)
        {
            SlotId = slotId;
            CurrentGenerationId = currentGenerationId;
            Health = health;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            CreatedUtc = createdUtc ?? string.Empty;
            UpdatedUtc = updatedUtc ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
            SaveKind = saveKind ?? string.Empty;
            ProjectId = projectId ?? string.Empty;
            ProjectVersion = projectVersion ?? string.Empty;
            BuildId = buildId ?? string.Empty;
            ParticipantCount = participantCount;
            PayloadByteLength = payloadByteLength;
        }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId CurrentGenerationId { get; }

        public SaveSlotHealth Health { get; }

        public bool IsSelectable =>
            Health == SaveSlotHealth.Healthy;

        public string DiagnosticCode { get; }

        public string Message { get; }

        public string CreatedUtc { get; }

        public string UpdatedUtc { get; }

        public string DisplayName { get; }

        public string SaveKind { get; }

        public string ProjectId { get; }

        public string ProjectVersion { get; }

        public string BuildId { get; }

        public int ParticipantCount { get; }

        public long PayloadByteLength { get; }
    }
}
