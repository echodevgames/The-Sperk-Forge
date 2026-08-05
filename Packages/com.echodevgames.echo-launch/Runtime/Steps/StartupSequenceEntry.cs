//----- StartupSequenceEntry.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores one authored position inside a startup sequence.
    ///
    /// The entry owns stable authored identity, enabled state, and one
    /// immutable step-definition reference. It does not contain active
    /// execution state.
    /// </summary>
    [Serializable]
    public sealed class StartupSequenceEntry
    {
        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string entryId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        private bool enabled = true;

        [SerializeField]
        private StartupStepDefinition stepDefinition;

        /// <summary>
        /// Gets the stable runtime-safe identity of this sequence entry.
        /// </summary>
        public string EntryId =>
            entryId ?? string.Empty;

        /// <summary>
        /// Gets whether this authored entry is enabled.
        /// </summary>
        public bool IsEnabled =>
            enabled;

        /// <summary>
        /// Gets the immutable startup-step definition referenced by this
        /// entry.
        /// </summary>
        public StartupStepDefinition StepDefinition =>
            stepDefinition;

        /// <summary>
        /// Returns true when the stored identity uses the canonical
        /// lowercase 32-character hexadecimal format.
        /// </summary>
        internal bool HasValidIdentity =>
            IsCanonicalEntryId(entryId);

        private static bool IsCanonicalEntryId(
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

//----- StartupSequenceEntry.cs END -----
