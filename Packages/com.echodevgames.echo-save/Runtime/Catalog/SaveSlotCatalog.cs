
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-owned derived slot catalog plus session-only active selection.
    /// </summary>
    public sealed class SaveSlotCatalog
    {
        private readonly SaveSlotCatalogScanner scanner;
        private readonly SaveCatalogCacheCoordinator cacheCoordinator;
        private readonly SaveActiveSlotSession activeSession =
            new SaveActiveSlotSession();

        private SaveSlotCatalogSnapshot snapshot =
            SaveSlotCatalogSnapshot.Empty;

        public SaveSlotCatalog(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            int maxScanEntries)
            : this(
                storageBackend,
                serializer,
                maxScanEntries,
                false)
        {
        }

        internal SaveSlotCatalog(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            int maxScanEntries,
            bool enablePersistentCache)
        {
            scanner =
                new SaveSlotCatalogScanner(
                    storageBackend,
                    serializer,
                    maxScanEntries);

            if (enablePersistentCache)
            {
                cacheCoordinator =
                    new SaveCatalogCacheCoordinator(
                        storageBackend,
                        serializer,
                        maxScanEntries);
            }
        }

        public SaveSlotCatalogSnapshot Snapshot =>
            snapshot;

        public bool HasActiveSlot =>
            activeSession.HasActiveSlot;

        public SaveSlotId ActiveSlotId =>
            activeSession.ActiveSlotId;

        public SaveSlotCatalogRefreshResult Refresh()
        {
            SaveSlotCatalogRefreshResult scan =
                scanner.Scan();

            if (!scan.Succeeded)
            {
                return new SaveSlotCatalogRefreshResult(
                    scan.Status,
                    scan.DiagnosticCode,
                    scan.Message,
                    snapshot,
                    false);
            }

            snapshot =
                scan.Snapshot;

            bool cleared =
                activeSession.Reconcile(
                    snapshot);

            if (cacheCoordinator == null)
            {
                return new SaveSlotCatalogRefreshResult(
                    scan.Status,
                    scan.DiagnosticCode,
                    scan.Message,
                    snapshot,
                    cleared);
            }

            SaveCatalogCachePreview cache =
                cacheCoordinator.Inspect(
                    snapshot);

            if (cache.State ==
                SaveCatalogCacheState.Valid)
            {
                return new SaveSlotCatalogRefreshResult(
                    scan.Status,
                    scan.DiagnosticCode,
                    scan.Message,
                    snapshot,
                    cleared,
                    SaveCatalogCacheMaintenanceStatus.Valid,
                    cache.DiagnosticCode,
                    cache.Message);
            }

            SaveCatalogCacheRebuildResult rebuild =
                cacheCoordinator.RebuildFromSnapshot(
                    snapshot);

            return new SaveSlotCatalogRefreshResult(
                scan.Status,
                scan.DiagnosticCode,
                scan.Message,
                snapshot,
                cleared,
                rebuild.Succeeded
                    ? SaveCatalogCacheMaintenanceStatus.Rebuilt
                    : SaveCatalogCacheMaintenanceStatus.RebuildFailed,
                rebuild.DiagnosticCode,
                rebuild.Message);
        }

        internal SaveCatalogCachePreview
            PreviewPersistentCache() =>
            cacheCoordinator == null
                ? new SaveCatalogCachePreview(
                    SaveCatalogCacheState.BackendUnsupported,
                    "ECHOSAVE-CACHE-NOT-CONFIGURED",
                    "This Chronicle catalog instance does not own persistent-cache maintenance.",
                    snapshot,
                    0,
                    string.Empty,
                    string.Empty,
                    false)
                : cacheCoordinator.Preview();

        internal SaveCatalogCacheRebuildResult
            RebuildPersistentCache() =>
            cacheCoordinator == null
                ? new SaveCatalogCacheRebuildResult(
                    false,
                    SaveCatalogCacheState.BackendUnsupported,
                    "ECHOSAVE-CACHE-NOT-CONFIGURED",
                    "This Chronicle catalog instance does not own persistent-cache maintenance.",
                    snapshot,
                    string.Empty)
                : cacheCoordinator.Rebuild();

        public SaveActiveSlotSelectionResult SelectActiveSlot(
            SaveSlotId slotId) =>
            activeSession.Select(
                snapshot,
                slotId);

        public SaveActiveSlotSelectionResult ClearActiveSlot() =>
            activeSession.Clear();
    }
}
