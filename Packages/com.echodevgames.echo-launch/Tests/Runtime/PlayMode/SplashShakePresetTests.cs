//----- SplashShakePresetTests.cs START -----

using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class SplashShakePresetTests
    {
        private const string EntryId =
            "11111111111111111111111111111111";

        private Texture2D texture;
        private Sprite sprite;

        [SetUp]
        public void SetUp()
        {
            texture =
                new Texture2D(
                    2,
                    2);

            sprite =
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
        }

        [TearDown]
        public void TearDown()
        {
            if (sprite != null)
            {
                Object.DestroyImmediate(sprite);
            }

            if (texture != null)
            {
                Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void SerializedValuesRemainStable()
        {
            Assert.That(
                (int)SplashShakePreset.None,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashShakePreset.Subtle,
                Is.EqualTo(1));

            Assert.That(
                (int)SplashShakePreset.Medium,
                Is.EqualTo(2));

            Assert.That(
                (int)SplashShakePreset.Nightmare,
                Is.EqualTo(3));
        }

        [Test]
        public void DefaultEntryUsesNoShake()
        {
            SplashEntry entry =
                new SplashEntry();

            Assert.That(
                entry.ShakePreset,
                Is.EqualTo(
                    SplashShakePreset.None));
        }

        [TestCase(SplashShakePreset.None)]
        [TestCase(SplashShakePreset.Subtle)]
        [TestCase(SplashShakePreset.Medium)]
        [TestCase(SplashShakePreset.Nightmare)]
        public void AuthoredPresetIsValidDefinitionMetadata(
            SplashShakePreset preset)
        {
            SplashEntry entry =
                new SplashEntry(
                    EntryId,
                    sprite,
                    "Shake Test",
                    0.1d,
                    0.2d,
                    0.1d,
                    0d,
                    SplashSkipPolicy.AfterMinimumDisplay,
                    authoredShakePreset: preset);

            Assert.That(
                entry.ShakePreset,
                Is.EqualTo(preset));

            Assert.That(
                entry.HasValidDefinition,
                Is.True);
        }

        [Test]
        public void UndefinedPresetIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    new SplashEntry(
                        EntryId,
                        sprite,
                        "Shake Test",
                        0.1d,
                        0.2d,
                        0.1d,
                        0d,
                        SplashSkipPolicy.AfterMinimumDisplay,
                        authoredShakePreset:
                            (SplashShakePreset)999));
        }
    }
}

//----- SplashShakePresetTests.cs END -----
