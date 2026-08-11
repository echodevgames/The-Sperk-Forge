
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotDeleteResult
    {
        internal SaveSlotDeleteResult(
            SaveSlotDeleteStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            string trashRecordId,
            bool deleteCommitted,
            bool catalogReconciled,
            bool activeSlotCleared,
            SaveTrashRetentionResult trashRetentionResult)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            SlotId = slotId;
            SourceGenerationId = sourceGenerationId;
            TrashRecordId = trashRecordId ?? string.Empty;
            DeleteCommitted = deleteCommitted;
            CatalogReconciled = catalogReconciled;
            ActiveSlotCleared = activeSlotCleared;
            TrashRetentionResult = trashRetentionResult;
        }

        public SaveSlotDeleteStatus Status { get; }
        public string DiagnosticCode { get; }
        public string Message { get; }
        public SaveSlotId SlotId { get; }
        public SaveGenerationId SourceGenerationId { get; }
        public string TrashRecordId { get; }
        public bool DeleteCommitted { get; }
        public bool CatalogReconciled { get; }
        public bool ActiveSlotCleared { get; }
        public SaveTrashRetentionResult TrashRetentionResult { get; }

        public bool Succeeded =>
            Status == SaveSlotDeleteStatus.Succeeded;

        internal static SaveSlotDeleteResult Failure(
            SaveSlotDeleteStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default,
            string trashRecordId = "",
            bool deleteCommitted = false,
            bool catalogReconciled = false,
            bool activeSlotCleared = false,
            SaveTrashRetentionResult trashRetentionResult = default) =>
            new SaveSlotDeleteResult(
                status,
                diagnosticCode,
                message,
                slotId,
                sourceGenerationId,
                trashRecordId,
                deleteCommitted,
                catalogReconciled,
                activeSlotCleared,
                trashRetentionResult);
    }
}
