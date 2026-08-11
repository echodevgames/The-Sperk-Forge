using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoSave.Editor;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveSandboxPathGuardTests
    {
        [Test]
        public void Evaluate_ProductionRoot_IsRefused()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string persistent =
                TempRoot();

            try
            {
                string production =
                    Path.Combine(
                        persistent,
                        "EchoSave");

                EchoSaveSandboxPathResult result =
                    EchoSaveSandboxPathGuard.Evaluate(
                        configuration,
                        production,
                        persistent);

                Assert.That(
                    result.Succeeded,
                    Is.False);

                Assert.That(
                    result.DiagnosticCode,
                    Is.EqualTo(
                        "M504-SANDBOX-COLLISION"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Evaluate_NestedInsideProduction_IsRefused()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string persistent =
                TempRoot();

            try
            {
                string nested =
                    Path.Combine(
                        persistent,
                        "EchoSave",
                        "Sandbox");

                Assert.That(
                    EchoSaveSandboxPathGuard
                        .Evaluate(
                            configuration,
                            nested,
                            persistent)
                        .Succeeded,
                    Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Evaluate_ContainsProduction_IsRefused()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string persistent =
                Path.Combine(
                    TempRoot(),
                    "Persistent");

            try
            {
                string sandbox =
                    Directory.GetParent(
                        persistent).FullName;

                Assert.That(
                    EchoSaveSandboxPathGuard
                        .Evaluate(
                            configuration,
                            sandbox,
                            persistent)
                        .Succeeded,
                    Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Evaluate_SiblingSandbox_IsAccepted()
        {
            EchoSaveConfiguration configuration =
                Configuration();

            string persistent =
                TempRoot();

            try
            {
                string sandbox =
                    Path.Combine(
                        persistent,
                        "EchoSave-M504");

                EchoSaveSandboxPathResult result =
                    EchoSaveSandboxPathGuard.Evaluate(
                        configuration,
                        sandbox,
                        persistent);

                Assert.That(
                    result.Succeeded,
                    Is.True,
                    result.Message);
            }
            finally
            {
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

        private static string TempRoot()
        {
            return Path.Combine(
                Path.GetTempPath(),
                "EchoSaveM504_" +
                Guid.NewGuid().ToString("N"));
        }
    }
}
