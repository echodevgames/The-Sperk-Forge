
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotCatalogRefreshResult
    {
        internal SaveSlotCatalogRefreshResult(
            SaveSlotCatalogRefreshStatus status,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot snapshot,
            bool activeSelectionCleared,
            SaveCatalogCacheMaintenanceStatus cacheMaintenanceStatus =
                SaveCatalogCacheMaintenanceStatus.NotConfigured,
            string cacheDiagnosticCode = "",
            string cacheMessage = "")
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot =
                snapshot ??
                SaveSlotCatalogSnapshot.Empty;
            ActiveSelectionCleared =
                activeSelectionCleared;

            CacheMaintenanceStatus =
                cacheMaintenanceStatus;

            CacheDiagnosticCode =
                cacheDiagnosticCode ?? string.Empty;

            CacheMessage =
                cacheMessage ?? string.Empty;
        }

        public SaveSlotCatalogRefreshStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotCatalogSnapshot Snapshot { get; }

        public bool ActiveSelectionCleared { get; }

        public SaveCatalogCacheMaintenanceStatus
            CacheMaintenanceStatus { get; }

        public string CacheDiagnosticCode { get; }

        public string CacheMessage { get; }

        public bool CacheMaintenanceFailed =>
            CacheMaintenanceStatus ==
            SaveCatalogCacheMaintenanceStatus.RebuildFailed;

        public bool Succeeded =>
            Status == SaveSlotCatalogRefreshStatus.Succeeded ||
            Status == SaveSlotCatalogRefreshStatus.SucceededEmpty ||
            Status == SaveSlotCatalogRefreshStatus.SucceededWithDegradedSlots;
    }
}
