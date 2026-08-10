using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// One detached current-version participant DTO prepared from validated
    /// durable transport data. The object is not durable and has not been
    /// applied to gameplay state.
    ///
    /// M3-07 adds transport-only migration provenance. Provenance contains
    /// stable migration IDs/version edges only, never serialized payload text.
    /// </summary>
    internal sealed class SavePreparedParticipantEntry
    {
        private readonly
            SaveParticipantMigrationProvenanceEntry[]
                migrationProvenance;

        internal SavePreparedParticipantEntry(
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            int participantSchemaVersion,
            SaveSerializerId serializerId,
            Type detachedStateType,
            object detachedState)
            : this(
                persistedParticipantId,
                canonicalParticipantId,
                participantSchemaVersion,
                participantSchemaVersion,
                serializerId,
                detachedStateType,
                detachedState,
                Array.Empty<
                    SaveParticipantMigrationProvenanceEntry>())
        {
        }

        internal SavePreparedParticipantEntry(
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            int storedParticipantSchemaVersion,
            int participantSchemaVersion,
            SaveSerializerId serializerId,
            Type detachedStateType,
            object detachedState,
            SaveParticipantMigrationProvenanceEntry[]
                migrationProvenance)
        {
            PersistedParticipantId =
                persistedParticipantId;

            CanonicalParticipantId =
                canonicalParticipantId;

            StoredParticipantSchemaVersion =
                storedParticipantSchemaVersion;

            ParticipantSchemaVersion =
                participantSchemaVersion;

            SerializerId =
                serializerId;

            DetachedStateType =
                detachedStateType;

            DetachedState =
                detachedState;

            this.migrationProvenance =
                migrationProvenance == null
                    ? Array.Empty<
                        SaveParticipantMigrationProvenanceEntry>()
                    : (SaveParticipantMigrationProvenanceEntry[])
                        migrationProvenance.Clone();
        }

        internal SaveParticipantId PersistedParticipantId { get; }

        internal SaveParticipantId CanonicalParticipantId { get; }

        internal int StoredParticipantSchemaVersion { get; }

        internal int ParticipantSchemaVersion { get; }

        internal SaveSerializerId SerializerId { get; }

        internal Type DetachedStateType { get; }

        internal object DetachedState { get; }

        internal int MigrationStepCount =>
            migrationProvenance.Length;

        internal IReadOnlyList<
            SaveParticipantMigrationProvenanceEntry>
            MigrationProvenance =>
            Array.AsReadOnly(
                (SaveParticipantMigrationProvenanceEntry[])
                    migrationProvenance.Clone());
    }
}
