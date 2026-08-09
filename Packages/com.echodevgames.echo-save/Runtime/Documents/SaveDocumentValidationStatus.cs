
namespace EchoDevGames.EchoSave
{
    public enum SaveDocumentValidationStatus
    {
        Succeeded = 0,
        InvalidDocument = 1,
        IdentityMismatch = 2,
        PayloadLengthMismatch = 3,
        UnsupportedIntegrityProvider = 4,
        IntegrityMismatch = 5,
        InventoryMismatch = 6
    }
}
