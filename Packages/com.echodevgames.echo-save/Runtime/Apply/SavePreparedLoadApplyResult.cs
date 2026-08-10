
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Structured payload-free result for one deterministic prepared-load
    /// apply attempt.
    /// </summary>
    public sealed class SavePreparedLoadApplyResult
    {
        private readonly
            ReadOnlyCollection<SaveParticipantApplyReportEntry>
                entries;

        internal SavePreparedLoadApplyResult(
            SavePreparedLoadApplyStatus status,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            bool mutationBegan,
            bool handleConsumed,
            SaveParticipantId failureParticipantId,
            string diagnosticCode,
            string message,
            SaveParticipantApplyReportEntry[] entries)
        {
            Status = status;
            SourceSlotId = sourceSlotId;
            SourceGenerationId = sourceGenerationId;
            MutationBegan = mutationBegan;
            HandleConsumed = handleConsumed;
            FailureParticipantId = failureParticipantId;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;

            SaveParticipantApplyReportEntry[] copy =
                entries == null
                    ? Array.Empty<SaveParticipantApplyReportEntry>()
                    : (SaveParticipantApplyReportEntry[])
                        entries.Clone();

            this.entries =
                Array.AsReadOnly(
                    copy);
        }

        public SavePreparedLoadApplyStatus Status { get; }

        public SaveSlotId SourceSlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public bool MutationBegan { get; }

        public bool HandleConsumed { get; }

        public SaveParticipantId FailureParticipantId { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public IReadOnlyList<SaveParticipantApplyReportEntry>
            Entries =>
            entries;

        public bool Succeeded =>
            Status ==
            SavePreparedLoadApplyStatus.Succeeded;
    }
}
