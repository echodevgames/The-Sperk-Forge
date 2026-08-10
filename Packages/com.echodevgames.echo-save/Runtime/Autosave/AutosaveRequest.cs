using System.Threading;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Explicit caller-triggered M4-05 autosave request.
    ///
    /// Chronicle owns safe submission/coalescing after this request exists.
    /// Project/game code still owns when an autosave should be requested.
    /// </summary>
    public readonly struct AutosaveRequest
    {
        public AutosaveRequest(
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
