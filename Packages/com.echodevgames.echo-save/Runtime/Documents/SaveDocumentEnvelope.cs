
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Small package-owned envelope DTO used to prove Chronicle document
    /// identity, explicit format versions, serializer identity, and detached
    /// in-memory serialization.
    ///
    /// It intentionally contains no project gameplay payload schema.
    /// </summary>
    [Serializable]
    public sealed class SaveDocumentEnvelope :
        ISavePackageDocument
    {
        public string documentKind =
            SaveDocumentKinds.Envelope;

        public int formatMajor =
            SaveDocumentVersions.EnvelopeMajor;

        public int formatMinor =
            SaveDocumentVersions.EnvelopeMinor;

        public int formatRevision =
            SaveDocumentVersions.EnvelopeRevision;

        public string serializerId =
            UnityJsonSaveSerializer.StableId;

        public string documentId =
            string.Empty;

        public string technicalTimestampUtc =
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
