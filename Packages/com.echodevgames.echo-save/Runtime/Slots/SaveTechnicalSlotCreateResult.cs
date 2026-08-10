
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Truthful technical slot-creation result.
    ///
    /// SlotPublished may be true even when catalog reconciliation later fails.
    /// A committed slot is never hidden behind fictional rollback semantics.
    /// </summary>
    internal sealed class SaveTechnicalSlotCreateResult
    {
        internal SaveTechnicalSlotCreateResult(
            SaveTechnicalSlotCreateStatus status,
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
            CatalogReconciled =
                catalogReconciled;
            CreatedEntry = createdEntry;
        }

        internal SaveTechnicalSlotCreateStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId GenerationId { get; }

        internal bool SlotPublished { get; }

        internal bool CatalogReconciled { get; }

        internal SaveSlotCatalogEntry CreatedEntry { get; }

        internal bool Succeeded =>
            Status ==
            SaveTechnicalSlotCreateStatus.Succeeded;
    }
}
