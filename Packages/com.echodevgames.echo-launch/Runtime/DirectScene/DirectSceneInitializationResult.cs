//----- DirectSceneInitializationResult.cs START -----

using System;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Immutable observation describing one settled direct-scene initializer.
    ///
    /// This result reports helper settlement only. LaunchReport remains the
    /// authoritative record of startup execution.
    /// </summary>
    public sealed class DirectSceneInitializationResult
    {
        internal DirectSceneInitializationResult(
            DirectSceneInitializationStatus status,
            DirectSceneEntryPolicy policy,
            string diagnosticCode,
            string message,
            string containingScenePath,
            EchoLaunchRoot authoritativeRoot,
            bool createdRoot,
            bool reusedExistingAuthority)
        {
            if (!Enum.IsDefined(
                    typeof(DirectSceneInitializationStatus),
                    status) ||
                status == DirectSceneInitializationStatus.NotStarted)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "A settled direct-scene status is required.");
            }

            ValidateSettlementShape(
                status,
                authoritativeRoot,
                createdRoot,
                reusedExistingAuthority);

            Status = status;
            Policy = policy;
            DiagnosticCode = NormalizeOptional(diagnosticCode);
            Message = NormalizeOptional(message);
            ContainingScenePath = NormalizePath(containingScenePath);
            AuthoritativeRoot = authoritativeRoot;
            CreatedRoot = createdRoot;
            ReusedExistingAuthority = reusedExistingAuthority;
        }

        public DirectSceneInitializationStatus Status { get; }

        public DirectSceneEntryPolicy Policy { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public string ContainingScenePath { get; }

        public EchoLaunchRoot AuthoritativeRoot { get; }

        public bool CreatedRoot { get; }

        public bool ReusedExistingAuthority { get; }

        public bool IsSuccessful =>
            Status ==
                DirectSceneInitializationStatus.ReusedExistingAuthority ||
            Status ==
                DirectSceneInitializationStatus.CreatedDevelopmentAuthority;


        private static void ValidateSettlementShape(
            DirectSceneInitializationStatus status,
            EchoLaunchRoot authoritativeRoot,
            bool createdRoot,
            bool reusedExistingAuthority)
        {
            bool created =
                status ==
                DirectSceneInitializationStatus
                    .CreatedDevelopmentAuthority;

            bool reused =
                status ==
                DirectSceneInitializationStatus
                    .ReusedExistingAuthority;

            if (createdRoot != created ||
                reusedExistingAuthority != reused ||
                (created || reused) !=
                    (authoritativeRoot != null))
            {
                throw new ArgumentException(
                    "The direct-scene settlement flags and authority do not match the terminal status.");
            }
        }

        private static string NormalizeOptional(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim()
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private static string NormalizePath(string value)
        {
            string normalized = NormalizeOptional(value)
                .Replace('\\', '/');

            if (normalized.IndexOf(':') >= 0 ||
                normalized.StartsWith(
                    "/",
                    StringComparison.Ordinal))
            {
                return string.Empty;
            }

            return normalized;
        }
    }
}

//----- DirectSceneInitializationResult.cs END -----
