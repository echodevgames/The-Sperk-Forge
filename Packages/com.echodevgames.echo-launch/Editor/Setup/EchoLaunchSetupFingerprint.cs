using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal static class EchoLaunchSetupFingerprint
    {
        internal static string ForRequest(EchoLaunchSetupRequest request)
        {
            if (request == null)
            {
                return Hash("request:null");
            }

            StringBuilder builder = new StringBuilder();
            Append(builder, "root", request.ProjectRootPath);
            Append(builder, "boot", request.BootScenePath);
            Append(builder, "destination", request.DestinationScenePath);
            Append(builder, "splash", request.CreateSplashSequence ? "1" : "0");
            Append(builder, "build", ((int)request.BuildSettingsPolicy).ToString());
            Append(builder, "configuration", request.SelectedConfigurationPath);
            Append(builder, "sequence", request.SelectedStartupSequencePath);
            Append(builder, "launchDestination", request.SelectedLaunchDestinationPath);
            Append(builder, "splashSequence", request.SelectedSplashSequencePath);
            Append(builder, "rootPrefab", request.SelectedRootPrefabPath);
            return Hash(builder.ToString());
        }

        internal static string ForSnapshot(EchoLaunchProjectSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return Hash("snapshot:null");
            }

            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < snapshot.AssetFacts.Count; index++)
            {
                EchoLaunchProjectAssetFact fact = snapshot.AssetFacts[index];
                Append(builder, "asset.path", fact.Path);
                Append(builder, "asset.exists", fact.Exists ? "1" : "0");
                Append(builder, "asset.folder", fact.IsFolder ? "1" : "0");
                Append(builder, "asset.guid", fact.Guid);
                Append(builder, "asset.type", fact.MainAssetTypeName);
                Append(
                    builder,
                    "asset.schema",
                    fact.ConfigurationSchemaVersion.HasValue
                        ? fact.ConfigurationSchemaVersion.Value.ToString()
                        : string.Empty);
                Append(builder, "asset.repairEvidence", fact.HasRepairEvidence ? "1" : "0");
                Append(builder, "asset.stableId", fact.StableId);
                Append(builder, "asset.sequence", fact.StartupSequencePath);
                Append(builder, "asset.destination", fact.LaunchDestinationPath);
                Append(builder, "asset.splash", fact.SplashSequencePath);
                Append(builder, "asset.destinationScene", fact.DestinationScenePath);
                Append(builder, "asset.destinationLabel", fact.DestinationDisplayName);
                Append(builder, "asset.prefabType", fact.PrefabAssetType);
                Append(builder, "asset.prefabSource", fact.PrefabSourcePath);
                Append(builder, "asset.prefabLineage", fact.PrefabLineageMatchesTemplate ? "1" : "0");
                Append(builder, "asset.rootCount", fact.EchoLaunchRootCount.HasValue ? fact.EchoLaunchRootCount.Value.ToString() : string.Empty);
                Append(builder, "asset.rootConfiguration", fact.RootConfigurationPath);
                Append(builder, "asset.sceneSafe", fact.SceneInspectionSafe ? "1" : "0");
                Append(builder, "asset.sceneMessage", fact.SceneInspectionMessage);
                Append(builder, "asset.sceneWasOpen", fact.SceneWasOpen ? "1" : "0");
            }

            for (int index = 0; index < snapshot.BuildSettingsScenes.Count; index++)
            {
                EchoLaunchBuildSettingsSceneFact scene =
                    snapshot.BuildSettingsScenes[index];

                Append(builder, "build.index", scene.Index.ToString());
                Append(builder, "build.path", scene.Path);
                Append(builder, "build.enabled", scene.Enabled ? "1" : "0");
            }

            EchoLaunchSetupAssetRole[] roles =
            {
                EchoLaunchSetupAssetRole.Configuration,
                EchoLaunchSetupAssetRole.StartupSequence,
                EchoLaunchSetupAssetRole.LaunchDestination,
                EchoLaunchSetupAssetRole.SplashSequence,
                EchoLaunchSetupAssetRole.RootPrefab
            };

            for (int roleIndex = 0; roleIndex < roles.Length; roleIndex++)
            {
                IReadOnlyList<EchoLaunchProjectAssetFact> candidates =
                    snapshot.GetCandidates(roles[roleIndex]);

                for (int candidateIndex = 0;
                     candidateIndex < candidates.Count;
                     candidateIndex++)
                {
                    EchoLaunchProjectAssetFact candidate =
                        candidates[candidateIndex];

                    string prefix =
                        "candidate." + ((int)roles[roleIndex]).ToString();

                    Append(builder, prefix + ".path", candidate.Path);
                    Append(builder, prefix + ".guid", candidate.Guid);
                    Append(builder, prefix + ".type", candidate.MainAssetTypeName);
                    Append(builder, prefix + ".folder", candidate.IsFolder ? "1" : "0");
                    Append(
                        builder,
                        prefix + ".schema",
                        candidate.ConfigurationSchemaVersion.HasValue
                            ? candidate.ConfigurationSchemaVersion.Value.ToString()
                            : string.Empty);
                    Append(
                        builder,
                        prefix + ".repairEvidence",
                        candidate.HasRepairEvidence ? "1" : "0");
                    Append(builder, prefix + ".stableId", candidate.StableId);
                    Append(
                        builder,
                        prefix + ".sequence",
                        candidate.StartupSequencePath);
                    Append(
                        builder,
                        prefix + ".destination",
                        candidate.LaunchDestinationPath);
                    Append(
                        builder,
                        prefix + ".splash",
                        candidate.SplashSequencePath);
                    Append(
                        builder,
                        prefix + ".destinationScene",
                        candidate.DestinationScenePath);
                    Append(
                        builder,
                        prefix + ".destinationLabel",
                        candidate.DestinationDisplayName);
                    Append(
                        builder,
                        prefix + ".prefabType",
                        candidate.PrefabAssetType);
                    Append(
                        builder,
                        prefix + ".prefabSource",
                        candidate.PrefabSourcePath);
                    Append(
                        builder,
                        prefix + ".prefabLineage",
                        candidate.PrefabLineageMatchesTemplate ? "1" : "0");
                    Append(
                        builder,
                        prefix + ".rootCount",
                        candidate.EchoLaunchRootCount.HasValue
                            ? candidate.EchoLaunchRootCount.Value.ToString()
                            : string.Empty);
                    Append(
                        builder,
                        prefix + ".rootConfiguration",
                        candidate.RootConfigurationPath);
                    Append(
                        builder,
                        prefix + ".sceneSafe",
                        candidate.SceneInspectionSafe ? "1" : "0");
                    Append(
                        builder,
                        prefix + ".sceneMessage",
                        candidate.SceneInspectionMessage);
                    Append(
                        builder,
                        prefix + ".sceneWasOpen",
                        candidate.SceneWasOpen ? "1" : "0");
                }
            }

            Append(
                builder,
                "template.available",
                snapshot.PackageRootTemplateAvailable ? "1" : "0");

            Append(builder, "template.guid", snapshot.PackageRootTemplateGuid);
            return Hash(builder.ToString());
        }

        internal static string ForPlan(
            string requestFingerprint,
            string evidenceFingerprint,
            EchoLaunchSetupPlanStatus status,
            IReadOnlyList<EchoLaunchSetupOperation> operations,
            IReadOnlyList<EchoLaunchSetupDiagnostic> diagnostics)
        {
            StringBuilder builder = new StringBuilder();
            Append(builder, "request", requestFingerprint);
            Append(builder, "evidence", evidenceFingerprint);
            Append(builder, "status", ((int)status).ToString());

            if (operations != null)
            {
                for (int index = 0; index < operations.Count; index++)
                {
                    EchoLaunchSetupOperation operation = operations[index];
                    Append(builder, "operation.key", operation.Key);
                    Append(builder, "operation.phase", operation.Phase.ToString());
                    Append(builder, "operation.kind", ((int)operation.Kind).ToString());
                    Append(
                        builder,
                        "operation.disposition",
                        ((int)operation.Disposition).ToString());
                    Append(builder, "operation.path", operation.TargetPath);
                    Append(builder, "operation.reason", operation.Reason);
                    Append(builder, "operation.code", operation.DiagnosticCode);
                    Append(
                        builder,
                        "operation.approval",
                        operation.RequiresExplicitApproval ? "1" : "0");
                    Append(builder, "operation.before", operation.ExistingState);
                    Append(builder, "operation.after", operation.ProposedState);
                    Append(builder, "operation.proof", operation.ProofSummary);
                }
            }

            if (diagnostics != null)
            {
                for (int index = 0; index < diagnostics.Count; index++)
                {
                    EchoLaunchSetupDiagnostic diagnostic = diagnostics[index];
                    Append(builder, "diagnostic.code", diagnostic.Code);
                    Append(
                        builder,
                        "diagnostic.severity",
                        ((int)diagnostic.Severity).ToString());
                    Append(builder, "diagnostic.message", diagnostic.Message);
                    Append(builder, "diagnostic.path", diagnostic.TargetPath);
                }
            }

            return Hash(builder.ToString());
        }

        internal static string Hash(string value)
        {
            byte[] input = Encoding.UTF8.GetBytes(value ?? string.Empty);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] digest = sha256.ComputeHash(input);
                StringBuilder builder = new StringBuilder(digest.Length * 2);

                for (int index = 0; index < digest.Length; index++)
                {
                    builder.Append(digest[index].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static void Append(
            StringBuilder builder,
            string key,
            string value)
        {
            string safeValue = value ?? string.Empty;
            builder.Append(key);
            builder.Append(':');
            builder.Append(safeValue.Length);
            builder.Append(':');
            builder.Append(safeValue);
            builder.Append('\n');
        }
    }
}
