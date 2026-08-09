
using System;

namespace EchoDevGames.EchoSave
{
    public readonly struct SaveIntegrityProviderId :
        IEquatable<SaveIntegrityProviderId>
    {
        public SaveIntegrityProviderId(
            string value)
        {
            Value =
                SaveStableId.Normalize(
                    value,
                    nameof(value));
        }

        public string Value { get; }

        public bool Equals(
            SaveIntegrityProviderId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveIntegrityProviderId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(
                    Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveIntegrityProviderId left,
            SaveIntegrityProviderId right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveIntegrityProviderId left,
            SaveIntegrityProviderId right) =>
            !left.Equals(right);
    }
}
