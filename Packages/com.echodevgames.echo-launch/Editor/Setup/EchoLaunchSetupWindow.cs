using System;
using System.Collections.Generic;
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
            "Apply is create-only. Repair is separate, proof-gated, backed up, and limited to the displayed before/after changes.";

        private string projectRootPath;
        private string bootScenePath;
        private SceneAsset destinationScene;
        private bool createSplashSequence;

        [SerializeField]
        private EchoLaunchSetupFoundationResolutionPolicy
            foundationResolutionPolicy =
                EchoLaunchSetupFoundationResolutionPolicy
                    .ReuseCompatibleAssets;

        [SerializeField]
        private SplashPresentationMode splashPresentationMode =
            SplashPresentationMode.SplashOnly;

        [SerializeField]
        private Color splashBackgroundColor =
            Color.black;

        [SerializeField]
        private bool allowSplashAdvancement =
            true;

        [SerializeField]
        private bool showSplashAuthoring =
            true;

        [SerializeField]
        private List<SplashEntryDraft> splashEntries =
            new List<SplashEntryDraft>();

        private EchoLaunchBuildSettingsPolicy buildSettingsPolicy;
        private bool approvePlaceFirst;
        private EchoLaunchSetupPlan currentPlan;
        private EchoLaunchSetupApplyResult currentApplyResult;
        private EchoLaunchSetupRepairResult currentRepairResult;
        private string currentPlanReport = string.Empty;
        private string currentApplyReport = string.Empty;
        private string currentRepairReport = string.Empty;
        private Vector2 scrollPosition;
        private EchoLaunchSetupApplyService applyService;
        private EchoLaunchSetupRepairService repairService;

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

            if (splashEntries == null)
            {
                splashEntries =
                    new List<SplashEntryDraft>();
            }

            applyService = new EchoLaunchSetupApplyService();
            repairService = new EchoLaunchSetupRepairService();
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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Foundation",
                EditorStyles.boldLabel);

            foundationResolutionPolicy =
                (EchoLaunchSetupFoundationResolutionPolicy)
                EditorGUILayout.EnumPopup(
                    "Asset Resolution",
                    foundationResolutionPolicy);

            if (foundationResolutionPolicy ==
                EchoLaunchSetupFoundationResolutionPolicy
                    .CreateProjectOwnedSetup)
            {
                EditorGUILayout.HelpBox(
                    "Missing First Light foundation assets will be created " +
                    "under the requested Project Root instead of reusing " +
                    "compatible assets elsewhere in the project. Existing " +
                    "compatible target assets and the selected destination " +
                    "scene remain untouched.",
                    MessageType.Info);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Splash Sequence",
                EditorStyles.boldLabel);

            createSplashSequence = EditorGUILayout.Toggle(
                "Create Splash Sequence",
                createSplashSequence);

            if (createSplashSequence)
            {
                DrawSplashAuthoring();
            }

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

                EchoLaunchSetupApplyEligibility applyEligibility =
                    EchoLaunchSetupApplyService.EvaluateEligibility(
                        currentPlan,
                        approvePlaceFirst);

                EchoLaunchSetupRepairEligibility repairEligibility =
                    EchoLaunchSetupRepairService.EvaluateEligibility(
                        currentPlan,
                        approvePlaceFirst);

                bool editorBusy =
                    EditorApplication.isCompiling ||
                    EditorApplication.isUpdating ||
                    EditorApplication.isPlayingOrWillChangePlaymode;

                using (new EditorGUI.DisabledScope(
                           !applyEligibility.CanApply ||
                           editorBusy ||
                           EchoLaunchSetupApplyService.IsMutationActive))
                {
                    if (GUILayout.Button(
                            "Apply Plan...",
                            GUILayout.Height(28f)))
                    {
                        ApplyCurrentPlan();
                    }
                }

                using (new EditorGUI.DisabledScope(
                           !repairEligibility.CanRepair ||
                           editorBusy ||
                           EchoLaunchSetupApplyService.IsMutationActive))
                {
                    if (GUILayout.Button(
                            "Repair Plan...",
                            GUILayout.Height(28f)))
                    {
                        RepairCurrentPlan();
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

            if (currentRepairResult != null)
            {
                EditorGUILayout.Space();
                DrawRepairResult();
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

        internal EchoLaunchSetupRepairResult RepairPlanForTests(
            EchoLaunchSetupPlan plan,
            bool confirmed,
            bool placeFirstApproved,
            EchoLaunchSetupRepairService service = null)
        {
            currentPlan = plan;
            currentRepairResult =
                (service ?? repairService ?? new EchoLaunchSetupRepairService())
                    .Repair(
                        new EchoLaunchSetupRepairRequest(
                            plan,
                            confirmed,
                            placeFirstApproved));
            currentRepairReport =
                new EchoLaunchSetupRepairResultFormatter().Format(
                    currentRepairResult);
            Repaint();
            return currentRepairResult;
        }

        internal bool CanRepairCurrentPlanForTests(
            bool placeFirstApproved)
        {
            return EchoLaunchSetupRepairService.EvaluateEligibility(
                currentPlan,
                placeFirstApproved).CanRepair;
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
        internal string CurrentRepairReportForTests => currentRepairReport;
        internal EchoLaunchSetupApplyResult CurrentApplyResultForTests =>
            currentApplyResult;
        internal EchoLaunchSetupRepairResult CurrentRepairResultForTests =>
            currentRepairResult;

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
                    buildSettingsPolicy,
                    splashAuthoring:
                        createSplashSequence
                            ? BuildSplashAuthoringRequest()
                            : null,
                    foundationResolutionPolicy:
                        foundationResolutionPolicy);

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

        private void RepairCurrentPlan()
        {
            string confirmation = BuildRepairConfirmationText(
                currentPlan,
                approvePlaceFirst);
            bool confirmed = EditorUtility.DisplayDialog(
                "Repair First Light Setup",
                confirmation,
                "Repair",
                "Cancel");
            currentRepairResult = repairService.Repair(
                new EchoLaunchSetupRepairRequest(
                    currentPlan,
                    confirmed,
                    approvePlaceFirst));
            currentRepairReport =
                new EchoLaunchSetupRepairResultFormatter().Format(
                    currentRepairResult);
            RefreshPlan();
        }

        private static string BuildRepairConfirmationText(
            EchoLaunchSetupPlan plan,
            bool placeFirstApproved)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "First Light will back up and perform only the following proven repairs:");
            bool found = false;
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];
                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    found = true;
                    builder.AppendLine("- " + operation.TargetPath);
                    builder.AppendLine("  Before: " + operation.ExistingState);
                    builder.AppendLine("  After: " + operation.ProposedState);
                    builder.AppendLine("  Proof: " + operation.ProofSummary);
                }
                else if (operation.Disposition ==
                         EchoLaunchSetupOperationDisposition.Create)
                {
                    builder.AppendLine("- Create missing: " + operation.TargetPath);
                }
            }
            if (!found && !plan.HasCreates)
            {
                builder.AppendLine("- No changes. This run should settle as NoChanges.");
            }
            builder.AppendLine();
            builder.AppendLine(
                "Existing asset and .meta bytes are preserved under Library/EchoDevGames/FirstLight/RepairBackups before mutation.");
            builder.AppendLine(
                "Place-first approval: " +
                (placeFirstApproved ? "Approved" : "Not approved"));
            builder.AppendLine(
                "No schema migration, ID regeneration, type replacement, deletion, move, rename, or arbitrary scene cleanup is authorized.");
            return builder.ToString();
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

            if (plan.Request.SplashAuthoring != null)
            {
                builder.AppendLine();
                builder.AppendLine(
                    "Splash creation authoring: " +
                    plan.Request.SplashAuthoring.Summary);
                builder.AppendLine(
                    "These values apply only if Setup creates a new SplashSequence; a reused sequence is never re-authored.");
            }

            builder.AppendLine();
            builder.AppendLine(
                "Existing project assets will not be overwritten, repaired, moved, renamed, or deleted.");

            return builder.ToString();
        }

        private void DrawSplashAuthoring()
        {
            EditorGUILayout.Space();

            showSplashAuthoring =
                EditorGUILayout.Foldout(
                    showSplashAuthoring,
                    "Splash Authoring",
                    true);

            if (!showSplashAuthoring)
            {
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField(
                "Presentation",
                EditorStyles.boldLabel);

            int modeIndex =
                splashPresentationMode ==
                    SplashPresentationMode.SplashOnly
                    ? 1
                    : 0;

            modeIndex =
                EditorGUILayout.Popup(
                    "Mode",
                    modeIndex,
                    new[]
                    {
                        "Splash + Status",
                        "Splash Only",
                    });

            splashPresentationMode =
                modeIndex == 1
                    ? SplashPresentationMode.SplashOnly
                    : SplashPresentationMode.SplashAndStatus;

            splashBackgroundColor =
                EditorGUILayout.ColorField(
                    "Background",
                    splashBackgroundColor);

            allowSplashAdvancement =
                EditorGUILayout.Toggle(
                    "Allow Advancement",
                    allowSplashAdvancement);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Splashes",
                EditorStyles.boldLabel);

            for (int index = 0;
                 index < splashEntries.Count;
                 index++)
            {
                if (DrawSplashEntry(
                        splashEntries[index],
                        index))
                {
                    splashEntries.RemoveAt(index);
                    break;
                }
            }

            if (splashEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No splash entries are authored yet. Setup may still create an empty sequence, or add one or more project-owned images below.",
                    MessageType.Info);
            }

            if (GUILayout.Button("Add Splash"))
            {
                splashEntries.Add(
                    new SplashEntryDraft());
            }

            EchoLaunchSetupSplashAuthoringRequest preview =
                BuildSplashAuthoringRequest();

            if (!preview.TryValidate(
                    out string message))
            {
                EditorGUILayout.HelpBox(
                    message,
                    MessageType.Error);
            }

            EditorGUI.indentLevel--;
        }

        private bool DrawSplashEntry(
            SplashEntryDraft entry,
            int index)
        {
            EditorGUILayout.BeginVertical(
                EditorStyles.helpBox);

            string title =
                string.IsNullOrWhiteSpace(
                    entry.DisplayLabel)
                    ? $"Splash {index + 1}"
                    : entry.DisplayLabel.Trim();

            entry.Expanded =
                EditorGUILayout.Foldout(
                    entry.Expanded,
                    title,
                    true);

            bool remove = false;

            if (entry.Expanded)
            {
                EditorGUI.indentLevel++;

                entry.Image =
                    (Sprite)EditorGUILayout.ObjectField(
                        "Image",
                        entry.Image,
                        typeof(Sprite),
                        false);

                entry.AudioIntent =
                    (AudioClip)EditorGUILayout.ObjectField(
                        "Audio Intent",
                        entry.AudioIntent,
                        typeof(AudioClip),
                        false);

                entry.DisplayLabel =
                    EditorGUILayout.TextField(
                        "Display Label",
                        entry.DisplayLabel);

                entry.FadeInSeconds =
                    Mathf.Max(
                        0f,
                        EditorGUILayout.FloatField(
                            "Fade In",
                            entry.FadeInSeconds));

                entry.HoldSeconds =
                    Mathf.Max(
                        0f,
                        EditorGUILayout.FloatField(
                            "Hold",
                            entry.HoldSeconds));

                entry.FadeOutSeconds =
                    Mathf.Max(
                        0f,
                        EditorGUILayout.FloatField(
                            "Fade Out",
                            entry.FadeOutSeconds));

                entry.MinimumDisplaySeconds =
                    Mathf.Max(
                        0f,
                        EditorGUILayout.FloatField(
                            "Minimum",
                            entry.MinimumDisplaySeconds));

                entry.MotionStyle =
                    (SplashMotionStyle)
                    EditorGUILayout.EnumPopup(
                        "Motion",
                        entry.MotionStyle);

                if (entry.MotionStyle ==
                    SplashMotionStyle.Pulse)
                {
                    entry.PulseMaximumScale =
                        Mathf.Max(
                            1f,
                            EditorGUILayout.FloatField(
                                "Maximum Scale",
                                entry.PulseMaximumScale));

                    entry.PulseCycleSeconds =
                        Mathf.Max(
                            0.01f,
                            EditorGUILayout.FloatField(
                                "Cycle Seconds",
                                entry.PulseCycleSeconds));
                }

                int advanceIndex =
                    GetAdvanceIndex(
                        entry.AdvancePolicy);

                advanceIndex =
                    EditorGUILayout.Popup(
                        "Advance",
                        advanceIndex,
                        new[]
                        {
                            "Automatic",
                            "Skippable After Minimum",
                            "Wait For Input After Minimum",
                        });

                entry.AdvancePolicy =
                    GetAdvancePolicy(
                        advanceIndex);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(
                    index <= 0);

                if (GUILayout.Button("Move Up"))
                {
                    SplashEntryDraft moving =
                        splashEntries[index];

                    splashEntries.RemoveAt(index);
                    splashEntries.Insert(
                        index - 1,
                        moving);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(
                    index >=
                    splashEntries.Count - 1);

                if (GUILayout.Button("Move Down"))
                {
                    SplashEntryDraft moving =
                        splashEntries[index];

                    splashEntries.RemoveAt(index);
                    splashEntries.Insert(
                        index + 1,
                        moving);
                }

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Remove"))
                {
                    remove = true;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            return remove;
        }

        private EchoLaunchSetupSplashAuthoringRequest
            BuildSplashAuthoringRequest()
        {
            List<EchoLaunchSetupSplashEntryRequest>
                entries =
                    new List<
                        EchoLaunchSetupSplashEntryRequest>();

            for (int index = 0;
                 index < splashEntries.Count;
                 index++)
            {
                SplashEntryDraft entry =
                    splashEntries[index];

                entries.Add(
                    new EchoLaunchSetupSplashEntryRequest(
                        GetAssetPath(entry.Image),
                        GetAssetPath(entry.AudioIntent),
                        entry.DisplayLabel,
                        entry.FadeInSeconds,
                        entry.HoldSeconds,
                        entry.FadeOutSeconds,
                        entry.MinimumDisplaySeconds,
                        entry.MotionStyle,
                        entry.PulseMaximumScale,
                        entry.PulseCycleSeconds,
                        entry.AdvancePolicy));
            }

            return new
                EchoLaunchSetupSplashAuthoringRequest(
                    splashPresentationMode,
                    splashBackgroundColor,
                    allowSplashAdvancement,
                    entries);
        }

        private static string GetAssetPath(
            UnityEngine.Object asset)
        {
            return asset == null
                ? string.Empty
                : AssetDatabase.GetAssetPath(asset);
        }

        private static int GetAdvanceIndex(
            SplashSkipPolicy policy)
        {
            switch (policy)
            {
                case SplashSkipPolicy.Disallowed:
                    return 0;
                case SplashSkipPolicy.AfterMinimumDisplay:
                    return 1;
                case SplashSkipPolicy.WaitForInputAfterMinimum:
                    return 2;
                default:
                    return 0;
            }
        }

        private static SplashSkipPolicy GetAdvancePolicy(
            int index)
        {
            switch (index)
            {
                case 1:
                    return SplashSkipPolicy.AfterMinimumDisplay;
                case 2:
                    return SplashSkipPolicy.WaitForInputAfterMinimum;
                default:
                    return SplashSkipPolicy.Disallowed;
            }
        }

        [Serializable]
        private sealed class SplashEntryDraft
        {
            [SerializeField]
            internal bool Expanded = true;

            [SerializeField]
            internal Sprite Image;

            [SerializeField]
            internal AudioClip AudioIntent;

            [SerializeField]
            internal string DisplayLabel = string.Empty;

            [SerializeField]
            internal float FadeInSeconds = 0.25f;

            [SerializeField]
            internal float HoldSeconds = 1f;

            [SerializeField]
            internal float FadeOutSeconds = 0.25f;

            [SerializeField]
            internal float MinimumDisplaySeconds;

            [SerializeField]
            internal SplashMotionStyle MotionStyle =
                SplashMotionStyle.None;

            [SerializeField]
            internal float PulseMaximumScale = 1.05f;

            [SerializeField]
            internal float PulseCycleSeconds = 1f;

            [SerializeField]
            internal SplashSkipPolicy AdvancePolicy =
                SplashSkipPolicy.AfterMinimumDisplay;
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

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    EditorGUILayout.LabelField(
                        "Before: " + operation.ExistingState,
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "After: " + operation.ProposedState,
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "Proof: " + operation.ProofSummary,
                        EditorStyles.wordWrappedLabel);
                }

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

        private void DrawRepairResult()
        {
            MessageType messageType =
                currentRepairResult.Status ==
                    EchoLaunchSetupRepairStatus.Succeeded ||
                currentRepairResult.Status ==
                    EchoLaunchSetupRepairStatus.NoChanges
                    ? MessageType.Info
                    : currentRepairResult.Status ==
                      EchoLaunchSetupRepairStatus.Cancelled
                        ? MessageType.Warning
                        : MessageType.Error;

            EditorGUILayout.HelpBox(
                "Repair status: " + currentRepairResult.Status +
                "\n" + currentRepairResult.Message +
                (string.IsNullOrEmpty(currentRepairResult.BackupDirectory)
                    ? string.Empty
                    : "\nBackup: " + currentRepairResult.BackupDirectory),
                messageType);

            if (GUILayout.Button(
                    "Copy Repair Result",
                    GUILayout.Height(24f)))
            {
                EditorGUIUtility.systemCopyBuffer = currentRepairReport;
            }
        }
    }
}
