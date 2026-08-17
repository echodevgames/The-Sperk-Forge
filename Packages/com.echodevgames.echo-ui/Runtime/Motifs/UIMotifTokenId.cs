using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored identity shared by all Motif token families.
    /// </summary>
    public readonly struct UIMotifTokenId : IEquatable<UIMotifTokenId>
    {
        private readonly string value;

        public UIMotifTokenId(string value)
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

        public bool Equals(UIMotifTokenId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UIMotifTokenId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UIMotifTokenId left,
            UIMotifTokenId right) =>
            left.Equals(right);

        public static bool operator !=(
            UIMotifTokenId left,
            UIMotifTokenId right) =>
            !left.Equals(right);
    }
}
