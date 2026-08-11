
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotRenameResult
    {
        internal SaveSlotRenameResult(
            SaveSlotRenameStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            SaveGenerationId publishedGenerationId,
            bool generationPublished,
            bool headPublished,
            bool catalogReconciled,
            SaveSlotCatalogEntry reconciledEntry,
            SaveRetentionResult retentionResult)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            SlotId = slotId;
            SourceGenerationId = sourceGenerationId;
            PublishedGenerationId = publishedGenerationId;
            GenerationPublished = generationPublished;
            HeadPublished = headPublished;
            CatalogReconciled = catalogReconciled;
            ReconciledEntry = reconciledEntry;
            RetentionResult = retentionResult;
        }

        public SaveSlotRenameStatus Status { get; }
        public string DiagnosticCode { get; }
        public string Message { get; }
        public SaveSlotId SlotId { get; }
        public SaveGenerationId SourceGenerationId { get; }
        public SaveGenerationId PublishedGenerationId { get; }
        public bool GenerationPublished { get; }
        public bool HeadPublished { get; }
        public bool CatalogReconciled { get; }
        public SaveSlotCatalogEntry ReconciledEntry { get; }
        public SaveRetentionResult RetentionResult { get; }
        public bool RenameCommitted => HeadPublished;
        public bool Succeeded =>
            Status == SaveSlotRenameStatus.Succeeded ||
            Status == SaveSlotRenameStatus.NoChange;

        internal static SaveSlotRenameResult Failure(
            SaveSlotRenameStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default,
            SaveGenerationId publishedGenerationId = default,
            bool generationPublished = false,
            bool headPublished = false,
            bool catalogReconciled = false,
            SaveRetentionResult retentionResult = default) =>
            new SaveSlotRenameResult(
                status,
                diagnosticCode,
                message,
                slotId,
                sourceGenerationId,
                publishedGenerationId,
                generationPublished,
                headPublished,
                catalogReconciled,
                null,
                retentionResult);
    }
}
