
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional generic storage publication capability.
    ///
    /// Chronicle generation semantics remain outside the provider. Providers
    /// only understand storage trees and one small current-object replacement.
    /// </summary>
    public interface ISaveStoragePublicationBackend :
        ISaveStorageBackend
    {
        SaveStoragePublicationCapabilities
            PublicationCapabilities { get; }

        SaveStorageResult PublishNewTree(
            SaveStorageKey sourceDirectoryKey,
            SaveStorageKey destinationDirectoryKey);

        SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data);
    }
}
