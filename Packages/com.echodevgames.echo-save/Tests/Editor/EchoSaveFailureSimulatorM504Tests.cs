using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoSave.Editor;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveFailureSimulatorM504Tests
    {
        [Test]
        public void Preview_PerformsZeroWrites_AndApplyMutatesOnlyOwnedSandboxTarget()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string sandbox =
                Sandbox();

            EchoSaveTestDataGeneratorService generator =
                new EchoSaveTestDataGeneratorService();

            EchoSaveFailureSimulatorService simulator =
                new EchoSaveFailureSimulatorService();

            try
            {
                EchoSaveToolingOperationResult generated =
                    generator.Generate(
                        configuration,
                        sandbox,
                        new EchoSaveTestDataRequest(
                            1,
                            1,
                            64,
                            504));

                Assert.That(
                    generated.Succeeded,
                    Is.True,
                    generated.Message);

                EchoSaveFailureSimulationPlan preview =
                    simulator.Preview(
                        configuration,
                        sandbox,
                        EchoSaveFailureScenario.DeleteManifest);

                Assert.That(
                    preview.Succeeded,
                    Is.True,
                    preview.Message);

                string target =
                    Path.Combine(
                        sandbox,
                        preview.TargetRelativePath);

                Assert.That(
                    File.Exists(
                        target),
                    Is.True,
                    "Preview must not delete the manifest.");

                EchoSaveToolingOperationResult applied =
                    simulator.Apply(
                        configuration,
                        preview);

                Assert.That(
                    applied.Succeeded,
                    Is.True,
                    applied.Message);

                Assert.That(
                    File.Exists(
                        target),
                    Is.False);

                Assert.That(
                    Directory.Exists(
                        sandbox),
                    Is.True);
            }
            finally
            {
                generator.Cleanup(
                    configuration,
                    sandbox);

                if (Directory.Exists(
                        sandbox))
                {
                    Directory.Delete(
                        sandbox,
                        true);
                }

                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Apply_RefusesStalePreview()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string sandbox =
                Sandbox();

            EchoSaveTestDataGeneratorService generator =
                new EchoSaveTestDataGeneratorService();

            EchoSaveFailureSimulatorService simulator =
                new EchoSaveFailureSimulatorService();

            try
            {
                Assert.That(
                    generator.Generate(
                            configuration,
                            sandbox,
                            new EchoSaveTestDataRequest(
                                1,
                                1,
                                0,
                                9))
                        .Succeeded,
                    Is.True);

                EchoSaveFailureSimulationPlan preview =
                    simulator.Preview(
                        configuration,
                        sandbox,
                        EchoSaveFailureScenario.TruncateManifest);

                Assert.That(
                    preview.Succeeded,
                    Is.True);

                File.AppendAllText(
                    Path.Combine(
                        sandbox,
                        preview.TargetRelativePath),
                    "\nchanged");

                EchoSaveToolingOperationResult result =
                    simulator.Apply(
                        configuration,
                        preview);

                Assert.That(
                    result.Succeeded,
                    Is.False);

                Assert.That(
                    result.DiagnosticCode,
                    Is.EqualTo(
                        "M504-SIM-STALE"));
            }
            finally
            {
                generator.Cleanup(
                    configuration,
                    sandbox);

                if (Directory.Exists(
                        sandbox))
                {
                    Directory.Delete(
                        sandbox,
                        true);
                }

                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        private static EchoSaveConfiguration Configuration()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration.CurrentSchemaVersion,
                "EchoSave");

            return configuration;
        }

        private static string Sandbox()
        {
            return Path.Combine(
                Application.persistentDataPath,
                "EchoSave-M504-Sim-" +
                Guid.NewGuid().ToString("N"));
        }
    }
}
