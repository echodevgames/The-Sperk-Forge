using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveInspectionSessionTests
    {
        [Test]
        public void TryOpen_MissingRoot_DoesNotCreateDirectory()
        {
            string rootName =
                "EchoSaveInspection_" +
                Guid.NewGuid().ToString("N");

            string absoluteRoot =
                Path.Combine(
                    Application.persistentDataPath,
                    rootName);

            if (Directory.Exists(
                    absoluteRoot))
            {
                Directory.Delete(
                    absoluteRoot,
                    true);
            }

            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    rootName);

                EchoSaveInspectionOpenResult open =
                    EchoSaveInspectionSession.TryOpen(
                        configuration,
                        out EchoSaveInspectionSession session);

                Assert.That(
                    open.Succeeded,
                    Is.True,
                    open.Message);

                Assert.That(
                    open.RootPresent,
                    Is.False);

                Assert.That(
                    Directory.Exists(
                        absoluteRoot),
                    Is.False);

                using (session)
                {
                    SaveSlotCatalogRefreshResult refresh =
                        session.RefreshCatalog();

                    Assert.That(
                        refresh.Succeeded,
                        Is.True);

                    Assert.That(
                        refresh.Snapshot.Count,
                        Is.EqualTo(0));

                    SaveMigrationGraphSnapshot graph =
                        session.MigrationGraph;

                    Assert.That(
                        graph.RegistryValid,
                        Is.True);

                    Assert.That(
                        graph.DocumentCount,
                        Is.EqualTo(4));

                    Assert.That(
                        graph.EdgeCount,
                        Is.EqualTo(0));
                }

                Assert.That(
                    Directory.Exists(
                        absoluteRoot),
                    Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);

                if (Directory.Exists(
                        absoluteRoot))
                {
                    Directory.Delete(
                        absoluteRoot,
                        true);
                }
            }
        }

        [Test]
        public void TryOpen_NullConfiguration_FailsClosed()
        {
            EchoSaveInspectionOpenResult open =
                EchoSaveInspectionSession.TryOpen(
                    null,
                    out EchoSaveInspectionSession session);

            Assert.That(
                open.Succeeded,
                Is.False);

            Assert.That(
                session,
                Is.Null);

            Assert.That(
                open.DiagnosticCode,
                Is.Not.Empty);
        }

        [Test]
        public void InitializeReadOnly_MissingRoot_ReturnsNotFoundWithoutCreatingIt()
        {
            string root =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave_ReadOnly_" +
                    Guid.NewGuid().ToString("N"));

            if (Directory.Exists(root))
            {
                Directory.Delete(
                    root,
                    true);
            }

            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    root);

            SaveStorageResult result =
                backend.InitializeReadOnly();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveStorageStatus.NotFound));

            Assert.That(
                Directory.Exists(root),
                Is.False);
        }

        [Test]
        public void InspectGenerations_MissingRoot_ReturnsEmptyWithoutCreatingIt()
        {
            string rootName =
                "EchoSaveInspection_" +
                Guid.NewGuid().ToString("N");

            string absoluteRoot =
                Path.Combine(
                    Application.persistentDataPath,
                    rootName);

            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    rootName);

                EchoSaveInspectionOpenResult open =
                    EchoSaveInspectionSession.TryOpen(
                        configuration,
                        out EchoSaveInspectionSession session);

                Assert.That(
                    open.Succeeded,
                    Is.True);

                using (session)
                {
                    SaveGenerationInspectionSnapshot snapshot =
                        session.InspectGenerations(
                            SaveSlotId.NewId());

                    Assert.That(
                        snapshot.Succeeded,
                        Is.True);

                    Assert.That(
                        snapshot.Status,
                        Is.EqualTo(
                            SaveGenerationInspectionSnapshotStatus.RootMissing));

                    Assert.That(
                        snapshot.Count,
                        Is.EqualTo(0));
                }

                Assert.That(
                    Directory.Exists(
                        absoluteRoot),
                    Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);

                if (Directory.Exists(
                        absoluteRoot))
                {
                    Directory.Delete(
                        absoluteRoot,
                        true);
                }
            }
        }
    }
}
