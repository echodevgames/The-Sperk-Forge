
namespace EchoDevGames.EchoSave
{
    internal interface ISaveRecoveryExecutor
    {
        SaveRecoveryResult Execute(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate);
    }
}
