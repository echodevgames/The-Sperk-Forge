using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveValidatorWindow : EditorWindow
    {
        private readonly EchoSaveValidationService service =
            new EchoSaveValidationService();

        private EchoSaveConfiguration configuration;
        private EchoSaveValidationReport report;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Validator")]
        public static void Open()
        {
            GetWindow<EchoSaveValidatorWindow>(
                "Chronicle Validator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle Validator",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Validation is read-only. M5-02 adds schema-3 retention, provider, discovery-limit, and fixed-slot-template checks while retaining the M5-01 root, storage, assembly, and slot-policy rules.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)
                EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            if (GUILayout.Button("Run Validation"))
            {
                report =
                    service.Validate(
                        configuration);
            }

            DrawReport();
        }

        private void DrawReport()
        {
            if (report == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                $"Issues: {report.Issues.Count}",
                EditorStyles.boldLabel);

            if (report.Issues.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No M5-02 Chronicle validation issues were found.",
                    MessageType.Info);
                return;
            }

            for (int i = 0;
                 i < report.Issues.Count;
                 i++)
            {
                EchoSaveValidationIssue issue =
                    report.Issues[i];

                string context =
                    string.IsNullOrEmpty(
                        issue.Context)
                        ? string.Empty
                        : $"\nContext: {issue.Context}";

                EditorGUILayout.HelpBox(
                    $"[{issue.CheckId}] {issue.Message}{context}",
                    ToMessageType(
                        issue.Severity));
            }
        }

        private static MessageType ToMessageType(
            EchoSaveValidationSeverity severity)
        {
            switch (severity)
            {
                case EchoSaveValidationSeverity.Error:
                    return MessageType.Error;
                case EchoSaveValidationSeverity.Warning:
                    return MessageType.Warning;
                default:
                    return MessageType.Info;
            }
        }
    }
}
