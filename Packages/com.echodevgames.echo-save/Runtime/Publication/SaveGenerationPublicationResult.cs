
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveGenerationPublicationResult
    {
        internal SaveGenerationPublicationResult(
            SaveGenerationPublicationStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            bool generationPublished,
            bool headPublished)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
            SlotId = slotId;
            GenerationId = generationId;
            GenerationPublished =
                generationPublished;
            HeadPublished =
                headPublished;
        }

        internal SaveGenerationPublicationStatus Status
        {
            get;
        }

        internal string DiagnosticCode
        {
            get;
        }

        internal string Message
        {
            get;
        }

        internal SaveSlotId SlotId
        {
            get;
        }

        internal SaveGenerationId GenerationId
        {
            get;
        }

        internal bool GenerationPublished
        {
            get;
        }

        internal bool HeadPublished
        {
            get;
        }

        internal bool Succeeded =>
            Status ==
            SaveGenerationPublicationStatus.Succeeded;
    }
}
