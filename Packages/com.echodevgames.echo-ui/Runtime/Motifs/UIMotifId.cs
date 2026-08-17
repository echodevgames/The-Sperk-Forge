using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored identity for one Motif definition.
    /// </summary>
    public readonly struct UIMotifId : IEquatable<UIMotifId>
    {
        private readonly string value;

        public UIMotifId(string value)
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

        public bool Equals(UIMotifId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UIMotifId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UIMotifId left,
            UIMotifId right) =>
            left.Equals(right);

        public static bool operator !=(
            UIMotifId left,
            UIMotifId right) =>
            !left.Equals(right);
    }
}
