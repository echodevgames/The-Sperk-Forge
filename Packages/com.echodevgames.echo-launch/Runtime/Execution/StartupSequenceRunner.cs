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
    /// FL-M3-01 captures immediate executor results only. Policy
    /// interpretation, exception conversion, timeout handling, retries,
    /// reporting, root integration, and lifecycle advancement belong to
    /// later checkpoints.
    /// </summary>
    internal sealed class StartupSequenceRunner
    {
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

                IStartupStepExecutor executor =
                    definition.CreateExecutor();

                if (executor == null)
                {
                    throw new InvalidOperationException(
                        $"Startup-step definition '{definition.StepId}' returned a null executor.");
                }

                StartupStepExecution execution =
                    new StartupStepExecution(
                        entry,
                        index,
                        authoredEntryCount,
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

                StartupStepResult result =
                    await executor.ExecuteAsync(
                        context);

                execution.Complete(result);

                completedExecutions.Add(
                    execution);
            }

            return new StartupSequenceRunResult(
                authoredEntryCount,
                disabledEntryCount,
                completedExecutions);
        }
    }
}

//----- StartupSequenceRunner.cs END -----
