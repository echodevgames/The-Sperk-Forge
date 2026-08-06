namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal static class LaunchSimulationDiagnosticCodes
    {
        internal const string InvalidRequest =
            "ELAUNCH-SIM-001";

        internal const string Busy =
            "ELAUNCH-SIM-002";

        internal const string Cancelled =
            "ELAUNCH-SIM-003";

        internal const string InfrastructureFailure =
            "ELAUNCH-SIM-004";

        internal const string SimulatedWarning =
            "ELAUNCH-SIM-STEP-001";

        internal const string SimulatedRecoverableFailure =
            "ELAUNCH-SIM-STEP-002";

        internal const string SimulatedBlockingFailure =
            "ELAUNCH-SIM-STEP-003";
    }
}
