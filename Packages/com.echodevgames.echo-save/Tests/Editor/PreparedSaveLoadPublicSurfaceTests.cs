
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class PreparedSaveLoadPublicSurfaceTests
    {
        [Test]
        public void HandleIsPublicSealedAndDisposable()
        {
            Type type =
                typeof(PreparedSaveLoad);

            Assert.That(
                type.IsPublic,
                Is.True);

            Assert.That(
                type.IsSealed,
                Is.True);

            Assert.That(
                typeof(IDisposable)
                    .IsAssignableFrom(
                        type),
                Is.True);
        }

        [Test]
        public void HandleHasNoPublicConstructor()
        {
            ConstructorInfo[] constructors =
                typeof(PreparedSaveLoad)
                    .GetConstructors(
                        BindingFlags.Public |
                        BindingFlags.Instance);

            Assert.That(
                constructors,
                Is.Empty);
        }

        [Test]
        public void HandlePublicPropertiesExposeNoPreparedInternals()
        {
            PropertyInfo[] properties =
                typeof(PreparedSaveLoad)
                    .GetProperties(
                        BindingFlags.Public |
                        BindingFlags.Instance);

            Type[] forbidden =
            {
                typeof(object),
                typeof(SavePreparedParticipantBatch),
                typeof(SavePreparedParticipantEntry),
                typeof(SaveUnknownPayloadSnapshot),
                typeof(SavePayloadEntry)
            };

            foreach (PropertyInfo property
                in properties)
            {
                Assert.That(
                    forbidden.Contains(
                        property.PropertyType),
                    Is.False,
                    property.Name);
            }
        }

        [Test]
        public void CreationResultExposesNoPreparedInternals()
        {
            PropertyInfo[] properties =
                typeof(PreparedLoadCreationResult)
                    .GetProperties(
                        BindingFlags.Public |
                        BindingFlags.Instance);

            Assert.That(
                properties.Any(
                    property =>
                        property.PropertyType ==
                        typeof(
                            SavePreparedParticipantBatch)),
                Is.False);

            Assert.That(
                properties.Any(
                    property =>
                        property.PropertyType ==
                        typeof(
                            SaveUnknownPayloadSnapshot)),
                Is.False);
        }
    }
}
