
using System;

namespace EchoDevGames.EchoSave
{
    [Serializable]
    public sealed class SavePayloadDocument :
        ISavePackageDocument
    {
        public string documentKind =
            SaveDocumentKinds.Payload;

        public int formatMajor =
            SaveDocumentVersions.PayloadMajor;

        public int formatMinor =
            SaveDocumentVersions.PayloadMinor;

        public int formatRevision =
            SaveDocumentVersions.PayloadRevision;

        public string slotId =
            string.Empty;

        public string generationId =
            string.Empty;

        public SavePayloadEntry[] entries =
            Array.Empty<SavePayloadEntry>();

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
