namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Provider identity seam for future storage implementations.
    ///
    /// ESV-M1-01 intentionally defines no read/write methods.
    /// </summary>
    public interface ISaveStorageBackend
    {
        SaveStorageBackendId Id { get; }
    }
}
