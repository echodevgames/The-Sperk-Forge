using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored Looking Glass layer identity.
    /// </summary>
    public readonly struct UILayerId : IEquatable<UILayerId>
    {
        private readonly string value;

        public UILayerId(string value)
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

        public bool Equals(UILayerId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UILayerId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UILayerId left,
            UILayerId right) =>
            left.Equals(right);

        public static bool operator !=(
            UILayerId left,
            UILayerId right) =>
            !left.Equals(right);
    }
}
