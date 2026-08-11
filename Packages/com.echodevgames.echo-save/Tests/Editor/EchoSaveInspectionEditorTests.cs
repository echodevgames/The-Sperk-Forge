using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoSave.Editor;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveInspectionEditorTests
    {
        [Test]
        public void Refresh_NullConfiguration_FailsClosed()
        {
            using (EchoSaveInspectionService service =
                   new EchoSaveInspectionService())
            {
                EchoSaveBrowserRefreshResult result =
                    service.Refresh(
                        null);

                Assert.That(
                    result.Succeeded,
                    Is.False);

                Assert.That(
                    result.OpenResult,
                    Is.Not.Null);

                Assert.That(
                    result.OpenResult.Succeeded,
                    Is.False);
            }
        }

        [Test]
        public void Refresh_MissingRoot_IsEmptyAndZeroWrite()
        {
            string rootName =
                "EchoSaveBrowser_" +
                Guid.NewGuid().ToString("N");

            string root =
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

                using (EchoSaveInspectionService service =
                       new EchoSaveInspectionService())
                {
                    EchoSaveBrowserRefreshResult first =
                        service.Refresh(
                            configuration);

                    Assert.That(
                        first.Succeeded,
                        Is.True);

                    Assert.That(
                        first.OpenResult.RootPresent,
                        Is.False);

                    Assert.That(
                        first.CatalogResult.Snapshot.Count,
                        Is.EqualTo(0));

                    Assert.That(
                        first.MigrationGraph.DocumentCount,
                        Is.EqualTo(4));

                    Assert.That(
                        Directory.Exists(root),
                        Is.False);

                    EchoSaveBrowserRefreshResult second =
                        service.Refresh(
                            configuration);

                    Assert.That(
                        second.Succeeded,
                        Is.True);

                    Assert.That(
                        second.CatalogResult.Snapshot.Count,
                        Is.EqualTo(
                            first.CatalogResult.Snapshot.Count));

                    Assert.That(
                        second.MigrationGraph.EdgeCount,
                        Is.EqualTo(
                            first.MigrationGraph.EdgeCount));

                    Assert.That(
                        Directory.Exists(root),
                        Is.False);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);

                if (Directory.Exists(root))
                {
                    Directory.Delete(
                        root,
                        true);
                }
            }
        }

        [Test]
        public void InspectSlot_WithoutRefresh_ReturnsNull()
        {
            using (EchoSaveInspectionService service =
                   new EchoSaveInspectionService())
            {
                Assert.That(
                    service.InspectSlot(
                        SaveSlotId.NewId()),
                    Is.Null);
            }
        }
    }
}
