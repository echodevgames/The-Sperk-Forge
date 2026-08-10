
namespace EchoDevGames.EchoSave
{
    internal enum SaveParticipantApplyPlanStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        HandleUnavailable = 2,
        ParticipantUnavailable = 3,
        StateIncompatible = 4,
        DuplicatePreparedParticipant = 5,
        MissingPayloadBlocked = 6,
        DefaultCapabilityMissing = 7
    }
}
