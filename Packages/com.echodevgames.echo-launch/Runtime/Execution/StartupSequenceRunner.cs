//----- StartupSequenceRunner.cs START -----

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Executes enabled startup-sequence entries in authored order.
    ///
    /// FL-M3-02 applies authored failure policy, converts bounded factory
    /// and executor failures into stable results, and stops traversal when
    /// the effective result requires it.
    ///
    /// Timeout handling, retries, reports, root integration, and lifecycle
    /// advancement remain later checkpoints.
    /// </summary>
    internal sealed class StartupSequenceRunner
    {
        private const int NoStoppingIndex = -1;

        /// <summary>
        /// Traverses one configured startup sequence and awaits each enabled
        /// entry's fresh executor.
        /// </summary>
        internal async Awaitable<StartupSequenceRunResult>
            RunAsync(
                LaunchMode launchMode,
                EchoLaunchConfiguration configuration,
                CancellationToken cancellationToken)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration));
            }

            if (!Enum.IsDefined(
                    typeof(LaunchMode),
                    launchMode) ||
                launchMode == LaunchMode.Unknown)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(launchMode),
                    launchMode,
                    "A defined active launch mode is required.");
            }

            StartupSequence sequence =
                configuration.StartupSequence;

            if (sequence == null)
            {
                throw new InvalidOperationException(
                    "The launch configuration does not reference a startup sequence.");
            }

            int authoredEntryCount =
                sequence.EntryCount;

            int disabledEntryCount = 0;

            int stoppingAuthoredEntryIndex =
                NoStoppingIndex;

            List<StartupStepExecution>
                completedExecutions =
                    new List<StartupStepExecution>();

            for (int index = 0;
                 index < authoredEntryCount;
                 index++)
            {
                StartupSequenceEntry entry =
                    sequence.GetEntry(index);

                if (entry == null)
                {
                    throw new InvalidOperationException(
                        $"Startup-sequence entry {index} is null.");
                }

                if (!entry.IsEnabled)
                {
                    disabledEntryCount++;
                    continue;
                }

                StartupStepDefinition definition =
                    entry.StepDefinition;

                if (definition == null)
                {
                    throw new InvalidOperationException(
                        $"Enabled startup-sequence entry {index} does not reference a step definition.");
                }

                StartupStepExecution execution =
                    new StartupStepExecution(
                        entry,
                        index,
                        authoredEntryCount);

                IStartupStepExecutor executor;

                try
                {
                    executor =
                        definition.CreateExecutor();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    StartupStepResult factoryFailure =
                        StartupStepExceptionConverter
                            .Convert(
                                StartupStepExceptionPhase
                                    .ExecutorFactory,
                                exception);

                    execution.CompleteBeforeStart(
                        factoryFailure);

                    completedExecutions.Add(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                if (executor == null)
                {
                    execution.CompleteBeforeStart(
                        StartupStepExceptionConverter
                            .CreateNullExecutorResult());

                    completedExecutions.Add(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                execution.AttachExecutor(
                    executor);

                StartupStepContext context =
                    new StartupStepContext(
                        launchMode,
                        configuration.ConfigurationId,
                        sequence.SequenceId,
                        execution.EntryId,
                        execution.StepId,
                        index,
                        authoredEntryCount,
                        cancellationToken,
                        execution);

                execution.Begin();

                StartupStepResult originalResult;

                try
                {
                    originalResult =
                        await executor.ExecuteAsync(
                            context);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    originalResult =
                        StartupStepExceptionConverter
                            .Convert(
                                StartupStepExceptionPhase
                                    .ExecutorExecution,
                                exception);
                }

                if (originalResult == null)
                {
                    execution.Complete(
                        StartupStepExceptionConverter
                            .CreateNullResult());

                    completedExecutions.Add(
                        execution);

                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }

                StartupStepPolicyDecision decision =
                    ApplyPolicy(
                        execution.Policy,
                        originalResult);

                execution.Complete(
                    decision.EffectiveResult);

                completedExecutions.Add(
                    execution);

                if (decision.StopsTraversal)
                {
                    stoppingAuthoredEntryIndex =
                        index;

                    break;
                }
            }

            return new StartupSequenceRunResult(
                authoredEntryCount,
                disabledEntryCount,
                completedExecutions,
                stoppingAuthoredEntryIndex);
        }

        private static StartupStepPolicyDecision
            ApplyPolicy(
                StartupStepPolicy policy,
                StartupStepResult originalResult)
        {
            if (policy.IsValid)
            {
                return StartupStepPolicyEvaluator
                    .Evaluate(
                        policy,
                        originalResult);
            }

            StartupStepResult invalidPolicyResult =
                StartupStepResult.BlockingFailure(
                    StartupStepExceptionConverter
                        .DiagnosticCode,
                    "The startup-step policy contains unsupported authored values.",
                    "ContractFailure: InvalidPolicy");

            return new StartupStepPolicyDecision(
                originalResult,
                invalidPolicyResult,
                false);
        }
    }
}

//----- StartupSequenceRunner.cs END -----
