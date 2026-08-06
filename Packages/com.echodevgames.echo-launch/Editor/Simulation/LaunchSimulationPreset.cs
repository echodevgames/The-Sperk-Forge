using System;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal enum LaunchSimulationPreset
    {
        ImmediateSuccess = 0,
        TimedProgressSuccess = 1,
        WarningContinues = 2,
        RecoverableFailureContinues = 3,
        BlockingFailureStops = 4,
        TimeoutStops = 5,
        ExecutorExceptionStops = 6,
        Cancellation = 7
    }

    internal static class LaunchSimulationPresetUtility
    {
        internal static bool IsDefined(
            LaunchSimulationPreset preset)
        {
            return Enum.IsDefined(
                typeof(LaunchSimulationPreset),
                preset);
        }
    }
}
