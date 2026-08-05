//----- EchoLaunchConfiguration.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores the project-owned authored configuration used by First Light.
    ///
    /// This asset contains immutable launch definition data only.
    /// Active launch state belongs to LaunchSession and must never be
    /// written back into this ScriptableObject during Play Mode.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EchoLaunchConfiguration",
        menuName =
            "EchoDevGames/First Light/Launch Configuration",
        order = 0)]
    public sealed class EchoLaunchConfiguration :
        ScriptableObject
    {
        /// <summary>
        /// Identifies the currently supported serialized structure of
        /// EchoLaunchConfiguration assets. Schema 4 adds the optional
        /// project-owned splash sequence and reduced-motion default; schemas
        /// 2 and 3 remain historical.
        /// </summary>
        public const int CurrentSchemaVersion = 4;

        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string configurationId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        [HideInInspector]
        private int schemaVersion =
            CurrentSchemaVersion;

        [SerializeField]
        private StartupSequence startupSequence;

        [SerializeField]
        private LaunchDestination initialDestination;

        [SerializeField]
        private SplashSequence splashSequence;

        [SerializeField]
        private bool useReducedMotionForSplash;

        /// <summary>
        /// Gets the stable runtime-safe identity of this configuration.
        /// </summary>
        public string ConfigurationId =>
            configurationId ?? string.Empty;

        /// <summary>
        /// Gets the serialized structure version of this configuration.
        /// </summary>
        public int SchemaVersion =>
            schemaVersion;

        /// <summary>
        /// Gets the project-owned ordered startup sequence assigned to
        /// this configuration.
        /// </summary>
        public StartupSequence StartupSequence =>
            startupSequence;

        /// <summary>
        /// Gets the project-owned initial destination assigned to this
        /// configuration.
        /// </summary>
        public LaunchDestination InitialDestination =>
            initialDestination;

        /// <summary>
        /// Gets the optional project-owned image splash sequence assigned to
        /// this configuration. A null reference intentionally omits the splash
        /// phase.
        /// </summary>
        public SplashSequence SplashSequence =>
            splashSequence;

        /// <summary>
        /// Gets the project-authored reduced-motion default used by root-owned
        /// splash playback.
        /// </summary>
        public bool UseReducedMotionForSplash =>
            useReducedMotionForSplash;

        /// <summary>
        /// Returns true when the stored identity uses the canonical
        /// lowercase 32-character hexadecimal format.
        /// </summary>
        internal bool HasValidIdentity =>
            IsCanonicalConfigurationId(
                configurationId);

        /// <summary>
        /// Returns true when this package version understands the
        /// configuration's serialized structure.
        /// </summary>
        internal bool HasSupportedSchema =>
            schemaVersion ==
            CurrentSchemaVersion;

        private static bool IsCanonicalConfigurationId(
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
                char character = value[index];

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
    }
}

//----- EchoLaunchConfiguration.cs END -----
