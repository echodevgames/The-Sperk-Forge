using System.Threading;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class EchoLaunchSimulatorWindow :
        EditorWindow
    {
        private const string MenuPath =
            "Tools/Sperk's Forge/First Light/Simulator";

        private LaunchSimulationPreset preset =
            LaunchSimulationPreset.ImmediateSuccess;

        private double logicalDurationSeconds;
        private int progressSampleCount;
        private double timeoutSeconds;
        private string message = string.Empty;
        private Vector2 reportScroll;
        private CancellationTokenSource cancellationSource;
        private LaunchSimulationReport lastReport;
        private bool isRunning;

        internal LaunchSimulationReport LastReport =>
            lastReport;

        internal bool IsRunning => isRunning;

        [MenuItem(MenuPath)]
        private static void Open()
        {
            EchoLaunchSimulatorWindow window =
                GetWindow<EchoLaunchSimulatorWindow>();

            window.titleContent =
                new GUIContent("First Light Simulator");

            window.minSize = new Vector2(560f, 520f);
            window.Show();
        }

        private void OnEnable()
        {
            titleContent =
                new GUIContent("First Light Simulator");

            minSize = new Vector2(560f, 520f);
        }

        private void OnDisable()
        {
            if (cancellationSource != null &&
                !cancellationSource.IsCancellationRequested)
            {
                cancellationSource.Cancel();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "First Light Launch Simulator",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Runs transient Editor-only startup-step scenarios through the real First Light sequence runner. It does not claim a root, play presentation, load a destination, or modify authored project assets.",
                MessageType.Info);

            EditorGUI.BeginDisabledGroup(isRunning);

            LaunchSimulationPreset selected =
                (LaunchSimulationPreset)
                EditorGUILayout.EnumPopup(
                    "Scenario",
                    preset);

            if (selected != preset)
            {
                preset = selected;
                ApplyPresetDefaults();
            }

            logicalDurationSeconds =
                EditorGUILayout.DoubleField(
                    "Logical Duration",
                    logicalDurationSeconds);

            progressSampleCount =
                EditorGUILayout.IntField(
                    "Progress Samples",
                    progressSampleCount);

            timeoutSeconds =
                EditorGUILayout.DoubleField(
                    "Timeout",
                    timeoutSeconds);

            message =
                EditorGUILayout.TextField(
                    "Optional Message",
                    message);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginDisabledGroup(isRunning);

                if (GUILayout.Button(
                        "Run Simulation",
                        GUILayout.Height(28f)))
                {
                    BeginRun();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!isRunning);

                if (GUILayout.Button(
                        "Cancel Simulation",
                        GUILayout.Height(28f)))
                {
                    CancelRun();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(
                    lastReport == null);

                if (GUILayout.Button(
                        "Copy Report",
                        GUILayout.Height(28f)))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        lastReport.Text;
                }

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                isRunning
                    ? "Status: Running"
                    : lastReport == null
                        ? "Status: Not Run"
                        : "Status: " + lastReport.Status,
                EditorStyles.boldLabel);

            reportScroll =
                EditorGUILayout.BeginScrollView(
                    reportScroll);

            EditorGUILayout.TextArea(
                lastReport != null
                    ? lastReport.Text
                    : "No simulation has been run.",
                GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }

        private async void BeginRun()
        {
            if (isRunning)
            {
                return;
            }

            LaunchSimulationRequest request =
                new LaunchSimulationRequest(
                    preset,
                    logicalDurationSeconds,
                    progressSampleCount,
                    timeoutSeconds,
                    message);

            CancellationTokenSource localSource =
                new CancellationTokenSource();

            cancellationSource = localSource;
            isRunning = true;
            Repaint();

            try
            {
                LaunchSimulationReport report =
                    await LaunchSimulationService.Shared.RunAsync(
                        request,
                        localSource.Token);

                if (this != null)
                {
                    lastReport = report;
                }
            }
            finally
            {
                if (ReferenceEquals(
                        cancellationSource,
                        localSource))
                {
                    cancellationSource = null;
                }

                localSource.Dispose();

                if (this != null)
                {
                    isRunning = false;
                    Repaint();
                }
            }
        }

        private void CancelRun()
        {
            if (cancellationSource == null ||
                cancellationSource.IsCancellationRequested)
            {
                return;
            }

            cancellationSource.Cancel();
        }

        private void ApplyPresetDefaults()
        {
            switch (preset)
            {
                case LaunchSimulationPreset
                    .TimedProgressSuccess:
                    logicalDurationSeconds = 1d;
                    progressSampleCount = 4;
                    timeoutSeconds = 0d;
                    break;

                case LaunchSimulationPreset.TimeoutStops:
                    logicalDurationSeconds = 0d;
                    progressSampleCount = 0;
                    timeoutSeconds = 0.5d;
                    break;

                default:
                    logicalDurationSeconds = 0d;
                    progressSampleCount = 0;
                    timeoutSeconds = 0d;
                    break;
            }
        }
    }
}
