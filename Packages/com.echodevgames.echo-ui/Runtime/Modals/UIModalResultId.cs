using System;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Stable project-authored semantic result identity for a modal completion.
    /// Looking Glass assigns no domain meaning to the value.
    /// </summary>
    public readonly struct UIModalResultId : IEquatable<UIModalResultId>
    {
        private readonly string value;

        public UIModalResultId(string value)
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

        public bool Equals(UIModalResultId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is UIModalResultId other &&
            Equals(other);

        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value;

        public static bool operator ==(
            UIModalResultId left,
            UIModalResultId right) =>
            left.Equals(right);

        public static bool operator !=(
            UIModalResultId left,
            UIModalResultId right) =>
            !left.Equals(right);
    }
}
