namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable session configuration resolved before Chronicle creates storage.
    /// </summary>
    public sealed class EchoSaveRuntimePolicy
    {
        internal EchoSaveRuntimePolicy(
            SaveSlotPolicy slotPolicy,
            SaveRetentionPolicy retentionPolicy,
            SaveLimitPolicy limits,
            string serializerProviderId,
            string storageProviderId,
            EchoSaveRecoveryPolicyMode recoveryPolicyMode,
            int sourceConfigurationSchema,
            bool compatibilityMapped)
        {
            SlotPolicy = slotPolicy;
            RetentionPolicy = retentionPolicy;
            Limits = limits;
            SerializerProviderId =
                serializerProviderId ?? string.Empty;
            StorageProviderId =
                storageProviderId ?? string.Empty;
            RecoveryPolicyMode = recoveryPolicyMode;
            SourceConfigurationSchema =
                sourceConfigurationSchema;
            CompatibilityMapped =
                compatibilityMapped;
        }

        public SaveSlotPolicy SlotPolicy { get; }

        public SaveRetentionPolicy RetentionPolicy { get; }

        public SaveLimitPolicy Limits { get; }

        public string SerializerProviderId { get; }

        public string StorageProviderId { get; }

        public EchoSaveRecoveryPolicyMode RecoveryPolicyMode { get; }

        public int SourceConfigurationSchema { get; }

        public bool CompatibilityMapped { get; }
    }
}
