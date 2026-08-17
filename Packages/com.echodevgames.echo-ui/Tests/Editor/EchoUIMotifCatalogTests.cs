using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifCatalogTests
    {
        private readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < created.Count; i++)
                if (created[i] != null) Object.DestroyImmediate(created[i]);
            created.Clear();
        }

        [Test]
        public void ValidCatalogResolvesDefaultAndStableId()
        {
            UIMotifDefinition first = Definition("motif.first", Color.red);
            UIMotifDefinition second = Definition("motif.second", Color.blue);
            UIMotifCatalog catalog = Catalog("motif.first", "motif.second", first, second);

            UIMotifCatalogResult built = catalog.CreateSnapshot(4, 4);
            UIMotifResolutionResult defaultResult = built.Snapshot.ResolveDefault();
            UIMotifResolutionResult requested = built.Snapshot.Resolve(new UIMotifId("motif.second"));

            Assert.That(built.Succeeded, Is.True);
            Assert.That(built.Snapshot.Count, Is.EqualTo(2));
            Assert.That(defaultResult.Status, Is.EqualTo(UIMotifResolutionStatus.Resolved));
            Assert.That(defaultResult.EffectiveMotifId, Is.EqualTo(new UIMotifId("motif.first")));
            Assert.That(requested.EffectiveMotifId, Is.EqualTo(new UIMotifId("motif.second")));
        }

        [Test]
        public void UnknownAndInvalidRequestsApplyConfiguredFallback()
        {
            UIMotifCatalogSnapshot snapshot = Catalog("motif.first", "motif.second",
                Definition("motif.first", Color.red), Definition("motif.second", Color.blue))
                .CreateSnapshot(4, 4).Snapshot;

            UIMotifResolutionResult unknown = snapshot.Resolve(new UIMotifId("motif.missing"));
            UIMotifResolutionResult invalid = snapshot.Resolve(default);

            Assert.That(unknown.Status, Is.EqualTo(UIMotifResolutionStatus.FallbackApplied));
            Assert.That(unknown.RequestedMotifId.Value, Is.EqualTo("motif.missing"));
            Assert.That(unknown.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(invalid.Status, Is.EqualTo(UIMotifResolutionStatus.FallbackApplied));
        }

        [Test]
        public void MissingOptionalFallbackReportsUnavailable()
        {
            UIMotifCatalogSnapshot snapshot = Catalog("motif.first", "",
                Definition("motif.first", Color.red)).CreateSnapshot(2, 2).Snapshot;

            UIMotifResolutionResult result = snapshot.Resolve(new UIMotifId("motif.missing"));

            Assert.That(result.Status, Is.EqualTo(UIMotifResolutionStatus.Unavailable));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void DuplicateMotifIdsRejectWithoutPartialSnapshot()
        {
            UIMotifCatalogResult result = Catalog("motif.same", "",
                Definition("motif.same", Color.red), Definition(" motif.same ", Color.blue))
                .CreateSnapshot(4, 4);

            Assert.That(result.Status, Is.EqualTo(UIMotifCatalogStatus.DuplicateMotifId));
            Assert.That(result.MotifId.Value, Is.EqualTo("motif.same"));
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void InvalidDefinitionFailureIsForwardedWithoutCatalogSnapshot()
        {
            UIMotifDefinition invalid = Track(UIMotifDefinition.CreateTransient("motif.invalid",
                numberTokens: new[] { new UIMotifNumberToken("number.bad", float.NaN) }));
            UIMotifCatalogResult result = Catalog("motif.invalid", "", invalid).CreateSnapshot(2, 2);

            Assert.That(result.Status, Is.EqualTo(UIMotifCatalogStatus.DefinitionRejected));
            Assert.That(result.DefinitionStatus, Is.EqualTo(UIMotifDefinitionStatus.InvalidTokenValue));
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void RequiredDefaultAndConfiguredFallbackMustExist()
        {
            UIMotifDefinition only = Definition("motif.only", Color.white);
            UIMotifCatalogResult missingDefault = Catalog("motif.missing", "", only).CreateSnapshot(2, 2);
            UIMotifCatalogResult missingFallback = Catalog("motif.only", "motif.missing", only).CreateSnapshot(2, 2);

            Assert.That(missingDefault.Status, Is.EqualTo(UIMotifCatalogStatus.DefaultMotifUnavailable));
            Assert.That(missingFallback.Status, Is.EqualTo(UIMotifCatalogStatus.FallbackMotifUnavailable));
            Assert.That(missingDefault.Snapshot, Is.Null);
            Assert.That(missingFallback.Snapshot, Is.Null);
        }

        [Test]
        public void NullEntriesAndInvalidCapacitiesRejectStructurally()
        {
            UIMotifCatalogResult missingCatalog = UIMotifCatalog.CreateSnapshot(null, 1, 1);
            UIMotifCatalogResult invalidCapacity = Catalog("motif.only", "", Definition("motif.only", Color.white))
                .CreateSnapshot(0, 1);
            UIMotifCatalogResult missingDefinition = Catalog("motif.only", "", (UIMotifDefinition)null)
                .CreateSnapshot(1, 1);

            Assert.That(missingCatalog.Status, Is.EqualTo(UIMotifCatalogStatus.MissingCatalog));
            Assert.That(invalidCapacity.Status, Is.EqualTo(UIMotifCatalogStatus.InvalidCapacity));
            Assert.That(missingDefinition.Status, Is.EqualTo(UIMotifCatalogStatus.MissingDefinition));
        }

        [Test]
        public void DefinitionCapacityIsBoundedBeforeAdmission()
        {
            UIMotifCatalogResult result = Catalog("motif.one", "",
                Definition("motif.one", Color.red), Definition("motif.two", Color.blue))
                .CreateSnapshot(1, 2);

            Assert.That(result.Status, Is.EqualTo(UIMotifCatalogStatus.CapacityExceeded));
            Assert.That(result.DefinitionCount, Is.EqualTo(2));
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void CatalogSnapshotRemainsDetachedFromCatalogAndDefinitions()
        {
            UIMotifDefinition definition = Definition("motif.first", Color.red);
            UIMotifDefinition[] authored = { definition };
            UIMotifCatalog catalog = Track(UIMotifCatalog.CreateTransient("motif.first", "", authored));
            UIMotifCatalogSnapshot snapshot = catalog.CreateSnapshot(2, 2).Snapshot;
            authored[0] = Definition("motif.other", Color.blue);

            Assert.That(snapshot.Count, Is.EqualTo(1));
            Assert.That(snapshot.Resolve(new UIMotifId("motif.first")).Status,
                Is.EqualTo(UIMotifResolutionStatus.Resolved));
            Assert.That(snapshot.Resolve(new UIMotifId("motif.other")).Status,
                Is.EqualTo(UIMotifResolutionStatus.Unavailable));
        }

        private UIMotifDefinition Definition(string id, Color color) =>
            Track(UIMotifDefinition.CreateTransient(id,
                colorTokens: new[] { new UIMotifColorToken("color.surface", color) }));

        private UIMotifCatalog Catalog(string defaultId, string fallbackId, params UIMotifDefinition[] definitions) =>
            Track(UIMotifCatalog.CreateTransient(defaultId, fallbackId, definitions));

        private T Track<T>(T value) where T : Object
        {
            created.Add(value);
            return value;
        }
    }
}
