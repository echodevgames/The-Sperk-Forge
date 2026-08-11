using System;
using System.Collections.Generic;
using System.IO;
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
        private const string InvalidSlotPolicyCode = "ESV-SETUP-005";

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

            string normalizedAssetPath =
                NormalizeAssetPath(
                    request.TargetAssetPath);

            bool safeTarget =
                IsSafeAssetPath(
                    normalizedAssetPath,
                    out string pathMessage);

            if (!safeTarget)
            {
                messages.Add(
                    Blocker(
                        InvalidTargetCode,
                        pathMessage));
            }

            bool destinationAvailable = false;

            if (safeTarget)
            {
                string parentPath =
                    GetParentAssetPath(
                        normalizedAssetPath);

                if (!AssetDatabase.IsValidFolder(
                        parentPath))
                {
                    messages.Add(
                        Blocker(
                            MissingFolderCode,
                            $"The target folder '{parentPath}' does not exist. M5-01 Setup does not create project folders."));
                }
                else if (
                    AssetDatabase.LoadMainAssetAtPath(
                        normalizedAssetPath) != null ||
                    File.Exists(
                        ToProjectAbsolutePath(
                            normalizedAssetPath)))
                {
                    messages.Add(
                        Blocker(
                            OccupiedTargetCode,
                            $"The target '{normalizedAssetPath}' is already occupied. M5-01 Setup never overwrites or edits an existing asset."));
                }
                else
                {
                    destinationAvailable = true;
                }
            }

            string normalizedStorageRoot =
                (request.StorageRootDirectoryName ??
                 string.Empty).Trim();

            if (!IsSafeStorageRoot(
                    normalizedStorageRoot))
            {
                messages.Add(
                    Blocker(
                        InvalidStorageRootCode,
                        "The Chronicle storage root must be one non-empty safe relative directory segment."));
            }

            int effectiveCapacity = 0;
            if (!TryResolveEffectiveCapacity(
                    normalizedStorageRoot,
                    request,
                    out effectiveCapacity,
                    out string slotPolicyMessage))
            {
                messages.Add(
                    Blocker(
                        InvalidSlotPolicyCode,
                        slotPolicyMessage));
            }

            EchoSaveSetupDisposition disposition =
                HasBlocker(messages)
                    ? EchoSaveSetupDisposition.Rejected
                    : EchoSaveSetupDisposition.Create;

            if (disposition ==
                EchoSaveSetupDisposition.Create)
            {
                assetsToCreate.Add(
                    normalizedAssetPath);
            }

            return new EchoSaveSetupPlan(
                request,
                ComputeRequestFingerprint(request),
                normalizedAssetPath,
                normalizedStorageRoot,
                EchoSaveConfiguration.CurrentSchemaVersion,
                request.SlotPolicyMode,
                effectiveCapacity,
                destinationAvailable,
                disposition,
                assetsToCreate.ToArray(),
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
                Preview(plan.Request);

            if (!EquivalentForApply(
                    plan,
                    current))
            {
                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Rejected,
                    plan.NormalizedAssetPath,
                    null,
                    "The Chronicle Setup preview is stale or no longer applicable. Preview again before Apply.");
            }

            if (!current.CanApply)
            {
                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Rejected,
                    current.NormalizedAssetPath,
                    null,
                    FirstMessageOrFallback(current));
            }

            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            try
            {
                AuthorConfiguration(
                    configuration,
                    current);

                AssetDatabase.CreateAsset(
                    configuration,
                    current.NormalizedAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(
                    current.NormalizedAssetPath,
                    ImportAssetOptions.ForceSynchronousImport);

                UnityEngine.Object created =
                    AssetDatabase.LoadAssetAtPath<
                        EchoSaveConfiguration>(
                        current.NormalizedAssetPath);

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Created,
                    current.NormalizedAssetPath,
                    created as EchoSaveConfiguration,
                    $"Created Chronicle configuration at '{current.NormalizedAssetPath}'.");
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
                        UnityEngine.Object.DestroyImmediate(
                            configuration);
                    }
                }

                return new EchoSaveSetupResult(
                    EchoSaveSetupResultStatus.Failed,
                    current.NormalizedAssetPath,
                    null,
                    $"Chronicle Setup failed without overwriting an existing asset: {exception.GetType().Name}.");
            }
        }

        public static string ComputeRequestFingerprint(
            EchoSaveSetupRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            string serialized =
                string.Concat(
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
                    request.ProfileSafetyLimit.ToString());

            return serialized;
        }

        private static bool EquivalentForApply(
            EchoSaveSetupPlan original,
            EchoSaveSetupPlan current)
        {
            return current != null &&
                   current.CanApply &&
                   original.CanApply &&
                   string.Equals(
                       original.RequestFingerprint,
                       current.RequestFingerprint,
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
                   original.SlotPolicyMode ==
                   current.SlotPolicyMode &&
                   original.EffectiveCapacity ==
                   current.EffectiveCapacity &&
                   original.DestinationAvailable ==
                   current.DestinationAvailable;
        }

        private static void AuthorConfiguration(
            EchoSaveConfiguration configuration,
            EchoSaveSetupPlan plan)
        {
            var serializedObject =
                new SerializedObject(configuration);

            serializedObject
                .FindProperty("schemaVersion")
                .intValue =
                EchoSaveConfiguration.CurrentSchemaVersion;

            serializedObject
                .FindProperty(
                    "storageRootDirectoryName")
                .stringValue =
                plan.NormalizedStorageRoot;

            serializedObject
                .FindProperty("slotPolicyMode")
                .enumValueIndex =
                (int)plan.Request.SlotPolicyMode;

            serializedObject
                .FindProperty("fixedSlotCount")
                .intValue =
                plan.Request.FixedSlotCount;

            serializedObject
                .FindProperty("configuredSlotLimit")
                .intValue =
                plan.Request.ConfiguredSlotLimit;

            serializedObject
                .FindProperty("profileSafetyLimit")
                .intValue =
                plan.Request.ProfileSafetyLimit;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static bool TryResolveEffectiveCapacity(
            string normalizedStorageRoot,
            EchoSaveSetupRequest request,
            out int effectiveCapacity,
            out string message)
        {
            effectiveCapacity = 0;

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
                var serializedObject =
                    new SerializedObject(configuration);

                serializedObject
                    .FindProperty("schemaVersion")
                    .intValue =
                    EchoSaveConfiguration.CurrentSchemaVersion;

                serializedObject
                    .FindProperty(
                        "storageRootDirectoryName")
                    .stringValue =
                    normalizedStorageRoot;

                serializedObject
                    .FindProperty("slotPolicyMode")
                    .enumValueIndex =
                    (int)request.SlotPolicyMode;

                serializedObject
                    .FindProperty("fixedSlotCount")
                    .intValue =
                    request.FixedSlotCount;

                serializedObject
                    .FindProperty("configuredSlotLimit")
                    .intValue =
                    request.ConfiguredSlotLimit;

                serializedObject
                    .FindProperty("profileSafetyLimit")
                    .intValue =
                    request.ProfileSafetyLimit;

                serializedObject.ApplyModifiedPropertiesWithoutUndo();

                if (!configuration.TryResolveSlotPolicy(
                        out SaveSlotPolicy policy,
                        out message))
                {
                    return false;
                }

                effectiveCapacity =
                    policy.EffectiveCapacity;
                return true;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
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

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length == 0 ||
                    segments[i] == "." ||
                    segments[i] == "..")
                {
                    message =
                        "The target asset path contains an unsafe empty or dot segment.";
                    return false;
                }

                if (segments[i].IndexOf(':') >= 0)
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

            for (int i = 0; i < value.Length; i++)
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
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Severity ==
                    EchoSaveSetupMessageSeverity.Blocker)
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
                    projectRoot ?? string.Empty,
                    assetPath));
        }
    }
}
