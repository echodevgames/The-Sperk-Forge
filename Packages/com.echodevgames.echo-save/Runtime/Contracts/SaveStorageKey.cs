
using System;
using System.IO;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Validated relative key used by Chronicle storage backends.
    /// </summary>
    public readonly struct SaveStorageKey :
        IEquatable<SaveStorageKey>
    {
        private static readonly char[]
            AlwaysInvalidCharacters =
            {
                ':',
                '"',
                '<',
                '>',
                '|',
                '?',
                '*'
            };

        private SaveStorageKey(
            string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static SaveStorageResult TryCreate(
            string value,
            out SaveStorageKey key)
        {
            key = default;

            if (string.IsNullOrEmpty(value))
            {
                return Invalid(
                    "A Chronicle storage key is required.");
            }

            if (!string.Equals(
                    value,
                    value.Trim(),
                    StringComparison.Ordinal))
            {
                return Invalid(
                    "Chronicle storage keys cannot have leading or trailing whitespace.");
            }

            if (Path.IsPathRooted(value) ||
                value[0] == '/' ||
                value[0] == '\\')
            {
                return Invalid(
                    "Chronicle storage keys must be relative.");
            }

            string normalized =
                value.Replace('\\', '/');

            string[] segments =
                normalized.Split('/');

            for (int segmentIndex = 0;
                 segmentIndex < segments.Length;
                 segmentIndex++)
            {
                string segment =
                    segments[segmentIndex];

                if (segment.Length == 0)
                {
                    return Invalid(
                        "Chronicle storage keys cannot contain empty path segments.");
                }

                if (segment == "." ||
                    segment == "..")
                {
                    return Invalid(
                        "Chronicle storage keys cannot contain traversal segments.");
                }

                if (!string.Equals(
                        segment,
                        segment.Trim(),
                        StringComparison.Ordinal))
                {
                    return Invalid(
                        "Chronicle storage-key segments cannot have leading or trailing whitespace.");
                }

                for (int charIndex = 0;
                     charIndex < segment.Length;
                     charIndex++)
                {
                    char character =
                        segment[charIndex];

                    if (char.IsControl(character) ||
                        Array.IndexOf(
                            AlwaysInvalidCharacters,
                            character) >= 0)
                    {
                        return Invalid(
                            "Chronicle storage keys contain an unsupported character.");
                    }
                }
            }

            key =
                new SaveStorageKey(
                    normalized);

            return SaveStorageResult.Success(
                "The Chronicle storage key is valid.");
        }

        public bool Equals(
            SaveStorageKey other) =>
            string.Equals(
                Value,
                other.Value,
                StringComparison.Ordinal);

        public override bool Equals(
            object obj) =>
            obj is SaveStorageKey other &&
            Equals(other);

        public override int GetHashCode() =>
            Value == null
                ? 0
                : StringComparer.Ordinal.GetHashCode(
                    Value);

        public override string ToString() =>
            Value ?? string.Empty;

        public static bool operator ==(
            SaveStorageKey left,
            SaveStorageKey right) =>
            left.Equals(right);

        public static bool operator !=(
            SaveStorageKey left,
            SaveStorageKey right) =>
            !left.Equals(right);

        private static SaveStorageResult Invalid(
            string message) =>
            new SaveStorageResult(
                SaveStorageStatus.InvalidPath,
                EchoSaveDiagnosticCodes
                    .StorageInvalidPath,
                message);
    }
}
