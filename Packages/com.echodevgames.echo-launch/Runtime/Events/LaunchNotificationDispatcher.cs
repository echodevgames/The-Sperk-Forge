//----- LaunchNotificationDispatcher.cs START -----

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Safely dispatches launch notifications one listener at a time.
    /// </summary>
    internal static class LaunchNotificationDispatcher
    {
        internal const string ListenerFailureDiagnosticCode =
            "ELAUNCH-EVENT-001";

        /// <summary>
        /// Invokes every subscribed listener in subscription order while
        /// isolating failures from individual listeners.
        /// </summary>
        internal static void Dispatch<TPayload>(
            Action<TPayload> listeners,
            TPayload payload,
            string eventName,
            Object context)
        {
            if (listeners == null)
            {
                return;
            }

            Delegate[] invocationList =
                listeners.GetInvocationList();

            for (int index = 0;
                 index < invocationList.Length;
                 index++)
            {
                Action<TPayload> listener =
                    invocationList[index] as Action<TPayload>;

                if (listener == null)
                {
                    continue;
                }

                try
                {
                    listener(payload);
                }
                catch (Exception exception)
                {
                    string failureMessage =
                        BuildFailureMessage(
                            listener,
                            eventName,
                            exception);

                    Debug.LogWarning(
                        failureMessage,
                        context);
                }
            }
        }

        private static string BuildFailureMessage(
            Delegate listener,
            string eventName,
            Exception exception)
        {
            string listenerType = "<unknown>";

            if (listener.Target != null)
            {
                Type targetType =
                    listener.Target.GetType();

                if (!string.IsNullOrWhiteSpace(
                        targetType.FullName))
                {
                    listenerType =
                        targetType.FullName;
                }
            }
            else if (listener.Method.DeclaringType != null &&
                     !string.IsNullOrWhiteSpace(
                         listener.Method.DeclaringType.FullName))
            {
                listenerType =
                    listener.Method.DeclaringType.FullName;
            }

            string listenerMethod =
                listener.Method.Name;

            string exceptionType =
                exception.GetType().FullName;

            if (string.IsNullOrWhiteSpace(
                    exceptionType))
            {
                exceptionType =
                    exception.GetType().Name;
            }

            string exceptionMessage =
                SanitizeMessage(
                    exception.Message);

            return
                "[" +
                ListenerFailureDiagnosticCode +
                "] Listener failure while dispatching '" +
                eventName +
                "'. Listener: '" +
                listenerType +
                "." +
                listenerMethod +
                "'. Exception: '" +
                exceptionType +
                "'. Message: '" +
                exceptionMessage +
                "'.";
        }

        private static string SanitizeMessage(
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "<no message>";
            }

            return message
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Trim();
        }
    }
}

//----- LaunchNotificationDispatcher.cs END -----
