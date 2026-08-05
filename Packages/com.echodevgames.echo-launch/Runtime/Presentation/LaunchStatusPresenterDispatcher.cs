//----- LaunchStatusPresenterDispatcher.cs START -----

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Resolves and safely invokes the neutral launch-status presenter.
    /// </summary>
    internal static class LaunchStatusPresenterDispatcher
    {
        internal static ILaunchStatusPresenter Resolve(
            MonoBehaviour component,
            Object context)
        {
            if (component == null)
            {
                return NullLaunchStatusPresenter.Shared;
            }

            if (component is
                ILaunchStatusPresenter presenter)
            {
                return presenter;
            }

            Debug.LogWarning(
                $"[{EchoLaunchRoot.PresenterUnavailableDiagnosticCode}] " +
                $"Assigned status presenter component '{component.GetType().FullName}' " +
                "does not implement ILaunchStatusPresenter. " +
                "First Light will continue through the headless presenter.",
                context);

            return NullLaunchStatusPresenter.Shared;
        }

        internal static void TryBind(
            ILaunchStatusPresenter presenter,
            LaunchProgressSnapshot initialSnapshot,
            Object context)
        {
            Invoke(
                presenter,
                () => presenter.Bind(
                    initialSnapshot),
                nameof(
                    ILaunchStatusPresenter.Bind),
                context);
        }

        internal static void TryPresent(
            ILaunchStatusPresenter presenter,
            LaunchProgressSnapshot snapshot,
            Object context)
        {
            Invoke(
                presenter,
                () => presenter.Present(
                    snapshot),
                nameof(
                    ILaunchStatusPresenter.Present),
                context);
        }

        internal static void TryPresentTerminal(
            ILaunchStatusPresenter presenter,
            LaunchReport report,
            Object context)
        {
            Invoke(
                presenter,
                () => presenter.PresentTerminal(
                    report),
                nameof(
                    ILaunchStatusPresenter
                        .PresentTerminal),
                context);
        }

        internal static void TryUnbind(
            ILaunchStatusPresenter presenter,
            Object context)
        {
            Invoke(
                presenter,
                presenter.Unbind,
                nameof(
                    ILaunchStatusPresenter.Unbind),
                context);
        }

        private static void Invoke(
            ILaunchStatusPresenter presenter,
            Action callback,
            string callbackName,
            Object context)
        {
            if (presenter == null ||
                callback == null)
            {
                return;
            }

            if (presenter is Object unityObject &&
                unityObject == null)
            {
                return;
            }

            try
            {
                callback();
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    BuildFailureMessage(
                        presenter,
                        callbackName,
                        exception),
                    context);
            }
        }

        private static string BuildFailureMessage(
            ILaunchStatusPresenter presenter,
            string callbackName,
            Exception exception)
        {
            string presenterType =
                presenter.GetType().FullName;

            if (string.IsNullOrWhiteSpace(
                    presenterType))
            {
                presenterType =
                    presenter.GetType().Name;
            }

            string exceptionType =
                exception.GetType().FullName;

            if (string.IsNullOrWhiteSpace(
                    exceptionType))
            {
                exceptionType =
                    exception.GetType().Name;
            }

            return
                "[" +
                EchoLaunchRoot
                    .PresenterFailureDiagnosticCode +
                "] Status presenter failure during '" +
                callbackName +
                "'. Presenter: '" +
                presenterType +
                "'. Exception: '" +
                exceptionType +
                "'. Message: '" +
                SanitizeMessage(
                    exception.Message) +
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

//----- LaunchStatusPresenterDispatcher.cs END -----
