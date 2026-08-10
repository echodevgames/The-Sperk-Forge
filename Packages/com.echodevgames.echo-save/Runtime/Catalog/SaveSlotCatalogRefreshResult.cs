
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotCatalogRefreshResult
    {
        internal SaveSlotCatalogRefreshResult(
            SaveSlotCatalogRefreshStatus status,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot snapshot,
            bool activeSelectionCleared)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot =
                snapshot ??
                SaveSlotCatalogSnapshot.Empty;
            ActiveSelectionCleared =
                activeSelectionCleared;
        }

        public SaveSlotCatalogRefreshStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotCatalogSnapshot Snapshot { get; }

        public bool ActiveSelectionCleared { get; }

        public bool Succeeded =>
            Status == SaveSlotCatalogRefreshStatus.Succeeded ||
            Status == SaveSlotCatalogRefreshStatus.SucceededEmpty ||
            Status == SaveSlotCatalogRefreshStatus.SucceededWithDegradedSlots;
    }
}
