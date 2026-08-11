using System;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Editor-only facade over the Runtime read-only inspection session.
    /// </summary>
    public sealed class EchoSaveInspectionService :
        IDisposable
    {
        private EchoSaveInspectionSession session;

        public EchoSaveBrowserRefreshResult Refresh(
            EchoSaveConfiguration configuration)
        {
            DisposeSession();

            EchoSaveInspectionOpenResult open =
                EchoSaveInspectionSession.TryOpen(
                    configuration,
                    out session);

            if (!open.Succeeded ||
                session == null)
            {
                return new EchoSaveBrowserRefreshResult(
                    open,
                    null,
                    null);
            }

            SaveSlotCatalogRefreshResult catalog =
                session.RefreshCatalog();

            return new EchoSaveBrowserRefreshResult(
                open,
                catalog,
                session.MigrationGraph);
        }

        public SaveGenerationInspectionSnapshot InspectSlot(
            SaveSlotId slotId)
        {
            if (session == null)
            {
                return null;
            }

            return session.InspectGenerations(
                slotId);
        }

        public SaveRecoveryPlan BuildRecoveryPlan(
            SaveSlotId slotId)
        {
            if (session == null)
            {
                return null;
            }

            return session.BuildRecoveryPlan(
                slotId);
        }

        public void Dispose()
        {
            DisposeSession();
        }

        private void DisposeSession()
        {
            if (session == null)
            {
                return;
            }

            session.Dispose();
            session = null;
        }
    }
}
