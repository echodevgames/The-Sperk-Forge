namespace EchoDevGames.EchoUI
{
    public readonly struct UITransitionResult
    {
        public UITransitionResult(
            UITransitionStatus status,
            UITransitionOperationId operationId,
            long generation,
            string surfaceId,
            UITransitionDirection direction,
            string driverId,
            string profileId,
            double elapsedSeconds,
            string message)
        {
            Status = status;
            OperationId = operationId;
            Generation = generation;
            SurfaceId = surfaceId ?? string.Empty;
            Direction = direction;
            DriverId = driverId ?? string.Empty;
            ProfileId = profileId ?? string.Empty;
            ElapsedSeconds = elapsedSeconds < 0d ? 0d : elapsedSeconds;
            Message = message ?? string.Empty;
        }

        public UITransitionStatus Status { get; }
        public UITransitionOperationId OperationId { get; }
        public long Generation { get; }
        public string SurfaceId { get; }
        public UITransitionDirection Direction { get; }
        public string DriverId { get; }
        public string ProfileId { get; }
        public double ElapsedSeconds { get; }
        public string Message { get; }
        public bool Succeeded => Status == UITransitionStatus.Completed;

        public static UITransitionResult ForRequest(
            UITransitionRequest request,
            UITransitionStatus status,
            double elapsedSeconds = 0d,
            string message = "") =>
            new UITransitionResult(
                status,
                request == null ? default : request.OperationId,
                request == null ? 0 : request.Generation,
                request == null ? string.Empty : request.SurfaceId,
                request == null ? UITransitionDirection.Enter : request.Direction,
                request == null || request.Policy == null ? string.Empty : request.Policy.DriverId,
                request == null || request.Policy == null ? string.Empty : request.Policy.ProfileId,
                elapsedSeconds,
                message);
    }
}
