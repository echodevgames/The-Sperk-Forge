
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRetentionPolicyTests
    {
        [Test]
        public void DefaultPolicyIsValidAndRecoverySafe()
        {
            SaveRetentionPolicy policy =
                SaveRetentionPolicy.Default;

            Assert.That(policy.IsValid, Is.True);
            Assert.That(
                policy.MaxTotalGenerations,
                Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void MinimumTwoGenerationsIsValid()
        {
            SaveRetentionPolicy policy =
                new SaveRetentionPolicy(2);

            Assert.That(policy.IsValid, Is.True);
        }

        [Test]
        public void OneGenerationPolicyIsRejected()
        {
            SaveRetentionPolicy policy =
                new SaveRetentionPolicy(1);

            Assert.That(policy.IsValid, Is.False);
        }

        [Test]
        public void PolicyAboveBoundIsRejected()
        {
            SaveRetentionPolicy policy =
                new SaveRetentionPolicy(
                    SaveRetentionPolicy
                        .MaximumTotalGenerations + 1);

            Assert.That(policy.IsValid, Is.False);
        }
    }
}
