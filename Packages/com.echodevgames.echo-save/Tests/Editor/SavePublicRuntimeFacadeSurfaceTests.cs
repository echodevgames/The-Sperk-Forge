
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicRuntimeFacadeSurfaceTests
    {
        [Test]
        public void PublicServiceExposesAuthorizedR1Facade()
        {
            AssertMethod(
                "RegisterParticipant",
                typeof(SaveParticipantRegistrationResult),
                typeof(ISaveParticipant));

            AssertMethod(
                "GetCatalogSnapshot",
                typeof(SaveSlotCatalogSnapshot));

            AssertMethod(
                "RefreshCatalogAsync",
                typeof(Awaitable<
                    SaveSlotCatalogRefreshResult>));

            AssertMethod(
                "CreateSlotAsync",
                typeof(Awaitable<
                    SaveSlotCreateResult>),
                typeof(SaveSlotCreateRequest));

            AssertMethod(
                "SelectSlot",
                typeof(SaveActiveSlotSelectionResult),
                typeof(SaveSlotId));

            AssertMethod(
                "PrepareLoadAsync",
                typeof(Awaitable<
                    PreparedLoadCreationResult>),
                typeof(SaveLoadRequest));

            AssertMethod(
                "ApplyPreparedLoadAsync",
                typeof(Awaitable<
                    SavePreparedLoadApplyResult>),
                typeof(PreparedSaveLoad));

            AssertMethod(
                "LoadAndApplyAsync",
                typeof(Awaitable<
                    SaveLoadResult>),
                typeof(SaveLoadRequest));
        }

        [Test]
        public void R1FacadeDoesNotExposeDeferredM5OrDestructiveExpansion()
        {
            Assert.That(
                typeof(IEchoSaveService)
                    .GetMethod("DeleteSlotAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService)
                    .GetMethod("RestoreFromTrashAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService)
                    .GetMethod("PermanentEraseAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService)
                    .GetMethod("LoadSceneAsync"),
                Is.Null);

            Assert.That(
                typeof(IEchoSaveService)
                    .GetMethod("ConfigureSlotPolicy"),
                Is.Null);
        }

        [Test]
        public void PublicCreateDoesNotExposeTechnicalCreationTypes()
        {
            MethodInfo method =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "CreateSlotAsync");

            Assert.That(method, Is.Not.Null);
            Assert.That(
                method.GetParameters()[0].ParameterType,
                Is.EqualTo(
                    typeof(SaveSlotCreateRequest)));

            Assert.That(
                method.GetParameters()[0].ParameterType.Name,
                Does.Not.Contain("Technical"));

            Assert.That(
                method.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<
                        SaveSlotCreateResult>)));
        }

        [Test]
        public void PublicLoadRequestOwnsOnlyExplicitSlotIdentity()
        {
            PropertyInfo[] properties =
                typeof(SaveLoadRequest)
                    .GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public);

            Assert.That(
                properties.Length,
                Is.EqualTo(1));

            Assert.That(
                properties[0].Name,
                Is.EqualTo("SlotId"));

            Assert.That(
                properties[0].PropertyType,
                Is.EqualTo(typeof(SaveSlotId)));
        }

        [Test]
        public void PreparedApplyDoesNotIntroduceCallerOwnedPolicyOverride()
        {
            MethodInfo apply =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "ApplyPreparedLoadAsync");

            Assert.That(apply, Is.Not.Null);
            Assert.That(
                apply.GetParameters().Length,
                Is.EqualTo(1));

            Assert.That(
                apply.GetParameters()[0].ParameterType,
                Is.EqualTo(
                    typeof(PreparedSaveLoad)));

            Assert.That(
                typeof(IEchoSaveService)
                    .Assembly
                    .GetType(
                        "EchoDevGames.EchoSave.ApplyLoadOptions"),
                Is.Null);
        }

        private static void AssertMethod(
            string name,
            System.Type returnType,
            params System.Type[] parameters)
        {
            MethodInfo method =
                typeof(IEchoSaveService)
                    .GetMethod(
                        name,
                        parameters);

            Assert.That(
                method,
                Is.Not.Null,
                "Expected public Chronicle method " + name);

            Assert.That(
                method.ReturnType,
                Is.EqualTo(returnType));
        }
    }
}
