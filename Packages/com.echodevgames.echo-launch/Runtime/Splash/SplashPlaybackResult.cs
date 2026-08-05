//----- SplashPlaybackResult.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores the immutable completed outcome of one splash sequence run.
    /// </summary>
    public sealed class SplashPlaybackResult
    {
        internal SplashPlaybackResult(
            string sequenceId,
            int presentedEntryCount,
            int skippedEntryCount,
            double elapsedSeconds,
            bool reducedMotion)
        {
            if (string.IsNullOrWhiteSpace(
                    sequenceId))
            {
                throw new ArgumentException(
                    "A splash result requires sequence identity.",
                    nameof(sequenceId));
            }

            if (presentedEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(presentedEntryCount));
            }

            if (skippedEntryCount < 0 ||
                skippedEntryCount >
                    presentedEntryCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(skippedEntryCount));
            }

            if (double.IsNaN(elapsedSeconds) ||
                double.IsInfinity(elapsedSeconds) ||
                elapsedSeconds < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedSeconds));
            }

            SequenceId =
                sequenceId.Trim();

            PresentedEntryCount =
                presentedEntryCount;

            SkippedEntryCount =
                skippedEntryCount;

            ElapsedSeconds =
                elapsedSeconds;

            ReducedMotion =
                reducedMotion;
        }

        public string SequenceId
        {
            get;
        }

        public int PresentedEntryCount
        {
            get;
        }

        public int SkippedEntryCount
        {
            get;
        }

        public double ElapsedSeconds
        {
            get;
        }

        public bool ReducedMotion
        {
            get;
        }

        public bool WasAnyEntrySkipped =>
            SkippedEntryCount > 0;
    }
}

//----- SplashPlaybackResult.cs END -----
