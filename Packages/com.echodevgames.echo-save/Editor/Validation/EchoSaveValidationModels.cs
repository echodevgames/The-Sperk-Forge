using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave.Editor
{
    public enum EchoSaveValidationSeverity
    {
        Error = 0,
        Warning = 1,
        Advisory = 2
    }

    public sealed class EchoSaveValidationIssue
    {
        public EchoSaveValidationIssue(
            string checkId,
            EchoSaveValidationSeverity severity,
            string message,
            string context,
            bool hasFix,
            bool automaticMutationPermitted)
        {
            CheckId = checkId ?? string.Empty;
            Severity = severity;
            Message = message ?? string.Empty;
            Context = context ?? string.Empty;
            HasFix = hasFix;
            AutomaticMutationPermitted =
                automaticMutationPermitted;
        }

        public string CheckId { get; }

        public EchoSaveValidationSeverity Severity { get; }

        public string Message { get; }

        public string Context { get; }

        public bool HasFix { get; }

        public bool AutomaticMutationPermitted { get; }
    }

    public sealed class EchoSaveValidationReport
    {
        public EchoSaveValidationReport(
            IReadOnlyList<EchoSaveValidationIssue> issues)
        {
            Issues =
                issues ??
                Array.Empty<EchoSaveValidationIssue>();
        }

        public IReadOnlyList<EchoSaveValidationIssue> Issues { get; }

        public bool HasErrors
        {
            get
            {
                for (int i = 0;
                     i < Issues.Count;
                     i++)
                {
                    if (Issues[i].Severity ==
                        EchoSaveValidationSeverity.Error)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
