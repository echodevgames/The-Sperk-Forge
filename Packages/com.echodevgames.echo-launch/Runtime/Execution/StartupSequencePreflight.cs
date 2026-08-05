//----- StartupSequencePreflight.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Validates one authored startup sequence before any executor factory is
    /// called.
    ///
    /// The preflight reads immutable definition data only. It does not repair,
    /// clamp, migrate, create executors, or write runtime state into assets.
    /// </summary>
    internal static class StartupSequencePreflight
    {
        internal const string ConfigurationDiagnosticCode =
            "ELAUNCH-CFG-001";

        internal const string SequenceDiagnosticCode =
            "ELAUNCH-SEQ-001";

        internal const string StepDiagnosticCode =
            "ELAUNCH-STEP-001";

        internal const string DuplicateStepDiagnosticCode =
            "ELAUNCH-STEP-002";

        /// <summary>
        /// Validates the active launch mode, configuration, sequence, entries,
        /// referenced definitions, schemas, identities, and duplicate
        /// identities. Enabled policy values are checked by the runner before
        /// executor creation so invalid policy keeps its structured blocking
        /// result behavior.
        ///
        /// Empty sequences remain valid in FL-M3-05. Disabled entries may omit
        /// a definition because they cannot create an executor.
        /// </summary>
        internal static StartupSequence Validate(
            LaunchMode launchMode,
            EchoLaunchConfiguration configuration)
        {
            ValidateLaunchMode(launchMode);

            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration));
            }

            if (!configuration.HasValidIdentity)
            {
                throw CreateInvalidOperation(
                    ConfigurationDiagnosticCode,
                    "The launch configuration identity is invalid.");
            }

            if (!configuration.HasSupportedSchema)
            {
                throw CreateInvalidOperation(
                    ConfigurationDiagnosticCode,
                    "The launch configuration schema version is unsupported.");
            }

            StartupSequence sequence =
                configuration.StartupSequence;

            if (sequence == null)
            {
                throw CreateInvalidOperation(
                    ConfigurationDiagnosticCode,
                    "The launch configuration does not reference a startup sequence.");
            }

            if (!sequence.HasValidIdentity)
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    "The startup-sequence identity is invalid.");
            }

            if (!sequence.HasSupportedSchema)
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    "The startup-sequence schema version is unsupported.");
            }

            ValidateEntries(sequence);
            return sequence;
        }

        private static void ValidateLaunchMode(
            LaunchMode launchMode)
        {
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
        }

        private static void ValidateEntries(
            StartupSequence sequence)
        {
            HashSet<string> entryIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            HashSet<string> stepIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            int entryCount =
                sequence.EntryCount;

            for (int index = 0;
                 index < entryCount;
                 index++)
            {
                StartupSequenceEntry entry =
                    sequence.GetEntry(index);

                ValidateEntry(
                    entry,
                    index,
                    entryIds,
                    stepIds);
            }
        }

        private static void ValidateEntry(
            StartupSequenceEntry entry,
            int index,
            HashSet<string> entryIds,
            HashSet<string> stepIds)
        {
            if (entry == null)
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    $"Startup-sequence entry {index} is null.");
            }

            if (!entry.HasValidIdentity)
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    $"Startup-sequence entry {index} has an invalid identity.");
            }

            if (!entryIds.Add(entry.EntryId))
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    $"Startup-sequence entry {index} duplicates entry identity '{entry.EntryId}'.");
            }

            if (!entry.HasValidActivation)
            {
                throw CreateInvalidOperation(
                    SequenceDiagnosticCode,
                    $"Startup-sequence entry {index} has an unsupported activation value.");
            }

            StartupStepDefinition definition =
                entry.StepDefinition;

            if (entry.IsEnabled &&
                definition == null)
            {
                throw CreateInvalidOperation(
                    StepDiagnosticCode,
                    $"Enabled startup-sequence entry {index} does not reference a step definition.");
            }

            if (definition != null)
            {
                ValidateDefinition(
                    definition,
                    index,
                    stepIds);
            }
        }

        private static void ValidateDefinition(
            StartupStepDefinition definition,
            int index,
            HashSet<string> stepIds)
        {
            if (!definition.HasValidIdentity)
            {
                throw CreateInvalidOperation(
                    StepDiagnosticCode,
                    $"Startup-sequence entry {index} references a step definition with an invalid identity.");
            }

            if (!definition.HasSupportedSchema)
            {
                throw CreateInvalidOperation(
                    StepDiagnosticCode,
                    $"Startup-sequence entry {index} references a step definition with an unsupported schema version.");
            }

            if (!stepIds.Add(definition.StepId))
            {
                throw CreateInvalidOperation(
                    DuplicateStepDiagnosticCode,
                    $"Startup-sequence entry {index} duplicates step identity '{definition.StepId}'.");
            }
        }

        private static InvalidOperationException
            CreateInvalidOperation(
                string diagnosticCode,
                string message)
        {
            return new InvalidOperationException(
                $"[{diagnosticCode}] {message}");
        }
    }
}

//----- StartupSequencePreflight.cs END -----
