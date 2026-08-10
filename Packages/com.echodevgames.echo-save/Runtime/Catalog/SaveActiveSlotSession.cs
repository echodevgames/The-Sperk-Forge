
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Session-only active slot selection. No durable writes are owned here.
    /// </summary>
    internal sealed class SaveActiveSlotSession
    {
        private bool hasActiveSlot;
        private SaveSlotId activeSlotId;

        internal bool HasActiveSlot =>
            hasActiveSlot;

        internal SaveSlotId ActiveSlotId =>
            activeSlotId;

        internal SaveActiveSlotSelectionResult Select(
            SaveSlotCatalogSnapshot snapshot,
            SaveSlotId slotId)
        {
            if (snapshot == null ||
                !SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validated) ||
                !snapshot.TryGetEntry(
                    validated,
                    out SaveSlotCatalogEntry entry) ||
                !entry.IsSelectable)
            {
                return Result(
                    SaveActiveSlotSelectionStatus.Rejected,
                    EchoSaveDiagnosticCodes.CatalogActiveSlotRejected,
                    "The requested Chronicle slot is not currently known and selectable.");
            }

            if (hasActiveSlot &&
                activeSlotId == validated)
            {
                return Result(
                    SaveActiveSlotSelectionStatus.NoChange,
                    string.Empty,
                    "The requested Chronicle slot is already active.");
            }

            hasActiveSlot = true;
            activeSlotId = validated;

            return Result(
                SaveActiveSlotSelectionStatus.Selected,
                string.Empty,
                "The Chronicle session active slot was selected.");
        }

        internal SaveActiveSlotSelectionResult Clear()
        {
            if (!hasActiveSlot)
            {
                return Result(
                    SaveActiveSlotSelectionStatus.NoChange,
                    string.Empty,
                    "The Chronicle session already has no active slot.");
            }

            hasActiveSlot = false;
            activeSlotId = default;

            return Result(
                SaveActiveSlotSelectionStatus.Cleared,
                string.Empty,
                "The Chronicle session active slot was cleared.");
        }

        internal bool Reconcile(
            SaveSlotCatalogSnapshot snapshot)
        {
            if (!hasActiveSlot)
            {
                return false;
            }

            if (snapshot != null &&
                snapshot.TryGetEntry(
                    activeSlotId,
                    out SaveSlotCatalogEntry entry) &&
                entry.IsSelectable)
            {
                return false;
            }

            hasActiveSlot = false;
            activeSlotId = default;

            return true;
        }

        private SaveActiveSlotSelectionResult Result(
            SaveActiveSlotSelectionStatus status,
            string diagnosticCode,
            string message) =>
            new SaveActiveSlotSelectionResult(
                status,
                hasActiveSlot,
                activeSlotId,
                diagnosticCode,
                message);
    }
}
