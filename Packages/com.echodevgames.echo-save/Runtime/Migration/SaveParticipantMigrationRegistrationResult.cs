namespace EchoDevGames.EchoSave
{
    public readonly struct SaveParticipantMigrationRegistrationResult
    {
        internal SaveParticipantMigrationRegistrationResult(
            SaveParticipantMigrationRegistrationStatus status,
            SaveParticipantMigrationRegistration registration,
            string diagnosticCode,
            string message)
        {
            Status =
                status;

            Registration =
                registration;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;
        }

        public SaveParticipantMigrationRegistrationStatus Status { get; }

        public SaveParticipantMigrationRegistration Registration { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveParticipantMigrationRegistrationStatus.Succeeded;
    }
}
