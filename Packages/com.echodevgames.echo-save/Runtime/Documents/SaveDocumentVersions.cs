
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Explicit Chronicle-owned package-document format versions.
    ///
    /// Each package document kind evolves independently. Version changes here
    /// describe Chronicle transport/document structure, never project gameplay
    /// schema meaning.
    /// </summary>
    public static class SaveDocumentVersions
    {
        public const int EnvelopeMajor = 1;
        public const int EnvelopeMinor = 0;
        public const int EnvelopeRevision = 0;

        public const int ManifestMajor = 1;
        public const int ManifestMinor = 0;
        public const int ManifestRevision = 0;

        public const int PayloadMajor = 1;
        public const int PayloadMinor = 0;
        public const int PayloadRevision = 0;

        public const int HeadPointerMajor = 1;
        public const int HeadPointerMinor = 0;
        public const int HeadPointerRevision = 0;
    }
}
