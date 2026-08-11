
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotCreateStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ServiceNotReady = 2,
        AdmissionClosed = 3,
        Busy = 4,
        CatalogUnavailable = 5,
        CapacityReached = 6,
        SlotIdGenerationFailed = 7,
        SlotIdCollisionLimitExceeded = 8,
        PublicationFailed = 9,
        PublishedCatalogReconciliationFailed = 10
    }
}
