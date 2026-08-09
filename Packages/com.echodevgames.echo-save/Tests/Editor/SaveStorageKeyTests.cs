
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveStorageKeyTests
    {
        [Test]
        public void SafeNestedKeyNormalizesSeparators()
        {
            SaveStorageResult result =
                SaveStorageKey.TryCreate(
                    "slots\\slot-01/generation.bin",
                    out SaveStorageKey key);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                key.Value,
                Is.EqualTo(
                    "slots/slot-01/generation.bin"));
        }

        [TestCase("/rooted/file.bin")]
        [TestCase("\\\\server\\share\\file.bin")]
        [TestCase("C:\\temp\\file.bin")]
        [TestCase("../file.bin")]
        [TestCase("a/../file.bin")]
        [TestCase("./file.bin")]
        [TestCase("a/./file.bin")]
        [TestCase("a//file.bin")]
        [TestCase("a:b.bin")]
        [TestCase("a?b.bin")]
        [TestCase(" file.bin")]
        [TestCase("file.bin ")]
        public void UnsafeKeysAreRejected(
            string value)
        {
            SaveStorageResult result =
                SaveStorageKey.TryCreate(
                    value,
                    out _);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveStorageStatus.InvalidPath));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-001"));
        }

        [Test]
        public void SafeKeyResolvesUnderSandboxRoot()
        {
            string root =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSaveKeyTests");

            SaveStorageKey.TryCreate(
                "nested/file.bin",
                out SaveStorageKey key);

            SaveStorageResult result =
                SaveStoragePath.TryResolveUnderRoot(
                    Path.GetFullPath(root),
                    key,
                    out string fullPath);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                fullPath.StartsWith(
                    Path.GetFullPath(root),
                    System.StringComparison.OrdinalIgnoreCase),
                Is.True);
        }
    }
}
