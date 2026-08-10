
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-owned derived slot catalog plus session-only active selection.
    /// </summary>
    public sealed class SaveSlotCatalog
    {
        private readonly SaveSlotCatalogScanner scanner;
        private readonly SaveActiveSlotSession activeSession =
            new SaveActiveSlotSession();

        private SaveSlotCatalogSnapshot snapshot =
            SaveSlotCatalogSnapshot.Empty;

        public SaveSlotCatalog(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            int maxScanEntries)
        {
            scanner =
                new SaveSlotCatalogScanner(
                    storageBackend,
                    serializer,
                    maxScanEntries);
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

            return new SaveSlotCatalogRefreshResult(
                scan.Status,
                scan.DiagnosticCode,
                scan.Message,
                snapshot,
                cleared);
        }

        public SaveActiveSlotSelectionResult SelectActiveSlot(
            SaveSlotId slotId) =>
            activeSession.Select(
                snapshot,
                slotId);

        public SaveActiveSlotSelectionResult ClearActiveSlot() =>
            activeSession.Clear();
    }
}
