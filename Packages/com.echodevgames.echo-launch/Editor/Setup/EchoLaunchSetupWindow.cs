
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupWindow : EditorWindow
    {
        internal const string MenuPath =
            "Tools/Sperk's Forge/First Light/Setup";

        internal const string PreviewOnlyMessage =
            "Preview only. This checkpoint changes nothing in the project.";

        private string projectRootPath;
        private string bootScenePath;
        private SceneAsset destinationScene;
        private bool createSplashSequence;
        private EchoLaunchBuildSettingsPolicy buildSettingsPolicy;
        private EchoLaunchSetupPlan currentPlan;
        private string currentReport = string.Empty;
        private Vector2 scrollPosition;

        [MenuItem(MenuPath)]
        private static void OpenFromMenu()
        {
            OpenWindow();
        }

        internal static EchoLaunchSetupWindow OpenWindow()
        {
            EchoLaunchSetupWindow window =
                GetWindow<EchoLaunchSetupWindow>("First Light Setup");

            window.minSize = new Vector2(680f, 520f);
            window.Show();
            return window;
        }

        private void OnEnable()
        {
            EchoLaunchSetupPathSet defaults =
                EchoLaunchSetupPathSet.CreateDefault();

            if (string.IsNullOrWhiteSpace(projectRootPath))
            {
                projectRootPath = defaults.ProjectRootPath;
            }

            if (string.IsNullOrWhiteSpace(bootScenePath))
            {
                bootScenePath = defaults.BootScenePath;
            }

            buildSettingsPolicy =
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd;

            RefreshPlan();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                PreviewOnlyMessage,
                MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Project Paths",
                EditorStyles.boldLabel);

            projectRootPath = EditorGUILayout.TextField(
                "Project Root",
                projectRootPath);

            bootScenePath = EditorGUILayout.TextField(
                "Boot Scene",
                bootScenePath);

            destinationScene =
                (SceneAsset)EditorGUILayout.ObjectField(
                    "Destination Scene",
                    destinationScene,
                    typeof(SceneAsset),
                    false);

            createSplashSequence = EditorGUILayout.Toggle(
                "Create Splash Sequence",
                createSplashSequence);

            buildSettingsPolicy =
                (EchoLaunchBuildSettingsPolicy)EditorGUILayout.EnumPopup(
                    "Build Settings Policy",
                    buildSettingsPolicy);

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Plan", GUILayout.Height(28f)))
                {
                    RefreshPlan();
                }

                using (new EditorGUI.DisabledScope(currentPlan == null))
                {
                    if (GUILayout.Button("Copy Plan", GUILayout.Height(28f)))
                    {
                        EditorGUIUtility.systemCopyBuffer = currentReport;
                    }
                }
            }

            EditorGUILayout.Space();

            if (currentPlan == null)
            {
                EditorGUILayout.HelpBox(
                    "No setup plan is available.",
                    MessageType.Warning);
                return;
            }

            DrawPlanSummary();
        }

        internal EchoLaunchSetupPlan RefreshPlanForTests(
            EchoLaunchSetupRequest request)
        {
            EchoLaunchProjectSnapshot snapshot =
                new EchoLaunchProjectSnapshotCollector().Collect(request);

            currentPlan =
                new EchoLaunchSetupPlanner().CreatePlan(request, snapshot);

            currentReport =
                new EchoLaunchSetupPlanTextFormatter().Format(currentPlan);

            Repaint();
            return currentPlan;
        }

        internal string CurrentReportForTests => currentReport;

        private void RefreshPlan()
        {
            string destinationPath =
                destinationScene == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(destinationScene);

            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    projectRootPath,
                    bootScenePath,
                    destinationPath,
                    createSplashSequence,
                    buildSettingsPolicy);

            RefreshPlanForTests(request);
        }

        private void DrawPlanSummary()
        {
            MessageType messageType =
                currentPlan.Status == EchoLaunchSetupPlanStatus.Blocked
                    ? MessageType.Error
                    : currentPlan.Status ==
                      EchoLaunchSetupPlanStatus.ReadyWithWarnings
                        ? MessageType.Warning
                        : MessageType.Info;

            EditorGUILayout.HelpBox(
                "Plan status: " + currentPlan.Status +
                "\nOperations: " + currentPlan.Operations.Count +
                "\nDiagnostics: " + currentPlan.Diagnostics.Count,
                messageType);

            scrollPosition =
                EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField(
                "Operations",
                EditorStyles.boldLabel);

            for (int index = 0; index < currentPlan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation =
                    currentPlan.Operations[index];

                EditorGUILayout.LabelField(
                    operation.Disposition + " — " + operation.Kind,
                    EditorStyles.boldLabel);

                EditorGUILayout.SelectableLabel(
                    operation.TargetPath,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));

                EditorGUILayout.LabelField(
                    operation.Reason,
                    EditorStyles.wordWrappedLabel);

                EditorGUILayout.Space(4f);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Diagnostics",
                EditorStyles.boldLabel);

            if (currentPlan.Diagnostics.Count == 0)
            {
                EditorGUILayout.LabelField("None.");
            }
            else
            {
                for (int index = 0;
                     index < currentPlan.Diagnostics.Count;
                     index++)
                {
                    EchoLaunchSetupDiagnostic diagnostic =
                        currentPlan.Diagnostics[index];

                    EditorGUILayout.HelpBox(
                        diagnostic.Code + ": " + diagnostic.Message,
                        diagnostic.Severity ==
                        EchoLaunchSetupDiagnosticSeverity.Blocker
                            ? MessageType.Error
                            : diagnostic.Severity ==
                              EchoLaunchSetupDiagnosticSeverity.Warning
                                ? MessageType.Warning
                                : MessageType.Info);
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
