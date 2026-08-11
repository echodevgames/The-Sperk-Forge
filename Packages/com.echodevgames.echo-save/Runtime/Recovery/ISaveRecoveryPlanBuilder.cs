
namespace EchoDevGames.EchoSave
{
    internal interface ISaveRecoveryPlanBuilder
    {
        SaveRecoveryPlan Build(
            SaveSlotId slotId);
    }
}
