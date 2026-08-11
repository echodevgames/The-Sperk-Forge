using System.IO;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveFailureSimulatorWindow :
        EditorWindow
    {
        private readonly EchoSaveFailureSimulatorService simulator =
            new EchoSaveFailureSimulatorService();

        private readonly EchoSaveTestDataGeneratorService generator =
            new EchoSaveTestDataGeneratorService();

        private EchoSaveConfiguration configuration;
        private string sandboxRoot = string.Empty;
        private EchoSaveFailureScenario scenario =
            EchoSaveFailureScenario.TruncateManifest;

        private EchoSaveFailureSimulationPlan preview;
        private string status = string.Empty;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Failure Simulator",
            priority = 320)]
        private static void Open()
        {
            GetWindow<EchoSaveFailureSimulatorWindow>(
                "Chronicle Failure Simulator");
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(
                    sandboxRoot))
            {
                sandboxRoot =
                    Path.Combine(
                        Application.persistentDataPath,
                        "EchoSave-M5-04-Sandbox");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle — Failure Simulator",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Sandbox-only. Preview is required before Apply. Production-root collisions are refused.",
                MessageType.Warning);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            DrawSandboxField();

            scenario =
                (EchoSaveFailureScenario)EditorGUILayout.EnumPopup(
                    "Scenario",
                    scenario);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Preview"))
            {
                preview =
                    simulator.Preview(
                        configuration,
                        sandboxRoot,
                        scenario);

                status =
                    preview.Message;
            }

            EditorGUI.BeginDisabledGroup(
                preview == null ||
                !preview.Succeeded);

            if (GUILayout.Button(
                    "Apply Sandbox Failure"))
            {
                EchoSaveToolingOperationResult result =
                    simulator.Apply(
                        configuration,
                        preview);

                status =
                    result.Message;

                preview = null;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (preview != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(
                    "Preview",
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    "Sandbox",
                    preview.SandboxRoot);

                EditorGUILayout.LabelField(
                    "Target",
                    preview.TargetRelativePath);

                EditorGUILayout.LabelField(
                    "Scenario",
                    preview.Scenario.ToString());

                EditorGUILayout.HelpBox(
                    preview.Message,
                    preview.Succeeded
                        ? MessageType.Info
                        : MessageType.Error);
            }

            if (!string.IsNullOrEmpty(
                    status))
            {
                EditorGUILayout.HelpBox(
                    status,
                    MessageType.None);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(
                    "Cleanup Owned Sandbox"))
            {
                EchoSaveToolingOperationResult result =
                    generator.Cleanup(
                        configuration,
                        sandboxRoot);

                status =
                    result.Message;

                preview = null;
            }
        }

        private void DrawSandboxField()
        {
            EditorGUILayout.BeginHorizontal();

            sandboxRoot =
                EditorGUILayout.TextField(
                    "Sandbox Root",
                    sandboxRoot);

            if (GUILayout.Button(
                    "Browse",
                    GUILayout.Width(70f)))
            {
                string selected =
                    EditorUtility.OpenFolderPanel(
                        "Select M5-04 Sandbox Root",
                        sandboxRoot,
                        string.Empty);

                if (!string.IsNullOrEmpty(
                        selected))
                {
                    sandboxRoot =
                        selected;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
