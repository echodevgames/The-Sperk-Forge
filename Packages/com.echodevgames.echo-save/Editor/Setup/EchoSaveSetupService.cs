using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveSetupService
    {
        private const string InvalidTargetCode = "ESV-SETUP-001";
        private const string OccupiedTargetCode = "ESV-SETUP-002";
        private const string MissingFolderCode = "ESV-SETUP-003";
        private const string InvalidStorageRootCode = "ESV-SETUP-004";
        private const string InvalidPolicyCode = "ESV-SETUP-005";
        private const string InvalidExistingAssetCode = "ESV-SETUP-006";
        private const string StalePreviewCode = "ESV-SETUP-007";
        private const string InvalidRootRepairCode = "ESV-SETUP-008";
        private const string DuplicateRootRepairCode = "ESV-SETUP-009";

        public EchoSaveSetupPlan Preview(
            EchoSaveSetupRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }

            var messages =
                new List<EchoSaveSetupMessage>();

            var assetsToCreate =
                new List<string>();

            var changes =
                new List<EchoSaveSetupChange>();

            string normalizedStorageRoot =
                (request.StorageRootDirectoryName ??
                 string.Empty).Trim();

            string normalizedAssetPath;
            bool destinationAvailable;
            int sourceSchemaVersion;
            string targetStateFingerprint;

            if (request.IsEdit)
            {
                EchoSaveConfiguration existing =
                    request.ExistingConfiguration;

                normalizedAssetPath =
                    AssetDatabase.GetAssetPath(
                        existing);

                sourceSchemaVersion =
                    existing != null
                        ? existing.SchemaVersion
                        : 0;

                targetStateFingerprint =
                    ComputeConfigurationStateFingerprint(
                        existing);

                destinationAvailable =
                    ValidateExistingConfigurationTarget(
                        existing,
                        normalizedAssetPath,
                        messages);
            }
            else
            {
                normalizedAssetPath =
                    NormalizeAssetPath(
                        request.TargetAssetPath);

                sourceSchemaVersion = 0;
                targetStateFingerprint =
                    ComputeCreateTargetStateFingerprint(
                        normalizedAssetPath);

                destinationAvailable =
                    ValidateCreateTarget(
                        normalizedAssetPath,
                        messages);

                if (destinationAvailable)
                {
                    assetsToCreate.Add(
                        normalizedAssetPath);
                }
            }

            if (!IsSafeStorageRoot(
                    normalizedStorageRoot))
            {
                messages.Add(
                    Blocker(
                        InvalidStorageRootCode,
                        "The Chronicle storage root must be one non-empty safe relative directory segment."));
            }

            int effectiveCapacity = 0;

            if (!TryResolveRequestRuntimePolicy(
                    normalizedStorageRoot,
                    request,
                    out EchoSaveRuntimePolicy resolvedPolicy,
                    out string policyMessage))
            {
                messages.Add(
                    Blocker(
                        InvalidPolicyCode,
                        policyMessage));
            }
            else
            {
                effectiveCapacity =
                    resolvedPolicy
                        .SlotPolicy
                        .EffectiveCapacity;
            }

            if (request.IsEdit &&
                request.ExistingConfiguration != null &&
                !HasBlocker(messages))
            {
                BuildConfigurationChanges(
                    request.ExistingConfiguration,
                    request,
                    normalizedStorageRoot,
                    changes);
            }

            EchoSaveSetupDisposition disposition;

            if (HasBlocker(messages))
            {
                disposition =
                    EchoSaveSetupDisposition.Rejected;
            }
            else if (!request.IsEdit)
            {
                disposition =
                    EchoSaveSetupDisposition.Create;
            }
            else if (changes.Count == 0)
            {
                disposition =
                    EchoSaveSetupDisposition.NoChanges;
            }
            else
            {
                disposition =
                    EchoSaveSetupDisposition.Update;
            }

            return new EchoSaveSetupPlan(
                request,
                ComputeRequestFingerprint(request),
                targetStateFingerprint,
                normalizedAssetPath,
                normalizedStorageRoot,
                sourceSchemaVersion,
                EchoSaveConfiguration.CurrentSchemaVersion,
                request.SlotPolicyMode,
                effectiveCapacity,
                destinationAvailable,
                disposition,
                assetsToCreate.ToArray(),
                changes.ToArray(),
                messages.ToArray());
        }

        public EchoSaveSetupResult Apply(
            EchoSaveSetupPlan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            EchoSaveSetupPlan current =
                Preview(
                    plan.Request);

            if (!EquivalentForApply(
                    plan,
                    current))
            {
                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Rejected,
                    plan.NormalizedAssetPath,
                    plan.Request.ExistingConfiguration,
                    $"[{StalePreviewCode}] The Chronicle Setup preview is stale or no longer applicable. Preview again before Apply.");
            }

            if (current.Disposition ==
                EchoSaveSetupDisposition.NoChanges)
            {
                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.NoChanges,
                    current.NormalizedAssetPath,
                    current.Request.ExistingConfiguration,
                    "The Chronicle configuration already matches the previewed schema-3 authoring state.");
            }

            if (!current.CanApply)
            {
                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Rejected,
                    current.NormalizedAssetPath,
                    current.Request.ExistingConfiguration,
                    FirstMessageOrFallback(current));
            }

            return current.Request.IsEdit
                ? ApplyExistingConfiguration(current)
                : ApplyNewConfiguration(current);
        }

        public EchoSaveRootRepairPlan PreviewRootRepair(
            EchoSaveRoot root,
            EchoSaveConfiguration targetConfiguration)
        {
            var messages =
                new List<EchoSaveSetupMessage>();

            var changes =
                new List<EchoSaveSetupChange>();

            if (root == null ||
                root.gameObject == null)
            {
                messages.Add(
                    Blocker(
                        InvalidRootRepairCode,
                        "Select one project-owned EchoSaveRoot before previewing a reference repair."));
            }

            if (targetConfiguration == null)
            {
                messages.Add(
                    Blocker(
                        InvalidRootRepairCode,
                        "Select one project-owned EchoSaveConfiguration before previewing a root reference repair."));
            }

            if (root != null &&
                root.gameObject != null &&
                CountLoadedSceneRoots() > 1)
            {
                messages.Add(
                    Blocker(
                        DuplicateRootRepairCode,
                        "Multiple loaded-scene EchoSaveRoot components exist. M5-02 will not choose one authority automatically."));
            }

            if (targetConfiguration != null)
            {
                string configPath =
                    AssetDatabase.GetAssetPath(
                        targetConfiguration);

                if (string.IsNullOrEmpty(configPath) ||
                    !configPath.StartsWith(
                        "Assets/",
                        StringComparison.Ordinal))
                {
                    messages.Add(
                        Blocker(
                            InvalidRootRepairCode,
                            "The target configuration must be one project-owned asset under Assets/."));
                }
            }

            EchoSaveConfiguration serializedRootConfiguration =
                GetSerializedRootConfiguration(root);

            if (!HasBlocker(messages) &&
                serializedRootConfiguration !=
                targetConfiguration)
            {
                changes.Add(
                    new EchoSaveSetupChange(
                        "EchoSaveRoot.configuration",
                        ObjectContext(serializedRootConfiguration),
                        ObjectContext(targetConfiguration)));
            }

            EchoSaveSetupDisposition disposition;

            if (HasBlocker(messages))
            {
                disposition =
                    EchoSaveSetupDisposition.Rejected;
            }
            else if (changes.Count == 0)
            {
                disposition =
                    EchoSaveSetupDisposition.NoChanges;
            }
            else
            {
                disposition =
                    EchoSaveSetupDisposition.Update;
            }

            return new EchoSaveRootRepairPlan(
                root,
                targetConfiguration,
                ComputeRootRepairStateFingerprint(
                    root,
                    targetConfiguration),
                disposition,
                changes.ToArray(),
                messages.ToArray());
        }

        public EchoSaveRootRepairResult ApplyRootRepair(
            EchoSaveRootRepairPlan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            EchoSaveRootRepairPlan current =
                PreviewRootRepair(
                    plan.Root,
                    plan.TargetConfiguration);

            if (!string.Equals(
                    plan.StateFingerprint,
                    current.StateFingerprint,
                    StringComparison.Ordinal) ||
                plan.Disposition !=
                current.Disposition)
            {
                return new EchoSaveRootRepairResult(
                    EchoSaveSetupResultStatus.Rejected,
                    plan.Root,
                    $"[{StalePreviewCode}] The selected Chronicle root/reference state changed after Preview.");
            }

            if (current.Disposition ==
                EchoSaveSetupDisposition.NoChanges)
            {
                return new EchoSaveRootRepairResult(
                    EchoSaveSetupResultStatus.NoChanges,
                    current.Root,
                    "The selected Chronicle root already references the previewed configuration.");
            }

            if (!current.CanApply)
            {
                return new EchoSaveRootRepairResult(
                    EchoSaveSetupResultStatus.Rejected,
                    current.Root,
                    current.Messages.Count > 0
                        ? current.Messages[0].Message
                        : "The Chronicle root reference repair is blocked.");
            }

            int undoGroup =
                Undo.GetCurrentGroup();

            Undo.SetCurrentGroupName(
                "Chronicle Root Configuration Repair");

            try
            {
                Undo.RecordObject(
                    current.Root,
                    "Chronicle Root Configuration Repair");

                var serialized =
                    new SerializedObject(
                        current.Root);

                SerializedProperty configuration =
                    serialized.FindProperty(
                        "configuration");

                if (configuration == null)
                {
                    throw new InvalidOperationException(
                        "EchoSaveRoot configuration property could not be located.");
                }

                configuration.objectReferenceValue =
                    current.TargetConfiguration;

                serialized.ApplyModifiedProperties();

                if (PrefabUtility.IsPartOfPrefabInstance(
                        current.Root))
                {
                    PrefabUtility
                        .RecordPrefabInstancePropertyModifications(
                            current.Root);
                }

                EditorUtility.SetDirty(
                    current.Root);

                UnityEngine.SceneManagement.Scene rootScene =
                    current.Root.gameObject.scene;

                if (rootScene.IsValid() &&
                    rootScene.isLoaded)
                {
                    UnityEditor.SceneManagement
                        .EditorSceneManager
                        .MarkSceneDirty(rootScene);
                }

                return new EchoSaveRootRepairResult(
                    EchoSaveSetupResultStatus.Updated,
                    current.Root,
                    "Updated the selected Chronicle root configuration reference through one Undo-recorded repair.");
            }
            catch (Exception exception)
            {
                Undo.RevertAllDownToGroup(
                    undoGroup);

                return new EchoSaveRootRepairResult(
                    EchoSaveSetupResultStatus.Failed,
                    current.Root,
                    $"Chronicle root reference repair failed and was reverted: {exception.GetType().Name}.");
            }
        }

        public static string ComputeRequestFingerprint(
            EchoSaveSetupRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            string templateIds =
                string.Join(
                    "\n",
                    request
                        .FixedSlotTemplates
                        .Select(
                            template =>
                                template != null
                                    ? template.GetInstanceID().ToString()
                                    : "null"));

            return string.Concat(
                request.ExistingConfiguration != null
                    ? request.ExistingConfiguration
                        .GetInstanceID()
                        .ToString()
                    : "create",
                "\n",
                NormalizeAssetPath(
                    request.TargetAssetPath),
                "\n",
                (request.StorageRootDirectoryName ??
                 string.Empty).Trim(),
                "\n",
                ((int)request.SlotPolicyMode).ToString(),
                "\n",
                request.FixedSlotCount.ToString(),
                "\n",
                request.ConfiguredSlotLimit.ToString(),
                "\n",
                request.ProfileSafetyLimit.ToString(),
                "\n",
                request.MaxTotalGenerations.ToString(),
                "\n",
                (request.SerializerProviderId ??
                 string.Empty).Trim(),
                "\n",
                (request.StorageProviderId ??
                 string.Empty).Trim(),
                "\n",
                request.CatalogScanLimit.ToString(),
                "\n",
                request.RetentionDiscoveryLimit.ToString(),
                "\n",
                request.RecoveryDiscoveryLimit.ToString(),
                "\n",
                ((int)request.RecoveryPolicyMode).ToString(),
                "\n",
                templateIds);
        }

        private static EchoSaveSetupResult
            ApplyNewConfiguration(
                EchoSaveSetupPlan plan)
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            try
            {
                AuthorConfiguration(
                    configuration,
                    plan.Request,
                    plan.NormalizedStorageRoot,
                    false);

                AssetDatabase.CreateAsset(
                    configuration,
                    plan.NormalizedAssetPath);

                AssetDatabase.SaveAssets();

                AssetDatabase.ImportAsset(
                    plan.NormalizedAssetPath,
                    ImportAssetOptions
                        .ForceSynchronousImport);

                EchoSaveConfiguration created =
                    AssetDatabase.LoadAssetAtPath<
                        EchoSaveConfiguration>(
                        plan.NormalizedAssetPath);

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Created,
                    plan.NormalizedAssetPath,
                    created,
                    $"Created Chronicle schema-{EchoSaveConfiguration.CurrentSchemaVersion} configuration at '{plan.NormalizedAssetPath}'.");
            }
            catch (Exception exception)
            {
                if (configuration != null)
                {
                    string createdPath =
                        AssetDatabase.GetAssetPath(
                            configuration);

                    if (!string.IsNullOrEmpty(
                            createdPath))
                    {
                        AssetDatabase.DeleteAsset(
                            createdPath);
                    }
                    else
                    {
                        UnityEngine.Object
                            .DestroyImmediate(
                                configuration);
                    }
                }

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Failed,
                    plan.NormalizedAssetPath,
                    null,
                    $"Chronicle Setup failed without overwriting an existing asset: {exception.GetType().Name}.");
            }
        }

        private static EchoSaveSetupResult
            ApplyExistingConfiguration(
                EchoSaveSetupPlan plan)
        {
            EchoSaveConfiguration configuration =
                plan.Request.ExistingConfiguration;

            int undoGroup =
                Undo.GetCurrentGroup();

            Undo.SetCurrentGroupName(
                "Chronicle Configuration Update");

            try
            {
                Undo.RecordObject(
                    configuration,
                    "Chronicle Configuration Update");

                AuthorConfiguration(
                    configuration,
                    plan.Request,
                    plan.NormalizedStorageRoot,
                    true);

                EditorUtility.SetDirty(
                    configuration);

                AssetDatabase.SaveAssets();

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Updated,
                    plan.NormalizedAssetPath,
                    configuration,
                    $"Updated '{plan.NormalizedAssetPath}' explicitly to Chronicle schema {EchoSaveConfiguration.CurrentSchemaVersion}.");
            }
            catch (Exception exception)
            {
                Undo.RevertAllDownToGroup(
                    undoGroup);

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Failed,
                    plan.NormalizedAssetPath,
                    configuration,
                    $"Chronicle configuration update failed and was reverted: {exception.GetType().Name}.");
            }
        }

        private static bool EquivalentForApply(
            EchoSaveSetupPlan original,
            EchoSaveSetupPlan current)
        {
            return current != null &&
                   original != null &&
                   original.CanApply &&
                   current.CanApply &&
                   original.Disposition ==
                   current.Disposition &&
                   string.Equals(
                       original.RequestFingerprint,
                       current.RequestFingerprint,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       original.TargetStateFingerprint,
                       current.TargetStateFingerprint,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       original.NormalizedAssetPath,
                       current.NormalizedAssetPath,
                       StringComparison.Ordinal) &&
                   string.Equals(
                       original.NormalizedStorageRoot,
                       current.NormalizedStorageRoot,
                       StringComparison.Ordinal) &&
                   original.SchemaVersion ==
                   current.SchemaVersion &&
                   original.EffectiveCapacity ==
                   current.EffectiveCapacity;
        }

        private static bool ValidateCreateTarget(
            string normalizedAssetPath,
            List<EchoSaveSetupMessage> messages)
        {
            if (!IsSafeAssetPath(
                    normalizedAssetPath,
                    out string pathMessage))
            {
                messages.Add(
                    Blocker(
                        InvalidTargetCode,
                        pathMessage));
                return false;
            }

            string parentPath =
                GetParentAssetPath(
                    normalizedAssetPath);

            if (!AssetDatabase.IsValidFolder(
                    parentPath))
            {
                messages.Add(
                    Blocker(
                        MissingFolderCode,
                        $"The target folder '{parentPath}' does not exist. Chronicle Setup does not create project folders implicitly."));
                return false;
            }

            if (AssetDatabase.LoadMainAssetAtPath(
                    normalizedAssetPath) != null ||
                File.Exists(
                    ToProjectAbsolutePath(
                        normalizedAssetPath)))
            {
                messages.Add(
                    Blocker(
                        OccupiedTargetCode,
                        $"The target '{normalizedAssetPath}' is already occupied. Create mode never overwrites an existing asset; select it in Edit mode instead."));
                return false;
            }

            return true;
        }

        private static bool
            ValidateExistingConfigurationTarget(
                EchoSaveConfiguration configuration,
                string path,
                List<EchoSaveSetupMessage> messages)
        {
            if (configuration == null ||
                string.IsNullOrEmpty(path) ||
                !path.StartsWith(
                    "Assets/",
                    StringComparison.Ordinal) ||
                AssetDatabase.LoadAssetAtPath<
                    EchoSaveConfiguration>(
                        path) !=
                configuration)
            {
                messages.Add(
                    Blocker(
                        InvalidExistingAssetCode,
                        "Edit mode requires one selected project-owned EchoSaveConfiguration asset under Assets/."));
                return false;
            }

            return true;
        }

        private static void BuildConfigurationChanges(
            EchoSaveConfiguration existing,
            EchoSaveSetupRequest request,
            string normalizedStorageRoot,
            List<EchoSaveSetupChange> changes)
        {
            AddChange(
                changes,
                "Schema Version",
                existing.SchemaVersion,
                EchoSaveConfiguration
                    .CurrentSchemaVersion);

            AddChange(
                changes,
                "Storage Root",
                existing.StorageRootDirectoryName,
                normalizedStorageRoot);

            AddChange(
                changes,
                "Slot Policy",
                existing.SlotPolicyMode,
                request.SlotPolicyMode);

            AddChange(
                changes,
                "Fixed Slot Count",
                existing.FixedSlotCount,
                request.FixedSlotCount);

            AddChange(
                changes,
                "Configured Slot Limit",
                existing.ConfiguredSlotLimit,
                request.ConfiguredSlotLimit);

            AddChange(
                changes,
                "Profile Safety Limit",
                existing.ProfileSafetyLimit,
                request.ProfileSafetyLimit);

            if (existing.SchemaVersion ==
                EchoSaveConfiguration
                    .CurrentSchemaVersion)
            {
                AddChange(
                    changes,
                    "Max Total Generations",
                    existing.MaxTotalGenerations,
                    request.MaxTotalGenerations);

                AddChange(
                    changes,
                    "Serializer Provider",
                    existing.SerializerProviderId,
                    (request.SerializerProviderId ??
                     string.Empty).Trim());

                AddChange(
                    changes,
                    "Storage Provider",
                    existing.StorageProviderId,
                    (request.StorageProviderId ??
                     string.Empty).Trim());

                AddChange(
                    changes,
                    "Catalog Scan Limit",
                    existing.CatalogScanLimit,
                    request.CatalogScanLimit);

                AddChange(
                    changes,
                    "Retention Discovery Limit",
                    existing.RetentionDiscoveryLimit,
                    request.RetentionDiscoveryLimit);

                AddChange(
                    changes,
                    "Recovery Discovery Limit",
                    existing.RecoveryDiscoveryLimit,
                    request.RecoveryDiscoveryLimit);

                AddChange(
                    changes,
                    "Recovery Policy",
                    existing.RecoveryPolicyMode,
                    request.RecoveryPolicyMode);

                string beforeTemplates =
                    DescribeTemplates(
                        existing.FixedSlotTemplates);

                string afterTemplates =
                    DescribeTemplates(
                        request.FixedSlotTemplates);

                if (!string.Equals(
                        beforeTemplates,
                        afterTemplates,
                        StringComparison.Ordinal))
                {
                    changes.Add(
                        new EchoSaveSetupChange(
                            "Fixed Slot Templates",
                            beforeTemplates,
                            afterTemplates));
                }
            }
            else
            {
                AddChange(
                    changes,
                    "Max Total Generations",
                    SaveRetentionPolicy
                        .DefaultTotalGenerations,
                    request.MaxTotalGenerations);

                AddChange(
                    changes,
                    "Serializer Provider",
                    EchoSaveConfiguration
                        .DefaultSerializerProviderId,
                    (request.SerializerProviderId ??
                     string.Empty).Trim());

                AddChange(
                    changes,
                    "Storage Provider",
                    EchoSaveConfiguration
                        .DefaultStorageProviderId,
                    (request.StorageProviderId ??
                     string.Empty).Trim());

                AddChange(
                    changes,
                    "Catalog Scan Limit",
                    SaveLimitPolicy
                        .DefaultCatalogScanLimit,
                    request.CatalogScanLimit);

                AddChange(
                    changes,
                    "Retention Discovery Limit",
                    SaveLimitPolicy
                        .DefaultRetentionDiscoveryLimit,
                    request.RetentionDiscoveryLimit);

                AddChange(
                    changes,
                    "Recovery Discovery Limit",
                    SaveLimitPolicy
                        .DefaultRecoveryDiscoveryLimit,
                    request.RecoveryDiscoveryLimit);

                AddChange(
                    changes,
                    "Recovery Policy",
                    EchoSaveRecoveryPolicyMode
                        .ManualOnly,
                    request.RecoveryPolicyMode);

                if (request.FixedSlotTemplates.Count >
                    0)
                {
                    changes.Add(
                        new EchoSaveSetupChange(
                            "Fixed Slot Templates",
                            "(compatibility default: none)",
                            DescribeTemplates(
                                request.FixedSlotTemplates)));
                }
            }
        }

        private static void AddChange<T>(
            List<EchoSaveSetupChange> changes,
            string name,
            T before,
            T after)
        {
            if (EqualityComparer<T>
                .Default
                .Equals(
                    before,
                    after))
            {
                return;
            }

            changes.Add(
                new EchoSaveSetupChange(
                    name,
                    ReferenceEquals(before, null)
                        ? string.Empty
                        : before.ToString(),
                    ReferenceEquals(after, null)
                        ? string.Empty
                        : after.ToString()));
        }

        private static bool TryResolveRequestRuntimePolicy(
            string normalizedStorageRoot,
            EchoSaveSetupRequest request,
            out EchoSaveRuntimePolicy policy,
            out string message)
        {
            policy = null;

            if (!Enum.IsDefined(
                    typeof(SaveSlotPolicyMode),
                    request.SlotPolicyMode))
            {
                message =
                    $"Chronicle slot policy mode value {(int)request.SlotPolicyMode} is undefined.";
                return false;
            }

            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            try
            {
                AuthorConfiguration(
                    configuration,
                    request,
                    normalizedStorageRoot,
                    false);

                if (!configuration
                    .TryValidateFixedSlotTemplates(
                        out message))
                {
                    return false;
                }

                return configuration
                    .TryResolveRuntimePolicy(
                        out policy,
                        out message);
            }
            finally
            {
                UnityEngine.Object
                    .DestroyImmediate(
                        configuration);
            }
        }

        private static void AuthorConfiguration(
            EchoSaveConfiguration configuration,
            EchoSaveSetupRequest request,
            string normalizedStorageRoot,
            bool withUndo)
        {
            var serialized =
                new SerializedObject(
                    configuration);

            serialized
                .FindProperty("schemaVersion")
                .intValue =
                EchoSaveConfiguration
                    .CurrentSchemaVersion;

            serialized
                .FindProperty(
                    "storageRootDirectoryName")
                .stringValue =
                normalizedStorageRoot;

            serialized
                .FindProperty("slotPolicyMode")
                .enumValueIndex =
                (int)request.SlotPolicyMode;

            serialized
                .FindProperty("fixedSlotCount")
                .intValue =
                request.FixedSlotCount;

            serialized
                .FindProperty(
                    "configuredSlotLimit")
                .intValue =
                request.ConfiguredSlotLimit;

            serialized
                .FindProperty(
                    "profileSafetyLimit")
                .intValue =
                request.ProfileSafetyLimit;

            serialized
                .FindProperty(
                    "maxTotalGenerations")
                .intValue =
                request.MaxTotalGenerations;

            serialized
                .FindProperty(
                    "serializerProviderId")
                .stringValue =
                (request.SerializerProviderId ??
                 string.Empty).Trim();

            serialized
                .FindProperty(
                    "storageProviderId")
                .stringValue =
                (request.StorageProviderId ??
                 string.Empty).Trim();

            serialized
                .FindProperty(
                    "catalogScanLimit")
                .intValue =
                request.CatalogScanLimit;

            serialized
                .FindProperty(
                    "retentionDiscoveryLimit")
                .intValue =
                request.RetentionDiscoveryLimit;

            serialized
                .FindProperty(
                    "recoveryDiscoveryLimit")
                .intValue =
                request.RecoveryDiscoveryLimit;

            serialized
                .FindProperty(
                    "recoveryPolicyMode")
                .enumValueIndex =
                (int)request.RecoveryPolicyMode;

            SerializedProperty templates =
                serialized.FindProperty(
                    "fixedSlotTemplates");

            templates.arraySize =
                request.FixedSlotTemplates.Count;

            for (int i = 0;
                 i < request.FixedSlotTemplates.Count;
                 i++)
            {
                templates
                    .GetArrayElementAtIndex(i)
                    .objectReferenceValue =
                    request.FixedSlotTemplates[i];
            }

            if (withUndo)
            {
                serialized.ApplyModifiedProperties();
            }
            else
            {
                serialized
                    .ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static string
            ComputeConfigurationStateFingerprint(
                EchoSaveConfiguration configuration)
        {
            if (configuration == null)
            {
                return string.Empty;
            }

            return string.Concat(
                AssetDatabase.GetAssetPath(
                    configuration),
                "\n",
                EditorJsonUtility.ToJson(
                    configuration),
                "\n",
                configuration.GetInstanceID()
                    .ToString());
        }

        private static string
            ComputeCreateTargetStateFingerprint(
                string path)
        {
            UnityEngine.Object asset =
                AssetDatabase.LoadMainAssetAtPath(
                    path);

            return string.Concat(
                path,
                "\n",
                asset != null
                    ? asset.GetInstanceID()
                        .ToString()
                    : "empty",
                "\n",
                File.Exists(
                    ToProjectAbsolutePath(path))
                    ? "file"
                    : "no-file");
        }

        private static string
            ComputeRootRepairStateFingerprint(
                EchoSaveRoot root,
                EchoSaveConfiguration target)
        {
            return string.Concat(
                root != null
                    ? root.GetInstanceID()
                        .ToString()
                    : "null",
                "\n",
                GetSerializedRootConfiguration(root) != null
                    ? GetSerializedRootConfiguration(root)
                        .GetInstanceID()
                        .ToString()
                    : "none",
                "\n",
                target != null
                    ? target.GetInstanceID()
                        .ToString()
                    : "null",
                "\n",
                CountLoadedSceneRoots()
                    .ToString());
        }

        private static EchoSaveConfiguration
            GetSerializedRootConfiguration(
                EchoSaveRoot root)
        {
            if (root == null)
            {
                return null;
            }

            var serialized =
                new SerializedObject(root);
            SerializedProperty configuration =
                serialized.FindProperty(
                    "configuration");

            return configuration != null
                ? configuration.objectReferenceValue as
                    EchoSaveConfiguration
                : null;
        }

        private static int CountLoadedSceneRoots()
        {
            EchoSaveRoot[] roots =
                Resources.FindObjectsOfTypeAll<
                    EchoSaveRoot>();

            int count = 0;

            for (int i = 0;
                 i < roots.Length;
                 i++)
            {
                EchoSaveRoot root =
                    roots[i];

                if (root == null ||
                    root.gameObject == null)
                {
                    continue;
                }

                UnityEngine.SceneManagement.Scene scene =
                    root.gameObject.scene;

                if (scene.IsValid() &&
                    scene.isLoaded)
                {
                    count++;
                }
            }

            return count;
        }

        private static string DescribeTemplates(
            IReadOnlyList<SaveSlotTemplate> templates)
        {
            if (templates == null ||
                templates.Count == 0)
            {
                return "(none)";
            }

            return string.Join(
                ", ",
                templates.Select(
                    template =>
                        template != null
                            ? template.TemplateId
                            : "(missing)"));
        }

        private static string ObjectContext(
            UnityEngine.Object value)
        {
            if (value == null)
            {
                return "(none)";
            }

            string path =
                AssetDatabase.GetAssetPath(
                    value);

            return !string.IsNullOrEmpty(path)
                ? path
                : value.name;
        }

        private static string NormalizeAssetPath(
            string path)
        {
            string value =
                (path ?? string.Empty)
                .Trim()
                .Replace('\\', '/');

            while (value.Contains("//"))
            {
                value =
                    value.Replace("//", "/");
            }

            while (value.StartsWith(
                       "Assets/Assets/",
                       StringComparison.Ordinal))
            {
                value =
                    value.Substring(
                        "Assets/".Length);
            }

            if (value.Length > 0 &&
                !value.StartsWith(
                    "Assets/",
                    StringComparison.Ordinal) &&
                !value.StartsWith(
                    "Packages/",
                    StringComparison.Ordinal))
            {
                value =
                    "Assets/" + value;
            }

            return value;
        }

        private static bool IsSafeAssetPath(
            string path,
            out string message)
        {
            if (path.Length == 0)
            {
                message =
                    "The target asset path is empty.";
                return false;
            }

            if (!path.StartsWith(
                    "Assets/",
                    StringComparison.Ordinal))
            {
                message =
                    "The target asset path must be project-relative and located under 'Assets/'.";
                return false;
            }

            if (!path.EndsWith(
                    ".asset",
                    StringComparison.OrdinalIgnoreCase))
            {
                message =
                    "The target asset path must end in '.asset'.";
                return false;
            }

            string[] segments =
                path.Split('/');

            for (int i = 0;
                 i < segments.Length;
                 i++)
            {
                if (segments[i].Length == 0 ||
                    segments[i] == "." ||
                    segments[i] == "..")
                {
                    message =
                        "The target asset path contains an unsafe empty or dot segment.";
                    return false;
                }

                if (segments[i]
                    .IndexOf(':') >= 0)
                {
                    message =
                        "The target asset path contains an unsafe ':' character.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        private static string GetParentAssetPath(
            string assetPath)
        {
            int separator =
                assetPath.LastIndexOf('/');

            return separator > 0
                ? assetPath.Substring(
                    0,
                    separator)
                : string.Empty;
        }

        private static bool IsSafeStorageRoot(
            string value)
        {
            if (value.Length == 0 ||
                value == "." ||
                value == ".." ||
                value.IndexOf('/') >= 0 ||
                value.IndexOf('\\') >= 0 ||
                value.IndexOf(':') >= 0)
            {
                return false;
            }

            for (int i = 0;
                 i < value.Length;
                 i++)
            {
                if (char.IsControl(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasBlocker(
            List<EchoSaveSetupMessage> messages)
        {
            for (int i = 0;
                 i < messages.Count;
                 i++)
            {
                if (messages[i].Severity ==
                    EchoSaveSetupMessageSeverity
                        .Blocker)
                {
                    return true;
                }
            }

            return false;
        }

        private static EchoSaveSetupMessage Blocker(
            string code,
            string message)
        {
            return new EchoSaveSetupMessage(
                code,
                EchoSaveSetupMessageSeverity.Blocker,
                message);
        }

        private static string FirstMessageOrFallback(
            EchoSaveSetupPlan plan)
        {
            return plan.Messages.Count > 0
                ? plan.Messages[0].Message
                : "Chronicle Setup cannot apply this plan.";
        }

        private static string ToProjectAbsolutePath(
            string assetPath)
        {
            string projectRoot =
                Path.GetDirectoryName(
                    Application.dataPath);

            return Path.GetFullPath(
                Path.Combine(
                    projectRoot ??
                    string.Empty,
                    assetPath));
        }
    }
}
