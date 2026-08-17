using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Fresh generation-safe handle and awaitable completion channel for one
    /// notification admission.
    /// </summary>
    public sealed class UINotificationHandle
    {
        private readonly TaskCompletionSource<UINotificationResult> completion =
            new TaskCompletionSource<UINotificationResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        internal UINotificationHandle(
            UINotificationAdmissionResult admission)
        {
            Admission = admission;
        }

        public UINotificationAdmissionResult Admission { get; }

        public UINotificationChannelId ChannelId =>
            Admission.ChannelId;

        public long Generation =>
            Admission.Generation;

        public bool Accepted =>
            Admission.Succeeded;

        public bool IsCompleted { get; private set; }

        public UINotificationResult Result { get; private set; }

        public Task<UINotificationResult> Completion =>
            completion.Task;

        public event Action<UINotificationResult> Completed;

        internal bool TryComplete(
            UINotificationResult result)
        {
            if (IsCompleted)
            {
                return false;
            }

            Result = result;
            IsCompleted = true;

            completion.TrySetResult(result);

            Action<UINotificationResult> handlers =
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
                        ((Action<UINotificationResult>)invocationList[index])(
                            result);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }

            return true;
        }

        internal static UINotificationHandle Rejected(
            UINotificationAdmissionResult admission)
        {
            UINotificationHandle handle =
                new UINotificationHandle(admission);

            handle.TryComplete(
                new UINotificationResult(
                    UINotificationOutcome.Rejected,
                    admission.ChannelId,
                    admission.Generation,
                    admission.CoalescingKey,
                    admission.CorrelationId,
                    admission.Message));

            return handle;
        }
    }
}
