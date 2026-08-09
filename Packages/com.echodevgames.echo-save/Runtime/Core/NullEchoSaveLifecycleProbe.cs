namespace EchoDevGames.EchoSave
{
    internal sealed class NullEchoSaveLifecycleProbe :
        IEchoSaveLifecycleProbe
    {
        internal static readonly
            NullEchoSaveLifecycleProbe Instance =
                new NullEchoSaveLifecycleProbe();

        private NullEchoSaveLifecycleProbe()
        {
        }

        public void OnInitializeAccepted(
            EchoSaveConfiguration configuration)
        {
        }

        public void OnShutdown()
        {
        }
    }
}
