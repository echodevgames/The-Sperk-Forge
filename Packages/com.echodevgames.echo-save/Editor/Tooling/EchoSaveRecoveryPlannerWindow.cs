using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveRecoveryPlannerWindow :
        EditorWindow
    {
        private readonly EchoSaveInspectionService inspection =
            new EchoSaveInspectionService();

        private EchoSaveConfiguration configuration;
        private string slotIdText = string.Empty;
        private SaveRecoveryPlan plan;
        private string status = string.Empty;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Recovery Planner",
            priority = 321)]
        private static void Open()
        {
            GetWindow<EchoSaveRecoveryPlannerWindow>(
                "Chronicle Recovery Planner");
        }

        private void OnDisable()
        {
            inspection.Dispose();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle — Recovery Planner",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Preview-only. This window never executes recovery and never rewrites a head.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            slotIdText =
                EditorGUILayout.TextField(
                    "Technical Slot ID",
                    slotIdText);

            if (GUILayout.Button(
                    "Preview Recovery Plan"))
            {
                Preview();
            }

            if (plan == null)
            {
                if (!string.IsNullOrEmpty(
                        status))
                {
                    EditorGUILayout.HelpBox(
                        status,
                        MessageType.None);
                }

                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Plan",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "Status",
                plan.Status.ToString());

            EditorGUILayout.LabelField(
                "Head Condition",
                plan.HeadCondition.ToString());

            EditorGUILayout.LabelField(
                "Observed Diagnostic",
                plan.ObservedDiagnosticCode);

            EditorGUILayout.LabelField(
                "Verified Candidates",
                plan.VerifiedCandidateCount.ToString());

            EditorGUILayout.LabelField(
                "Rejected Canonical",
                plan.RejectedCanonicalCount.ToString());

            EditorGUILayout.LabelField(
                "Ignored Non-Canonical",
                plan.IgnoredNonCanonicalCount.ToString());

            EditorGUILayout.LabelField(
                "Recovery Required",
                plan.RecoveryRequired
                    ? "Yes"
                    : "No");

            EditorGUILayout.LabelField(
                "Preferred Candidate",
                plan.HasPreferredCandidate
                    ? plan.PreferredCandidate.GenerationId.Value
                    : "(none)");

            EditorGUILayout.HelpBox(
                plan.Message,
                plan.Succeeded
                    ? MessageType.Info
                    : MessageType.Warning);

            if (plan.Candidates.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(
                    "Verified Candidate Order",
                    EditorStyles.boldLabel);

                for (int i = 0;
                     i < plan.Candidates.Count;
                     i++)
                {
                    SaveRecoveryCandidate candidate =
                        plan.Candidates[i];

                    EditorGUILayout.LabelField(
                        $"{i + 1}. {candidate.GenerationId.Value}");

                    EditorGUILayout.LabelField(
                        "    Updated",
                        candidate.TechnicalTimestampUtc);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "No Apply / Recover control exists in ESV-M5-04.",
                MessageType.None);
        }

        private void Preview()
        {
            plan = null;
            status = string.Empty;

            EchoSaveBrowserRefreshResult refresh =
                inspection.Refresh(
                    configuration);

            if (refresh == null ||
                !refresh.Succeeded)
            {
                status =
                    refresh?.OpenResult?.Message ??
                    "Chronicle read-only inspection could not open.";
                return;
            }

            if (!SaveSlotId.TryParse(
                    slotIdText,
                    out SaveSlotId slotId))
            {
                status =
                    "Enter one valid Chronicle technical slot ID.";
                return;
            }

            plan =
                inspection.BuildRecoveryPlan(
                    slotId);

            if (plan == null)
            {
                status =
                    "Chronicle recovery planning is unavailable.";
            }
        }
    }
}
