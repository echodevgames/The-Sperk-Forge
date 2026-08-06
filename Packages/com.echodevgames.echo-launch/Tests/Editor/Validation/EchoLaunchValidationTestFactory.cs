using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using EchoDevGames.EchoLaunch.Editor.Validation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    internal static class EchoLaunchValidationTestFactory
    {
        internal const string ConfigurationId =
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        internal const string SequenceId =
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        internal static EchoLaunchValidationEvidence CreateHealthyEvidence()
        {
            EchoLaunchValidationRequest request =
                EchoLaunchValidationRequest.CreateDefault();

            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchValidationRootEvidence prefabRoot =
                new EchoLaunchValidationRootEvidence(
                    paths.ConfigurationAssetPath,
                    paths.RootPrefabPath,
                    true,
                    true);

            EchoLaunchValidationRootEvidence sceneRoot =
                new EchoLaunchValidationRootEvidence(
                    paths.ConfigurationAssetPath,
                    paths.RootPrefabPath,
                    true,
                    true);

            return new EchoLaunchValidationEvidence(
                request,
                paths,
                true,
                new EchoLaunchValidationAssetEvidence(
                    paths.ConfigurationAssetPath,
                    true,
                    typeof(EchoLaunchConfiguration).FullName,
                    ConfigurationId,
                    EchoLaunchConfiguration.CurrentSchemaVersion),
                new EchoLaunchValidationAssetEvidence(
                    paths.StartupSequenceAssetPath,
                    true,
                    typeof(StartupSequence).FullName,
                    SequenceId,
                    StartupSequence.CurrentSchemaVersion),
                new EchoLaunchValidationAssetEvidence(
                    paths.LaunchDestinationAssetPath,
                    true,
                    typeof(LaunchDestination).FullName,
                    "cccccccccccccccccccccccccccccccc",
                    LaunchDestination.CurrentSchemaVersion),
                new EchoLaunchValidationAssetEvidence(
                    paths.SplashSequenceAssetPath,
                    true,
                    typeof(SplashSequence).FullName,
                    "dddddddddddddddddddddddddddddddd",
                    SplashSequence.CurrentSchemaVersion),
                new EchoLaunchValidationRootPrefabEvidence(
                    paths.RootPrefabPath,
                    true,
                    true,
                    new[] { prefabRoot }),
                paths.StartupSequenceAssetPath,
                paths.LaunchDestinationAssetPath,
                string.Empty,
                Array.Empty<EchoLaunchValidationSequenceEntryEvidence>(),
                "Assets/OutdoorsScene.unity",
                "Outdoors",
                Array.Empty<EchoLaunchValidationSplashEntryEvidence>(),
                new[]
                {
                    new EchoLaunchValidationBuildSceneEvidence(
                        "Assets/OutdoorsScene.unity",
                        true,
                        0),
                    new EchoLaunchValidationBuildSceneEvidence(
                        paths.BootScenePath,
                        true,
                        1)
                },
                new[]
                {
                    new EchoLaunchValidationSceneEvidence(
                        "Assets/OutdoorsScene.unity",
                        true,
                        true,
                        Array.Empty<EchoLaunchValidationRootEvidence>()),
                    new EchoLaunchValidationSceneEvidence(
                        paths.BootScenePath,
                        true,
                        true,
                        new[] { sceneRoot })
                },
                Array.Empty<string>());
        }

        internal static EchoLaunchValidationEvidence Rebuild(
            EchoLaunchValidationEvidence source,
            EchoLaunchValidationAssetEvidence configuration = null,
            EchoLaunchValidationRootPrefabEvidence rootPrefab = null,
            IEnumerable<EchoLaunchValidationBuildSceneEvidence> buildScenes = null,
            IEnumerable<EchoLaunchValidationSceneEvidence> scenes = null,
            IEnumerable<string> issues = null)
        {
            return new EchoLaunchValidationEvidence(
                source.Request,
                source.Paths,
                source.PackageTemplateAvailable,
                configuration ?? source.Configuration,
                source.StartupSequence,
                source.Destination,
                source.SplashSequence,
                rootPrefab ?? source.RootPrefab,
                source.ConfigurationStartupSequencePath,
                source.ConfigurationDestinationPath,
                source.ConfigurationSplashPath,
                source.SequenceEntries,
                source.DestinationScenePath,
                source.DestinationDisplayName,
                source.SplashEntries,
                buildScenes ?? source.BuildSettingsScenes,
                scenes ?? source.SceneEvidence,
                issues ?? source.CollectionIssues);
        }

        internal static bool HasCode(
            IReadOnlyList<EchoLaunchValidationFinding> findings,
            string code)
        {
            for (int index = 0; index < findings.Count; index++)
            {
                if (string.Equals(
                        findings[index].Code,
                        code,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
