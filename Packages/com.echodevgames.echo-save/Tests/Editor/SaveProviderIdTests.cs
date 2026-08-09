using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveProviderIdTests
    {
        [Test]
        public void StorageBackendIdNormalizesStableIdentity()
        {
            SaveStorageBackendId id =
                new SaveStorageBackendId(
                    " EchoDevGames.Local-File ");

            Assert.That(
                id.Value,
                Is.EqualTo(
                    "echodevgames.local-file"));
        }

        [Test]
        public void SerializerIdNormalizesStableIdentity()
        {
            SaveSerializerId id =
                new SaveSerializerId(
                    " EchoDevGames.Unity-Json ");

            Assert.That(
                id.Value,
                Is.EqualTo(
                    "echodevgames.unity-json"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("../bad")]
        [TestCase("bad/name")]
        [TestCase("bad\\name")]
        [TestCase("bad:name")]
        public void ProviderIdsRejectUnsafeValues(
            string value)
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new SaveSerializerId(
                        value));
        }
    }
}
