using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Scene-owned development readout for the standalone Laboratory.
    ///
    /// The readout observes First Light public state only. It never claims
    /// authority, starts launch execution, repairs configuration, or changes
    /// the launch result.
    /// </summary>
    public sealed class LaboratoryReadout : MonoBehaviour
    {
        [SerializeField]
        private string sceneRole = "First Light Laboratory";

        private EchoDirectSceneInitializer directSceneInitializer;

        private void Awake()
        {
            directSceneInitializer =
                Object.FindFirstObjectByType<EchoDirectSceneInitializer>();
        }

        private void OnGUI()
        {
            EchoLaunchRoot root =
                EchoLaunchRoot.Current;

            LaunchReport report =
                root == null
                    ? null
                    : root.LastReport;

            GUILayout.BeginArea(
                new Rect(
                    16f,
                    16f,
                    470f,
                    330f),
                GUI.skin.box);

            GUILayout.Label(
                sceneRole);

            GUILayout.Space(6f);

            GUILayout.Label(
                $"Scene: {gameObject.scene.name}");

            GUILayout.Label(
                root == null
                    ? "Authority: None"
                    : $"Authority: {(root.IsAuthoritative ? "Accepted" : "Rejected")}");

            GUILayout.Label(
                root == null
                    ? "State: None"
                    : $"State: {root.State}");

            GUILayout.Label(
                root == null ||
                root.Configuration == null
                    ? "Configuration: None"
                    : $"Configuration: {root.Configuration.name}");

            GUILayout.Label(
                root == null ||
                root.InitialDestination == null
                    ? "Destination: None"
                    : $"Destination: {root.InitialDestination.DisplayName}");

            if (report == null)
            {
                GUILayout.Label(
                    "Final report: None");
            }
            else
            {
                GUILayout.Label(
                    $"Final report: {report.FinalStatus}");

                GUILayout.Label(
                    $"Mode: {report.LaunchMode}");

                GUILayout.Label(
                    $"Attempted / Authored: {report.AttemptedStepCount} / {report.AuthoredEntryCount}");

                GUILayout.Label(
                    $"Warnings: {report.WarningCount}");

                GUILayout.Label(
                    $"Failures: {report.FailureCount}");

                GUILayout.Label(
                    $"Unvisited: {report.UnvisitedEntryCount}");
            }

            if (directSceneInitializer != null)
            {
                GUILayout.Space(6f);

                GUILayout.Label(
                    directSceneInitializer.HasSettled
                        ? $"Direct scene: {directSceneInitializer.LastResult.Status}"
                        : "Direct scene: Pending");

                if (directSceneInitializer.HasSettled &&
                    !string.IsNullOrEmpty(
                        directSceneInitializer
                            .LastResult
                            .DiagnosticCode))
                {
                    GUILayout.Label(
                        $"Direct diagnostic: {directSceneInitializer.LastResult.DiagnosticCode}");
                }
            }

            GUILayout.EndArea();
        }
    }
}
