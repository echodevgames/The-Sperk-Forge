namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded build metadata for one internal M4-03 manual-save transaction.
    ///
    /// The target slot comes only from the explicit active-slot session.
    /// Display name is intentionally absent because ordinary save is not rename.
    /// </summary>
    internal sealed class SaveManualTransactionRequest
    {
        internal SaveManualTransactionRequest(
            string projectId,
            string projectVersion,
            string buildId)
        {
            ProjectId =
                projectId ?? string.Empty;

            ProjectVersion =
                projectVersion ?? string.Empty;

            BuildId =
                buildId ?? string.Empty;
        }

        internal string ProjectId { get; }

        internal string ProjectVersion { get; }

        internal string BuildId { get; }
    }
}
