using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveValidationService
    {
        public const string MissingConfigurationCheckId =
            "ESV-VAL-001";
        public const string UnsafeStorageRootCheckId =
            "ESV-VAL-002";
        public const string DuplicateRootsCheckId =
            "ESV-VAL-003";
        public const string DuplicateFixedSlotIdsCheckId =
            "ESV-VAL-004";
        public const string InvalidRetentionCheckId =
            "ESV-VAL-005";
        public const string MissingProviderCheckId =
            "ESV-VAL-006";
        public const string RuntimeEditorReferenceCheckId =
            "ESV-VAL-009";
        public const string InvalidSlotPolicyCheckId =
            "ESV-VAL-015";
        public const string InvalidLimitPolicyCheckId =
            "ESV-VAL-016";

        private const string RuntimeAsmdefPath =
            "Packages/com.echodevgames.echo-save/Runtime/EchoDevGames.EchoSave.Runtime.asmdef";
        private const string RuntimeDirectoryPath =
            "Packages/com.echodevgames.echo-save/Runtime";

        public EchoSaveValidationReport Validate(
            EchoSaveConfiguration configuration)
        {
            var issues =
                new List<EchoSaveValidationIssue>();

            ValidateConfiguration(
                configuration,
                issues);
            ValidateLoadedSceneRoots(
                issues);
            ValidateRuntimeAssemblyIsolation(
                issues);

            issues.Sort(CompareIssues);

            return new EchoSaveValidationReport(
                issues.ToArray());
        }

        private static void ValidateConfiguration(
            EchoSaveConfiguration configuration,
            List<EchoSaveValidationIssue> issues)
        {
            if (configuration == null)
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        MissingConfigurationCheckId,
                        EchoSaveValidationSeverity.Error,
                        "No Chronicle configuration is selected for validation.",
                        string.Empty,
                        true,
                        false));
                return;
            }

            string root =
                configuration
                    .StorageRootDirectoryName
                    .Trim();

            if (!IsSafeStorageRoot(root))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        UnsafeStorageRootCheckId,
                        EchoSaveValidationSeverity.Error,
                        "The Chronicle storage root must be one non-empty safe relative directory segment.",
                        GetObjectContext(configuration),
                        true,
                        false));
            }

            if (!configuration.TryResolveSlotPolicy(
                    out _,
                    out string policyMessage))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        InvalidSlotPolicyCheckId,
                        EchoSaveValidationSeverity.Error,
                        policyMessage,
                        GetObjectContext(configuration),
                        true,
                        false));
            }

            if (configuration.SchemaVersion ==
                EchoSaveConfiguration.CurrentSchemaVersion)
            {
                ValidateCurrentSchemaConfiguration(
                    configuration,
                    issues);
            }
        }

        private static void ValidateCurrentSchemaConfiguration(
            EchoSaveConfiguration configuration,
            List<EchoSaveValidationIssue> issues)
        {
            string context =
                GetObjectContext(configuration);

            SaveRetentionPolicy retention =
                new SaveRetentionPolicy(
                    configuration.MaxTotalGenerations);

            if (!retention.IsValid)
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        InvalidRetentionCheckId,
                        EchoSaveValidationSeverity.Error,
                        $"Chronicle retention must keep between {SaveRetentionPolicy.MinimumTotalGenerations} and {SaveRetentionPolicy.MaximumTotalGenerations} committed generations.",
                        context,
                        true,
                        false));
            }

            if (!string.Equals(
                    configuration.SerializerProviderId.Trim(),
                    EchoSaveConfiguration.DefaultSerializerProviderId,
                    StringComparison.Ordinal))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        MissingProviderCheckId,
                        EchoSaveValidationSeverity.Error,
                        $"Configured serializer provider '{configuration.SerializerProviderId.Trim()}' is unavailable. M5-02 currently resolves '{EchoSaveConfiguration.DefaultSerializerProviderId}'.",
                        $"{context} / serializerProviderId",
                        true,
                        false));
            }

            if (!string.Equals(
                    configuration.StorageProviderId.Trim(),
                    EchoSaveConfiguration.DefaultStorageProviderId,
                    StringComparison.Ordinal))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        MissingProviderCheckId,
                        EchoSaveValidationSeverity.Error,
                        $"Configured storage provider '{configuration.StorageProviderId.Trim()}' is unavailable. M5-02 currently resolves '{EchoSaveConfiguration.DefaultStorageProviderId}'.",
                        $"{context} / storageProviderId",
                        true,
                        false));
            }

            if (!configuration.TryValidateFixedSlotTemplates(
                    out string templateMessage))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        DuplicateFixedSlotIdsCheckId,
                        EchoSaveValidationSeverity.Error,
                        templateMessage,
                        context,
                        true,
                        false));
            }

            SaveLimitPolicy limits =
                new SaveLimitPolicy(
                    configuration.CatalogScanLimit,
                    configuration.RetentionDiscoveryLimit,
                    configuration.RecoveryDiscoveryLimit);

            if (!limits.IsValid)
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        InvalidLimitPolicyCheckId,
                        EchoSaveValidationSeverity.Error,
                        $"Chronicle discovery limits must each be between {SaveLimitPolicy.MinimumDiscoveryLimit} and {SaveLimitPolicy.MaximumDiscoveryLimit}.",
                        context,
                        true,
                        false));
            }
        }

        private static void ValidateLoadedSceneRoots(
            List<EchoSaveValidationIssue> issues)
        {
            EchoSaveRoot[] roots =
                Resources.FindObjectsOfTypeAll<
                    EchoSaveRoot>();

            int loadedSceneRootCount = 0;

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
                    loadedSceneRootCount++;
                }
            }

            if (loadedSceneRootCount > 1)
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        DuplicateRootsCheckId,
                        EchoSaveValidationSeverity.Error,
                        $"Found {loadedSceneRootCount} EchoSaveRoot components in loaded scenes. Chronicle requires at most one loaded-scene root authority.",
                        $"{loadedSceneRootCount} loaded-scene roots",
                        false,
                        false));
            }
        }

        private static void ValidateRuntimeAssemblyIsolation(
            List<EchoSaveValidationIssue> issues)
        {
            string projectRoot =
                Path.GetDirectoryName(
                    Application.dataPath) ??
                Directory.GetCurrentDirectory();

            string asmdefAbsolutePath =
                Path.GetFullPath(
                    Path.Combine(
                        projectRoot,
                        RuntimeAsmdefPath));

            if (!File.Exists(
                    asmdefAbsolutePath))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        RuntimeEditorReferenceCheckId,
                        EchoSaveValidationSeverity.Error,
                        "The Chronicle Runtime asmdef could not be located for Editor-reference validation.",
                        RuntimeAsmdefPath,
                        false,
                        false));
                return;
            }

            string asmdefText =
                File.ReadAllText(
                    asmdefAbsolutePath);

            if (ContainsUnityEditorReference(
                    asmdefText) ||
                RuntimeSourcesReferenceUnityEditor(
                    projectRoot))
            {
                issues.Add(
                    new EchoSaveValidationIssue(
                        RuntimeEditorReferenceCheckId,
                        EchoSaveValidationSeverity.Error,
                        "The Chronicle Runtime assembly references UnityEditor. Editor dependencies must remain isolated in EchoDevGames.EchoSave.Editor.",
                        RuntimeAsmdefPath,
                        true,
                        false));
            }
        }

        private static bool RuntimeSourcesReferenceUnityEditor(
            string projectRoot)
        {
            string runtimeAbsolutePath =
                Path.GetFullPath(
                    Path.Combine(
                        projectRoot,
                        RuntimeDirectoryPath));

            if (!Directory.Exists(
                    runtimeAbsolutePath))
            {
                return true;
            }

            string[] files =
                Directory.GetFiles(
                    runtimeAbsolutePath,
                    "*.cs",
                    SearchOption.AllDirectories);

            for (int i = 0;
                 i < files.Length;
                 i++)
            {
                string text =
                    File.ReadAllText(files[i]);

                if (text.IndexOf(
                        "using UnityEditor",
                        StringComparison.Ordinal) >= 0 ||
                    text.IndexOf(
                        "UnityEditor.",
                        StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsUnityEditorReference(
            string asmdefText)
        {
            return (asmdefText ?? string.Empty)
                .IndexOf(
                    "UnityEditor",
                    StringComparison.Ordinal) >= 0;
        }

        private static bool IsSafeStorageRoot(
            string value)
        {
            if (string.IsNullOrEmpty(value) ||
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

        private static string GetObjectContext(
            UnityEngine.Object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string path =
                UnityEditor.AssetDatabase
                    .GetAssetPath(value);

            return string.IsNullOrEmpty(path)
                ? value.name
                : path;
        }

        private static int CompareIssues(
            EchoSaveValidationIssue left,
            EchoSaveValidationIssue right)
        {
            int severity =
                left.Severity.CompareTo(
                    right.Severity);

            if (severity != 0)
            {
                return severity;
            }

            int id =
                string.CompareOrdinal(
                    left.CheckId,
                    right.CheckId);

            if (id != 0)
            {
                return id;
            }

            return string.CompareOrdinal(
                left.Context,
                right.Context);
        }
    }
}
