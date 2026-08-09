
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Stable Chronicle slot identity.
    ///
    /// Slot IDs are package-generated lowercase canonical GUID strings and are
    /// never player-facing display names.
    /// </summary>
    public readonly struct SaveSlotId :
        IEquatable<SaveSlotId>
    {
        private SaveSlotId(
            string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static SaveSlotId NewId() =>
            new SaveSlotId(
                Guid.NewGuid()
                    .ToString("D")
                    .ToLowerInvariant());

        public static bool TryParse(
            string value,
            out SaveSlotId id)
        {
            id = default;

            if (string.IsNullOrEmpty(value) ||
                !string.Equals(
                    value,
                    value.Trim(),
                    StringComparison.Ordinal))
            {
                return false;
            }

            if (!Guid.TryParseExact(
                    value,
                    "D",
                    out Guid parsed))
            {
                return false;
            }

            string canonical =
                parsed.ToString("D")
                    .ToLowerInvariant();

            if (!string.Equals(
                    canonical,
                    value,
                    StringComparison.Ordinal))
            {
                return false;
            }

            id =
                new SaveSlotId(
                    canonical);

            return true;
        }

        public bool Equals(
            SaveSlotId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveSlotId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(
                    Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveSlotId left,
            SaveSlotId right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveSlotId left,
            SaveSlotId right) =>
            !left.Equals(right);
    }
}
