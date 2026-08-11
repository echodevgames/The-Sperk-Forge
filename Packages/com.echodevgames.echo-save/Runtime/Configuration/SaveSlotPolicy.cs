namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable normalized slot-policy truth for one Chronicle service session.
    /// </summary>
    public sealed class SaveSlotPolicy
    {
        internal SaveSlotPolicy(
            SaveSlotPolicyMode mode,
            int fixedSlotCount,
            int configuredSlotLimit,
            int profileSafetyLimit,
            int effectiveCapacity,
            int sourceConfigurationSchema,
            bool compatibilityMapped)
        {
            Mode = mode;
            FixedSlotCount = fixedSlotCount;
            ConfiguredSlotLimit = configuredSlotLimit;
            ProfileSafetyLimit = profileSafetyLimit;
            EffectiveCapacity = effectiveCapacity;
            SourceConfigurationSchema = sourceConfigurationSchema;
            CompatibilityMapped = compatibilityMapped;
        }

        public SaveSlotPolicyMode Mode { get; }

        public int FixedSlotCount { get; }

        public int ConfiguredSlotLimit { get; }

        public int ProfileSafetyLimit { get; }

        public int EffectiveCapacity { get; }

        public int SourceConfigurationSchema { get; }

        public bool CompatibilityMapped { get; }
    }
}
