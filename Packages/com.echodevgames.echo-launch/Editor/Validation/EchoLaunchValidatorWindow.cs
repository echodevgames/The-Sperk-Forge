using EchoDevGames.EchoLaunch.Editor.Setup;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidatorWindow : EditorWindow
    {
        private const string MenuPath =
            "Tools/Sperk's Forge/First Light/Validator";

        [SerializeField]
        private string projectRootPath =
            EchoLaunchSetupPathSet.DefaultProjectRootPath;

        [SerializeField]
        private bool includeInformation = true;

        private Vector2 scrollPosition;
        private EchoLaunchValidationReport lastReport;
        private EchoLaunchValidationService validationService;

        [MenuItem(MenuPath, priority = 201)]
        private static void OpenWindow()
        {
            EchoLaunchValidatorWindow window =
                GetWindow<EchoLaunchValidatorWindow>();

            window.titleContent =
                new GUIContent("First Light Validator");

            window.minSize = new Vector2(620f, 420f);
            window.Show();
        }

        internal EchoLaunchValidationReport LastReport =>
            lastReport;

        internal string ProjectRootPath
        {
            get => projectRootPath;
            set => projectRootPath = value;
        }

        internal bool IncludeInformation
        {
            get => includeInformation;
            set => includeInformation = value;
        }

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(projectRootPath))
            {
                projectRootPath =
                    EchoLaunchSetupPathSet.DefaultProjectRootPath;
            }

            validationService =
                new EchoLaunchValidationService();

            titleContent =
                new GUIContent("First Light Validator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "First Light — Project Validator",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Validation is explicit and read-only. It never applies, repairs, migrates, saves, deletes, or changes Build Settings.",
                MessageType.Info);

            projectRootPath =
                EditorGUILayout.TextField(
                    "Project Root",
                    projectRootPath);

            includeInformation =
                EditorGUILayout.Toggle(
                    "Include Information",
                    includeInformation);

            EditorGUI.BeginDisabledGroup(
                EchoLaunchValidationService.IsValidationActive);

            if (GUILayout.Button(
                    "Validate Project",
                    GUILayout.Height(30f)))
            {
                RunValidation();
            }

            EditorGUI.EndDisabledGroup();

            if (lastReport == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(
                    "No validation has been run in this window.",
                    MessageType.None);

                return;
            }

            DrawSummary();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Report"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        EchoLaunchValidationTextFormatter.Format(
                            lastReport);
                }

                if (GUILayout.Button("Clear Result"))
                {
                    lastReport = null;
                    GUIUtility.ExitGUI();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Findings",
                EditorStyles.boldLabel);

            scrollPosition =
                EditorGUILayout.BeginScrollView(
                    scrollPosition);

            if (lastReport.FindingCount == 0)
            {
                EditorGUILayout.HelpBox(
                    "No validation findings.",
                    MessageType.Info);
            }
            else
            {
                for (int index = 0;
                     index < lastReport.Findings.Count;
                     index++)
                {
                    DrawFinding(
                        lastReport.Findings[index]);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        internal void RunValidation()
        {
            if (validationService == null)
            {
                validationService =
                    new EchoLaunchValidationService();
            }

            lastReport =
                validationService.Validate(
                    new EchoLaunchValidationRequest(
                        projectRootPath,
                        includeInformation));

            Repaint();
        }

        private void DrawSummary()
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Health: " + lastReport.Health +
                "\nInformation: " + lastReport.InformationCount +
                "   Warnings: " + lastReport.WarningCount +
                "   Errors: " + lastReport.ErrorCount +
                "   Blockers: " + lastReport.BlockerCount +
                "\nReport fingerprint: " +
                lastReport.ReportFingerprint,
                ToMessageType(lastReport.Health));
        }

        private static void DrawFinding(
            EchoLaunchValidationFinding finding)
        {
            string text =
                finding.Code +
                " — " +
                finding.Title;

            if (!string.IsNullOrEmpty(
                    finding.ProjectPath))
            {
                text +=
                    "\nPath: " +
                    finding.ProjectPath;
            }

            if (!string.IsNullOrEmpty(finding.Message))
            {
                text +=
                    "\n" +
                    finding.Message;
            }

            if (!string.IsNullOrEmpty(finding.Evidence))
            {
                text +=
                    "\nEvidence: " +
                    finding.Evidence;
            }

            if (!string.IsNullOrEmpty(
                    finding.SuggestedAction))
            {
                text +=
                    "\nAction: " +
                    finding.SuggestedAction;
            }

            EditorGUILayout.HelpBox(
                text,
                ToMessageType(finding.Severity));
        }

        private static MessageType ToMessageType(
            EchoLaunchProjectHealth health)
        {
            switch (health)
            {
                case EchoLaunchProjectHealth.Healthy:
                    return MessageType.Info;

                case EchoLaunchProjectHealth.NeedsAttention:
                    return MessageType.Warning;

                case EchoLaunchProjectHealth.Invalid:
                case EchoLaunchProjectHealth.Blocked:
                    return MessageType.Error;

                default:
                    return MessageType.None;
            }
        }

        private static MessageType ToMessageType(
            EchoLaunchValidationSeverity severity)
        {
            switch (severity)
            {
                case EchoLaunchValidationSeverity.Information:
                    return MessageType.Info;

                case EchoLaunchValidationSeverity.Warning:
                    return MessageType.Warning;

                case EchoLaunchValidationSeverity.Error:
                case EchoLaunchValidationSeverity.Blocker:
                    return MessageType.Error;

                default:
                    return MessageType.None;
            }
        }
    }
}
