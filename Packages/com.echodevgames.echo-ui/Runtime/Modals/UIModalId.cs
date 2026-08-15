using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored identity for one blocking modal definition.
    /// </summary>
    public readonly struct UIModalId : IEquatable<UIModalId>
    {
        private readonly string value;

        public UIModalId(string value)
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

        public bool Equals(UIModalId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UIModalId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UIModalId left,
            UIModalId right) =>
            left.Equals(right);

        public static bool operator !=(
            UIModalId left,
            UIModalId right) =>
            !left.Equals(right);
    }
}
