//----- SplashSequencePlayerA1Tests.cs START -----

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class SplashSequencePlayerA1Tests
    {
        private const string SequenceId =
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        private const string EntryId =
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

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
        public void WaitForInputRemainsActiveBeyondNaturalTimeline()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy
                            .WaitForInputAfterMinimum,
                        hold: 0.25d,
                        minimum: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            presenter.FramePresented =
                frame =>
                {
                    if (frame.ElapsedSeconds >= 2d)
                    {
                        presenter.RequestSkip();
                    }
                };

            SplashPlaybackResult result =
                Complete(
                    CreatePlayer(
                        presenter,
                        0.25d)
                    .PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(2d));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(0));
        }

        [Test]
        public void EarlyWaitRequestLatchesUntilMinimumThenFadesOut()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy
                            .WaitForInputAfterMinimum,
                        fadeOut: 0.5d,
                        hold: 5d,
                        minimum: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            bool requested = false;

            presenter.FramePresented =
                frame =>
                {
                    if (!requested)
                    {
                        requested = true;
                        presenter.RequestSkip();
                    }
                };

            SplashPlaybackResult result =
                Complete(
                    CreatePlayer(
                        presenter,
                        0.25d)
                    .PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(1.5d));

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.ElapsedSeconds >= 1d &&
                        frame.Phase ==
                            SplashPlaybackPhase
                                .FadeOut),
                Is.True);

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(0));
        }

        [Test]
        public void WaitForInputFramesExposeAdvanceWithoutSkip()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy
                            .WaitForInputAfterMinimum,
                        minimum: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            presenter.FramePresented =
                frame =>
                {
                    if (frame.ElapsedSeconds >= 1d)
                    {
                        presenter.RequestSkip();
                    }
                };

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            SplashPresentationFrame readyFrame =
                presenter.Frames.Find(
                    frame =>
                        frame.ElapsedSeconds >= 0.5d &&
                        frame.CanAdvanceNow);

            Assert.That(
                readyFrame,
                Is.Not.Null);

            Assert.That(
                readyFrame.CanSkipNow,
                Is.False);

            Assert.That(
                readyFrame.AdvancePolicy,
                Is.EqualTo(
                    SplashSkipPolicy
                        .WaitForInputAfterMinimum));
        }

        [Test]
        public void DisabledGlobalAdvanceIgnoresSkippableRequest()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy
                            .AfterMinimumDisplay,
                        hold: 1d,
                        minimum: 0.25d));

            sequence.SetPresentationSettingsForTesting(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    false));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            presenter.FramePresented =
                frame =>
                    presenter.RequestSkip();

            SplashPlaybackResult result =
                Complete(
                    CreatePlayer(
                        presenter,
                        0.25d)
                    .PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(1d));

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.CanAdvanceNow ||
                        frame.CanSkipNow),
                Is.False);
        }

        [Test]
        public void PulseScaleIsDeterministic()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        hold: 1d,
                        motionStyle:
                            SplashMotionStyle.Pulse,
                        pulseMaximumScale: 1.1d,
                        pulseCycleSeconds: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            AssertScaleAt(
                presenter,
                0d,
                1f);

            AssertScaleAt(
                presenter,
                0.25d,
                1.05f);

            AssertScaleAt(
                presenter,
                0.5d,
                1.1f);

            AssertScaleAt(
                presenter,
                0.75d,
                1.05f);
        }

        [Test]
        public void ReducedMotionSuppressesPulseScale()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        hold: 1d,
                        motionStyle:
                            SplashMotionStyle.Pulse,
                        pulseMaximumScale: 1.2d,
                        pulseCycleSeconds: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    true,
                    CancellationToken.None));

            foreach (
                SplashPresentationFrame frame
                in presenter.Frames)
            {
                Assert.That(
                    frame.ImageScale,
                    Is.EqualTo(1f)
                        .Within(0.0001f));
            }
        }

        [Test]
        public void NoneMotionKeepsScaleOne()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        hold: 1d,
                        motionStyle:
                            SplashMotionStyle.None));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            foreach (
                SplashPresentationFrame frame
                in presenter.Frames)
            {
                Assert.That(
                    frame.ImageScale,
                    Is.EqualTo(1f)
                        .Within(0.0001f));
            }
        }

        [Test]
        public void FrameCarriesAuthoredPresentationSettings()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        hold: 0.25d));

            Color background =
                new Color(
                    0.12f,
                    0.23f,
                    0.34f,
                    1f);

            sequence.SetPresentationSettingsForTesting(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    background,
                    true));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            SplashPresentationFrame frame =
                presenter.Frames[0];

            Assert.That(
                frame.PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode.SplashOnly));

            Assert.That(
                frame.BackgroundColor,
                Is.EqualTo(background));

            Assert.That(
                frame.AllowUserAdvance,
                Is.True);
        }

        [Test]
        public void LegacySequenceFramesCarryLegacyPresentationDefaults()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        SplashSkipPolicy.Disallowed,
                        hold: 0.25d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            Complete(
                CreatePlayer(
                    presenter,
                    0.25d)
                .PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            SplashPresentationFrame frame =
                presenter.Frames[0];

            Assert.That(
                frame.PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashAndStatus));

            Assert.That(
                frame.BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                frame.AllowUserAdvance,
                Is.True);
        }

        private SplashSequencePlayer CreatePlayer(
            RecordingSplashPresenter presenter,
            double secondsPerTick)
        {
            return new SplashSequencePlayer(
                new SplashManualClock(
                    0d,
                    secondsPerTick),
                presenter);
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
            double fadeIn = 0d,
            double hold = 0d,
            double fadeOut = 0d,
            double minimum = 0d,
            SplashMotionStyle motionStyle =
                SplashMotionStyle.None,
            double pulseMaximumScale = 1.05d,
            double pulseCycleSeconds = 1d)
        {
            return new SplashEntry(
                EntryId,
                CreateSprite(),
                "A1 Player Test",
                fadeIn,
                hold,
                fadeOut,
                minimum,
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

        private static void AssertScaleAt(
            RecordingSplashPresenter presenter,
            double elapsedSeconds,
            float expectedScale)
        {
            SplashPresentationFrame frame =
                presenter.Frames.Find(
                    candidate =>
                        Math.Abs(
                            candidate.ElapsedSeconds -
                            elapsedSeconds) <
                        0.0001d);

            Assert.That(
                frame,
                Is.Not.Null,
                $"No frame was recorded at {elapsedSeconds:0.###} seconds.");

            Assert.That(
                frame.ImageScale,
                Is.EqualTo(expectedScale)
                    .Within(0.0002f));
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

//----- SplashSequencePlayerA1Tests.cs END -----
