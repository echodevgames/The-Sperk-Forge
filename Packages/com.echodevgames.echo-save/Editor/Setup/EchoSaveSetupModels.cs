using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave.Editor
{
    public enum EchoSaveSetupDisposition
    {
        Create = 0,
        NoChanges = 1,
        Rejected = 2
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

    public sealed class EchoSaveSetupRequest
    {
        public EchoSaveSetupRequest(
            string targetAssetPath,
            string storageRootDirectoryName,
            SaveSlotPolicyMode slotPolicyMode,
            int fixedSlotCount,
            int configuredSlotLimit,
            int profileSafetyLimit)
        {
            TargetAssetPath = targetAssetPath ?? string.Empty;
            StorageRootDirectoryName =
                storageRootDirectoryName ?? string.Empty;
            SlotPolicyMode = slotPolicyMode;
            FixedSlotCount = fixedSlotCount;
            ConfiguredSlotLimit = configuredSlotLimit;
            ProfileSafetyLimit = profileSafetyLimit;
        }

        public string TargetAssetPath { get; }

        public string StorageRootDirectoryName { get; }

        public SaveSlotPolicyMode SlotPolicyMode { get; }

        public int FixedSlotCount { get; }

        public int ConfiguredSlotLimit { get; }

        public int ProfileSafetyLimit { get; }
    }

    public sealed class EchoSaveSetupPlan
    {
        public EchoSaveSetupPlan(
            EchoSaveSetupRequest request,
            string requestFingerprint,
            string normalizedAssetPath,
            string normalizedStorageRoot,
            int schemaVersion,
            SaveSlotPolicyMode slotPolicyMode,
            int effectiveCapacity,
            bool destinationAvailable,
            EchoSaveSetupDisposition disposition,
            IReadOnlyList<string> assetsToCreate,
            IReadOnlyList<EchoSaveSetupMessage> messages)
        {
            Request = request;
            RequestFingerprint = requestFingerprint ?? string.Empty;
            NormalizedAssetPath = normalizedAssetPath ?? string.Empty;
            NormalizedStorageRoot = normalizedStorageRoot ?? string.Empty;
            SchemaVersion = schemaVersion;
            SlotPolicyMode = slotPolicyMode;
            EffectiveCapacity = effectiveCapacity;
            DestinationAvailable = destinationAvailable;
            Disposition = disposition;
            AssetsToCreate =
                assetsToCreate ?? Array.Empty<string>();
            Messages =
                messages ?? Array.Empty<EchoSaveSetupMessage>();
        }

        public EchoSaveSetupRequest Request { get; }

        public string RequestFingerprint { get; }

        public string NormalizedAssetPath { get; }

        public string NormalizedStorageRoot { get; }

        public int SchemaVersion { get; }

        public SaveSlotPolicyMode SlotPolicyMode { get; }

        public int EffectiveCapacity { get; }

        public bool DestinationAvailable { get; }

        public EchoSaveSetupDisposition Disposition { get; }

        public IReadOnlyList<string> AssetsToCreate { get; }

        public IReadOnlyList<EchoSaveSetupMessage> Messages { get; }

        public bool CanApply =>
            Disposition == EchoSaveSetupDisposition.Create;
    }

    public enum EchoSaveSetupResultStatus
    {
        Created = 0,
        NoChanges = 1,
        Rejected = 2,
        Failed = 3
    }

    public sealed class EchoSaveSetupResult
    {
        public EchoSaveSetupResult(
            EchoSaveSetupResultStatus status,
            string assetPath,
            EchoSaveConfiguration createdConfiguration,
            string message)
        {
            Status = status;
            AssetPath = assetPath ?? string.Empty;
            CreatedConfiguration = createdConfiguration;
            Message = message ?? string.Empty;
        }

        public EchoSaveSetupResultStatus Status { get; }

        public string AssetPath { get; }

        public EchoSaveConfiguration CreatedConfiguration { get; }

        public string Message { get; }
    }
}
