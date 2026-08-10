namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Structured output from one explicit participant migration edge.
    ///
    /// SerializerId is intentionally transport text here so Chronicle can
    /// validate migration output rather than trusting a preconstructed ID.
    /// </summary>
    public readonly struct SaveParticipantMigrationStepResult
    {
        public SaveParticipantMigrationStepResult(
            SaveParticipantMigrationStepStatus status,
            int targetSchemaVersion,
            string serializerId,
            string serializedPayload,
            string diagnosticCode,
            string message)
        {
            Status =
                status;

            TargetSchemaVersion =
                targetSchemaVersion;

            SerializerId =
                serializerId ?? string.Empty;

            SerializedPayload =
                serializedPayload;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;
        }

        public SaveParticipantMigrationStepStatus Status { get; }

        public int TargetSchemaVersion { get; }

        public string SerializerId { get; }

        public string SerializedPayload { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveParticipantMigrationStepStatus.Succeeded;

        public static SaveParticipantMigrationStepResult Success(
            int targetSchemaVersion,
            string serializerId,
            string serializedPayload,
            string message =
                "Chronicle participant migration step succeeded.") =>
            new SaveParticipantMigrationStepResult(
                SaveParticipantMigrationStepStatus.Succeeded,
                targetSchemaVersion,
                serializerId,
                serializedPayload,
                string.Empty,
                message);

        public static SaveParticipantMigrationStepResult Failure(
            string diagnosticCode,
            string message) =>
            new SaveParticipantMigrationStepResult(
                SaveParticipantMigrationStepStatus.Failed,
                0,
                string.Empty,
                null,
                diagnosticCode,
                message);
    }
}
