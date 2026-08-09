//----- EchoLaunchSetupSplashAuthoringTests.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    public sealed class EchoLaunchSetupSplashAuthoringTests
    {
        private const string Root =
            "Assets/__EchoLaunchSetupSplashAuthoringTests";

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(
                Root);

            AssetDatabase.CreateFolder(
                "Assets",
                "__EchoLaunchSetupSplashAuthoringTests");
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(
                Root);

            AssetDatabase.Refresh();
        }

        [Test]
        public void EmptyConsumerDefaultAuthoringIsValid()
        {
            EchoLaunchSetupSplashAuthoringRequest request =
                new EchoLaunchSetupSplashAuthoringRequest(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    true,
                    Array.Empty<
                        EchoLaunchSetupSplashEntryRequest>());

            Assert.That(
                request.TryValidate(
                    out string message),
                Is.True,
                message);
        }

        [Test]
        public void WaitForInputWithoutAdvancementIsRejected()
        {
            EchoLaunchSetupSplashAuthoringRequest request =
                new EchoLaunchSetupSplashAuthoringRequest(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    false,
                    new[]
                    {
                        new EchoLaunchSetupSplashEntryRequest(
                            "Assets/logo.png",
                            string.Empty,
                            "Logo",
                            0.25d,
                            1d,
                            0.25d,
                            0.5d,
                            SplashMotionStyle.None,
                            1.05d,
                            1d,
                            SplashSkipPolicy
                                .WaitForInputAfterMinimum),
                    });

            Assert.That(
                request.TryValidate(
                    out string message),
                Is.False);

            Assert.That(
                message,
                Does.Contain(
                    "Allow Advancement"));
        }

        [Test]
        public void EntryWithoutImageIsRejected()
        {
            EchoLaunchSetupSplashEntryRequest entry =
                new EchoLaunchSetupSplashEntryRequest(
                    string.Empty,
                    string.Empty,
                    "Missing",
                    0d,
                    1d,
                    0d,
                    0d,
                    SplashMotionStyle.None,
                    1.05d,
                    1d,
                    SplashSkipPolicy.Disallowed);

            Assert.That(
                entry.TryValidate(
                    out string message),
                Is.False);

            Assert.That(
                message,
                Does.Contain("Image"));
        }

        [Test]
        public void RequestFingerprintChangesWithCreationAuthoring()
        {
            EchoLaunchSetupRequest legacyRequest =
                new EchoLaunchSetupRequest(
                    "Assets/EchoDevGames/FirstLight",
                    "Assets/EchoDevGames/FirstLight/Scenes/Boot.unity",
                    "Assets/MainMenu.unity",
                    true,
                    EchoLaunchBuildSettingsPolicy
                        .AddIfMissingAtEnd);

            EchoLaunchSetupRequest authoredRequest =
                new EchoLaunchSetupRequest(
                    "Assets/EchoDevGames/FirstLight",
                    "Assets/EchoDevGames/FirstLight/Scenes/Boot.unity",
                    "Assets/MainMenu.unity",
                    true,
                    EchoLaunchBuildSettingsPolicy
                        .AddIfMissingAtEnd,
                    splashAuthoring:
                        new EchoLaunchSetupSplashAuthoringRequest(
                            SplashPresentationMode.SplashOnly,
                            Color.black,
                            true,
                            Array.Empty<
                                EchoLaunchSetupSplashEntryRequest>()));

            Assert.That(
                EchoLaunchSetupFingerprint
                    .ForRequest(legacyRequest),
                Is.Not.EqualTo(
                    EchoLaunchSetupFingerprint
                        .ForRequest(authoredRequest)));
        }

        [Test]
        public void AuthorCreatedSequenceWritesPresentationSettings()
        {
            string sequencePath =
                Root + "/SplashSequence.asset";

            CreateValidSequenceAsset(
                sequencePath);

            EchoLaunchSetupSplashAuthoringUtility
                .AuthorCreatedSequence(
                    sequencePath,
                    new EchoLaunchSetupSplashAuthoringRequest(
                        SplashPresentationMode.SplashOnly,
                        Color.black,
                        true,
                        Array.Empty<
                            EchoLaunchSetupSplashEntryRequest>()));

            SplashSequence reloaded =
                AssetDatabase.LoadAssetAtPath<
                    SplashSequence>(
                        sequencePath);

            Assert.That(
                reloaded,
                Is.Not.Null);

            Assert.That(
                reloaded
                    .HasAuthoredPresentationSettings,
                Is.True);

            Assert.That(
                reloaded
                    .PresentationSettings
                    .PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashOnly));

            Assert.That(
                reloaded
                    .PresentationSettings
                    .BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                reloaded
                    .PresentationSettings
                    .AllowUserAdvance,
                Is.True);

            Assert.That(
                reloaded.EntryCount,
                Is.EqualTo(0));

            Assert.That(
                SplashSequence
                    .CurrentSchemaVersion,
                Is.EqualTo(1));
        }

        [Test]
        public void AuthorCreatedSequenceWritesEntryAndStableIdentity()
        {
            string imagePath =
                CreateSpriteAsset();

            string sequencePath =
                Root + "/SplashSequence.asset";

            CreateValidSequenceAsset(
                sequencePath);

            EchoLaunchSetupSplashAuthoringUtility
                .AuthorCreatedSequence(
                    sequencePath,
                    new EchoLaunchSetupSplashAuthoringRequest(
                        SplashPresentationMode.SplashOnly,
                        Color.black,
                        true,
                        new[]
                        {
                            new EchoLaunchSetupSplashEntryRequest(
                                imagePath,
                                string.Empty,
                                "Studio Logo",
                                0.75d,
                                2.5d,
                                0.75d,
                                1.5d,
                                SplashMotionStyle.Pulse,
                                1.05d,
                                1d,
                                SplashSkipPolicy
                                    .AfterMinimumDisplay),
                        }));

            SplashSequence reloaded =
                AssetDatabase.LoadAssetAtPath<
                    SplashSequence>(
                        sequencePath);

            Assert.That(
                reloaded.EntryCount,
                Is.EqualTo(1));

            SplashEntry entry =
                reloaded.GetEntry(0);

            Assert.That(
                entry.EntryId,
                Does.Match(
                    "^[0-9a-f]{32}$"));

            Assert.That(
                entry.DisplayLabel,
                Is.EqualTo(
                    "Studio Logo"));

            Assert.That(
                entry.MotionStyle,
                Is.EqualTo(
                    SplashMotionStyle.Pulse));

            Assert.That(
                entry.PulseMaximumScale,
                Is.EqualTo(1.05d)
                    .Within(0.0001d));

            Assert.That(
                entry.PulseCycleSeconds,
                Is.EqualTo(1d)
                    .Within(0.0001d));

            Assert.That(
                entry.SkipPolicy,
                Is.EqualTo(
                    SplashSkipPolicy
                        .AfterMinimumDisplay));

            Assert.DoesNotThrow(
                () =>
                    InvokeValidateForPlayback(
                        reloaded));
        }

        [Test]
        public void ProgrammaticLegacyRequestRemainsEqualWithoutAuthoring()
        {
            EchoLaunchSetupRequest left =
                new EchoLaunchSetupRequest(
                    "Assets/EchoDevGames/FirstLight",
                    "Assets/EchoDevGames/FirstLight/Scenes/Boot.unity",
                    "Assets/MainMenu.unity",
                    true,
                    EchoLaunchBuildSettingsPolicy
                        .AddIfMissingAtEnd);

            EchoLaunchSetupRequest right =
                new EchoLaunchSetupRequest(
                    "Assets/EchoDevGames/FirstLight",
                    "Assets/EchoDevGames/FirstLight/Scenes/Boot.unity",
                    "Assets/MainMenu.unity",
                    true,
                    EchoLaunchBuildSettingsPolicy
                        .AddIfMissingAtEnd);

            Assert.That(
                left,
                Is.EqualTo(right));

            Assert.That(
                left.SplashAuthoring,
                Is.Null);

            Assert.That(
                EchoLaunchSetupFingerprint
                    .ForRequest(left),
                Is.EqualTo(
                    EchoLaunchSetupFingerprint
                        .ForRequest(right)));
        }

        private static void CreateValidSequenceAsset(
            string sequencePath)
        {
            SplashSequence sequence =
                ScriptableObject
                    .CreateInstance<SplashSequence>();

            AssetDatabase.CreateAsset(
                sequence,
                sequencePath);

            SerializedObject serialized =
                new SerializedObject(
                    sequence);

            SerializedProperty sequenceId =
                serialized.FindProperty(
                    "sequenceId");

            SerializedProperty schemaVersion =
                serialized.FindProperty(
                    "schemaVersion");

            Assert.That(
                sequenceId,
                Is.Not.Null);

            Assert.That(
                schemaVersion,
                Is.Not.Null);

            sequenceId.stringValue =
                "1234567890abcdef1234567890abcdef";

            schemaVersion.intValue =
                SplashSequence.CurrentSchemaVersion;

            serialized
                .ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(
                sequence);

            AssetDatabase.SaveAssetIfDirty(
                sequence);
        }

        private static void InvokeValidateForPlayback(
            SplashSequence sequence)
        {
            MethodInfo method =
                typeof(SplashSequence)
                    .GetMethod(
                        "ValidateForPlayback",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

            Assert.That(
                method,
                Is.Not.Null,
                "SplashSequence internal playback validator could not be resolved.");

            try
            {
                method.Invoke(
                    sequence,
                    null);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException ??
                      exception;
            }
        }

        private static string CreateSpriteAsset()
        {
            string imagePath =
                Root + "/logo.png";

            Texture2D texture =
                new Texture2D(
                    2,
                    2);

            try
            {
                texture.SetPixels(
                    new[]
                    {
                        Color.white,
                        Color.white,
                        Color.white,
                        Color.white,
                    });

                texture.Apply();

                File.WriteAllBytes(
                    Path.GetFullPath(imagePath),
                    texture.EncodeToPNG());
            }
            finally
            {
                Object.DestroyImmediate(
                    texture);
            }

            AssetDatabase.ImportAsset(
                imagePath,
                ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer =
                AssetImporter.GetAtPath(
                    imagePath)
                as TextureImporter;

            Assert.That(
                importer,
                Is.Not.Null);

            importer.textureType =
                TextureImporterType.Sprite;

            importer.spriteImportMode =
                SpriteImportMode.Single;

            importer.SaveAndReimport();

            Sprite sprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(
                    imagePath);

            Assert.That(
                sprite,
                Is.Not.Null);

            return imagePath;
        }
    }
}

//----- EchoLaunchSetupSplashAuthoringTests.cs END -----
