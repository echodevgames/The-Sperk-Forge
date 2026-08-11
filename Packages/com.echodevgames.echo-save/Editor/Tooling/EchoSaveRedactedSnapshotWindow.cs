using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveRedactedSnapshotWindow :
        EditorWindow
    {
        private readonly EchoSaveInspectionService inspection =
            new EchoSaveInspectionService();

        private readonly EchoSaveSupportSnapshotService support =
            new EchoSaveSupportSnapshotService();

        private EchoSaveConfiguration configuration;
        private string selectedSlotId = string.Empty;
        private EchoSaveSupportSnapshotResult snapshot;
        private Vector2 scroll;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Export Redacted Snapshot",
            priority = 323)]
        private static void Open()
        {
            GetWindow<EchoSaveRedactedSnapshotWindow>(
                "Chronicle Redacted Snapshot");
        }

        private void OnDisable()
        {
            inspection.Dispose();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle — Redacted Support Snapshot",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Explicit support export. Participant payload contents are never read into this exporter. Full local paths and raw technical slot/generation identity are not written.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            selectedSlotId =
                EditorGUILayout.TextField(
                    "Optional Slot ID",
                    selectedSlotId);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Build Preview"))
            {
                BuildPreview();
            }

            EditorGUI.BeginDisabledGroup(
                snapshot == null ||
                !snapshot.Succeeded);

            if (GUILayout.Button(
                    "Export JSON"))
            {
                Export();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (snapshot == null)
            {
                return;
            }

            EditorGUILayout.HelpBox(
                snapshot.Message,
                snapshot.Succeeded
                    ? MessageType.Info
                    : MessageType.Error);

            if (!snapshot.Succeeded)
            {
                return;
            }

            scroll =
                EditorGUILayout.BeginScrollView(
                    scroll);

            EditorGUILayout.TextArea(
                snapshot.Json,
                GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }

        private void BuildPreview()
        {
            EchoSaveBrowserRefreshResult refresh =
                inspection.Refresh(
                    configuration);

            if (refresh == null ||
                !refresh.Succeeded)
            {
                snapshot =
                    new EchoSaveSupportSnapshotResult(
                        false,
                        string.Empty,
                        refresh?.OpenResult?.DiagnosticCode ??
                        "M504-SUPPORT-OPEN",
                        refresh?.OpenResult?.Message ??
                        "Chronicle inspection could not open.");
                return;
            }

            SaveGenerationInspectionSnapshot generations =
                null;

            if (SaveSlotId.TryParse(
                    selectedSlotId,
                    out SaveSlotId slotId))
            {
                generations =
                    inspection.InspectSlot(
                        slotId);
            }

            snapshot =
                support.Build(
                    configuration,
                    refresh.CatalogResult.Snapshot,
                    generations,
                    selectedSlotId);
        }

        private void Export()
        {
            string path =
                EditorUtility.SaveFilePanel(
                    "Export Chronicle Redacted Snapshot",
                    string.Empty,
                    "Chronicle-Redacted-Snapshot.json",
                    "json");

            if (string.IsNullOrEmpty(
                    path))
            {
                return;
            }

            File.WriteAllText(
                path,
                snapshot.Json,
                new UTF8Encoding(false));

            EditorUtility.DisplayDialog(
                "Chronicle Redacted Snapshot",
                "The payload-free redacted support snapshot was exported.",
                "OK");
        }
    }
}
