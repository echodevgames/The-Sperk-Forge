using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// One live visible generation supplied only to the presentation seam.
    /// Unlike status and diagnostic snapshots, this value intentionally
    /// carries the opaque project-owned presentation payload.
    /// </summary>
    public readonly struct UINotificationPresentationEntry
    {
        internal UINotificationPresentationEntry(
            UINotificationHandle handle,
            object presentation,
            int priority,
            UINotificationCoalescingKey coalescingKey,
            UINotificationCorrelationId correlationId)
        {
            Handle = handle;
            Presentation = presentation;
            Priority = priority;
            CoalescingKey = coalescingKey;
            CorrelationId = correlationId;
        }

        public UINotificationHandle Handle { get; }

        public long Generation =>
            Handle == null
                ? 0
                : Handle.Generation;

        public object Presentation { get; }

        public int Priority { get; }

        public UINotificationCoalescingKey CoalescingKey { get; }

        public UINotificationCorrelationId CorrelationId { get; }
    }

    /// <summary>
    /// Immutable bounded read model of the visible entries in one channel.
    /// Entry order is the service's deterministic visible order. The service
    /// retains no presentation snapshot history.
    /// </summary>
    public readonly struct UINotificationPresentationSnapshot
    {
        private static readonly IReadOnlyList<
            UINotificationPresentationEntry> EmptyEntries =
                Array.Empty<UINotificationPresentationEntry>();

        private readonly IReadOnlyList<
            UINotificationPresentationEntry> visibleEntries;

        internal UINotificationPresentationSnapshot(
            UINotificationChannelId channelId,
            UINotificationPresentationEntry[] entries)
        {
            ChannelId = channelId;

            visibleEntries =
                entries == null || entries.Length == 0
                    ? EmptyEntries
                    : Array.AsReadOnly(entries);
        }

        public UINotificationChannelId ChannelId { get; }

        public IReadOnlyList<UINotificationPresentationEntry>
            VisibleEntries =>
                visibleEntries ?? EmptyEntries;

        public int VisibleCount =>
            VisibleEntries.Count;
    }
}
