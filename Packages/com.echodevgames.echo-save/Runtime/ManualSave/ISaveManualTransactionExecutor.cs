namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-local execution seam used by the M4-04 public facade.
    /// Production resolves to SaveManualTransactionCoordinator; tests may use
    /// a deterministic fake without replacing durable publication authority.
    /// </summary>
    internal interface ISaveManualTransactionExecutor
    {
        SaveManualTransactionResult Save(
            SaveManualTransactionRequest request,
            SaveManualTransactionControl control);
    }
}
