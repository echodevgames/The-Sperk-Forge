using System;
using System.Collections.Generic;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupRepairService
    {
        private readonly IEchoLaunchSetupSnapshotSource snapshotSource;
        private readonly IEchoLaunchSetupPlanSource planSource;
        private readonly IEchoLaunchSetupAssetWriter assetWriter;
        private readonly IEchoLaunchSetupPrefabWriter prefabWriter;
        private readonly IEchoLaunchSetupSceneWriter sceneWriter;
        private readonly IEchoLaunchSetupBuildSettingsWriter buildSettingsWriter;
        private readonly IEchoLaunchSetupRepairBackupStore backupStore;
        private readonly IEchoLaunchSetupFailureInjector failureInjector;
        private readonly Func<bool> isEditorBusy;

        internal EchoLaunchSetupRepairService()
            : this(
                new EchoLaunchProjectSnapshotCollector(),
                new EchoLaunchSetupPlanner(),
                new EchoLaunchSetupAssetWriter(),
                new EchoLaunchSetupPrefabWriter(),
                new EchoLaunchSetupSceneWriter(),
                new EchoLaunchSetupBuildSettingsWriter(),
                new EchoLaunchSetupRepairBackupStore(),
                new EchoLaunchSetupNoFailureInjector(),
                DefaultEditorBusy)
        {
        }

        internal EchoLaunchSetupRepairService(
            IEchoLaunchSetupSnapshotSource snapshotSource,
            IEchoLaunchSetupPlanSource planSource,
            IEchoLaunchSetupAssetWriter assetWriter,
            IEchoLaunchSetupPrefabWriter prefabWriter,
            IEchoLaunchSetupSceneWriter sceneWriter,
            IEchoLaunchSetupBuildSettingsWriter buildSettingsWriter,
            IEchoLaunchSetupRepairBackupStore backupStore,
            IEchoLaunchSetupFailureInjector failureInjector,
            Func<bool> isEditorBusy)
        {
            this.snapshotSource = snapshotSource ??
                throw new ArgumentNullException(nameof(snapshotSource));
            this.planSource = planSource ??
                throw new ArgumentNullException(nameof(planSource));
            this.assetWriter = assetWriter ??
                throw new ArgumentNullException(nameof(assetWriter));
            this.prefabWriter = prefabWriter ??
                throw new ArgumentNullException(nameof(prefabWriter));
            this.sceneWriter = sceneWriter ??
                throw new ArgumentNullException(nameof(sceneWriter));
            this.buildSettingsWriter = buildSettingsWriter ??
                throw new ArgumentNullException(nameof(buildSettingsWriter));
            this.backupStore = backupStore ??
                throw new ArgumentNullException(nameof(backupStore));
            this.failureInjector = failureInjector ??
                throw new ArgumentNullException(nameof(failureInjector));
            this.isEditorBusy = isEditorBusy ??
                throw new ArgumentNullException(nameof(isEditorBusy));
        }

        internal static EchoLaunchSetupRepairEligibility EvaluateEligibility(
            EchoLaunchSetupPlan plan,
            bool approvePlaceFirst)
        {
            if (plan == null)
            {
                return Blocked(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "Refresh a setup plan before repairing.");
            }

            if (plan.Status == EchoLaunchSetupPlanStatus.Blocked)
            {
                return Blocked(
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "The displayed setup plan is blocked.");
            }

            bool hasRepair = false;
            bool hasCreate = false;
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
                        EchoLaunchSetupOperationDisposition.Unsupported ||
                    operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.ManualDecision)
                {
                    return Blocked(
                        string.IsNullOrEmpty(operation.DiagnosticCode)
                            ? EchoLaunchSetupDiagnosticCodes
                                .AmbiguousRepairEvidence
                            : operation.DiagnosticCode,
                        "The plan contains a non-executable repair operation.");
                }

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    if (!IsRepairAuthorized(operation.Kind))
                    {
                        return Blocked(
                            EchoLaunchSetupDiagnosticCodes
                                .UnauthorizedApplyOperation,
                            "The plan requests an unauthorized repair operation kind.");
                    }

                    hasRepair = true;
                    if (operation.RequiresExplicitApproval &&
                        !approvePlaceFirst)
                    {
                        return Blocked(
                            EchoLaunchSetupDiagnosticCodes
                                .BuildSettingsApproval,
                            "Explicit approval is required before the Build Settings repair.");
                    }
                }

                if (operation.Disposition ==
                    EchoLaunchSetupOperationDisposition.Create)
                {
                    hasCreate = true;
                }
            }

            if (!hasRepair && hasCreate)
            {
                return Blocked(
                    EchoLaunchSetupDiagnosticCodes
                        .RepairApprovalRequired,
                    "This is a create-only plan. Use Apply Plan instead.");
            }

            return new EchoLaunchSetupRepairEligibility(
                true,
                string.Empty,
                hasRepair
                    ? "The displayed repair plan is eligible for a freshness check."
                    : "The project is already reconciled; repair will settle as NoChanges.");
        }

        internal EchoLaunchSetupRepairResult Repair(
            EchoLaunchSetupRepairRequest request)
        {
            if (request == null || request.DisplayedPlan == null)
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.Blocked,
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "A displayed setup plan is required.");
            }

            if (!request.Confirmed)
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.Cancelled,
                    string.Empty,
                    "Setup repair was cancelled.",
                    request.DisplayedPlan);
            }

            if (isEditorBusy())
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.Blocked,
                    EchoLaunchSetupDiagnosticCodes.InvalidRequest,
                    "Wait for Unity compilation, import, or Play Mode transition to finish.",
                    request.DisplayedPlan);
            }

            EchoLaunchSetupRepairEligibility eligibility =
                EvaluateEligibility(
                    request.DisplayedPlan,
                    request.ApprovePlaceFirst);
            if (!eligibility.CanRepair)
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.Blocked,
                    eligibility.DiagnosticCode,
                    eligibility.Message,
                    request.DisplayedPlan);
            }

            if (!EchoLaunchSetupApplyService.TryEnterMutationAuthority())
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.AlreadyRunning,
                    EchoLaunchSetupDiagnosticCodes.ApplyAlreadyRunning,
                    "Another First Light setup mutation is active.",
                    request.DisplayedPlan);
            }

            try
            {
                return RepairUnderAuthority(request);
            }
            finally
            {
                EchoLaunchSetupApplyService.ExitMutationAuthority();
            }
        }

        private EchoLaunchSetupRepairResult RepairUnderAuthority(
            EchoLaunchSetupRepairRequest request)
        {
            EchoLaunchSetupPlan displayedPlan = request.DisplayedPlan;
            EchoLaunchProjectSnapshot freshSnapshot =
                snapshotSource.Collect(displayedPlan.Request);
            EchoLaunchSetupPlan freshPlan =
                planSource.CreatePlan(displayedPlan.Request, freshSnapshot);

            if (!string.Equals(
                    displayedPlan.PlanFingerprint,
                    freshPlan.PlanFingerprint,
                    StringComparison.Ordinal))
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.StalePlan,
                    EchoLaunchSetupDiagnosticCodes.StalePlan,
                    "Project evidence changed after preview. Refresh and review the repair plan.",
                    freshPlan);
            }

            EchoLaunchSetupRepairEligibility freshEligibility =
                EvaluateEligibility(
                    freshPlan,
                    request.ApprovePlaceFirst);
            if (!freshEligibility.CanRepair)
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.Blocked,
                    freshEligibility.DiagnosticCode,
                    freshEligibility.Message,
                    freshPlan);
            }

            EchoLaunchSetupExecutionLog log =
                new EchoLaunchSetupExecutionLog();
            log.RecordPlanReuse(freshPlan);

            if (!RequiresMutation(freshPlan))
            {
                return CreateResult(
                    EchoLaunchSetupRepairStatus.NoChanges,
                    string.Empty,
                    "The project already matches the approved repair plan.",
                    log,
                    string.Empty,
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    false,
                    Array.Empty<string>(),
                    freshPlan,
                    freshPlan);
            }

            EchoLaunchSetupRepairBackupSession backup;
            try
            {
                backup = backupStore.CreateBackup(
                    CollectRepairAssetPaths(freshPlan));
            }
            catch (Exception exception)
            {
                return EchoLaunchSetupRepairResult.Simple(
                    EchoLaunchSetupRepairStatus.BackupFailed,
                    EchoLaunchSetupDiagnosticCodes.RepairBackupFailed,
                    "Repair did not start because exact-byte backup failed: " +
                    exception.Message,
                    freshPlan);
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
                    request.ApprovePlaceFirst,
                    journal,
                    log);
                AssetDatabase.Refresh(
                    ImportAssetOptions.ForceSynchronousImport);

                EchoLaunchSetupPlan finalPlan =
                    planSource.CreatePlan(
                        freshPlan.Request,
                        snapshotSource.Collect(freshPlan.Request));

                if (finalPlan.HasBlockers ||
                    finalPlan.HasCreates ||
                    finalPlan.HasRepairs)
                {
                    throw new InvalidOperationException(
                        "Post-repair validation did not settle to a fully reconciled plan.");
                }

                backup.DeleteBackup();

                return CreateResult(
                    EchoLaunchSetupRepairStatus.Succeeded,
                    string.Empty,
                    "First Light setup repair completed successfully.",
                    log,
                    string.Empty,
                    buildSettingsBefore,
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    false,
                    Array.Empty<string>(),
                    finalPlan,
                    freshPlan);
            }
            catch (Exception exception)
            {
                EchoLaunchSetupRollbackResult createRollback =
                    journal.Rollback();
                EchoLaunchSetupRollbackResult backupRollback =
                    backup.Restore();
                List<string> manualRecovery = new List<string>();
                AddUnique(
                    manualRecovery,
                    createRollback.ManualRecoveryPaths);
                AddUnique(
                    manualRecovery,
                    backupRollback.ManualRecoveryPaths);
                bool completed =
                    createRollback.Completed && backupRollback.Completed;
                if (completed)
                {
                    try
                    {
                        backup.DeleteBackup();
                    }
                    catch
                    {
                        AddUnique(
                            manualRecovery,
                            backup.BackupDirectory);
                        completed = false;
                    }
                }
                else
                {
                    AddUnique(
                        manualRecovery,
                        backup.BackupDirectory);
                }

                EchoLaunchSetupPlan finalPlan = null;
                try
                {
                    finalPlan = planSource.CreatePlan(
                        freshPlan.Request,
                        snapshotSource.Collect(freshPlan.Request));
                }
                catch
                {
                    AddUnique(manualRecovery, "Fresh setup validation");
                    completed = false;
                }

                return CreateResult(
                    completed
                        ? EchoLaunchSetupRepairStatus.FailedRolledBack
                        : EchoLaunchSetupRepairStatus
                            .FailedRollbackIncomplete,
                    completed
                        ? EchoLaunchSetupDiagnosticCodes
                            .RepairFailedRolledBack
                        : EchoLaunchSetupDiagnosticCodes
                            .RepairRollbackIncomplete,
                    "Setup repair failed: " + exception.Message,
                    log,
                    completed ? string.Empty : backup.BackupDirectory,
                    buildSettingsBefore,
                    EchoLaunchSetupBuildSettingsWriter.Summarize(
                        EditorBuildSettings.scenes),
                    completed,
                    manualRecovery,
                    finalPlan,
                    freshPlan);
            }
        }

        private void ExecutePlan(
            EchoLaunchSetupPlan plan,
            bool approvePlaceFirst,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            ExecuteFolderCreates(plan, journal, log);

            ExecuteCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveStartupSequence,
                delegate(string path)
                {
                    assetWriter.CreateStartupSequence(path, journal, log);
                });
            ExecuteCreate(
                plan,
                EchoLaunchSetupOperationKind.ResolveSplashSequence,
                delegate(string path)
                {
                    assetWriter.CreateSplashSequence(path, journal, log);
                });

            EchoLaunchSetupOperation destination = FindOperation(
                plan,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination);
            if (IsCreate(destination))
            {
                failureInjector.ThrowIfRequested(destination.Kind);
                assetWriter.CreateLaunchDestination(
                    destination.TargetPath,
                    plan.Request.DestinationScenePath,
                    journal,
                    log);
            }
            else if (IsRepair(destination))
            {
                failureInjector.ThrowIfRequested(destination.Kind);
                assetWriter.RepairLaunchDestination(
                    destination.TargetPath,
                    plan.Request.DestinationScenePath,
                    log);
            }

            EchoLaunchSetupOperation configuration = FindOperation(
                plan,
                EchoLaunchSetupOperationKind.ResolveConfiguration);
            string sequencePath = ResolveRequiredPath(
                plan,
                EchoLaunchSetupOperationKind.ResolveStartupSequence);
            string destinationPath = ResolveRequiredPath(
                plan,
                EchoLaunchSetupOperationKind.ResolveLaunchDestination);
            string splashPath = ResolveOptionalPath(
                plan,
                EchoLaunchSetupOperationKind.ResolveSplashSequence);
            if (IsCreate(configuration))
            {
                failureInjector.ThrowIfRequested(configuration.Kind);
                assetWriter.CreateConfiguration(
                    configuration.TargetPath,
                    sequencePath,
                    destinationPath,
                    splashPath,
                    journal,
                    log);
            }
            else if (IsRepair(configuration))
            {
                failureInjector.ThrowIfRequested(configuration.Kind);
                assetWriter.RepairConfiguration(
                    configuration.TargetPath,
                    sequencePath,
                    destinationPath,
                    splashPath,
                    log);
            }

            EchoLaunchSetupOperation root = FindOperation(
                plan,
                EchoLaunchSetupOperationKind.ResolveRootPrefabVariant);
            if (IsCreate(root))
            {
                failureInjector.ThrowIfRequested(root.Kind);
                prefabWriter.CreateRootVariant(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                    root.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveConfiguration),
                    journal,
                    log);
            }
            else if (IsRepair(root))
            {
                failureInjector.ThrowIfRequested(root.Kind);
                prefabWriter.RepairRootConfiguration(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                    root.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveConfiguration),
                    log);
            }

            EchoLaunchSetupOperation boot = FindOperation(
                plan,
                EchoLaunchSetupOperationKind.ResolveBootScene);
            if (IsCreate(boot))
            {
                failureInjector.ThrowIfRequested(boot.Kind);
                sceneWriter.CreateBootScene(
                    boot.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveRootPrefabVariant),
                    journal,
                    log);
            }
            else if (IsRepair(boot))
            {
                failureInjector.ThrowIfRequested(boot.Kind);
                sceneWriter.RepairBootSceneWithRoot(
                    boot.TargetPath,
                    ResolveRequiredPath(
                        plan,
                        EchoLaunchSetupOperationKind.ResolveRootPrefabVariant),
                    log);
            }

            EchoLaunchSetupOperation build = FindOperation(
                plan,
                EchoLaunchSetupOperationKind.ResolveBuildSettings);
            if (IsCreate(build))
            {
                failureInjector.ThrowIfRequested(build.Kind);
                buildSettingsWriter.Apply(
                    plan.Request.BuildSettingsPolicy,
                    plan.Paths.BootScenePath,
                    approvePlaceFirst,
                    journal,
                    log);
            }
            else if (IsRepair(build))
            {
                failureInjector.ThrowIfRequested(build.Kind);
                buildSettingsWriter.Repair(
                    plan.Request.BuildSettingsPolicy,
                    plan.Paths.BootScenePath,
                    approvePlaceFirst,
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
                    failureInjector.ThrowIfRequested(operation.Kind);
                    assetWriter.EnsureFolder(
                        operation.TargetPath,
                        journal,
                        log);
                }
            }
        }

        private void ExecuteCreate(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind,
            Action<string> action)
        {
            EchoLaunchSetupOperation operation = FindOperation(plan, kind);
            if (!IsCreate(operation))
            {
                return;
            }
            failureInjector.ThrowIfRequested(kind);
            action(operation.TargetPath);
        }

        private static IEnumerable<string> CollectRepairAssetPaths(
            EchoLaunchSetupPlan plan)
        {
            List<string> result = new List<string>();
            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];
                if (operation.Disposition ==
                        EchoLaunchSetupOperationDisposition.Repair &&
                    operation.Kind !=
                        EchoLaunchSetupOperationKind.ResolveBuildSettings &&
                    !result.Contains(operation.TargetPath))
                {
                    result.Add(operation.TargetPath);
                }
            }
            return result;
        }

        private static bool RequiresMutation(EchoLaunchSetupPlan plan)
        {
            return plan.HasCreates || plan.HasRepairs;
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
            EchoLaunchSetupOperation operation = FindOperation(plan, kind);
            if (operation == null ||
                string.IsNullOrEmpty(operation.TargetPath))
            {
                throw new InvalidOperationException(
                    "The setup plan has no resolved path for " + kind + ".");
            }
            return operation.TargetPath;
        }

        private static string ResolveOptionalPath(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            EchoLaunchSetupOperation operation = FindOperation(plan, kind);
            return operation == null ? string.Empty : operation.TargetPath;
        }

        private static bool IsRepairAuthorized(
            EchoLaunchSetupOperationKind kind)
        {
            return kind == EchoLaunchSetupOperationKind.ResolveConfiguration ||
                   kind == EchoLaunchSetupOperationKind.ResolveLaunchDestination ||
                   kind == EchoLaunchSetupOperationKind.ResolveRootPrefabVariant ||
                   kind == EchoLaunchSetupOperationKind.ResolveBootScene ||
                   kind == EchoLaunchSetupOperationKind.ResolveBuildSettings;
        }

        private static bool IsCreate(EchoLaunchSetupOperation operation)
        {
            return operation != null &&
                   operation.Disposition ==
                   EchoLaunchSetupOperationDisposition.Create;
        }

        private static bool IsRepair(EchoLaunchSetupOperation operation)
        {
            return operation != null &&
                   operation.Disposition ==
                   EchoLaunchSetupOperationDisposition.Repair;
        }

        private static EchoLaunchSetupRepairResult CreateResult(
            EchoLaunchSetupRepairStatus status,
            string diagnosticCode,
            string message,
            EchoLaunchSetupExecutionLog log,
            string backupDirectory,
            string buildSettingsBefore,
            string buildSettingsAfter,
            bool rollbackCompleted,
            IEnumerable<string> manualRecovery,
            EchoLaunchSetupPlan finalPlan,
            EchoLaunchSetupPlan executedPlan)
        {
            return new EchoLaunchSetupRepairResult(
                status,
                diagnosticCode,
                message,
                log.Changes,
                log.CreatedPaths,
                log.RepairedPaths,
                log.ReusedPaths,
                backupDirectory,
                buildSettingsBefore,
                buildSettingsAfter,
                rollbackCompleted,
                manualRecovery,
                finalPlan == null
                    ? (EchoLaunchSetupPlanStatus?)null
                    : finalPlan.Status,
                finalPlan == null
                    ? string.Empty
                    : finalPlan.PlanFingerprint,
                CollectDispositionPaths(
                    executedPlan,
                    EchoLaunchSetupOperationDisposition.NoChange),
                CollectRepairOperations(executedPlan));
        }

        private static IEnumerable<string> CollectDispositionPaths(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationDisposition disposition)
        {
            List<string> result = new List<string>();
            if (plan == null)
            {
                return result;
            }

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                EchoLaunchSetupOperation operation = plan.Operations[index];
                if (operation.Disposition == disposition &&
                    !string.IsNullOrEmpty(operation.TargetPath) &&
                    !result.Contains(operation.TargetPath))
                {
                    result.Add(operation.TargetPath);
                }
            }

            return result;
        }

        private static IEnumerable<EchoLaunchSetupOperation>
            CollectRepairOperations(EchoLaunchSetupPlan plan)
        {
            List<EchoLaunchSetupOperation> result =
                new List<EchoLaunchSetupOperation>();
            if (plan == null)
            {
                return result;
            }

            for (int index = 0; index < plan.Operations.Count; index++)
            {
                if (plan.Operations[index].Disposition ==
                    EchoLaunchSetupOperationDisposition.Repair)
                {
                    result.Add(plan.Operations[index]);
                }
            }

            return result;
        }

        private static EchoLaunchSetupRepairEligibility Blocked(
            string diagnosticCode,
            string message)
        {
            return new EchoLaunchSetupRepairEligibility(
                false,
                diagnosticCode,
                message);
        }

        private static void AddUnique(
            List<string> target,
            IEnumerable<string> source)
        {
            if (source == null)
            {
                return;
            }
            foreach (string value in source)
            {
                if (!target.Contains(value))
                {
                    target.Add(value);
                }
            }
        }

        private static void AddUnique(
            List<string> target,
            string value)
        {
            if (!target.Contains(value))
            {
                target.Add(value);
            }
        }

        private static bool DefaultEditorBusy()
        {
            return EditorApplication.isCompiling ||
                   EditorApplication.isUpdating ||
                   EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
