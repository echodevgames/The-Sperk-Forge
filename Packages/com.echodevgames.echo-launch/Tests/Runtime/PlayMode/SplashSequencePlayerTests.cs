//----- SplashSequencePlayerTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class SplashManualClock :
        ILaunchClock
    {
        internal SplashManualClock(
            double initialSeconds,
            double secondsPerTick)
        {
            CurrentSeconds =
                initialSeconds;

            SecondsPerTick =
                secondsPerTick;
        }

        internal double CurrentSeconds
        {
            get;
            set;
        }

        internal double SecondsPerTick
        {
            get;
            set;
        }

        internal int TickCount
        {
            get;
            private set;
        }

        public double NowSeconds =>
            CurrentSeconds;

#pragma warning disable CS1998
        public async Awaitable NextTickAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            TickCount++;

            CurrentSeconds +=
                SecondsPerTick;
        }
#pragma warning restore CS1998
    }

    internal sealed class RecordingSplashPresenter :
        IImageSplashPresenter
    {
        internal readonly List<
            SplashPresentationFrame> Frames =
                new List<
                    SplashPresentationFrame>();

        internal int ClearCount
        {
            get;
            private set;
        }

        internal Action<SplashPresentationFrame>
            FramePresented
        {
            get;
            set;
        }

        public event Action SkipRequested;

        public void PresentSplash(
            SplashPresentationFrame frame)
        {
            Frames.Add(frame);

            FramePresented?.Invoke(frame);
        }

        public void ClearSplash()
        {
            ClearCount++;
        }

        internal void RequestSkip()
        {
            SkipRequested?.Invoke();
        }
    }

    public sealed class SplashSequencePlayerTests
    {
        private const string SequenceId =
            "11111111111111111111111111111111";

        private const string FirstEntryId =
            "22222222222222222222222222222222";

        private const string SecondEntryId =
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
        public void SkipPolicyVocabularyIsStable()
        {
            Assert.That(
                (int)SplashSkipPolicy.Disallowed,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashSkipPolicy
                    .AfterMinimumDisplay,
                Is.EqualTo(1));
        }

        [Test]
        public void PlaybackPhaseVocabularyIsStable()
        {
            Assert.That(
                (int)SplashPlaybackPhase.None,
                Is.EqualTo(0));

            Assert.That(
                (int)SplashPlaybackPhase.FadeIn,
                Is.EqualTo(1));

            Assert.That(
                (int)SplashPlaybackPhase.Hold,
                Is.EqualTo(2));

            Assert.That(
                (int)SplashPlaybackPhase.FadeOut,
                Is.EqualTo(3));
        }

        [Test]
        public void NewSequenceUsesSchemaOneAndCanonicalIdentity()
        {
            SplashSequence sequence =
                CreateSequence();

            Assert.That(
                SplashSequence
                    .CurrentSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                sequence.SequenceId,
                Does.Match(
                    "^[0-9a-f]{32}$"));
        }

        [Test]
        public void SeparateSequencesReceiveDifferentIds()
        {
            SplashSequence first =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            SplashSequence second =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(first);
            createdAssets.Add(second);

            Assert.That(
                second.SequenceId,
                Is.Not.EqualTo(
                    first.SequenceId));
        }

        [Test]
        public void EntryUsesCanonicalIdentity()
        {
            SplashEntry entry =
                CreateEntry(
                    FirstEntryId);

            Assert.That(
                entry.EntryId,
                Does.Match(
                    "^[0-9a-f]{32}$"));

            Assert.That(
                entry.HasValidIdentity,
                Is.True);
        }

        [Test]
        public void EntryRejectsNegativeTiming()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new SplashEntry(
                        FirstEntryId,
                        CreateSprite(),
                        "Invalid",
                        -0.01d,
                        1d,
                        0d,
                        0d,
                        SplashSkipPolicy
                            .Disallowed));
        }

        [Test]
        public void EntryRejectsNonfiniteTiming()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new SplashEntry(
                        FirstEntryId,
                        CreateSprite(),
                        "Invalid",
                        double.NaN,
                        1d,
                        0d,
                        0d,
                        SplashSkipPolicy
                            .Disallowed));
        }

        [Test]
        public void SequenceRejectsNullEntry()
        {
            SplashSequence sequence =
                CreateSequence(null);

            Assert.Throws<
                InvalidOperationException>(
                sequence
                    .ValidateForPlayback);
        }

        [Test]
        public void SequenceRejectsDuplicateEntryIds()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId),
                    CreateEntry(
                        FirstEntryId));

            Assert.Throws<
                InvalidOperationException>(
                sequence
                    .ValidateForPlayback);
        }

        [Test]
        public void SequenceRejectsMissingImage()
        {
            SplashEntry entry =
                new SplashEntry(
                    FirstEntryId,
                    null,
                    "Missing Image",
                    0d,
                    0d,
                    0d,
                    0d,
                    SplashSkipPolicy
                        .Disallowed);

            SplashSequence sequence =
                CreateSequence(entry);

            Assert.Throws<
                InvalidOperationException>(
                sequence
                    .ValidateForPlayback);
        }

        [Test]
        public void EmptySequenceCompletesWithoutFrames()
        {
            SplashSequence sequence =
                CreateSequence();

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashManualClock clock =
                new SplashManualClock(
                    0d,
                    0.25d);

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    clock,
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.PresentedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                presenter.Frames,
                Is.Empty);

            Assert.That(
                presenter.ClearCount,
                Is.EqualTo(1));
        }

        [Test]
        public void OneEntryCompletesAuthoredTimeline()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        fadeIn: 0.5d,
                        hold: 0.5d,
                        fadeOut: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.PresentedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                presenter.Frames.Count,
                Is.GreaterThan(1));

            Assert.That(
                presenter.ClearCount,
                Is.EqualTo(1));
        }

        [Test]
        public void TwoEntriesPreserveAuthoredOrder()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 0.25d),
                    CreateEntry(
                        SecondEntryId,
                        hold: 0.25d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            Complete(
                player.PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            int firstSecondEntryFrame =
                presenter.Frames.FindIndex(
                    frame =>
                        frame.EntryId ==
                        SecondEntryId);

            Assert.That(
                firstSecondEntryFrame,
                Is.GreaterThan(0));

            for (int index = 0;
                 index < firstSecondEntryFrame;
                 index++)
            {
                Assert.That(
                    presenter.Frames[index]
                        .EntryId,
                    Is.EqualTo(
                        FirstEntryId));
            }
        }

        [Test]
        public void FadeTimelinePublishesExpectedPhases()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        fadeIn: 0.5d,
                        hold: 0.5d,
                        fadeOut: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            Complete(
                player.PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.Phase ==
                        SplashPlaybackPhase
                            .FadeIn),
                Is.True);

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.Phase ==
                        SplashPlaybackPhase
                            .Hold),
                Is.True);

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.Phase ==
                        SplashPlaybackPhase
                            .FadeOut),
                Is.True);
        }

        [Test]
        public void FadeAlphaIsNormalized()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        fadeIn: 0.5d,
                        hold: 0.25d,
                        fadeOut: 0.5d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            Complete(
                player.PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            foreach (
                SplashPresentationFrame frame
                in presenter.Frames)
            {
                Assert.That(
                    frame.Alpha,
                    Is.InRange(
                        0f,
                        1f));
            }
        }

        [Test]
        public void MinimumDisplayExtendsHold()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 0.25d,
                        minimum: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashManualClock clock =
                new SplashManualClock(
                    0d,
                    0.25d);

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    clock,
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    1d));

            Assert.That(
                clock.TickCount,
                Is.GreaterThanOrEqualTo(
                    4));
        }

        [Test]
        public void SkipAfterMinimumEndsEntry()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 5d,
                        minimum: 0.5d,
                        skipPolicy:
                            SplashSkipPolicy
                                .AfterMinimumDisplay));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            presenter.FramePresented =
                frame =>
                {
                    if (frame.CanSkipNow)
                    {
                        presenter.RequestSkip();
                    }
                };

            SplashManualClock clock =
                new SplashManualClock(
                    0d,
                    0.25d);

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    clock,
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    0.5d));

            Assert.That(
                result.ElapsedSeconds,
                Is.LessThan(5d));
        }

        [Test]
        public void EarlySkipWaitsForMinimum()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 5d,
                        minimum: 1d,
                        skipPolicy:
                            SplashSkipPolicy
                                .AfterMinimumDisplay));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            bool requested = false;

            presenter.FramePresented =
                frame =>
                {
                    if (requested)
                    {
                        return;
                    }

                    requested = true;
                    presenter.RequestSkip();
                };

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    1d));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(1));
        }

        [Test]
        public void DisallowedSkipIsIgnored()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 1d,
                        minimum: 0d,
                        skipPolicy:
                            SplashSkipPolicy
                                .Disallowed));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            bool requested = false;

            presenter.FramePresented =
                frame =>
                {
                    if (requested)
                    {
                        return;
                    }

                    requested = true;
                    presenter.RequestSkip();
                };

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(0));

            Assert.That(
                result.ElapsedSeconds,
                Is.GreaterThanOrEqualTo(
                    1d));
        }

        [Test]
        public void ReducedMotionRemovesFadePhases()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        fadeIn: 1d,
                        hold: 0.5d,
                        fadeOut: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        true,
                        CancellationToken.None));

            Assert.That(
                result.ReducedMotion,
                Is.True);

            Assert.That(
                presenter.Frames.Exists(
                    frame =>
                        frame.Phase ==
                            SplashPlaybackPhase
                                .FadeIn ||
                        frame.Phase ==
                            SplashPlaybackPhase
                                .FadeOut),
                Is.False);
        }

        [Test]
        public void CancellationBeforePlaybackClearsPresenter()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            CancellationTokenSource source =
                new CancellationTokenSource();

            source.Cancel();

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    presenter);

            Assert.Throws<
                OperationCanceledException>(
                () =>
                    Complete(
                        player.PlayAsync(
                            sequence,
                            false,
                            source.Token)));

            Assert.That(
                presenter.ClearCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ConcurrentPlaybackIsRejected()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 1d));

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    new RecordingSplashPresenter());

            SetPrivateField(
                player,
                "activePlaybackState",
                1);

            Assert.Throws<
                InvalidOperationException>(
                () =>
                    Complete(
                        player.PlayAsync(
                            sequence,
                            false,
                            CancellationToken.None)));
        }

        [Test]
        public void BackwardClockIsRejectedAndPresentationClears()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 1d));

            RecordingSplashPresenter presenter =
                new RecordingSplashPresenter();

            SplashManualClock clock =
                new SplashManualClock(
                    1d,
                    -0.25d);

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    clock,
                    presenter);

            Assert.Throws<
                InvalidOperationException>(
                () =>
                    Complete(
                        player.PlayAsync(
                            sequence,
                            false,
                            CancellationToken.None)));

            Assert.That(
                presenter.ClearCount,
                Is.EqualTo(1));
        }

        [Test]
        public void NullPresenterUsesHeadlessFallback()
        {
            SplashSequence sequence =
                CreateSequence(
                    CreateEntry(
                        FirstEntryId,
                        hold: 0.25d));

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    null);

            SplashPlaybackResult result =
                Complete(
                    player.PlayAsync(
                        sequence,
                        false,
                        CancellationToken.None));

            Assert.That(
                result.PresentedEntryCount,
                Is.EqualTo(1));
        }

        [Test]
        public void PlaybackResultReportsSkippedEntry()
        {
            SplashPlaybackResult result =
                new SplashPlaybackResult(
                    SequenceId,
                    2,
                    1,
                    3d,
                    false);

            Assert.That(
                result.WasAnyEntrySkipped,
                Is.True);

            Assert.That(
                result.SkippedEntryCount,
                Is.EqualTo(1));
        }

        [Test]
        public void PlaybackDoesNotMutateAuthoredAssets()
        {
            SplashEntry entry =
                CreateEntry(
                    FirstEntryId,
                    fadeIn: 0.25d,
                    hold: 0.5d,
                    fadeOut: 0.25d,
                    minimum: 0.75d);

            SplashSequence sequence =
                CreateSequence(entry);

            string originalSequenceId =
                sequence.SequenceId;

            int originalSchema =
                sequence.SchemaVersion;

            string originalEntryId =
                entry.EntryId;

            double originalHold =
                entry.HoldSeconds;

            SplashSequencePlayer player =
                new SplashSequencePlayer(
                    new SplashManualClock(
                        0d,
                        0.25d),
                    new RecordingSplashPresenter());

            Complete(
                player.PlayAsync(
                    sequence,
                    false,
                    CancellationToken.None));

            Assert.That(
                sequence.SequenceId,
                Is.EqualTo(
                    originalSequenceId));

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(
                    originalSchema));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(
                    originalEntryId));

            Assert.That(
                entry.HoldSeconds,
                Is.EqualTo(
                    originalHold));
        }

        private SplashSequence CreateSequence(
            params SplashEntry[] entries)
        {
            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(sequence);

            sequence.SetIdentityForTesting(
                SequenceId,
                SplashSequence
                    .CurrentSchemaVersion);

            sequence.SetEntriesForTesting(
                entries);

            return sequence;
        }

        private SplashEntry CreateEntry(
            string entryId,
            double fadeIn = 0d,
            double hold = 0d,
            double fadeOut = 0d,
            double minimum = 0d,
            SplashSkipPolicy skipPolicy =
                SplashSkipPolicy
                    .AfterMinimumDisplay)
        {
            return new SplashEntry(
                entryId,
                CreateSprite(),
                $"Splash {entryId[0]}",
                fadeIn,
                hold,
                fadeOut,
                minimum,
                skipPolicy);
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
                Is.Not.Null,
                $"Missing field {fieldName}.");

            field.SetValue(
                target,
                value);
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

//----- SplashSequencePlayerTests.cs END -----
