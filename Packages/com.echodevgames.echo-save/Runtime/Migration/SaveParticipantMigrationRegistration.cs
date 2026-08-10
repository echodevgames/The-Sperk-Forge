using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Owns one active migration registry membership. Disposal is idempotent
    /// and token-protected so stale leases cannot remove replacements.
    /// </summary>
    public sealed class SaveParticipantMigrationRegistration :
        IDisposable
    {
        private SaveParticipantMigrationRegistry registry;
        private readonly SaveParticipantMigrationId migrationId;
        private readonly long ownershipToken;
        private bool disposed;

        internal SaveParticipantMigrationRegistration(
            SaveParticipantMigrationRegistry registry,
            SaveParticipantMigrationId migrationId,
            long ownershipToken)
        {
            this.registry =
                registry;

            this.migrationId =
                migrationId;

            this.ownershipToken =
                ownershipToken;
        }

        public SaveParticipantMigrationId MigrationId =>
            migrationId;

        public bool IsActive =>
            !disposed &&
            registry != null &&
            registry.Owns(
                migrationId,
                ownershipToken);

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed =
                true;

            SaveParticipantMigrationRegistry owner =
                registry;

            registry =
                null;

            owner?.Release(
                migrationId,
                ownershipToken);
        }
    }
}
