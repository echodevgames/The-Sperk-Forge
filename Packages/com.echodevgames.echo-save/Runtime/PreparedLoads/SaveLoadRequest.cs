
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public same-scene load request targeting one explicit technical slot.
    /// Recovery remains a separate explicit operation.
    /// </summary>
    public readonly struct SaveLoadRequest
    {
        public SaveLoadRequest(
            SaveSlotId slotId)
        {
            SlotId = slotId;
        }

        public SaveSlotId SlotId { get; }
    }
}
