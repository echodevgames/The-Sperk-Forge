
using System;
using System.Linq;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveStorageDiscoveryCapabilityTests
    {
        [Test]
        public void BaseStorageInterfaceRemainsUnchanged()
        {
            string[] methods =
                typeof(ISaveStorageBackend)
                    .GetMethods()
                    .Select(
                        method =>
                            method.Name)
                    .OrderBy(
                        value =>
                            value,
                        StringComparer.Ordinal)
                    .ToArray();

            Assert.That(
                methods,
                Is.EqualTo(
                    new[]
                    {
                        "Delete",
                        "Exists",
                        "Initialize",
                        "Read",
                        "Shutdown",
                        "WriteNew",
                        "get_Id",
                        "get_RootPath"
                    }));
        }

        [Test]
        public void DiscoveryIsAnAdditiveOptionalCapability()
        {
            Assert.That(
                typeof(ISaveStorageDiscoveryBackend)
                    .IsAssignableFrom(
                        typeof(ISaveStorageBackend)),
                Is.False);

            Assert.That(
                typeof(ISaveStorageDiscoveryBackend)
                    .GetMethods()
                    .Single()
                    .Name,
                Is.EqualTo(
                    "DiscoverChildDirectories"));
        }

        [Test]
        public void LocalBackendExposesDiscoveryCapability()
        {
            Assert.That(
                typeof(ISaveStorageDiscoveryBackend)
                    .IsAssignableFrom(
                        typeof(LocalFileSaveStorageBackend)),
                Is.True);
        }
    }
}
