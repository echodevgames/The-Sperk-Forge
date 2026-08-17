using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable-at-runtime project-authored notification channel definition.
    /// Visible and pending capacity remain independently bounded.
    /// </summary>
    [Serializable]
    public sealed class UINotificationChannelDefinition
    {
        [SerializeField]
        private string channelId = "notification.default";

        [SerializeField, Min(1)]
        private int visibleCapacity = 1;

        [SerializeField, Min(0)]
        private int pendingCapacity = 8;

        [SerializeField, Min(0.001f)]
        private float defaultLifetimeSeconds = 4f;

        [SerializeField]
        private UINotificationOverflowPolicy overflowPolicy =
            UINotificationOverflowPolicy.RejectNewest;

        public UINotificationChannelDefinition()
        {
        }

        public UINotificationChannelDefinition(
            string channelId,
            int visibleCapacity = 1,
            int pendingCapacity = 8,
            float defaultLifetimeSeconds = 4f,
            UINotificationOverflowPolicy overflowPolicy =
                UINotificationOverflowPolicy.RejectNewest)
        {
            this.channelId = channelId ?? string.Empty;
            this.visibleCapacity = visibleCapacity;
            this.pendingCapacity = pendingCapacity;
            this.defaultLifetimeSeconds = defaultLifetimeSeconds;
            this.overflowPolicy = overflowPolicy;
        }

        public UINotificationChannelId ChannelId =>
            new UINotificationChannelId(channelId);

        public int VisibleCapacity =>
            visibleCapacity;

        public int PendingCapacity =>
            pendingCapacity;

        public float DefaultLifetimeSeconds =>
            defaultLifetimeSeconds;

        public UINotificationOverflowPolicy OverflowPolicy =>
            overflowPolicy;

        internal UINotificationChannelDefinition Snapshot() =>
            new UINotificationChannelDefinition(
                ChannelId.Value,
                visibleCapacity,
                pendingCapacity,
                defaultLifetimeSeconds,
                overflowPolicy);
    }
}
