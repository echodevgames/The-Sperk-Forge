
using System;
using System.IO;

namespace EchoDevGames.EchoSave
{
    internal static class SaveStoragePath
    {
        internal static SaveStorageResult TryNormalizeRoot(
            string rootPath,
            out string normalizedRoot)
        {
            normalizedRoot = string.Empty;

            if (string.IsNullOrWhiteSpace(
                    rootPath))
            {
                return Invalid(
                    "The Chronicle storage root is empty.");
            }

            try
            {
                if (!Path.IsPathRooted(
                        rootPath))
                {
                    return Invalid(
                        "The Chronicle storage root must be an absolute path.");
                }

                normalizedRoot =
                    Path.GetFullPath(
                        rootPath);

                return SaveStorageResult.Success(
                    "The Chronicle storage root is valid.");
            }
            catch (Exception exception)
                when (IsExpectedPathException(
                    exception))
            {
                normalizedRoot =
                    string.Empty;

                return Invalid(
                    "The Chronicle storage root is invalid.");
            }
        }

        internal static SaveStorageResult TryResolveUnderRoot(
            string rootPath,
            SaveStorageKey key,
            out string fullPath)
        {
            fullPath = string.Empty;

            SaveStorageResult rootResult =
                TryNormalizeRoot(
                    rootPath,
                    out string normalizedRoot);

            if (!rootResult.Succeeded)
            {
                return rootResult;
            }

            if (string.IsNullOrEmpty(
                    key.Value))
            {
                return Invalid(
                    "The Chronicle storage key is not initialized.");
            }

            try
            {
                string relativePlatformPath =
                    key.Value.Replace(
                        '/',
                        Path.DirectorySeparatorChar);

                string candidate =
                    Path.GetFullPath(
                        Path.Combine(
                            normalizedRoot,
                            relativePlatformPath));

                string rootWithSeparator =
                    AppendDirectorySeparator(
                        normalizedRoot);

                StringComparison comparison =
                    Path.DirectorySeparatorChar == '\\'
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal;

                if (!candidate.StartsWith(
                        rootWithSeparator,
                        comparison))
                {
                    return Invalid(
                        "The Chronicle storage path escapes its configured root.");
                }

                fullPath = candidate;

                return SaveStorageResult.Success(
                    "The Chronicle storage path is contained by its configured root.");
            }
            catch (Exception exception)
                when (IsExpectedPathException(
                    exception))
            {
                fullPath =
                    string.Empty;

                return Invalid(
                    "The Chronicle storage path is invalid.");
            }
        }

        private static string AppendDirectorySeparator(
            string value)
        {
            if (value.EndsWith(
                    Path.DirectorySeparatorChar
                        .ToString(),
                    StringComparison.Ordinal) ||
                value.EndsWith(
                    Path.AltDirectorySeparatorChar
                        .ToString(),
                    StringComparison.Ordinal))
            {
                return value;
            }

            return
                value +
                Path.DirectorySeparatorChar;
        }

        private static bool IsExpectedPathException(
            Exception exception) =>
            exception is ArgumentException ||
            exception is NotSupportedException ||
            exception is PathTooLongException;

        private static SaveStorageResult Invalid(
            string message) =>
            new SaveStorageResult(
                SaveStorageStatus.InvalidPath,
                EchoSaveDiagnosticCodes
                    .StorageInvalidPath,
                message);
    }
}
