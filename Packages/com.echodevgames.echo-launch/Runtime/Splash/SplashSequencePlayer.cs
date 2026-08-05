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

                bool canSkipNow =
                    entry.SkipPolicy ==
                        SplashSkipPolicy
                            .AfterMinimumDisplay &&
                    elapsedSeconds >=
                        entry.MinimumDisplaySeconds;

                SplashPresentationFrame frame =
                    CreateFrame(
                        sequenceId,
                        entry,
                        entryIndex,
                        entryCount,
                        fadeInSeconds,
                        holdSeconds,
                        fadeOutSeconds,
                        elapsedSeconds,
                        canSkipNow,
                        reducedMotion);

                presenter.PresentSplash(frame);

                bool skipRequested =
                    Volatile.Read(
                        ref skipRequestedState) != 0;

                if (skipRequested &&
                    canSkipNow)
                {
                    return true;
                }

                if (elapsedSeconds >=
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
                SplashEntry entry,
                int entryIndex,
                int entryCount,
                double fadeInSeconds,
                double holdSeconds,
                double fadeOutSeconds,
                double elapsedSeconds,
                bool canSkipNow,
                bool reducedMotion)
        {
            SplashPlaybackPhase phase;
            float alpha;

            if (fadeInSeconds > 0d &&
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
                canSkipNow,
                reducedMotion);
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
