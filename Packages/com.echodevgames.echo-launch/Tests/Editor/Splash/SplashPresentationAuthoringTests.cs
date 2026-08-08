//----- SplashPresentationAuthoringTests.cs START -----

using EchoDevGames.EchoLaunch.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor
{
    public sealed class
        SplashPresentationAuthoringTests
    {
        private SplashSequence sequence;

        [SetUp]
        public void SetUp()
        {
            sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();
        }

        [TearDown]
        public void TearDown()
        {
            if (sequence != null)
            {
                Object.DestroyImmediate(
                    sequence);
            }
        }

        [Test]
        public void LegacySequenceIsNotAuthoredByDefault()
        {
            Assert.That(
                sequence
                    .HasAuthoredPresentationSettings,
                Is.False);

            Assert.That(
                sequence
                    .PresentationSettings
                    .PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashAndStatus));
        }

        [Test]
        public void CreatingSerializedObjectDoesNotOptLegacySequenceIntoA1()
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            Assert.That(
                serializedSequence,
                Is.Not.Null);

            Assert.That(
                sequence
                    .HasAuthoredPresentationSettings,
                Is.False);

            Assert.That(
                sequence
                    .PresentationSettings
                    .PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashAndStatus));
        }

        [Test]
        public void ExplicitCustomizationCreatesConsumerDefaults()
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            bool created =
                SplashPresentationAuthoringUtility
                    .EnsurePresentationSettings(
                        serializedSequence);

            Assert.That(
                created,
                Is.True);

            Assert.That(
                sequence
                    .HasAuthoredPresentationSettings,
                Is.True);

            Assert.That(
                sequence
                    .PresentationSettings
                    .PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashOnly));

            Assert.That(
                sequence
                    .PresentationSettings
                    .BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                sequence
                    .PresentationSettings
                    .AllowUserAdvance,
                Is.True);
        }

        [Test]
        public void SecondCustomizationPassDoesNotRewriteSettings()
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            SplashPresentationAuthoringUtility
                .EnsurePresentationSettings(
                    serializedSequence);

            SerializedProperty presentation =
                serializedSequence
                    .FindProperty(
                        "presentationSettings");

            presentation
                .FindPropertyRelative(
                    "presentationMode")
                .intValue =
                    (int)SplashPresentationMode
                        .SplashAndStatus;

            presentation
                .FindPropertyRelative(
                    "backgroundColor")
                .colorValue =
                    Color.magenta;

            presentation
                .FindPropertyRelative(
                    "allowUserAdvance")
                .boolValue =
                    false;

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            SerializedObject secondPass =
                new SerializedObject(
                    sequence);

            bool created =
                SplashPresentationAuthoringUtility
                    .EnsurePresentationSettings(
                        secondPass);

            Assert.That(
                created,
                Is.False);

            Assert.That(
                sequence
                    .PresentationSettings
                    .PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashAndStatus));

            Assert.That(
                sequence
                    .PresentationSettings
                    .BackgroundColor,
                Is.EqualTo(Color.magenta));

            Assert.That(
                sequence
                    .PresentationSettings
                    .AllowUserAdvance,
                Is.False);
        }

        [Test]
        public void SerializedPresentationFieldsResolveAfterCustomization()
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            SplashPresentationAuthoringUtility
                .EnsurePresentationSettings(
                    serializedSequence);

            SerializedProperty presentation =
                serializedSequence
                    .FindProperty(
                        "presentationSettings");

            Assert.That(
                presentation,
                Is.Not.Null);

            Assert.That(
                presentation
                    .FindPropertyRelative(
                        "presentationMode"),
                Is.Not.Null);

            Assert.That(
                presentation
                    .FindPropertyRelative(
                        "backgroundColor"),
                Is.Not.Null);

            Assert.That(
                presentation
                    .FindPropertyRelative(
                        "allowUserAdvance"),
                Is.Not.Null);
        }

        [Test]
        public void ExistingH1IdentityAuthoringStillWorksAfterCustomization()
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            SplashPresentationAuthoringUtility
                .EnsurePresentationSettings(
                    serializedSequence);

            SerializedProperty entries =
                serializedSequence
                    .FindProperty(
                        "entries");

            entries.arraySize = 1;

            SerializedProperty entry =
                entries
                    .GetArrayElementAtIndex(
                        0);

            entry
                .FindPropertyRelative(
                    "entryId")
                .stringValue =
                    string.Empty;

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedSequence);

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            Assert.That(
                generatedCount,
                Is.EqualTo(1));

            Assert.That(
                sequence.GetEntry(0)
                    .EntryId,
                Does.Match(
                    "^[0-9a-f]{32}$"));
        }

        [Test]
        public void SplashSequenceSchemaRemainsVersionOne()
        {
            Assert.That(
                SplashSequence
                    .CurrentSchemaVersion,
                Is.EqualTo(1));
        }
    }
}

//----- SplashPresentationAuthoringTests.cs END -----
