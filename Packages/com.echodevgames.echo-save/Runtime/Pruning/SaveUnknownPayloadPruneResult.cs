
namespace EchoDevGames.EchoSave
{
    public sealed class SaveUnknownPayloadPruneResult
    {
        internal SaveUnknownPayloadPruneResult(
            SaveUnknownPayloadPruneStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            SaveGenerationId publishedGenerationId,
            int prunedCount,
            int remainingUnknownCount,
            bool generationPublished,
            bool headPublished,
            bool catalogReconciled,
            bool maintenanceFailed)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            SlotId = slotId;
            SourceGenerationId = sourceGenerationId;
            PublishedGenerationId = publishedGenerationId;
            PrunedCount = prunedCount;
            RemainingUnknownCount = remainingUnknownCount;
            GenerationPublished = generationPublished;
            HeadPublished = headPublished;
            CatalogReconciled = catalogReconciled;
            MaintenanceFailed = maintenanceFailed;
        }

        public SaveUnknownPayloadPruneStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public SaveGenerationId PublishedGenerationId { get; }

        public int PrunedCount { get; }

        public int RemainingUnknownCount { get; }

        public bool GenerationPublished { get; }

        public bool HeadPublished { get; }

        public bool CatalogReconciled { get; }

        public bool MaintenanceFailed { get; }

        public bool Succeeded =>
            Status == SaveUnknownPayloadPruneStatus.Succeeded ||
            Status ==
                SaveUnknownPayloadPruneStatus
                    .PublishedMaintenanceFailed;

        internal static SaveUnknownPayloadPruneResult Failure(
            SaveUnknownPayloadPruneStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default,
            SaveGenerationId publishedGenerationId = default,
            int prunedCount = 0,
            int remainingUnknownCount = 0,
            bool generationPublished = false,
            bool headPublished = false,
            bool catalogReconciled = false,
            bool maintenanceFailed = false) =>
            new SaveUnknownPayloadPruneResult(
                status,
                diagnosticCode,
                message,
                slotId,
                sourceGenerationId,
                publishedGenerationId,
                prunedCount,
                remainingUnknownCount,
                generationPublished,
                headPublished,
                catalogReconciled,
                maintenanceFailed);
    }
}
