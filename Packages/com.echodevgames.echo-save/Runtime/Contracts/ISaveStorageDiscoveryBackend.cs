
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional additive read-only storage capability for discovering immediate
    /// child directories beneath one validated Chronicle storage key.
    ///
    /// The base ISaveStorageBackend contract deliberately remains unchanged.
    /// </summary>
    public interface ISaveStorageDiscoveryBackend
    {
        SaveStorageDiscoveryResult DiscoverChildDirectories(
            SaveStorageKey parentKey,
            int maxChildren);
    }
}
