
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Replaceable detached-byte integrity provider.
    ///
    /// Integrity hashes detect accidental corruption. They do not authenticate
    /// saves, prevent deliberate tampering, or provide anti-cheat guarantees.
    /// </summary>
    public interface IIntegrityProvider
    {
        SaveIntegrityProviderId Id { get; }

        SaveIntegrityResult Calculate(
            byte[] data,
            out string checksum);

        SaveIntegrityResult Verify(
            byte[] data,
            string expectedChecksum);
    }
}
