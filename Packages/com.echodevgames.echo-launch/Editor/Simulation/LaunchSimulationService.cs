using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationService
    {
        private int activeRunState;

        internal static LaunchSimulationService Shared { get; } =
            new LaunchSimulationService();

        internal bool IsRunning =>
            Volatile.Read(ref activeRunState) != 0;

        internal async Awaitable<LaunchSimulationReport> RunAsync(
            LaunchSimulationRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return CreateInvalidRequest(
                    null,
                    "A Launch Simulator request is required.");
            }

            string validationMessage;
            if (!request.TryValidate(out validationMessage))
            {
                return CreateInvalidRequest(
                    request,
                    validationMessage);
            }

            if (Interlocked.CompareExchange(
                    ref activeRunState,
                    1,
                    0) != 0)
            {
                return CreateSimpleReport(
                    LaunchSimulationStatus.Busy,
                    request,
                    string.Empty,
                    LaunchSimulationDiagnosticCodes.Busy,
                    "A Launch Simulator run is already active.",
                    "Wait for settlement or cancel the active simulation.");
            }

            try
            {
                LaunchSimulationPlan plan =
                    LaunchSimulationPlan.Create(request);

                using (
                    LaunchSimulationTransientPlan transient =
                        LaunchSimulationTransientPlanBuilder.Build(
                            plan))
                {
                    LaunchSimulationObserver observer =
                        new LaunchSimulationObserver(plan);

                    StartupSequenceRunner runner =
                        new StartupSequenceRunner(
                            transient.Clock);

                    StartupSequenceRunResult result =
                        await runner.RunAsync(
                            LaunchMode.CanonicalBoot,
                            transient.Configuration,
                            cancellationToken,
                            observer);

                    return BuildCompletedReport(
                        plan,
                        result,
                        observer);
                }
            }
            catch (OperationCanceledException)
            {
                return CreateSimpleReport(
                    LaunchSimulationStatus.Cancelled,
                    request,
                    string.Empty,
                    LaunchSimulationDiagnosticCodes.Cancelled,
                    "The Launch Simulator run was cancelled.",
                    string.Empty);
            }
            catch (Exception exception)
            {
                return CreateSimpleReport(
                    LaunchSimulationStatus.InfrastructureFailure,
                    request,
                    string.Empty,
                    LaunchSimulationDiagnosticCodes
                        .InfrastructureFailure,
                    "The Launch Simulator could not complete its transient run.",
                    SanitizeException(exception));
            }
            finally
            {
                Volatile.Write(
                    ref activeRunState,
                    0);
            }
        }

        private static LaunchSimulationReport
            BuildCompletedReport(
                LaunchSimulationPlan plan,
                StartupSequenceRunResult result,
                LaunchSimulationObserver observer)
        {
            List<LaunchSimulationStepReport> steps =
                new List<LaunchSimulationStepReport>();

            for (int index = 0;
                 index < result.AttemptedExecutionCount;
                 index++)
            {
                StartupStepExecution execution =
                    result.GetExecution(index);

                LaunchSimulationStepPlan stepPlan =
                    plan.GetStep(execution.StepIndex);

                bool hasProgress =
                    observer.GetProgressCountForStep(
                        execution.StepIndex) > 0;



                StartupStepResult reportResult =
                    NormalizeStepResultForReport(
                        stepPlan,
                        execution.Result);

                double elapsed =
                    DetermineLogicalElapsed(
                        plan.Request,
                        stepPlan,
                        execution);

                steps.Add(
                    new LaunchSimulationStepReport(
                        stepPlan,
                        reportResult,
                        hasProgress,
                        execution.LatestProgress,
                        elapsed));
            }

            bool cancelled = result.WasCancelled;

            return new LaunchSimulationReport(
                cancelled
                    ? LaunchSimulationStatus.Cancelled
                    : LaunchSimulationStatus.Completed,
                plan.Request,
                plan.PlanFingerprint,
                result.AuthoredEntryCount,
                result.DisabledEntryCount,
                result.UnvisitedEntryCount,
                cancelled,
                steps.ToArray(),
                observer.CopyProgressSamples(),
                cancelled
                    ? LaunchSimulationDiagnosticCodes.Cancelled
                    : string.Empty,
                cancelled
                    ? "The Launch Simulator run was cancelled by the user."
                    : string.Empty,
                string.Empty);
        }

        private static StartupStepResult
            NormalizeStepResultForReport(
                LaunchSimulationStepPlan plan,
                StartupStepResult result)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            if (result == null)
            {
                throw new ArgumentNullException(
                    nameof(result));
            }

            if (plan.Behavior !=
                    LaunchSimulationStepBehavior
                        .WaitForCancellation ||
                result.Status != StartupStepStatus.Cancelled)
            {
                return result;
            }

            return StartupStepResult.Cancelled(
                result.Code,
                result.Message,
                "ExecutorCompletedWithoutException: False");
        }

        private static double DetermineLogicalElapsed(
            LaunchSimulationRequest request,
            LaunchSimulationStepPlan plan,
            StartupStepExecution execution)
        {
            if (plan.Behavior ==
                LaunchSimulationStepBehavior.TimedProgressSuccess)
            {
                return request.LogicalDurationSeconds;
            }

            if (plan.Behavior ==
                LaunchSimulationStepBehavior.WaitForTimeout)
            {
                return request.TimeoutSeconds;
            }

            if (plan.Behavior ==
                LaunchSimulationStepBehavior.WaitForCancellation)
            {
                return 0d;
            }

            return execution.HasTiming
                ? execution.Timing.ElapsedSeconds
                : 0d;
        }

        private static LaunchSimulationReport
            CreateInvalidRequest(
                LaunchSimulationRequest request,
                string message)
        {
            return CreateSimpleReport(
                LaunchSimulationStatus.InvalidRequest,
                request,
                string.Empty,
                LaunchSimulationDiagnosticCodes.InvalidRequest,
                message,
                string.Empty);
        }

        private static LaunchSimulationReport
            CreateSimpleReport(
                LaunchSimulationStatus status,
                LaunchSimulationRequest request,
                string planFingerprint,
                string diagnosticCode,
                string diagnosticMessage,
                string diagnosticDetails)
        {
            return new LaunchSimulationReport(
                status,
                request,
                planFingerprint,
                0,
                0,
                0,
                status == LaunchSimulationStatus.Cancelled,
                Array.Empty<LaunchSimulationStepReport>(),
                Array.Empty<LaunchSimulationProgressSample>(),
                diagnosticCode,
                diagnosticMessage,
                diagnosticDetails);
        }

        private static string SanitizeException(
            Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            string type =
                exception.GetType().FullName ??
                exception.GetType().Name;

            string message =
                string.IsNullOrWhiteSpace(exception.Message)
                    ? string.Empty
                    : exception.Message.Trim();

            return string.IsNullOrEmpty(message)
                ? "ExceptionType: " + type
                : "ExceptionType: " + type +
                  "\nExceptionMessage: " + message;
        }
    }
}
