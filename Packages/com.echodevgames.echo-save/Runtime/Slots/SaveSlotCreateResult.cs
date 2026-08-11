
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public truthful slot-creation result. SlotPublished may remain true when
    /// later catalog reconciliation fails; Chronicle never fabricates rollback.
    /// </summary>
    public sealed class SaveSlotCreateResult
    {
        internal SaveSlotCreateResult(
            SaveSlotCreateStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            bool slotPublished,
            bool catalogReconciled,
            SaveSlotCatalogEntry createdEntry)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            SlotId = slotId;
            GenerationId = generationId;
            SlotPublished = slotPublished;
            CatalogReconciled = catalogReconciled;
            CreatedEntry = createdEntry;
        }

        public SaveSlotCreateStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId GenerationId { get; }

        public bool SlotPublished { get; }

        public bool CatalogReconciled { get; }

        public SaveSlotCatalogEntry CreatedEntry { get; }

        public bool Succeeded =>
            Status == SaveSlotCreateStatus.Succeeded;
    }
}
