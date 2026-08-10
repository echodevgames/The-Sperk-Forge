using System;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantMigrationExecutionResult
    {
        internal SaveParticipantMigrationExecutionResult(
            SaveParticipantMigrationExecutionStatus status,
            int finalSchemaVersion,
            SaveSerializerId serializerId,
            string serializedPayload,
            SaveParticipantMigrationProvenanceEntry[] provenance,
            string diagnosticCode,
            string message)
        {
            Status =
                status;

            FinalSchemaVersion =
                finalSchemaVersion;

            SerializerId =
                serializerId;

            SerializedPayload =
                serializedPayload;

            Provenance =
                provenance == null
                    ? Array.Empty<
                        SaveParticipantMigrationProvenanceEntry>()
                    : (SaveParticipantMigrationProvenanceEntry[])
                        provenance.Clone();

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;
        }

        internal SaveParticipantMigrationExecutionStatus Status { get; }

        internal int FinalSchemaVersion { get; }

        internal SaveSerializerId SerializerId { get; }

        internal string SerializedPayload { get; }

        internal SaveParticipantMigrationProvenanceEntry[] Provenance { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status ==
            SaveParticipantMigrationExecutionStatus.Succeeded;
    }
}
