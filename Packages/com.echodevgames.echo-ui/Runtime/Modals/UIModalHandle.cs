using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Fresh handle and awaitable completion channel for one modal opening.
    /// </summary>
    public sealed class UIModalHandle
    {
        private readonly TaskCompletionSource<UIModalResult> completion =
            new TaskCompletionSource<UIModalResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        internal UIModalHandle(
            UIModalId modalId,
            long generation,
            bool accepted)
        {
            ModalId = modalId;
            Generation = generation;
            Accepted = accepted;
        }

        public UIModalId ModalId { get; }

        public long Generation { get; }

        public bool Accepted { get; }

        public bool IsCompleted { get; private set; }

        public UIModalResult Result { get; private set; }

        public Task<UIModalResult> Completion =>
            completion.Task;

        public event Action<UIModalResult> Completed;

        internal bool TryComplete(
            UIModalResult result)
        {
            if (IsCompleted)
            {
                return false;
            }

            Result = result;
            IsCompleted = true;

            completion.TrySetResult(
                result);

            Action<UIModalResult> handlers =
                Completed;

            if (handlers != null)
            {
                Delegate[] invocationList =
                    handlers.GetInvocationList();

                for (int index = 0;
                     index < invocationList.Length;
                     index++)
                {
                    try
                    {
                        ((Action<UIModalResult>)invocationList[index])(
                            result);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(
                            exception);
                    }
                }
            }

            return true;
        }

        internal static UIModalHandle Rejected(
            UIModalId modalId,
            long generation,
            string message)
        {
            UIModalHandle handle =
                new UIModalHandle(
                    modalId,
                    generation,
                    false);

            handle.TryComplete(
                new UIModalResult(
                    UIModalOutcome.Rejected,
                    modalId,
                    generation,
                    message: message));

            return handle;
        }
    }
}
