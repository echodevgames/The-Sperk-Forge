using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDevGames.EchoSave
{
    internal static class SaveParticipantMigrationExecutor
    {
        private const long MaxSerializedPayloadBytes =
            SaveUnknownPayloadStore.DefaultMaxAggregateBytes;

        internal static SaveParticipantMigrationExecutionResult
            Execute(
                SaveParticipantMigrationRegistry registry,
                SaveParticipantMigrationPlan plan,
                SaveParticipantMigrationInput input)
        {
            if (registry == null ||
                plan == null ||
                !SaveParticipantId.TryParse(
                    input.PersistedParticipantId.Value,
                    out SaveParticipantId persistedId) ||
                !SaveParticipantId.TryParse(
                    input.CanonicalParticipantId.Value,
                    out SaveParticipantId canonicalId) ||
                canonicalId !=
                    plan.ParticipantId ||
                input.SourceSchemaVersion !=
                    plan.SourceSchemaVersion ||
                input.SerializedPayload == null)
            {
                return Failure(
                    SaveParticipantMigrationExecutionStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationInvalidRequest,
                    "Chronicle participant migration execution requires one valid complete plan and matching detached migration input.");
            }

            int currentSchemaVersion =
                input.SourceSchemaVersion;

            SaveSerializerId currentSerializerId =
                input.SerializerId;

            string currentSerializedPayload =
                input.SerializedPayload;

            List<SaveParticipantMigrationProvenanceEntry>
                provenance =
                    new List<SaveParticipantMigrationProvenanceEntry>(
                        plan.Count);

            IReadOnlyList<SaveParticipantMigrationPlanStep>
                steps =
                    plan.Steps;

            for (int i = 0;
                 i < steps.Count;
                 i++)
            {
                SaveParticipantMigrationPlanStep
                    plannedStep =
                        steps[i];

                if (!registry.Owns(
                        plannedStep) ||
                    plannedStep.ParticipantId !=
                        canonicalId ||
                    plannedStep.FromSchemaVersion !=
                        currentSchemaVersion ||
                    plannedStep.ToSchemaVersion !=
                        currentSchemaVersion + 1)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.RegistryChanged,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationRegistryChanged,
                        "The Chronicle participant migration registry changed after planning; migration execution was aborted.");
                }

                SaveParticipantMigrationStepResult stepResult;

                try
                {
                    stepResult =
                        plannedStep.Step.Migrate(
                            new SaveParticipantMigrationInput(
                                persistedId,
                                canonicalId,
                                currentSchemaVersion,
                                currentSerializerId,
                                currentSerializedPayload,
                                input.Required,
                                input.Flags));
                }
                catch (Exception exception)
                    when (exception is ArgumentException ||
                          exception is InvalidOperationException ||
                          exception is NotSupportedException)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.StepFailed,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationStepFailed,
                        $"Chronicle participant migration '{plannedStep.MigrationId.Value}' threw {exception.GetType().Name}: {exception.Message}");
                }

                if (!stepResult.Succeeded)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.StepFailed,
                        string.IsNullOrEmpty(
                            stepResult.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .ParticipantMigrationStepFailed
                            : stepResult.DiagnosticCode,
                        stepResult.Message);
                }

                if (stepResult.TargetSchemaVersion !=
                    plannedStep.ToSchemaVersion)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.InvalidOutput,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationInvalidOutput,
                        "A Chronicle participant migration step returned an unexpected target schema version.");
                }

                SaveSerializerId nextSerializerId;

                try
                {
                    nextSerializerId =
                        new SaveSerializerId(
                            stepResult.SerializerId);
                }
                catch (ArgumentException)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.InvalidOutput,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationInvalidOutput,
                        "A Chronicle participant migration step returned an invalid serializer provider ID.");
                }

                if (!string.Equals(
                        nextSerializerId.Value,
                        stepResult.SerializerId,
                        StringComparison.Ordinal) ||
                    stepResult.SerializedPayload == null)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.InvalidOutput,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationInvalidOutput,
                        "A Chronicle participant migration step returned non-canonical serializer identity or a null serialized payload.");
                }

                long payloadBytes =
                    Encoding.UTF8.GetByteCount(
                        stepResult.SerializedPayload);

                if (payloadBytes >
                    MaxSerializedPayloadBytes)
                {
                    return Failure(
                        SaveParticipantMigrationExecutionStatus.InvalidOutput,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationInvalidOutput,
                        "A Chronicle participant migration step returned a serialized payload that exceeds the bounded in-memory migration size.");
                }

                provenance.Add(
                    new SaveParticipantMigrationProvenanceEntry(
                        plannedStep.MigrationId,
                        plannedStep.FromSchemaVersion,
                        plannedStep.ToSchemaVersion));

                currentSchemaVersion =
                    plannedStep.ToSchemaVersion;

                currentSerializerId =
                    nextSerializerId;

                currentSerializedPayload =
                    stepResult.SerializedPayload;
            }

            if (currentSchemaVersion !=
                plan.TargetSchemaVersion)
            {
                return Failure(
                    SaveParticipantMigrationExecutionStatus.InvalidOutput,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationInvalidOutput,
                    "Chronicle participant migration execution ended before the planned current schema version.");
            }

            return new SaveParticipantMigrationExecutionResult(
                SaveParticipantMigrationExecutionStatus.Succeeded,
                currentSchemaVersion,
                currentSerializerId,
                currentSerializedPayload,
                provenance.ToArray(),
                string.Empty,
                "The Chronicle participant migration chain executed successfully in memory.");
        }

        private static SaveParticipantMigrationExecutionResult
            Failure(
                SaveParticipantMigrationExecutionStatus status,
                string diagnosticCode,
                string message) =>
            new SaveParticipantMigrationExecutionResult(
                status,
                0,
                default,
                null,
                Array.Empty<
                    SaveParticipantMigrationProvenanceEntry>(),
                diagnosticCode,
                message);
    }
}
