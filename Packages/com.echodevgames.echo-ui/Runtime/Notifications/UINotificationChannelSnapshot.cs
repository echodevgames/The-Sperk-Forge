namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Side-effect-free bounded state for one notification channel.
    /// Presentation payloads and visible text are deliberately excluded.
    /// </summary>
    public readonly struct UINotificationChannelSnapshot
    {
        public UINotificationChannelSnapshot(
            UINotificationChannelId channelId,
            int visibleCapacity,
            int pendingCapacity,
            int visibleCount,
            int pendingCount,
            UINotificationOverflowPolicy overflowPolicy)
        {
            ChannelId = channelId;
            VisibleCapacity = visibleCapacity;
            PendingCapacity = pendingCapacity;
            VisibleCount = visibleCount;
            PendingCount = pendingCount;
            OverflowPolicy = overflowPolicy;
        }

        public UINotificationChannelId ChannelId { get; }

        public int VisibleCapacity { get; }

        public int PendingCapacity { get; }

        public int VisibleCount { get; }

        public int PendingCount { get; }

        public UINotificationOverflowPolicy OverflowPolicy { get; }
    }
}
