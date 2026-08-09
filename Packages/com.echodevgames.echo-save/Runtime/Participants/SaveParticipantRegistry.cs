
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Application-session participant registry.
    ///
    /// This registry is deliberately open-ended: it has no predefined list of
    /// package or project participants. Any future system that implements the
    /// public participant contract may register if its descriptor is valid and
    /// its canonical/alias claims do not collide with active registrations.
    ///
    /// M3-01 registry operations are memory-only. They never invoke Capture or
    /// Apply and never touch Chronicle storage/publication.
    /// </summary>
    internal sealed class SaveParticipantRegistry
    {
        private sealed class Entry
        {
            internal Entry(
                ISaveParticipant participant,
                SaveParticipantDescriptor descriptor,
                long ownershipToken)
            {
                Participant =
                    participant;
                Descriptor =
                    descriptor;
                OwnershipToken =
                    ownershipToken;
            }

            internal ISaveParticipant Participant
            {
                get;
            }

            internal SaveParticipantDescriptor Descriptor
            {
                get;
            }

            internal long OwnershipToken
            {
                get;
            }
        }

        private readonly
            Dictionary<string, Entry> canonicalEntries =
                new Dictionary<string, Entry>(
                    StringComparer.Ordinal);

        private readonly
            Dictionary<string, Entry> identityClaims =
                new Dictionary<string, Entry>(
                    StringComparer.Ordinal);

        private long nextOwnershipToken;

        internal int Count =>
            canonicalEntries.Count;

        internal SaveParticipantRegistrationResult
            Register(
                ISaveParticipant participant)
        {
            if (participant == null)
            {
                return Failure(
                    SaveParticipantRegistrationStatus
                        .InvalidParticipant,
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor,
                    "A Chronicle participant instance is required.");
            }

            SaveParticipantDescriptor descriptor;

            try
            {
                descriptor =
                    participant.Descriptor;
            }
            catch (Exception exception)
            {
                return Failure(
                    SaveParticipantRegistrationStatus
                        .InvalidDescriptor,
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor,
                    $"The Chronicle participant descriptor could not be read. {exception.GetType().Name}: {exception.Message}");
            }

            if (!descriptor.TryValidate(
                    out string diagnosticCode,
                    out string validationMessage))
            {
                return Failure(
                    SaveParticipantRegistrationStatus
                        .InvalidDescriptor,
                    diagnosticCode,
                    validationMessage);
            }

            string canonical =
                descriptor.Id.Value;

            if (identityClaims.ContainsKey(
                    canonical))
            {
                Entry existing =
                    identityClaims[canonical];

                return Failure(
                    existing.Descriptor.Id.Value ==
                        canonical
                        ? SaveParticipantRegistrationStatus
                            .DuplicateId
                        : SaveParticipantRegistrationStatus
                            .AliasCollision,
                    existing.Descriptor.Id.Value ==
                        canonical
                        ? EchoSaveDiagnosticCodes
                            .ParticipantDuplicateId
                        : EchoSaveDiagnosticCodes
                            .ParticipantAliasCollision,
                    existing.Descriptor.Id.Value ==
                        canonical
                        ? "A Chronicle participant with this canonical ID is already registered."
                        : "The Chronicle participant canonical ID collides with an active participant alias.");
            }

            for (int i = 0;
                 i < descriptor.Aliases.Count;
                 i++)
            {
                string alias =
                    descriptor.Aliases[i]
                        .Value;

                if (identityClaims.ContainsKey(
                        alias))
                {
                    return Failure(
                        SaveParticipantRegistrationStatus
                            .AliasCollision,
                        EchoSaveDiagnosticCodes
                            .ParticipantAliasCollision,
                        "A Chronicle participant alias collides with an active canonical ID or alias.");
                }
            }

            if (nextOwnershipToken ==
                long.MaxValue)
            {
                return Failure(
                    SaveParticipantRegistrationStatus
                        .InvalidParticipant,
                    EchoSaveDiagnosticCodes
                        .ParticipantInvalidDescriptor,
                    "The Chronicle participant registry ownership-token space is exhausted.");
            }

            long token =
                ++nextOwnershipToken;

            Entry entry =
                new Entry(
                    participant,
                    descriptor,
                    token);

            canonicalEntries.Add(
                canonical,
                entry);

            identityClaims.Add(
                canonical,
                entry);

            for (int i = 0;
                 i < descriptor.Aliases.Count;
                 i++)
            {
                identityClaims.Add(
                    descriptor.Aliases[i]
                        .Value,
                    entry);
            }

            SaveParticipantRegistration
                registration =
                    new SaveParticipantRegistration(
                        this,
                        descriptor.Id,
                        token);

            return new SaveParticipantRegistrationResult(
                SaveParticipantRegistrationStatus.Succeeded,
                registration,
                string.Empty,
                "The Chronicle participant registered successfully.");
        }

        internal bool TryResolve(
            SaveParticipantId identity,
            out ISaveParticipant participant)
        {
            participant =
                null;

            if (!SaveParticipantId.TryParse(
                    identity.Value,
                    out SaveParticipantId validated))
            {
                return false;
            }

            if (!identityClaims.TryGetValue(
                    validated.Value,
                    out Entry entry))
            {
                return false;
            }

            participant =
                entry.Participant;

            return true;
        }

        internal SaveParticipantRegistrySnapshot
            GetSnapshot()
        {
            List<SaveParticipantDescriptor>
                descriptors =
                    new List<SaveParticipantDescriptor>(
                        canonicalEntries.Count);

            foreach (Entry entry in
                canonicalEntries.Values)
            {
                descriptors.Add(
                    entry.Descriptor);
            }

            descriptors.Sort(
                CompareDescriptors);

            return new SaveParticipantRegistrySnapshot(
                descriptors.ToArray());
        }

        internal void Clear()
        {
            canonicalEntries.Clear();
            identityClaims.Clear();
        }

        internal bool Owns(
            SaveParticipantId participantId,
            long ownershipToken)
        {
            return
                SaveParticipantId.TryParse(
                    participantId.Value,
                    out SaveParticipantId validated) &&
                canonicalEntries.TryGetValue(
                    validated.Value,
                    out Entry entry) &&
                entry.OwnershipToken ==
                    ownershipToken;
        }

        internal void Release(
            SaveParticipantId participantId,
            long ownershipToken)
        {
            if (!SaveParticipantId.TryParse(
                    participantId.Value,
                    out SaveParticipantId validated) ||
                !canonicalEntries.TryGetValue(
                    validated.Value,
                    out Entry entry) ||
                entry.OwnershipToken !=
                    ownershipToken)
            {
                return;
            }

            canonicalEntries.Remove(
                validated.Value);

            RemoveClaimIfOwned(
                validated.Value,
                entry);

            for (int i = 0;
                 i < entry.Descriptor.Aliases.Count;
                 i++)
            {
                RemoveClaimIfOwned(
                    entry.Descriptor.Aliases[i]
                        .Value,
                    entry);
            }
        }

        private void RemoveClaimIfOwned(
            string identity,
            Entry expectedOwner)
        {
            if (identityClaims.TryGetValue(
                    identity,
                    out Entry currentOwner) &&
                ReferenceEquals(
                    currentOwner,
                    expectedOwner))
            {
                identityClaims.Remove(
                    identity);
            }
        }

        private static int CompareDescriptors(
            SaveParticipantDescriptor left,
            SaveParticipantDescriptor right) =>
            left.Id.CompareTo(
                right.Id);

        private static SaveParticipantRegistrationResult
            Failure(
                SaveParticipantRegistrationStatus status,
                string diagnosticCode,
                string message) =>
            new SaveParticipantRegistrationResult(
                status,
                null,
                diagnosticCode,
                message);
    }
}
