
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotDuplicateStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ServiceNotReady = 2,
        AdmissionClosed = 3,
        Busy = 4,
        CatalogUnavailable = 5,
        SlotNotFound = 6,
        SourceInvalid = 7,
        CapacityReached = 8,
        SlotIdGenerationFailed = 9,
        SlotIdCollisionLimitExceeded = 10,
        SourceStale = 11,
        PublicationFailed = 12,
        PublishedCatalogReconciliationFailed = 13
    }
}
