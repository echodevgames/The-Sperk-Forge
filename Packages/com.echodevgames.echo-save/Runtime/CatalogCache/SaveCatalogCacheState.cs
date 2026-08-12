
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Derived cache condition relative to current bounded durable catalog truth.
    /// </summary>
    public enum SaveCatalogCacheState
    {
        Missing = 0,
        Valid = 1,
        Stale = 2,
        Corrupt = 3,
        Incompatible = 4,
        BackendUnsupported = 5,
        DurableCatalogUnavailable = 6
    }
}
