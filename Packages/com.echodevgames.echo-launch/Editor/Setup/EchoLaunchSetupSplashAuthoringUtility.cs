//----- EchoLaunchSetupSplashAuthoringUtility.cs START -----

using System;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    /// <summary>
    /// Authors A1 splash presentation data only onto a SplashSequence that
    /// was created by the active create-only Setup apply transaction.
    ///
    /// Existing/reused sequences never flow through this utility.
    /// </summary>
    internal static class EchoLaunchSetupSplashAuthoringUtility
    {
        internal static void AuthorCreatedSequence(
            string splashSequencePath,
            EchoLaunchSetupSplashAuthoringRequest request)
        {
            if (request == null)
            {
                return;
            }

            if (!request.TryValidate(
                    out string validationMessage))
            {
                throw new InvalidOperationException(
                    "Splash authoring request is invalid. " +
                    validationMessage);
            }

            string normalizedPath =
                EchoLaunchSetupPathUtility
                    .NormalizeSeparators(
                        splashSequencePath);

            SplashSequence sequence =
                AssetDatabase
                    .LoadAssetAtPath<SplashSequence>(
                        normalizedPath);

            if (sequence == null)
            {
                throw new InvalidOperationException(
                    "The newly created SplashSequence is unavailable for creation-time authoring.");
            }

            SerializedObject serialized =
                new SerializedObject(sequence);

            SerializedProperty authoredProperty =
                serialized.FindProperty(
                    "hasAuthoredPresentationSettings");

            SerializedProperty presentationProperty =
                serialized.FindProperty(
                    "presentationSettings");

            SerializedProperty entriesProperty =
                serialized.FindProperty(
                    "entries");

            if (authoredProperty == null ||
                presentationProperty == null ||
                entriesProperty == null)
            {
                throw new InvalidOperationException(
                    "SplashSequence A1 serialized fields are unavailable for Setup authoring.");
            }

            SerializedProperty modeProperty =
                presentationProperty
                    .FindPropertyRelative(
                        "presentationMode");

            SerializedProperty backgroundProperty =
                presentationProperty
                    .FindPropertyRelative(
                        "backgroundColor");

            SerializedProperty allowAdvanceProperty =
                presentationProperty
                    .FindPropertyRelative(
                        "allowUserAdvance");

            if (modeProperty == null ||
                backgroundProperty == null ||
                allowAdvanceProperty == null)
            {
                throw new InvalidOperationException(
                    "Splash presentation serialized fields are unavailable for Setup authoring.");
            }

            authoredProperty.boolValue = true;
            modeProperty.intValue =
                (int)request.PresentationMode;
            backgroundProperty.colorValue =
                request.BackgroundColor;
            allowAdvanceProperty.boolValue =
                request.AllowUserAdvance;

            entriesProperty.arraySize =
                request.Entries.Count;

            for (int index = 0;
                 index < request.Entries.Count;
                 index++)
            {
                AuthorEntry(
                    entriesProperty
                        .GetArrayElementAtIndex(
                            index),
                    request.Entries[index],
                    index);
            }

            EchoDevGames.EchoLaunch.Editor
                .SplashEntryIdentityAuthoringUtility
                .EnsureMissingEntryIdentities(
                    serialized);

            serialized
                .ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(
                sequence);

            AssetDatabase.SaveAssetIfDirty(
                sequence);
        }

        private static void AuthorEntry(
            SerializedProperty entryProperty,
            EchoLaunchSetupSplashEntryRequest request,
            int index)
        {
            Sprite image =
                AssetDatabase.LoadAssetAtPath<Sprite>(
                    request.ImagePath);

            if (image == null)
            {
                throw new InvalidOperationException(
                    $"Splash entry {index + 1} Image is unavailable at '{request.ImagePath}'.");
            }

            AudioClip audioClip = null;

            if (!string.IsNullOrEmpty(
                    request.AudioClipPath))
            {
                audioClip =
                    AssetDatabase
                        .LoadAssetAtPath<AudioClip>(
                            request.AudioClipPath);

                if (audioClip == null)
                {
                    throw new InvalidOperationException(
                        $"Splash entry {index + 1} Audio Intent is unavailable at '{request.AudioClipPath}'.");
                }
            }

            SetString(
                entryProperty,
                "entryId",
                string.Empty);

            SetObject(
                entryProperty,
                "image",
                image);

            SetObject(
                entryProperty,
                "preferredAudioClip",
                audioClip);

            SetString(
                entryProperty,
                "displayLabel",
                request.DisplayLabel);

            SetFloat(
                entryProperty,
                "fadeInSeconds",
                request.FadeInSeconds);

            SetFloat(
                entryProperty,
                "holdSeconds",
                request.HoldSeconds);

            SetFloat(
                entryProperty,
                "fadeOutSeconds",
                request.FadeOutSeconds);

            SetFloat(
                entryProperty,
                "minimumDisplaySeconds",
                request.MinimumDisplaySeconds);

            SetInt(
                entryProperty,
                "skipPolicy",
                (int)request.AdvancePolicy);

            SetInt(
                entryProperty,
                "motionStyle",
                (int)request.MotionStyle);

            SetFloat(
                entryProperty,
                "pulseMaximumScale",
                request.PulseMaximumScale);

            SetFloat(
                entryProperty,
                "pulseCycleSeconds",
                request.PulseCycleSeconds);
        }

        private static void SetString(
            SerializedProperty parent,
            string propertyName,
            string value)
        {
            SerializedProperty property =
                RequireProperty(
                    parent,
                    propertyName);

            property.stringValue =
                value ?? string.Empty;
        }

        private static void SetObject(
            SerializedProperty parent,
            string propertyName,
            UnityEngine.Object value)
        {
            SerializedProperty property =
                RequireProperty(
                    parent,
                    propertyName);

            property.objectReferenceValue =
                value;
        }

        private static void SetFloat(
            SerializedProperty parent,
            string propertyName,
            double value)
        {
            SerializedProperty property =
                RequireProperty(
                    parent,
                    propertyName);

            property.floatValue =
                checked((float)value);
        }

        private static void SetInt(
            SerializedProperty parent,
            string propertyName,
            int value)
        {
            SerializedProperty property =
                RequireProperty(
                    parent,
                    propertyName);

            property.intValue =
                value;
        }

        private static SerializedProperty
            RequireProperty(
                SerializedProperty parent,
                string propertyName)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"SplashEntry serialized field '{propertyName}' is unavailable for Setup authoring.");
            }

            return property;
        }
    }
}

//----- EchoLaunchSetupSplashAuthoringUtility.cs END -----
