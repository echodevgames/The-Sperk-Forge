
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Replaceable Chronicle storage-provider contract.
    ///
    /// ESV-M2-01 establishes safe byte-oriented local storage primitives only.
    /// Chronicle save documents, slots, generations, and participant payloads
    /// are deliberately outside this contract.
    /// </summary>
    public interface ISaveStorageBackend
    {
        SaveStorageBackendId Id { get; }

        string RootPath { get; }

        SaveStorageResult Initialize();

        SaveStorageResult Exists(
            SaveStorageKey key,
            out bool exists);

        SaveStorageReadResult Read(
            SaveStorageKey key);

        SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data);

        SaveStorageResult Delete(
            SaveStorageKey key);

        SaveStorageResult Shutdown();
    }
}
