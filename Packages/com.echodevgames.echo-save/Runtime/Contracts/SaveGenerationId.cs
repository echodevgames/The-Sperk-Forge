
using System;
using System.Globalization;
using System.Threading;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Unique sortable Chronicle generation identity.
    ///
    /// The format contains a UTC technical timestamp, a monotonic process
    /// sequence, and independent random entropy. Uniqueness never relies on
    /// the wall clock alone.
    /// </summary>
    public readonly struct SaveGenerationId :
        IEquatable<SaveGenerationId>,
        IComparable<SaveGenerationId>
    {
        private const string TimestampFormat =
            "yyyyMMdd'T'HHmmssfffffff'Z'";

        private static long sessionSequence;

        private SaveGenerationId(
            string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static SaveGenerationId NewId()
        {
            long sequence =
                Interlocked.Increment(
                    ref sessionSequence);

            return Create(
                DateTime.UtcNow,
                sequence,
                Guid.NewGuid());
        }

        public static bool TryParse(
            string value,
            out SaveGenerationId id)
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

            string[] parts =
                value.Split('-');

            if (parts.Length != 3 ||
                parts[0].Length != 23 ||
                parts[1].Length != 16 ||
                parts[2].Length != 32)
            {
                return false;
            }

            if (!DateTime.TryParseExact(
                    parts[0],
                    TimestampFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal,
                    out _))
            {
                return false;
            }

            if (!long.TryParse(
                    parts[1],
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out long sequence) ||
                sequence <= 0)
            {
                return false;
            }

            if (!Guid.TryParseExact(
                    parts[2],
                    "N",
                    out Guid random))
            {
                return false;
            }

            string canonicalRandom =
                random.ToString("N")
                    .ToLowerInvariant();

            if (!string.Equals(
                    canonicalRandom,
                    parts[2],
                    StringComparison.Ordinal))
            {
                return false;
            }

            id =
                new SaveGenerationId(
                    value);

            return true;
        }

        internal static SaveGenerationId
            CreateForTesting(
                DateTime utcTimestamp,
                long sequence,
                Guid random) =>
            Create(
                utcTimestamp,
                sequence,
                random);

        public int CompareTo(
            SaveGenerationId other) =>
            string.Compare(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public bool Equals(
            SaveGenerationId other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveGenerationId other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(
                    Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveGenerationId left,
            SaveGenerationId right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveGenerationId left,
            SaveGenerationId right) =>
            !left.Equals(right);

        private static SaveGenerationId Create(
            DateTime utcTimestamp,
            long sequence,
            Guid random)
        {
            if (sequence <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sequence));
            }

            DateTime normalizedUtc =
                utcTimestamp.Kind == DateTimeKind.Utc
                    ? utcTimestamp
                    : utcTimestamp.ToUniversalTime();

            string value =
                normalizedUtc.ToString(
                    TimestampFormat,
                    CultureInfo.InvariantCulture) +
                "-" +
                sequence.ToString(
                    "D16",
                    CultureInfo.InvariantCulture) +
                "-" +
                random.ToString("N")
                    .ToLowerInvariant();

            return new SaveGenerationId(
                value);
        }
    }
}
