
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
    }
}
