
namespace EchoDevGames.EchoSave
{
    public enum PreparedLoadCreationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        SourceProvenanceMismatch = 2,
        UnknownPayloadProvenanceMismatch = 3,
        CountLimitExceeded = 4,
        ByteLimitExceeded = 5,
        OwnerUnavailable = 6,
        ServiceNotReady = 7,
        AdmissionClosed = 8,
        Busy = 9,
        SourceUnavailable = 10,
        SourceInvalid = 11,
        ParticipantPreparationFailed = 12
    }
}
