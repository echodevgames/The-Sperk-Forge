//----- SplashPresentationAuthoringUtility.cs START -----

using System;
using System.Reflection;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor
{
    /// <summary>
    /// Explicitly materializes and opts into A1 presentation settings for a
    /// legacy SplashSequence when the user chooses to customize presentation.
    ///
    /// Unity can materialize inline serialized class instances while a
    /// SerializedObject is created, so null is not used as the authored-state
    /// signal. The explicit additive authored flag preserves legacy behavior.
    /// </summary>
    internal static class
        SplashPresentationAuthoringUtility
    {
        private static readonly FieldInfo
            PresentationSettingsField =
                typeof(SplashSequence)
                    .GetField(
                        "presentationSettings",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            AuthoredStateField =
                typeof(SplashSequence)
                    .GetField(
                        "hasAuthoredPresentationSettings",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        internal static bool
            EnsurePresentationSettings(
                SerializedObject serializedSequence)
        {
            if (serializedSequence == null)
            {
                throw new ArgumentNullException(
                    nameof(serializedSequence));
            }

            SplashSequence sequence =
                serializedSequence.targetObject
                    as SplashSequence;

            if (sequence == null)
            {
                throw new InvalidOperationException(
                    "Serialized target is not a SplashSequence.");
            }

            if (sequence
                    .HasAuthoredPresentationSettings)
            {
                return false;
            }

            if (PresentationSettingsField == null ||
                AuthoredStateField == null)
            {
                throw new InvalidOperationException(
                    "SplashSequence A1 presentation fields could not be resolved for Editor authoring.");
            }

            Undo.RecordObject(
                sequence,
                "Customize Splash Presentation");

            PresentationSettingsField.SetValue(
                sequence,
                new SplashPresentationSettings());

            AuthoredStateField.SetValue(
                sequence,
                true);

            EditorUtility.SetDirty(
                sequence);

            serializedSequence.Update();

            return true;
        }
    }
}

//----- SplashPresentationAuthoringUtility.cs END -----
