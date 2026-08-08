//----- SplashEntryAudioIntentTests.cs START -----

using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class SplashEntryAudioIntentTests
    {
        private const string BaselineSequenceId =
            "11111111111111111111111111111111";

        private const string AudioSequenceId =
            "44444444444444444444444444444444";

        private const string BaselineEntryId =
            "22222222222222222222222222222222";

        private const string AudioEntryId =
            "33333333333333333333333333333333";

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
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void PreferredAudioClipIsOptionalSerializedMetadata()
        {
            Sprite sprite =
                CreateSprite();

            SplashEntry withoutAudio =
                new SplashEntry(
                    BaselineEntryId,
                    sprite,
                    "Without Audio Intent",
                    0d,
                    0d,
                    0d,
                    0d,
                    SplashSkipPolicy
                        .AfterMinimumDisplay);

            Assert.That(
                withoutAudio.PreferredAudioClip,
                Is.Null);

            Assert.That(
                withoutAudio.HasValidDefinition,
                Is.True);

            AudioClip clip =
                CreateAudioClip(
                    "Preferred Splash Audio");

            SplashEntry withAudio =
                new SplashEntry(
                    AudioEntryId,
                    sprite,
                    "With Audio Intent",
                    0d,
                    0d,
                    0d,
                    0d,
                    SplashSkipPolicy
                        .AfterMinimumDisplay,
                    clip);

            Assert.That(
                withAudio.PreferredAudioClip,
                Is.SameAs(clip));

            Assert.That(
                withAudio.HasValidDefinition,
                Is.True);
        }

        [Test]
        public void PreferredAudioClipDoesNotChangeSplashPlayback()
        {
            Sprite sprite =
                CreateSprite();

            AudioClip clip =
                CreateAudioClip(
                    "Playback Neutral Splash Audio");

            SplashEntry baselineEntry =
                new SplashEntry(
                    BaselineEntryId,
                    sprite,
                    "Baseline",
                    0.25d,
                    0.5d,
                    0.25d,
                    0d,
                    SplashSkipPolicy
                        .AfterMinimumDisplay);

            SplashEntry audioIntentEntry =
                new SplashEntry(
                    AudioEntryId,
                    sprite,
                    "Audio Intent",
                    0.25d,
                    0.5d,
                    0.25d,
                    0d,
                    SplashSkipPolicy
                        .AfterMinimumDisplay,
                    clip);

            RecordingSplashPresenter baselinePresenter =
                new RecordingSplashPresenter();

            RecordingSplashPresenter audioPresenter =
                new RecordingSplashPresenter();

            SplashPlaybackResult baselineResult =
                Complete(
                    new SplashSequencePlayer(
                        new SplashManualClock(
                            0d,
                            0.25d),
                        baselinePresenter)
                        .PlayAsync(
                            CreateSequence(
                                BaselineSequenceId,
                                baselineEntry),
                            false,
                            CancellationToken.None));

            SplashPlaybackResult audioResult =
                Complete(
                    new SplashSequencePlayer(
                        new SplashManualClock(
                            0d,
                            0.25d),
                        audioPresenter)
                        .PlayAsync(
                            CreateSequence(
                                AudioSequenceId,
                                audioIntentEntry),
                            false,
                            CancellationToken.None));

            Assert.That(
                audioIntentEntry.PreferredAudioClip,
                Is.SameAs(clip));

            Assert.That(
                audioResult.PresentedEntryCount,
                Is.EqualTo(
                    baselineResult.PresentedEntryCount));

            Assert.That(
                audioResult.SkippedEntryCount,
                Is.EqualTo(
                    baselineResult.SkippedEntryCount));

            Assert.That(
                audioResult.ElapsedSeconds,
                Is.EqualTo(
                    baselineResult.ElapsedSeconds));

            Assert.That(
                audioResult.ReducedMotion,
                Is.EqualTo(
                    baselineResult.ReducedMotion));

            Assert.That(
                audioPresenter.Frames.Count,
                Is.EqualTo(
                    baselinePresenter.Frames.Count));

            Assert.That(
                audioPresenter.ClearCount,
                Is.EqualTo(
                    baselinePresenter.ClearCount));
        }

        private SplashSequence CreateSequence(
            string sequenceId,
            SplashEntry entry)
        {
            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(sequence);

            sequence.SetIdentityForTesting(
                sequenceId,
                SplashSequence
                    .CurrentSchemaVersion);

            sequence.SetEntriesForTesting(
                entry);

            return sequence;
        }

        private Sprite CreateSprite()
        {
            Texture2D texture =
                new Texture2D(
                    2,
                    2);

            createdAssets.Add(texture);

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

            createdAssets.Add(sprite);

            return sprite;
        }

        private AudioClip CreateAudioClip(
            string name)
        {
            AudioClip clip =
                AudioClip.Create(
                    name,
                    16,
                    1,
                    44100,
                    false);

            createdAssets.Add(clip);

            return clip;
        }

        private static T Complete<T>(
            Awaitable<T> awaitable)
        {
            Awaitable<T>.Awaiter awaiter =
                awaitable.GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The deterministic splash awaitable did not settle synchronously.");

            return awaiter.GetResult();
        }
    }
}

//----- SplashEntryAudioIntentTests.cs END -----
