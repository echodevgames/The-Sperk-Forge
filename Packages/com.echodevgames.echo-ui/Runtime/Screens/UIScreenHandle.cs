namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Handle for one admitted Screen operation. Normal runtime submission drains
    /// synchronously, while tests/samples may observe completion across delayed queue steps.
    /// </summary>
    public sealed class UIScreenHandle
    {
        internal UIScreenHandle(
            UIScreenOperationRequest request,
            bool accepted)
        {
            Request = request;
            Accepted = accepted;
        }

        public UIScreenOperationRequest Request { get; }

        public bool Accepted { get; }

        public bool IsCompleted { get; private set; }

        public UIScreenOperationResult Result { get; private set; }

        internal void Complete(
            UIScreenOperationResult result)
        {
            if (IsCompleted)
            {
                return;
            }

            Result = result;
            IsCompleted = true;
        }

        internal static UIScreenHandle Rejected(
            UIScreenOperationRequest request,
            string message)
        {
            UIScreenHandle handle =
                new UIScreenHandle(
                    request,
                    false);

            handle.Complete(
                new UIScreenOperationResult(
                    UIScreenOperationStatus.Rejected,
                    request.Kind,
                    request.ScreenId,
                    request.ScopeId,
                    request.Sequence,
                    message));

            return handle;
        }
    }
}
