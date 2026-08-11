using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Bounded deterministic M5-04 synthetic Chronicle-like sandbox fixtures.
    /// These records are QA fixtures only and never become production saves.
    /// </summary>
    public sealed class EchoSaveTestDataGeneratorService
    {
        public const int MaximumSlots = 16;
        public const int MaximumGenerationsPerSlot = 32;
        public const int MaximumPayloadPaddingBytes = 4096;
        public const long MaximumEstimatedBytes = 2 * 1024 * 1024;

        internal const string OwnershipMarkerFileName =
            ".echosave-m504-owned";

        internal const string OwnershipMarkerContents =
            "EchoSave M5-04 sandbox fixture v1";

        public EchoSaveTestDataPlan Preview(
            EchoSaveConfiguration configuration,
            string sandboxRoot,
            EchoSaveTestDataRequest request)
        {
            EchoSaveSandboxPathResult path =
                EchoSaveSandboxPathGuard.Evaluate(
                    configuration,
                    sandboxRoot);

            if (!path.Succeeded)
            {
                return FailurePlan(
                    path.DiagnosticCode,
                    path.Message);
            }

            if (request == null)
            {
                return FailurePlan(
                    "M504-DATA-REQUEST",
                    "A bounded test-data request is required.");
            }

            if (request.SlotCount < 1 ||
                request.SlotCount > MaximumSlots)
            {
                return FailurePlan(
                    "M504-DATA-SLOTS",
                    $"SlotCount must be between 1 and {MaximumSlots}.");
            }

            if (request.GenerationsPerSlot < 1 ||
                request.GenerationsPerSlot >
                    MaximumGenerationsPerSlot)
            {
                return FailurePlan(
                    "M504-DATA-GENERATIONS",
                    $"GenerationsPerSlot must be between 1 and {MaximumGenerationsPerSlot}.");
            }

            if (request.PayloadPaddingBytes < 0 ||
                request.PayloadPaddingBytes >
                    MaximumPayloadPaddingBytes)
            {
                return FailurePlan(
                    "M504-DATA-BYTES",
                    $"PayloadPaddingBytes must be between 0 and {MaximumPayloadPaddingBytes}.");
            }

            int generationCount =
                checked(
                    request.SlotCount *
                    request.GenerationsPerSlot);

            long estimatedBytes =
                checked(
                    (long)generationCount *
                    (request.PayloadPaddingBytes + 1024L));

            if (estimatedBytes >
                MaximumEstimatedBytes)
            {
                return FailurePlan(
                    "M504-DATA-BOUND",
                    "The requested sandbox fixture exceeds the M5-04 aggregate byte bound.");
            }

            if (Directory.Exists(
                    path.SandboxRoot))
            {
                return FailurePlan(
                    "M504-DATA-EXISTS",
                    "The requested sandbox root already exists. M5-04 refuses to clobber an existing directory; clean up an owned fixture or choose another sandbox.");
            }

            return new EchoSaveTestDataPlan(
                true,
                path.SandboxRoot,
                request.SlotCount,
                generationCount,
                estimatedBytes,
                string.Empty,
                "The bounded synthetic Chronicle sandbox fixture is ready to generate.");
        }

        public EchoSaveToolingOperationResult Generate(
            EchoSaveConfiguration configuration,
            string sandboxRoot,
            EchoSaveTestDataRequest request)
        {
            EchoSaveTestDataPlan plan =
                Preview(
                    configuration,
                    sandboxRoot,
                    request);

            if (!plan.Succeeded)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    plan.DiagnosticCode,
                    plan.Message);
            }

            try
            {
                Directory.CreateDirectory(
                    plan.SandboxRoot);

                WriteText(
                    Path.Combine(
                        plan.SandboxRoot,
                        OwnershipMarkerFileName),
                    OwnershipMarkerContents);

                string slotsRoot =
                    Path.Combine(
                        plan.SandboxRoot,
                        "slots");

                Directory.CreateDirectory(
                    slotsRoot);

                for (int slotIndex = 0;
                     slotIndex < request.SlotCount;
                     slotIndex++)
                {
                    string slotId =
                        CreateSlotId(
                            request.Seed,
                            slotIndex);

                    string slotRoot =
                        Path.Combine(
                            slotsRoot,
                            slotId);

                    string generationsRoot =
                        Path.Combine(
                            slotRoot,
                            "generations");

                    Directory.CreateDirectory(
                        generationsRoot);

                    string currentGenerationId =
                        string.Empty;

                    for (int generationIndex = 0;
                         generationIndex <
                            request.GenerationsPerSlot;
                         generationIndex++)
                    {
                        string generationId =
                            CreateGenerationId(
                                request.Seed,
                                slotIndex,
                                generationIndex);

                        currentGenerationId =
                            generationId;

                        string generationRoot =
                            Path.Combine(
                                generationsRoot,
                                generationId);

                        Directory.CreateDirectory(
                            generationRoot);

                        WriteText(
                            Path.Combine(
                                generationRoot,
                                "manifest.json"),
                            BuildManifest(
                                slotId,
                                generationId,
                                request.PayloadPaddingBytes));

                        WriteText(
                            Path.Combine(
                                generationRoot,
                                "payload.json"),
                            BuildPayload(
                                slotId,
                                generationId,
                                request.PayloadPaddingBytes));
                    }

                    WriteText(
                        Path.Combine(
                            slotRoot,
                            "head.json"),
                        BuildHead(
                            slotId,
                            currentGenerationId));
                }

                return new EchoSaveToolingOperationResult(
                    true,
                    string.Empty,
                    $"Generated {request.SlotCount} synthetic slots and {plan.GenerationCount} synthetic generations under the isolated M5-04 sandbox.");
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-DATA-WRITE",
                    "The M5-04 sandbox fixture could not be generated. " +
                    exception.Message);
            }
        }

        public EchoSaveToolingOperationResult Cleanup(
            EchoSaveConfiguration configuration,
            string sandboxRoot)
        {
            EchoSaveSandboxPathResult path =
                EchoSaveSandboxPathGuard.Evaluate(
                    configuration,
                    sandboxRoot);

            if (!path.Succeeded)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    path.DiagnosticCode,
                    path.Message);
            }

            if (!Directory.Exists(
                    path.SandboxRoot))
            {
                return new EchoSaveToolingOperationResult(
                    true,
                    string.Empty,
                    "The M5-04 sandbox is already absent.");
            }

            string marker =
                Path.Combine(
                    path.SandboxRoot,
                    OwnershipMarkerFileName);

            if (!File.Exists(
                    marker))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-CLEANUP-OWNERSHIP",
                    "Cleanup refused the directory because the M5-04 ownership marker is missing.");
            }

            string markerContents =
                File.ReadAllText(
                    marker,
                    Encoding.UTF8);

            if (!string.Equals(
                    markerContents,
                    OwnershipMarkerContents,
                    StringComparison.Ordinal))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-CLEANUP-OWNERSHIP",
                    "Cleanup refused the directory because the M5-04 ownership marker does not match.");
            }

            try
            {
                Directory.Delete(
                    path.SandboxRoot,
                    true);
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException)
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-CLEANUP-FAILED",
                    "The M5-04 sandbox cleanup failed. " +
                    exception.Message);
            }

            if (Directory.Exists(
                    path.SandboxRoot))
            {
                return new EchoSaveToolingOperationResult(
                    false,
                    "M504-CLEANUP-RESIDUE",
                    "The M5-04 sandbox cleanup returned but owned residue remains.");
            }

            return new EchoSaveToolingOperationResult(
                true,
                string.Empty,
                "The owned M5-04 sandbox fixture was removed and post-cleanup absence was verified.");
        }

        public static bool IsOwnedSandbox(
            string sandboxRoot)
        {
            if (string.IsNullOrWhiteSpace(
                    sandboxRoot))
            {
                return false;
            }

            try
            {
                string root =
                    Path.GetFullPath(
                        sandboxRoot);

                string marker =
                    Path.Combine(
                        root,
                        OwnershipMarkerFileName);

                return File.Exists(
                           marker) &&
                       string.Equals(
                           File.ReadAllText(
                               marker,
                               Encoding.UTF8),
                           OwnershipMarkerContents,
                           StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        private static string BuildManifest(
            string slotId,
            string generationId,
            int payloadPaddingBytes)
        {
            return
                "{\n" +
                "  \"documentKind\": \"echosave.manifest\",\n" +
                "  \"documentVersion\": \"1.0.0\",\n" +
                "  \"slotId\": \"" + slotId + "\",\n" +
                "  \"generationId\": \"" + generationId + "\",\n" +
                "  \"commitState\": \"Committed\",\n" +
                "  \"updatedUtc\": \"2026-01-01T00:00:00.0000000+00:00\",\n" +
                "  \"participantCount\": 0,\n" +
                "  \"payloadByteLength\": " + payloadPaddingBytes + "\n" +
                "}\n";
        }

        private static string BuildPayload(
            string slotId,
            string generationId,
            int payloadPaddingBytes)
        {
            return
                "{\n" +
                "  \"documentKind\": \"echosave.payload\",\n" +
                "  \"documentVersion\": \"1.0.0\",\n" +
                "  \"slotId\": \"" + slotId + "\",\n" +
                "  \"generationId\": \"" + generationId + "\",\n" +
                "  \"entries\": [],\n" +
                "  \"padding\": \"" +
                new string(
                    'x',
                    payloadPaddingBytes) +
                "\"\n" +
                "}\n";
        }

        private static string BuildHead(
            string slotId,
            string generationId)
        {
            return
                "{\n" +
                "  \"documentKind\": \"echosave.head\",\n" +
                "  \"documentVersion\": \"1.0.0\",\n" +
                "  \"slotId\": \"" + slotId + "\",\n" +
                "  \"currentGenerationId\": \"" + generationId + "\"\n" +
                "}\n";
        }

        private static string CreateSlotId(
            int seed,
            int slotIndex)
        {
            byte[] hash =
                Hash(
                    $"slot|{seed}|{slotIndex}");

            byte[] guidBytes =
                new byte[16];

            Array.Copy(
                hash,
                guidBytes,
                guidBytes.Length);

            return new Guid(
                guidBytes)
                .ToString("D");
        }

        private static string CreateGenerationId(
            int seed,
            int slotIndex,
            int generationIndex)
        {
            string token =
                ToHex(
                    Hash(
                        $"generation|{seed}|{slotIndex}|{generationIndex}"))
                    .Substring(
                        0,
                        32);

            return
                "20260101T0000000000000Z-" +
                (generationIndex + 1)
                    .ToString("D16") +
                "-" +
                token;
        }

        private static byte[] Hash(
            string value)
        {
            using (SHA256 sha =
                   SHA256.Create())
            {
                return sha.ComputeHash(
                    Encoding.UTF8.GetBytes(
                        value ?? string.Empty));
            }
        }

        private static string ToHex(
            byte[] bytes)
        {
            StringBuilder builder =
                new StringBuilder(
                    bytes.Length * 2);

            for (int i = 0;
                 i < bytes.Length;
                 i++)
            {
                builder.Append(
                    bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static void WriteText(
            string path,
            string contents)
        {
            File.WriteAllText(
                path,
                contents,
                new UTF8Encoding(false));
        }

        private static EchoSaveTestDataPlan FailurePlan(
            string diagnosticCode,
            string message)
        {
            return new EchoSaveTestDataPlan(
                false,
                string.Empty,
                0,
                0,
                0,
                diagnosticCode,
                message);
        }
    }
}
