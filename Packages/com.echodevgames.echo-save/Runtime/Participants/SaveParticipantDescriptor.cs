
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Stable persistence contract declared by one participant.
    ///
    /// The descriptor carries persistence identity and policy only. It never
    /// contains live gameplay state, Unity object references, or CLR type names
    /// loaded from save files.
    /// </summary>
    public readonly struct SaveParticipantDescriptor
    {
        public const int MaxAliases = 16;

        private readonly
            ReadOnlyCollection<SaveParticipantId> aliases;

        public SaveParticipantDescriptor(
            SaveParticipantId id,
            int currentSchemaVersion,
            SaveParticipantCriticality criticality,
            SaveMissingPayloadPolicy missingPayloadPolicy,
            SaveSerializerId serializerId,
            params SaveParticipantId[] aliases)
        {
            Id = id;
            CurrentSchemaVersion =
                currentSchemaVersion;
            Criticality =
                criticality;
            MissingPayloadPolicy =
                missingPayloadPolicy;
            SerializerId =
                serializerId;

            SaveParticipantId[] aliasCopy =
                aliases == null
                    ? Array.Empty<SaveParticipantId>()
                    : (SaveParticipantId[])
                        aliases.Clone();

            this.aliases =
                Array.AsReadOnly(
                    aliasCopy);
        }

        public SaveParticipantId Id { get; }

        public int CurrentSchemaVersion { get; }

        public SaveParticipantCriticality Criticality
        {
            get;
        }

        public SaveMissingPayloadPolicy
            MissingPayloadPolicy
        {
            get;
        }

        /// <summary>
        /// Default value means "use the Chronicle-selected default serializer".
        /// A non-default value selects that stable serializer provider ID.
        /// </summary>
        public SaveSerializerId SerializerId { get; }

        public IReadOnlyList<SaveParticipantId>
            Aliases =>
            aliases ??
            Array.AsReadOnly(
                Array.Empty<SaveParticipantId>());

        public bool TryValidate(
            out string diagnosticCode,
            out string message)
        {
            diagnosticCode =
                string.Empty;
            message =
                string.Empty;

            if (!SaveParticipantId.TryParse(
                    Id.Value,
                    out _))
            {
                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidId;
                message =
                    "The Chronicle participant canonical ID is invalid.";

                return false;
            }

            if (CurrentSchemaVersion <= 0)
            {
                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor;
                message =
                    "The Chronicle participant schema version must be positive.";

                return false;
            }

            if (Criticality !=
                    SaveParticipantCriticality.Required &&
                Criticality !=
                    SaveParticipantCriticality.Optional)
            {
                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor;
                message =
                    "The Chronicle participant criticality value is invalid.";

                return false;
            }

            if (MissingPayloadPolicy !=
                    SaveMissingPayloadPolicy.InitializeDefault &&
                MissingPayloadPolicy !=
                    SaveMissingPayloadPolicy.Ignore &&
                MissingPayloadPolicy !=
                    SaveMissingPayloadPolicy.Fail)
            {
                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor;
                message =
                    "The Chronicle participant missing-payload policy is invalid.";

                return false;
            }

            IReadOnlyList<SaveParticipantId>
                descriptorAliases =
                    Aliases;

            if (descriptorAliases.Count >
                MaxAliases)
            {
                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor;
                message =
                    $"The Chronicle participant descriptor exceeds the alias limit of {MaxAliases}.";

                return false;
            }

            HashSet<SaveParticipantId>
                uniqueAliases =
                    new HashSet<SaveParticipantId>();

            for (int i = 0;
                 i < descriptorAliases.Count;
                 i++)
            {
                SaveParticipantId alias =
                    descriptorAliases[i];

                if (!SaveParticipantId.TryParse(
                        alias.Value,
                        out _))
                {
                    diagnosticCode =
                        EchoSaveDiagnosticCodes
                            .ParticipantInvalidId;
                    message =
                        "A Chronicle participant alias is invalid.";

                    return false;
                }

                if (alias == Id)
                {
                    diagnosticCode =
                        EchoSaveDiagnosticCodes
                            .ParticipantInvalidDescriptor;
                    message =
                        "A Chronicle participant alias cannot equal its canonical ID.";

                    return false;
                }

                if (!uniqueAliases.Add(
                        alias))
                {
                    diagnosticCode =
                        EchoSaveDiagnosticCodes
                            .ParticipantInvalidDescriptor;
                    message =
                        "Chronicle participant aliases must be unique within one descriptor.";

                    return false;
                }
            }

            return true;
        }
    }
}
