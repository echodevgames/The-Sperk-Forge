//----- SplashSequence.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores one project-owned ordered image splash sequence.
    ///
    /// Runtime reads but never rewrites this asset.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SplashSequence",
        menuName =
            "EchoDevGames/First Light/Splash Sequence",
        order = 20)]
    public sealed class SplashSequence :
        ScriptableObject
    {
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
        private List<SplashEntry> entries =
            new List<SplashEntry>();

        /// <summary>
        /// Gets the stable runtime-safe sequence identity.
        /// </summary>
        public string SequenceId =>
            sequenceId ?? string.Empty;

        /// <summary>
        /// Gets the serialized definition schema.
        /// </summary>
        public int SchemaVersion =>
            schemaVersion;

        /// <summary>
        /// Gets the authored entry count.
        /// </summary>
        public int EntryCount =>
            entries == null
                ? 0
                : entries.Count;

        internal bool HasValidIdentity =>
            IsCanonicalId(sequenceId);

        internal bool HasSupportedSchema =>
            schemaVersion ==
            CurrentSchemaVersion;

        /// <summary>
        /// Gets one authored entry by stable order.
        /// </summary>
        public SplashEntry GetEntry(
            int index)
        {
            if (index < 0 ||
                index >= EntryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index));
            }

            return entries[index];
        }

        internal void ValidateForPlayback()
        {
            if (!HasValidIdentity)
            {
                throw new InvalidOperationException(
                    "The splash sequence has an invalid stable identity.");
            }

            if (!HasSupportedSchema)
            {
                throw new InvalidOperationException(
                    $"Splash sequence schema {schemaVersion} is unsupported. Expected {CurrentSchemaVersion}.");
            }

            if (entries == null)
            {
                throw new InvalidOperationException(
                    "The splash sequence entry collection is missing.");
            }

            HashSet<string> entryIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int index = 0;
                 index < entries.Count;
                 index++)
            {
                SplashEntry entry =
                    entries[index];

                if (entry == null)
                {
                    throw new InvalidOperationException(
                        $"Splash entry {index} is missing.");
                }

                if (!entry.HasValidDefinition)
                {
                    throw new InvalidOperationException(
                        $"Splash entry {index} is invalid.");
                }

                if (!entryIds.Add(
                        entry.EntryId))
                {
                    throw new InvalidOperationException(
                        $"Splash entry ID '{entry.EntryId}' is duplicated.");
                }
            }
        }

        internal void SetEntriesForTesting(
            params SplashEntry[] configuredEntries)
        {
            entries =
                configuredEntries == null
                    ? null
                    : new List<SplashEntry>(
                        configuredEntries);
        }

        internal void SetIdentityForTesting(
            string configuredSequenceId,
            int configuredSchemaVersion)
        {
            sequenceId =
                configuredSequenceId;

            schemaVersion =
                configuredSchemaVersion;
        }

        private static bool IsCanonicalId(
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
    }
}

//----- SplashSequence.cs END -----
