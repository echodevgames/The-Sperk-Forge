namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal enum LaunchSimulationStatus
    {
        NotRun = 0,
        Completed = 1,
        Cancelled = 2,
        InvalidRequest = 3,
        Busy = 4,
        InfrastructureFailure = 5
    }
}
