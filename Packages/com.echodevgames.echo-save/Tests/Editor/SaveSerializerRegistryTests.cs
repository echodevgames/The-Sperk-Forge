
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSerializerRegistryTests
    {
        [Test]
        public void DefaultRegistryResolvesUnityJsonSerializer()
        {
            SaveSerializerRegistry registry =
                new SaveSerializerRegistry();

            SaveSerializerResult result =
                registry.TryResolve(
                    new SaveSerializerId(
                        UnityJsonSaveSerializer.StableId),
                    out ISaveSerializer serializer);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                serializer,
                Is.TypeOf<
                    UnityJsonSaveSerializer>());
        }

        [Test]
        public void UniqueProviderRegisters()
        {
            SaveSerializerRegistry registry =
                new SaveSerializerRegistry(
                    registerDefaults: false);

            FakeSerializer serializer =
                new FakeSerializer(
                    "tests.serializer-a");

            SaveSerializerResult result =
                registry.TryRegister(
                    serializer);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                registry.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void DuplicateProviderIdentityIsRejected()
        {
            SaveSerializerRegistry registry =
                new SaveSerializerRegistry(
                    registerDefaults: false);

            FakeSerializer first =
                new FakeSerializer(
                    "tests.serializer-a");

            FakeSerializer duplicate =
                new FakeSerializer(
                    "tests.serializer-a");

            Assert.That(
                registry.TryRegister(
                    first)
                    .Succeeded,
                Is.True);

            SaveSerializerResult result =
                registry.TryRegister(
                    duplicate);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .DuplicateProvider));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-004"));
        }

        [Test]
        public void MissingProviderReturnsStructuredResult()
        {
            SaveSerializerRegistry registry =
                new SaveSerializerRegistry(
                    registerDefaults: false);

            SaveSerializerResult result =
                registry.TryResolve(
                    new SaveSerializerId(
                        "tests.missing"),
                    out ISaveSerializer serializer);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .ProviderNotFound));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-SERIAL-005"));

            Assert.That(
                serializer,
                Is.Null);
        }

        private sealed class FakeSerializer :
            ISaveSerializer
        {
            internal FakeSerializer(
                string id)
            {
                Id =
                    new SaveSerializerId(
                        id);
            }

            public SaveSerializerId Id
            {
                get;
            }

            public SaveSerializerResult Serialize<T>(
                T value,
                out string serialized)
            {
                serialized =
                    "{}";

                return SaveSerializerResult.Success(
                    "Fake serialize.");
            }

            public SaveSerializerResult Deserialize<T>(
                string serialized,
                out T value)
            {
                value =
                    default;

                return SaveSerializerResult.Success(
                    "Fake deserialize.");
            }
        }
    }
}
