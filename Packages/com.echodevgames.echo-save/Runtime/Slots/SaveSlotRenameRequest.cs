
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotRenameRequest
    {
        public SaveSlotRenameRequest(
            SaveSlotId slotId,
            string displayName)
        {
            SlotId = slotId;
            DisplayName = displayName;
        }

        public SaveSlotId SlotId { get; }
        public string DisplayName { get; }
    }
}
