using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Stable provider identity. ESV-M1-01 defines identity only; storage
    /// operations are intentionally deferred.
    /// </summary>
    public readonly struct SaveStorageBackendId :
        IEquatable<SaveStorageBackendId>
    {
        public SaveStorageBackendId(string value)
        {
            Value = SaveStableId.Normalize(
                value,
                nameof(value));
        }

        public string Value { get; }

        public bool Equals(
            SaveStorageBackendId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is SaveStorageBackendId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveStorageBackendId left,
            SaveStorageBackendId right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveStorageBackendId left,
            SaveStorageBackendId right) =>
            !left.Equals(right);
    }
}
