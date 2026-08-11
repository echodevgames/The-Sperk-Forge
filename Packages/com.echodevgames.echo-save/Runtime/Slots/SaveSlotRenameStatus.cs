
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotRenameStatus
    {
        Succeeded = 0,
        NoChange = 1,
        InvalidRequest = 2,
        ServiceNotReady = 3,
        AdmissionClosed = 4,
        Busy = 5,
        CatalogUnavailable = 6,
        SlotNotFound = 7,
        SourceInvalid = 8,
        SourceStale = 9,
        PublicationFailed = 10,
        PublishedRetentionMaintenanceFailed = 11,
        PublishedCatalogReconciliationFailed = 12
    }
}
