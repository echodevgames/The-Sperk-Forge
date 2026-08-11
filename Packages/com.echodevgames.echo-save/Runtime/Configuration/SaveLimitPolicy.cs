namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable bounded discovery limits resolved from project configuration.
    /// </summary>
    public readonly struct SaveLimitPolicy
    {
        public const int MinimumDiscoveryLimit = 1;
        public const int MaximumDiscoveryLimit = 4096;

        public const int DefaultCatalogScanLimit = 256;
        public const int DefaultRetentionDiscoveryLimit = 512;
        public const int DefaultRecoveryDiscoveryLimit = 512;

        public SaveLimitPolicy(
            int catalogScanLimit,
            int retentionDiscoveryLimit,
            int recoveryDiscoveryLimit)
        {
            CatalogScanLimit = catalogScanLimit;
            RetentionDiscoveryLimit = retentionDiscoveryLimit;
            RecoveryDiscoveryLimit = recoveryDiscoveryLimit;
        }

        public int CatalogScanLimit { get; }

        public int RetentionDiscoveryLimit { get; }

        public int RecoveryDiscoveryLimit { get; }

        public bool IsValid =>
            Bounded(CatalogScanLimit) &&
            Bounded(RetentionDiscoveryLimit) &&
            Bounded(RecoveryDiscoveryLimit);

        public static SaveLimitPolicy Default =>
            new SaveLimitPolicy(
                DefaultCatalogScanLimit,
                DefaultRetentionDiscoveryLimit,
                DefaultRecoveryDiscoveryLimit);

        private static bool Bounded(int value) =>
            value >= MinimumDiscoveryLimit &&
            value <= MaximumDiscoveryLimit;
    }
}
