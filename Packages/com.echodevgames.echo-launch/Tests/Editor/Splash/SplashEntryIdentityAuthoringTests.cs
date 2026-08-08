//----- SplashEntryIdentityAuthoringTests.cs START -----

using EchoDevGames.EchoLaunch.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor
{
    public sealed class SplashEntryIdentityAuthoringTests
    {
        private const string CanonicalIdPattern =
            "^[0-9a-f]{32}$";

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
        public void EmptyEntryReceivesCanonicalIdentity()
        {
            SerializedObject serializedSequence =
                ConfigureEntryIds(
                    string.Empty);

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedSequence);

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            string generatedId =
                ReadEntryId(
                    0);

            Assert.That(
                generatedCount,
                Is.EqualTo(1));

            Assert.That(
                generatedId,
                Does.Match(
                    CanonicalIdPattern));
        }

        [Test]
        public void TwoEmptyEntriesReceiveDistinctIdentities()
        {
            SerializedObject serializedSequence =
                ConfigureEntryIds(
                    string.Empty,
                    string.Empty);

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedSequence);

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            string firstId =
                ReadEntryId(
                    0);

            string secondId =
                ReadEntryId(
                    1);

            Assert.That(
                generatedCount,
                Is.EqualTo(2));

            Assert.That(
                firstId,
                Does.Match(
                    CanonicalIdPattern));

            Assert.That(
                secondId,
                Does.Match(
                    CanonicalIdPattern));

            Assert.That(
                secondId,
                Is.Not.EqualTo(
                    firstId));
        }

        [Test]
        public void ExistingNonemptyIdentityIsPreservedExactly()
        {
            const string existingId =
                "NONEMPTY-VALUE-IS-PRESERVED";

            SerializedObject serializedSequence =
                ConfigureEntryIds(
                    existingId);

            int generatedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        serializedSequence);

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            Assert.That(
                generatedCount,
                Is.Zero);

            Assert.That(
                ReadEntryId(
                    0),
                Is.EqualTo(
                    existingId));
        }

        [Test]
        public void SecondAuthoringPassDoesNotRegenerateIdentity()
        {
            SerializedObject firstPass =
                ConfigureEntryIds(
                    string.Empty);

            int firstGeneratedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        firstPass);

            firstPass
                .ApplyModifiedPropertiesWithoutUndo();

            string firstId =
                ReadEntryId(
                    0);

            SerializedObject secondPass =
                new SerializedObject(
                    sequence);

            int secondGeneratedCount =
                SplashEntryIdentityAuthoringUtility
                    .EnsureMissingEntryIdentities(
                        secondPass);

            secondPass
                .ApplyModifiedPropertiesWithoutUndo();

            string secondId =
                ReadEntryId(
                    0);

            Assert.That(
                firstGeneratedCount,
                Is.EqualTo(1));

            Assert.That(
                secondGeneratedCount,
                Is.Zero);

            Assert.That(
                secondId,
                Is.EqualTo(
                    firstId));
        }

        [Test]
        public void SplashSequenceSchemaRemainsVersionOne()
        {
            Assert.That(
                SplashSequence
                    .CurrentSchemaVersion,
                Is.EqualTo(1));
        }

        private SerializedObject ConfigureEntryIds(
            params string[] entryIds)
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            SerializedProperty entriesProperty =
                serializedSequence
                    .FindProperty(
                        "entries");

            entriesProperty.arraySize =
                entryIds.Length;

            for (int index = 0;
                 index < entryIds.Length;
                 index++)
            {
                SerializedProperty entryProperty =
                    entriesProperty
                        .GetArrayElementAtIndex(
                            index);

                SerializedProperty entryIdProperty =
                    entryProperty
                        .FindPropertyRelative(
                            "entryId");

                entryIdProperty.stringValue =
                    entryIds[index];
            }

            serializedSequence
                .ApplyModifiedPropertiesWithoutUndo();

            return new SerializedObject(
                sequence);
        }

        private string ReadEntryId(
            int index)
        {
            SerializedObject serializedSequence =
                new SerializedObject(
                    sequence);

            SerializedProperty entryProperty =
                serializedSequence
                    .FindProperty(
                        "entries")
                    .GetArrayElementAtIndex(
                        index);

            return entryProperty
                .FindPropertyRelative(
                    "entryId")
                .stringValue;
        }
    }
}

//----- SplashEntryIdentityAuthoringTests.cs END -----
