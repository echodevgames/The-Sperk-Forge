//----- SplashPresentationDefinitionTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class SplashPresentationDefinitionTests
    {
        private const string SequenceId =
            "11111111111111111111111111111111";

        private const string EntryId =
            "22222222222222222222222222222222";

        private readonly List<Object> createdAssets =
            new List<Object>();

        [TearDown]
        public void TearDown()
        {
            for (int index =
                     createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object asset =
                    createdAssets[index];

                if (asset != null)
                {
                    Object.DestroyImmediate(
                        asset);
                }
            }

            createdAssets.Clear();
        }

        [Test]
        public void A1VocabularyPreservesExistingNumericValues()
        {
            Assert.That(
                (int)SplashSkipPolicy.Disallowed,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashSkipPolicy.AfterMinimumDisplay,
                Is.EqualTo(1));

            Assert.That(
                (int)SplashSkipPolicy.WaitForInputAfterMinimum,
                Is.EqualTo(2));

            Assert.That(
                (int)SplashPresentationMode.SplashAndStatus,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashPresentationMode.SplashOnly,
                Is.EqualTo(1));

            Assert.That(
                (int)SplashMotionStyle.None,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashMotionStyle.Pulse,
                Is.EqualTo(1));
        }

        [Test]
        public void SplashSequenceSchemaRemainsOne()
        {
            Assert.That(
                SplashSequence.CurrentSchemaVersion,
                Is.EqualTo(1));
        }

        [Test]
        public void MissingPresentationSettingsUseLegacyDefaults()
        {
            SplashSequence sequence =
                CreateSequence();

            Assert.That(
                sequence.HasAuthoredPresentationSettings,
                Is.False);

            Assert.That(
                sequence.PresentationSettings.PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode.SplashAndStatus));

            Assert.That(
                sequence.PresentationSettings.BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                sequence.PresentationSettings.AllowUserAdvance,
                Is.True);

            Assert.DoesNotThrow(
                sequence.ValidateForPlayback);
        }

        [Test]
        public void ExplicitNewPresentationSettingsUseConsumerDefaults()
        {
            SplashPresentationSettings settings =
                new SplashPresentationSettings();

            Assert.That(
                settings.PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode.SplashOnly));

            Assert.That(
                settings.BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                settings.AllowUserAdvance,
                Is.True);
        }

        [Test]
        public void AuthoredPresentationSettingsRoundTripThroughSequence()
        {
            SplashSequence sequence =
                CreateSequence();

            SplashPresentationSettings settings =
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    new Color(
                        0.1f,
                        0.2f,
                        0.3f,
                        1f),
                    false);

            sequence.SetPresentationSettingsForTesting(
                settings);

            Assert.That(
                sequence.HasAuthoredPresentationSettings,
                Is.True);

            Assert.That(
                sequence.PresentationSettings,
                Is.SameAs(settings));

            Assert.That(
                sequence.PresentationSettings.AllowUserAdvance,
                Is.False);

            Assert.DoesNotThrow(
                sequence.ValidateForPlayback);
        }

        [Test]
        public void PulseEntryStoresBoundedMotionMetadata()
        {
            SplashEntry entry =
                CreateEntry(
                    SplashSkipPolicy.Disallowed,
                    SplashMotionStyle.Pulse,
                    1.08d,
                    0.8d);

            Assert.That(
                entry.MotionStyle,
                Is.EqualTo(
                    SplashMotionStyle.Pulse));

            Assert.That(
                entry.PulseMaximumScale,
                Is.EqualTo(1.08d)
                    .Within(0.0001d));

            Assert.That(
                entry.PulseCycleSeconds,
                Is.EqualTo(0.8d)
                    .Within(0.0001d));

            SplashSequence sequence =
                CreateSequence(entry);

            Assert.DoesNotThrow(
                sequence.ValidateForPlayback);
        }

        [Test]
        public void PulseEntryRejectsScaleBelowOne()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        SplashMotionStyle.Pulse,
                        0.99d,
                        1d));
        }

        [Test]
        public void PulseEntryRejectsNonpositiveCycle()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        SplashMotionStyle.Pulse,
                        1.05d,
                        0d));
        }

        [Test]
        public void UndefinedPresentationModeIsRejected()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new SplashPresentationSettings(
                        (SplashPresentationMode)99,
                        Color.black,
                        true));
        }

        [Test]
        public void NonfiniteBackgroundColorIsRejected()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new SplashPresentationSettings(
                        SplashPresentationMode.SplashOnly,
                        new Color(
                            float.NaN,
                            0f,
                            0f,
                            1f),
                        true));
        }

        [Test]
        public void DisabledAdvancementRejectsWaitForInputEntry()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.WaitForInputAfterMinimum));

            sequence.SetPresentationSettingsForTesting(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    false));

            Assert.Throws<
                InvalidOperationException>(
                sequence.ValidateForPlayback);
        }

        [Test]
        public void DisabledAdvancementAllowsAutomaticTimelineEntry()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed));

            sequence.SetPresentationSettingsForTesting(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    false));

            Assert.DoesNotThrow(
                sequence.ValidateForPlayback);
        }

        [Test]
        public void SerializedUndefinedPresentationModeIsRejectedBySequence()
        {
            SplashPresentationSettings settings =
                new SplashPresentationSettings();

            SetPrivateField(
                settings,
                "presentationMode",
                (SplashPresentationMode)99);

            SplashSequence sequence =
                CreateSequence();

            sequence.SetPresentationSettingsForTesting(
                settings);

            Assert.Throws<
                InvalidOperationException>(
                sequence.ValidateForPlayback);
        }

        private SplashSequence CreateSequence(
            params SplashEntry[] entries)
        {
            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(
                sequence);

            sequence.SetIdentityForTesting(
                SequenceId,
                SplashSequence.CurrentSchemaVersion);

            sequence.SetEntriesForTesting(
                entries);

            return sequence;
        }

        private SplashEntry CreateEntry(
            SplashSkipPolicy skipPolicy,
            SplashMotionStyle motionStyle =
                SplashMotionStyle.None,
            double pulseMaximumScale = 1.05d,
            double pulseCycleSeconds = 1d)
        {
            return new SplashEntry(
                EntryId,
                CreateSprite(),
                "A1 Test Splash",
                0.25d,
                1d,
                0.25d,
                0.5d,
                skipPolicy,
                null,
                motionStyle,
                pulseMaximumScale,
                pulseCycleSeconds);
        }

        private Sprite CreateSprite()
        {
            Texture2D texture =
                new Texture2D(
                    2,
                    2);

            createdAssets.Add(
                texture);

            Sprite sprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        2f,
                        2f),
                    new Vector2(
                        0.5f,
                        0.5f));

            createdAssets.Add(
                sprite);

            return sprite;
        }

        private static void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field =
                target.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                field,
                Is.Not.Null);

            field.SetValue(
                target,
                value);
        }
    }
}

//----- SplashPresentationDefinitionTests.cs END -----
