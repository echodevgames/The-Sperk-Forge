
namespace EchoDevGames.EchoSave
{
    internal interface ISaveGenerationRetentionExecutor
    {
        SaveRetentionResult Apply(
            SaveSlotId slotId,
            SaveRetentionPolicy policy);
    }
}
