using System;
using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationRuleTests
    {
        [Test]
        public void HealthyEvidenceHasNoFindings()
        {
            EchoLaunchValidationEvidence evidence =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            Assert.That(
                EchoLaunchValidationRuleCatalog.Evaluate(evidence),
                Is.Empty);
        }

        [Test]
        public void MissingBootEmits001()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    scenes: new[]
                    {
                        source.FindScene("Assets/OutdoorsScene.unity")
                    });

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.MissingBootScene);
        }

        [Test]
        public void DuplicateRootEmits002()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationSceneEvidence boot =
                source.FindScene(source.Paths.BootScenePath);

            EchoLaunchValidationRootEvidence extra =
                new EchoLaunchValidationRootEvidence(
                    source.Paths.ConfigurationAssetPath,
                    source.Paths.RootPrefabPath,
                    true,
                    true);

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    scenes: new[]
                    {
                        source.FindScene("Assets/OutdoorsScene.unity"),
                        new EchoLaunchValidationSceneEvidence(
                            boot.Path,
                            true,
                            true,
                            new[] { boot.Roots[0], extra })
                    });

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.DuplicateRoots);
        }

        [Test]
        public void MissingRootConfigurationEmits003()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationRootEvidence invalid =
                new EchoLaunchValidationRootEvidence(
                    string.Empty,
                    source.Paths.RootPrefabPath,
                    true,
                    true);

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    rootPrefab:
                        new EchoLaunchValidationRootPrefabEvidence(
                            source.Paths.RootPrefabPath,
                            true,
                            true,
                            new[] { invalid }));

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.RootConfiguration);
        }

        [Test]
        public void UnsupportedConfigurationEmits004()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    configuration:
                        new EchoLaunchValidationAssetEvidence(
                            source.Configuration.Path,
                            true,
                            typeof(EchoLaunchConfiguration).FullName,
                            EchoLaunchValidationTestFactory.ConfigurationId,
                            -1));

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.Configuration);
        }

        [Test]
        public void MissingDestinationBuildEntryEmits007()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    buildScenes: new[]
                    {
                        new EchoLaunchValidationBuildSceneEvidence(
                            source.Paths.BootScenePath,
                            true,
                            0)
                    });

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.Destination);
        }

        [Test]
        public void DisabledBootEmits008()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    buildScenes: new[]
                    {
                        new EchoLaunchValidationBuildSceneEvidence(
                            "Assets/OutdoorsScene.unity",
                            true,
                            0),
                        new EchoLaunchValidationBuildSceneEvidence(
                            source.Paths.BootScenePath,
                            false,
                            1)
                    });

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.BootBuildSettings);
        }

        [Test]
        public void HealthyEvidenceEmitsNoDirectSceneFinding()
        {
            EchoLaunchValidationEvidence evidence =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence),
                    EchoLaunchValidationDiagnosticCodes
                        .DirectSceneReleaseSafety),
                Is.False);
        }

        [Test]
        public void CollectionIssueEmits014()
        {
            EchoLaunchValidationEvidence source =
                EchoLaunchValidationTestFactory.CreateHealthyEvidence();

            EchoLaunchValidationEvidence changed =
                EchoLaunchValidationTestFactory.Rebuild(
                    source,
                    issues: new[]
                    {
                        "Scene inspection failed safely."
                    });

            AssertCode(
                changed,
                EchoLaunchValidationDiagnosticCodes.EvidenceUnavailable);
        }

        private static void AssertCode(
            EchoLaunchValidationEvidence evidence,
            string code)
        {
            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    EchoLaunchValidationRuleCatalog.Evaluate(evidence),
                    code),
                Is.True);
        }
    }
}
