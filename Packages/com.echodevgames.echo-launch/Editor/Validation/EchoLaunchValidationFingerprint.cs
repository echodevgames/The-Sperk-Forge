using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal static class EchoLaunchValidationFingerprint
    {
        internal static string ForRequest(
            EchoLaunchValidationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return Hash(
                "ValidationRequest\n" +
                request.ProjectRootPath + "\n" +
                (request.IncludeInformation ? "1" : "0"));
        }

        internal static string ForEvidence(
            EchoLaunchValidationEvidence evidence)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException(nameof(evidence));
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ValidationEvidence");
            builder.AppendLine(evidence.Paths.ProjectRootPath);
            builder.AppendLine(evidence.Paths.BootScenePath);
            builder.AppendLine(Bool(evidence.PackageTemplateAvailable));
            AppendAsset(builder, evidence.Configuration);
            AppendAsset(builder, evidence.StartupSequence);
            AppendAsset(builder, evidence.Destination);
            AppendAsset(builder, evidence.SplashSequence);
            AppendRootPrefab(builder, evidence.RootPrefab);
            builder.AppendLine(evidence.ConfigurationStartupSequencePath);
            builder.AppendLine(evidence.ConfigurationDestinationPath);
            builder.AppendLine(evidence.ConfigurationSplashPath);

            for (int index = 0; index < evidence.SequenceEntries.Count; index++)
            {
                EchoLaunchValidationSequenceEntryEvidence entry =
                    evidence.SequenceEntries[index];

                builder.Append("Entry|")
                    .Append(entry.Index.ToString(CultureInfo.InvariantCulture))
                    .Append('|').Append(entry.EntryId)
                    .Append('|').Append(Bool(entry.Enabled))
                    .Append('|').Append(entry.DefinitionPath)
                    .Append('|').Append(entry.DefinitionId)
                    .Append('|').Append(entry.DefinitionSchema.ToString(CultureInfo.InvariantCulture))
                    .Append('|').Append(Bool(entry.IsRequired))
                    .Append('|').Append(Bool(entry.IsOptional))
                    .Append('|').Append(entry.FailureActionValue.ToString(CultureInfo.InvariantCulture))
                    .Append('|').Append(Float(entry.TimeoutSeconds))
                    .Append('|').Append(Bool(entry.SupportsCancellation))
                    .AppendLine();
            }

            builder.AppendLine(evidence.DestinationScenePath);
            builder.AppendLine(evidence.DestinationDisplayName);

            for (int index = 0; index < evidence.SplashEntries.Count; index++)
            {
                EchoLaunchValidationSplashEntryEvidence entry =
                    evidence.SplashEntries[index];

                builder.Append("Splash|")
                    .Append(entry.Index.ToString(CultureInfo.InvariantCulture))
                    .Append('|').Append(entry.EntryId)
                    .Append('|').Append(entry.ImagePath)
                    .Append('|').Append(Float(entry.FadeInSeconds))
                    .Append('|').Append(Float(entry.HoldSeconds))
                    .Append('|').Append(Float(entry.FadeOutSeconds))
                    .Append('|').Append(Float(entry.MinimumDisplaySeconds))
                    .Append('|').Append(entry.SkipPolicyValue.ToString(CultureInfo.InvariantCulture))
                    .AppendLine();
            }

            for (int index = 0; index < evidence.BuildSettingsScenes.Count; index++)
            {
                EchoLaunchValidationBuildSceneEvidence scene =
                    evidence.BuildSettingsScenes[index];

                builder.Append("Build|")
                    .Append(scene.Index.ToString(CultureInfo.InvariantCulture))
                    .Append('|').Append(Bool(scene.Enabled))
                    .Append('|').Append(scene.Path)
                    .AppendLine();
            }

            for (int index = 0; index < evidence.SceneEvidence.Count; index++)
            {
                EchoLaunchValidationSceneEvidence scene =
                    evidence.SceneEvidence[index];

                builder.Append("Scene|")
                    .Append(scene.Path)
                    .Append('|').Append(Bool(scene.Exists))
                    .Append('|').Append(Bool(scene.Inspected))
                    .AppendLine();

                for (int rootIndex = 0;
                     rootIndex < scene.Roots.Count;
                     rootIndex++)
                {
                    AppendRoot(builder, scene.Roots[rootIndex]);
                }

                for (int initializerIndex = 0;
                     initializerIndex < scene.DirectInitializers.Count;
                     initializerIndex++)
                {
                    AppendDirectInitializer(
                        builder,
                        scene.DirectInitializers[initializerIndex]);
                }
            }

            for (int index = 0; index < evidence.CollectionIssues.Count; index++)
            {
                builder.Append("Issue|")
                    .Append(evidence.CollectionIssues[index])
                    .AppendLine();
            }

            return Hash(builder.ToString());
        }

        internal static string ForReportCore(
            int schemaVersion,
            EchoLaunchValidationRequest request,
            string requestFingerprint,
            string evidenceFingerprint,
            EchoLaunchProjectHealth health,
            IList<EchoLaunchValidationFinding> findings)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ValidationReport");
            builder.AppendLine(schemaVersion.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine(request.ProjectRootPath);
            builder.AppendLine(request.IncludeInformation ? "1" : "0");
            builder.AppendLine(requestFingerprint ?? string.Empty);
            builder.AppendLine(evidenceFingerprint ?? string.Empty);
            builder.AppendLine(((int)health).ToString(CultureInfo.InvariantCulture));

            if (findings != null)
            {
                for (int index = 0; index < findings.Count; index++)
                {
                    EchoLaunchValidationFinding finding = findings[index];

                    builder.Append(finding.Code).Append('|')
                        .Append(((int)finding.Severity).ToString(CultureInfo.InvariantCulture)).Append('|')
                        .Append(finding.Title).Append('|')
                        .Append(finding.ProjectPath).Append('|')
                        .Append(finding.Message).Append('|')
                        .Append(finding.Evidence).Append('|')
                        .Append(finding.SuggestedAction)
                        .AppendLine();
                }
            }

            return Hash(builder.ToString());
        }

        internal static string Hash(string value)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(value ?? string.Empty);
                byte[] hash = sha.ComputeHash(bytes);
                StringBuilder builder =
                    new StringBuilder(hash.Length * 2);

                for (int index = 0; index < hash.Length; index++)
                {
                    builder.Append(hash[index].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static void AppendAsset(
            StringBuilder builder,
            EchoLaunchValidationAssetEvidence asset)
        {
            builder.Append("Asset|")
                .Append(asset.Path)
                .Append('|').Append(Bool(asset.Exists))
                .Append('|').Append(asset.TypeName)
                .Append('|').Append(asset.StableId)
                .Append('|').Append(asset.SchemaVersion.ToString(CultureInfo.InvariantCulture))
                .AppendLine();
        }

        private static void AppendRootPrefab(
            StringBuilder builder,
            EchoLaunchValidationRootPrefabEvidence prefab)
        {
            builder.Append("Prefab|")
                .Append(prefab.Path)
                .Append('|').Append(Bool(prefab.Exists))
                .Append('|').Append(Bool(prefab.ReachesPackageTemplate))
                .AppendLine();

            for (int index = 0; index < prefab.Roots.Count; index++)
            {
                AppendRoot(builder, prefab.Roots[index]);
            }
        }

        private static void AppendRoot(
            StringBuilder builder,
            EchoLaunchValidationRootEvidence root)
        {
            builder.Append("Root|")
                .Append(root.ConfigurationPath)
                .Append('|').Append(root.PrefabSourcePath)
                .Append('|').Append(Bool(root.HasStatusPresenter))
                .Append('|').Append(Bool(root.HasImageSplashPresenter))
                .AppendLine();
        }

        private static void AppendDirectInitializer(
            StringBuilder builder,
            EchoLaunchValidationDirectSceneEvidence initializer)
        {
            builder.Append("Direct|")
                .Append(initializer.ContainingScenePath)
                .Append('|').Append(Bool(initializer.ComponentEnabled))
                .Append('|').Append(
                    initializer.PolicyValue.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(initializer.DirectConfigurationPath)
                .Append('|').Append(initializer.DirectConfigurationTypeName)
                .Append('|').Append(initializer.DirectConfigurationId)
                .Append('|').Append(
                    initializer.DirectConfigurationSchema.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(initializer.RootPrefabPath)
                .Append('|').Append(
                    initializer.RootCount.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(
                    initializer.ActiveRootCount.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(
                    Bool(initializer.ReachesPackageTemplate))
                .Append('|').Append(
                    initializer.LaunchModeValue.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(initializer.LaunchConfigurationPath)
                .Append('|').Append(
                    initializer.LaunchConfigurationSchema.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(initializer.DestinationAssetPath)
                .Append('|').Append(
                    initializer.DestinationSchema.ToString(
                        CultureInfo.InvariantCulture))
                .Append('|').Append(initializer.DestinationScenePath)
                .AppendLine();
        }

        private static string Bool(bool value)
        {
            return value ? "1" : "0";
        }

        private static string Float(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }
    }
}
