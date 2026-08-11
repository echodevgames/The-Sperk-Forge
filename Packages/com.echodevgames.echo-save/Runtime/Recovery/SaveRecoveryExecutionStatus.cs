
namespace EchoDevGames.EchoSave
{
    public enum SaveRecoveryExecutionStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ServiceNotReady = 2,
        AdmissionClosed = 3,
        Busy = 4,
        RevalidationFailed = 5,
        StalePlan = 6,
        CandidateInvalid = 7,
        BackendUnsupported = 8,
        HeadPublicationFailed = 9,
        CatalogReconciliationFailed = 10
    }
}
