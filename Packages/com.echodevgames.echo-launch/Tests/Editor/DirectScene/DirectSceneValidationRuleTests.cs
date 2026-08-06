//----- DirectSceneValidationRuleTests.cs START -----

using System;
using System.Linq;
using EchoDevGames.EchoLaunch.Editor.Validation;
using EchoDevGames.EchoLaunch.Tests.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.DirectScene
{
    public sealed class DirectSceneValidationRuleTests
    {
        private const string DirectConfigurationPath =
            "Assets/EchoDevGames/FirstLight/Configuration/DirectSceneConfiguration.asset";

        private const string DirectRootPrefabPath =
            "Assets/EchoDevGames/FirstLight/Prefabs/EchoLaunchRoot_Direct.prefab";

        private const string DirectConfigurationId =
            "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";

        [Test]
        public void ValidEditorOnlyHelperEmitsNo009()
        {
            EchoLaunchValidationEvidence evidence =
                AddDirectInitializer(
                    EchoLaunchValidationTestFactory.CreateHealthyEvidence(),
                    CreateValidDirectEvidence(
                        DirectSceneEntryPolicy.EditorOnly));

            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence),
                    EchoLaunchValidationDiagnosticCodes
                        .DirectSceneReleaseSafety),
                Is.False);
        }

        [Test]
        public void DevelopmentBuildOptInEmits009Warning()
        {
            EchoLaunchValidationEvidence evidence =
                AddDirectInitializer(
                    EchoLaunchValidationTestFactory.CreateHealthyEvidence(),
                    CreateValidDirectEvidence(
                        DirectSceneEntryPolicy
                            .EditorAndDevelopmentBuilds));

            EchoLaunchValidationFinding finding =
                EchoLaunchValidationRuleCatalog.Evaluate(evidence)
                    .Single(item =>
                        item.Code ==
                        EchoLaunchValidationDiagnosticCodes
                            .DirectSceneReleaseSafety);

            Assert.That(
                finding.Severity,
                Is.EqualTo(
                    EchoLaunchValidationSeverity.Warning));
        }

        [Test]
        public void DestinationMismatchEmits009Blocker()
        {
            EchoLaunchValidationDirectSceneEvidence invalid =
                CreateValidDirectEvidence(
                    DirectSceneEntryPolicy.EditorOnly,
                    destinationScenePath:
                        "Assets/SomeOtherScene.unity");

            EchoLaunchValidationFinding finding =
                EchoLaunchValidationRuleCatalog.Evaluate(
                        AddDirectInitializer(
                            EchoLaunchValidationTestFactory
                                .CreateHealthyEvidence(),
                            invalid))
                    .Single(item =>
                        item.Code ==
                        EchoLaunchValidationDiagnosticCodes
                            .DirectSceneReleaseSafety);

            Assert.That(
                finding.Severity,
                Is.EqualTo(
                    EchoLaunchValidationSeverity.Blocker));
        }

        [Test]
        public void HelperInCanonicalBootEmits009Blocker()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationSceneEvidence outdoors =
                source.FindScene("Assets/OutdoorsScene.unity");

            EchoLaunchValidationSceneEvidence boot =
                source.FindScene(source.Paths.BootScenePath);

            EchoLaunchValidationDirectSceneEvidence direct =
                CreateValidDirectEvidence(
                    DirectSceneEntryPolicy.EditorOnly,
                    containingScenePath: source.Paths.BootScenePath,
                    destinationScenePath: source.Paths.BootScenePath);

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    scenes: new[]
                    {
                        outdoors,
                        new EchoLaunchValidationSceneEvidence(
                            boot.Path,
                            boot.Exists,
                            boot.Inspected,
                            boot.Roots,
                            new[] { direct })
                    });

            Assert.That(
                EchoLaunchValidationRuleCatalog.Evaluate(changed)
                    .Any(item =>
                        item.Code ==
                            EchoLaunchValidationDiagnosticCodes
                                .DirectSceneReleaseSafety &&
                        item.Severity ==
                            EchoLaunchValidationSeverity.Blocker),
                Is.True);
        }

        [Test]
        public void DirectEvidenceChangesDeterministicFingerprint()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                AddDirectInitializer(
                    source,
                    CreateValidDirectEvidence(
                        DirectSceneEntryPolicy.EditorOnly));

            string first = changed.EvidenceFingerprint;
            string second = changed.EvidenceFingerprint;

            Assert.That(first, Is.EqualTo(second));
            Assert.That(
                first,
                Is.Not.EqualTo(source.EvidenceFingerprint));
        }

        private static EchoLaunchValidationEvidence AddDirectInitializer(
            EchoLaunchValidationEvidence source,
            EchoLaunchValidationDirectSceneEvidence direct)
        {
            EchoLaunchValidationSceneEvidence outdoors =
                source.FindScene("Assets/OutdoorsScene.unity");

            EchoLaunchValidationSceneEvidence boot =
                source.FindScene(source.Paths.BootScenePath);

            return EchoLaunchValidationTestFactory.Rebuild(
                source,
                scenes: new[]
                {
                    new EchoLaunchValidationSceneEvidence(
                        outdoors.Path,
                        outdoors.Exists,
                        outdoors.Inspected,
                        outdoors.Roots,
                        new[] { direct }),
                    boot
                });
        }

        private static EchoLaunchValidationDirectSceneEvidence
            CreateValidDirectEvidence(
                DirectSceneEntryPolicy policy,
                string containingScenePath =
                    "Assets/OutdoorsScene.unity",
                string destinationScenePath =
                    "Assets/OutdoorsScene.unity")
        {
            return new EchoLaunchValidationDirectSceneEvidence(
                containingScenePath,
                true,
                (int)policy,
                DirectConfigurationPath,
                typeof(DirectSceneConfiguration).FullName,
                DirectConfigurationId,
                DirectSceneConfiguration.CurrentSchemaVersion,
                DirectRootPrefabPath,
                1,
                1,
                true,
                (int)LaunchMode.DirectSceneDevelopment,
                "Assets/EchoDevGames/FirstLight/Configuration/EchoLaunchConfiguration.asset",
                EchoLaunchConfiguration.CurrentSchemaVersion,
                "Assets/EchoDevGames/FirstLight/Configuration/LaunchDestination.asset",
                LaunchDestination.CurrentSchemaVersion,
                destinationScenePath);
        }
    }
}

//----- DirectSceneValidationRuleTests.cs END -----
