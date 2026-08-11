using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Preview-first one-file corruption simulator for owned M5-04 sandboxes.
    /// </summary>
    public sealed class EchoSaveFailureSimulatorService
    {
        public EchoSaveFailureSimulationPlan Preview(
            EchoSaveConfiguration configuration,
            string sandboxRoot,
            EchoSaveFailureScenario scenario)
        {
            EchoSaveSandboxPathResult path =
                EchoSaveSandboxPathGuard.Evaluate(
                    configuration,
                    sandboxRoot);

            if (!path.Succeeded)
            {
                return Failure(
                    scenario,
                    path.DiagnosticCode,
                    path.Message);
            }

            if (!EchoSaveTestDataGeneratorService.IsOwnedSandbox(
                    path.SandboxRoot))
            {
                return Failure(
                    scenario,
                    "M504-SIM-OWNERSHIP",
                    "Failure simulation requires an M5-04 owned sandbox fixture.");
            }

            string target =
                ResolveTarget(
                    path.SandboxRoot,
                    scenario);

            if (string.IsNullOrEmpty(
                    target))
            {
                return Failure(
                    scenario,
                    "M504-SIM-TARGET",
                    "The requested simulator scenario could not resolve one bounded sandbox target.");
            }

            bool exists =
                File.Exists(
                    target);

            if (!exists)
            {
                return Failure(
                    scenario,
                    "M504-SIM-MISSING",
                    "The bounded simulator target does not exist.");
            }

            string fingerprint =
                Fingerprint(
                    File.ReadAllBytes(
                        target));

            string relative =
                MakeRelative(
                    path.SandboxRoot,
                    target);

            return new EchoSaveFailureSimulationPlan(
                true,
                scenario,
                path.SandboxRoot,
                relative,
                true,
                fingerprint,
                string.Empty,
                $"Preview only: {scenario} would mutate exactly '{relative}' inside the owned sandbox.");
        }

        public EchoSaveToolingOperationResult Apply(
            EchoSaveConfiguration configuration,
            EchoSaveFailureSimulationPlan plan)
        {
            if (plan == null ||
                !plan.Succeeded)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-PLAN",
                    "A successful Failure Simulator preview is required before Apply.");
            }

            EchoSaveSandboxPathResult path =
                EchoSaveSandboxPathGuard.Evaluate(
                    configuration,
                    plan.SandboxRoot);

            if (!path.Succeeded)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    path.DiagnosticCode,
                    path.Message);
            }

            if (!EchoSaveTestDataGeneratorService.IsOwnedSandbox(
                    path.SandboxRoot))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-OWNERSHIP",
                    "Failure simulation refused an unowned sandbox.");
            }

            string target =
                Path.GetFullPath(
                    Path.Combine(
                        path.SandboxRoot,
                        plan.TargetRelativePath));

            if (!IsUnder(
                    target,
                    path.SandboxRoot))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-ESCAPE",
                    "Failure simulation refused a target that escaped the owned sandbox.");
            }

            if (!File.Exists(
                    target))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-STALE",
                    "The simulator target changed after Preview.");
            }

            string currentFingerprint =
                Fingerprint(
                    File.ReadAllBytes(
                        target));

            if (!string.Equals(
                    currentFingerprint,
                    plan.SourceFingerprint,
                    StringComparison.Ordinal))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-STALE",
                    "The simulator target bytes changed after Preview; Apply was refused.");
            }

            try
            {
                switch (plan.Scenario)
                {
                    case EchoSaveFailureScenario.DeleteManifest:
                    case EchoSaveFailureScenario.DeleteHead:
                        File.Delete(
                            target);
                        break;

                    case EchoSaveFailureScenario.TruncateManifest:
                    {
                        byte[] bytes =
                            File.ReadAllBytes(
                                target);

                        int length =
                            Math.Max(
                                1,
                                bytes.Length / 2);

                        byte[] truncated =
                            new byte[length];

                        Array.Copy(
                            bytes,
                            truncated,
                            length);

                        File.WriteAllBytes(
                            target,
                            truncated);
                        break;
                    }

                    case EchoSaveFailureScenario.FutureManifestVersion:
                    {
                        string text =
                            File.ReadAllText(
                                target,
                                Encoding.UTF8);

                        string replaced =
                            text.Replace(
                                "\"documentVersion\": \"1.0.0\"",
                                "\"documentVersion\": \"999.0.0\"");

                        if (string.Equals(
                                text,
                                replaced,
                                StringComparison.Ordinal))
                        {
                            return new EchoSaveToolingOperationResult(
                                false,
                                "M504-SIM-VERSION",
                                "The simulator could not find the bounded manifest version token.");
                        }

                        File.WriteAllText(
                            target,
                            replaced,
                            new UTF8Encoding(false));
                        break;
                    }

                    default:
                        return new EchoSaveToolingOperationResult(
                            false,
                            "M504-SIM-SCENARIO",
                            "The simulator scenario is undefined.");
                }
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-SIM-WRITE",
                    "Failure Simulator Apply failed. " +
                    exception.Message);
            }

            return new EchoSaveToolingOperationResult(
                true,
                string.Empty,
                $"Applied {plan.Scenario} to exactly '{plan.TargetRelativePath}' inside the owned M5-04 sandbox.");
        }

        private static string ResolveTarget(
            string sandboxRoot,
            EchoSaveFailureScenario scenario)
        {
            string slotsRoot =
                Path.Combine(
                    sandboxRoot,
                    "slots");

            string[] slots =
                Directory.Exists(
                    slotsRoot)
                    ? Directory.GetDirectories(
                        slotsRoot)
                    : Array.Empty<string>();

            Array.Sort(
                slots,
                StringComparer.Ordinal);

            if (slots.Length == 0)
            {
                return string.Empty;
            }

            if (scenario ==
                EchoSaveFailureScenario.DeleteHead)
            {
                return Path.Combine(
                    slots[0],
                    "head.json");
            }

            string generationsRoot =
                Path.Combine(
                    slots[0],
                    "generations");

            string[] generations =
                Directory.Exists(
                    generationsRoot)
                    ? Directory.GetDirectories(
                        generationsRoot)
                    : Array.Empty<string>();

            Array.Sort(
                generations,
                StringComparer.Ordinal);

            if (generations.Length == 0)
            {
                return string.Empty;
            }

            return Path.Combine(
                generations[0],
                "manifest.json");
        }

        private static string MakeRelative(
            string root,
            string target)
        {
            Uri rootUri =
                new Uri(
                    EnsureTrailingSeparator(
                        Path.GetFullPath(
                            root)));

            Uri targetUri =
                new Uri(
                    Path.GetFullPath(
                        target));

            return Uri.UnescapeDataString(
                    rootUri
                        .MakeRelativeUri(
                            targetUri)
                        .ToString())
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);
        }

        private static bool IsUnder(
            string candidate,
            string root)
        {
            string normalizedCandidate =
                Path.GetFullPath(
                    candidate);

            string normalizedRoot =
                EnsureTrailingSeparator(
                    Path.GetFullPath(
                        root));

            return normalizedCandidate.StartsWith(
                normalizedRoot,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string EnsureTrailingSeparator(
            string path)
        {
            return path.TrimEnd(
                       Path.DirectorySeparatorChar,
                       Path.AltDirectorySeparatorChar) +
                   Path.DirectorySeparatorChar;
        }

        private static string Fingerprint(
            byte[] bytes)
        {
            using (SHA256 sha =
                   SHA256.Create())
            {
                byte[] hash =
                    sha.ComputeHash(
                        bytes ??
                        Array.Empty<byte>());

                StringBuilder builder =
                    new StringBuilder(
                        hash.Length * 2);

                for (int i = 0;
                     i < hash.Length;
                     i++)
                {
                    builder.Append(
                        hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static EchoSaveFailureSimulationPlan Failure(
            EchoSaveFailureScenario scenario,
            string diagnosticCode,
            string message)
        {
            return new EchoSaveFailureSimulationPlan(
                false,
                scenario,
                string.Empty,
                string.Empty,
                false,
                string.Empty,
                diagnosticCode,
                message);
        }
    }
}
