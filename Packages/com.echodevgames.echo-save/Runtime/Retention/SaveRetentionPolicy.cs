
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-owned bounded policy for ordinary committed generation history.
    ///
    /// M4-06 intentionally bounds total committed history first. Autosave- and
    /// trash-specific sub-bounds remain later policy extensions.
    /// </summary>
    public readonly struct SaveRetentionPolicy
    {
        public const int MinimumTotalGenerations = 2;
        public const int MaximumTotalGenerations = 256;
        public const int DefaultTotalGenerations = 5;

        public SaveRetentionPolicy(
            int maxTotalGenerations)
        {
            MaxTotalGenerations =
                maxTotalGenerations;
        }

        public int MaxTotalGenerations { get; }

        public bool IsValid =>
            MaxTotalGenerations >=
                MinimumTotalGenerations &&
            MaxTotalGenerations <=
                MaximumTotalGenerations;

        public static SaveRetentionPolicy Default =>
            new SaveRetentionPolicy(
                DefaultTotalGenerations);
    }
}
