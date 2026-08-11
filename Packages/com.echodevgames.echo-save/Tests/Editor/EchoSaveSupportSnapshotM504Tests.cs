using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoSave.Editor;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveSupportSnapshotM504Tests
    {
        [Test]
        public void Build_RedactsRootAndRawSlotIdentity_AndIsPayloadFree()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            string rawSlot =
                "11111111-2222-3333-4444-555555555555";

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    "EchoSave");

                EchoSaveSupportSnapshotService service =
                    new EchoSaveSupportSnapshotService();

                EchoSaveSupportSnapshotResult result =
                    service.Build(
                        configuration,
                        SaveSlotCatalogSnapshot.Empty,
                        null,
                        rawSlot);

                Assert.That(
                    result.Succeeded,
                    Is.True,
                    result.Message);

                Assert.That(
                    result.Json,
                    Does.Not.Contain(
                        rawSlot));

                Assert.That(
                    result.Json,
                    Does.Not.Contain(
                        Application.persistentDataPath));

                Assert.That(
                    result.Json,
                    Does.Not.Contain(
                        "PAYLOAD_SECRET_MARKER"));

                Assert.That(
                    result.Json.Length,
                    Is.LessThan(32768));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }

        [Test]
        public void Build_IsDeterministicForSamePayloadFreeInputs()
        {
            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    "EchoSave");

                EchoSaveSupportSnapshotService service =
                    new EchoSaveSupportSnapshotService();

                EchoSaveSupportSnapshotResult first =
                    service.Build(
                        configuration,
                        SaveSlotCatalogSnapshot.Empty,
                        null,
                        string.Empty);

                EchoSaveSupportSnapshotResult second =
                    service.Build(
                        configuration,
                        SaveSlotCatalogSnapshot.Empty,
                        null,
                        string.Empty);

                Assert.That(
                    second.Json,
                    Is.EqualTo(
                        first.Json));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }
        }
    }
}
