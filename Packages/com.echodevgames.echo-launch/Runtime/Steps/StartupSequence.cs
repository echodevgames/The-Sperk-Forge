//----- StartupSequence.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores one project-owned ordered set of startup-step entries.
    ///
    /// This asset contains authored definition data only. Active execution
    /// state must never be written into the sequence or its entries.
    /// </summary>
    [CreateAssetMenu(
        fileName = "StartupSequence",
        menuName =
            "EchoDevGames/First Light/Startup Sequence",
        order = 1)]
    public sealed class StartupSequence :
        ScriptableObject
    {
        /// <summary>
        /// Identifies the currently supported serialized structure of
        /// StartupSequence assets.
        /// </summary>
        public const int CurrentSchemaVersion = 1;

        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string sequenceId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        [HideInInspector]
        private int schemaVersion =
            CurrentSchemaVersion;

        [SerializeField]
        private List<StartupSequenceEntry> entries =
            new List<StartupSequenceEntry>();

        /// <summary>
        /// Gets the stable runtime-safe identity of this sequence.
        /// </summary>
        public string SequenceId =>
            sequenceId ?? string.Empty;

        /// <summary>
        /// Gets the serialized structure version of this sequence.
        /// </summary>
        public int SchemaVersion =>
            schemaVersion;

        /// <summary>
        /// Gets the number of authored entries in this sequence.
        /// </summary>
        public int EntryCount =>
            entries != null
                ? entries.Count
                : 0;

        /// <summary>
        /// Gets one authored entry by its current ordered position.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the index is outside the current sequence bounds.
        /// </exception>
        public StartupSequenceEntry GetEntry(
            int index)
        {
            if (entries == null ||
                index < 0 ||
                index >= entries.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "The startup-sequence entry index is outside the authored sequence bounds.");
            }

            return entries[index];
        }

        /// <summary>
        /// Returns true when the stored identity uses the canonical
        /// lowercase 32-character hexadecimal format.
        /// </summary>
        internal bool HasValidIdentity =>
            IsCanonicalSequenceId(sequenceId);

        /// <summary>
        /// Returns true when this package version understands the
        /// sequence's serialized structure.
        /// </summary>
        internal bool HasSupportedSchema =>
            schemaVersion ==
            CurrentSchemaVersion;

        private static bool IsCanonicalSequenceId(
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

//----- StartupSequence.cs END -----
