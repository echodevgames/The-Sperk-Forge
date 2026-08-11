
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotDuplicateResult
    {
        internal SaveSlotDuplicateResult(
            SaveSlotDuplicateStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SaveSlotId duplicateSlotId,
            SaveGenerationId duplicateGenerationId,
            bool generationPublished,
            bool headPublished,
            bool catalogReconciled,
            SaveSlotCatalogEntry duplicateEntry)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            SourceSlotId = sourceSlotId;
            SourceGenerationId = sourceGenerationId;
            DuplicateSlotId = duplicateSlotId;
            DuplicateGenerationId = duplicateGenerationId;
            GenerationPublished = generationPublished;
            HeadPublished = headPublished;
            CatalogReconciled = catalogReconciled;
            DuplicateEntry = duplicateEntry;
        }

        public SaveSlotDuplicateStatus Status { get; }
        public string DiagnosticCode { get; }
        public string Message { get; }
        public SaveSlotId SourceSlotId { get; }
        public SaveGenerationId SourceGenerationId { get; }
        public SaveSlotId DuplicateSlotId { get; }
        public SaveGenerationId DuplicateGenerationId { get; }
        public bool GenerationPublished { get; }
        public bool HeadPublished { get; }
        public bool CatalogReconciled { get; }
        public SaveSlotCatalogEntry DuplicateEntry { get; }
        public bool DuplicateCommitted => HeadPublished;
        public bool Succeeded => Status == SaveSlotDuplicateStatus.Succeeded;

        internal static SaveSlotDuplicateResult Failure(
            SaveSlotDuplicateStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId sourceSlotId = default,
            SaveGenerationId sourceGenerationId = default,
            SaveSlotId duplicateSlotId = default,
            SaveGenerationId duplicateGenerationId = default,
            bool generationPublished = false,
            bool headPublished = false,
            bool catalogReconciled = false) =>
            new SaveSlotDuplicateResult(
                status,
                diagnosticCode,
                message,
                sourceSlotId,
                sourceGenerationId,
                duplicateSlotId,
                duplicateGenerationId,
                generationPublished,
                headPublished,
                catalogReconciled,
                null);
    }
}
