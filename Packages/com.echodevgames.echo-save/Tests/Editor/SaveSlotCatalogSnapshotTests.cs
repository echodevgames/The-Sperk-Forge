
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotCatalogSnapshotTests
    {
        [Test]
        public void SnapshotEntriesCannotBeMutatedThroughPublicList()
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

            SaveSlotCatalogSnapshot snapshot =
                SlotCatalogTestSupport.CreateCatalog(
                    backend)
                    .Refresh()
                    .Snapshot;

            Assert.That(
                snapshot.Entries,
                Is.Not.InstanceOf<
                    List<SaveSlotCatalogEntry>>());

            Assert.That(
                snapshot.TryGetEntry(
                    slot,
                    out SaveSlotCatalogEntry entry),
                Is.True);

            Assert.That(
                entry,
                Is.SameAs(
                    snapshot.Entries[0]));
        }

        [Test]
        public void PublicCatalogEntrySurfaceContainsNoPayloadBodiesOrPaths()
        {
            Type[] propertyTypes =
                Array.ConvertAll(
                    typeof(SaveSlotCatalogEntry)
                        .GetProperties(),
                    property =>
                        property.PropertyType);

            Assert.That(
                Array.IndexOf(
                    propertyTypes,
                    typeof(byte[])),
                Is.EqualTo(-1));

            Assert.That(
                Array.IndexOf(
                    propertyTypes,
                    typeof(SavePayloadEntry)),
                Is.EqualTo(-1));

            Assert.That(
                Array.IndexOf(
                    propertyTypes,
                    typeof(object)),
                Is.EqualTo(-1));

            Assert.That(
                typeof(SaveSlotCatalogEntry)
                    .GetProperty(
                        "RootPath"),
                Is.Null);
        }

        [Test]
        public void CatalogRuntimeTypesAreNotUnityObjects()
        {
            Assert.That(
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        typeof(SaveSlotCatalog)),
                Is.False);

            Assert.That(
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        typeof(SaveSlotCatalogSnapshot)),
                Is.False);

            Assert.That(
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        typeof(SaveSlotCatalogEntry)),
                Is.False);
        }
    }
}
