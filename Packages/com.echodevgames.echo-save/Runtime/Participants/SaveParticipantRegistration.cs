
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Owns one active participant-registry membership.
    ///
    /// Disposal is idempotent. A registration carries an internal ownership
    /// token so a stale handle cannot unregister a later participant that
    /// reuses the same canonical ID.
    /// </summary>
    public sealed class SaveParticipantRegistration :
        IDisposable
    {
        private SaveParticipantRegistry registry;
        private readonly SaveParticipantId participantId;
        private readonly long ownershipToken;
        private bool disposed;

        internal SaveParticipantRegistration(
            SaveParticipantRegistry registry,
            SaveParticipantId participantId,
            long ownershipToken)
        {
            this.registry = registry;
            this.participantId =
                participantId;
            this.ownershipToken =
                ownershipToken;
        }

        public SaveParticipantId ParticipantId =>
            participantId;

        public bool IsActive =>
            !disposed &&
            registry != null &&
            registry.Owns(
                participantId,
                ownershipToken);

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed =
                true;

            SaveParticipantRegistry owner =
                registry;

            registry =
                null;

            owner?.Release(
                participantId,
                ownershipToken);
        }
    }
}
