
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public bounded metadata for creating one new technical Chronicle slot.
    /// These values become manifest metadata only and never become storage
    /// path authority.
    /// </summary>
    public sealed class SaveSlotCreateRequest
    {
        public SaveSlotCreateRequest(
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

        public string DisplayName { get; }

        public string ProjectId { get; }

        public string ProjectVersion { get; }

        public string BuildId { get; }
    }
}
