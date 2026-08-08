//----- SplashSequencePlayer.cs START -----

using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Plays one immutable image splash sequence against a monotonic clock.
    ///
    /// The player owns only temporary traversal, timing, alpha, and latched
    /// skip state. It does not own launch authority, configuration binding,
    /// scene loading, project input, or general UI navigation.
    /// </summary>
    public sealed class SplashSequencePlayer
    {
        private readonly ILaunchClock clock;
        private readonly IImageSplashPresenter presenter;

        private int activePlaybackState;
        private int skipRequestedState;

        /// <summary>
        /// Creates a runtime player using Unity's unscaled launch clock.
        /// A null presenter uses the logging-free headless fallback.
        /// </summary>
        public SplashSequencePlayer(
            IImageSplashPresenter presenter = null)
            : this(
                UnityLaunchClock.Shared,
                presenter)
        {
        }

        internal SplashSequencePlayer(
            ILaunchClock clock,
            IImageSplashPresenter presenter)
        {
            this.clock =
                clock ??
                throw new ArgumentNullException(
                    nameof(clock));

            this.presenter =
                presenter ??
                NullImageSplashPresenter.Shared;
        }

        public bool IsPlaying =>
            Volatile.Read(
                ref activePlaybackState) != 0;

        /// <summary>
        /// Plays the authored sequence once.
        ///
        /// Cancellation throws <see cref="OperationCanceledException"/>
        /// after clearing splash presentation.
        /// </summary>
        public async Awaitable<SplashPlaybackResult>
            PlayAsync(
                SplashSequence sequence,
                bool reducedMotion,
                CancellationToken cancellationToken)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(
                    nameof(sequence));
            }

            sequence.ValidateForPlayback();

            if (Interlocked.CompareExchange(
                    ref activePlaybackState,
                    1,
                    0) != 0)
            {
                throw new InvalidOperationException(
                    "The splash sequence player already owns active playback.");
            }

            double playbackStartSeconds =
                ReadClock(
                    0d,
                    false);

            int presentedEntryCount = 0;
            int skippedEntryCount = 0;

            SplashPresentationSettings presentationSettings =
                sequence.PresentationSettings;

            presenter.SkipRequested +=
                OnSkipRequested;

            try
            {
                cancellationToken
                    .ThrowIfCancellationRequested();

                for (int index = 0;
                     index < sequence.EntryCount;
                     index++)
                {
                    cancellationToken
                        .ThrowIfCancellationRequested();

                    Interlocked.Exchange(
                        ref skipRequestedState,
                        0);

                    SplashEntry entry =
                        sequence.GetEntry(index);

                    bool skipped =
                        await PlayEntryAsync(
                            sequence.SequenceId,
                            presentationSettings,
                            entry,
                            index,
                            sequence.EntryCount,
                            reducedMotion,
                            cancellationToken);

                    presentedEntryCount++;

                    if (skipped)
                    {
                        skippedEntryCount++;
                    }
                }

                double completionSeconds =
                    ReadClock(
                        playbackStartSeconds,
                        true);

                return new SplashPlaybackResult(
                    sequence.SequenceId,
                    presentedEntryCount,
                    skippedEntryCount,
                    completionSeconds -
                        playbackStartSeconds,
                    reducedMotion);
            }
            finally
            {
                presenter.SkipRequested -=
                    OnSkipRequested;

                presenter.ClearSplash();

                Interlocked.Exchange(
                    ref skipRequestedState,
                    0);

                Volatile.Write(
                    ref activePlaybackState,
                    0);
            }
        }

        private async Awaitable<bool>
            PlayEntryAsync(
                string sequenceId,
                SplashPresentationSettings presentationSettings,
                SplashEntry entry,
                int entryIndex,
                int entryCount,
                bool reducedMotion,
                CancellationToken cancellationToken)
        {
            double fadeInSeconds =
                reducedMotion
                    ? 0d
                    : entry.FadeInSeconds;

            double fadeOutSeconds =
                reducedMotion
                    ? 0d
                    : entry.FadeOutSeconds;

            double minimumHoldSeconds =
                Math.Max(
                    0d,
                    entry.MinimumDisplaySeconds -
                    fadeInSeconds -
                    fadeOutSeconds);

            double holdSeconds =
                Math.Max(
                    entry.HoldSeconds,
                    minimumHoldSeconds);

            double totalSeconds =
                fadeInSeconds +
                holdSeconds +
                fadeOutSeconds;

            double entryStartSeconds =
                ReadClock(
                    0d,
                    false);

            double previousSeconds =
                entryStartSeconds;

            bool waitsForInput =
                entry.SkipPolicy ==
                    SplashSkipPolicy
                        .WaitForInputAfterMinimum;

            double? waitFadeOutStartSeconds =
                null;

            while (true)
            {
                cancellationToken
                    .ThrowIfCancellationRequested();

                double currentSeconds =
                    ReadClock(
                        previousSeconds,
                        true);

                previousSeconds =
                    currentSeconds;

                double elapsedSeconds =
                    currentSeconds -
                    entryStartSeconds;

                bool minimumSatisfied =
                    elapsedSeconds >=
                        entry.MinimumDisplaySeconds;

                bool canAdvanceNow =
                    presentationSettings
                        .AllowUserAdvance &&
                    entry.SkipPolicy !=
                        SplashSkipPolicy
                            .Disallowed &&
                    minimumSatisfied;

                bool canSkipNow =
                    presentationSettings
                        .AllowUserAdvance &&
                    entry.SkipPolicy ==
                        SplashSkipPolicy
                            .AfterMinimumDisplay &&
                    minimumSatisfied;

                double? waitFadeOutElapsedSeconds =
                    waitFadeOutStartSeconds.HasValue
                        ? Math.Max(
                            0d,
                            currentSeconds -
                            waitFadeOutStartSeconds.Value)
                        : (double?)null;

                SplashPresentationFrame frame =
                    CreateFrame(
                        sequenceId,
                        presentationSettings,
                        entry,
                        entryIndex,
                        entryCount,
                        fadeInSeconds,
                        holdSeconds,
                        fadeOutSeconds,
                        elapsedSeconds,
                        waitFadeOutElapsedSeconds,
                        canAdvanceNow,
                        canSkipNow,
                        reducedMotion);

                presenter.PresentSplash(frame);

                bool advanceRequested =
                    Volatile.Read(
                        ref skipRequestedState) != 0;

                if (entry.SkipPolicy ==
                        SplashSkipPolicy
                            .AfterMinimumDisplay &&
                    advanceRequested &&
                    canSkipNow)
                {
                    return true;
                }

                if (waitsForInput)
                {
                    if (waitFadeOutStartSeconds.HasValue)
                    {
                        if (waitFadeOutElapsedSeconds
                                .GetValueOrDefault() >=
                            fadeOutSeconds)
                        {
                            return false;
                        }
                    }
                    else if (advanceRequested &&
                             canAdvanceNow)
                    {
                        if (fadeOutSeconds <= 0d)
                        {
                            return false;
                        }

                        waitFadeOutStartSeconds =
                            currentSeconds;
                    }
                }
                else if (elapsedSeconds >=
                         totalSeconds)
                {
                    return false;
                }

                await clock.NextTickAsync(
                    cancellationToken);
            }
        }

        private static SplashPresentationFrame
            CreateFrame(
                string sequenceId,
                SplashPresentationSettings presentationSettings,
                SplashEntry entry,
                int entryIndex,
                int entryCount,
                double fadeInSeconds,
                double holdSeconds,
                double fadeOutSeconds,
                double elapsedSeconds,
                double? waitFadeOutElapsedSeconds,
                bool canAdvanceNow,
                bool canSkipNow,
                bool reducedMotion)
        {
            SplashPlaybackPhase phase;
            float alpha;

            bool waitsForInput =
                entry.SkipPolicy ==
                    SplashSkipPolicy
                        .WaitForInputAfterMinimum;

            if (waitsForInput &&
                waitFadeOutElapsedSeconds.HasValue)
            {
                phase =
                    SplashPlaybackPhase
                        .FadeOut;

                alpha =
                    fadeOutSeconds <= 0d
                        ? 0f
                        : Mathf.Clamp01(
                            1f -
                            (float)(
                                waitFadeOutElapsedSeconds
                                    .Value /
                                fadeOutSeconds));
            }
            else if (fadeInSeconds > 0d &&
                     elapsedSeconds <
                         fadeInSeconds)
            {
                phase =
                    SplashPlaybackPhase
                        .FadeIn;

                alpha =
                    Mathf.Clamp01(
                        (float)(
                            elapsedSeconds /
                            fadeInSeconds));
            }
            else if (waitsForInput)
            {
                phase =
                    SplashPlaybackPhase
                        .Hold;

                alpha = 1f;
            }
            else if (elapsedSeconds <
                     fadeInSeconds +
                     holdSeconds)
            {
                phase =
                    SplashPlaybackPhase
                        .Hold;

                alpha = 1f;
            }
            else if (fadeOutSeconds > 0d)
            {
                phase =
                    SplashPlaybackPhase
                        .FadeOut;

                double fadeElapsed =
                    elapsedSeconds -
                    fadeInSeconds -
                    holdSeconds;

                alpha =
                    Mathf.Clamp01(
                        1f -
                        (float)(
                            fadeElapsed /
                            fadeOutSeconds));
            }
            else
            {
                phase =
                    SplashPlaybackPhase
                        .Hold;

                alpha = 1f;
            }

            float imageScale =
                CalculateImageScale(
                    entry,
                    elapsedSeconds,
                    reducedMotion);

            return new SplashPresentationFrame(
                sequenceId,
                entry,
                entryIndex,
                entryCount,
                phase,
                alpha,
                Math.Max(
                    0d,
                    elapsedSeconds),
                entry.MinimumDisplaySeconds,
                presentationSettings,
                imageScale,
                canAdvanceNow,
                canSkipNow,
                reducedMotion);
        }

        private static float CalculateImageScale(
            SplashEntry entry,
            double elapsedSeconds,
            bool reducedMotion)
        {
            if (reducedMotion ||
                entry.MotionStyle !=
                    SplashMotionStyle.Pulse)
            {
                return 1f;
            }

            double cycleSeconds =
                entry.PulseCycleSeconds;

            double normalizedCycle =
                elapsedSeconds /
                cycleSeconds;

            double pulse01 =
                0.5d -
                0.5d *
                Math.Cos(
                    normalizedCycle *
                    Math.PI *
                    2d);

            double scale =
                1d +
                (
                    entry.PulseMaximumScale -
                    1d
                ) *
                pulse01;

            return (float)scale;
        }

        private double ReadClock(
            double previousSeconds,
            bool enforceMonotonic)
        {
            double currentSeconds =
                clock.NowSeconds;

            if (double.IsNaN(
                    currentSeconds) ||
                double.IsInfinity(
                    currentSeconds) ||
                currentSeconds < 0d)
            {
                throw new InvalidOperationException(
                    "The splash playback clock returned an invalid time value.");
            }

            if (enforceMonotonic &&
                currentSeconds <
                    previousSeconds)
            {
                throw new InvalidOperationException(
                    "The splash playback clock moved backward.");
            }

            return currentSeconds;
        }

        private void OnSkipRequested()
        {
            Interlocked.Exchange(
                ref skipRequestedState,
                1);
        }
    }
}

//----- SplashSequencePlayer.cs END -----
