using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// One package-owned migration edge as read-only inspection data.
    /// </summary>
    public sealed class SaveMigrationGraphEdge
    {
        internal SaveMigrationGraphEdge(
            string stepId,
            string documentKind,
            string sourceVersion,
            string targetVersion,
            bool reachesCurrent,
            int pathStepCount,
            string diagnosticCode,
            string message)
        {
            StepId = stepId ?? string.Empty;
            DocumentKind = documentKind ?? string.Empty;
            SourceVersion = sourceVersion ?? string.Empty;
            TargetVersion = targetVersion ?? string.Empty;
            ReachesCurrent = reachesCurrent;
            PathStepCount = pathStepCount;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string StepId { get; }

        public string DocumentKind { get; }

        public string SourceVersion { get; }

        public string TargetVersion { get; }

        public bool ReachesCurrent { get; }

        public int PathStepCount { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }

    /// <summary>
    /// Current package-document authority and registered-edge count.
    /// </summary>
    public sealed class SaveMigrationGraphDocument
    {
        internal SaveMigrationGraphDocument(
            string documentKind,
            string currentVersion,
            int registeredEdgeCount)
        {
            DocumentKind = documentKind ?? string.Empty;
            CurrentVersion = currentVersion ?? string.Empty;
            RegisteredEdgeCount = registeredEdgeCount;
        }

        public string DocumentKind { get; }

        public string CurrentVersion { get; }

        public int RegisteredEdgeCount { get; }

        public bool IsCurrentOnly =>
            RegisteredEdgeCount == 0;
    }

    /// <summary>
    /// Immutable copied description of the package-owned migration registry.
    /// </summary>
    public sealed class SaveMigrationGraphSnapshot
    {
        private readonly ReadOnlyCollection<SaveMigrationGraphDocument> documents;
        private readonly ReadOnlyCollection<SaveMigrationGraphEdge> edges;

        internal SaveMigrationGraphSnapshot(
            bool registryValid,
            SaveMigrationGraphDocument[] documents,
            SaveMigrationGraphEdge[] edges,
            string diagnosticCode,
            string message)
        {
            RegistryValid = registryValid;

            SaveMigrationGraphDocument[] documentCopy =
                documents == null
                    ? Array.Empty<SaveMigrationGraphDocument>()
                    : (SaveMigrationGraphDocument[])documents.Clone();

            SaveMigrationGraphEdge[] edgeCopy =
                edges == null
                    ? Array.Empty<SaveMigrationGraphEdge>()
                    : (SaveMigrationGraphEdge[])edges.Clone();

            this.documents = Array.AsReadOnly(documentCopy);
            this.edges = Array.AsReadOnly(edgeCopy);
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool RegistryValid { get; }

        public IReadOnlyList<SaveMigrationGraphDocument> Documents =>
            documents;

        public IReadOnlyList<SaveMigrationGraphEdge> Edges =>
            edges;

        public int DocumentCount =>
            documents.Count;

        public int EdgeCount =>
            edges.Count;

        public string DiagnosticCode { get; }

        public string Message { get; }
    }
}
