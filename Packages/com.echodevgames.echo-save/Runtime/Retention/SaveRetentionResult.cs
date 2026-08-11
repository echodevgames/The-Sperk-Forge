
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Truthful post-publication retention-maintenance result.
    ///
    /// A failed retention result never implies rollback of an already
    /// committed generation/head transaction.
    /// </summary>
    public readonly struct SaveRetentionResult
    {
        private readonly string diagnosticCode;
        private readonly string message;

        internal SaveRetentionResult(
            SaveRetentionStatus status,
            string diagnosticCode,
            string message,
            int discoveredCanonicalCount,
            int verifiedCommittedCount,
            int plannedDeletionCount,
            int deletedCount,
            SaveGenerationId failingGenerationId)
        {
            Status =
                status;

            this.diagnosticCode =
                diagnosticCode ?? string.Empty;

            this.message =
                message ?? string.Empty;

            DiscoveredCanonicalCount =
                discoveredCanonicalCount;

            VerifiedCommittedCount =
                verifiedCommittedCount;

            PlannedDeletionCount =
                plannedDeletionCount;

            DeletedCount =
                deletedCount;

            FailingGenerationId =
                failingGenerationId;
        }

        public SaveRetentionStatus Status { get; }

        public string DiagnosticCode =>
            diagnosticCode ?? string.Empty;

        public string Message =>
            message ?? string.Empty;

        public int DiscoveredCanonicalCount { get; }

        public int VerifiedCommittedCount { get; }

        public int PlannedDeletionCount { get; }

        public int DeletedCount { get; }

        public SaveGenerationId FailingGenerationId { get; }

        public bool Succeeded =>
            Status ==
                SaveRetentionStatus.NotRequired ||
            Status ==
                SaveRetentionStatus.Completed;

        public bool MaintenanceFailed =>
            !Succeeded;

        internal static SaveRetentionResult NotRequired(
            string message,
            int discoveredCanonicalCount = 0,
            int verifiedCommittedCount = 0) =>
            new SaveRetentionResult(
                SaveRetentionStatus.NotRequired,
                string.Empty,
                message,
                discoveredCanonicalCount,
                verifiedCommittedCount,
                0,
                0,
                default);
    }
}
