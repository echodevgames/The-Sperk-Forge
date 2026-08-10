
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveCarryForwardPublicationResult
    {
        internal SaveCarryForwardPublicationResult(
            SaveCarryForwardPublicationStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            SaveGenerationId publishedGenerationId,
            SaveParticipantId failingPersistedId,
            SaveParticipantId currentOwnerId,
            int freshParticipantCount,
            int preservedUnknownCount,
            long totalPayloadBytes,
            bool generationPublished,
            bool headPublished)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            SlotId =
                slotId;

            SourceGenerationId =
                sourceGenerationId;

            PublishedGenerationId =
                publishedGenerationId;

            FailingPersistedId =
                failingPersistedId;

            CurrentOwnerId =
                currentOwnerId;

            FreshParticipantCount =
                freshParticipantCount;

            PreservedUnknownCount =
                preservedUnknownCount;

            TotalPayloadBytes =
                totalPayloadBytes;

            GenerationPublished =
                generationPublished;

            HeadPublished =
                headPublished;
        }

        internal SaveCarryForwardPublicationStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId SourceGenerationId { get; }

        internal SaveGenerationId PublishedGenerationId { get; }

        internal SaveParticipantId FailingPersistedId { get; }

        internal SaveParticipantId CurrentOwnerId { get; }

        internal int FreshParticipantCount { get; }

        internal int PreservedUnknownCount { get; }

        internal long TotalPayloadBytes { get; }

        internal bool GenerationPublished { get; }

        internal bool HeadPublished { get; }

        internal bool Succeeded =>
            Status ==
            SaveCarryForwardPublicationStatus.Succeeded;
    }
}
