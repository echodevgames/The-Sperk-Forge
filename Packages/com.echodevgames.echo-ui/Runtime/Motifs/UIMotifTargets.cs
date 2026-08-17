using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public enum UIMotifBindingMode
    {
        UseMotif = 0,
        KeepLocal = 1
    }

    public enum UIMotifTargetApplyStatus
    {
        Invalid = 0,
        Applied = 1,
        Partial = 2,
        Failed = 3
    }

    public readonly struct UIMotifTargetApplyResult
    {
        public UIMotifTargetApplyResult(
            UIMotifTargetApplyStatus status,
            int appliedBindingCount = 0,
            int keptLocalBindingCount = 0,
            int failedBindingCount = 0,
            string message = "")
        {
            Status = status;
            AppliedBindingCount = appliedBindingCount;
            KeptLocalBindingCount = keptLocalBindingCount;
            FailedBindingCount = failedBindingCount;
            Message = message ?? string.Empty;
        }

        public UIMotifTargetApplyStatus Status { get; }
        public int AppliedBindingCount { get; }
        public int KeptLocalBindingCount { get; }
        public int FailedBindingCount { get; }
        public string Message { get; }
        public bool Succeeded =>
            Status == UIMotifTargetApplyStatus.Applied ||
            Status == UIMotifTargetApplyStatus.Partial;
    }

    public interface IUIMotifTarget
    {
        UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot);
    }

    public enum UIMotifRegistrationStatus
    {
        Registered = 0,
        RegisteredWithApplyFailure = 1,
        InvalidTarget = 2,
        Unavailable = 3,
        Shutdown = 4
    }

    public readonly struct UIMotifRegistrationResult
    {
        public UIMotifRegistrationResult(
            UIMotifRegistrationStatus status,
            long generation = 0,
            UIMotifTargetApplyResult applyResult = default,
            string message = "")
        {
            Status = status;
            Generation = generation;
            ApplyResult = applyResult;
            Message = message ?? string.Empty;
        }

        public UIMotifRegistrationStatus Status { get; }
        public long Generation { get; }
        public UIMotifTargetApplyResult ApplyResult { get; }
        public string Message { get; }
        public bool Succeeded =>
            Status == UIMotifRegistrationStatus.Registered ||
            Status == UIMotifRegistrationStatus.RegisteredWithApplyFailure;
    }

    public enum UIMotifRegistrationReleaseStatus
    {
        Released = 0,
        AlreadyReleased = 1,
        Stale = 2,
        Unavailable = 3,
        Shutdown = 4
    }

    public readonly struct UIMotifRegistrationReleaseResult
    {
        public UIMotifRegistrationReleaseResult(
            UIMotifRegistrationReleaseStatus status,
            long generation = 0)
        {
            Status = status;
            Generation = generation;
        }

        public UIMotifRegistrationReleaseStatus Status { get; }
        public long Generation { get; }
        public bool Succeeded =>
            Status == UIMotifRegistrationReleaseStatus.Released ||
            Status == UIMotifRegistrationReleaseStatus.AlreadyReleased;
    }

    public sealed class UIMotifRegistrationHandle
    {
        private readonly UIMotifService service;
        private bool released;

        internal UIMotifRegistrationHandle(
            UIMotifService service,
            long generation,
            UIMotifRegistrationResult result)
        {
            this.service = service;
            Generation = generation;
            Result = result;
        }

        public long Generation { get; }
        public UIMotifRegistrationResult Result { get; }
        public bool IsReleased => released;

        public UIMotifRegistrationReleaseResult Release()
        {
            if (released)
                return new UIMotifRegistrationReleaseResult(
                    UIMotifRegistrationReleaseStatus.AlreadyReleased,
                    Generation);

            UIMotifRegistrationReleaseResult result =
                service == null
                    ? new UIMotifRegistrationReleaseResult(
                        UIMotifRegistrationReleaseStatus.Unavailable,
                        Generation)
                    : service.Release(this);

            if (result.Succeeded ||
                result.Status == UIMotifRegistrationReleaseStatus.Stale ||
                result.Status == UIMotifRegistrationReleaseStatus.Shutdown)
            {
                released = true;
            }

            return result;
        }
    }

    internal static class UIMotifUnityObjectUtility
    {
        public static bool IsDestroyed(object value) =>
            value is Object unityObject && unityObject == null;
    }
}
