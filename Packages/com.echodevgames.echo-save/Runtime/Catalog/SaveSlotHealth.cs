
namespace EchoDevGames.EchoSave
{
    public enum SaveSlotHealth
    {
        Healthy = 0,
        MissingHead = 1,
        InvalidHead = 2,
        UnsupportedHead = 3,
        MissingManifest = 4,
        InvalidManifest = 5,
        UnsupportedManifest = 6,
        IdentityMismatch = 7,
        BackendReadFailure = 8
    }
}
