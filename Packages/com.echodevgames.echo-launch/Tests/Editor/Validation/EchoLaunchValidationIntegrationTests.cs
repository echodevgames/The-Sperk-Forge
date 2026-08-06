using System.Linq;
using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidationIntegrationTests
    {
        private const string MissingRoot =
            "Assets/__EchoLaunch_FL_M5_04_Missing";

        [Test]
        public void MissingFoundationIsBlockedWithoutChangingEditorState()
        {
            EditorBuildSettingsScene[] buildBefore =
                EditorBuildSettings.scenes;

            SceneSetup[] scenesBefore =
                EditorSceneManager.GetSceneManagerSetup();

            EchoLaunchValidationReport report =
                new EchoLaunchValidationService().Validate(
                    new EchoLaunchValidationRequest(
                        MissingRoot,
                        true));

            EditorBuildSettingsScene[] buildAfter =
                EditorBuildSettings.scenes;

            SceneSetup[] scenesAfter =
                EditorSceneManager.GetSceneManagerSetup();

            Assert.That(
                report.Health,
                Is.EqualTo(EchoLaunchProjectHealth.Blocked));

            Assert.That(
                EchoLaunchValidationTestFactory.HasCode(
                    report.Findings,
                    EchoLaunchValidationDiagnosticCodes.MissingBootScene),
                Is.True);

            Assert.That(
                buildAfter.Select(BuildSummary),
                Is.EqualTo(buildBefore.Select(BuildSummary)));

            Assert.That(
                scenesAfter.Select(SceneSummary),
                Is.EqualTo(scenesBefore.Select(SceneSummary)));
        }

        [Test]
        public void RepeatedMissingFoundationValidationIsDeterministic()
        {
            EchoLaunchValidationService service =
                new EchoLaunchValidationService();

            EchoLaunchValidationRequest request =
                new EchoLaunchValidationRequest(
                    MissingRoot,
                    true);

            EchoLaunchValidationReport first =
                service.Validate(request);

            EchoLaunchValidationReport second =
                service.Validate(request);

            Assert.That(
                second.EvidenceFingerprint,
                Is.EqualTo(first.EvidenceFingerprint));

            Assert.That(
                second.ReportFingerprint,
                Is.EqualTo(first.ReportFingerprint));

            Assert.That(
                EchoLaunchValidationTextFormatter.Format(second),
                Is.EqualTo(
                    EchoLaunchValidationTextFormatter.Format(first)));
        }

        private static string BuildSummary(
            EditorBuildSettingsScene scene)
        {
            return scene.path + "|" + scene.enabled;
        }

        private static string SceneSummary(
            SceneSetup setup)
        {
            return setup.path +
                   "|" + setup.isLoaded +
                   "|" + setup.isActive;
        }
    }
}
