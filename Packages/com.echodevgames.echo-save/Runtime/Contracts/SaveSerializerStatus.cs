
namespace EchoDevGames.EchoSave
{
    public enum SaveSerializerStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        MalformedData = 2,
        UnsupportedDocumentVersion = 3,
        DuplicateProvider = 4,
        ProviderNotFound = 5,
        Failed = 6
    }
}
