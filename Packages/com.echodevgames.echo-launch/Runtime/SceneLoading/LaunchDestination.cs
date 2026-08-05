//----- LaunchDestination.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Project-owned immutable definition of the one initial destination used
    /// by First Light after startup-sequence execution succeeds.
    ///
    /// Runtime code reads this asset but never repairs, migrates, or rewrites
    /// its serialized values.
    /// </summary>
    [CreateAssetMenu(
        fileName = "LaunchDestination",
        menuName =
            "EchoDevGames/First Light/Launch Destination",
        order = 1)]
    public sealed class LaunchDestination :
        ScriptableObject
    {
        /// <summary>
        /// Identifies the currently supported serialized destination shape.
        /// </summary>
        public const int CurrentSchemaVersion = 1;

        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string destinationId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        [HideInInspector]
        private int schemaVersion =
            CurrentSchemaVersion;

        [SerializeField]
        private string displayName =
            "Initial Destination";

        [SerializeField]
        private string scenePath =
            string.Empty;

        /// <summary>
        /// Gets the stable runtime-safe destination identity.
        /// </summary>
        public string DestinationId =>
            destinationId ?? string.Empty;

        /// <summary>
        /// Gets the serialized destination structure version.
        /// </summary>
        public int SchemaVersion =>
            schemaVersion;

        /// <summary>
        /// Gets the user-facing destination label.
        /// </summary>
        public string DisplayName =>
            displayName ?? string.Empty;

        /// <summary>
        /// Gets the runtime-safe Unity scene asset path.
        /// </summary>
        public string ScenePath =>
            scenePath ?? string.Empty;

        internal bool HasValidIdentity =>
            IsCanonicalDestinationId(
                destinationId);

        internal bool HasSupportedSchema =>
            schemaVersion ==
            CurrentSchemaVersion;

        internal bool HasValidDisplayName =>
            IsTrimmedNonblank(
                displayName);

        internal bool HasValidScenePath =>
            IsCanonicalScenePath(
                scenePath);

        internal static bool IsCanonicalDestinationId(
            string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length != CanonicalIdLength)
            {
                return false;
            }

            for (int index = 0;
                 index < value.Length;
                 index++)
            {
                char character =
                    value[index];

                bool isNumber =
                    character >= '0' &&
                    character <= '9';

                bool isLowercaseHexLetter =
                    character >= 'a' &&
                    character <= 'f';

                if (!isNumber &&
                    !isLowercaseHexLetter)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsTrimmedNonblank(
            string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   string.Equals(
                       value,
                       value.Trim(),
                       StringComparison.Ordinal);
        }

        private static bool IsCanonicalScenePath(
            string value)
        {
            if (!IsTrimmedNonblank(value))
            {
                return false;
            }

            return value.StartsWith(
                       "Assets/",
                       StringComparison.Ordinal) &&
                   value.EndsWith(
                       ".unity",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}

//----- LaunchDestination.cs END -----
