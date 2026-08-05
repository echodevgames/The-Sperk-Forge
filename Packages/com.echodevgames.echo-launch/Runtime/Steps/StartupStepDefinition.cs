//----- StartupStepDefinition.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable authored definition for one startup operation.
    ///
    /// Runtime execution behavior is deliberately introduced by a later
    /// checkpoint. Active execution state must never be stored here.
    /// </summary>
    public abstract class StartupStepDefinition :
        ScriptableObject
    {
        /// <summary>
        /// Identifies the currently supported serialized structure of
        /// startup-step definition assets.
        /// </summary>
        public const int CurrentSchemaVersion = 1;

        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string stepId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        [HideInInspector]
        private int schemaVersion =
            CurrentSchemaVersion;

        [SerializeField]
        private string displayName =
            "Startup Step";

        /// <summary>
        /// Gets the stable runtime-safe identity of this step definition.
        /// </summary>
        public string StepId =>
            stepId ?? string.Empty;

        /// <summary>
        /// Gets the serialized structure version of this definition.
        /// </summary>
        public int SchemaVersion =>
            schemaVersion;

        /// <summary>
        /// Gets the authored presentation label.
        /// </summary>
        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName)
                ? name
                : displayName;

        /// <summary>
        /// Returns true when the stored identity uses the canonical
        /// lowercase 32-character hexadecimal format.
        /// </summary>
        internal bool HasValidIdentity =>
            IsCanonicalStepId(stepId);

        /// <summary>
        /// Returns true when this package version understands the
        /// definition's serialized structure.
        /// </summary>
        internal bool HasSupportedSchema =>
            schemaVersion ==
            CurrentSchemaVersion;

        private static bool IsCanonicalStepId(
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

//----- StartupStepDefinition.cs END -----
