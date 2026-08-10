using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// One detached current-version participant DTO prepared from validated
    /// durable transport data. The object is not durable and has not been
    /// applied to gameplay state.
    /// </summary>
    internal sealed class SavePreparedParticipantEntry
    {
        internal SavePreparedParticipantEntry(
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            int participantSchemaVersion,
            SaveSerializerId serializerId,
            Type detachedStateType,
            object detachedState)
        {
            PersistedParticipantId =
                persistedParticipantId;

            CanonicalParticipantId =
                canonicalParticipantId;

            ParticipantSchemaVersion =
                participantSchemaVersion;

            SerializerId =
                serializerId;

            DetachedStateType =
                detachedStateType;

            DetachedState =
                detachedState;
        }

        internal SaveParticipantId PersistedParticipantId { get; }

        internal SaveParticipantId CanonicalParticipantId { get; }

        internal int ParticipantSchemaVersion { get; }

        internal SaveSerializerId SerializerId { get; }

        internal Type DetachedStateType { get; }

        internal object DetachedState { get; }
    }
}
