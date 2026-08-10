
namespace EchoDevGames.EchoSave
{
    public enum SavePreparedLoadApplyStatus
    {
        Succeeded = 0,
        PreflightRejected = 1,
        RegistryChanged = 2,
        ParticipantFailed = 3,
        ParticipantException = 4,
        HandleUnavailable = 5
    }
}
