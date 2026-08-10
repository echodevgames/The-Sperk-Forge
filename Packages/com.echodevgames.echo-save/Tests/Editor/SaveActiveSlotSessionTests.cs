
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveActiveSlotSessionTests
    {
        [Test]
        public void ActiveSlotBeginsUnsetAndRefreshNeverAutoSelects()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();

            Assert.That(
                catalog.HasActiveSlot,
                Is.False);
        }

        [Test]
        public void HealthyKnownSlotSelectsAndSameSlotIsNoChange()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();

            Assert.That(
                catalog.SelectActiveSlot(
                    slot)
                    .Status,
                Is.EqualTo(
                    SaveActiveSlotSelectionStatus.Selected));

            Assert.That(
                catalog.SelectActiveSlot(
                    slot)
                    .Status,
                Is.EqualTo(
                    SaveActiveSlotSelectionStatus.NoChange));

            Assert.That(
                catalog.ActiveSlotId,
                Is.EqualTo(
                    slot));
        }

        [Test]
        public void UnknownOrUnhealthySelectionRejectsWithoutChangingPrior()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId healthy =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveSlotId unhealthy =
                SlotCatalogTestSupport.Slot(
                    2);

            backend.SetChildren(
                healthy.Value,
                unhealthy.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                healthy,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();
            catalog.SelectActiveSlot(
                healthy);

            Assert.That(
                catalog.SelectActiveSlot(
                    unhealthy)
                    .Status,
                Is.EqualTo(
                    SaveActiveSlotSelectionStatus.Rejected));

            Assert.That(
                catalog.ActiveSlotId,
                Is.EqualTo(
                    healthy));

            Assert.That(
                catalog.SelectActiveSlot(
                    SlotCatalogTestSupport.Slot(
                        99))
                    .Status,
                Is.EqualTo(
                    SaveActiveSlotSelectionStatus.Rejected));

            Assert.That(
                catalog.ActiveSlotId,
                Is.EqualTo(
                    healthy));
        }

        [Test]
        public void ExplicitClearRemovesSelection()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();
            catalog.SelectActiveSlot(
                slot);

            Assert.That(
                catalog.ClearActiveSlot()
                    .Status,
                Is.EqualTo(
                    SaveActiveSlotSelectionStatus.Cleared));

            Assert.That(
                catalog.HasActiveSlot,
                Is.False);
        }

        [Test]
        public void RefreshRemovalClearsStaleActiveSelection()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();
            catalog.SelectActiveSlot(
                slot);

            backend.SetChildren();

            SaveSlotCatalogRefreshResult refreshed =
                catalog.Refresh();

            Assert.That(
                refreshed.ActiveSelectionCleared,
                Is.True);

            Assert.That(
                catalog.HasActiveSlot,
                Is.False);
        }

        [Test]
        public void RefreshUnhealthinessClearsActiveSelection()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            SaveGenerationId generation =
                SlotCatalogTestSupport.Generation(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                generation);

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();
            catalog.SelectActiveSlot(
                slot);

            backend.Remove(
                SlotCatalogTestSupport.ManifestKey(
                    slot,
                    generation));

            SaveSlotCatalogRefreshResult refreshed =
                catalog.Refresh();

            Assert.That(
                refreshed.ActiveSelectionCleared,
                Is.True);

            Assert.That(
                catalog.HasActiveSlot,
                Is.False);
        }

        [Test]
        public void SelectionAndReconciliationPerformZeroDurableWrites()
        {
            SlotCatalogFakeStorageBackend backend =
                new SlotCatalogFakeStorageBackend();

            SaveSlotId slot =
                SlotCatalogTestSupport.Slot(
                    1);

            backend.SetChildren(
                slot.Value);

            SlotCatalogTestSupport.PutHealthy(
                backend,
                slot,
                SlotCatalogTestSupport.Generation(
                    1));

            SaveSlotCatalog catalog =
                SlotCatalogTestSupport.CreateCatalog(
                    backend);

            catalog.Refresh();
            catalog.SelectActiveSlot(
                slot);
            catalog.ClearActiveSlot();
            catalog.Refresh();

            Assert.That(
                backend.WriteCount,
                Is.Zero);
        }
    }
}
