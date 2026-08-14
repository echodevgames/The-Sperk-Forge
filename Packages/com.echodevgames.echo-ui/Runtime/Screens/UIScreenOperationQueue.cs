using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Bounded strict FIFO queue for accepted structural Screen mutations.
    /// </summary>
    public sealed class UIScreenOperationQueue
    {
        private readonly Queue<UIScreenHandle> pending =
            new Queue<UIScreenHandle>();

        public UIScreenOperationQueue(
            int capacity)
        {
            Capacity =
                capacity < 1
                    ? 1
                    : capacity;
        }

        public int Capacity { get; }

        public int Count =>
            pending.Count;

        public bool TryEnqueue(
            UIScreenOperationRequest request,
            out UIScreenHandle handle)
        {
            if (request == null)
            {
                handle = null;
                return false;
            }

            if (pending.Count >= Capacity)
            {
                handle =
                    UIScreenHandle.Rejected(
                        request,
                        "Looking Glass Screen operation queue capacity was reached.");
                return false;
            }

            handle =
                new UIScreenHandle(
                    request,
                    true);

            pending.Enqueue(
                handle);

            return true;
        }

        public bool TryProcessNext(
            Func<UIScreenOperationRequest, UIScreenOperationResult> executor,
            out UIScreenHandle settledHandle)
        {
            settledHandle = null;

            if (pending.Count == 0)
            {
                return false;
            }

            UIScreenHandle handle =
                pending.Dequeue();

            UIScreenOperationResult result;
            try
            {
                result =
                    executor == null
                        ? new UIScreenOperationResult(
                            UIScreenOperationStatus.Failed,
                            handle.Request.Kind,
                            handle.Request.ScreenId,
                            handle.Request.ScopeId,
                            handle.Request.Sequence,
                            "No Screen operation executor was supplied.")
                        : executor(handle.Request);
            }
            catch (Exception exception)
            {
                result =
                    new UIScreenOperationResult(
                        UIScreenOperationStatus.Failed,
                        handle.Request.Kind,
                        handle.Request.ScreenId,
                        handle.Request.ScopeId,
                        handle.Request.Sequence,
                        exception.Message);
            }

            handle.Complete(
                result);

            settledHandle =
                handle;

            return true;
        }

        public void ClearRejected(
            string message)
        {
            while (pending.Count > 0)
            {
                UIScreenHandle handle =
                    pending.Dequeue();

                handle.Complete(
                    new UIScreenOperationResult(
                        UIScreenOperationStatus.Rejected,
                        handle.Request.Kind,
                        handle.Request.ScreenId,
                        handle.Request.ScopeId,
                        handle.Request.Sequence,
                        message));
            }
        }
    }
}
