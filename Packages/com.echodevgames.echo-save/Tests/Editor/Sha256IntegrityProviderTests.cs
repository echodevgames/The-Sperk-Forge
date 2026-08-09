
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class Sha256IntegrityProviderTests
    {
        private Sha256IntegrityProvider provider;

        [SetUp]
        public void SetUp()
        {
            provider =
                new Sha256IntegrityProvider();
        }

        [Test]
        public void KnownAbcVectorMatchesSha256()
        {
            SaveIntegrityResult result =
                provider.Calculate(
                    Encoding.UTF8.GetBytes(
                        "abc"),
                    out string checksum);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                checksum,
                Is.EqualTo(
                    "ba7816bf8f01cfea414140de5dae2223" +
                    "b00361a396177a9cb410ff61f20015ad"));
        }

        [Test]
        public void MatchingBytesVerify()
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    "Chronicle");

            provider.Calculate(
                bytes,
                out string checksum);

            SaveIntegrityResult result =
                provider.Verify(
                    bytes,
                    checksum);

            Assert.That(
                result.Succeeded,
                Is.True);
        }

        [Test]
        public void AlteredBytesFailVerification()
        {
            byte[] original =
                Encoding.UTF8.GetBytes(
                    "Chronicle");

            provider.Calculate(
                original,
                out string checksum);

            SaveIntegrityResult result =
                provider.Verify(
                    Encoding.UTF8.GetBytes(
                        "Chronicle!"),
                    checksum);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveIntegrityStatus.Mismatch));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-INTEGRITY-002"));
        }

        [TestCase("")]
        [TestCase("ABC")]
        [TestCase("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD")]
        public void InvalidChecksumTextFailsStructurally(
            string checksum)
        {
            SaveIntegrityResult result =
                provider.Verify(
                    new byte[] { 1, 2, 3 },
                    checksum);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveIntegrityStatus
                        .InvalidRequest));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-INTEGRITY-001"));
        }

        [Test]
        public void NullBytesFailStructurally()
        {
            SaveIntegrityResult result =
                provider.Calculate(
                    null,
                    out string checksum);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveIntegrityStatus
                        .InvalidRequest));

            Assert.That(
                checksum,
                Is.Empty);
        }
    }
}
