
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Opaque caller-held lifetime handle for one fully validated, migrated,
    /// prepared Chronicle load. The public surface exposes metadata only.
    /// </summary>
    public sealed class PreparedSaveLoad : IDisposable
    {
        private SavePreparedLoadStore owner;
        private readonly long ownershipToken;
        private readonly long ownerEpoch;
        private PreparedLoadState state;

        internal PreparedSaveLoad(
            SavePreparedLoadStore owner,
            long ownershipToken,
            long ownerEpoch,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            int preparedParticipantCount,
            int unknownPayloadCount,
            long sourceTransportByteEstimate,
            DateTimeOffset createdUtc,
            DateTimeOffset expiresUtc)
        {
            this.owner = owner;
            this.ownershipToken = ownershipToken;
            this.ownerEpoch = ownerEpoch;
            SourceSlotId = sourceSlotId;
            SourceGenerationId = sourceGenerationId;
            PreparedParticipantCount = preparedParticipantCount;
            UnknownPayloadCount = unknownPayloadCount;
            SourceTransportByteEstimate = sourceTransportByteEstimate;
            CreatedUtc = createdUtc;
            ExpiresUtc = expiresUtc;
            state = PreparedLoadState.Live;
        }

        public SaveSlotId SourceSlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public int PreparedParticipantCount { get; }

        public int UnknownPayloadCount { get; }

        public long SourceTransportByteEstimate { get; }

        public DateTimeOffset CreatedUtc { get; }

        public DateTimeOffset ExpiresUtc { get; }

        public PreparedLoadState State
        {
            get
            {
                SavePreparedLoadStore currentOwner = owner;
                currentOwner?.RefreshState(
                    this,
                    ownershipToken,
                    ownerEpoch);

                return state;
            }
        }

        public bool IsValid =>
            State == PreparedLoadState.Live;

        internal long OwnershipToken =>
            ownershipToken;

        internal long OwnerEpoch =>
            ownerEpoch;

        internal PreparedLoadState UnsafeState =>
            state;

        internal bool IsOwnedBy(
            SavePreparedLoadStore candidateOwner,
            long candidateToken,
            long candidateEpoch) =>
            ReferenceEquals(owner, candidateOwner) &&
            ownershipToken == candidateToken &&
            ownerEpoch == candidateEpoch &&
            state == PreparedLoadState.Live;

        internal void SetTerminalState(
            PreparedLoadState terminalState)
        {
            if (state != PreparedLoadState.Live)
            {
                return;
            }

            if (terminalState == PreparedLoadState.Live)
            {
                throw new ArgumentException(
                    "A terminal prepared-load state is required.",
                    nameof(terminalState));
            }

            state = terminalState;
            owner = null;
        }

        public void Dispose()
        {
            if (state != PreparedLoadState.Live)
            {
                return;
            }

            SavePreparedLoadStore currentOwner = owner;

            if (currentOwner == null)
            {
                SetTerminalState(
                    PreparedLoadState.Disposed);

                return;
            }

            currentOwner.ReleaseOwned(
                this,
                ownershipToken,
                ownerEpoch,
                PreparedLoadState.Disposed);
        }
    }
}
