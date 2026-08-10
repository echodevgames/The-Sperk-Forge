namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public terminal status for one M4-04 manual-save request.
    /// Cancellation that arrives after durable publication begins is reported
    /// separately through SaveCancellationDisposition.TooLate.
    /// </summary>
    public enum SaveOperationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ServiceNotReady = 2,
        AdmissionClosed = 3,
        Busy = 4,
        Canceled = 5,
        TransactionFailed = 6,
        PublishedCatalogReconciliationFailed = 7
    }
}
