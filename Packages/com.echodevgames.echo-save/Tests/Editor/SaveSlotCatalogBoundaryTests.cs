
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveSlotCatalogBoundaryTests
    {
        [Test]
        public void CatalogConstructorsOwnNoParticipantsOrUnityLifetime()
        {
            Type[] parameterTypes =
                typeof(SaveSlotCatalog)
                    .GetConstructors()
                    .SelectMany(
                        constructor =>
                            constructor.GetParameters())
                    .Select(
                        parameter =>
                            parameter.ParameterType)
                    .ToArray();

            Assert.That(
                Array.IndexOf(
                    parameterTypes,
                    typeof(SaveParticipantRegistry)),
                Is.EqualTo(-1));

            Assert.That(
                Array.IndexOf(
                    parameterTypes,
                    typeof(UnityEngine.MonoBehaviour)),
                Is.EqualTo(-1));

            Assert.That(
                Array.IndexOf(
                    parameterTypes,
                    typeof(UnityEngine.GameObject)),
                Is.EqualTo(-1));
        }

        [Test]
        public void ActiveSelectionApiExposesNoStorageMutationMethod()
        {
            string[] names =
                typeof(SaveSlotCatalog)
                    .GetMethods(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.DeclaredOnly)
                    .Select(
                        method =>
                            method.Name)
                    .ToArray();

            Assert.That(
                names,
                Does.Not.Contain(
                    "CreateSlot"));

            Assert.That(
                names,
                Does.Not.Contain(
                    "DeleteSlot"));

            Assert.That(
                names,
                Does.Not.Contain(
                    "RenameSlot"));

            Assert.That(
                names,
                Does.Not.Contain(
                    "DuplicateSlot"));
        }

        [Test]
        public void CatalogHasNoPersistentCacheSurface()
        {
            Assert.That(
                typeof(SaveSlotCatalog)
                    .GetMethods()
                    .Any(
                        method =>
                            method.Name.IndexOf(
                                "Cache",
                                StringComparison.OrdinalIgnoreCase) >= 0),
                Is.False);
        }
    }
}
