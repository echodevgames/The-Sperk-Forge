namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Internal deterministic seam used to prove that duplicate roots and
    /// invalid configuration paths reach no Chronicle initialization side
    /// effects. It is not a storage provider.
    /// </summary>
    internal interface IEchoSaveLifecycleProbe
    {
        void OnInitializeAccepted(
            EchoSaveConfiguration configuration);

        void OnShutdown();
    }
}
