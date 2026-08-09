namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Provider identity seam for future serializer implementations.
    ///
    /// ESV-M1-01 intentionally defines no serialization methods.
    /// </summary>
    public interface ISaveSerializer
    {
        SaveSerializerId Id { get; }
    }
}
