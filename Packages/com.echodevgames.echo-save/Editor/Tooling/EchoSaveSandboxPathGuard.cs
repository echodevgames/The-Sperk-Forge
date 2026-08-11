using System;
using System.IO;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Canonical path collision guard for M5-04 sandbox-only mutation tools.
    /// </summary>
    public static class EchoSaveSandboxPathGuard
    {
        public static EchoSaveSandboxPathResult Evaluate(
            EchoSaveConfiguration configuration,
            string proposedSandboxRoot)
        {
            return Evaluate(
                configuration,
                proposedSandboxRoot,
                Application.persistentDataPath);
        }

        public static EchoSaveSandboxPathResult Evaluate(
            EchoSaveConfiguration configuration,
            string proposedSandboxRoot,
            string persistentDataPath)
        {
            if (configuration == null)
            {
                return Failure(
                    "M504-SANDBOX-CONFIG",
                    "A Chronicle configuration is required.");
            }

            if (!configuration.TryResolveRuntimePolicy(
                    out _,
                    out string policyMessage))
            {
                return Failure(
                    "M504-SANDBOX-CONFIG",
                    policyMessage);
            }

            if (string.IsNullOrWhiteSpace(
                    proposedSandboxRoot))
            {
                return Failure(
                    "M504-SANDBOX-PATH",
                    "A non-empty sandbox root is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    persistentDataPath))
            {
                return Failure(
                    "M504-SANDBOX-PATH",
                    "A persistent-data root is required.");
            }

            try
            {
                string production =
                    Path.GetFullPath(
                        Path.Combine(
                            persistentDataPath,
                            configuration.StorageRootDirectoryName));

                string sandbox =
                    Path.GetFullPath(
                        proposedSandboxRoot);

                string filesystemRoot =
                    Path.GetPathRoot(
                        sandbox);

                if (!string.IsNullOrEmpty(
                        filesystemRoot) &&
                    PathsEqual(
                        filesystemRoot,
                        sandbox))
                {
                    return Failure(
                        "M504-SANDBOX-ROOT",
                        "The sandbox may not be a filesystem root.");
                }

                if (PathsEqual(
                        production,
                        sandbox) ||
                    IsNestedUnder(
                        sandbox,
                        production) ||
                    IsNestedUnder(
                        production,
                        sandbox))
                {
                    return new EchoSaveSandboxPathResult(
                        false,
                        sandbox,
                        production,
                        "M504-SANDBOX-COLLISION",
                        "The M5-04 sandbox must be disjoint from the production Chronicle root. Equal, containing, and nested paths are refused.");
                }

                return new EchoSaveSandboxPathResult(
                    true,
                    sandbox,
                    production,
                    string.Empty,
                    "The M5-04 sandbox is canonically disjoint from the production Chronicle root.");
            }
            catch (Exception exception)
                when (exception is ArgumentException ||
                      exception is NotSupportedException ||
                      exception is PathTooLongException)
            {
                return Failure(
                    "M504-SANDBOX-PATH",
                    "The M5-04 sandbox path could not be normalized. " +
                    exception.Message);
            }
        }

        private static bool IsNestedUnder(
            string candidate,
            string parent)
        {
            string normalizedCandidate =
                WithTrailingSeparator(
                    candidate);

            string normalizedParent =
                WithTrailingSeparator(
                    parent);

            return normalizedCandidate.StartsWith(
                normalizedParent,
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool PathsEqual(
            string left,
            string right)
        {
            return string.Equals(
                TrimTrailingSeparators(left),
                TrimTrailingSeparators(right),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string WithTrailingSeparator(
            string path)
        {
            return TrimTrailingSeparators(
                       path) +
                   Path.DirectorySeparatorChar;
        }

        private static string TrimTrailingSeparators(
            string path)
        {
            return (path ?? string.Empty).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
        }

        private static EchoSaveSandboxPathResult Failure(
            string diagnosticCode,
            string message)
        {
            return new EchoSaveSandboxPathResult(
                false,
                string.Empty,
                string.Empty,
                diagnosticCode,
                message);
        }
    }
}
