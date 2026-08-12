
namespace EchoDevGames.EchoSave
{
    public enum SaveUnknownPayloadPrunePlanStatus
    {
        Ready = 0,
        ServiceNotReady = 1,
        InvalidRequest = 2,
        SourceUnavailable = 3,
        SourceInvalid = 4,
        RequestedIdNotFound = 5,
        RequestedIdClaimed = 6
    }
}
