
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional additive storage capability for deleting one complete
    /// Chronicle subtree beneath a validated provider-owned storage key.
    ///
    /// The base ISaveStorageBackend contract deliberately remains unchanged.
    /// </summary>
    public interface ISaveStorageTreeDeletionBackend
    {
        SaveStorageResult DeleteTree(
            SaveStorageKey directoryKey);
    }
}
