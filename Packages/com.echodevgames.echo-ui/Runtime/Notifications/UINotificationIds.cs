using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored identity for one notification channel.
    /// </summary>
    public readonly struct UINotificationChannelId :
        IEquatable<UINotificationChannelId>
    {
        private readonly string value;

        public UINotificationChannelId(string value)
        {
            this.value =
                value == null
                    ? string.Empty
                    : value.Trim();
        }

        public string Value =>
            value ?? string.Empty;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Value);

        public bool Equals(UINotificationChannelId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UINotificationChannelId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UINotificationChannelId left,
            UINotificationChannelId right) =>
            left.Equals(right);

        public static bool operator !=(
            UINotificationChannelId left,
            UINotificationChannelId right) =>
            !left.Equals(right);
    }

    /// <summary>
    /// Optional stable key used to coalesce live entries inside one channel.
    /// </summary>
    public readonly struct UINotificationCoalescingKey :
        IEquatable<UINotificationCoalescingKey>
    {
        private readonly string value;

        public UINotificationCoalescingKey(string value)
        {
            this.value =
                value == null
                    ? string.Empty
                    : value.Trim();
        }

        public string Value =>
            value ?? string.Empty;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Value);

        public bool IsEmpty =>
            !IsValid;

        public bool Equals(UINotificationCoalescingKey other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UINotificationCoalescingKey other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UINotificationCoalescingKey left,
            UINotificationCoalescingKey right) =>
            left.Equals(right);

        public static bool operator !=(
            UINotificationCoalescingKey left,
            UINotificationCoalescingKey right) =>
            !left.Equals(right);
    }

    /// <summary>
    /// Optional project identity used to correlate admission and settlement.
    /// Looking Glass assigns no domain meaning to the value.
    /// </summary>
    public readonly struct UINotificationCorrelationId :
        IEquatable<UINotificationCorrelationId>
    {
        private readonly string value;

        public UINotificationCorrelationId(string value)
        {
            this.value =
                value == null
                    ? string.Empty
                    : value.Trim();
        }

        public string Value =>
            value ?? string.Empty;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Value);

        public bool IsEmpty =>
            !IsValid;

        public bool Equals(UINotificationCorrelationId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UINotificationCorrelationId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UINotificationCorrelationId left,
            UINotificationCorrelationId right) =>
            left.Equals(right);

        public static bool operator !=(
            UINotificationCorrelationId left,
            UINotificationCorrelationId right) =>
            !left.Equals(right);
    }
}
