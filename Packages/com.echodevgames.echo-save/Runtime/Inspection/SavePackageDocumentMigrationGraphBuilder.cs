using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal readonly struct SavePackageDocumentMigrationInspectionEntry
    {
        internal SavePackageDocumentMigrationInspectionEntry(
            string stepId,
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion targetVersion)
        {
            StepId = stepId ?? string.Empty;
            DocumentKind = documentKind ?? string.Empty;
            SourceVersion = sourceVersion;
            TargetVersion = targetVersion;
        }

        internal string StepId { get; }

        internal string DocumentKind { get; }

        internal SavePackageDocumentVersion SourceVersion { get; }

        internal SavePackageDocumentVersion TargetVersion { get; }
    }

    internal static class SavePackageDocumentMigrationGraphBuilder
    {
        private static readonly string[] SupportedDocumentKinds =
        {
            SaveDocumentKinds.Envelope,
            SaveDocumentKinds.Manifest,
            SaveDocumentKinds.Payload,
            SaveDocumentKinds.HeadPointer
        };

        internal static SaveMigrationGraphSnapshot Build(
            SavePackageDocumentMigrationRegistry registry)
        {
            if (registry == null)
            {
                return new SaveMigrationGraphSnapshot(
                    false,
                    Array.Empty<SaveMigrationGraphDocument>(),
                    Array.Empty<SaveMigrationGraphEdge>(),
                    SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid,
                    "The Chronicle migration graph requires a package-owned registry.");
            }

            SavePackageDocumentMigrationInspectionEntry[] registered =
                registry.CreateInspectionEntries();

            List<SaveMigrationGraphDocument> documents =
                new List<SaveMigrationGraphDocument>(
                    SupportedDocumentKinds.Length);

            List<SaveMigrationGraphEdge> edges =
                new List<SaveMigrationGraphEdge>(
                    registered.Length);

            for (int kindIndex = 0;
                 kindIndex < SupportedDocumentKinds.Length;
                 kindIndex++)
            {
                string documentKind =
                    SupportedDocumentKinds[kindIndex];

                SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion currentVersion);

                int edgeCount = 0;

                for (int entryIndex = 0;
                     entryIndex < registered.Length;
                     entryIndex++)
                {
                    if (string.Equals(
                            registered[entryIndex].DocumentKind,
                            documentKind,
                            StringComparison.Ordinal))
                    {
                        edgeCount++;
                    }
                }

                documents.Add(
                    new SaveMigrationGraphDocument(
                        documentKind,
                        currentVersion.ToString(),
                        edgeCount));
            }

            for (int i = 0;
                 i < registered.Length;
                 i++)
            {
                SavePackageDocumentMigrationInspectionEntry entry =
                    registered[i];

                if (!SavePackageDocumentVersionAuthority.TryGetCurrent(
                        entry.DocumentKind,
                        out SavePackageDocumentVersion current))
                {
                    edges.Add(
                        new SaveMigrationGraphEdge(
                            entry.StepId,
                            entry.DocumentKind,
                            entry.SourceVersion.ToString(),
                            entry.TargetVersion.ToString(),
                            false,
                            0,
                            SavePackageDocumentMigrationDiagnosticCodes.InvalidRequest,
                            "The Chronicle migration edge uses an unsupported document kind."));
                    continue;
                }

                SavePackageDocumentMigrationPlanResult planResult =
                    registry.TryBuildPlan(
                        entry.DocumentKind,
                        entry.SourceVersion,
                        current,
                        SavePackageDocumentMigrationRegistry.DefaultMaximumPlanSteps,
                        out SavePackageDocumentMigrationPlan plan);

                edges.Add(
                    new SaveMigrationGraphEdge(
                        entry.StepId,
                        entry.DocumentKind,
                        entry.SourceVersion.ToString(),
                        entry.TargetVersion.ToString(),
                        planResult.Succeeded,
                        plan == null
                            ? 0
                            : plan.Count,
                        planResult.DiagnosticCode,
                        planResult.Message));
            }

            return new SaveMigrationGraphSnapshot(
                registry.IsValid,
                documents.ToArray(),
                edges.ToArray(),
                registry.DiagnosticCode,
                registry.Count == 0 && registry.IsValid
                    ? "No package-document migration edges are registered. Chronicle production formats are current-only."
                    : registry.Message);
        }
    }
}
