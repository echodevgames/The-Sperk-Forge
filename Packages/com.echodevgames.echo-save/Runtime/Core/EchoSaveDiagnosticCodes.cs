
namespace EchoDevGames.EchoSave
{
    internal static class EchoSaveDiagnosticCodes
    {
        internal const string DuplicateRoot =
            "ESV-LIFE-001";

        internal const string InvalidLifecycle =
            "ESV-LIFE-002";

        internal const string AuthorityUnavailable =
            "ESV-LIFE-003";

        internal const string MissingOrInvalidConfiguration =
            "ESV-CFG-001";

        internal const string StorageInvalidPath =
            "ESV-STORAGE-001";

        internal const string StorageInitializationFailed =
            "ESV-STORAGE-002";

        internal const string StorageNotFound =
            "ESV-STORAGE-003";

        internal const string StorageConflict =
            "ESV-STORAGE-004";

        internal const string StorageIoFailure =
            "ESV-STORAGE-005";

        internal const string StorageNotReady =
            "ESV-STORAGE-006";

        internal const string StorageInvalidData =
            "ESV-STORAGE-007";

        internal const string SerializerInvalidRequest =
            "ESV-SERIAL-001";

        internal const string SerializerMalformedData =
            "ESV-SERIAL-002";

        internal const string SerializerUnsupportedDocumentVersion =
            "ESV-SERIAL-003";

        internal const string SerializerDuplicateProvider =
            "ESV-SERIAL-004";

        internal const string SerializerProviderNotFound =
            "ESV-SERIAL-005";

        internal const string SerializerFailure =
            "ESV-SERIAL-006";

        internal const string SerializerInvalidDocument =
            "ESV-SERIAL-007";

        internal const string IntegrityInvalidRequest =
            "ESV-INTEGRITY-001";

        internal const string IntegrityMismatch =
            "ESV-INTEGRITY-002";

        internal const string IntegrityFailure =
            "ESV-INTEGRITY-003";

        internal const string DocumentInvalidIdentity =
            "ESV-DOC-001";

        internal const string DocumentIdentityMismatch =
            "ESV-DOC-002";

        internal const string DocumentPayloadLengthMismatch =
            "ESV-DOC-003";

        internal const string DocumentUnsupportedIntegrityProvider =
            "ESV-DOC-004";

        internal const string DocumentInventoryMismatch =
            "ESV-DOC-005";

        internal const string DocumentInvalidHead =
            "ESV-DOC-006";

        internal const string StoragePublicationUnsupported =
            "ESV-STORAGE-008";

        internal const string StoragePublicationFailed =
            "ESV-STORAGE-009";

        internal const string PublicationInvalidRequest =
            "ESV-PUBLISH-001";

        internal const string PublicationBackendUnsupported =
            "ESV-PUBLISH-002";

        internal const string PublicationExistingHeadInvalid =
            "ESV-PUBLISH-003";

        internal const string PublicationSerializationFailed =
            "ESV-PUBLISH-004";

        internal const string PublicationCandidateWriteFailed =
            "ESV-PUBLISH-005";

        internal const string PublicationCandidateVerificationFailed =
            "ESV-PUBLISH-006";

        internal const string PublicationGenerationFailed =
            "ESV-PUBLISH-007";

        internal const string PublicationHeadFailed =
            "ESV-PUBLISH-008";

        internal const string ParticipantInvalidId =
            "ESV-PART-001";

        internal const string ParticipantInvalidDescriptor =
            "ESV-PART-002";

        internal const string ParticipantDuplicateId =
            "ESV-PART-003";

        internal const string ParticipantRequiredMissing =
            "ESV-PART-004";

        internal const string ParticipantAliasCollision =
            "ESV-PART-005";

        internal const string ParticipantCaptureFailed =
            "ESV-PART-006";

        internal const string ParticipantApplyFailed =
            "ESV-PART-007";

        internal const string ParticipantNotFound =
            "ESV-PART-008";

        internal const string ParticipantCaptureInvalidRequest =
            "ESV-PART-009";

        internal const string ParticipantCaptureTypeUnavailable =
            "ESV-PART-010";

        internal const string ParticipantCaptureTypeMismatch =
            "ESV-PART-011";

        internal const string ParticipantCaptureSerializerUnavailable =
            "ESV-PART-012";

        internal const string ParticipantCaptureSerializationFailed =
            "ESV-PART-013";

        internal const string ParticipantCaptureIntegrityFailed =
            "ESV-PART-014";

        internal const string ParticipantCaptureRegistryChanged =
            "ESV-PART-015";

        internal const string PublicationParticipantBatchInvalid =
            "ESV-PUBLISH-009";

        internal const string CurrentReadInvalidRequest =
            "ESV-READ-001";

        internal const string CurrentReadHeadUnavailable =
            "ESV-READ-002";

        internal const string CurrentReadHeadInvalid =
            "ESV-READ-003";

        internal const string CurrentReadGenerationUnavailable =
            "ESV-READ-004";

        internal const string CurrentReadGenerationInvalid =
            "ESV-READ-005";

        internal const string UnknownPayloadInvalid =
            "ESV-UNKNOWN-001";

        internal const string UnknownPayloadDuplicate =
            "ESV-UNKNOWN-002";

        internal const string UnknownPayloadLimitExceeded =
            "ESV-UNKNOWN-003";

        internal const string CarryForwardInvalidRequest =
            "ESV-CARRY-001";

        internal const string CarryForwardProvenanceMissing =
            "ESV-CARRY-002";

        internal const string CarryForwardSlotMismatch =
            "ESV-CARRY-003";

        internal const string CarryForwardSourceUnavailable =
            "ESV-CARRY-004";

        internal const string CarryForwardSourceInvalid =
            "ESV-CARRY-005";

        internal const string CarryForwardSourceStale =
            "ESV-CARRY-006";

        internal const string CarryForwardOwnershipCollision =
            "ESV-CARRY-007";

        internal const string CarryForwardMergeInvalid =
            "ESV-CARRY-008";

        internal const string CarryForwardPublicationFailed =
            "ESV-CARRY-009";

        internal const string ParticipantPreparationInvalidRequest =
            "ESV-PREP-001";

        internal const string ParticipantPreparationRegistryChanged =
            "ESV-PREP-002";

        internal const string ParticipantPreparationTypeUnavailable =
            "ESV-PREP-003";

        internal const string ParticipantPreparationMigrationRequired =
            "ESV-PREP-004";

        internal const string ParticipantPreparationNewerSchema =
            "ESV-PREP-005";

        internal const string ParticipantPreparationSerializerUnavailable =
            "ESV-PREP-006";

        internal const string ParticipantPreparationDeserializationFailed =
            "ESV-PREP-007";

        internal const string ParticipantPreparationStateInvalid =
            "ESV-PREP-008";

        internal const string ParticipantPreparationDuplicateOwner =
            "ESV-PREP-009";

        internal const string ParticipantMigrationInvalidStep =
            "ESV-MIGRATE-001";

        internal const string ParticipantMigrationDuplicateId =
            "ESV-MIGRATE-002";

        internal const string ParticipantMigrationDuplicateEdge =
            "ESV-MIGRATE-003";

        internal const string ParticipantMigrationInvalidRequest =
            "ESV-MIGRATE-004";

        internal const string ParticipantMigrationChainMissing =
            "ESV-MIGRATE-005";

        internal const string ParticipantMigrationStepLimitExceeded =
            "ESV-MIGRATE-006";

        internal const string ParticipantMigrationRegistryChanged =
            "ESV-MIGRATE-007";

        internal const string ParticipantMigrationStepFailed =
            "ESV-MIGRATE-008";

        internal const string ParticipantMigrationInvalidOutput =
            "ESV-MIGRATE-009";

        internal const string PreparedLoadInvalidRequest =
            "ESV-PLOAD-001";

        internal const string PreparedLoadSourceMismatch =
            "ESV-PLOAD-002";

        internal const string PreparedLoadUnknownProvenanceMismatch =
            "ESV-PLOAD-003";

        internal const string PreparedLoadCountLimitExceeded =
            "ESV-PLOAD-004";

        internal const string PreparedLoadByteLimitExceeded =
            "ESV-PLOAD-005";

        internal const string PreparedLoadOwnerUnavailable =
            "ESV-PLOAD-006";

        internal const string PreparedApplyInvalidRequest =
            "ESV-APPLY-001";

        internal const string PreparedApplyHandleUnavailable =
            "ESV-APPLY-002";

        internal const string PreparedApplyParticipantUnavailable =
            "ESV-APPLY-003";

        internal const string PreparedApplyStateIncompatible =
            "ESV-APPLY-004";

        internal const string PreparedApplyDuplicateParticipant =
            "ESV-APPLY-005";

        internal const string PreparedApplyMissingPayloadBlocked =
            "ESV-APPLY-006";

        internal const string PreparedApplyDefaultCapabilityMissing =
            "ESV-APPLY-007";

        internal const string PreparedApplyRegistryChanged =
            "ESV-APPLY-008";

        internal const string PreparedApplyParticipantFailed =
            "ESV-APPLY-009";

        internal const string PreparedApplyParticipantException =
            "ESV-APPLY-010";

        internal const string StorageDiscoveryInvalidRequest =
            "ESV-STORAGE-010";

        internal const string StorageDiscoveryFailed =
            "ESV-STORAGE-011";

        internal const string StorageDiscoveryLimitExceeded =
            "ESV-STORAGE-012";

        internal const string CatalogDiscoveryUnavailable =
            "ESV-CATALOG-001";

        internal const string CatalogDiscoveryFailed =
            "ESV-CATALOG-002";

        internal const string CatalogScanLimitExceeded =
            "ESV-CATALOG-003";

        internal const string CatalogHeadMissing =
            "ESV-CATALOG-004";

        internal const string CatalogHeadInvalid =
            "ESV-CATALOG-005";

        internal const string CatalogHeadUnsupported =
            "ESV-CATALOG-006";

        internal const string CatalogManifestMissing =
            "ESV-CATALOG-007";

        internal const string CatalogManifestInvalid =
            "ESV-CATALOG-008";

        internal const string CatalogManifestUnsupported =
            "ESV-CATALOG-009";

        internal const string CatalogIdentityMismatch =
            "ESV-CATALOG-010";

        internal const string CatalogBackendReadFailed =
            "ESV-CATALOG-011";

        internal const string CatalogActiveSlotRejected =
            "ESV-CATALOG-012";

        internal const string SlotCreateInvalidRequest =
            "ESV-SLOT-001";

        internal const string SlotCreateCatalogUnavailable =
            "ESV-SLOT-002";

        internal const string SlotCreateCapacityReached =
            "ESV-SLOT-003";

        internal const string SlotCreateIdGenerationFailed =
            "ESV-SLOT-004";

        internal const string SlotCreateCollisionLimitExceeded =
            "ESV-SLOT-005";

        internal const string SlotCreatePublicationFailed =
            "ESV-SLOT-006";

        internal const string SlotCreateCatalogReconciliationFailed =
            "ESV-SLOT-007";

        internal const string SlotCreateExistingHead =
            "ESV-SLOT-008";

        internal const string ManualSaveInvalidRequest =
            "ESV-SAVE-001";

        internal const string ManualSaveCatalogUnavailable =
            "ESV-SAVE-002";

        internal const string ManualSaveNoActiveSlot =
            "ESV-SAVE-003";

        internal const string ManualSaveActiveSlotUnavailable =
            "ESV-SAVE-004";

        internal const string ManualSaveSourceReadFailed =
            "ESV-SAVE-005";

        internal const string ManualSaveSourceChanged =
            "ESV-SAVE-006";

        internal const string ManualSaveCaptureFailed =
            "ESV-SAVE-007";

        internal const string ManualSaveCarryForwardFailed =
            "ESV-SAVE-008";

        internal const string ManualSaveStaleSource =
            "ESV-SAVE-009";

        internal const string ManualSavePublicationFailed =
            "ESV-SAVE-010";

        internal const string ManualSaveCatalogReconciliationFailed =
            "ESV-SAVE-011";

        internal const string ManualSaveCanceled =
            "ESV-SAVE-012";

        internal const string PublicSaveInvalidRequest =
            "ESV-SAVE-013";

        internal const string PublicSaveServiceNotReady =
            "ESV-SAVE-014";

        internal const string PublicSaveAdmissionClosed =
            "ESV-SAVE-015";

        internal const string PublicSaveBusy =
            "ESV-SAVE-016";

        internal const string PublicSaveCancellationTooLate =
            "ESV-SAVE-017";

        internal const string PublicSaveShutdownPending =
            "ESV-SAVE-018";

        internal const string AutosaveInvalidRequest =
            "ESV-AUTO-001";

        internal const string AutosaveServiceNotReady =
            "ESV-AUTO-002";

        internal const string AutosaveAdmissionClosed =
            "ESV-AUTO-003";

        internal const string AutosaveNoActiveSlot =
            "ESV-AUTO-004";

        internal const string AutosaveCanceled =
            "ESV-AUTO-005";

        internal const string AutosavePending =
            "ESV-AUTO-006";

        internal const string AutosaveCoalesced =
            "ESV-AUTO-007";

        internal const string AutosaveSuperseded =
            "ESV-AUTO-008";

        internal const string AutosaveDiscarded =
            "ESV-AUTO-009";

        internal const string AutosaveExecuted =
            "ESV-AUTO-010";

        internal const string RetentionInvalidPolicy =
            "ESV-RET-001";

        internal const string RetentionUnsupportedStorage =
            "ESV-RET-002";

        internal const string RetentionUntrustworthy =
            "ESV-RET-003";

        internal const string RetentionDeleteFailed =
            "ESV-RET-004";
    }
}
