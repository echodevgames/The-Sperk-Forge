
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable payload-free summary of one fully verified committed
    /// generation eligible for recovery planning.
    /// </summary>
    public readonly struct SaveRecoveryCandidate
    {
        internal SaveRecoveryCandidate(
            SaveGenerationId generationId,
            string technicalTimestampUtc,
            string saveKind,
            string projectId,
            string projectVersion,
            string buildId)
        {
            GenerationId =
                generationId;

            TechnicalTimestampUtc =
                technicalTimestampUtc ?? string.Empty;

            SaveKind =
                saveKind ?? string.Empty;

            ProjectId =
                projectId ?? string.Empty;

            ProjectVersion =
                projectVersion ?? string.Empty;

            BuildId =
                buildId ?? string.Empty;
        }

        public SaveGenerationId GenerationId { get; }

        public string TechnicalTimestampUtc { get; }

        public string SaveKind { get; }

        public string ProjectId { get; }

        public string ProjectVersion { get; }

        public string BuildId { get; }
    }
}
