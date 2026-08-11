namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-owned deterministic migration edge for one Chronicle document kind.
    /// </summary>
    internal interface ISavePackageDocumentMigrationStep
    {
        string StepId { get; }

        string DocumentKind { get; }

        SavePackageDocumentVersion SourceVersion { get; }

        SavePackageDocumentVersion TargetVersion { get; }

        SavePackageDocumentMigrationStepResult Migrate(
            string serializedDocument);
    }

    internal readonly struct SavePackageDocumentMigrationStepResult
    {
        private SavePackageDocumentMigrationStepResult(
            bool succeeded,
            string serializedDocument,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            SerializedDocument = serializedDocument;
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
        }

        internal bool Succeeded { get; }

        internal string SerializedDocument { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal static SavePackageDocumentMigrationStepResult Success(
            string serializedDocument) =>
            new SavePackageDocumentMigrationStepResult(
                true,
                serializedDocument,
                string.Empty,
                "Chronicle package-document migration step succeeded.");

        internal static SavePackageDocumentMigrationStepResult Failure(
            string diagnosticCode,
            string message) =>
            new SavePackageDocumentMigrationStepResult(
                false,
                null,
                diagnosticCode,
                message);
    }
}
