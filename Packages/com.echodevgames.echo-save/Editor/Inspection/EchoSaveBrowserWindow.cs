using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveBrowserWindow :
        EditorWindow
    {
        private enum InspectionView
        {
            Browser = 0,
            MigrationGraph = 1
        }

        private readonly EchoSaveInspectionService service =
            new EchoSaveInspectionService();

        private EchoSaveConfiguration configuration;
        private EchoSaveBrowserRefreshResult refreshResult;
        private SaveGenerationInspectionSnapshot generationSnapshot;
        private string selectedSlotId = string.Empty;
        private Vector2 scroll;
        private InspectionView view;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Save Browser",
            priority = 310)]
        private static void OpenBrowser()
        {
            EchoSaveBrowserWindow window =
                GetWindow<EchoSaveBrowserWindow>(
                    "Chronicle Save Browser");

            window.view = InspectionView.Browser;
            window.Show();
        }

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Migration Graph",
            priority = 311)]
        private static void OpenMigrationGraph()
        {
            EchoSaveBrowserWindow window =
                GetWindow<EchoSaveBrowserWindow>(
                    "Chronicle Save Browser");

            window.view = InspectionView.MigrationGraph;
            window.Show();
        }

        private void OnDisable()
        {
            service.Dispose();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "The Chronicle — Read-Only Inspection",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "M5-03 inspection is read-only. Refresh, selection, generation inspection, and migration-graph viewing do not repair, recover, delete, restore, change heads, or run migrations.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Save Browser"))
            {
                view = InspectionView.Browser;
            }

            if (GUILayout.Button(
                    "Migration Graph"))
            {
                view = InspectionView.MigrationGraph;
            }

            if (GUILayout.Button(
                    "Refresh"))
            {
                Refresh();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (refreshResult == null)
            {
                EditorGUILayout.HelpBox(
                    "Select an EchoSaveConfiguration and click Refresh.",
                    MessageType.None);
                return;
            }

            DrawRefreshStatus();

            scroll =
                EditorGUILayout.BeginScrollView(
                    scroll);

            if (view == InspectionView.MigrationGraph)
            {
                DrawMigrationGraph();
            }
            else
            {
                DrawBrowser();
            }

            EditorGUILayout.EndScrollView();
        }

        private void Refresh()
        {
            refreshResult =
                service.Refresh(
                    configuration);

            generationSnapshot = null;
            selectedSlotId = string.Empty;
            Repaint();
        }

        private void DrawRefreshStatus()
        {
            EchoSaveInspectionOpenResult open =
                refreshResult.OpenResult;

            if (open == null ||
                !open.Succeeded)
            {
                EditorGUILayout.HelpBox(
                    open == null
                        ? "Chronicle inspection did not return an open result."
                        : FormatDiagnostic(
                            open.DiagnosticCode,
                            open.Message),
                    MessageType.Error);
                return;
            }

            EditorGUILayout.HelpBox(
                open.RootPresent
                    ? "Production save root found. Inspection is open read-only."
                    : "Production save root is absent. The Browser is empty and no directory was created.",
                MessageType.Info);

            if (refreshResult.CatalogResult != null)
            {
                EditorGUILayout.LabelField(
                    "Catalog",
                    refreshResult.CatalogResult.Status.ToString());

                if (!string.IsNullOrEmpty(
                        refreshResult.CatalogResult.DiagnosticCode))
                {
                    EditorGUILayout.HelpBox(
                        FormatDiagnostic(
                            refreshResult.CatalogResult.DiagnosticCode,
                            refreshResult.CatalogResult.Message),
                        refreshResult.CatalogResult.Succeeded
                            ? MessageType.Warning
                            : MessageType.Error);
                }
            }
        }

        private void DrawBrowser()
        {
            if (refreshResult.CatalogResult == null ||
                !refreshResult.CatalogResult.Succeeded)
            {
                EditorGUILayout.HelpBox(
                    "The Chronicle catalog is not available for inspection.",
                    MessageType.Warning);
                return;
            }

            SaveSlotCatalogSnapshot catalog =
                refreshResult.CatalogResult.Snapshot;

            EditorGUILayout.LabelField(
                $"Slots ({catalog.Count})",
                EditorStyles.boldLabel);

            if (catalog.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No Chronicle technical slots were discovered.",
                    MessageType.None);
                return;
            }

            for (int i = 0;
                 i < catalog.Entries.Count;
                 i++)
            {
                SaveSlotCatalogEntry entry =
                    catalog.Entries[i];

                if (entry == null)
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                string label =
                    string.IsNullOrEmpty(
                        entry.DisplayName)
                        ? entry.SlotId.Value
                        : entry.DisplayName +
                          "  [" +
                          entry.SlotId.Value +
                          "]";

                if (GUILayout.Button(
                        label,
                        EditorStyles.miniButton))
                {
                    selectedSlotId =
                        entry.SlotId.Value;

                    generationSnapshot =
                        service.InspectSlot(
                            entry.SlotId);
                }

                EditorGUILayout.LabelField(
                    "Health",
                    entry.Health.ToString());

                EditorGUILayout.LabelField(
                    "Current Generation",
                    entry.CurrentGenerationId.Value ?? string.Empty);

                EditorGUILayout.LabelField(
                    "Updated",
                    entry.UpdatedUtc);

                EditorGUILayout.LabelField(
                    "Participants / Payload",
                    entry.ParticipantCount +
                    " / " +
                    entry.PayloadByteLength +
                    " bytes");

                if (!string.IsNullOrEmpty(
                        entry.DiagnosticCode))
                {
                    EditorGUILayout.HelpBox(
                        FormatDiagnostic(
                            entry.DiagnosticCode,
                            entry.Message),
                        MessageType.Warning);
                }

                EditorGUILayout.EndVertical();
            }

            if (generationSnapshot != null)
            {
                DrawGenerationInspector();
            }
        }

        private void DrawGenerationInspector()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Generation Inspector",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "Selected Slot",
                selectedSlotId);

            EditorGUILayout.LabelField(
                "Status",
                generationSnapshot.Status.ToString());

            if (!string.IsNullOrEmpty(
                    generationSnapshot.DiagnosticCode))
            {
                EditorGUILayout.HelpBox(
                    FormatDiagnostic(
                        generationSnapshot.DiagnosticCode,
                        generationSnapshot.Message),
                    generationSnapshot.Succeeded
                        ? MessageType.Warning
                        : MessageType.Error);
            }

            if (generationSnapshot.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    generationSnapshot.Message,
                    MessageType.None);
                return;
            }

            for (int i = 0;
                 i < generationSnapshot.Entries.Count;
                 i++)
            {
                SaveGenerationInspectionEntry entry =
                    generationSnapshot.Entries[i];

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                EditorGUILayout.LabelField(
                    entry.IsCurrentHead
                        ? "CURRENT  " + entry.GenerationId
                        : entry.GenerationId,
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    "Status",
                    entry.Status.ToString());

                EditorGUILayout.LabelField(
                    "Manifest Version",
                    entry.SourceManifestVersion +
                    " → " +
                    entry.CurrentManifestVersion);

                EditorGUILayout.LabelField(
                    "Migrated In Memory",
                    entry.WasMigratedInMemory
                        ? "Yes"
                        : "No");

                EditorGUILayout.LabelField(
                    "Commit State",
                    entry.CommitState);

                EditorGUILayout.LabelField(
                    "Updated",
                    entry.UpdatedUtc);

                EditorGUILayout.LabelField(
                    "Participants / Payload",
                    entry.ParticipantCount +
                    " / " +
                    entry.PayloadByteLength +
                    " bytes");

                if (!string.IsNullOrEmpty(
                        entry.DiagnosticCode))
                {
                    EditorGUILayout.HelpBox(
                        FormatDiagnostic(
                            entry.DiagnosticCode,
                            entry.Message),
                        MessageType.Warning);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMigrationGraph()
        {
            SaveMigrationGraphSnapshot graph =
                refreshResult.MigrationGraph;

            EditorGUILayout.LabelField(
                "Package-Document Migration Graph",
                EditorStyles.boldLabel);

            if (graph == null)
            {
                EditorGUILayout.HelpBox(
                    "The Chronicle migration graph is unavailable.",
                    MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField(
                "Registry",
                graph.RegistryValid
                    ? "Valid"
                    : "Invalid");

            EditorGUILayout.LabelField(
                "Registered Edges",
                graph.EdgeCount.ToString());

            if (!string.IsNullOrEmpty(
                    graph.Message))
            {
                EditorGUILayout.HelpBox(
                    FormatDiagnostic(
                        graph.DiagnosticCode,
                        graph.Message),
                    graph.RegistryValid
                        ? MessageType.Info
                        : MessageType.Error);
            }

            for (int i = 0;
                 i < graph.Documents.Count;
                 i++)
            {
                SaveMigrationGraphDocument document =
                    graph.Documents[i];

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                EditorGUILayout.LabelField(
                    document.DocumentKind,
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    "Current Version",
                    document.CurrentVersion);

                EditorGUILayout.LabelField(
                    "Registered Edges",
                    document.RegisteredEdgeCount.ToString());

                EditorGUILayout.EndVertical();
            }

            if (graph.EdgeCount == 0)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Registered Migration Edges",
                EditorStyles.boldLabel);

            for (int i = 0;
                 i < graph.Edges.Count;
                 i++)
            {
                SaveMigrationGraphEdge edge =
                    graph.Edges[i];

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                EditorGUILayout.LabelField(
                    edge.StepId,
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    "Document",
                    edge.DocumentKind);

                EditorGUILayout.LabelField(
                    "Edge",
                    edge.SourceVersion +
                    " → " +
                    edge.TargetVersion);

                EditorGUILayout.LabelField(
                    "Reaches Current",
                    edge.ReachesCurrent
                        ? "Yes"
                        : "No");

                EditorGUILayout.LabelField(
                    "Path Steps",
                    edge.PathStepCount.ToString());

                if (!string.IsNullOrEmpty(
                        edge.DiagnosticCode))
                {
                    EditorGUILayout.HelpBox(
                        FormatDiagnostic(
                            edge.DiagnosticCode,
                            edge.Message),
                        edge.ReachesCurrent
                            ? MessageType.Info
                            : MessageType.Warning);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private static string FormatDiagnostic(
            string diagnosticCode,
            string message)
        {
            if (string.IsNullOrEmpty(
                    diagnosticCode))
            {
                return message ?? string.Empty;
            }

            return
                "[" +
                diagnosticCode +
                "] " +
                (message ?? string.Empty);
        }
    }
}
