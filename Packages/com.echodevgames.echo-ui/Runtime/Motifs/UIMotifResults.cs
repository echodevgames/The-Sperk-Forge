namespace EchoDevGames.EchoUI
{
    public enum UIMotifDefinitionStatus
    {
        Ready = 0,
        MissingDefinition = 1,
        InvalidCapacity = 2,
        InvalidMotifId = 3,
        InvalidTokenId = 4,
        DuplicateTokenId = 5,
        CapacityExceeded = 6,
        InvalidTokenValue = 7
    }

    /// <summary>
    /// Structured result of validating and detaching one authored Motif.
    /// A failed result never carries a partially built snapshot.
    /// </summary>
    public readonly struct UIMotifDefinitionResult
    {
        public UIMotifDefinitionResult(
            UIMotifDefinitionStatus status,
            UIMotifId motifId = default,
            UIMotifTokenId tokenId = default,
            UIMotifTokenKind tokenKind = UIMotifTokenKind.None,
            int tokenCount = 0,
            UIMotifSnapshot snapshot = null,
            string message = "")
        {
            Status = status;
            MotifId = motifId;
            TokenId = tokenId;
            TokenKind = tokenKind;
            TokenCount = tokenCount;
            Snapshot = snapshot;
            Message = message ?? string.Empty;
        }

        public UIMotifDefinitionStatus Status { get; }

        public UIMotifId MotifId { get; }

        public UIMotifTokenId TokenId { get; }

        public UIMotifTokenKind TokenKind { get; }

        public int TokenCount { get; }

        public UIMotifSnapshot Snapshot { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UIMotifDefinitionStatus.Ready &&
            Snapshot != null;
    }
}
