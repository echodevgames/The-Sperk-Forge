using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored presentation context identity consumed by Looking Glass.
    /// </summary>
    public readonly struct UIContextId : IEquatable<UIContextId>
    {
        public UIContextId(string value)
        {
            Value = Normalize(value);
        }

        public string Value { get; }

        public bool IsValid =>
            !string.IsNullOrEmpty(Value);

        public static string Normalize(string value) =>
            string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();

        public bool Equals(UIContextId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UIContextId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(
                Value ?? string.Empty);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            UIContextId left,
            UIContextId right) =>
            left.Equals(right);

        public static bool operator !=(
            UIContextId left,
            UIContextId right) =>
            !left.Equals(right);
    }
}
