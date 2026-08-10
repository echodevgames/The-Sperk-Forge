namespace EchoDevGames.EchoSave
{
    internal enum SaveManualTransactionStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        CatalogUnavailable = 2,
        NoActiveSlot = 3,
        ActiveSlotUnavailable = 4,
        SourceReadFailed = 5,
        SourceChanged = 6,
        CaptureFailed = 7,
        CarryForwardFailed = 8,
        StaleSource = 9,
        PublicationFailed = 10,
        PublishedCatalogReconciliationFailed = 11,
        Canceled = 12
    }
}
