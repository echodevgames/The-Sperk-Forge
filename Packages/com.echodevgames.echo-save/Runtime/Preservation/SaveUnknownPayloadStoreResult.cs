
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveUnknownPayloadStoreResult
    {
        internal SaveUnknownPayloadStoreResult(
            SaveUnknownPayloadStoreStatus status,
            string diagnosticCode,
            string message)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;
        }

        internal SaveUnknownPayloadStoreStatus Status
        {
            get;
        }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status ==
            SaveUnknownPayloadStoreStatus.Succeeded;

        internal static SaveUnknownPayloadStoreResult
            Success(
                string message) =>
            new SaveUnknownPayloadStoreResult(
                SaveUnknownPayloadStoreStatus.Succeeded,
                string.Empty,
                message);
    }
}
