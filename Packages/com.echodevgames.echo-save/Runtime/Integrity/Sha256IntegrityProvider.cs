
using System;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Default Chronicle corruption-detection provider.
    ///
    /// SHA-256 here is an integrity checksum only. It is not an authentication
    /// or anti-cheat mechanism.
    /// </summary>
    public sealed class Sha256IntegrityProvider :
        IIntegrityProvider
    {
        public const string StableId =
            "echodevgames.sha256";

        private static readonly
            SaveIntegrityProviderId ProviderId =
                new SaveIntegrityProviderId(
                    StableId);

        public SaveIntegrityProviderId Id =>
            ProviderId;

        public SaveIntegrityResult Calculate(
            byte[] data,
            out string checksum)
        {
            checksum =
                string.Empty;

            if (data == null)
            {
                return Invalid(
                    "Detached Chronicle bytes are required for checksum calculation.");
            }

            try
            {
                using (SHA256 sha =
                    SHA256.Create())
                {
                    byte[] hash =
                        sha.ComputeHash(
                            data);

                    checksum =
                        ToLowerHex(
                            hash);
                }

                return SaveIntegrityResult.Success(
                    "The Chronicle SHA-256 checksum was calculated.");
            }
            catch (Exception exception)
            {
                checksum =
                    string.Empty;

                return new SaveIntegrityResult(
                    SaveIntegrityStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .IntegrityFailure,
                    $"Chronicle SHA-256 calculation failed. {exception.GetType().Name}: {exception.Message}");
            }
        }

        public SaveIntegrityResult Verify(
            byte[] data,
            string expectedChecksum)
        {
            if (data == null ||
                !IsCanonicalChecksum(
                    expectedChecksum))
            {
                return Invalid(
                    "Detached Chronicle bytes and one canonical 64-character lowercase SHA-256 checksum are required.");
            }

            SaveIntegrityResult calculated =
                Calculate(
                    data,
                    out string actualChecksum);

            if (!calculated.Succeeded)
            {
                return calculated;
            }

            if (!string.Equals(
                    actualChecksum,
                    expectedChecksum,
                    StringComparison.Ordinal))
            {
                return new SaveIntegrityResult(
                    SaveIntegrityStatus.Mismatch,
                    EchoSaveDiagnosticCodes
                        .IntegrityMismatch,
                    "The Chronicle SHA-256 checksum does not match the detached bytes.");
            }

            return SaveIntegrityResult.Success(
                "The Chronicle SHA-256 checksum matches the detached bytes.");
        }

        internal static bool IsCanonicalChecksum(
            string value)
        {
            if (value == null ||
                value.Length != 64)
            {
                return false;
            }

            for (int i = 0;
                 i < value.Length;
                 i++)
            {
                char character =
                    value[i];

                bool decimalDigit =
                    character >= '0' &&
                    character <= '9';

                bool lowerHex =
                    character >= 'a' &&
                    character <= 'f';

                if (!decimalDigit &&
                    !lowerHex)
                {
                    return false;
                }
            }

            return true;
        }

        private static SaveIntegrityResult Invalid(
            string message) =>
            new SaveIntegrityResult(
                SaveIntegrityStatus.InvalidRequest,
                EchoSaveDiagnosticCodes
                    .IntegrityInvalidRequest,
                message);

        private static string ToLowerHex(
            byte[] bytes)
        {
            StringBuilder builder =
                new StringBuilder(
                    bytes.Length * 2);

            for (int i = 0;
                 i < bytes.Length;
                 i++)
            {
                builder.Append(
                    bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
