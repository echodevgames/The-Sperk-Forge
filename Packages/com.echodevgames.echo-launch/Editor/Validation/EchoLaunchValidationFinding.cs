using System;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidationFinding :
        IEquatable<EchoLaunchValidationFinding>
    {
        internal EchoLaunchValidationFinding(
            string code,
            EchoLaunchValidationSeverity severity,
            string title,
            string message,
            string projectPath,
            string evidence,
            string suggestedAction)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "A stable validation code is required.",
                    nameof(code));
            }

            if (!Enum.IsDefined(
                    typeof(EchoLaunchValidationSeverity),
                    severity))
            {
                throw new ArgumentOutOfRangeException(nameof(severity));
            }

            Code = code.Trim();
            Severity = severity;
            Title = Normalize(title);
            Message = Normalize(message);
            ProjectPath = NormalizePath(projectPath);
            Evidence = Normalize(evidence);
            SuggestedAction = Normalize(suggestedAction);
        }

        internal string Code { get; }
        internal EchoLaunchValidationSeverity Severity { get; }
        internal string Title { get; }
        internal string Message { get; }
        internal string ProjectPath { get; }
        internal string Evidence { get; }
        internal string SuggestedAction { get; }

        public bool Equals(EchoLaunchValidationFinding other)
        {
            return other != null &&
                   string.Equals(Code, other.Code, StringComparison.Ordinal) &&
                   Severity == other.Severity &&
                   string.Equals(Title, other.Title, StringComparison.Ordinal) &&
                   string.Equals(Message, other.Message, StringComparison.Ordinal) &&
                   string.Equals(ProjectPath, other.ProjectPath, StringComparison.Ordinal) &&
                   string.Equals(Evidence, other.Evidence, StringComparison.Ordinal) &&
                   string.Equals(
                       SuggestedAction,
                       other.SuggestedAction,
                       StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchValidationFinding);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Code.GetHashCode() * 397) ^ Severity.GetHashCode();
            }
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().Replace("\r", " ").Replace("\n", " ");
        }

        private static string NormalizePath(string value)
        {
            string normalized = Normalize(value).Replace('\\', '/');

            if (normalized.IndexOf(':') >= 0 ||
                normalized.StartsWith("/", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            return normalized;
        }
    }
}
