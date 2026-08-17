namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Replaceable project-owned notification presentation boundary.
    /// Implementations reconcile their visuals to each bounded channel
    /// snapshot and must not treat snapshots as diagnostic history.
    /// </summary>
    public interface IUINotificationPresenter
    {
        void ApplyChannel(
            UINotificationPresentationSnapshot snapshot);
    }
}
