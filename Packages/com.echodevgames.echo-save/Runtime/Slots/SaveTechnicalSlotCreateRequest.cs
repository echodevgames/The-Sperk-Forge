
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded technical metadata used when creating one Chronicle slot.
    ///
    /// These values are manifest metadata only. None of them are storage keys.
    /// </summary>
    internal sealed class SaveTechnicalSlotCreateRequest
    {
        internal SaveTechnicalSlotCreateRequest(
            string displayName,
            string projectId,
            string projectVersion,
            string buildId)
        {
            DisplayName =
                displayName ?? string.Empty;

            ProjectId =
                projectId ?? string.Empty;

            ProjectVersion =
                projectVersion ?? string.Empty;

            BuildId =
                buildId ?? string.Empty;
        }

        internal string DisplayName { get; }

        internal string ProjectId { get; }

        internal string ProjectVersion { get; }

        internal string BuildId { get; }
    }
}
