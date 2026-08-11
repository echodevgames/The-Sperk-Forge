using System.IO;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveTestDataGeneratorWindow :
        EditorWindow
    {
        private readonly EchoSaveTestDataGeneratorService service =
            new EchoSaveTestDataGeneratorService();

        private EchoSaveConfiguration configuration;
        private string sandboxRoot = string.Empty;
        private int slotCount = 2;
        private int generationsPerSlot = 2;
        private int payloadPaddingBytes = 64;
        private int seed = 504;

        private EchoSaveTestDataPlan preview;
        private string status = string.Empty;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Test Data Generator",
            priority = 322)]
        private static void Open()
        {
            GetWindow<EchoSaveTestDataGeneratorWindow>(
                "Chronicle Test Data");
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
                "The Chronicle — Bounded Test Data",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Synthetic records are sandbox-only. Counts and byte padding are hard bounded; production-root collisions are refused.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            DrawSandboxField();

            slotCount =
                EditorGUILayout.IntField(
                    "Slot Count",
                    slotCount);

            generationsPerSlot =
                EditorGUILayout.IntField(
                    "Generations / Slot",
                    generationsPerSlot);

            payloadPaddingBytes =
                EditorGUILayout.IntField(
                    "Payload Padding Bytes",
                    payloadPaddingBytes);

            seed =
                EditorGUILayout.IntField(
                    "Deterministic Seed",
                    seed);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Preview"))
            {
                preview =
                    service.Preview(
                        configuration,
                        sandboxRoot,
                        Request());

                status =
                    preview.Message;
            }

            EditorGUI.BeginDisabledGroup(
                preview == null ||
                !preview.Succeeded);

            if (GUILayout.Button(
                    "Generate"))
            {
                EchoSaveToolingOperationResult result =
                    service.Generate(
                        configuration,
                        sandboxRoot,
                        Request());

                status =
                    result.Message;

                preview = null;
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(
                    "Cleanup"))
            {
                EchoSaveToolingOperationResult result =
                    service.Cleanup(
                        configuration,
                        sandboxRoot);

                status =
                    result.Message;

                preview = null;
            }

            EditorGUILayout.EndHorizontal();

            if (preview != null)
            {
                EditorGUILayout.LabelField(
                    "Preview",
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    "Slots",
                    preview.SlotCount.ToString());

                EditorGUILayout.LabelField(
                    "Generations",
                    preview.GenerationCount.ToString());

                EditorGUILayout.LabelField(
                    "Estimated Bytes",
                    preview.EstimatedBytes.ToString());

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
        }

        private EchoSaveTestDataRequest Request()
        {
            return new EchoSaveTestDataRequest(
                slotCount,
                generationsPerSlot,
                payloadPaddingBytes,
                seed);
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
