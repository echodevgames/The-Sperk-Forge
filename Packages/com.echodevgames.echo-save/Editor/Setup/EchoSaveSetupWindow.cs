using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveSetupWindow : EditorWindow
    {
        private readonly EchoSaveSetupService service =
            new EchoSaveSetupService();

        private string targetAssetPath =
            "Assets/EchoSaveConfiguration.asset";
        private string storageRootDirectoryName =
            "EchoSave";
        private SaveSlotPolicyMode slotPolicyMode =
            SaveSlotPolicyMode.ConfigurableMultiSlot;
        private int fixedSlotCount = 4;
        private int configuredSlotLimit = 64;
        private int profileSafetyLimit = 64;

        private EchoSaveSetupPlan preview;
        private EchoSaveSetupResult lastApplyResult;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Setup")]
        public static void Open()
        {
            GetWindow<EchoSaveSetupWindow>(
                "Chronicle Setup");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle Setup",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "M5-01 Setup is create-only. Preview never writes project assets, and Apply never overwrites an occupied target.",
                MessageType.Info);

            targetAssetPath =
                EditorGUILayout.TextField(
                    "Configuration Asset",
                    targetAssetPath);

            storageRootDirectoryName =
                EditorGUILayout.TextField(
                    "Storage Root",
                    storageRootDirectoryName);

            slotPolicyMode =
                (SaveSlotPolicyMode)
                EditorGUILayout.EnumPopup(
                    "Slot Policy",
                    slotPolicyMode);

            DrawRelevantPolicyField();

            EditorGUILayout.Space();

            if (GUILayout.Button("Preview"))
            {
                preview =
                    service.Preview(
                        BuildRequest());
                lastApplyResult = null;
            }

            DrawPreview();

            bool inputsMatchPreview =
                preview != null &&
                string.Equals(
                    preview.RequestFingerprint,
                    EchoSaveSetupService
                        .ComputeRequestFingerprint(
                            BuildRequest()),
                    System.StringComparison.Ordinal);

            using (new EditorGUI.DisabledScope(
                       preview == null ||
                       !preview.CanApply ||
                       !inputsMatchPreview))
            {
                if (GUILayout.Button("Apply"))
                {
                    lastApplyResult =
                        service.Apply(preview);

                    if (lastApplyResult.Status ==
                        EchoSaveSetupResultStatus.Created &&
                        lastApplyResult
                            .CreatedConfiguration != null)
                    {
                        Selection.activeObject =
                            lastApplyResult
                                .CreatedConfiguration;
                        EditorGUIUtility.PingObject(
                            lastApplyResult
                                .CreatedConfiguration);
                    }

                    preview =
                        service.Preview(
                            BuildRequest());
                }
            }

            if (lastApplyResult != null)
            {
                MessageType type =
                    lastApplyResult.Status ==
                    EchoSaveSetupResultStatus.Created
                        ? MessageType.Info
                        : MessageType.Warning;

                EditorGUILayout.HelpBox(
                    lastApplyResult.Message,
                    type);
            }

            if (preview != null &&
                !inputsMatchPreview)
            {
                EditorGUILayout.HelpBox(
                    "Inputs changed after Preview. Preview again before Apply.",
                    MessageType.Warning);
            }
        }

        private EchoSaveSetupRequest BuildRequest()
        {
            return new EchoSaveSetupRequest(
                targetAssetPath,
                storageRootDirectoryName,
                slotPolicyMode,
                fixedSlotCount,
                configuredSlotLimit,
                profileSafetyLimit);
        }

        private void DrawRelevantPolicyField()
        {
            switch (slotPolicyMode)
            {
                case SaveSlotPolicyMode.FixedMultiSlot:
                    fixedSlotCount =
                        EditorGUILayout.IntField(
                            "Fixed Slot Count",
                            fixedSlotCount);
                    break;

                case SaveSlotPolicyMode.ConfigurableMultiSlot:
                    configuredSlotLimit =
                        EditorGUILayout.IntField(
                            "Configured Slot Limit",
                            configuredSlotLimit);
                    break;

                case SaveSlotPolicyMode.BoundedProfiles:
                    profileSafetyLimit =
                        EditorGUILayout.IntField(
                            "Profile Safety Limit",
                            profileSafetyLimit);
                    break;
            }
        }

        private void DrawPreview()
        {
            if (preview == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Preview",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "Disposition",
                preview.Disposition.ToString());
            EditorGUILayout.LabelField(
                "Target",
                preview.NormalizedAssetPath);
            EditorGUILayout.LabelField(
                "Destination Available",
                preview.DestinationAvailable
                    ? "Yes"
                    : "No");
            EditorGUILayout.LabelField(
                "Schema",
                preview.SchemaVersion.ToString());
            EditorGUILayout.LabelField(
                "Storage Root",
                preview.NormalizedStorageRoot);
            EditorGUILayout.LabelField(
                "Slot Policy",
                preview.SlotPolicyMode.ToString());
            EditorGUILayout.LabelField(
                "Effective Capacity",
                preview.EffectiveCapacity.ToString());

            for (int i = 0;
                 i < preview.AssetsToCreate.Count;
                 i++)
            {
                EditorGUILayout.LabelField(
                    "Would Create",
                    preview.AssetsToCreate[i]);
            }

            for (int i = 0;
                 i < preview.Messages.Count;
                 i++)
            {
                EchoSaveSetupMessage message =
                    preview.Messages[i];

                EditorGUILayout.HelpBox(
                    $"[{message.Code}] {message.Message}",
                    message.Severity ==
                    EchoSaveSetupMessageSeverity.Blocker
                        ? MessageType.Error
                        : MessageType.Info);
            }
        }
    }
}
