using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupDiagnostic :
        IEquatable<EchoLaunchSetupDiagnostic>
    {
        internal EchoLaunchSetupDiagnostic(
            string code,
            EchoLaunchSetupDiagnosticSeverity severity,
            string message,
            string targetPath = null)
        {
            Code = code ?? string.Empty;
            Severity = severity;
            Message = message ?? string.Empty;
            TargetPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(targetPath);
        }

        internal string Code { get; }
        internal EchoLaunchSetupDiagnosticSeverity Severity { get; }
        internal string Message { get; }
        internal string TargetPath { get; }

        public bool Equals(EchoLaunchSetupDiagnostic other)
        {
            return other != null &&
                   string.Equals(Code, other.Code, StringComparison.Ordinal) &&
                   Severity == other.Severity &&
                   string.Equals(Message, other.Message, StringComparison.Ordinal) &&
                   string.Equals(
                       TargetPath,
                       other.TargetPath,
                       StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchSetupDiagnostic);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }

    internal sealed class EchoLaunchSetupOperation :
        IEquatable<EchoLaunchSetupOperation>
    {
        internal EchoLaunchSetupOperation(
            string key,
            int phase,
            EchoLaunchSetupOperationKind kind,
            EchoLaunchSetupOperationDisposition disposition,
            string targetPath,
            string reason,
            string diagnosticCode = null,
            bool requiresExplicitApproval = false,
            string existingState = null,
            string proposedState = null,
            string proofSummary = null)
        {
            Key = key ?? string.Empty;
            Phase = phase;
            Kind = kind;
            Disposition = disposition;
            TargetPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(targetPath);
            Reason = reason ?? string.Empty;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            RequiresExplicitApproval = requiresExplicitApproval;
            ExistingState = existingState ?? string.Empty;
            ProposedState = proposedState ?? string.Empty;
            ProofSummary = proofSummary ?? string.Empty;
        }

        internal string Key { get; }
        internal int Phase { get; }
        internal EchoLaunchSetupOperationKind Kind { get; }
        internal EchoLaunchSetupOperationDisposition Disposition { get; }
        internal string TargetPath { get; }
        internal string Reason { get; }
        internal string DiagnosticCode { get; }
        internal bool RequiresExplicitApproval { get; }
        internal string ExistingState { get; }
        internal string ProposedState { get; }
        internal string ProofSummary { get; }

        public bool Equals(EchoLaunchSetupOperation other)
        {
            return other != null &&
                   string.Equals(Key, other.Key, StringComparison.Ordinal) &&
                   Phase == other.Phase &&
                   Kind == other.Kind &&
                   Disposition == other.Disposition &&
                   string.Equals(
                       TargetPath,
                       other.TargetPath,
                       StringComparison.Ordinal) &&
                   string.Equals(Reason, other.Reason, StringComparison.Ordinal) &&
                   string.Equals(
                       DiagnosticCode,
                       other.DiagnosticCode,
                       StringComparison.Ordinal) &&
                   RequiresExplicitApproval ==
                   other.RequiresExplicitApproval &&
                   string.Equals(ExistingState, other.ExistingState, StringComparison.Ordinal) &&
                   string.Equals(ProposedState, other.ProposedState, StringComparison.Ordinal) &&
                   string.Equals(ProofSummary, other.ProofSummary, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchSetupOperation);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    internal sealed class EchoLaunchSetupPlan
    {
        private readonly ReadOnlyCollection<EchoLaunchSetupOperation> operations;
        private readonly ReadOnlyCollection<EchoLaunchSetupDiagnostic> diagnostics;

        internal EchoLaunchSetupPlan(
            EchoLaunchSetupRequest request,
            EchoLaunchSetupPathSet paths,
            string snapshotEvidenceSummary,
            EchoLaunchSetupPlanStatus status,
            IEnumerable<EchoLaunchSetupOperation> operations,
            IEnumerable<EchoLaunchSetupDiagnostic> diagnostics,
            string requestFingerprint = null,
            string evidenceFingerprint = null,
            string planFingerprint = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Paths = paths;
            SnapshotEvidenceSummary = snapshotEvidenceSummary ?? string.Empty;
            Status = status;
            RequestFingerprint =
                string.IsNullOrEmpty(requestFingerprint)
                    ? EchoLaunchSetupFingerprint.ForRequest(request)
                    : requestFingerprint;
            EvidenceFingerprint = evidenceFingerprint ?? string.Empty;

            this.operations = new ReadOnlyCollection<EchoLaunchSetupOperation>(
                operations == null
                    ? new List<EchoLaunchSetupOperation>()
                    : new List<EchoLaunchSetupOperation>(operations));

            this.diagnostics = new ReadOnlyCollection<EchoLaunchSetupDiagnostic>(
                diagnostics == null
                    ? new List<EchoLaunchSetupDiagnostic>()
                    : new List<EchoLaunchSetupDiagnostic>(diagnostics));

            PlanFingerprint =
                string.IsNullOrEmpty(planFingerprint)
                    ? EchoLaunchSetupFingerprint.ForPlan(
                        RequestFingerprint,
                        EvidenceFingerprint,
                        Status,
                        this.operations,
                        this.diagnostics)
                    : planFingerprint;
        }

        internal EchoLaunchSetupRequest Request { get; }
        internal EchoLaunchSetupPathSet Paths { get; }
        internal string SnapshotEvidenceSummary { get; }
        internal string RequestFingerprint { get; }
        internal string EvidenceFingerprint { get; }
        internal string PlanFingerprint { get; }
        internal EchoLaunchSetupPlanStatus Status { get; }
        internal IReadOnlyList<EchoLaunchSetupOperation> Operations => operations;
        internal IReadOnlyList<EchoLaunchSetupDiagnostic> Diagnostics => diagnostics;

        internal bool HasBlockers =>
            Status == EchoLaunchSetupPlanStatus.Blocked;

        internal bool HasRepairs =>
            CountDisposition(EchoLaunchSetupOperationDisposition.Repair) > 0;

        internal bool HasCreates =>
            CountDisposition(EchoLaunchSetupOperationDisposition.Create) > 0;

        internal bool RequiresExplicitApproval
        {
            get
            {
                for (int index = 0; index < operations.Count; index++)
                {
                    if (operations[index].RequiresExplicitApproval)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal int CountDisposition(
            EchoLaunchSetupOperationDisposition disposition)
        {
            int count = 0;

            for (int index = 0; index < operations.Count; index++)
            {
                if (operations[index].Disposition == disposition)
                {
                    count++;
                }
            }

            return count;
        }

        internal bool ValueEquals(EchoLaunchSetupPlan other)
        {
            if (other == null ||
                Status != other.Status ||
                !Request.Equals(other.Request) ||
                !Equals(Paths, other.Paths) ||
                !string.Equals(
                    SnapshotEvidenceSummary,
                    other.SnapshotEvidenceSummary,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    RequestFingerprint,
                    other.RequestFingerprint,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    EvidenceFingerprint,
                    other.EvidenceFingerprint,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    PlanFingerprint,
                    other.PlanFingerprint,
                    StringComparison.Ordinal) ||
                operations.Count != other.operations.Count ||
                diagnostics.Count != other.diagnostics.Count)
            {
                return false;
            }

            for (int index = 0; index < operations.Count; index++)
            {
                if (!operations[index].Equals(other.operations[index]))
                {
                    return false;
                }
            }

            for (int index = 0; index < diagnostics.Count; index++)
            {
                if (!diagnostics[index].Equals(other.diagnostics[index]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
