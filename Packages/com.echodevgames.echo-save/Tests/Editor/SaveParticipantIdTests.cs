
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveParticipantIdTests
    {
        [TestCase("com.echodevgames.inventory")]
        [TestCase("com.mygame.player-progress")]
        [TestCase("org.example.quest_state")]
        public void CanonicalParticipantIdIsAccepted(
            string value)
        {
            Assert.That(
                SaveParticipantId.TryParse(
                    value,
                    out SaveParticipantId id),
                Is.True);

            Assert.That(
                id.Value,
                Is.EqualTo(
                    value));
        }

        [TestCase("")]
        [TestCase("inventory")]
        [TestCase("Com.Mygame.Inventory")]
        [TestCase("com.mygame.")]
        [TestCase(".com.mygame")]
        [TestCase("com..inventory")]
        [TestCase("com/mygame/inventory")]
        [TestCase("com\\mygame\\inventory")]
        [TestCase("com:mygame:inventory")]
        [TestCase("com.mygame.-inventory")]
        [TestCase("com.mygame.inventory-")]
        [TestCase("com.con.inventory")]
        [TestCase("../com.mygame.inventory")]
        public void NonCanonicalOrPathLikeParticipantIdIsRejected(
            string value)
        {
            Assert.That(
                SaveParticipantId.TryParse(
                    value,
                    out _),
                Is.False);
        }

        [Test]
        public void ConstructorRejectsInvalidIdentity()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new SaveParticipantId(
                        "Inventory"));
        }

        [Test]
        public void EqualityAndOrderingUseCanonicalIdentity()
        {
            SaveParticipantId first =
                new SaveParticipantId(
                    "com.example.alpha");

            SaveParticipantId same =
                new SaveParticipantId(
                    "com.example.alpha");

            SaveParticipantId later =
                new SaveParticipantId(
                    "com.example.beta");

            Assert.That(
                first,
                Is.EqualTo(
                    same));

            Assert.That(
                first.CompareTo(
                    later),
                Is.LessThan(0));
        }
    }
}
