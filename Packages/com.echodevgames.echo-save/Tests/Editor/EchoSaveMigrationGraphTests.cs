using System.Linq;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveMigrationGraphTests
    {
        private sealed class TestStep :
            ISavePackageDocumentMigrationStep
        {
            internal TestStep(
                string stepId,
                string documentKind,
                SavePackageDocumentVersion source,
                SavePackageDocumentVersion target)
            {
                StepId = stepId;
                DocumentKind = documentKind;
                SourceVersion = source;
                TargetVersion = target;
            }

            public string StepId { get; }

            public string DocumentKind { get; }

            public SavePackageDocumentVersion SourceVersion { get; }

            public SavePackageDocumentVersion TargetVersion { get; }

            public SavePackageDocumentMigrationStepResult Migrate(
                string serializedDocument) =>
                SavePackageDocumentMigrationStepResult.Success(
                    serializedDocument);
        }

        [Test]
        public void ProductionGraph_IsValidCurrentOnlyState()
        {
            SaveMigrationGraphSnapshot graph =
                SavePackageDocumentMigrationGraphBuilder.Build(
                    SavePackageDocumentMigrationRegistry.CreateProduction());

            Assert.That(
                graph.RegistryValid,
                Is.True);

            Assert.That(
                graph.DocumentCount,
                Is.EqualTo(4));

            Assert.That(
                graph.EdgeCount,
                Is.EqualTo(0));

            Assert.That(
                graph.Documents.All(
                    document =>
                        document.IsCurrentOnly),
                Is.True);
        }

        [Test]
        public void Graph_SingleStepToCurrent_ReportsReachablePath()
        {
            SavePackageDocumentVersion current =
                Current(
                    SaveDocumentKinds.Manifest);

            SavePackageDocumentVersion source =
                new SavePackageDocumentVersion(
                    current.Major == 0
                        ? 0
                        : current.Major - 1,
                    0,
                    0);

            if (source == current)
            {
                source =
                    new SavePackageDocumentVersion(
                        0,
                        0,
                        0);
            }

            SavePackageDocumentMigrationRegistry registry =
                new SavePackageDocumentMigrationRegistry(
                    new[]
                    {
                        new TestStep(
                            "manifest-to-current",
                            SaveDocumentKinds.Manifest,
                            source,
                            current)
                    });

            SaveMigrationGraphSnapshot graph =
                SavePackageDocumentMigrationGraphBuilder.Build(
                    registry);

            SaveMigrationGraphEdge edge =
                graph.Edges.Single();

            Assert.That(
                edge.ReachesCurrent,
                Is.True);

            Assert.That(
                edge.PathStepCount,
                Is.EqualTo(1));

            Assert.That(
                edge.SourceVersion,
                Is.EqualTo(
                    source.ToString()));

            Assert.That(
                edge.TargetVersion,
                Is.EqualTo(
                    current.ToString()));
        }

        [Test]
        public void Graph_IncompleteChain_ReportsMissingPath()
        {
            SavePackageDocumentVersion current =
                Current(
                    SaveDocumentKinds.Manifest);

            SavePackageDocumentVersion source =
                new SavePackageDocumentVersion(
                    0,
                    0,
                    0);

            SavePackageDocumentVersion middle =
                new SavePackageDocumentVersion(
                    0,
                    0,
                    1);

            if (current <= middle)
            {
                Assert.Pass(
                    "The current manifest version does not permit this historical-gap fixture.");
                return;
            }

            SavePackageDocumentMigrationRegistry registry =
                new SavePackageDocumentMigrationRegistry(
                    new[]
                    {
                        new TestStep(
                            "manifest-partial",
                            SaveDocumentKinds.Manifest,
                            source,
                            middle)
                    });

            SaveMigrationGraphSnapshot graph =
                SavePackageDocumentMigrationGraphBuilder.Build(
                    registry);

            SaveMigrationGraphEdge edge =
                graph.Edges.Single();

            Assert.That(
                edge.ReachesCurrent,
                Is.False);

            Assert.That(
                edge.DiagnosticCode,
                Is.Not.Empty);
        }

        [Test]
        public void InspectionEntries_AreDeterministicallySorted()
        {
            SavePackageDocumentVersion manifestCurrent =
                Current(
                    SaveDocumentKinds.Manifest);

            SavePackageDocumentVersion headCurrent =
                Current(
                    SaveDocumentKinds.HeadPointer);

            SavePackageDocumentVersion source =
                new SavePackageDocumentVersion(
                    0,
                    0,
                    0);

            SavePackageDocumentMigrationRegistry registry =
                new SavePackageDocumentMigrationRegistry(
                    new ISavePackageDocumentMigrationStep[]
                    {
                        new TestStep(
                            "z-manifest",
                            SaveDocumentKinds.Manifest,
                            source,
                            manifestCurrent),
                        new TestStep(
                            "a-head",
                            SaveDocumentKinds.HeadPointer,
                            source,
                            headCurrent)
                    });

            SavePackageDocumentMigrationInspectionEntry[] entries =
                registry.CreateInspectionEntries();

            Assert.That(
                entries.Length,
                Is.EqualTo(2));

            Assert.That(
                string.CompareOrdinal(
                    entries[0].DocumentKind,
                    entries[1].DocumentKind),
                Is.LessThanOrEqualTo(0));
        }

        private static SavePackageDocumentVersion Current(
            string documentKind)
        {
            Assert.That(
                SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion current),
                Is.True);

            return current;
        }
    }
}
