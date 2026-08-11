
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M4-06 provider-neutral post-publication generation-retention engine.
    ///
    /// This coordinator never owns filesystem authority directly. It discovers
    /// through ISaveStorageDiscoveryBackend and deletes complete generation
    /// trees only through ISaveStorageTreeDeletionBackend.
    /// </summary>
    internal sealed class SaveGenerationRetentionCoordinator :
        ISaveGenerationRetentionExecutor
    {
        internal const int DefaultDiscoveryLimit =
            512;

        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly int discoveryLimit;

        internal SaveGenerationRetentionCoordinator(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
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

            if (discoveryLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(discoveryLimit));
            }

            this.discoveryLimit =
                discoveryLimit;
        }

        public SaveRetentionResult Apply(
            SaveSlotId slotId,
            SaveRetentionPolicy policy)
        {
            if (!policy.IsValid)
            {
                return Failure(
                    SaveRetentionStatus.InvalidPolicy,
                    EchoSaveDiagnosticCodes
                        .RetentionInvalidPolicy,
                    "Chronicle retention requires a total-generation bound between 2 and 256.");
            }

            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes
                        .RetentionUntrustworthy,
                    "Chronicle retention requires one valid technical slot identity.");
            }

            if (!(storage is
                    ISaveStorageDiscoveryBackend discovery))
            {
                return Failure(
                    SaveRetentionStatus.UnsupportedStorage,
                    EchoSaveDiagnosticCodes
                        .RetentionUnsupportedStorage,
                    "The active Chronicle storage provider does not expose bounded child-directory discovery.");
            }

            if (!TryReadHead(
                    validatedSlot,
                    out SaveHeadPointer head,
                    out SaveGenerationId currentGeneration,
                    out SaveGenerationId previousGeneration,
                    out bool hasPrevious,
                    out SaveRetentionResult headFailure))
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
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes
                        .RetentionUntrustworthy,
                    "Chronicle retention could not construct the validated generation-discovery key.");
            }

            SaveStorageDiscoveryResult discovered =
                discovery.DiscoverChildDirectories(
                    generationsRoot,
                    discoveryLimit);

            if (!discovered.Succeeded)
            {
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    string.IsNullOrEmpty(
                        discovered.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .RetentionUntrustworthy
                        : discovered.DiagnosticCode,
                    "Chronicle retention could not establish one bounded trustworthy generation-directory snapshot. " +
                    discovered.Message);
            }

            if (discovered.Status ==
                SaveStorageDiscoveryStatus.ParentNotFound)
            {
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes
                        .RetentionUntrustworthy,
                    "Chronicle retention found a valid head but no committed-generation directory.");
            }

            List<RetentionCandidate> candidates =
                new List<RetentionCandidate>();

            int canonicalCount =
                0;

            for (int i = 0;
                 i < discovered.ChildNames.Count;
                 i++)
            {
                string childName =
                    discovered.ChildNames[i];

                if (!SaveGenerationId.TryParse(
                        childName,
                        out SaveGenerationId generationId))
                {
                    continue;
                }

                canonicalCount++;

                if (!TryReadCandidate(
                        validatedSlot,
                        generationId,
                        out RetentionCandidate candidate,
                        out string failureMessage))
                {
                    return Failure(
                        SaveRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes
                            .RetentionUntrustworthy,
                        failureMessage,
                        canonicalCount,
                        candidates.Count);
                }

                candidates.Add(
                    candidate);
            }

            if (!ContainsGeneration(
                    candidates,
                    currentGeneration))
            {
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes
                        .RetentionUntrustworthy,
                    "Chronicle retention could not verify the current head generation inside the discovered committed history.",
                    canonicalCount,
                    candidates.Count);
            }

            if (hasPrevious &&
                !ContainsGeneration(
                    candidates,
                    previousGeneration))
            {
                return Failure(
                    SaveRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes
                        .RetentionUntrustworthy,
                    "Chronicle retention could not verify the immediate recovery predecessor inside the discovered committed history.",
                    canonicalCount,
                    candidates.Count);
            }

            if (candidates.Count <=
                policy.MaxTotalGenerations)
            {
                return SaveRetentionResult.NotRequired(
                    "Chronicle committed generation history is already within the configured retention bound.",
                    canonicalCount,
                    candidates.Count);
            }

            List<RetentionCandidate> eligible =
                new List<RetentionCandidate>();

            for (int i = 0;
                 i < candidates.Count;
                 i++)
            {
                RetentionCandidate candidate =
                    candidates[i];

                if (candidate.GenerationId ==
                        currentGeneration ||
                    hasPrevious &&
                    candidate.GenerationId ==
                        previousGeneration)
                {
                    continue;
                }

                eligible.Add(
                    candidate);
            }

            int requestedDeletionCount =
                candidates.Count -
                policy.MaxTotalGenerations;

            if (eligible.Count <
                requestedDeletionCount)
            {
                requestedDeletionCount =
                    eligible.Count;
            }

            if (requestedDeletionCount <= 0)
            {
                return SaveRetentionResult.NotRequired(
                    "Chronicle retention preserved protected current/recovery history even though it exceeds the nominal bound.",
                    canonicalCount,
                    candidates.Count);
            }

            if (!(storage is
                    ISaveStorageTreeDeletionBackend treeDeletion))
            {
                return Failure(
                    SaveRetentionStatus.UnsupportedStorage,
                    EchoSaveDiagnosticCodes
                        .RetentionUnsupportedStorage,
                    "The active Chronicle storage provider cannot delete a complete committed-generation tree.",
                    canonicalCount,
                    candidates.Count,
                    requestedDeletionCount);
            }

            eligible.Sort(
                CompareCandidates);

            int deleted =
                0;

            for (int i = 0;
                 i < requestedDeletionCount;
                 i++)
            {
                RetentionCandidate candidate =
                    eligible[i];

                SaveStorageResult deletion =
                    treeDeletion.DeleteTree(
                        candidate.GenerationDirectory);

                if (!deletion.Succeeded)
                {
                    return new SaveRetentionResult(
                        deleted > 0
                            ? SaveRetentionStatus.PartialFailure
                            : SaveRetentionStatus.Failed,
                        string.IsNullOrEmpty(
                            deletion.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .RetentionDeleteFailed
                            : deletion.DiagnosticCode,
                        "Chronicle committed save remains authoritative, but generation-retention cleanup did not fully complete. " +
                        deletion.Message,
                        canonicalCount,
                        candidates.Count,
                        requestedDeletionCount,
                        deleted,
                        candidate.GenerationId);
                }

                deleted++;
            }

            return new SaveRetentionResult(
                SaveRetentionStatus.Completed,
                string.Empty,
                "Chronicle deleted the oldest excess verified committed generations while preserving current and immediate recovery history.",
                canonicalCount,
                candidates.Count,
                requestedDeletionCount,
                deleted,
                default);
        }

        private bool TryReadHead(
            SaveSlotId slotId,
            out SaveHeadPointer head,
            out SaveGenerationId currentGeneration,
            out SaveGenerationId previousGeneration,
            out bool hasPrevious,
            out SaveRetentionResult failure)
        {
            head =
                null;

            currentGeneration =
                default;

            previousGeneration =
                default;

            hasPrevious =
                false;

            failure =
                default;

            if (!SaveStorageKey.TryCreate(
                    "slots/" +
                    slotId.Value +
                    "/head.json",
                    out SaveStorageKey headKey)
                    .Succeeded)
            {
                failure =
                    Failure(
                        SaveRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes
                            .RetentionUntrustworthy,
                        "Chronicle retention could not construct the validated head key.");

                return false;
            }

            SaveStorageReadResult read =
                storage.Read(
                    headKey);

            if (!read.Succeeded)
            {
                failure =
                    Failure(
                        SaveRetentionStatus.Untrustworthy,
                        string.IsNullOrEmpty(
                            read.Result.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .RetentionUntrustworthy
                            : read.Result.DiagnosticCode,
                        "Chronicle retention requires a readable authoritative head before cleanup. " +
                        read.Result.Message);

                return false;
            }

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out head);

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
                    out currentGeneration))
            {
                failure =
                    Failure(
                        SaveRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes
                            .RetentionUntrustworthy,
                        "Chronicle retention requires one structurally valid head matching the target slot.");

                return false;
            }

            if (!string.IsNullOrEmpty(
                    head.previousGenerationId))
            {
                if (!SaveGenerationId.TryParse(
                        head.previousGenerationId,
                        out previousGeneration))
                {
                    failure =
                        Failure(
                            SaveRetentionStatus.Untrustworthy,
                            EchoSaveDiagnosticCodes
                                .RetentionUntrustworthy,
                            "Chronicle retention could not validate the immediate recovery predecessor identity.");

                    return false;
                }

                hasPrevious =
                    true;
            }

            return true;
        }

        private bool TryReadCandidate(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            out RetentionCandidate candidate,
            out string failureMessage)
        {
            candidate =
                default;

            failureMessage =
                string.Empty;

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    slotId,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                failureMessage =
                    "Chronicle retention could not construct one validated generation key set.";

                return false;
            }

            SaveStorageReadResult read =
                storage.Read(
                    keys.GenerationManifest);

            if (!read.Succeeded)
            {
                failureMessage =
                    "Chronicle retention preserved a canonical generation because its manifest was unreadable.";

                return false;
            }

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveManifest manifest);

            if (!deserialized.Succeeded ||
                manifest == null)
            {
                failureMessage =
                    "Chronicle retention preserved a canonical generation because its manifest was malformed or unsupported.";

                return false;
            }

            if (!SaveSlotId.TryParse(
                    manifest.slotId,
                    out SaveSlotId manifestSlot) ||
                manifestSlot !=
                    slotId ||
                !SaveGenerationId.TryParse(
                    manifest.generationId,
                    out SaveGenerationId manifestGeneration) ||
                manifestGeneration !=
                    generationId ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed)
            {
                failureMessage =
                    "Chronicle retention preserved a canonical generation because its committed slot/generation identity could not be trusted.";

                return false;
            }

            if (!DateTimeOffset.TryParse(
                    manifest.updatedUtc,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTimeOffset technicalTimestamp))
            {
                failureMessage =
                    "Chronicle retention preserved a canonical generation because its technical timestamp was invalid.";

                return false;
            }

            candidate =
                new RetentionCandidate(
                    generationId,
                    keys.GenerationDirectory,
                    technicalTimestamp);

            return true;
        }

        private static bool ContainsGeneration(
            List<RetentionCandidate> candidates,
            SaveGenerationId generationId)
        {
            for (int i = 0;
                 i < candidates.Count;
                 i++)
            {
                if (candidates[i]
                    .GenerationId ==
                    generationId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CompareCandidates(
            RetentionCandidate left,
            RetentionCandidate right)
        {
            int timestamp =
                left.TechnicalTimestamp
                    .CompareTo(
                        right.TechnicalTimestamp);

            if (timestamp != 0)
            {
                return timestamp;
            }

            return string.Compare(
                left.GenerationId.Value,
                right.GenerationId.Value,
                StringComparison.Ordinal);
        }

        private static SaveRetentionResult Failure(
            SaveRetentionStatus status,
            string diagnosticCode,
            string message,
            int discoveredCanonicalCount = 0,
            int verifiedCommittedCount = 0,
            int plannedDeletionCount = 0) =>
            new SaveRetentionResult(
                status,
                diagnosticCode,
                message,
                discoveredCanonicalCount,
                verifiedCommittedCount,
                plannedDeletionCount,
                0,
                default);

        private readonly struct RetentionCandidate
        {
            internal RetentionCandidate(
                SaveGenerationId generationId,
                SaveStorageKey generationDirectory,
                DateTimeOffset technicalTimestamp)
            {
                GenerationId =
                    generationId;

                GenerationDirectory =
                    generationDirectory;

                TechnicalTimestamp =
                    technicalTimestamp;
            }

            internal SaveGenerationId GenerationId { get; }

            internal SaveStorageKey GenerationDirectory { get; }

            internal DateTimeOffset TechnicalTimestamp { get; }
        }
    }
}
