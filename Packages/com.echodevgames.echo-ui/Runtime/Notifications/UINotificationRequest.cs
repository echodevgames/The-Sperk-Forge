using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable request for one notification admission.
    /// Presentation is an opaque project-owned live payload. It is never part
    /// of status snapshots or bounded diagnostics.
    /// </summary>
    public sealed class UINotificationRequest
    {
        public UINotificationRequest(
            string channelId,
            object presentation,
            int priority = 0,
            UINotificationLifetimeMode lifetimeMode =
                UINotificationLifetimeMode.Automatic,
            float durationSeconds = 0f,
            string coalescingKey = "",
            Object owner = null,
            string correlationId = "")
        {
            ChannelId =
                new UINotificationChannelId(channelId);

            Presentation = presentation;
            Priority = priority;
            LifetimeMode = lifetimeMode;
            DurationSeconds = durationSeconds;
            CoalescingKey =
                new UINotificationCoalescingKey(coalescingKey);

            Owner = owner;
            HasOwner = owner != null;
            CorrelationId =
                new UINotificationCorrelationId(correlationId);
        }

        public UINotificationChannelId ChannelId { get; }

        public object Presentation { get; }

        public int Priority { get; }

        public UINotificationLifetimeMode LifetimeMode { get; }

        /// <summary>
        /// Positive values override the channel default for automatic entries.
        /// Zero selects the channel default. Manual entries ignore duration.
        /// Other values remain visible to validation and are never normalized.
        /// </summary>
        public float DurationSeconds { get; }

        public bool UsesChannelDefaultLifetime =>
            LifetimeMode == UINotificationLifetimeMode.Automatic &&
            DurationSeconds == 0f;

        public bool HasLifetimeOverride =>
            LifetimeMode == UINotificationLifetimeMode.Automatic &&
            DurationSeconds > 0f;

        public UINotificationCoalescingKey CoalescingKey { get; }

        public Object Owner { get; }

        public bool HasOwner { get; }

        public UINotificationCorrelationId CorrelationId { get; }
    }
}
