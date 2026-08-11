namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Truthful result for the bounded M4-03 manual-save transaction.
    ///
    /// GenerationPublished and HeadPublished describe durable publication truth.
    /// CatalogReconciled is separate because a committed head must never be
    /// fictionalized as rolled back when the derived catalog refresh fails.
    /// </summary>
    internal sealed class SaveManualTransactionResult
    {
        internal SaveManualTransactionResult(
            SaveManualTransactionStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            SaveGenerationId publishedGenerationId,
            SaveParticipantId failingParticipantId,
            SaveParticipantId currentOwnerId,
            int freshParticipantCount,
            int preservedUnknownCount,
            long totalPayloadBytes,
            bool generationPublished,
            bool headPublished,
            bool catalogReconciled,
            SaveSlotCatalogEntry reconciledEntry,
            SaveRetentionResult retentionResult = default)
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

            FailingParticipantId =
                failingParticipantId;

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

            CatalogReconciled =
                catalogReconciled;

            ReconciledEntry =
                reconciledEntry;

            RetentionResult =
                retentionResult;
        }

        internal SaveManualTransactionStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId SourceGenerationId { get; }

        internal SaveGenerationId PublishedGenerationId { get; }

        internal SaveParticipantId FailingParticipantId { get; }

        internal SaveParticipantId CurrentOwnerId { get; }

        internal int FreshParticipantCount { get; }

        internal int PreservedUnknownCount { get; }

        internal long TotalPayloadBytes { get; }

        internal bool GenerationPublished { get; }

        internal bool HeadPublished { get; }

        internal bool CatalogReconciled { get; }

        internal SaveSlotCatalogEntry ReconciledEntry { get; }

        internal SaveRetentionResult RetentionResult { get; }

        internal bool Succeeded =>
            Status ==
            SaveManualTransactionStatus.Succeeded;
    }
}
