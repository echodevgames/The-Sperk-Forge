using System.Threading;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public bounded M4-04 manual-save request.
    ///
    /// The target is always Chronicle's explicitly selected active slot.
    /// Ordinary save cannot provide a path or rename the slot.
    /// </summary>
    public readonly struct SaveRequest
    {
        public SaveRequest(
            string projectId,
            string projectVersion,
            string buildId,
            CancellationToken cancellationToken = default)
        {
            ProjectId =
                projectId;

            ProjectVersion =
                projectVersion;

            BuildId =
                buildId;

            CancellationToken =
                cancellationToken;
        }

        public string ProjectId { get; }

        public string ProjectVersion { get; }

        public string BuildId { get; }

        public CancellationToken CancellationToken { get; }
    }
}
