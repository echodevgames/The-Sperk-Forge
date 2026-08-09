
namespace EchoDevGames.EchoSave
{
    internal enum SaveCurrentGenerationReadStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        HeadUnavailable = 2,
        HeadInvalid = 3,
        GenerationUnavailable = 4,
        GenerationInvalid = 5,
        UnknownPayloadRejected = 6
    }
}
