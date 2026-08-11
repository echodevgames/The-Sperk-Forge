
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotDeleteStatus
    {
        Succeeded = 0,
        InvalidPlan = 1,
        ServiceNotReady = 2,
        AdmissionClosed = 3,
        Busy = 4,
        ForeignSession = 5,
        Expired = 6,
        Consumed = 7,
        SourceStale = 8,
        SourceInvalid = 9,
        BackendUnsupported = 10,
        TrashPublicationFailed = 11,
        PublishedCatalogReconciliationFailed = 12,
        PublishedTrashRetentionFailed = 13
    }
}
