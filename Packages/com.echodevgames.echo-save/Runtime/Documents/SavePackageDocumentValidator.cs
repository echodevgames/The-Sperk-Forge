
namespace EchoDevGames.EchoSave
{
    internal static class SavePackageDocumentValidator
    {
        internal static SaveSerializerResult
            ValidateCurrent(
                ISavePackageDocument document)
        {
            if (document == null)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "A Chronicle package document is required.");
            }

            if (!TryGetCurrentVersion(
                    document.DocumentKind,
                    out int expectedMajor,
                    out int expectedMinor,
                    out int expectedRevision))
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidDocument,
                    "The Chronicle package document kind is unsupported.");
            }

            if (document.FormatMajor != expectedMajor ||
                document.FormatMinor != expectedMinor ||
                document.FormatRevision != expectedRevision)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus
                        .UnsupportedDocumentVersion,
                    EchoSaveDiagnosticCodes
                        .SerializerUnsupportedDocumentVersion,
                    "The Chronicle package document version is unsupported by this runtime.");
            }

            return SaveSerializerResult.Success(
                "The Chronicle package document version is supported.");
        }

        private static bool TryGetCurrentVersion(
            string documentKind,
            out int major,
            out int minor,
            out int revision)
        {
            switch (documentKind)
            {
                case SaveDocumentKinds.Envelope:
                    major =
                        SaveDocumentVersions.EnvelopeMajor;
                    minor =
                        SaveDocumentVersions.EnvelopeMinor;
                    revision =
                        SaveDocumentVersions.EnvelopeRevision;
                    return true;

                case SaveDocumentKinds.Manifest:
                    major =
                        SaveDocumentVersions.ManifestMajor;
                    minor =
                        SaveDocumentVersions.ManifestMinor;
                    revision =
                        SaveDocumentVersions.ManifestRevision;
                    return true;

                case SaveDocumentKinds.Payload:
                    major =
                        SaveDocumentVersions.PayloadMajor;
                    minor =
                        SaveDocumentVersions.PayloadMinor;
                    revision =
                        SaveDocumentVersions.PayloadRevision;
                    return true;

                case SaveDocumentKinds.HeadPointer:
                    major =
                        SaveDocumentVersions.HeadPointerMajor;
                    minor =
                        SaveDocumentVersions.HeadPointerMinor;
                    revision =
                        SaveDocumentVersions.HeadPointerRevision;
                    return true;

                default:
                    major = 0;
                    minor = 0;
                    revision = 0;
                    return false;
            }
        }
    }
}
