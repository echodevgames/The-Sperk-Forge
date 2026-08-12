
namespace EchoDevGames.EchoSave
{
    public enum SaveUnknownPayloadPruneStatus
    {
        Succeeded = 0,
        ServiceNotReady = 1,
        AdmissionClosed = 2,
        Busy = 3,
        InvalidPlan = 4,
        ForeignSession = 5,
        Consumed = 6,
        Expired = 7,
        SourceStale = 8,
        SourceInvalid = 9,
        RequestedIdClaimed = 10,
        PublicationFailed = 11,
        HeadPublicationFailed = 12,
        PublishedSessionReconciliationFailed = 13,
        PublishedCatalogReconciliationFailed = 14,
        PublishedMaintenanceFailed = 15
    }
}
