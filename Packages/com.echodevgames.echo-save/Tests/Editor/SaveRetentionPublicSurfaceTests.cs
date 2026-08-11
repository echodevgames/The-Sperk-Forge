
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRetentionPublicSurfaceTests
    {
        [Test]
        public void BaseStorageContractRemainsUnchanged()
        {
            Assert.That(
                typeof(ISaveStorageBackend)
                    .GetMethod(
                        "DeleteTree"),
                Is.Null);

            Assert.That(
                typeof(ISaveStorageTreeDeletionBackend)
                    .GetMethod(
                        "DeleteTree",
                        new[]
                        {
                            typeof(SaveStorageKey)
                        }),
                Is.Not.Null);
        }

        [Test]
        public void PublicSaveResultExposesRetentionMaintenanceTruth()
        {
            Assert.That(
                typeof(SaveOperationResult)
                    .GetProperty(
                        "RetentionResult")
                    ?.PropertyType,
                Is.EqualTo(
                    typeof(SaveRetentionResult)));
        }
    }
}
