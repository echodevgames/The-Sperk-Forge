namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Exact-once terminal result for one admitted modal generation.
    /// </summary>
    public readonly struct UIModalResult
    {
        public UIModalResult(
            UIModalOutcome outcome,
            UIModalId modalId,
            long generation,
            UIModalResultId resultId = default,
            UIModalAbortReason abortReason = UIModalAbortReason.None,
            string message = "")
        {
            Outcome = outcome;
            ModalId = modalId;
            Generation = generation;
            ResultId = resultId;
            AbortReason = abortReason;
            Message = message ?? string.Empty;
        }

        public UIModalOutcome Outcome { get; }

        public UIModalId ModalId { get; }

        public long Generation { get; }

        public UIModalResultId ResultId { get; }

        public UIModalAbortReason AbortReason { get; }

        public string Message { get; }

        public bool IsSemanticCompletion =>
            Outcome == UIModalOutcome.Completed;

        public bool IsAborted =>
            Outcome == UIModalOutcome.Aborted;
    }
}
