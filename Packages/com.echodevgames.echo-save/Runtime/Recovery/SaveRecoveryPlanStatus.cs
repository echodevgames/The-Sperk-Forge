
namespace EchoDevGames.EchoSave
{
    public enum SaveRecoveryPlanStatus
    {
        RecoveryNotRequired = 0,
        RecoveryAvailable = 1,
        NoValidCandidate = 2,
        InvalidRequest = 3,
        ServiceNotReady = 4,
        InspectionFailed = 5,
        DiscoveryFailed = 6
    }
}
