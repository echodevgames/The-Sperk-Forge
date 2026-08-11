
namespace EchoDevGames.EchoSave
{
    public readonly struct SaveTrashRetentionResult
    {
        internal SaveTrashRetentionResult(
            SaveTrashRetentionStatus status,
            string diagnosticCode,
            string message,
            int discoveredRecordCount,
            int plannedDeletionCount,
            int deletedCount,
            string failingTrashRecordId)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            DiscoveredRecordCount = discoveredRecordCount;
            PlannedDeletionCount = plannedDeletionCount;
            DeletedCount = deletedCount;
            FailingTrashRecordId =
                failingTrashRecordId ?? string.Empty;
        }

        public SaveTrashRetentionStatus Status { get; }
        public string DiagnosticCode { get; }
        public string Message { get; }
        public int DiscoveredRecordCount { get; }
        public int PlannedDeletionCount { get; }
        public int DeletedCount { get; }
        public string FailingTrashRecordId { get; }

        public bool Succeeded =>
            Status == SaveTrashRetentionStatus.NotRequired ||
            Status == SaveTrashRetentionStatus.Completed;

        public bool MaintenanceFailed => !Succeeded;

        internal static SaveTrashRetentionResult NotRequired(
            string message,
            int discoveredRecordCount = 0) =>
            new SaveTrashRetentionResult(
                SaveTrashRetentionStatus.NotRequired,
                string.Empty,
                message,
                discoveredRecordCount,
                0,
                0,
                string.Empty);
    }
}
