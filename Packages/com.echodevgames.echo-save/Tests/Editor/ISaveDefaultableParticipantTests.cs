
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class ISaveDefaultableParticipantTests
    {
        [Test]
        public void DefaultCapabilityIsSeparatePublicInterface()
        {
            Assert.That(
                typeof(ISaveDefaultableParticipant)
                    .IsPublic,
                Is.True);

            Assert.That(
                typeof(ISaveDefaultableParticipant)
                    .IsAssignableFrom(
                        typeof(ISaveParticipant)),
                Is.False);
        }

        [Test]
        public void DefaultCapabilityExposesOnlyInitializeDefault()
        {
            MethodInfo[] methods =
                typeof(ISaveDefaultableParticipant)
                    .GetMethods();

            Assert.That(
                methods.Length,
                Is.EqualTo(1));

            Assert.That(
                methods[0].Name,
                Is.EqualTo(
                    "InitializeDefault"));

            Assert.That(
                methods[0].ReturnType,
                Is.EqualTo(
                    typeof(
                        SaveParticipantApplyResult)));

            Assert.That(
                methods[0].GetParameters(),
                Is.Empty);
        }

        [Test]
        public void BaseParticipantContractRemainsDescriptorCaptureApplyOnly()
        {
            string[] methodNames =
                typeof(ISaveParticipant)
                    .GetMethods()
                    .Select(
                        method =>
                            method.Name)
                    .OrderBy(
                        name =>
                            name,
                        StringComparer.Ordinal)
                    .ToArray();

            Assert.That(
                methodNames,
                Is.EqualTo(
                    new[]
                    {
                        "Apply",
                        "Capture",
                        "get_Descriptor"
                    }));
        }
    }
}
