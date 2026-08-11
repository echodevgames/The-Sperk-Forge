
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotCatalogRefreshStatus
    {
        Succeeded = 0,
        SucceededEmpty = 1,
        SucceededWithDegradedSlots = 2,
        DiscoveryUnavailable = 3,
        DiscoveryFailed = 4,
        ScanLimitExceeded = 5,
        ServiceNotReady = 6,
        AdmissionClosed = 7,
        Busy = 8
    }
}
