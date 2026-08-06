using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationLogicalClock :
        ILaunchClock
    {
        private readonly double tickSeconds;
        private double nowSeconds;

        internal LaunchSimulationLogicalClock(
            double tickSeconds)
        {
            if (double.IsNaN(tickSeconds) ||
                double.IsInfinity(tickSeconds) ||
                tickSeconds <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tickSeconds),
                    tickSeconds,
                    "Logical clock tick duration must be finite and positive.");
            }

            this.tickSeconds = tickSeconds;
        }

        public double NowSeconds => nowSeconds;

        public async Awaitable NextTickAsync(
            CancellationToken cancellationToken)
        {
            await LaunchSimulationEditorTick.NextAsync(
                cancellationToken);

            nowSeconds += tickSeconds;
        }
    }
}
