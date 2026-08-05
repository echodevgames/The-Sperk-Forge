using System.Text;
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

        internal const string ApplyBoundaryMessage =
            "Create-only setup. Existing project assets are never overwritten.";

        private string projectRootPath;
        private string bootScenePath;
        private SceneAsset destinationScene;
        private bool createSplashSequence;
        private EchoLaunchBuildSettingsPolicy buildSettingsPolicy;
        private bool approvePlaceFirst;
        private EchoLaunchSetupPlan currentPlan;
        private EchoLaunchSetupApplyResult currentApplyResult;
        private string currentPlanReport = string.Empty;
        private string currentApplyReport = string.Empty;
        private Vector2 scrollPosition;
        private EchoLaunchSetupApplyService applyService;

        [MenuItem(MenuPath)]
        private static void OpenFromMenu()
        {
            OpenWindow();
        }

        internal static EchoLaunchSetupWindow OpenWindow()
        {
            EchoLaunchSetupWindow window =
                GetWindow<EchoLaunchSetupWindow>("First Light Setup");

            window.minSize = new Vector2(720f, 560f);
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

            applyService = new EchoLaunchSetupApplyService();
            RefreshPlan();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                ApplyBoundaryMessage,
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

            if (buildSettingsPolicy ==
                EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval)
            {
                approvePlaceFirst = EditorGUILayout.ToggleLeft(
                    "I approve placing Boot first while preserving unrelated scene order.",
                    approvePlaceFirst);
            }
            else
            {
                approvePlaceFirst = false;
            }

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
                        EditorGUIUtility.systemCopyBuffer =
                            currentPlanReport;
                    }
                }

                EchoLaunchSetupApplyEligibility eligibility =
                    EchoLaunchSetupApplyService.EvaluateEligibility(
                        currentPlan,
                        approvePlaceFirst);

                bool editorBusy =
                    EditorApplication.isCompiling ||
                    EditorApplication.isUpdating ||
                    EditorApplication.isPlayingOrWillChangePlaymode;

                using (new EditorGUI.DisabledScope(
                           !eligibility.CanApply ||
                           editorBusy ||
                           EchoLaunchSetupApplyService.IsApplyActive))
                {
                    if (GUILayout.Button(
                            "Apply Plan...",
                            GUILayout.Height(28f)))
                    {
                        ApplyCurrentPlan();
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

            if (currentApplyResult != null)
            {
                EditorGUILayout.Space();
                DrawApplyResult();
            }
        }

        internal EchoLaunchSetupPlan RefreshPlanForTests(
            EchoLaunchSetupRequest request)
        {
            EchoLaunchProjectSnapshot snapshot =
                new EchoLaunchProjectSnapshotCollector().Collect(request);

            currentPlan =
                new EchoLaunchSetupPlanner().CreatePlan(request, snapshot);

            currentPlanReport =
                new EchoLaunchSetupPlanTextFormatter().Format(currentPlan);

            Repaint();
            return currentPlan;
        }

        internal EchoLaunchSetupApplyResult ApplyPlanForTests(
            EchoLaunchSetupPlan plan,
            bool confirmed,
            bool placeFirstApproved,
            EchoLaunchSetupApplyService service = null)
        {
            currentPlan = plan;
            currentApplyResult =
                (service ?? applyService ?? new EchoLaunchSetupApplyService())
                    .Apply(
                        new EchoLaunchSetupApplyRequest(
                            plan,
                            confirmed,
                            placeFirstApproved));

            currentApplyReport =
                new EchoLaunchSetupApplyResultFormatter().Format(
                    currentApplyResult);

            Repaint();
            return currentApplyResult;
        }

        internal bool CanApplyCurrentPlanForTests(
            bool placeFirstApproved)
        {
            return EchoLaunchSetupApplyService.EvaluateEligibility(
                currentPlan,
                placeFirstApproved).CanApply;
        }

        internal string CurrentReportForTests => currentPlanReport;
        internal string CurrentApplyReportForTests => currentApplyReport;
        internal EchoLaunchSetupApplyResult CurrentApplyResultForTests =>
            currentApplyResult;

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

        private void ApplyCurrentPlan()
        {
            string confirmation =
                BuildConfirmationText(
                    currentPlan,
                    approvePlaceFirst);

            bool confirmed =
                EditorUtility.DisplayDialog(
                    "Apply First Light Setup",
                    confirmation,
                    "Apply",
                    "Cancel");

            currentApplyResult =
                applyService.Apply(
                    new EchoLaunchSetupApplyRequest(
                        currentPlan,
                        confirmed,
                        approvePlaceFirst));

            currentApplyReport =
                new EchoLaunchSetupApplyResultFormatter().Format(
                    currentApplyResult);

            RefreshPlan();
        }

        private static string BuildConfirmationText(
            EchoLaunchSetupPlan plan,
            bool placeFirstApproved)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(
                "First Light will create only the following missing project-owned targets:");

            bool foundCreate = false;

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Create)
                {
                    builder.AppendLine("- " + operation.TargetPath);
                    foundCreate = true;
                }
            }

            if (!foundCreate)
            {
                builder.AppendLine("- No asset or folder creation.");
            }

            builder.AppendLine();
            builder.AppendLine(
                "Build Settings policy: " +
                plan.Request.BuildSettingsPolicy);

            builder.AppendLine(
                "Place-first approval: " +
                (placeFirstApproved ? "Approved" : "Not approved"));

            builder.AppendLine();
            builder.AppendLine(
                "Existing project assets will not be overwritten, repaired, moved, renamed, or deleted.");

            return builder.ToString();
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
                "\nDiagnostics: " + currentPlan.Diagnostics.Count +
                "\nFingerprint: " + currentPlan.PlanFingerprint,
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

        private void DrawApplyResult()
        {
            MessageType messageType =
                currentApplyResult.Status ==
                    EchoLaunchSetupApplyStatus.Succeeded ||
                currentApplyResult.Status ==
                    EchoLaunchSetupApplyStatus.NoChanges
                    ? MessageType.Info
                    : currentApplyResult.Status ==
                      EchoLaunchSetupApplyStatus.Cancelled
                        ? MessageType.Warning
                        : MessageType.Error;

            EditorGUILayout.HelpBox(
                "Apply status: " + currentApplyResult.Status +
                "\n" + currentApplyResult.Message,
                messageType);

            if (GUILayout.Button("Copy Result", GUILayout.Height(24f)))
            {
                EditorGUIUtility.systemCopyBuffer =
                    currentApplyReport;
            }
        }
    }
}
