
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRuntimeTypeSerializerTests
    {
        private UnityJsonSaveSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer =
                new UnityJsonSaveSerializer();
        }

        [Test]
        public void RuntimeTypeRoundTripPreservesDtoFields()
        {
            RuntimeDto source =
                new RuntimeDto
                {
                    gold = 100,
                    label = "Chronicle"
                };

            SaveSerializerResult serialized =
                serializer.Serialize(
                    source,
                    typeof(RuntimeDto),
                    out string json);

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    json,
                    typeof(RuntimeDto),
                    out object restoredObject);

            RuntimeDto restored =
                restoredObject as
                    RuntimeDto;

            Assert.That(
                serialized.Succeeded,
                Is.True);

            Assert.That(
                deserialized.Succeeded,
                Is.True);

            Assert.That(
                restored,
                Is.Not.Null);

            Assert.That(
                restored.gold,
                Is.EqualTo(100));

            Assert.That(
                restored.label,
                Is.EqualTo(
                    "Chronicle"));
        }

        [Test]
        public void RuntimeTypeSerializeRejectsNullType()
        {
            SaveSerializerResult result =
                serializer.Serialize(
                    new RuntimeDto(),
                    null,
                    out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .InvalidRequest));
        }

        [Test]
        public void RuntimeTypeSerializeRejectsMismatchedValue()
        {
            SaveSerializerResult result =
                serializer.Serialize(
                    new OtherDto(),
                    typeof(RuntimeDto),
                    out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .InvalidRequest));
        }

        [Test]
        public void RuntimeTypeDeserializeRejectsNullType()
        {
            SaveSerializerResult result =
                serializer.Deserialize(
                    "{\"gold\":1}",
                    null,
                    out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveSerializerStatus
                        .InvalidRequest));
        }

        [Test]
        public void RuntimeSerializationDoesNotInjectClrTypeMetadata()
        {
            RuntimeDto source =
                new RuntimeDto
                {
                    gold = 7,
                    label = "NoTypeName"
                };

            Assert.That(
                serializer.Serialize(
                    source,
                    typeof(RuntimeDto),
                    out string json)
                    .Succeeded,
                Is.True);

            Assert.That(
                json.Contains(
                    typeof(RuntimeDto)
                        .FullName),
                Is.False);

            Assert.That(
                json.Contains(
                    typeof(RuntimeDto)
                        .Assembly.GetName().Name),
                Is.False);
        }

        [Serializable]
        private sealed class RuntimeDto
        {
            public int gold;
            public string label;
        }

        [Serializable]
        private sealed class OtherDto
        {
            public int value;
        }
    }
}
