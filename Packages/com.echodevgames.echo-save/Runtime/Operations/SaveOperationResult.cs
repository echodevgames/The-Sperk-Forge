namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable public M4-04 manual-save result.
    ///
    /// Durable generation/head/catalog truth remains visible even when a
    /// cancellation request arrived too late to stop publication.
    /// </summary>
    public readonly struct SaveOperationResult
    {
        internal SaveOperationResult(
            SaveOperationStatus status,
            SaveCancellationDisposition cancellationDisposition,
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
            SaveSlotCatalogEntry reconciledEntry)
        {
            Status =
                status;

            CancellationDisposition =
                cancellationDisposition;

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
        }

        public SaveOperationStatus Status { get; }

        public SaveCancellationDisposition CancellationDisposition { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public SaveGenerationId PublishedGenerationId { get; }

        public SaveParticipantId FailingParticipantId { get; }

        public SaveParticipantId CurrentOwnerId { get; }

        public int FreshParticipantCount { get; }

        public int PreservedUnknownCount { get; }

        public long TotalPayloadBytes { get; }

        public bool GenerationPublished { get; }

        public bool HeadPublished { get; }

        public bool CatalogReconciled { get; }

        public SaveSlotCatalogEntry ReconciledEntry { get; }

        public bool Succeeded =>
            Status ==
            SaveOperationStatus.Succeeded;

        public bool CancellationWasTooLate =>
            CancellationDisposition ==
            SaveCancellationDisposition.TooLate;
    }
}
