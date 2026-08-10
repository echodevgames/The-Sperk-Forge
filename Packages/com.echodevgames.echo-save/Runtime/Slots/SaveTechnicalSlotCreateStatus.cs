
namespace EchoDevGames.EchoSave
{
    internal enum SaveTechnicalSlotCreateStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        CatalogUnavailable = 2,
        CapacityReached = 3,
        SlotIdGenerationFailed = 4,
        SlotIdCollisionLimitExceeded = 5,
        PublicationFailed = 6,
        PublishedCatalogReconciliationFailed = 7
    }
}
