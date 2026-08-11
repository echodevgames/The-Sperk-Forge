using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveSetupWindow : EditorWindow
    {
        private enum SetupMode
        {
            CreateConfiguration = 0,
            EditConfiguration = 1,
            RepairRootReference = 2
        }

        private readonly EchoSaveSetupService service =
            new EchoSaveSetupService();

        private SetupMode mode;
        private EchoSaveConfiguration existingConfiguration;
        private EchoSaveRoot repairRoot;
        private EchoSaveConfiguration repairConfiguration;

        private string targetAssetPath =
            "Assets/EchoSaveConfiguration.asset";
        private string storageRootDirectoryName =
            "EchoSave";
        private SaveSlotPolicyMode slotPolicyMode =
            SaveSlotPolicyMode.ConfigurableMultiSlot;
        private int fixedSlotCount = 4;
        private int configuredSlotLimit = 64;
        private int profileSafetyLimit = 64;
        private int maxTotalGenerations =
            SaveRetentionPolicy.DefaultTotalGenerations;
        private string serializerProviderId =
            EchoSaveConfiguration.DefaultSerializerProviderId;
        private string storageProviderId =
            EchoSaveConfiguration.DefaultStorageProviderId;
        private int catalogScanLimit =
            SaveLimitPolicy.DefaultCatalogScanLimit;
        private int retentionDiscoveryLimit =
            SaveLimitPolicy.DefaultRetentionDiscoveryLimit;
        private int recoveryDiscoveryLimit =
            SaveLimitPolicy.DefaultRecoveryDiscoveryLimit;
        private EchoSaveRecoveryPolicyMode recoveryPolicyMode =
            EchoSaveRecoveryPolicyMode.ManualOnly;
        private readonly List<SaveSlotTemplate> fixedSlotTemplates =
            new List<SaveSlotTemplate>();

        private EchoSaveSetupPlan preview;
        private EchoSaveSetupResult lastApplyResult;
        private EchoSaveRootRepairPlan repairPreview;
        private EchoSaveRootRepairResult lastRepairResult;

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
                "M5-02 never silently upgrades or repairs project state. Preview is read-only. Apply revalidates the selected target and changes only the previewed project asset/reference.",
                MessageType.Info);

            SetupMode nextMode =
                (SetupMode)EditorGUILayout.EnumPopup(
                    "Mode",
                    mode);

            if (nextMode != mode)
            {
                mode = nextMode;
                ClearPreview();
            }

            EditorGUILayout.Space();

            switch (mode)
            {
                case SetupMode.CreateConfiguration:
                    DrawCreateOrEdit(false);
                    break;

                case SetupMode.EditConfiguration:
                    DrawCreateOrEdit(true);
                    break;

                case SetupMode.RepairRootReference:
                    DrawRootRepair();
                    break;
            }
        }

        private void DrawCreateOrEdit(bool edit)
        {
            if (edit)
            {
                EchoSaveConfiguration next =
                    (EchoSaveConfiguration)
                    EditorGUILayout.ObjectField(
                        "Configuration",
                        existingConfiguration,
                        typeof(EchoSaveConfiguration),
                        false);

                if (next != existingConfiguration)
                {
                    existingConfiguration = next;
                    ClearPreview();
                }

                using (new EditorGUI.DisabledScope(
                           existingConfiguration == null))
                {
                    if (GUILayout.Button(
                            "Load Selected Values"))
                    {
                        LoadExistingConfiguration();
                        ClearPreview();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Configuration Asset accepts a project path under Assets/. A leading Assets/ is optional; repeated Assets/ prefixes are normalized instead of duplicated.",
                    MessageType.None);

                targetAssetPath =
                    EditorGUILayout.TextField(
                        "Configuration Asset",
                        targetAssetPath);
            }

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
            EditorGUILayout.LabelField(
                "Runtime Policy",
                EditorStyles.boldLabel);

            maxTotalGenerations =
                EditorGUILayout.IntField(
                    "Max Total Generations",
                    maxTotalGenerations);
            serializerProviderId =
                EditorGUILayout.TextField(
                    "Serializer Provider",
                    serializerProviderId);
            storageProviderId =
                EditorGUILayout.TextField(
                    "Storage Provider",
                    storageProviderId);
            catalogScanLimit =
                EditorGUILayout.IntField(
                    "Catalog Scan Limit",
                    catalogScanLimit);
            retentionDiscoveryLimit =
                EditorGUILayout.IntField(
                    "Retention Discovery Limit",
                    retentionDiscoveryLimit);
            recoveryDiscoveryLimit =
                EditorGUILayout.IntField(
                    "Recovery Discovery Limit",
                    recoveryDiscoveryLimit);
            recoveryPolicyMode =
                (EchoSaveRecoveryPolicyMode)
                EditorGUILayout.EnumPopup(
                    "Recovery Policy",
                    recoveryPolicyMode);

            DrawFixedSlotTemplates();

            EditorGUILayout.Space();

            if (GUILayout.Button("Preview"))
            {
                preview = service.Preview(BuildRequest(edit));
                lastApplyResult = null;
            }

            DrawPreview();

            EchoSaveSetupRequest currentRequest =
                BuildRequest(edit);
            bool inputsMatchPreview =
                preview != null &&
                string.Equals(
                    preview.RequestFingerprint,
                    EchoSaveSetupService
                        .ComputeRequestFingerprint(
                            currentRequest),
                    StringComparison.Ordinal);

            using (new EditorGUI.DisabledScope(
                       preview == null ||
                       !preview.CanApply ||
                       !inputsMatchPreview))
            {
                if (GUILayout.Button("Apply"))
                {
                    lastApplyResult =
                        service.Apply(preview);

                    if (lastApplyResult.Configuration != null)
                    {
                        existingConfiguration =
                            lastApplyResult.Configuration;
                        Selection.activeObject =
                            lastApplyResult.Configuration;
                        EditorGUIUtility.PingObject(
                            lastApplyResult.Configuration);
                    }

                    preview =
                        service.Preview(
                            BuildRequest(edit));
                }
            }

            if (lastApplyResult != null)
            {
                EditorGUILayout.HelpBox(
                    lastApplyResult.Message,
                    lastApplyResult.Status ==
                    EchoSaveSetupResultStatus.Created ||
                    lastApplyResult.Status ==
                    EchoSaveSetupResultStatus.Updated ||
                    lastApplyResult.Status ==
                    EchoSaveSetupResultStatus.NoChanges
                        ? MessageType.Info
                        : MessageType.Warning);
            }

            if (preview != null &&
                !inputsMatchPreview)
            {
                EditorGUILayout.HelpBox(
                    "Inputs changed after Preview. Preview again before Apply.",
                    MessageType.Warning);
            }
        }

        private void DrawRootRepair()
        {
            repairRoot =
                (EchoSaveRoot)
                EditorGUILayout.ObjectField(
                    "Selected Root",
                    repairRoot,
                    typeof(EchoSaveRoot),
                    true);

            repairConfiguration =
                (EchoSaveConfiguration)
                EditorGUILayout.ObjectField(
                    "Target Configuration",
                    repairConfiguration,
                    typeof(EchoSaveConfiguration),
                    false);

            EditorGUILayout.HelpBox(
                "Root repair changes only the selected EchoSaveRoot.configuration reference. Duplicate loaded roots are blocked rather than auto-resolved.",
                MessageType.None);

            if (GUILayout.Button("Preview Repair"))
            {
                repairPreview =
                    service.PreviewRootRepair(
                        repairRoot,
                        repairConfiguration);
                lastRepairResult = null;
            }

            DrawRootRepairPreview();

            using (new EditorGUI.DisabledScope(
                       repairPreview == null ||
                       !repairPreview.CanApply))
            {
                if (GUILayout.Button("Apply Repair"))
                {
                    lastRepairResult =
                        service.ApplyRootRepair(
                            repairPreview);
                    repairPreview =
                        service.PreviewRootRepair(
                            repairRoot,
                            repairConfiguration);
                }
            }

            if (lastRepairResult != null)
            {
                EditorGUILayout.HelpBox(
                    lastRepairResult.Message,
                    lastRepairResult.Status ==
                    EchoSaveSetupResultStatus.Updated ||
                    lastRepairResult.Status ==
                    EchoSaveSetupResultStatus.NoChanges
                        ? MessageType.Info
                        : MessageType.Warning);
            }
        }

        private EchoSaveSetupRequest BuildRequest(
            bool edit)
        {
            string path =
                edit && existingConfiguration != null
                    ? AssetDatabase.GetAssetPath(
                        existingConfiguration)
                    : targetAssetPath;

            return new EchoSaveSetupRequest(
                edit ? existingConfiguration : null,
                path,
                storageRootDirectoryName,
                slotPolicyMode,
                fixedSlotCount,
                configuredSlotLimit,
                profileSafetyLimit,
                maxTotalGenerations,
                serializerProviderId,
                storageProviderId,
                catalogScanLimit,
                retentionDiscoveryLimit,
                recoveryDiscoveryLimit,
                recoveryPolicyMode,
                fixedSlotTemplates.ToArray());
        }

        private void LoadExistingConfiguration()
        {
            if (existingConfiguration == null)
            {
                return;
            }

            targetAssetPath =
                AssetDatabase.GetAssetPath(
                    existingConfiguration);
            storageRootDirectoryName =
                existingConfiguration
                    .StorageRootDirectoryName;
            slotPolicyMode =
                existingConfiguration.SlotPolicyMode;
            fixedSlotCount =
                existingConfiguration.FixedSlotCount;
            configuredSlotLimit =
                existingConfiguration.ConfiguredSlotLimit;
            profileSafetyLimit =
                existingConfiguration.ProfileSafetyLimit;

            if (existingConfiguration.SchemaVersion >=
                EchoSaveConfiguration.CurrentSchemaVersion)
            {
                maxTotalGenerations =
                    existingConfiguration.MaxTotalGenerations;
                serializerProviderId =
                    existingConfiguration.SerializerProviderId;
                storageProviderId =
                    existingConfiguration.StorageProviderId;
                catalogScanLimit =
                    existingConfiguration.CatalogScanLimit;
                retentionDiscoveryLimit =
                    existingConfiguration.RetentionDiscoveryLimit;
                recoveryDiscoveryLimit =
                    existingConfiguration.RecoveryDiscoveryLimit;
                recoveryPolicyMode =
                    existingConfiguration.RecoveryPolicyMode;
                fixedSlotTemplates.Clear();
                fixedSlotTemplates.AddRange(
                    existingConfiguration.FixedSlotTemplates);
            }
            else
            {
                maxTotalGenerations =
                    SaveRetentionPolicy
                        .DefaultTotalGenerations;
                serializerProviderId =
                    EchoSaveConfiguration
                        .DefaultSerializerProviderId;
                storageProviderId =
                    EchoSaveConfiguration
                        .DefaultStorageProviderId;
                catalogScanLimit =
                    SaveLimitPolicy
                        .DefaultCatalogScanLimit;
                retentionDiscoveryLimit =
                    SaveLimitPolicy
                        .DefaultRetentionDiscoveryLimit;
                recoveryDiscoveryLimit =
                    SaveLimitPolicy
                        .DefaultRecoveryDiscoveryLimit;
                recoveryPolicyMode =
                    EchoSaveRecoveryPolicyMode.ManualOnly;
                fixedSlotTemplates.Clear();
            }
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

        private void DrawFixedSlotTemplates()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Fixed Slot Templates (Authoring Metadata)",
                EditorStyles.boldLabel);

            for (int i = 0;
                 i < fixedSlotTemplates.Count;
                 i++)
            {
                EditorGUILayout.BeginHorizontal();
                fixedSlotTemplates[i] =
                    (SaveSlotTemplate)
                    EditorGUILayout.ObjectField(
                        $"Template {i + 1}",
                        fixedSlotTemplates[i],
                        typeof(SaveSlotTemplate),
                        false);

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(70f)))
                {
                    fixedSlotTemplates.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Template"))
            {
                fixedSlotTemplates.Add(null);
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
                "Source Schema",
                preview.SourceSchemaVersion.ToString());
            EditorGUILayout.LabelField(
                "Target Schema",
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

            DrawChanges(preview.Changes);
            DrawMessages(preview.Messages);
        }

        private void DrawRootRepairPreview()
        {
            if (repairPreview == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Repair Preview",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Disposition",
                repairPreview.Disposition.ToString());
            DrawChanges(repairPreview.Changes);
            DrawMessages(repairPreview.Messages);
        }

        private static void DrawChanges(
            IReadOnlyList<EchoSaveSetupChange> changes)
        {
            for (int i = 0;
                 i < changes.Count;
                 i++)
            {
                EchoSaveSetupChange change =
                    changes[i];
                EditorGUILayout.LabelField(
                    $"Would Change: {change.PropertyName}: '{change.Before}' -> '{change.After}'",
                    EditorStyles.wordWrappedLabel);
            }
        }

        private static void DrawMessages(
            IReadOnlyList<EchoSaveSetupMessage> messages)
        {
            for (int i = 0;
                 i < messages.Count;
                 i++)
            {
                EchoSaveSetupMessage message =
                    messages[i];
                EditorGUILayout.HelpBox(
                    $"[{message.Code}] {message.Message}",
                    message.Severity ==
                    EchoSaveSetupMessageSeverity.Blocker
                        ? MessageType.Error
                        : MessageType.Info);
            }
        }

        private void ClearPreview()
        {
            preview = null;
            lastApplyResult = null;
            repairPreview = null;
            lastRepairResult = null;
        }
    }
}
