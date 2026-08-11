using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoSave.Editor;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveTestDataGeneratorM504Tests
    {
        [Test]
        public void Preview_RejectsUnboundedCounts()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string sandbox =
                Sandbox();

            try
            {
                EchoSaveTestDataGeneratorService service =
                    new EchoSaveTestDataGeneratorService();

                EchoSaveTestDataPlan plan =
                    service.Preview(
                        configuration,
                        sandbox,
                        new EchoSaveTestDataRequest(
                            EchoSaveTestDataGeneratorService.MaximumSlots + 1,
                            1,
                            0,
                            1));

                Assert.That(
                    plan.Succeeded,
                    Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Generate_CreatesBoundedDeterministicOwnedFixture_AndCleanupVerifiesAbsence()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string sandbox =
                Sandbox();

            EchoSaveTestDataGeneratorService service =
                new EchoSaveTestDataGeneratorService();

            try
            {
                EchoSaveTestDataRequest request =
                    new EchoSaveTestDataRequest(
                        2,
                        2,
                        32,
                        504);

                EchoSaveTestDataPlan preview =
                    service.Preview(
                        configuration,
                        sandbox,
                        request);

                Assert.That(
                    preview.Succeeded,
                    Is.True,
                    preview.Message);

                Assert.That(
                    Directory.Exists(
                        sandbox),
                    Is.False,
                    "Preview must perform zero writes.");

                EchoSaveToolingOperationResult generated =
                    service.Generate(
                        configuration,
                        sandbox,
                        request);

                Assert.That(
                    generated.Succeeded,
                    Is.True,
                    generated.Message);

                Assert.That(
                    EchoSaveTestDataGeneratorService
                        .IsOwnedSandbox(
                            sandbox),
                    Is.True);

                string[] slots =
                    Directory.GetDirectories(
                        Path.Combine(
                            sandbox,
                            "slots"));

                Assert.That(
                    slots.Length,
                    Is.EqualTo(2));

                foreach (string slot in slots)
                {
                    string[] generations =
                        Directory.GetDirectories(
                            Path.Combine(
                                slot,
                                "generations"));

                    Assert.That(
                        generations.Length,
                        Is.EqualTo(2));
                }

                EchoSaveToolingOperationResult cleanup =
                    service.Cleanup(
                        configuration,
                        sandbox);

                Assert.That(
                    cleanup.Succeeded,
                    Is.True,
                    cleanup.Message);

                Assert.That(
                    Directory.Exists(
                        sandbox),
                    Is.False);
            }
            finally
            {
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
                "EchoSave-M504-Test-" +
                Guid.NewGuid().ToString("N"));
        }
    }
}
