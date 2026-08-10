
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePreparedLoadStoreBoundaryTests
    {
        [Test]
        public void StoreOwnsNoStorageBackend()
        {
            FieldInfo[] fields =
                typeof(SavePreparedLoadStore)
                    .GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance);

            Assert.That(
                fields.Any(
                    field =>
                        typeof(ISaveStorageBackend)
                            .IsAssignableFrom(
                                field.FieldType)),
                Is.False);
        }

        [Test]
        public void StoreOwnsNoParticipantOrMigrationRegistry()
        {
            FieldInfo[] fields =
                typeof(SavePreparedLoadStore)
                    .GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance);

            Assert.That(
                fields.Any(
                    field =>
                        field.FieldType ==
                        typeof(
                            SaveParticipantRegistry)),
                Is.False);

            Assert.That(
                fields.Any(
                    field =>
                        field.FieldType ==
                        typeof(
                            SaveParticipantMigrationRegistry)),
                Is.False);
        }

        [Test]
        public void StoreOwnsNoSerializer()
        {
            FieldInfo[] fields =
                typeof(SavePreparedLoadStore)
                    .GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance);

            Assert.That(
                fields.Any(
                    field =>
                        typeof(ISaveSerializer)
                            .IsAssignableFrom(
                                field.FieldType)),
                Is.False);
        }

        [Test]
        public void HandleAndStoreAreNotUnityObjects()
        {
            Assert.That(
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        typeof(
                            PreparedSaveLoad)),
                Is.False);

            Assert.That(
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        typeof(
                            SavePreparedLoadStore)),
                Is.False);
        }
    }
}
