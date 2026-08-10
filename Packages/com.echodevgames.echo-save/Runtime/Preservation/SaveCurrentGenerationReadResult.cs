
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveCurrentGenerationReadResult
    {
        internal SaveCurrentGenerationReadResult(
            SaveCurrentGenerationReadStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            int knownParticipantCount,
            int unknownParticipantCount)
            : this(
                status,
                diagnosticCode,
                message,
                slotId,
                generationId,
                knownParticipantCount,
                unknownParticipantCount,
                null)
        {
        }

        internal SaveCurrentGenerationReadResult(
            SaveCurrentGenerationReadStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            int knownParticipantCount,
            int unknownParticipantCount,
            SaveValidatedParticipantSnapshot validatedParticipants)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            SlotId =
                slotId;

            GenerationId =
                generationId;

            KnownParticipantCount =
                knownParticipantCount;

            UnknownParticipantCount =
                unknownParticipantCount;

            ValidatedParticipants =
                validatedParticipants;
        }

        internal SaveCurrentGenerationReadStatus Status
        {
            get;
        }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveSlotId SlotId { get; }

        internal SaveGenerationId GenerationId
        {
            get;
        }

        internal int KnownParticipantCount { get; }

        internal int UnknownParticipantCount { get; }

        internal SaveValidatedParticipantSnapshot
            ValidatedParticipants
        {
            get;
        }

        internal bool Succeeded =>
            Status ==
            SaveCurrentGenerationReadStatus.Succeeded;
    }
}
