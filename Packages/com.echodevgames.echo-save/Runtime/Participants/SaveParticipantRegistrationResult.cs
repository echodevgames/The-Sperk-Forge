
namespace EchoDevGames.EchoSave
{
    public readonly struct SaveParticipantRegistrationResult
    {
        internal SaveParticipantRegistrationResult(
            SaveParticipantRegistrationStatus status,
            SaveParticipantRegistration registration,
            string diagnosticCode,
            string message)
        {
            Status = status;
            Registration = registration;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveParticipantRegistrationStatus
            Status
        {
            get;
        }

        public SaveParticipantRegistration
            Registration
        {
            get;
        }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveParticipantRegistrationStatus.Succeeded &&
            Registration != null;
    }
}
