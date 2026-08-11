
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M4-07 read-only recovery-candidate planner.
    ///
    /// It performs bounded provider-neutral reads only. It does not own
    /// recovery execution, head repair, catalog mutation, quarantine,
    /// participant callbacks, migration, scene lifetime, or DDOL.
    /// </summary>
    internal sealed class SaveRecoveryPlanBuilder :
        ISaveRecoveryPlanBuilder
    {
        internal const int DefaultDiscoveryLimit =
            512;

        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly IIntegrityProvider integrity;
        private readonly int discoveryLimit;

        internal SaveRecoveryPlanBuilder(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            IIntegrityProvider integrity,
            int discoveryLimit =
                DefaultDiscoveryLimit)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            this.serializer =
                serializer ??
                throw new ArgumentNullException(
                    nameof(serializer));

            this.integrity =
                integrity ??
                throw new ArgumentNullException(
                    nameof(integrity));

            if (discoveryLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(discoveryLimit));
            }

            this.discoveryLimit =
                discoveryLimit;
        }

        public SaveRecoveryPlan Build(
            SaveSlotId slotId)
        {
            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .RecoveryInvalidRequest,
                    "Chronicle recovery planning requires one valid technical slot identity.",
                    slotId);
            }

            if (!(storage is
                    ISaveStorageDiscoveryBackend discovery))
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.DiscoveryFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryDiscoveryFailed,
                    "The active Chronicle storage provider does not expose bounded child-directory discovery.",
                    validatedSlot);
            }

            if (!TryReadObservedHead(
                    validatedSlot,
                    out ObservedHead observedHead,
                    out SaveRecoveryPlan headFailure))
            {
                return headFailure;
            }

            if (!SaveStorageKey.TryCreate(
                    "slots/" +
                    validatedSlot.Value +
                    "/generations",
                    out SaveStorageKey generationsRoot)
                    .Succeeded)
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .RecoveryInvalidRequest,
                    "Chronicle recovery planning could not construct the validated generation-discovery key.",
                    validatedSlot);
            }

            SaveStorageDiscoveryResult discovered =
                discovery.DiscoverChildDirectories(
                    generationsRoot,
                    discoveryLimit);

            if (!discovered.Succeeded)
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.DiscoveryFailed,
                    string.IsNullOrEmpty(
                        discovered.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .RecoveryDiscoveryFailed
                        : discovered.DiagnosticCode,
                    "Chronicle recovery planning could not establish one bounded generation-directory snapshot. " +
                    discovered.Message,
                    validatedSlot);
            }

            string[] childNames =
                CopyAndSort(
                    discovered.ChildNames);

            List<GenerationObservation> observations =
                new List<GenerationObservation>();

            int ignoredNonCanonical =
                0;

            for (int i = 0;
                 i < childNames.Length;
                 i++)
            {
                string childName =
                    childNames[i];

                if (!SaveGenerationId.TryParse(
                        childName,
                        out SaveGenerationId generationId))
                {
                    ignoredNonCanonical++;
                    continue;
                }

                GenerationObservation observation =
                    InspectGeneration(
                        validatedSlot,
                        generationId);

                if (observation.FatalInspectionFailure)
                {
                    return SaveRecoveryPlan.Failure(
                        SaveRecoveryPlanStatus.InspectionFailed,
                        EchoSaveDiagnosticCodes
                            .RecoveryInspectionFailed,
                        observation.Message,
                        validatedSlot);
                }

                observations.Add(
                    observation);
            }

            List<GenerationObservation> verified =
                new List<GenerationObservation>();

            int rejectedCanonical =
                0;

            for (int i = 0;
                 i < observations.Count;
                 i++)
            {
                if (observations[i].Verified)
                {
                    verified.Add(
                        observations[i]);
                }
                else
                {
                    rejectedCanonical++;
                }
            }

            verified.Sort(
                CompareNewestFirst);

            SaveRecoveryHeadCondition finalHeadCondition =
                observedHead.Condition;

            string observedDiagnostic =
                observedHead.DiagnosticCode;

            if (observedHead.HasCurrentGeneration)
            {
                GenerationObservation current =
                    FindObservation(
                        observations,
                        observedHead.CurrentGenerationId);

                if (current == null)
                {
                    finalHeadCondition =
                        SaveRecoveryHeadCondition
                            .CurrentMissing;

                    observedDiagnostic =
                        EchoSaveDiagnosticCodes
                            .RecoveryCurrentMissing;
                }
                else if (!current.Verified)
                {
                    finalHeadCondition =
                        SaveRecoveryHeadCondition
                            .CurrentInvalid;

                    observedDiagnostic =
                        EchoSaveDiagnosticCodes
                            .RecoveryCurrentInvalid;
                }
                else
                {
                    finalHeadCondition =
                        SaveRecoveryHeadCondition
                            .Healthy;

                    observedDiagnostic =
                        string.Empty;
                }
            }

            SaveRecoveryCandidate[] candidates =
                new SaveRecoveryCandidate[
                    verified.Count];

            for (int i = 0;
                 i < verified.Count;
                 i++)
            {
                candidates[i] =
                    verified[i].Candidate;
            }

            bool recoveryRequired =
                finalHeadCondition !=
                    SaveRecoveryHeadCondition.Healthy;

            bool hasPreferred =
                recoveryRequired &&
                candidates.Length > 0;

            SaveRecoveryCandidate preferred =
                hasPreferred
                    ? candidates[0]
                    : default;

            if (!TryBuildSourceProvenance(
                    validatedSlot,
                    finalHeadCondition,
                    observedHead,
                    observations,
                    hasPreferred
                        ? preferred.GenerationId
                        : default,
                    hasPreferred,
                    out string sourceFingerprint))
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.InspectionFailed,
                    EchoSaveDiagnosticCodes
                        .RecoveryInspectionFailed,
                    "Chronicle recovery planning could not fingerprint the bounded observed source evidence.",
                    validatedSlot);
            }

            SaveRecoveryPlanStatus status;
            string diagnosticCode;
            string message;

            if (!recoveryRequired)
            {
                status =
                    SaveRecoveryPlanStatus
                        .RecoveryNotRequired;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .RecoveryNotRequired;

                message =
                    "The Chronicle head and current generation are fully verified; recovery is not required.";
            }
            else if (hasPreferred)
            {
                status =
                    SaveRecoveryPlanStatus
                        .RecoveryAvailable;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .RecoveryAvailable;

                message =
                    "Chronicle recovery planning found at least one fully verified committed generation and selected the newest valid candidate without mutating storage.";
            }
            else
            {
                status =
                    SaveRecoveryPlanStatus
                        .NoValidCandidate;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .RecoveryNoValidCandidate;

                message =
                    "Chronicle recovery is required, but no fully verified committed recovery candidate is available. Source evidence was preserved.";
            }

            return new SaveRecoveryPlan(
                status,
                diagnosticCode,
                message,
                validatedSlot,
                finalHeadCondition,
                observedDiagnostic,
                observedHead.CurrentGenerationId,
                observedHead.HasCurrentGeneration,
                candidates,
                preferred,
                hasPreferred,
                rejectedCanonical,
                ignoredNonCanonical,
                sourceFingerprint);
        }

        private bool TryReadObservedHead(
            SaveSlotId slotId,
            out ObservedHead observed,
            out SaveRecoveryPlan failure)
        {
            observed =
                default;

            failure =
                null;

            if (!SaveStorageKey.TryCreate(
                    "slots/" +
                    slotId.Value +
                    "/head.json",
                    out SaveStorageKey headKey)
                    .Succeeded)
            {
                failure =
                    SaveRecoveryPlan.Failure(
                        SaveRecoveryPlanStatus.InvalidRequest,
                        EchoSaveDiagnosticCodes
                            .RecoveryInvalidRequest,
                        "Chronicle recovery planning could not construct the validated head key.",
                        slotId);

                return false;
            }

            SaveStorageReadResult read =
                storage.Read(
                    headKey);

            if (read.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                observed =
                    new ObservedHead(
                        SaveRecoveryHeadCondition.Missing,
                        EchoSaveDiagnosticCodes
                            .RecoveryHeadMissing,
                        default,
                        false,
                        "missing");

                return true;
            }

            if (!read.Succeeded)
            {
                observed =
                    new ObservedHead(
                        SaveRecoveryHeadCondition.Unreadable,
                        EchoSaveDiagnosticCodes
                            .RecoveryHeadUnreadable,
                        default,
                        false,
                        "read:" +
                        read.Result.Status +
                        ":" +
                        read.Result.DiagnosticCode);

                return true;
            }

            if (!TryFingerprintBytes(
                    read.Data,
                    out string headBytesFingerprint))
            {
                failure =
                    SaveRecoveryPlan.Failure(
                        SaveRecoveryPlanStatus.InspectionFailed,
                        EchoSaveDiagnosticCodes
                            .RecoveryInspectionFailed,
                        "Chronicle recovery planning could not fingerprint the observed head bytes.",
                        slotId);

                return false;
            }

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head);

            if (!deserialized.Succeeded ||
                !SaveCommitDocumentValidator
                    .ValidateHead(
                        head)
                    .Succeeded ||
                !SaveSlotId.TryParse(
                    head.slotId,
                    out SaveSlotId headSlot) ||
                headSlot !=
                    slotId ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId current))
            {
                observed =
                    new ObservedHead(
                        SaveRecoveryHeadCondition.Invalid,
                        EchoSaveDiagnosticCodes
                            .RecoveryHeadInvalid,
                        default,
                        false,
                        "bytes:" +
                        headBytesFingerprint);

                return true;
            }

            observed =
                new ObservedHead(
                    SaveRecoveryHeadCondition.Healthy,
                    string.Empty,
                    current,
                    true,
                    "bytes:" +
                    headBytesFingerprint);

            return true;
        }

        private GenerationObservation InspectGeneration(
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    slotId,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return GenerationObservation.Fatal(
                    generationId,
                    "Chronicle recovery planning could not construct validated keys for one canonical generation.");
            }

            SaveStorageReadResult manifestRead =
                storage.Read(
                    keys.GenerationManifest);

            SaveStorageReadResult payloadRead =
                storage.Read(
                    keys.GenerationPayload);

            if (!TryFingerprintRead(
                    manifestRead,
                    out string manifestEvidence) ||
                !TryFingerprintRead(
                    payloadRead,
                    out string payloadEvidence))
            {
                return GenerationObservation.Fatal(
                    generationId,
                    "Chronicle recovery planning could not fingerprint one bounded generation observation.");
            }

            string evidence =
                generationId.Value +
                "|m=" +
                manifestEvidence +
                "|p=" +
                payloadEvidence;

            if (!manifestRead.Succeeded ||
                !payloadRead.Succeeded)
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    "One committed-generation document is missing or unreadable.");
            }

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest);

            SaveSerializerResult payloadDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        payloadRead.Data),
                    out SavePayloadDocument payload);

            if (!manifestDeserialize.Succeeded ||
                !payloadDeserialize.Succeeded ||
                manifest == null ||
                payload == null)
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    "One committed-generation document is malformed or unsupported by the active serializer.");
            }

            if (!string.Equals(
                    manifest.slotId,
                    slotId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.slotId,
                    slotId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    generationId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    payload.generationId,
                    generationId.Value,
                    StringComparison.Ordinal) ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    "The generation is not one committed matching slot/generation identity.");
            }

            SaveDocumentValidationResult documentValidation =
                SaveCommitDocumentValidator
                    .ValidateManifestAndPayload(
                        manifest,
                        payload,
                        payloadRead.Data,
                        integrity);

            if (!documentValidation.Succeeded)
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    documentValidation.Message);
            }

            SaveDocumentValidationResult entryValidation =
                SaveParticipantPublicationBatchValidator
                    .ValidateStoredEntries(
                        payload.entries ??
                            Array.Empty<SavePayloadEntry>(),
                        manifest.payloadEntries ??
                            Array.Empty<SavePayloadInventoryEntry>(),
                        integrity);

            if (!entryValidation.Succeeded)
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    entryValidation.Message);
            }

            if (!DateTimeOffset.TryParse(
                    manifest.updatedUtc,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTimeOffset technicalTimestamp))
            {
                return GenerationObservation.Rejected(
                    generationId,
                    evidence,
                    "The generation technical timestamp is invalid.");
            }

            SaveRecoveryCandidate candidate =
                new SaveRecoveryCandidate(
                    generationId,
                    manifest.updatedUtc,
                    manifest.saveKind,
                    manifest.projectId,
                    manifest.projectVersion,
                    manifest.buildId);

            return GenerationObservation.VerifiedCandidate(
                generationId,
                evidence,
                technicalTimestamp,
                candidate);
        }

        private bool TryBuildSourceProvenance(
            SaveSlotId slotId,
            SaveRecoveryHeadCondition headCondition,
            ObservedHead observedHead,
            List<GenerationObservation> observations,
            SaveGenerationId preferredGeneration,
            bool hasPreferredGeneration,
            out string fingerprint)
        {
            observations.Sort(
                CompareObservationIdentity);

            StringBuilder source =
                new StringBuilder();

            source.Append(
                "slot=")
                .Append(
                    slotId.Value)
                .Append('\n');

            source.Append(
                "headCondition=")
                .Append(
                    (int)headCondition)
                .Append('\n');

            source.Append(
                "headEvidence=")
                .Append(
                    observedHead.Evidence)
                .Append('\n');

            source.Append(
                "current=")
                .Append(
                    observedHead.HasCurrentGeneration
                        ? observedHead
                            .CurrentGenerationId
                            .Value
                        : string.Empty)
                .Append('\n');

            for (int i = 0;
                 i < observations.Count;
                 i++)
            {
                source.Append(
                    observations[i].Evidence)
                    .Append('\n');
            }

            source.Append(
                "preferred=")
                .Append(
                    hasPreferredGeneration
                        ? preferredGeneration.Value
                        : string.Empty);

            return TryFingerprintBytes(
                Encoding.UTF8.GetBytes(
                    source.ToString()),
                out fingerprint);
        }

        private bool TryFingerprintRead(
            SaveStorageReadResult read,
            out string fingerprint)
        {
            if (!read.Succeeded)
            {
                fingerprint =
                    "read:" +
                    read.Result.Status +
                    ":" +
                    read.Result.DiagnosticCode;

                return true;
            }

            if (!TryFingerprintBytes(
                    read.Data,
                    out string bytesFingerprint))
            {
                fingerprint =
                    string.Empty;

                return false;
            }

            fingerprint =
                "bytes:" +
                bytesFingerprint;

            return true;
        }

        private bool TryFingerprintBytes(
            byte[] bytes,
            out string fingerprint)
        {
            SaveIntegrityResult result =
                integrity.Calculate(
                    bytes ??
                        Array.Empty<byte>(),
                    out fingerprint);

            if (!result.Succeeded)
            {
                fingerprint =
                    string.Empty;

                return false;
            }

            return true;
        }

        private static string[] CopyAndSort(
            IReadOnlyList<string> source)
        {
            if (source == null ||
                source.Count == 0)
            {
                return Array.Empty<string>();
            }

            string[] copy =
                new string[
                    source.Count];

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                copy[i] =
                    source[i] ??
                    string.Empty;
            }

            Array.Sort(
                copy,
                StringComparer.Ordinal);

            return copy;
        }

        private static GenerationObservation FindObservation(
            List<GenerationObservation> observations,
            SaveGenerationId generationId)
        {
            for (int i = 0;
                 i < observations.Count;
                 i++)
            {
                if (observations[i]
                    .GenerationId ==
                    generationId)
                {
                    return observations[i];
                }
            }

            return null;
        }

        private static int CompareNewestFirst(
            GenerationObservation left,
            GenerationObservation right)
        {
            int timestamp =
                right.TechnicalTimestamp
                    .CompareTo(
                        left.TechnicalTimestamp);

            if (timestamp != 0)
            {
                return timestamp;
            }

            return string.Compare(
                right.GenerationId.Value,
                left.GenerationId.Value,
                StringComparison.Ordinal);
        }

        private static int CompareObservationIdentity(
            GenerationObservation left,
            GenerationObservation right) =>
            string.Compare(
                left.GenerationId.Value,
                right.GenerationId.Value,
                StringComparison.Ordinal);

        private readonly struct ObservedHead
        {
            internal ObservedHead(
                SaveRecoveryHeadCondition condition,
                string diagnosticCode,
                SaveGenerationId currentGenerationId,
                bool hasCurrentGeneration,
                string evidence)
            {
                Condition =
                    condition;

                DiagnosticCode =
                    diagnosticCode ?? string.Empty;

                CurrentGenerationId =
                    currentGenerationId;

                HasCurrentGeneration =
                    hasCurrentGeneration;

                Evidence =
                    evidence ?? string.Empty;
            }

            internal SaveRecoveryHeadCondition Condition { get; }

            internal string DiagnosticCode { get; }

            internal SaveGenerationId CurrentGenerationId { get; }

            internal bool HasCurrentGeneration { get; }

            internal string Evidence { get; }
        }

        private sealed class GenerationObservation
        {
            private GenerationObservation(
                SaveGenerationId generationId,
                string evidence,
                bool verified,
                bool fatalInspectionFailure,
                string message,
                DateTimeOffset technicalTimestamp,
                SaveRecoveryCandidate candidate)
            {
                GenerationId =
                    generationId;

                Evidence =
                    evidence ?? string.Empty;

                Verified =
                    verified;

                FatalInspectionFailure =
                    fatalInspectionFailure;

                Message =
                    message ?? string.Empty;

                TechnicalTimestamp =
                    technicalTimestamp;

                Candidate =
                    candidate;
            }

            internal SaveGenerationId GenerationId { get; }

            internal string Evidence { get; }

            internal bool Verified { get; }

            internal bool FatalInspectionFailure { get; }

            internal string Message { get; }

            internal DateTimeOffset TechnicalTimestamp { get; }

            internal SaveRecoveryCandidate Candidate { get; }

            internal static GenerationObservation VerifiedCandidate(
                SaveGenerationId generationId,
                string evidence,
                DateTimeOffset technicalTimestamp,
                SaveRecoveryCandidate candidate) =>
                new GenerationObservation(
                    generationId,
                    evidence,
                    true,
                    false,
                    string.Empty,
                    technicalTimestamp,
                    candidate);

            internal static GenerationObservation Rejected(
                SaveGenerationId generationId,
                string evidence,
                string message) =>
                new GenerationObservation(
                    generationId,
                    evidence,
                    false,
                    false,
                    message,
                    default,
                    default);

            internal static GenerationObservation Fatal(
                SaveGenerationId generationId,
                string message) =>
                new GenerationObservation(
                    generationId,
                    string.Empty,
                    false,
                    true,
                    message,
                    default,
                    default);
        }
    }
}
