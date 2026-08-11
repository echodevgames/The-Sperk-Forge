
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveTrashRetentionCoordinator
    {
        private readonly ISaveStorageBackend storage;
        private readonly int discoveryLimit;

        internal SaveTrashRetentionCoordinator(
            ISaveStorageBackend storage,
            int discoveryLimit)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(nameof(storage));

            if (discoveryLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(discoveryLimit));
            }

            this.discoveryLimit = discoveryLimit;
        }

        internal SaveTrashRetentionResult Apply(
            int maxTrashRecords)
        {
            if (maxTrashRecords <= 0 ||
                maxTrashRecords > 256)
            {
                return Failure(
                    SaveTrashRetentionStatus.InvalidPolicy,
                    EchoSaveDiagnosticCodes.TrashRetentionInvalidPolicy,
                    "Chronicle trash retention requires a positive record bound no greater than 256.");
            }

            if (!(storage is ISaveStorageDiscoveryBackend discovery))
            {
                return Failure(
                    SaveTrashRetentionStatus.UnsupportedStorage,
                    EchoSaveDiagnosticCodes.TrashRetentionUnsupportedStorage,
                    "The active Chronicle storage provider does not expose bounded trash discovery.");
            }

            SaveStorageResult rootKeyResult =
                SaveStorageKey.TryCreate(
                    "trash",
                    out SaveStorageKey trashRoot);

            if (!rootKeyResult.Succeeded)
            {
                return Failure(
                    SaveTrashRetentionStatus.Untrustworthy,
                    EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy,
                    "Chronicle could not construct the package-owned trash root.");
            }

            SaveStorageDiscoveryResult discovered =
                discovery.DiscoverChildDirectories(
                    trashRoot,
                    discoveryLimit);

            if (discovered.Status ==
                SaveStorageDiscoveryStatus.ParentNotFound)
            {
                return SaveTrashRetentionResult.NotRequired(
                    "Chronicle trash is empty.");
            }

            if (!discovered.Succeeded)
            {
                return Failure(
                    SaveTrashRetentionStatus.Untrustworthy,
                    string.IsNullOrEmpty(discovered.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy
                        : discovered.DiagnosticCode,
                    "Chronicle could not establish one bounded trustworthy trash snapshot. " +
                    discovered.Message);
            }

            List<TrashRecord> records =
                new List<TrashRecord>();

            for (int i = 0;
                 i < discovered.ChildNames.Count;
                 i++)
            {
                string name =
                    discovered.ChildNames[i];

                if (!TryParseRecord(
                        name,
                        out TrashRecord record))
                {
                    return Failure(
                        SaveTrashRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy,
                        "Chronicle preserved trash because one discovered record identity was not a trusted package-owned canonical trash identity.",
                        discovered.ChildNames.Count);
                }

                SaveStorageResult recordKeyResult =
                    SaveStorageKey.TryCreate(
                        "trash/" + record.RecordId,
                        out SaveStorageKey recordKey);

                if (!recordKeyResult.Succeeded)
                {
                    return Failure(
                        SaveTrashRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy,
                        "Chronicle preserved trash because one record key could not be reconstructed safely.",
                        discovered.ChildNames.Count);
                }

                SaveStorageDiscoveryResult recordChildren =
                    discovery.DiscoverChildDirectories(
                        recordKey,
                        2);

                if (!recordChildren.Succeeded ||
                    recordChildren.ChildNames.Count != 1 ||
                    !string.Equals(
                        recordChildren.ChildNames[0],
                        "slot",
                        StringComparison.Ordinal))
                {
                    return Failure(
                        SaveTrashRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy,
                        "Chronicle preserved trash because one canonical record did not contain exactly one package-owned slot tree.",
                        discovered.ChildNames.Count);
                }

                records.Add(record);
            }

            if (records.Count <= maxTrashRecords)
            {
                return SaveTrashRetentionResult.NotRequired(
                    "Chronicle recoverable trash is already within the configured bound.",
                    records.Count);
            }

            if (!(storage is ISaveStorageTreeDeletionBackend treeDeletion))
            {
                return Failure(
                    SaveTrashRetentionStatus.UnsupportedStorage,
                    EchoSaveDiagnosticCodes.TrashRetentionUnsupportedStorage,
                    "The active Chronicle storage provider cannot delete an excess complete trash record.",
                    records.Count,
                    records.Count - maxTrashRecords);
            }

            records.Sort(Compare);

            int requested =
                records.Count - maxTrashRecords;

            int deleted = 0;

            for (int i = 0;
                 i < requested;
                 i++)
            {
                TrashRecord record =
                    records[i];

                SaveStorageResult recordKeyResult =
                    SaveStorageKey.TryCreate(
                        "trash/" + record.RecordId,
                        out SaveStorageKey recordKey);

                if (!recordKeyResult.Succeeded)
                {
                    return Failure(
                        SaveTrashRetentionStatus.Untrustworthy,
                        EchoSaveDiagnosticCodes.TrashRetentionUntrustworthy,
                        "Chronicle could not construct one trusted trash-record key.",
                        records.Count,
                        requested);
                }

                SaveStorageResult deletion =
                    treeDeletion.DeleteTree(recordKey);

                if (!deletion.Succeeded)
                {
                    return new SaveTrashRetentionResult(
                        deleted > 0
                            ? SaveTrashRetentionStatus.PartialFailure
                            : SaveTrashRetentionStatus.Failed,
                        string.IsNullOrEmpty(deletion.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes.TrashRetentionDeleteFailed
                            : deletion.DiagnosticCode,
                        "Chronicle live-slot deletion remains committed, but bounded trash cleanup did not fully complete. " +
                        deletion.Message,
                        records.Count,
                        requested,
                        deleted,
                        record.RecordId);
                }

                deleted++;
            }

            return new SaveTrashRetentionResult(
                SaveTrashRetentionStatus.Completed,
                string.Empty,
                "Chronicle removed the oldest excess trusted recoverable trash records.",
                records.Count,
                requested,
                deleted,
                string.Empty);
        }

        internal static bool TryParseRecord(
            string recordId,
            out long utcTicks)
        {
            utcTicks = 0;

            if (string.IsNullOrEmpty(recordId) ||
                recordId.Length != 52 ||
                recordId[19] != '-')
            {
                return false;
            }

            string ticksText =
                recordId.Substring(0, 19);

            string token =
                recordId.Substring(20);

            if (!long.TryParse(
                    ticksText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out utcTicks) ||
                utcTicks <= 0 ||
                token.Length != 32)
            {
                utcTicks = 0;
                return false;
            }

            for (int i = 0;
                 i < token.Length;
                 i++)
            {
                char c = token[i];

                bool hex =
                    c >= '0' && c <= '9' ||
                    c >= 'a' && c <= 'f';

                if (!hex)
                {
                    utcTicks = 0;
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseRecord(
            string recordId,
            out TrashRecord record)
        {
            record = default;

            if (!TryParseRecord(
                    recordId,
                    out long ticks))
            {
                return false;
            }

            record =
                new TrashRecord(
                    recordId,
                    ticks);

            return true;
        }

        private static int Compare(
            TrashRecord left,
            TrashRecord right)
        {
            int ticks =
                left.UtcTicks.CompareTo(
                    right.UtcTicks);

            if (ticks != 0)
            {
                return ticks;
            }

            return string.CompareOrdinal(
                left.RecordId,
                right.RecordId);
        }

        private static SaveTrashRetentionResult Failure(
            SaveTrashRetentionStatus status,
            string diagnosticCode,
            string message,
            int discoveredRecordCount = 0,
            int plannedDeletionCount = 0) =>
            new SaveTrashRetentionResult(
                status,
                diagnosticCode,
                message,
                discoveredRecordCount,
                plannedDeletionCount,
                0,
                string.Empty);

        private readonly struct TrashRecord
        {
            internal TrashRecord(
                string recordId,
                long utcTicks)
            {
                RecordId = recordId;
                UtcTicks = utcTicks;
            }

            internal string RecordId { get; }
            internal long UtcTicks { get; }
        }
    }
}
