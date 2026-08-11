using System;
using System.Text;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    internal enum SavePackageDocumentVersionProbeStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        InputTooLarge = 2,
        MalformedDocument = 3,
        UnsupportedDocumentKind = 4,
        InvalidVersion = 5
    }

    internal readonly struct SavePackageDocumentVersionProbeResult
    {
        internal SavePackageDocumentVersionProbeResult(
            SavePackageDocumentVersionProbeStatus status,
            string documentKind,
            SavePackageDocumentVersion version,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DocumentKind = documentKind ?? string.Empty;
            Version = version;
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
        }

        internal SavePackageDocumentVersionProbeStatus Status { get; }

        internal string DocumentKind { get; }

        internal SavePackageDocumentVersion Version { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status == SavePackageDocumentVersionProbeStatus.Succeeded;
    }

    /// <summary>
    /// Bounded package-header probe that does not require exact-current DTO validation.
    /// </summary>
    internal static class SavePackageDocumentVersionProbe
    {
        internal const int MaximumSerializedDocumentBytes =
            64 * 1024 * 1024;

        [Serializable]
        private sealed class ProbeDocument
        {
            public string documentKind;
            public int formatMajor;
            public int formatMinor;
            public int formatRevision;
        }

        internal static SavePackageDocumentVersionProbeResult Probe(
            string serializedDocument)
        {
            if (serializedDocument == null)
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.InvalidRequest,
                    "Chronicle package-document version probing requires serialized text.");
            }

            int byteCount;
            try
            {
                byteCount =
                    Encoding.UTF8.GetByteCount(
                        serializedDocument);
            }
            catch (ArgumentException exception)
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.MalformedDocument,
                    "Chronicle package-document version probing could not measure UTF-8 input. " +
                    exception.Message);
            }

            if (byteCount <= 0)
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.MalformedDocument,
                    "Chronicle package-document version probing rejects empty input.");
            }

            if (byteCount > MaximumSerializedDocumentBytes)
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.InputTooLarge,
                    "Chronicle package-document version probing rejects input beyond its bounded in-memory size.");
            }

            if (!HasObjectShape(serializedDocument) ||
                !HasRequiredFieldMarker(
                    serializedDocument,
                    "documentKind") ||
                !HasRequiredFieldMarker(
                    serializedDocument,
                    "formatMajor") ||
                !HasRequiredFieldMarker(
                    serializedDocument,
                    "formatMinor") ||
                !HasRequiredFieldMarker(
                    serializedDocument,
                    "formatRevision"))
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.MalformedDocument,
                    "Chronicle package-document version probing requires one object with explicit kind and exact version fields.");
            }

            ProbeDocument probe;
            try
            {
                probe =
                    JsonUtility.FromJson<ProbeDocument>(
                        serializedDocument);
            }
            catch (ArgumentException exception)
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.MalformedDocument,
                    "Chronicle package-document version probing rejected malformed serialized input. " +
                    exception.Message);
            }

            if (probe == null ||
                string.IsNullOrEmpty(
                    probe.documentKind))
            {
                return Failure(
                    SavePackageDocumentVersionProbeStatus.MalformedDocument,
                    "Chronicle package-document version probing could not read a document kind.");
            }

            if (!SavePackageDocumentVersionAuthority.TryGetCurrent(
                    probe.documentKind,
                    out _))
            {
                return new SavePackageDocumentVersionProbeResult(
                    SavePackageDocumentVersionProbeStatus.UnsupportedDocumentKind,
                    probe.documentKind,
                    default,
                    SavePackageDocumentMigrationDiagnosticCodes.ProbeFailed,
                    "Chronicle package-document version probing found an unsupported document kind.");
            }

            if (!SavePackageDocumentVersion.TryCreate(
                    probe.formatMajor,
                    probe.formatMinor,
                    probe.formatRevision,
                    out SavePackageDocumentVersion version))
            {
                return new SavePackageDocumentVersionProbeResult(
                    SavePackageDocumentVersionProbeStatus.InvalidVersion,
                    probe.documentKind,
                    default,
                    SavePackageDocumentMigrationDiagnosticCodes.ProbeFailed,
                    "Chronicle package-document version probing found invalid or unbounded version components.");
            }

            return new SavePackageDocumentVersionProbeResult(
                SavePackageDocumentVersionProbeStatus.Succeeded,
                probe.documentKind,
                version,
                string.Empty,
                "Chronicle package-document kind and exact version were probed successfully.");
        }

        private static bool HasObjectShape(
            string serializedDocument)
        {
            int first = 0;
            while (first < serializedDocument.Length &&
                   char.IsWhiteSpace(
                       serializedDocument[first]))
            {
                first++;
            }

            int last =
                serializedDocument.Length - 1;
            while (last >= first &&
                   char.IsWhiteSpace(
                       serializedDocument[last]))
            {
                last--;
            }

            return
                first <= last &&
                serializedDocument[first] == '{' &&
                serializedDocument[last] == '}';
        }

        private static bool HasRequiredFieldMarker(
            string serializedDocument,
            string fieldName) =>
            serializedDocument.IndexOf(
                "\"" + fieldName + "\"",
                StringComparison.Ordinal) >= 0;

        private static SavePackageDocumentVersionProbeResult Failure(
            SavePackageDocumentVersionProbeStatus status,
            string message) =>
            new SavePackageDocumentVersionProbeResult(
                status,
                string.Empty,
                default,
                SavePackageDocumentMigrationDiagnosticCodes.ProbeFailed,
                message);
    }
}
