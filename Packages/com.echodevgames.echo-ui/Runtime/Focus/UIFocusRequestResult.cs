using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public readonly struct UIFocusRequestResult
    {
        public UIFocusRequestResult(
            UIFocusRequestStatus status,
            UIFocusResolutionSource source,
            GameObject selectedObject,
            long generation,
            string message)
        {
            Status = status;
            Source = source;
            SelectedObject = selectedObject;
            Generation = generation;
            Message = message ?? string.Empty;
        }

        public UIFocusRequestStatus Status { get; }
        public UIFocusResolutionSource Source { get; }
        public GameObject SelectedObject { get; }
        public long Generation { get; }
        public string Message { get; }

        public bool Succeeded =>
            Status == UIFocusRequestStatus.Succeeded ||
            Status == UIFocusRequestStatus.NoFocus ||
            Status == UIFocusRequestStatus.NoChange;

        public static UIFocusRequestResult Unavailable(
            long generation,
            string message) =>
            new UIFocusRequestResult(
                UIFocusRequestStatus.Unavailable,
                UIFocusResolutionSource.None,
                null,
                generation,
                message);
    }
}
