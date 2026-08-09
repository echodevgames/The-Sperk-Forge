
namespace EchoDevGames.EchoSave
{
    internal interface IEchoSaveStorageBackendFactory
    {
        SaveStorageResult TryCreate(
            EchoSaveConfiguration configuration,
            out ISaveStorageBackend backend);
    }
}
