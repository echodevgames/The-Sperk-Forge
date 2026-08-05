
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    internal static class EchoLaunchSetupTestFactory
    {
        internal const string DestinationScenePath =
            "Assets/Scenes/MainMenu.unity";

        internal static EchoLaunchSetupRequest CreateRequest(
            bool createSplash = false,
            EchoLaunchBuildSettingsPolicy policy =
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
            string destinationPath = DestinationScenePath,
            string projectRoot =
                EchoLaunchSetupPathSet.DefaultProjectRootPath,
            string bootPath = null,
            string selectedConfiguration = null)
        {
            string resolvedBoot =
                bootPath ?? projectRoot + "/Scenes/Boot.unity";

            return new EchoLaunchSetupRequest(
                projectRoot,
                resolvedBoot,
                destinationPath,
                createSplash,
                policy,
                selectedConfiguration);
        }

        internal static EchoLaunchProjectSnapshot CreateSnapshot(
            IEnumerable<EchoLaunchProjectAssetFact> facts = null,
            IEnumerable<EchoLaunchBuildSettingsSceneFact> buildScenes = null,
            bool templateAvailable = true,
            IDictionary<
                EchoLaunchSetupAssetRole,
                IEnumerable<EchoLaunchProjectAssetFact>> candidates = null)
        {
            List<EchoLaunchProjectAssetFact> allFacts =
                facts == null
                    ? new List<EchoLaunchProjectAssetFact>()
                    : new List<EchoLaunchProjectAssetFact>(facts);

            bool hasDestination = false;

            for (int index = 0; index < allFacts.Count; index++)
            {
                if (allFacts[index].Path == DestinationScenePath)
                {
                    hasDestination = true;
                    break;
                }
            }

            if (!hasDestination)
            {
                allFacts.Add(Scene(DestinationScenePath));
            }

            return new EchoLaunchProjectSnapshot(
                allFacts,
                buildScenes,
                templateAvailable,
                templateAvailable ? "template-guid" : string.Empty,
                candidates);
        }

        internal static EchoLaunchProjectAssetFact Folder(string path)
        {
            return new EchoLaunchProjectAssetFact(
                path,
                true,
                true,
                "folder-guid",
                EchoLaunchSetupAssetTypeNames.Folder);
        }

        internal static EchoLaunchProjectAssetFact Asset(
            string path,
            string typeName,
            int? schema = null)
        {
            return new EchoLaunchProjectAssetFact(
                path,
                true,
                false,
                "asset-guid",
                typeName,
                schema);
        }

        internal static EchoLaunchProjectAssetFact Scene(string path)
        {
            return Asset(path, EchoLaunchSetupAssetTypeNames.SceneAsset);
        }

        internal static EchoLaunchSetupOperation FindOperation(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                if (plan.Operations[index].Kind == kind)
                {
                    return plan.Operations[index];
                }
            }

            return null;
        }

        internal static bool HasDiagnostic(
            EchoLaunchSetupPlan plan,
            string code)
        {
            for (int index = 0; index < plan.Diagnostics.Count; index++)
            {
                if (plan.Diagnostics[index].Code == code)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
