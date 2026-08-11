using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Exact Chronicle-owned package-document format version.
    /// </summary>
    internal readonly struct SavePackageDocumentVersion :
        IEquatable<SavePackageDocumentVersion>,
        IComparable<SavePackageDocumentVersion>
    {
        internal const int MaximumComponent =
            1000000;

        internal SavePackageDocumentVersion(
            int major,
            int minor,
            int revision)
        {
            if (!IsValidComponent(major) ||
                !IsValidComponent(minor) ||
                !IsValidComponent(revision))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(major),
                    "Chronicle package-document version components must be bounded non-negative integers.");
            }

            Major = major;
            Minor = minor;
            Revision = revision;
        }

        internal int Major { get; }

        internal int Minor { get; }

        internal int Revision { get; }

        internal static bool TryCreate(
            int major,
            int minor,
            int revision,
            out SavePackageDocumentVersion version)
        {
            version = default;

            if (!IsValidComponent(major) ||
                !IsValidComponent(minor) ||
                !IsValidComponent(revision))
            {
                return false;
            }

            version =
                new SavePackageDocumentVersion(
                    major,
                    minor,
                    revision);

            return true;
        }

        public int CompareTo(
            SavePackageDocumentVersion other)
        {
            int majorComparison =
                Major.CompareTo(
                    other.Major);

            if (majorComparison != 0)
            {
                return majorComparison;
            }

            int minorComparison =
                Minor.CompareTo(
                    other.Minor);

            if (minorComparison != 0)
            {
                return minorComparison;
            }

            return Revision.CompareTo(
                other.Revision);
        }

        public bool Equals(
            SavePackageDocumentVersion other) =>
            Major == other.Major &&
            Minor == other.Minor &&
            Revision == other.Revision;

        public override bool Equals(
            object obj) =>
            obj is SavePackageDocumentVersion other &&
            Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Major;
                hash = (hash * 397) ^ Minor;
                hash = (hash * 397) ^ Revision;
                return hash;
            }
        }

        public override string ToString() =>
            Major + "." +
            Minor + "." +
            Revision;

        public static bool operator ==(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            left.Equals(right);

        public static bool operator !=(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            !left.Equals(right);

        public static bool operator <(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            left.CompareTo(right) < 0;

        public static bool operator >(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            left.CompareTo(right) > 0;

        public static bool operator <=(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >=(
            SavePackageDocumentVersion left,
            SavePackageDocumentVersion right) =>
            left.CompareTo(right) >= 0;

        private static bool IsValidComponent(
            int value) =>
            value >= 0 &&
            value <= MaximumComponent;
    }

    internal static class SavePackageDocumentVersionAuthority
    {
        internal static bool TryGetCurrent(
            string documentKind,
            out SavePackageDocumentVersion version)
        {
            version = default;

            switch (documentKind)
            {
                case SaveDocumentKinds.Envelope:
                    return SavePackageDocumentVersion.TryCreate(
                        SaveDocumentVersions.EnvelopeMajor,
                        SaveDocumentVersions.EnvelopeMinor,
                        SaveDocumentVersions.EnvelopeRevision,
                        out version);

                case SaveDocumentKinds.Manifest:
                    return SavePackageDocumentVersion.TryCreate(
                        SaveDocumentVersions.ManifestMajor,
                        SaveDocumentVersions.ManifestMinor,
                        SaveDocumentVersions.ManifestRevision,
                        out version);

                case SaveDocumentKinds.Payload:
                    return SavePackageDocumentVersion.TryCreate(
                        SaveDocumentVersions.PayloadMajor,
                        SaveDocumentVersions.PayloadMinor,
                        SaveDocumentVersions.PayloadRevision,
                        out version);

                case SaveDocumentKinds.HeadPointer:
                    return SavePackageDocumentVersion.TryCreate(
                        SaveDocumentVersions.HeadPointerMajor,
                        SaveDocumentVersions.HeadPointerMinor,
                        SaveDocumentVersions.HeadPointerRevision,
                        out version);

                default:
                    return false;
            }
        }
    }
}
