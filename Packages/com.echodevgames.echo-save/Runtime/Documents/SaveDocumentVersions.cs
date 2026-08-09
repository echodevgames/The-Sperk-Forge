
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Explicit Chronicle-owned package-document format versions.
    ///
    /// M2-02 begins with the generic envelope document only. Later bounded
    /// checkpoints may add independently versioned package document kinds.
    /// </summary>
    public static class SaveDocumentVersions
    {
        public const int EnvelopeMajor = 1;
        public const int EnvelopeMinor = 0;
        public const int EnvelopeRevision = 0;
    }
}
