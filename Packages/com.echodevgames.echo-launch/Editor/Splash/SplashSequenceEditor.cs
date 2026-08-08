//----- SplashSequenceEditor.cs START -----

using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor
{
    /// <summary>
    /// Keeps normal SplashSequence authoring on the public Inspector surface
    /// while supplying missing hidden SplashEntry identities.
    /// </summary>
    [CustomEditor(
        typeof(SplashSequence))]
    internal sealed class SplashSequenceEditor :
        UnityEditor.Editor
    {
        private const string EntriesPropertyName =
            "entries";

        private SerializedProperty entriesProperty;

        private void OnEnable()
        {
            entriesProperty =
                serializedObject.FindProperty(
                    EntriesPropertyName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(
                entriesProperty,
                includeChildren: true);

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedObject);

            bool appliedChanges =
                serializedObject
                    .ApplyModifiedProperties();

            if (generatedCount > 0 &&
                appliedChanges)
            {
                EditorUtility.SetDirty(
                    target);
            }
        }
    }
}

//----- SplashSequenceEditor.cs END -----
