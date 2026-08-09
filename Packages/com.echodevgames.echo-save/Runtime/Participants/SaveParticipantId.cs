
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Durable developer-authored identity for one persistence participant.
    ///
    /// IDs are canonical lowercase reverse-domain / namespace-like values.
    /// Chronicle never derives this identity from a scene object, display name,
    /// Unity instance ID, file path, or CLR type name.
    /// </summary>
    public readonly struct SaveParticipantId :
        IEquatable<SaveParticipantId>,
        IComparable<SaveParticipantId>
    {
        public const int MaxLength = 128;

        private static readonly HashSet<string>
            ReservedSegments =
                new HashSet<string>(
                    StringComparer.Ordinal)
                {
                    "con",
                    "prn",
                    "aux",
                    "nul",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9"
                };

        public SaveParticipantId(
            string value)
        {
            if (!TryParse(
                    value,
                    out SaveParticipantId parsed))
            {
                throw new ArgumentException(
                    "Chronicle participant IDs must be canonical lowercase reverse-domain / namespace-like identifiers.",
                    nameof(value));
            }

            Value =
                parsed.Value;
        }

        private SaveParticipantId(
            string value,
            bool validated)
        {
            Value =
                value;
        }

        public string Value { get; }

        public static bool TryParse(
            string value,
            out SaveParticipantId id)
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

            string[] segments =
                value.Split('.');

            if (segments.Length < 2)
            {
                return false;
            }

            for (int segmentIndex = 0;
                 segmentIndex < segments.Length;
                 segmentIndex++)
            {
                string segment =
                    segments[segmentIndex];

                if (!IsValidSegment(
                        segment))
                {
                    return false;
                }
            }

            id =
                new SaveParticipantId(
                    value,
                    true);

            return true;
        }

        public int CompareTo(
            SaveParticipantId other) =>
            string.Compare(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public bool Equals(
            SaveParticipantId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveParticipantId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(
                    Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveParticipantId left,
            SaveParticipantId right) =>
            left.Equals(
                right);

        public static bool operator !=(
            SaveParticipantId left,
            SaveParticipantId right) =>
            !left.Equals(
                right);

        private static bool IsValidSegment(
            string segment)
        {
            if (string.IsNullOrEmpty(
                    segment) ||
                segment == "." ||
                segment == ".." ||
                ReservedSegments.Contains(
                    segment))
            {
                return false;
            }

            if (!IsAsciiLetterOrDigit(
                    segment[0]) ||
                !IsAsciiLetterOrDigit(
                    segment[
                        segment.Length - 1]))
            {
                return false;
            }

            for (int i = 0;
                 i < segment.Length;
                 i++)
            {
                char character =
                    segment[i];

                if (char.IsControl(
                        character) ||
                    character == '/' ||
                    character == '\\' ||
                    character == ':' ||
                    character == ' ')
                {
                    return false;
                }

                if (!IsAsciiLetterOrDigit(
                        character) &&
                    character != '-' &&
                    character != '_')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAsciiLetterOrDigit(
            char character) =>
            (character >= 'a' &&
             character <= 'z') ||
            (character >= '0' &&
             character <= '9');
    }
}
