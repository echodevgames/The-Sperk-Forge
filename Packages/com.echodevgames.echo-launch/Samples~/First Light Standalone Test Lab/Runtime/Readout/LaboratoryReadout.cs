using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    [DisallowMultipleComponent]
    public sealed class LaboratoryReadout :
        MonoBehaviour
    {
        [SerializeField]
        private string title =
            "First Light Standalone Test Lab";

        [SerializeField]
        private bool destinationIsActive;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        public bool DestinationIsActive =>
            destinationIsActive;

        private void OnGUI()
        {
            EnsureStyles();

            EchoLaunchRoot root =
                EchoLaunchRoot.Current;

            GUILayout.BeginArea(
                new Rect(20f, 20f, 560f, 460f),
                GUI.skin.box);

            GUILayout.Label(title, titleStyle);
            GUILayout.Space(8f);
            GUILayout.Label(
                destinationIsActive
                    ? "Destination scene: ACTIVE"
                    : "Boot scene: ACTIVE",
                bodyStyle);

            if (root == null)
            {
                GUILayout.Label(
                    "Authority: None",
                    bodyStyle);
                GUILayout.EndArea();
                return;
            }

            LaunchProgressSnapshot progress =
                root.Progress;

            GUILayout.Label(
                "Authority: " +
                (root.IsAuthoritative
                    ? "Accepted"
                    : "Not accepted"),
                bodyStyle);
            GUILayout.Label(
                "Mode: " + progress.Mode,
                bodyStyle);
            GUILayout.Label(
                "State: " + root.State,
                bodyStyle);
            GUILayout.Label(
                "Message: " + progress.Message,
                bodyStyle);
            GUILayout.Label(
                "Step: " +
                (progress.ActiveStepIndex + 1) +
                " / " +
                progress.TotalStepCount,
                bodyStyle);

            if (progress.IsProgressIndeterminate)
            {
                GUILayout.Label(
                    "Progress: Indeterminate",
                    bodyStyle);
            }
            else
            {
                GUILayout.Label(
                    "Progress: " +
                    (progress.Progress01 * 100f)
                    .ToString("0.0") +
                    "%",
                    bodyStyle);
            }

            LaunchReport report =
                root.LastReport;

            if (report != null)
            {
                GUILayout.Space(8f);
                GUILayout.Label(
                    "Final report: " +
                    report.FinalStatus,
                    bodyStyle);
                GUILayout.Label(
                    "Attempted / Authored: " +
                    report.AttemptedStepCount +
                    " / " +
                    report.AuthoredEntryCount,
                    bodyStyle);
                GUILayout.Label(
                    "Warnings: " +
                    report.WarningCount,
                    bodyStyle);
                GUILayout.Label(
                    "Failures: " +
                    report.FailureCount,
                    bodyStyle);
                GUILayout.Label(
                    "Destination: " +
                    report.DestinationDisplayName,
                    bodyStyle);
            }

            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (titleStyle == null)
            {
                titleStyle =
                    new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 20,
                        fontStyle = FontStyle.Bold,
                        wordWrap = true
                    };
            }

            if (bodyStyle == null)
            {
                bodyStyle =
                    new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 14,
                        wordWrap = true
                    };
            }
        }
    }
}
