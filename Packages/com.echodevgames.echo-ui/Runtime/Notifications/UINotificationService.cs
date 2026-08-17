using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Root-owned bounded notification channel state.
    /// Admission and lifecycle mutation are added by later EUI-M4-02 slices.
    /// </summary>
    public sealed class UINotificationService
    {
        private sealed class Entry
        {
            public Entry(
                UINotificationRequest request,
                UINotificationHandle handle,
                long admissionSequence)
            {
                Request = request;
                Handle = handle;
                AdmissionSequence = admissionSequence;
            }

            public UINotificationRequest Request { get; }

            public UINotificationHandle Handle { get; }

            public long AdmissionSequence { get; }
        }

        private sealed class ChannelState
        {
            public ChannelState(
                UINotificationChannelDefinition definition)
            {
                Definition = definition;
                Visible =
                    new List<Entry>(
                        definition.VisibleCapacity);

                Pending =
                    new List<Entry>(
                        definition.PendingCapacity);
            }

            public UINotificationChannelDefinition Definition { get; }

            public List<Entry> Visible { get; }

            public List<Entry> Pending { get; }
        }

        private readonly Dictionary<string, ChannelState> channels =
            new Dictionary<string, ChannelState>(
                StringComparer.Ordinal);

        public UINotificationService(
            IEnumerable<UINotificationChannelDefinition> definitions,
            out string validationError)
        {
            validationError =
                ValidateAndSnapshotDefinitions(definitions);
        }

        public bool IsValid { get; private set; }

        public int ChannelCount =>
            channels.Count;

        public int VisibleCount
        {
            get
            {
                int count = 0;

                foreach (ChannelState channel in channels.Values)
                {
                    count += channel.Visible.Count;
                }

                return count;
            }
        }

        public int PendingCount
        {
            get
            {
                int count = 0;

                foreach (ChannelState channel in channels.Values)
                {
                    count += channel.Pending.Count;
                }

                return count;
            }
        }

        public bool TryGetDefinition(
            string channelId,
            out UINotificationChannelDefinition definition) =>
            TryGetDefinition(
                new UINotificationChannelId(channelId),
                out definition);

        public bool TryGetDefinition(
            UINotificationChannelId channelId,
            out UINotificationChannelDefinition definition)
        {
            definition = null;

            if (!channelId.IsValid ||
                !channels.TryGetValue(
                    channelId.Value,
                    out ChannelState channel))
            {
                return false;
            }

            definition = channel.Definition;
            return true;
        }

        public bool TryGetSnapshot(
            string channelId,
            out UINotificationChannelSnapshot snapshot) =>
            TryGetSnapshot(
                new UINotificationChannelId(channelId),
                out snapshot);

        public bool TryGetSnapshot(
            UINotificationChannelId channelId,
            out UINotificationChannelSnapshot snapshot)
        {
            snapshot = default;

            if (!channelId.IsValid ||
                !channels.TryGetValue(
                    channelId.Value,
                    out ChannelState channel))
            {
                return false;
            }

            UINotificationChannelDefinition definition =
                channel.Definition;

            snapshot =
                new UINotificationChannelSnapshot(
                    definition.ChannelId,
                    definition.VisibleCapacity,
                    definition.PendingCapacity,
                    channel.Visible.Count,
                    channel.Pending.Count,
                    definition.OverflowPolicy);

            return true;
        }

        private string ValidateAndSnapshotDefinitions(
            IEnumerable<UINotificationChannelDefinition> source)
        {
            if (source == null)
            {
                return
                    "Notification channel definitions are required.";
            }

            Dictionary<string, ChannelState> candidates =
                new Dictionary<string, ChannelState>(
                    StringComparer.Ordinal);

            foreach (UINotificationChannelDefinition authored in source)
            {
                string validationError =
                    ValidateDefinition(authored);

                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    return validationError;
                }

                UINotificationChannelDefinition definition =
                    authored.Snapshot();

                if (candidates.ContainsKey(
                        definition.ChannelId.Value))
                {
                    return
                        "Duplicate notification channel ID: " +
                        definition.ChannelId.Value;
                }

                candidates.Add(
                    definition.ChannelId.Value,
                    new ChannelState(definition));
            }

            if (candidates.Count == 0)
            {
                return
                    "At least one notification channel definition is required.";
            }

            foreach (KeyValuePair<string, ChannelState> candidate in candidates)
            {
                channels.Add(
                    candidate.Key,
                    candidate.Value);
            }

            IsValid = true;
            return string.Empty;
        }

        private static string ValidateDefinition(
            UINotificationChannelDefinition definition)
        {
            if (definition == null)
            {
                return
                    "A notification channel definition reference is missing.";
            }

            if (!definition.ChannelId.IsValid)
            {
                return
                    "Notification channel IDs must be nonempty stable project-authored values.";
            }

            if (definition.VisibleCapacity < 1)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' requires visible capacity of at least one.";
            }

            if (definition.PendingCapacity < 0)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' cannot use a negative pending capacity.";
            }

            if (float.IsNaN(
                    definition.DefaultLifetimeSeconds) ||
                float.IsInfinity(
                    definition.DefaultLifetimeSeconds) ||
                definition.DefaultLifetimeSeconds <= 0f)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' requires a finite positive default lifetime.";
            }

            switch (definition.OverflowPolicy)
            {
                case UINotificationOverflowPolicy.RejectNewest:
                case UINotificationOverflowPolicy.DropOldestPending:
                case UINotificationOverflowPolicy.ReplaceLowestPriorityPending:
                    return string.Empty;

                default:
                    return
                        "Notification channel '" +
                        definition.ChannelId.Value +
                        "' uses an unsupported pending overflow policy.";
            }
        }
    }
}
