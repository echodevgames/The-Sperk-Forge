using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Stable serializer-provider identity. Serializer implementation is not
    /// part of ESV-M1-01.
    /// </summary>
    public readonly struct SaveSerializerId :
        IEquatable<SaveSerializerId>
    {
        public SaveSerializerId(string value)
        {
            Value = SaveStableId.Normalize(
                value,
                nameof(value));
        }

        public string Value { get; }

        public bool Equals(
            SaveSerializerId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is SaveSerializerId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveSerializerId left,
            SaveSerializerId right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveSerializerId left,
            SaveSerializerId right) =>
            !left.Equals(right);
    }
}
