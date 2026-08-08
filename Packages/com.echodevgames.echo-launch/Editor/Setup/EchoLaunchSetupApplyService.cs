using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupSnapshotSource
    {
        EchoLaunchProjectSnapshot Collect(EchoLaunchSetupRequest request);
    }

    internal interface IEchoLaunchSetupPlanSource
    {
        EchoLaunchSetupPlan CreatePlan(
            EchoLaunchSetupRequest request,
            EchoLaunchProjectSnapshot snapshot);
    }

    internal interface IEchoLaunchSetupFailureInjector
    {
        void ThrowIfRequested(EchoLaunchSetupOperationKind operationKind);
    }

    internal sealed class EchoLaunchSetupNoFailureInjector :
        IEchoLaunchSetupFailureInjector
    {
        public void ThrowIfRequested(
            EchoLaunchSetupOperationKind operationKind)
        {
        }
    }

    internal sealed class EchoLaunchSetupExecutionLog
    {
        private readonly List<EchoLaunchSetupChange> changes =
            new List<EchoLaunchSetupChange>();

        private readonly List<string> createdPaths =
            new List<string>();

        private readonly List<string> reusedPaths =
            new List<string>();

        private readonly List<string> repairedPaths =
            new List<string>();

        internal IReadOnlyList<EchoLaunchSetupChange> Changes => changes;
        internal IReadOnlyList<string> CreatedPaths => createdPaths;
        internal IReadOnlyList<string> ReusedPaths => reusedPaths;
        internal IReadOnlyList<string> RepairedPaths => repairedPaths;

        internal void Add(
            EchoLaunchSetupChangeKind kind,
            string path,
            string message)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            changes.Add(
                new EchoLaunchSetupChange(
                    kind,
                    normalized,
                    message));

            if (kind == EchoLaunchSetupChangeKind.CreatedFolder ||
                kind == EchoLaunchSetupChangeKind.CreatedAsset ||
                kind == EchoLaunchSetupChangeKind.CreatedPrefabVariant ||
                kind == EchoLaunchSetupChangeKind.CreatedScene)
            {
                AddUnique(createdPaths, normalized);
            }
            else if (kind == EchoLaunchSetupChangeKind.Reused)
            {
                AddUnique(reusedPaths, normalized);
            }
            else if (kind == EchoLaunchSetupChangeKind.RepairedAsset ||
                     kind == EchoLaunchSetupChangeKind.RepairedPrefab ||
                     kind == EchoLaunchSetupChangeKind.RepairedScene ||
                     kind == EchoLaunchSetupChangeKind.BuildSettingsChanged)
            {
                AddUnique(repairedPaths, normalized);
            }
        }

        internal void RecordPlanReuse(EchoLaunchSetupPlan plan)
        {
            if (plan == null)
            {
                return;
            }

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Reuse)
                {
                    Add(
                        EchoLaunchSetupChangeKind.Reused,
                        operation.TargetPath,
                        operation.Reason);
                }
            }
        }

        private static void AddUnique(
            List<string> paths,
            string value)
        {
            if (!string.IsNullOrEmpty(value) &&
                !paths.Contains(value))
            {
                paths.Add(value);
            }
        }
    }

    internal sealed class EchoLaunchSetupApplyService
    {
        private static int activeApplyState;

        private readonly IEchoLaunchSetupSnapshotSource snapshotSource;
        private readonly IEchoLaunchSetupPlanSource planSource;
        private readonly IEchoLaunchSetupAssetWriter assetWriter;
        private readonly IEchoLaunchSetupPrefabWriter prefabWriter;
        private readonly IEchoLaunchSetupSceneWriter sceneWriter;
        private readonly IEchoLaunchSetupBuildSettingsWriter
            buildSettingsWriter;
        private readonly IEchoLaunchSetupFailureInjector failureInjector;
        private readonly Func<bool> isEditorBusy;

        internal EchoLaunchSetupApplyService()
            : this(
                new EchoLaunchProjectSnapshotCollector(),
                new EchoLaunchSetupPlanner(),
                new EchoLaunchSetupAssetWriter(),
                new EchoLaunchSetupPrefabWriter(),
                new EchoLaunchSetupSceneWriter(),
                new EchoLaunchSetupBuildSettingsWriter(),
                new EchoLaunchSetupNoFailureInjector(),
                DefaultEditorBusy)
        {
        }

        internal EchoLaunchSetupApplyService(
            IEchoLaunchSetupSnapshotSource snapshotSource,
            IEchoLaunchSetupPlanSource planSource,
            IEchoLaunchSetupAssetWriter assetWriter,
            IEchoLaunchSetupPrefabWriter prefabWriter,
            IEchoLaunchSetupSceneWriter sceneWriter,
            IEchoLaunchSetupBuildSettingsWriter buildSettingsWriter,
            IEchoLaunchSetupFailureInjector failureInjector,
            Func<bool> isEditorBusy)
        {
            this.snapshotSource =
                snapshotSource ??
                throw new ArgumentNullException(nameof(snapshotSource));

            this.planSource =
                planSource ??
                throw new ArgumentNullException(nameof(planSource));

            this.assetWriter =
                assetWriter ??
                throw new ArgumentNullException(nameof(assetWriter));

            this.prefabWriter =
                prefabWriter ??
                throw new ArgumentNullException(nameof(prefabWriter));

            this.sceneWriter =
                sceneWriter ??
                throw new ArgumentNullException(nameof(sceneWriter));

            this.buildSettingsWriter =
                buildSettingsWriter ??
                throw new ArgumentNullException(nameof(buildSettingsWriter));

            this.failureInjector =
                failureInjector ??
                throw new ArgumentNullException(nameof(failureInjector));

            this.isEditorBusy =
                isEditorBusy ??
                throw new ArgumentNullException(nameof(isEditorBusy));
        }

        internal static bool IsApplyActive => IsMutationActive;

        internal static bool IsMutationActive =>
            Interlocked.CompareExchange(
                ref activeApplyState,
                0,
                0) != 0;

        internal static bool TryEnterMutationAuthority()
        {
            return Interlocked.CompareExchange(
                ref activeApplyState,
                1,
                0) == 0;
        }

        internal static void ExitMutationAuthority()
        {
            Interlocked.Exchange(ref activeApplyState, 0);
        }

        internal static EchoLaunchSetupApplyEligibility EvaluateEligibility(
            EchoLaunchSetupPlan plan,
            bool approvePlaceFirst)
        {
            if (plan == null)
            {
                return Blocked(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "Refresh a setup plan before applying.");
            }

            if (plan.Status == EchoLaunchSetupPlanStatus.Blocked)
            {
                return Blocked(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "The displayed setup plan is blocked.");
            }

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                if (!Enum.IsDefined(
                        typeof(EchoLaunchSetupOperationKind),
                        operation.Kind))
                {
                    return Blocked(
                        EchoLaunchSetupDiagnosticCodes
                            .UnauthorizedApplyOperation,
                        "The plan contains an unknown operation.");
                }

                if (operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Conflict ||
                    operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Unsupported)
                {
                    return Blocked(
                        operation.DiagnosticCode,
                        "The plan contains a non-executable operation.");
                }

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    return Blocked(
                        EchoLaunchSetupDiagnosticCodes
                            .UnauthorizedApplyOperation,
                        "The plan contains existing-asset repairs. Use Repair Plan instead of the create-only Apply action.");
                }

                if (operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Create &&
                    !IsCreateAuthorized(operation.Kind))
                {
                    return Blocked(
                        EchoLaunchSetupDiagnosticCodes
                            .UnauthorizedApplyOperation,
                        "The plan requests creation through an unauthorized operation kind.");
                }

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.ManualDecision)
                {
                    bool isApprovedBuildSettingsDecision =
                        operation.Kind ==
                            EchoLaunchSetupOperationKind
                                .ResolveBuildSettings &&
                        operation.RequiresExplicitApproval &&
                        approvePlaceFirst;

                    if (!isApprovedBuildSettingsDecision)
                    {
                        return Blocked(
                            string.IsNullOrEmpty(operation.DiagnosticCode)
                                ? EchoLaunchSetupDiagnosticCodes
                                    .UnauthorizedApplyOperation
                                : operation.DiagnosticCode,
                            "The plan contains an unresolved manual decision.");
                    }
                }

                if (operation.RequiresExplicitApproval &&
                    !approvePlaceFirst)
                {
                    return Blocked(
                        EchoLaunchSetupDiagnosticCodes
                            .BuildSettingsApproval,
                        "Explicit approval is required before moving Boot first.");
                }
            }

            return new EchoLaunchSetupApplyEligibility(
                true,
                string.Empty,
                "The displayed plan is eligible for a freshness check.");
        }

        internal EchoLaunchSetupApplyResult Apply(
            EchoLaunchSetupApplyRequest applyRequest)
        {
            if (applyRequest == null ||
                applyRequest.DisplayedPlan == null)
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Blocked,
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "A displayed setup plan is required.");
            }

            if (!applyRequest.Confirmed)
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Cancelled,
                    string.Empty,
                    "Setup apply was cancelled.",
                    applyRequest.DisplayedPlan);
            }

            if (isEditorBusy())
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Blocked,
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "Wait for Unity compilation, import, or Play Mode transition to finish.",
                    applyRequest.DisplayedPlan);
            }

            EchoLaunchSetupApplyEligibility displayedEligibility =
                EvaluateEligibility(
                    applyRequest.DisplayedPlan,
                    applyRequest.ApprovePlaceFirst);

            if (!displayedEligibility.CanApply)
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Blocked,
                    displayedEligibility.DiagnosticCode,
                    displayedEligibility.Message,
                    applyRequest.DisplayedPlan);
            }

            if (!TryEnterMutationAuthority())
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.AlreadyRunning,
                    EchoLaunchSetupDiagnosticCodes.ApplyAlreadyRunning,
                    "Another First Light setup apply operation is active.",
                    applyRequest.DisplayedPlan);
            }

            try
            {
                return ApplyUnderAuthority(applyRequest);
            }
            finally
            {
                ExitMutationAuthority();
            }
        }

        private EchoLaunchSetupApplyResult ApplyUnderAuthority(
            EchoLaunchSetupApplyRequest applyRequest)
        {
            EchoLaunchSetupPlan displayedPlan =
                applyRequest.DisplayedPlan;

            EchoLaunchProjectSnapshot freshSnapshot =
                snapshotSource.Collect(displayedPlan.Request);

            EchoLaunchSetupPlan freshPlan =
                planSource.CreatePlan(
                    displayedPlan.Request,
                    freshSnapshot);

            if (!string.Equals(
                    displayedPlan.PlanFingerprint,
                    freshPlan.PlanFingerprint,
                    StringComparison.Ordinal))
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.StalePlan,
                    EchoLaunchSetupDiagnosticCodes.StalePlan,
                    "Project evidence changed after preview. Refresh and review the plan.",
                    freshPlan);
            }

            EchoLaunchSetupApplyEligibility freshEligibility =
                EvaluateEligibility(
                    freshPlan,
                    applyRequest.ApprovePlaceFirst);

            if (!freshEligibility.CanApply)
            {
                return EchoLaunchSetupApplyResult.Simple(
                    EchoLaunchSetupApplyStatus.Blocked,
                    freshEligibility.DiagnosticCode,
                    freshEligibility.Message,
                    freshPlan);
            }

            EchoLaunchSetupExecutionLog log =
                new EchoLaunchSetupExecutionLog();

            log.RecordPlanReuse(freshPlan);

            if (!RequiresMutation(
                    freshPlan,
                    applyRequest.ApprovePlaceFirst))
            {
                return CreateSettledResult(
                    EchoLaunchSetupApplyStatus.NoChanges,
                    string.Empty,
                    "The project already matches the approved setup plan.",
                    log,
                    null,
                    freshPlan,
                    false);
            }

            EchoLaunchSetupRollbackJournal journal =
                new EchoLaunchSetupRollbackJournal();

            string buildSettingsBefore =
                EchoLaunchSetupBuildSettingsWriter.Summarize(
                    EditorBuildSettings.scenes);

            try
            {
                ExecutePlan(
                    freshPlan,
                    applyRequest.ApprovePlaceFirst,
                    journal,
                    log);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EchoLaunchProjectSnapshot finalSnapshot =
                    snapshotSource.Collect(freshPlan.Request);

                EchoLaunchSetupPlan finalPlan =
                    planSource.CreatePlan(
                        freshPlan.Request,
                        finalSnapshot);

                return new EchoLaunchSetupApplyResult(
                    EchoLaunchSetupApplyStatus.Succeeded,
                    string.Empty,
                    "First Light setup was applied successfully.",
                    log.Changes,
                    log.CreatedPaths,
                    log.ReusedPaths,
                    buildSettingsBefore,
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    false,
                    Array.Empty<string>(),
                    finalPlan.Status,
                    finalPlan.PlanFingerprint);
            }
            catch (Exception exception)
            {
                EchoLaunchSetupRollbackResult rollback =
                    journal.Rollback();

                EchoLaunchSetupPlan finalPlan = null;

                try
                {
                    EchoLaunchProjectSnapshot rollbackSnapshot =
                        snapshotSource.Collect(freshPlan.Request);

                    finalPlan =
                        planSource.CreatePlan(
                            freshPlan.Request,
                            rollbackSnapshot);
                }
                catch
                {
                }

                return new EchoLaunchSetupApplyResult(
                    rollback.Completed
                        ? EchoLaunchSetupApplyStatus.FailedRolledBack
                        : EchoLaunchSetupApplyStatus.FailedRollbackIncomplete,
                    rollback.Completed
                        ? EchoLaunchSetupDiagnosticCodes.ApplyFailedRolledBack
                        : EchoLaunchSetupDiagnosticCodes.RollbackIncomplete,
                    rollback.Completed
                        ? "Setup failed and active-attempt changes were rolled back. " +
                          exception.Message
                        : "Setup failed and rollback was incomplete. " +
                          exception.Message,
                    log.Changes,
                    log.CreatedPaths,
                    log.ReusedPaths,
                    buildSettingsBefore,
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    rollback.Completed,
                    rollback.ManualRecoveryPaths,
                    finalPlan == null
                        ? (EchoLaunchSetupPlanStatus?)null
                        : finalPlan.Status,
                    finalPlan == null
                        ? string.Empty
                        : finalPlan.PlanFingerprint);
            }
        }

        private void ExecutePlan(
            EchoLaunchSetupPlan plan,
            bool approvePlaceFirst,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            ExecuteFolderCreates(plan, journal, log);

            ExecuteAssetCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveStartupSequence,
                delegate(string path)
                {
                    assetWriter.CreateStartupSequence(path, journal, log);
                });

            ExecuteAssetCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination,
                delegate(string path)
                {
                    assetWriter.CreateLaunchDestination(
                        path,
                        plan.Request.DestinationScenePath,
                        journal,
                        log);
                });

            ExecuteAssetCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveSplashSequence,
                delegate(string path)
                {
                    assetWriter.CreateSplashSequence(path, journal, log);
                });

            EchoLaunchSetupOperation configuration =
                FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

            if (IsCreate(configuration))
            {
                failureInjector.ThrowIfRequested(
                    EchoLaunchSetupOperationKind.ResolveConfiguration);

                assetWriter.CreateConfiguration(
                    configuration.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveStartupSequence),
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveLaunchDestination),
                    ResolveOptionalPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveSplashSequence),
                    journal,
                    log);
            }

            EchoLaunchSetupOperation rootPrefab =
                FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);

            if (IsCreate(rootPrefab))
            {
                failureInjector.ThrowIfRequested(
                    EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);

                prefabWriter.CreateRootVariant(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                    rootPrefab.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveConfiguration),
                    journal,
                    log);
            }

            EchoLaunchSetupOperation bootScene =
                FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveBootScene);

            if (IsCreate(bootScene))
            {
                failureInjector.ThrowIfRequested(
                    EchoLaunchSetupOperationKind.ResolveBootScene);

                sceneWriter.CreateBootScene(
                    bootScene.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveRootPrefabVariant),
                    journal,
                    log);
            }

            EchoLaunchSetupOperation buildSettings =
                FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveBuildSettings);

            if (buildSettings != null &&
                (buildSettings.Disposition ==
                    EchoLaunchSetupOperationDisposition.Create ||
                 buildSettings.Disposition ==
                    EchoLaunchSetupOperationDisposition.ManualDecision))
            {
                failureInjector.ThrowIfRequested(
                    EchoLaunchSetupOperationKind.ResolveBuildSettings);

                buildSettingsWriter.Apply(
                    plan.Request.BuildSettingsPolicy,
                    plan.Paths.BootScenePath,
                    approvePlaceFirst,
                    journal,
                    log);
            }

            EchoLaunchSetupOperation destinationBuildSettings =
                FindOperation(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings);

            if (destinationBuildSettings != null &&
                destinationBuildSettings.Disposition ==
                    EchoLaunchSetupOperationDisposition.Create)
            {
                failureInjector.ThrowIfRequested(
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings);

                buildSettingsWriter.Apply(
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                    destinationBuildSettings.TargetPath,
                    false,
                    journal,
                    log);
            }
        }

        private void ExecuteFolderCreates(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                if (operation.Kind ==
                        EchoLaunchSetupOperationKind.EnsureFolder &&
                    IsCreate(operation))
                {
                    failureInjector.ThrowIfRequested(
                        EchoLaunchSetupOperationKind.EnsureFolder);

                    assetWriter.EnsureFolder(
                        operation.TargetPath,
                        journal,
                        log);
                }
            }
        }

        private void ExecuteAssetCreate(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind,
            Action<string> create)
        {
            EchoLaunchSetupOperation operation =
                FindOperation(plan, kind);

            if (!IsCreate(operation))
            {
                return;
            }

            failureInjector.ThrowIfRequested(kind);
            create(operation.TargetPath);
        }

        private static bool RequiresMutation(
            EchoLaunchSetupPlan plan,
            bool approvePlaceFirst)
        {
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Create)
                {
                    return true;
                }

                if (approvePlaceFirst &&
                    operation.Kind ==
                        EchoLaunchSetupOperationKind.ResolveBuildSettings &&
                    operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.ManualDecision)
                {
                    return true;
                }
            }

            return false;
        }

        private static EchoLaunchSetupOperation FindOperation(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                if (plan.Operations[index].Kind == kind)
                {
                    return plan.Operations[index];
                }
            }

            return null;
        }

        private static string ResolveRequiredPath(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            EchoLaunchSetupOperation operation =
                FindOperation(plan, kind);

            if (operation == null ||
                string.IsNullOrEmpty(operation.TargetPath))
            {
                throw new InvalidOperationException(
                    "The setup plan has no resolved path for " +
                    kind +
                    ".");
            }

            return operation.TargetPath;
        }

        private static string ResolveOptionalPath(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            EchoLaunchSetupOperation operation =
                FindOperation(plan, kind);

            return operation == null
                ? string.Empty
                : operation.TargetPath;
        }

        private static bool IsCreate(
            EchoLaunchSetupOperation operation)
        {
            return operation != null &&
                   operation.Disposition ==
                   EchoLaunchSetupOperationDisposition.Create;
        }


        private static bool IsCreateAuthorized(
            EchoLaunchSetupOperationKind kind)
        {
            return kind == EchoLaunchSetupOperationKind.EnsureFolder ||
                   kind == EchoLaunchSetupOperationKind.ResolveConfiguration ||
                   kind == EchoLaunchSetupOperationKind.ResolveStartupSequence ||
                   kind == EchoLaunchSetupOperationKind.ResolveLaunchDestination ||
                   kind == EchoLaunchSetupOperationKind.ResolveSplashSequence ||
                   kind == EchoLaunchSetupOperationKind.ResolveRootPrefabVariant ||
                   kind == EchoLaunchSetupOperationKind.ResolveBootScene ||
                   kind == EchoLaunchSetupOperationKind.ResolveBuildSettings ||
                   kind ==
                       EchoLaunchSetupOperationKind
                           .ResolveDestinationBuildSettings;
        }

        private static EchoLaunchSetupApplyResult CreateSettledResult(
            EchoLaunchSetupApplyStatus status,
            string diagnosticCode,
            string message,
            EchoLaunchSetupExecutionLog log,
            EchoLaunchSetupRollbackResult rollback,
            EchoLaunchSetupPlan finalPlan,
            bool rollbackCompleted)
        {
            return new EchoLaunchSetupApplyResult(
                status,
                diagnosticCode,
                message,
                log.Changes,
                log.CreatedPaths,
                log.ReusedPaths,
                EchoLaunchSetupBuildSettingsWriter.Summarize(
                    EditorBuildSettings.scenes),
                EchoLaunchSetupBuildSettingsWriter.Summarize(
                    EditorBuildSettings.scenes),
                rollbackCompleted,
                rollback == null
                    ? Array.Empty<string>()
                    : rollback.ManualRecoveryPaths,
                finalPlan == null
                    ? (EchoLaunchSetupPlanStatus?)null
                    : finalPlan.Status,
                finalPlan == null
                    ? string.Empty
                    : finalPlan.PlanFingerprint);
        }

        private static EchoLaunchSetupApplyEligibility Blocked(
            string diagnosticCode,
            string message)
        {
            return new EchoLaunchSetupApplyEligibility(
                false,
                diagnosticCode,
                message);
        }

        private static bool DefaultEditorBusy()
        {
            return EditorApplication.isCompiling ||
                   EditorApplication.isUpdating ||
                   EditorApplication.isPlayingOrWillChangePlaymode;
        }

        internal static void SetApplyActiveForTests(bool active)
        {
            Interlocked.Exchange(
                ref activeApplyState,
                active ? 1 : 0);
        }
    }
}
