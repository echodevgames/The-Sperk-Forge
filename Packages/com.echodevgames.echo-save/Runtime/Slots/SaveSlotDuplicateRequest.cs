
namespace EchoDevGames.EchoSave
{
    public sealed class SaveSlotDuplicateRequest
    {
        public SaveSlotDuplicateRequest(
            SaveSlotId sourceSlotId)
        {
            SourceSlotId = sourceSlotId;
        }

        public SaveSlotId SourceSlotId { get; }
    }
}
