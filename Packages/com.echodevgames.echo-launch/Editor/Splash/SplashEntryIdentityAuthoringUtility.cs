//----- SplashEntryIdentityAuthoringUtility.cs START -----

using System;
using System.Collections.Generic;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor
{
    /// <summary>
    /// Supplies stable identities only for SplashEntry definitions whose
    /// serialized identity is currently empty.
    ///
    /// Existing non-empty values are preserved exactly. Runtime remains
    /// read-only and continues to validate malformed or duplicate IDs.
    /// </summary>
    internal static class SplashEntryIdentityAuthoringUtility
    {
        private const string EntriesPropertyName =
            "entries";

        private const string EntryIdPropertyName =
            "entryId";

        internal static int EnsureMissingEntryIdentities(
            SerializedObject serializedSequence)
        {
            if (serializedSequence == null)
            {
                throw new ArgumentNullException(
                    nameof(serializedSequence));
            }

            SerializedProperty entriesProperty =
                serializedSequence.FindProperty(
                    EntriesPropertyName);

            if (entriesProperty == null ||
                !entriesProperty.isArray)
            {
                throw new InvalidOperationException(
                    "SplashSequence entries could not be resolved for Editor authoring.");
            }

            HashSet<string> occupiedIds =
                CollectNonemptyIds(
                    entriesProperty);

            int generatedCount = 0;

            for (int index = 0;
                 index < entriesProperty.arraySize;
                 index++)
            {
                SerializedProperty entryProperty =
                    entriesProperty.GetArrayElementAtIndex(
                        index);

                SerializedProperty entryIdProperty =
                    entryProperty.FindPropertyRelative(
                        EntryIdPropertyName);

                if (entryIdProperty == null)
                {
                    throw new InvalidOperationException(
                        $"Splash entry {index} identity could not be resolved for Editor authoring.");
                }

                if (!string.IsNullOrEmpty(
                        entryIdProperty.stringValue))
                {
                    continue;
                }

                string generatedId =
                    GenerateUniqueId(
                        occupiedIds);

                entryIdProperty.stringValue =
                    generatedId;

                occupiedIds.Add(
                    generatedId);

                generatedCount++;
            }

            return generatedCount;
        }

        private static HashSet<string> CollectNonemptyIds(
            SerializedProperty entriesProperty)
        {
            HashSet<string> occupiedIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int index = 0;
                 index < entriesProperty.arraySize;
                 index++)
            {
                SerializedProperty entryProperty =
                    entriesProperty.GetArrayElementAtIndex(
                        index);

                SerializedProperty entryIdProperty =
                    entryProperty.FindPropertyRelative(
                        EntryIdPropertyName);

                if (entryIdProperty == null)
                {
                    throw new InvalidOperationException(
                        $"Splash entry {index} identity could not be resolved for Editor authoring.");
                }

                string existingId =
                    entryIdProperty.stringValue;

                if (!string.IsNullOrEmpty(
                        existingId))
                {
                    occupiedIds.Add(
                        existingId);
                }
            }

            return occupiedIds;
        }

        private static string GenerateUniqueId(
            HashSet<string> occupiedIds)
        {
            string generatedId;

            do
            {
                generatedId =
                    Guid.NewGuid().ToString(
                        "N");
            }
            while (occupiedIds.Contains(
                generatedId));

            return generatedId;
        }
    }
}

//----- SplashEntryIdentityAuthoringUtility.cs END -----
