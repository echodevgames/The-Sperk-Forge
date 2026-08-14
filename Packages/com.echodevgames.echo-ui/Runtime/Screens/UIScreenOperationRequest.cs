namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable structural Screen mutation request.
    /// </summary>
    public sealed class UIScreenOperationRequest
    {
        private UIScreenOperationRequest(
            UIScreenOperationKind kind,
            string screenId,
            string scopeId,
            long sequence)
        {
            Kind = kind;
            ScreenId =
                screenId == null
                    ? string.Empty
                    : screenId.Trim();
            ScopeId =
                scopeId == null
                    ? string.Empty
                    : scopeId.Trim();
            Sequence = sequence;
        }

        public UIScreenOperationKind Kind { get; }

        public string ScreenId { get; }

        public string ScopeId { get; }

        public long Sequence { get; }

        public static UIScreenOperationRequest Push(
            string screenId) =>
            new UIScreenOperationRequest(
                UIScreenOperationKind.Push,
                screenId,
                string.Empty,
                0);

        public static UIScreenOperationRequest Replace(
            string screenId) =>
            new UIScreenOperationRequest(
                UIScreenOperationKind.Replace,
                screenId,
                string.Empty,
                0);

        public static UIScreenOperationRequest Reset(
            string screenId) =>
            new UIScreenOperationRequest(
                UIScreenOperationKind.Reset,
                screenId,
                string.Empty,
                0);

        public static UIScreenOperationRequest Back(
            string scopeId) =>
            new UIScreenOperationRequest(
                UIScreenOperationKind.Back,
                string.Empty,
                scopeId,
                0);

        public static UIScreenOperationRequest Close(
            string screenId,
            string scopeId = "") =>
            new UIScreenOperationRequest(
                UIScreenOperationKind.Close,
                screenId,
                scopeId,
                0);

        internal UIScreenOperationRequest WithSequence(
            long sequence) =>
            new UIScreenOperationRequest(
                Kind,
                ScreenId,
                ScopeId,
                sequence);
    }
}
