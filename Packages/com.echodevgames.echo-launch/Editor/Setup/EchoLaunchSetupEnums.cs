namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal enum EchoLaunchBuildSettingsPolicy
    {
        DoNotChange = 0,
        AddIfMissingAtEnd = 1,
        PlaceFirstAfterApproval = 2
    }

    internal enum EchoLaunchSetupPlanStatus
    {
        Ready = 0,
        ReadyWithWarnings = 1,
        Blocked = 2
    }

    internal enum EchoLaunchSetupOperationDisposition
    {
        Create = 0,
        Reuse = 1,
        NoChange = 2,
        ManualDecision = 3,
        Conflict = 4,
        Unsupported = 5,
        Repair = 6
    }

    internal enum EchoLaunchSetupOperationKind
    {
        ValidateRequest = 0,
        ValidatePackageTemplate = 1,
        EnsureFolder = 2,
        ResolveConfiguration = 3,
        ResolveStartupSequence = 4,
        ValidateDestinationScene = 5,
        ResolveLaunchDestination = 6,
        ResolveSplashSequence = 7,
        ResolveRootPrefabVariant = 8,
        ResolveBootScene = 9,
        ResolveBuildSettings = 10
    }

    internal enum EchoLaunchSetupDiagnosticSeverity
    {
        Information = 0,
        Warning = 1,
        Blocker = 2
    }

    internal enum EchoLaunchSetupAssetRole
    {
        Configuration = 0,
        StartupSequence = 1,
        LaunchDestination = 2,
        SplashSequence = 3,
        RootPrefab = 4
    }

    internal enum EchoLaunchSetupApplyStatus
    {
        Succeeded = 0,
        NoChanges = 1,
        Cancelled = 2,
        Blocked = 3,
        StalePlan = 4,
        AlreadyRunning = 5,
        FailedRolledBack = 6,
        FailedRollbackIncomplete = 7
    }

    internal enum EchoLaunchSetupRepairStatus
    {
        Succeeded = 0,
        NoChanges = 1,
        Cancelled = 2,
        Blocked = 3,
        StalePlan = 4,
        AlreadyRunning = 5,
        BackupFailed = 6,
        FailedRolledBack = 7,
        FailedRollbackIncomplete = 8
    }

    internal enum EchoLaunchSetupChangeKind
    {
        CreatedFolder = 0,
        CreatedAsset = 1,
        CreatedPrefabVariant = 2,
        CreatedScene = 3,
        BuildSettingsChanged = 4,
        Reused = 5,
        NoChange = 6,
        RepairedAsset = 7,
        RepairedPrefab = 8,
        RepairedScene = 9
    }
}
