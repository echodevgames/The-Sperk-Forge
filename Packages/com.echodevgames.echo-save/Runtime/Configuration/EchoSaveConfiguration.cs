
using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-owned Chronicle configuration definition.
    ///
    /// The storage-root directory remains one safe relative segment. Schema 2
    /// additionally owns the project-authored slot policy resolved once by the
    /// Chronicle service for each successful runtime session.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EchoSaveConfiguration",
        menuName = "EchoDevGames/The Chronicle/Echo Save Configuration")]
    public sealed class EchoSaveConfiguration : ScriptableObject
    {
        public const int LegacySchemaVersion = 1;
        public const int CurrentSchemaVersion = 2;

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

        public bool IsCurrentSchema =>
            schemaVersion == CurrentSchemaVersion;

        /// <summary>
        /// Resolves the serialized configuration into one immutable runtime
        /// policy snapshot without mutating this asset.
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

            if (schemaVersion != CurrentSchemaVersion)
            {
                message =
                    $"EchoSaveConfiguration schema {schemaVersion} is unsupported. " +
                    $"Expected schema {LegacySchemaVersion} or {CurrentSchemaVersion}.";
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
                    CurrentSchemaVersion,
                    false);

            message = string.Empty;
            return true;
        }

        internal bool TryValidate(
            out string message)
        {
            if (!TryResolveSlotPolicy(
                    out _,
                    out message))
            {
                return false;
            }

            string value = StorageRootDirectoryName.Trim();
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
            for (int i = 0; i < value.Length; i++)
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
    }
}
