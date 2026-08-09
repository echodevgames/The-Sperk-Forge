
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

            if (document.DocumentKind !=
                SaveDocumentKinds.Envelope)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidDocument,
                    "The Chronicle package document kind is unsupported.");
            }

            if (document.FormatMajor !=
                    SaveDocumentVersions.EnvelopeMajor ||
                document.FormatMinor !=
                    SaveDocumentVersions.EnvelopeMinor ||
                document.FormatRevision !=
                    SaveDocumentVersions.EnvelopeRevision)
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
    }
}
