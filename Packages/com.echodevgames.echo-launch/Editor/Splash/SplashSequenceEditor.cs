//----- SplashSequenceEditor.cs START -----

using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor
{
    /// <summary>
    /// Provides the normal public authoring surface for SplashSequence assets.
    ///
    /// Hidden stable entry identities remain Editor-authored through the H1
    /// utility while A1 presentation, motion, timing, audio-intent, and
    /// advancement controls remain visible and project-owned.
    /// </summary>
    [CustomEditor(
        typeof(SplashSequence))]
    internal sealed class SplashSequenceEditor :
        UnityEditor.Editor
    {
        private const string PresentationPropertyName =
            "presentationSettings";

        private const string EntriesPropertyName =
            "entries";

        private static readonly GUIContent
            AudioIntentLabel =
                new GUIContent(
                    "Audio Intent",
                    "Optional project-owned audio content intent. First Light stores this reference but does not play it.");

        private static readonly string[]
            PresentationModeLabels =
            {
                "Splash + Status",
                "Splash Only",
            };

        private static readonly int[]
            PresentationModeValues =
            {
                (int)SplashPresentationMode
                    .SplashAndStatus,
                (int)SplashPresentationMode
                    .SplashOnly,
            };

        private static readonly string[]
            AdvanceLabels =
            {
                "Automatic",
                "Skippable After Minimum",
                "Wait For Input After Minimum",
            };

        private static readonly int[]
            AdvanceValues =
            {
                (int)SplashSkipPolicy
                    .Disallowed,
                (int)SplashSkipPolicy
                    .AfterMinimumDisplay,
                (int)SplashSkipPolicy
                    .WaitForInputAfterMinimum,
            };

        private SerializedProperty
            presentationProperty;

        private SerializedProperty
            entriesProperty;

        private void OnEnable()
        {
            RefreshProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPresentation();

            EditorGUILayout.Space();

            DrawSplashes();

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedObject);

            bool appliedChanges =
                serializedObject
                    .ApplyModifiedProperties();

            if ((generatedCount > 0 ||
                 appliedChanges) &&
                target != null)
            {
                EditorUtility.SetDirty(
                    target);
            }
        }

        private void DrawPresentation()
        {
            EditorGUILayout.LabelField(
                "Presentation",
                EditorStyles.boldLabel);

            SplashSequence sequence =
                target as SplashSequence;

            if (sequence == null)
            {
                EditorGUILayout.HelpBox(
                    "SplashSequence target is unavailable.",
                    MessageType.Error);

                return;
            }

            if (!sequence
                    .HasAuthoredPresentationSettings)
            {
                EditorGUILayout.HelpBox(
                    "Legacy presentation is active: Splash + Status, black background, advancement allowed. Customize presentation to author A1 settings without changing the schema.",
                    MessageType.Info);

                if (GUILayout.Button(
                        "Customize Presentation"))
                {
                    serializedObject
                        .ApplyModifiedProperties();

                    SplashPresentationAuthoringUtility
                        .EnsurePresentationSettings(
                            serializedObject);

                    RefreshProperties();
                    serializedObject.Update();
                }
            }

            if (!sequence
                    .HasAuthoredPresentationSettings)
            {
                return;
            }

            if (presentationProperty == null)
            {
                EditorGUILayout.HelpBox(
                    "Presentation settings could not be resolved for Editor authoring.",
                    MessageType.Error);

                return;
            }

            SerializedProperty modeProperty =
                presentationProperty
                    .FindPropertyRelative(
                        "presentationMode");

            SerializedProperty
                backgroundProperty =
                    presentationProperty
                        .FindPropertyRelative(
                            "backgroundColor");

            SerializedProperty
                allowAdvanceProperty =
                    presentationProperty
                        .FindPropertyRelative(
                            "allowUserAdvance");

            DrawMappedEnum(
                modeProperty,
                "Mode",
                PresentationModeLabels,
                PresentationModeValues);

            EditorGUILayout.PropertyField(
                backgroundProperty,
                new GUIContent(
                    "Background"));

            EditorGUILayout.PropertyField(
                allowAdvanceProperty,
                new GUIContent(
                    "Allow Advancement"));

            if (!allowAdvanceProperty.boolValue &&
                ContainsWaitForInputEntry())
            {
                EditorGUILayout.HelpBox(
                    "Wait For Input After Minimum requires Allow Advancement. This sequence would be rejected before playback.",
                    MessageType.Error);
            }
        }

        private void DrawSplashes()
        {
            EditorGUILayout.LabelField(
                "Splashes",
                EditorStyles.boldLabel);

            if (entriesProperty == null ||
                !entriesProperty.isArray)
            {
                EditorGUILayout.HelpBox(
                    "Splash entries could not be resolved for Editor authoring.",
                    MessageType.Error);

                return;
            }

            for (int index = 0;
                 index < entriesProperty.arraySize;
                 index++)
            {
                SerializedProperty entryProperty =
                    entriesProperty
                        .GetArrayElementAtIndex(
                            index);

                bool removeRequested =
                    DrawEntry(
                        entryProperty,
                        index);

                if (removeRequested)
                {
                    entriesProperty
                        .DeleteArrayElementAtIndex(
                            index);

                    break;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(
                    "Add Splash"))
            {
                AppendDefaultSplash();
            }
        }

        private bool DrawEntry(
            SerializedProperty entryProperty,
            int index)
        {
            SerializedProperty labelProperty =
                entryProperty
                    .FindPropertyRelative(
                        "displayLabel");

            string displayLabel =
                labelProperty == null ||
                string.IsNullOrWhiteSpace(
                    labelProperty.stringValue)
                    ? $"Splash {index + 1}"
                    : labelProperty.stringValue.Trim();

            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox);

            entryProperty.isExpanded =
                EditorGUILayout.Foldout(
                    entryProperty.isExpanded,
                    displayLabel,
                    true);

            bool removeRequested = false;

            if (entryProperty.isExpanded)
            {
                EditorGUI.indentLevel++;

                DrawEntryFields(
                    entryProperty);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(
                    index <= 0);

                if (GUILayout.Button(
                        "Move Up"))
                {
                    entriesProperty
                        .MoveArrayElement(
                            index,
                            index - 1);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(
                    index >=
                    entriesProperty.arraySize - 1);

                if (GUILayout.Button(
                        "Move Down"))
                {
                    entriesProperty
                        .MoveArrayElement(
                            index,
                            index + 1);
                }

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button(
                        "Remove"))
                {
                    removeRequested = true;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            return removeRequested;
        }

        private void DrawEntryFields(
            SerializedProperty entryProperty)
        {
            SerializedProperty imageProperty =
                entryProperty
                    .FindPropertyRelative(
                        "image");

            SerializedProperty audioProperty =
                entryProperty
                    .FindPropertyRelative(
                        "preferredAudioClip");

            SerializedProperty labelProperty =
                entryProperty
                    .FindPropertyRelative(
                        "displayLabel");

            SerializedProperty fadeInProperty =
                entryProperty
                    .FindPropertyRelative(
                        "fadeInSeconds");

            SerializedProperty holdProperty =
                entryProperty
                    .FindPropertyRelative(
                        "holdSeconds");

            SerializedProperty fadeOutProperty =
                entryProperty
                    .FindPropertyRelative(
                        "fadeOutSeconds");

            SerializedProperty minimumProperty =
                entryProperty
                    .FindPropertyRelative(
                        "minimumDisplaySeconds");

            SerializedProperty skipProperty =
                entryProperty
                    .FindPropertyRelative(
                        "skipPolicy");

            SerializedProperty motionProperty =
                entryProperty
                    .FindPropertyRelative(
                        "motionStyle");

            SerializedProperty pulseScaleProperty =
                entryProperty
                    .FindPropertyRelative(
                        "pulseMaximumScale");

            SerializedProperty pulseCycleProperty =
                entryProperty
                    .FindPropertyRelative(
                        "pulseCycleSeconds");

            EditorGUILayout.PropertyField(
                imageProperty,
                new GUIContent(
                    "Image"));

            EditorGUILayout.PropertyField(
                audioProperty,
                AudioIntentLabel);

            EditorGUILayout.PropertyField(
                labelProperty,
                new GUIContent(
                    "Display Label"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Timing",
                EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(
                fadeInProperty,
                new GUIContent(
                    "Fade In"));

            EditorGUILayout.PropertyField(
                holdProperty,
                new GUIContent(
                    "Hold"));

            EditorGUILayout.PropertyField(
                fadeOutProperty,
                new GUIContent(
                    "Fade Out"));

            EditorGUILayout.PropertyField(
                minimumProperty,
                new GUIContent(
                    "Minimum Display"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Motion",
                EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(
                motionProperty,
                new GUIContent(
                    "Motion"));

            if (motionProperty.intValue ==
                (int)SplashMotionStyle.Pulse)
            {
                EditorGUILayout.PropertyField(
                    pulseScaleProperty,
                    new GUIContent(
                        "Maximum Scale"));

                EditorGUILayout.PropertyField(
                    pulseCycleProperty,
                    new GUIContent(
                        "Cycle Seconds"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Advancement",
                EditorStyles.miniBoldLabel);

            DrawMappedEnum(
                skipProperty,
                "Advance",
                AdvanceLabels,
                AdvanceValues);

            if (skipProperty.intValue ==
                    (int)SplashSkipPolicy
                        .WaitForInputAfterMinimum &&
                IsAdvancementDisabled())
            {
                EditorGUILayout.HelpBox(
                    "Wait For Input After Minimum requires sequence-level Allow Advancement.",
                    MessageType.Error);
            }
        }

        private void AppendDefaultSplash()
        {
            int newIndex =
                entriesProperty.arraySize;

            entriesProperty.arraySize =
                newIndex + 1;

            SerializedProperty entryProperty =
                entriesProperty
                    .GetArrayElementAtIndex(
                        newIndex);

            SetString(
                entryProperty,
                "entryId",
                string.Empty);

            SetObject(
                entryProperty,
                "image",
                null);

            SetObject(
                entryProperty,
                "preferredAudioClip",
                null);

            SetString(
                entryProperty,
                "displayLabel",
                string.Empty);

            SetFloat(
                entryProperty,
                "fadeInSeconds",
                0.25f);

            SetFloat(
                entryProperty,
                "holdSeconds",
                1f);

            SetFloat(
                entryProperty,
                "fadeOutSeconds",
                0.25f);

            SetFloat(
                entryProperty,
                "minimumDisplaySeconds",
                0f);

            SetInt(
                entryProperty,
                "skipPolicy",
                (int)SplashSkipPolicy
                    .AfterMinimumDisplay);

            SetInt(
                entryProperty,
                "motionStyle",
                (int)SplashMotionStyle.None);

            SetFloat(
                entryProperty,
                "pulseMaximumScale",
                1.05f);

            SetFloat(
                entryProperty,
                "pulseCycleSeconds",
                1f);

            entryProperty.isExpanded =
                true;
        }

        private bool ContainsWaitForInputEntry()
        {
            if (entriesProperty == null)
            {
                return false;
            }

            for (int index = 0;
                 index < entriesProperty.arraySize;
                 index++)
            {
                SerializedProperty skipProperty =
                    entriesProperty
                        .GetArrayElementAtIndex(
                            index)
                        .FindPropertyRelative(
                            "skipPolicy");

                if (skipProperty != null &&
                    skipProperty.intValue ==
                        (int)SplashSkipPolicy
                            .WaitForInputAfterMinimum)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsAdvancementDisabled()
        {
            if (presentationProperty == null)
            {
                return false;
            }

            SerializedProperty property =
                presentationProperty
                    .FindPropertyRelative(
                        "allowUserAdvance");

            return property != null &&
                   !property.boolValue;
        }

        private void RefreshProperties()
        {
            presentationProperty =
                serializedObject.FindProperty(
                    PresentationPropertyName);

            entriesProperty =
                serializedObject.FindProperty(
                    EntriesPropertyName);
        }

        private static void DrawMappedEnum(
            SerializedProperty property,
            string label,
            string[] labels,
            int[] values)
        {
            if (property == null)
            {
                EditorGUILayout.HelpBox(
                    $"{label} could not be resolved.",
                    MessageType.Error);

                return;
            }

            int selectedIndex =
                FindValueIndex(
                    values,
                    property.intValue);

            if (selectedIndex < 0)
            {
                EditorGUILayout.PropertyField(
                    property,
                    new GUIContent(label));

                EditorGUILayout.HelpBox(
                    $"{label} contains an unsupported serialized value.",
                    MessageType.Error);

                return;
            }

            int nextIndex =
                EditorGUILayout.Popup(
                    label,
                    selectedIndex,
                    labels);

            if (nextIndex >= 0 &&
                nextIndex < values.Length)
            {
                property.intValue =
                    values[nextIndex];
            }
        }

        private static int FindValueIndex(
            int[] values,
            int value)
        {
            for (int index = 0;
                 index < values.Length;
                 index++)
            {
                if (values[index] == value)
                {
                    return index;
                }
            }

            return -1;
        }

        private static void SetString(
            SerializedProperty parent,
            string propertyName,
            string value)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            if (property != null)
            {
                property.stringValue =
                    value;
            }
        }

        private static void SetObject(
            SerializedProperty parent,
            string propertyName,
            Object value)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            if (property != null)
            {
                property.objectReferenceValue =
                    value;
            }
        }

        private static void SetFloat(
            SerializedProperty parent,
            string propertyName,
            float value)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            if (property != null)
            {
                property.floatValue =
                    value;
            }
        }

        private static void SetInt(
            SerializedProperty parent,
            string propertyName,
            int value)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            if (property != null)
            {
                property.intValue =
                    value;
            }
        }
    }
}

//----- SplashSequenceEditor.cs END -----
