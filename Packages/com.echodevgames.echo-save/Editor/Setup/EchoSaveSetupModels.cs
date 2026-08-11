using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave.Editor
{
    public enum EchoSaveSetupDisposition
    {
        Create = 0,
        Update = 1,
        NoChanges = 2,
        Rejected = 3
    }

    public enum EchoSaveSetupMessageSeverity
    {
        Blocker = 0,
        Advisory = 1
    }

    public sealed class EchoSaveSetupMessage
    {
        public EchoSaveSetupMessage(
            string code,
            EchoSaveSetupMessageSeverity severity,
            string message)
        {
            Code = code ?? string.Empty;
            Severity = severity;
            Message = message ?? string.Empty;
        }

        public string Code { get; }

        public EchoSaveSetupMessageSeverity Severity { get; }

        public string Message { get; }
    }

    public sealed class EchoSaveSetupChange
    {
        public EchoSaveSetupChange(
            string propertyName,
            string before,
            string after)
        {
            PropertyName = propertyName ?? string.Empty;
            Before = before ?? string.Empty;
            After = after ?? string.Empty;
        }

        public string PropertyName { get; }

        public string Before { get; }

        public string After { get; }
    }

    public sealed class EchoSaveSetupRequest
    {
        public EchoSaveSetupRequest(
            string targetAssetPath,
            string storageRootDirectoryName,
            SaveSlotPolicyMode slotPolicyMode,
            int fixedSlotCount,
            int configuredSlotLimit,
            int profileSafetyLimit)
            : this(
                null,
                targetAssetPath,
                storageRootDirectoryName,
                slotPolicyMode,
                fixedSlotCount,
                configuredSlotLimit,
                profileSafetyLimit,
                SaveRetentionPolicy.DefaultTotalGenerations,
                EchoSaveConfiguration.DefaultSerializerProviderId,
                EchoSaveConfiguration.DefaultStorageProviderId,
                SaveLimitPolicy.DefaultCatalogScanLimit,
                SaveLimitPolicy.DefaultRetentionDiscoveryLimit,
                SaveLimitPolicy.DefaultRecoveryDiscoveryLimit,
                EchoSaveRecoveryPolicyMode.ManualOnly,
                Array.Empty<SaveSlotTemplate>())
        {
        }

        public EchoSaveSetupRequest(
            EchoSaveConfiguration existingConfiguration,
            string targetAssetPath,
            string storageRootDirectoryName,
            SaveSlotPolicyMode slotPolicyMode,
            int fixedSlotCount,
            int configuredSlotLimit,
            int profileSafetyLimit,
            int maxTotalGenerations,
            string serializerProviderId,
            string storageProviderId,
            int catalogScanLimit,
            int retentionDiscoveryLimit,
            int recoveryDiscoveryLimit,
            EchoSaveRecoveryPolicyMode recoveryPolicyMode,
            IReadOnlyList<SaveSlotTemplate> fixedSlotTemplates)
        {
            ExistingConfiguration = existingConfiguration;
            TargetAssetPath = targetAssetPath ?? string.Empty;
            StorageRootDirectoryName =
                storageRootDirectoryName ?? string.Empty;
            SlotPolicyMode = slotPolicyMode;
            FixedSlotCount = fixedSlotCount;
            ConfiguredSlotLimit = configuredSlotLimit;
            ProfileSafetyLimit = profileSafetyLimit;
            MaxTotalGenerations = maxTotalGenerations;
            SerializerProviderId =
                serializerProviderId ?? string.Empty;
            StorageProviderId =
                storageProviderId ?? string.Empty;
            CatalogScanLimit = catalogScanLimit;
            RetentionDiscoveryLimit =
                retentionDiscoveryLimit;
            RecoveryDiscoveryLimit =
                recoveryDiscoveryLimit;
            RecoveryPolicyMode =
                recoveryPolicyMode;
            FixedSlotTemplates =
                fixedSlotTemplates ??
                Array.Empty<SaveSlotTemplate>();
        }

        public EchoSaveConfiguration ExistingConfiguration { get; }

        public string TargetAssetPath { get; }

        public string StorageRootDirectoryName { get; }

        public SaveSlotPolicyMode SlotPolicyMode { get; }

        public int FixedSlotCount { get; }

        public int ConfiguredSlotLimit { get; }

        public int ProfileSafetyLimit { get; }

        public int MaxTotalGenerations { get; }

        public string SerializerProviderId { get; }

        public string StorageProviderId { get; }

        public int CatalogScanLimit { get; }

        public int RetentionDiscoveryLimit { get; }

        public int RecoveryDiscoveryLimit { get; }

        public EchoSaveRecoveryPolicyMode RecoveryPolicyMode { get; }

        public IReadOnlyList<SaveSlotTemplate> FixedSlotTemplates { get; }

        public bool IsEdit =>
            ExistingConfiguration != null;
    }

    public sealed class EchoSaveSetupPlan
    {
        public EchoSaveSetupPlan(
            EchoSaveSetupRequest request,
            string requestFingerprint,
            string targetStateFingerprint,
            string normalizedAssetPath,
            string normalizedStorageRoot,
            int sourceSchemaVersion,
            int schemaVersion,
            SaveSlotPolicyMode slotPolicyMode,
            int effectiveCapacity,
            bool destinationAvailable,
            EchoSaveSetupDisposition disposition,
            IReadOnlyList<string> assetsToCreate,
            IReadOnlyList<EchoSaveSetupChange> changes,
            IReadOnlyList<EchoSaveSetupMessage> messages)
        {
            Request = request;
            RequestFingerprint =
                requestFingerprint ?? string.Empty;
            TargetStateFingerprint =
                targetStateFingerprint ?? string.Empty;
            NormalizedAssetPath =
                normalizedAssetPath ?? string.Empty;
            NormalizedStorageRoot =
                normalizedStorageRoot ?? string.Empty;
            SourceSchemaVersion = sourceSchemaVersion;
            SchemaVersion = schemaVersion;
            SlotPolicyMode = slotPolicyMode;
            EffectiveCapacity = effectiveCapacity;
            DestinationAvailable = destinationAvailable;
            Disposition = disposition;
            AssetsToCreate =
                assetsToCreate ??
                Array.Empty<string>();
            Changes =
                changes ??
                Array.Empty<EchoSaveSetupChange>();
            Messages =
                messages ??
                Array.Empty<EchoSaveSetupMessage>();
        }

        public EchoSaveSetupRequest Request { get; }

        public string RequestFingerprint { get; }

        public string TargetStateFingerprint { get; }

        public string NormalizedAssetPath { get; }

        public string NormalizedStorageRoot { get; }

        public int SourceSchemaVersion { get; }

        public int SchemaVersion { get; }

        public SaveSlotPolicyMode SlotPolicyMode { get; }

        public int EffectiveCapacity { get; }

        public bool DestinationAvailable { get; }

        public EchoSaveSetupDisposition Disposition { get; }

        public IReadOnlyList<string> AssetsToCreate { get; }

        public IReadOnlyList<EchoSaveSetupChange> Changes { get; }

        public IReadOnlyList<EchoSaveSetupMessage> Messages { get; }

        public bool CanApply =>
            Disposition == EchoSaveSetupDisposition.Create ||
            Disposition == EchoSaveSetupDisposition.Update;
    }

    public enum EchoSaveSetupResultStatus
    {
        Created = 0,
        Updated = 1,
        NoChanges = 2,
        Rejected = 3,
        Failed = 4
    }

    public sealed class EchoSaveSetupResult
    {
        public EchoSaveSetupResult(
            EchoSaveSetupResultStatus status,
            string assetPath,
            EchoSaveConfiguration configuration,
            string message)
        {
            Status = status;
            AssetPath = assetPath ?? string.Empty;
            Configuration = configuration;
            Message = message ?? string.Empty;
        }

        public EchoSaveSetupResultStatus Status { get; }

        public string AssetPath { get; }

        public EchoSaveConfiguration Configuration { get; }

        public EchoSaveConfiguration CreatedConfiguration =>
            Status == EchoSaveSetupResultStatus.Created
                ? Configuration
                : null;

        public string Message { get; }
    }

    public sealed class EchoSaveRootRepairPlan
    {
        public EchoSaveRootRepairPlan(
            EchoSaveRoot root,
            EchoSaveConfiguration targetConfiguration,
            string stateFingerprint,
            EchoSaveSetupDisposition disposition,
            IReadOnlyList<EchoSaveSetupChange> changes,
            IReadOnlyList<EchoSaveSetupMessage> messages)
        {
            Root = root;
            TargetConfiguration = targetConfiguration;
            StateFingerprint =
                stateFingerprint ?? string.Empty;
            Disposition = disposition;
            Changes =
                changes ??
                Array.Empty<EchoSaveSetupChange>();
            Messages =
                messages ??
                Array.Empty<EchoSaveSetupMessage>();
        }

        public EchoSaveRoot Root { get; }

        public EchoSaveConfiguration TargetConfiguration { get; }

        public string StateFingerprint { get; }

        public EchoSaveSetupDisposition Disposition { get; }

        public IReadOnlyList<EchoSaveSetupChange> Changes { get; }

        public IReadOnlyList<EchoSaveSetupMessage> Messages { get; }

        public bool CanApply =>
            Disposition == EchoSaveSetupDisposition.Update;
    }

    public sealed class EchoSaveRootRepairResult
    {
        public EchoSaveRootRepairResult(
            EchoSaveSetupResultStatus status,
            EchoSaveRoot root,
            string message)
        {
            Status = status;
            Root = root;
            Message = message ?? string.Empty;
        }

        public EchoSaveSetupResultStatus Status { get; }

        public EchoSaveRoot Root { get; }

        public string Message { get; }
    }
}
