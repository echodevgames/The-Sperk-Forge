
namespace EchoDevGames.EchoSave
{
    public sealed class SaveActiveSlotSelectionResult
    {
        internal SaveActiveSlotSelectionResult(
            SaveActiveSlotSelectionStatus status,
            bool hasActiveSlot,
            SaveSlotId activeSlotId,
            string diagnosticCode,
            string message)
        {
            Status = status;
            HasActiveSlot = hasActiveSlot;
            ActiveSlotId = activeSlotId;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public SaveActiveSlotSelectionStatus Status { get; }

        public bool HasActiveSlot { get; }

        public SaveSlotId ActiveSlotId { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status != SaveActiveSlotSelectionStatus.Rejected;
    }
}
