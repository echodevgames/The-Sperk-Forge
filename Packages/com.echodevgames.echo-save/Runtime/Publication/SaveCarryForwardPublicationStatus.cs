namespace EchoDevGames.EchoSave
{
    internal enum SaveCarryForwardPublicationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        MissingProvenance = 2,
        SlotMismatch = 3,
        SourceUnavailable = 4,
        SourceInvalid = 5,
        StaleSource = 6,
        OwnershipCollision = 7,
        MergeInvalid = 8,
        PublicationFailed = 9,
        Canceled = 10
    }
}
