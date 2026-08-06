using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidationReport
    {
        internal const int CurrentSchemaVersion = 1;

        private readonly ReadOnlyCollection<EchoLaunchValidationFinding>
            findings;

        internal EchoLaunchValidationReport(
            EchoLaunchValidationRequest request,
            string requestFingerprint,
            string evidenceFingerprint,
            IEnumerable<EchoLaunchValidationFinding> findings)
        {
            Request =
                request ??
                throw new ArgumentNullException(nameof(request));

            RequestFingerprint = requestFingerprint ?? string.Empty;
            EvidenceFingerprint = evidenceFingerprint ?? string.Empty;

            List<EchoLaunchValidationFinding> copied =
                findings == null
                    ? new List<EchoLaunchValidationFinding>()
                    : new List<EchoLaunchValidationFinding>(findings);

            copied.Sort(CompareFindings);
            this.findings =
                new ReadOnlyCollection<EchoLaunchValidationFinding>(copied);

            CountFindings(
                copied,
                out int information,
                out int warnings,
                out int errors,
                out int blockers);

            InformationCount = information;
            WarningCount = warnings;
            ErrorCount = errors;
            BlockerCount = blockers;
            Health = DeriveHealth(warnings, errors, blockers);

            ReportFingerprint =
                EchoLaunchValidationFingerprint.ForReportCore(
                    CurrentSchemaVersion,
                    Request,
                    RequestFingerprint,
                    EvidenceFingerprint,
                    Health,
                    copied);
        }

        internal int SchemaVersion => CurrentSchemaVersion;
        internal EchoLaunchValidationRequest Request { get; }
        internal string RequestFingerprint { get; }
        internal string EvidenceFingerprint { get; }
        internal string ReportFingerprint { get; }
        internal EchoLaunchProjectHealth Health { get; }
        internal int InformationCount { get; }
        internal int WarningCount { get; }
        internal int ErrorCount { get; }
        internal int BlockerCount { get; }
        internal int FindingCount => findings.Count;
        internal IReadOnlyList<EchoLaunchValidationFinding> Findings => findings;
        internal bool HasBlockingFindings => BlockerCount > 0;
        internal bool IsHealthy => Health == EchoLaunchProjectHealth.Healthy;

        private static void CountFindings(
            IList<EchoLaunchValidationFinding> source,
            out int information,
            out int warnings,
            out int errors,
            out int blockers)
        {
            information = 0;
            warnings = 0;
            errors = 0;
            blockers = 0;

            for (int index = 0; index < source.Count; index++)
            {
                switch (source[index].Severity)
                {
                    case EchoLaunchValidationSeverity.Information:
                        information++;
                        break;
                    case EchoLaunchValidationSeverity.Warning:
                        warnings++;
                        break;
                    case EchoLaunchValidationSeverity.Error:
                        errors++;
                        break;
                    case EchoLaunchValidationSeverity.Blocker:
                        blockers++;
                        break;
                    default:
                        throw new InvalidOperationException(
                            "The validation report contains an unknown severity.");
                }
            }
        }

        private static EchoLaunchProjectHealth DeriveHealth(
            int warnings,
            int errors,
            int blockers)
        {
            if (blockers > 0)
            {
                return EchoLaunchProjectHealth.Blocked;
            }

            if (errors > 0)
            {
                return EchoLaunchProjectHealth.Invalid;
            }

            return warnings > 0
                ? EchoLaunchProjectHealth.NeedsAttention
                : EchoLaunchProjectHealth.Healthy;
        }

        private static int CompareFindings(
            EchoLaunchValidationFinding left,
            EchoLaunchValidationFinding right)
        {
            int code =
                string.Compare(
                    left.Code,
                    right.Code,
                    StringComparison.Ordinal);

            if (code != 0)
            {
                return code;
            }

            int path =
                string.Compare(
                    left.ProjectPath,
                    right.ProjectPath,
                    StringComparison.Ordinal);

            if (path != 0)
            {
                return path;
            }

            int title =
                string.Compare(
                    left.Title,
                    right.Title,
                    StringComparison.Ordinal);

            return title != 0
                ? title
                : string.Compare(
                    left.Message,
                    right.Message,
                    StringComparison.Ordinal);
        }
    }
}
