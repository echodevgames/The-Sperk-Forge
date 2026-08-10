
namespace EchoDevGames.EchoSave
{
    internal enum SaveUnknownPayloadMergeStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        MissingProvenance = 2,
        FreshCaptureInvalid = 3,
        UnknownPayloadInvalid = 4,
        OwnershipCollision = 5,
        MergeInvalid = 6
    }
}
