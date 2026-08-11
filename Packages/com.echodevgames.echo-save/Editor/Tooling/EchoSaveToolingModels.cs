using System;

namespace EchoDevGames.EchoSave.Editor
{
    public sealed class EchoSaveSandboxPathResult
    {
        internal EchoSaveSandboxPathResult(
            bool succeeded,
            string sandboxRoot,
            string productionRoot,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            SandboxRoot = sandboxRoot ?? string.Empty;
            ProductionRoot = productionRoot ?? string.Empty;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public string SandboxRoot { get; }

        public string ProductionRoot { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }

    public sealed class EchoSaveTestDataRequest
    {
        public EchoSaveTestDataRequest(
            int slotCount,
            int generationsPerSlot,
            int payloadPaddingBytes,
            int seed)
        {
            SlotCount = slotCount;
            GenerationsPerSlot = generationsPerSlot;
            PayloadPaddingBytes = payloadPaddingBytes;
            Seed = seed;
        }

        public int SlotCount { get; }

        public int GenerationsPerSlot { get; }

        public int PayloadPaddingBytes { get; }

        public int Seed { get; }
    }

    public sealed class EchoSaveTestDataPlan
    {
        internal EchoSaveTestDataPlan(
            bool succeeded,
            string sandboxRoot,
            int slotCount,
            int generationCount,
            long estimatedBytes,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            SandboxRoot = sandboxRoot ?? string.Empty;
            SlotCount = slotCount;
            GenerationCount = generationCount;
            EstimatedBytes = estimatedBytes;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public string SandboxRoot { get; }

        public int SlotCount { get; }

        public int GenerationCount { get; }

        public long EstimatedBytes { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }

    public sealed class EchoSaveToolingOperationResult
    {
        internal EchoSaveToolingOperationResult(
            bool succeeded,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }

    public enum EchoSaveFailureScenario
    {
        DeleteManifest = 0,
        TruncateManifest = 1,
        DeleteHead = 2,
        FutureManifestVersion = 3
    }

    public sealed class EchoSaveFailureSimulationPlan
    {
        internal EchoSaveFailureSimulationPlan(
            bool succeeded,
            EchoSaveFailureScenario scenario,
            string sandboxRoot,
            string targetRelativePath,
            bool targetExists,
            string sourceFingerprint,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            Scenario = scenario;
            SandboxRoot = sandboxRoot ?? string.Empty;
            TargetRelativePath = targetRelativePath ?? string.Empty;
            TargetExists = targetExists;
            SourceFingerprint = sourceFingerprint ?? string.Empty;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public EchoSaveFailureScenario Scenario { get; }

        public string SandboxRoot { get; }

        public string TargetRelativePath { get; }

        public bool TargetExists { get; }

        public string SourceFingerprint { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }

    public sealed class EchoSaveSupportSnapshotResult
    {
        internal EchoSaveSupportSnapshotResult(
            bool succeeded,
            string json,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            Json = json ?? string.Empty;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public string Json { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }
}
