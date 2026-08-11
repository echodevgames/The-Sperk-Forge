namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Editor-facing result for one read-only Browser refresh.
    /// </summary>
    public sealed class EchoSaveBrowserRefreshResult
    {
        internal EchoSaveBrowserRefreshResult(
            EchoSaveInspectionOpenResult openResult,
            SaveSlotCatalogRefreshResult catalogResult,
            SaveMigrationGraphSnapshot migrationGraph)
        {
            OpenResult = openResult;
            CatalogResult = catalogResult;
            MigrationGraph = migrationGraph;
        }

        public EchoSaveInspectionOpenResult OpenResult { get; }

        public SaveSlotCatalogRefreshResult CatalogResult { get; }

        public SaveMigrationGraphSnapshot MigrationGraph { get; }

        public bool Succeeded =>
            OpenResult != null &&
            OpenResult.Succeeded &&
            CatalogResult != null &&
            CatalogResult.Succeeded;
    }
}
