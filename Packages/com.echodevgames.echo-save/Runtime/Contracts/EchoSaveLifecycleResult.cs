using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable terminal result for the M1 Chronicle lifecycle.
    /// </summary>
    public readonly struct EchoSaveLifecycleResult
    {
        public EchoSaveLifecycleResult(
            EchoSaveLifecycleStatus status,
            EchoSaveServiceState state,
            string diagnosticCode,
            string message)
        {
            Status = status;
            State = state;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public EchoSaveLifecycleStatus Status { get; }

        public EchoSaveServiceState State { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == EchoSaveLifecycleStatus.Succeeded ||
            Status == EchoSaveLifecycleStatus.NoChange;

        public override string ToString()
        {
            if (DiagnosticCode.Length == 0)
            {
                return $"{Status}: {Message}";
            }

            return
                $"[{DiagnosticCode}] {Status}: {Message}";
        }
    }
}
