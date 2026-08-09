
using System;

namespace EchoDevGames.EchoSave
{
    [Serializable]
    public sealed class SaveHeadPointer :
        ISavePackageDocument
    {
        public string documentKind =
            SaveDocumentKinds.HeadPointer;

        public int formatMajor =
            SaveDocumentVersions.HeadPointerMajor;

        public int formatMinor =
            SaveDocumentVersions.HeadPointerMinor;

        public int formatRevision =
            SaveDocumentVersions.HeadPointerRevision;

        public string slotId =
            string.Empty;

        public string currentGenerationId =
            string.Empty;

        public string previousGenerationId =
            string.Empty;

        public long updateSequence;

        public string checksum =
            string.Empty;

        public string DocumentKind =>
            documentKind ?? string.Empty;

        public int FormatMajor =>
            formatMajor;

        public int FormatMinor =>
            formatMinor;

        public int FormatRevision =>
            formatRevision;
    }
}
