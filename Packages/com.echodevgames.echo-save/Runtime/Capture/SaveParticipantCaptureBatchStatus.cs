
namespace EchoDevGames.EchoSave
{
    internal enum SaveParticipantCaptureBatchStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ParticipantUnavailable = 2,
        CaptureFailed = 3,
        DetachedStateInvalid = 4,
        SerializerUnavailable = 5,
        SerializationFailed = 6,
        IntegrityFailed = 7
    }
}
