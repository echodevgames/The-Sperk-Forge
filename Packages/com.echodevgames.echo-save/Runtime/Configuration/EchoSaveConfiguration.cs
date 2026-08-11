using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-owned Chronicle configuration definition.
    ///
    /// Schema 1 retains historical 64-slot compatibility. Schema 2 owns the
    /// M4 slot policy. Schema 3 adds bounded retention/discovery/provider and
    /// authoring metadata while preserving non-mutating compatibility.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EchoSaveConfiguration",
        menuName = "EchoDevGames/The Chronicle/Echo Save Configuration")]
    public sealed class EchoSaveConfiguration : ScriptableObject
    {
        public const int LegacySchemaVersion = 1;
        public const int SlotPolicySchemaVersion = 2;
        public const int CurrentSchemaVersion = 3;

        public const string DefaultStorageProviderId =
            "echodevgames.local-file";

        public const string DefaultSerializerProviderId =
            UnityJsonSaveSerializer.StableId;

        internal const int LegacySchemaOneTechnicalSlotCapacity =
            64;

        [SerializeField]
        private int schemaVersion = CurrentSchemaVersion;

        [SerializeField]
        private string storageRootDirectoryName = "EchoSave";

        [SerializeField]
        private SaveSlotPolicyMode slotPolicyMode =
            SaveSlotPolicyMode.ConfigurableMultiSlot;

        [SerializeField]
        private int fixedSlotCount = 4;

        [SerializeField]
        private int configuredSlotLimit = 64;

        [SerializeField]
        private int profileSafetyLimit = 64;

        [SerializeField]
        private int maxTotalGenerations =
            SaveRetentionPolicy.DefaultTotalGenerations;

        [SerializeField]
        private string serializerProviderId =
            DefaultSerializerProviderId;

        [SerializeField]
        private string storageProviderId =
            DefaultStorageProviderId;

        [SerializeField]
        private int catalogScanLimit =
            SaveLimitPolicy.DefaultCatalogScanLimit;

        [SerializeField]
        private int retentionDiscoveryLimit =
            SaveLimitPolicy.DefaultRetentionDiscoveryLimit;

        [SerializeField]
        private int recoveryDiscoveryLimit =
            SaveLimitPolicy.DefaultRecoveryDiscoveryLimit;

        [SerializeField]
        private EchoSaveRecoveryPolicyMode recoveryPolicyMode =
            EchoSaveRecoveryPolicyMode.ManualOnly;

        [SerializeField]
        private SaveSlotTemplate[] fixedSlotTemplates =
            Array.Empty<SaveSlotTemplate>();

        public int SchemaVersion => schemaVersion;

        public string StorageRootDirectoryName =>
            storageRootDirectoryName ?? string.Empty;

        public SaveSlotPolicyMode SlotPolicyMode =>
            slotPolicyMode;

        public int FixedSlotCount =>
            fixedSlotCount;

        public int ConfiguredSlotLimit =>
            configuredSlotLimit;

        public int ProfileSafetyLimit =>
            profileSafetyLimit;

        public int MaxTotalGenerations =>
            maxTotalGenerations;

        public string SerializerProviderId =>
            serializerProviderId ?? string.Empty;

        public string StorageProviderId =>
            storageProviderId ?? string.Empty;

        public int CatalogScanLimit =>
            catalogScanLimit;

        public int RetentionDiscoveryLimit =>
            retentionDiscoveryLimit;

        public int RecoveryDiscoveryLimit =>
            recoveryDiscoveryLimit;

        public EchoSaveRecoveryPolicyMode RecoveryPolicyMode =>
            recoveryPolicyMode;

        public IReadOnlyList<SaveSlotTemplate> FixedSlotTemplates =>
            fixedSlotTemplates ?? Array.Empty<SaveSlotTemplate>();

        public bool IsCurrentSchema =>
            schemaVersion == CurrentSchemaVersion;

        /// <summary>
        /// Resolves serialized slot capacity without mutating the asset.
        /// </summary>
        public bool TryResolveSlotPolicy(
            out SaveSlotPolicy policy,
            out string message)
        {
            policy = null;

            if (schemaVersion == LegacySchemaVersion)
            {
                policy =
                    new SaveSlotPolicy(
                        SaveSlotPolicyMode.ConfigurableMultiSlot,
                        0,
                        LegacySchemaOneTechnicalSlotCapacity,
                        0,
                        LegacySchemaOneTechnicalSlotCapacity,
                        LegacySchemaVersion,
                        true);

                message =
                    "Chronicle schema-1 slot policy compatibility is active at capacity 64; the configuration asset was not rewritten.";
                return true;
            }

            if (schemaVersion != SlotPolicySchemaVersion &&
                schemaVersion != CurrentSchemaVersion)
            {
                message =
                    $"EchoSaveConfiguration schema {schemaVersion} is unsupported. " +
                    $"Expected schema {LegacySchemaVersion}, {SlotPolicySchemaVersion}, or {CurrentSchemaVersion}.";
                return false;
            }

            int effectiveCapacity;

            switch (slotPolicyMode)
            {
                case SaveSlotPolicyMode.SingleSlot:
                    effectiveCapacity = 1;
                    break;

                case SaveSlotPolicyMode.FixedMultiSlot:
                    if (fixedSlotCount < 2)
                    {
                        message =
                            "Chronicle FixedMultiSlot policy requires FixedSlotCount >= 2.";
                        return false;
                    }

                    effectiveCapacity = fixedSlotCount;
                    break;

                case SaveSlotPolicyMode.ConfigurableMultiSlot:
                    if (configuredSlotLimit < 1)
                    {
                        message =
                            "Chronicle ConfigurableMultiSlot policy requires ConfiguredSlotLimit >= 1.";
                        return false;
                    }

                    effectiveCapacity = configuredSlotLimit;
                    break;

                case SaveSlotPolicyMode.BoundedProfiles:
                    if (profileSafetyLimit < 1)
                    {
                        message =
                            "Chronicle BoundedProfiles policy requires ProfileSafetyLimit >= 1.";
                        return false;
                    }

                    effectiveCapacity = profileSafetyLimit;
                    break;

                default:
                    message =
                        $"Chronicle slot policy mode value {(int)slotPolicyMode} is undefined.";
                    return false;
            }

            policy =
                new SaveSlotPolicy(
                    slotPolicyMode,
                    fixedSlotCount,
                    configuredSlotLimit,
                    profileSafetyLimit,
                    effectiveCapacity,
                    schemaVersion,
                    false);

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Resolves all runtime-consumed project configuration into one
        /// immutable session snapshot without mutating this asset.
        /// </summary>
        public bool TryResolveRuntimePolicy(
            out EchoSaveRuntimePolicy policy,
            out string message)
        {
            policy = null;

            if (!TryResolveSlotPolicy(
                    out SaveSlotPolicy resolvedSlotPolicy,
                    out message))
            {
                return false;
            }

            if (!TryValidateStorageRoot(
                    StorageRootDirectoryName,
                    out message))
            {
                return false;
            }

            if (schemaVersion == LegacySchemaVersion ||
                schemaVersion == SlotPolicySchemaVersion)
            {
                policy =
                    new EchoSaveRuntimePolicy(
                        resolvedSlotPolicy,
                        SaveRetentionPolicy.Default,
                        SaveLimitPolicy.Default,
                        DefaultSerializerProviderId,
                        DefaultStorageProviderId,
                        EchoSaveRecoveryPolicyMode.ManualOnly,
                        schemaVersion,
                        true);

                message =
                    $"Chronicle schema-{schemaVersion} configuration compatibility is active for M5-02 fields; deterministic defaults were resolved in memory and the asset was not rewritten.";
                return true;
            }

            SaveRetentionPolicy retention =
                new SaveRetentionPolicy(
                    maxTotalGenerations);

            if (!retention.IsValid)
            {
                message =
                    $"Chronicle retention requires MaxTotalGenerations between {SaveRetentionPolicy.MinimumTotalGenerations} and {SaveRetentionPolicy.MaximumTotalGenerations}.";
                return false;
            }

            SaveLimitPolicy limits =
                new SaveLimitPolicy(
                    catalogScanLimit,
                    retentionDiscoveryLimit,
                    recoveryDiscoveryLimit);

            if (!limits.IsValid)
            {
                message =
                    $"Chronicle discovery limits must each be between {SaveLimitPolicy.MinimumDiscoveryLimit} and {SaveLimitPolicy.MaximumDiscoveryLimit}.";
                return false;
            }

            string serializerId =
                SerializerProviderId.Trim();

            if (!string.Equals(
                    serializerId,
                    DefaultSerializerProviderId,
                    StringComparison.Ordinal))
            {
                message =
                    $"Chronicle serializer provider '{serializerId}' is unavailable during project initialization. M5-02 currently supports '{DefaultSerializerProviderId}'.";
                return false;
            }

            string storageId =
                StorageProviderId.Trim();

            if (!string.Equals(
                    storageId,
                    DefaultStorageProviderId,
                    StringComparison.Ordinal))
            {
                message =
                    $"Chronicle storage provider '{storageId}' is unavailable during project initialization. M5-02 currently supports '{DefaultStorageProviderId}'.";
                return false;
            }

            if (!Enum.IsDefined(
                    typeof(EchoSaveRecoveryPolicyMode),
                    recoveryPolicyMode))
            {
                message =
                    $"Chronicle recovery policy mode value {(int)recoveryPolicyMode} is undefined.";
                return false;
            }

            if (recoveryPolicyMode !=
                EchoSaveRecoveryPolicyMode.ManualOnly)
            {
                message =
                    "Chronicle M5-02 exposes only ManualOnly recovery because automatic fallback remains separately gated.";
                return false;
            }

            policy =
                new EchoSaveRuntimePolicy(
                    resolvedSlotPolicy,
                    retention,
                    limits,
                    serializerId,
                    storageId,
                    recoveryPolicyMode,
                    CurrentSchemaVersion,
                    false);

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates authoring-only fixed-slot template metadata.
        /// Runtime slot identity/capacity does not depend on these assets.
        /// </summary>
        public bool TryValidateFixedSlotTemplates(
            out string message)
        {
            SaveSlotTemplate[] templates =
                fixedSlotTemplates ??
                Array.Empty<SaveSlotTemplate>();

            var ids =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0;
                 i < templates.Length;
                 i++)
            {
                SaveSlotTemplate template =
                    templates[i];

                if (template == null)
                {
                    message =
                        $"Chronicle fixed-slot template entry {i} is missing.";
                    return false;
                }

                string id =
                    template.TemplateId.Trim();

                if (!IsSafeTemplateId(id))
                {
                    message =
                        $"Chronicle fixed-slot template entry {i} has an invalid stable ID.";
                    return false;
                }

                if (!ids.Add(id))
                {
                    message =
                        $"Chronicle fixed-slot template ID '{id}' is duplicated.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        internal bool TryValidate(
            out string message)
        {
            return TryResolveRuntimePolicy(
                out _,
                out message);
        }

        internal void SetDefinitionForTesting(
            int schema,
            string rootDirectoryName)
        {
            schemaVersion = schema;
            storageRootDirectoryName = rootDirectoryName;
        }

        internal void SetSlotPolicyForTesting(
            SaveSlotPolicyMode mode,
            int fixedCount,
            int configuredLimit,
            int profileLimit)
        {
            slotPolicyMode = mode;
            fixedSlotCount = fixedCount;
            configuredSlotLimit = configuredLimit;
            profileSafetyLimit = profileLimit;
        }

        internal void SetRuntimePolicyForTesting(
            int maxGenerations,
            string serializerId,
            string storageId,
            int catalogLimit,
            int retentionLimit,
            int recoveryLimit,
            EchoSaveRecoveryPolicyMode recoveryMode)
        {
            maxTotalGenerations = maxGenerations;
            serializerProviderId = serializerId;
            storageProviderId = storageId;
            catalogScanLimit = catalogLimit;
            retentionDiscoveryLimit = retentionLimit;
            recoveryDiscoveryLimit = recoveryLimit;
            recoveryPolicyMode = recoveryMode;
        }

        internal void SetFixedSlotTemplatesForTesting(
            params SaveSlotTemplate[] templates)
        {
            fixedSlotTemplates =
                templates ??
                Array.Empty<SaveSlotTemplate>();
        }

        private static bool TryValidateStorageRoot(
            string root,
            out string message)
        {
            string value =
                (root ?? string.Empty).Trim();

            if (value.Length == 0)
            {
                message =
                    "The Chronicle storage-root directory name is empty.";
                return false;
            }

            if (value == "." ||
                value == ".." ||
                value.IndexOf('/') >= 0 ||
                value.IndexOf('\\') >= 0 ||
                value.IndexOf(':') >= 0)
            {
                message =
                    "The Chronicle storage-root directory name must be one safe relative directory segment.";
                return false;
            }

            for (int i = 0;
                 i < value.Length;
                 i++)
            {
                if (char.IsControl(value[i]))
                {
                    message =
                        "The Chronicle storage-root directory name contains a control character.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        private static bool IsSafeTemplateId(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                value.Length > 128)
            {
                return false;
            }

            for (int i = 0;
                 i < value.Length;
                 i++)
            {
                char character =
                    value[i];

                if (char.IsControl(character) ||
                    char.IsWhiteSpace(character) ||
                    character == '/' ||
                    character == '\\' ||
                    character == ':')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
