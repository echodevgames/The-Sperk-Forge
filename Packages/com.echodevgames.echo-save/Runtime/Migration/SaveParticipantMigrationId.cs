using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Stable developer-authored identity for one participant migration step.
    /// Never derived from CLR type names or display labels.
    /// </summary>
    public readonly struct SaveParticipantMigrationId :
        IEquatable<SaveParticipantMigrationId>,
        IComparable<SaveParticipantMigrationId>
    {
        public const int MaxLength = 128;

        public SaveParticipantMigrationId(
            string value)
        {
            if (!TryParse(
                    value,
                    out SaveParticipantMigrationId parsed))
            {
                throw new ArgumentException(
                    "Chronicle participant migration IDs must be canonical lowercase stable identifiers.",
                    nameof(value));
            }

            Value =
                parsed.Value;
        }

        private SaveParticipantMigrationId(
            string value,
            bool validated)
        {
            Value =
                value;
        }

        public string Value { get; }

        public static bool TryParse(
            string value,
            out SaveParticipantMigrationId id)
        {
            id =
                default;

            if (string.IsNullOrEmpty(
                    value) ||
                value.Length >
                    MaxLength ||
                !string.Equals(
                    value,
                    value.Trim(),
                    StringComparison.Ordinal) ||
                !string.Equals(
                    value,
                    value.ToLowerInvariant(),
                    StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = 0;
                 i < value.Length;
                 i++)
            {
                char character =
                    value[i];

                bool valid =
                    (character >= 'a' &&
                     character <= 'z') ||
                    (character >= '0' &&
                     character <= '9') ||
                    character == '.' ||
                    character == '-' ||
                    character == '_';

                if (!valid)
                {
                    return false;
                }
            }

            if (value == "." ||
                value == "..")
            {
                return false;
            }

            id =
                new SaveParticipantMigrationId(
                    value,
                    true);

            return true;
        }

        public int CompareTo(
            SaveParticipantMigrationId other) =>
            string.Compare(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public bool Equals(
            SaveParticipantMigrationId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveParticipantMigrationId other &&
            Equals(
                other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal
                    .GetHashCode(
                        Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveParticipantMigrationId left,
            SaveParticipantMigrationId right) =>
            left.Equals(
                right);

        public static bool operator !=(
            SaveParticipantMigrationId left,
            SaveParticipantMigrationId right) =>
            !left.Equals(
                right);
    }
}
