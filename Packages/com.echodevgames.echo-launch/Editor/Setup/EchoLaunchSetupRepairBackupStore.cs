using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupRepairBackupStore
    {
        EchoLaunchSetupRepairBackupSession CreateBackup(
            IEnumerable<string> projectAssetPaths);
    }

    internal sealed class EchoLaunchSetupRepairBackupEntry
    {
        internal EchoLaunchSetupRepairBackupEntry(
            string projectPath,
            string assetBackupPath,
            string metaBackupPath,
            bool metaExisted,
            string assetHash,
            string metaHash)
        {
            ProjectPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(projectPath);
            AssetBackupPath = assetBackupPath ?? string.Empty;
            MetaBackupPath = metaBackupPath ?? string.Empty;
            MetaExisted = metaExisted;
            AssetHash = assetHash ?? string.Empty;
            MetaHash = metaHash ?? string.Empty;
        }

        internal string ProjectPath { get; }
        internal string AssetBackupPath { get; }
        internal string MetaBackupPath { get; }
        internal bool MetaExisted { get; }
        internal string AssetHash { get; }
        internal string MetaHash { get; }
    }

    internal sealed class EchoLaunchSetupRepairBackupSession
    {
        private readonly List<EchoLaunchSetupRepairBackupEntry> entries;

        internal EchoLaunchSetupRepairBackupSession(
            string repairId,
            string backupDirectory,
            IEnumerable<EchoLaunchSetupRepairBackupEntry> entries)
        {
            RepairId = repairId ?? string.Empty;
            BackupDirectory = backupDirectory ?? string.Empty;
            this.entries = entries == null
                ? new List<EchoLaunchSetupRepairBackupEntry>()
                : new List<EchoLaunchSetupRepairBackupEntry>(entries);
        }

        internal string RepairId { get; }
        internal string BackupDirectory { get; }
        internal IReadOnlyList<EchoLaunchSetupRepairBackupEntry> Entries =>
            entries;

        internal EchoLaunchSetupRollbackResult Restore()
        {
            List<string> manualRecovery = new List<string>();

            for (int index = entries.Count - 1; index >= 0; index--)
            {
                EchoLaunchSetupRepairBackupEntry entry = entries[index];
                try
                {
                    string absoluteAsset =
                        EchoLaunchSetupRepairBackupPathUtility
                            .ResolveProjectAssetAbsolutePath(
                                entry.ProjectPath);
                    Directory.CreateDirectory(
                        Path.GetDirectoryName(absoluteAsset));
                    File.Copy(
                        entry.AssetBackupPath,
                        absoluteAsset,
                        true);
                    VerifyHash(
                        absoluteAsset,
                        entry.AssetHash,
                        entry.ProjectPath);

                    string absoluteMeta = absoluteAsset + ".meta";
                    if (entry.MetaExisted)
                    {
                        File.Copy(
                            entry.MetaBackupPath,
                            absoluteMeta,
                            true);
                        VerifyHash(
                            absoluteMeta,
                            entry.MetaHash,
                            entry.ProjectPath + ".meta");
                    }
                    else if (File.Exists(absoluteMeta))
                    {
                        File.Delete(absoluteMeta);
                    }
                }
                catch
                {
                    manualRecovery.Add(entry.ProjectPath);
                }
            }

            try
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            catch
            {
                manualRecovery.Add("AssetDatabase refresh");
            }

            manualRecovery.Sort(StringComparer.Ordinal);
            return new EchoLaunchSetupRollbackResult(
                manualRecovery.Count == 0,
                manualRecovery);
        }

        internal void DeleteBackup()
        {
            if (string.IsNullOrEmpty(BackupDirectory))
            {
                return;
            }

            string absoluteDirectory =
                EchoLaunchSetupRepairBackupPathUtility
                    .ResolveBackupAbsoluteDirectory(
                        BackupDirectory,
                        EchoLaunchSetupRepairBackupStore.BackupRoot);
            if (Directory.Exists(absoluteDirectory))
            {
                Directory.Delete(absoluteDirectory, true);
            }
        }

        private static void VerifyHash(
            string absolutePath,
            string expectedHash,
            string displayPath)
        {
            string actualHash = ComputeHash(absolutePath);
            if (!string.Equals(
                    actualHash,
                    expectedHash,
                    StringComparison.Ordinal))
            {
                throw new IOException(
                    "Restored bytes did not match the secured repair backup: " +
                    displayPath);
            }
        }

        private static string ComputeHash(string absolutePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(absolutePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                for (int index = 0; index < hash.Length; index++)
                {
                    builder.Append(hash[index].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

    internal static class EchoLaunchSetupRepairBackupPathUtility
    {
        internal static string ResolveProjectAssetAbsolutePath(
            string projectPath)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(projectPath);
            if (string.IsNullOrEmpty(normalized) ||
                !normalized.StartsWith("Assets/", StringComparison.Ordinal) ||
                normalized.Contains("/../") ||
                normalized.EndsWith("/..", StringComparison.Ordinal) ||
                normalized.IndexOf(':') >= 0 ||
                normalized.StartsWith("/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Repair backup paths must identify an asset below Assets/.");
            }

            string projectRoot =
                Directory.GetParent(Application.dataPath).FullName;
            string absoluteAssets =
                Path.GetFullPath(Application.dataPath)
                    .TrimEnd(Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string absolutePath = Path.GetFullPath(
                Path.Combine(projectRoot, normalized));
            if (!absolutePath.StartsWith(
                    absoluteAssets,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Repair backup paths cannot escape the project Assets directory.");
            }

            return absolutePath;
        }

        internal static string ResolveBackupAbsoluteDirectory(
            string backupDirectory,
            string approvedRoot)
        {
            string normalizedDirectory =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    backupDirectory);
            string normalizedRoot =
                EchoLaunchSetupPathUtility.NormalizeSeparators(approvedRoot);
            if (string.IsNullOrEmpty(normalizedDirectory) ||
                !normalizedDirectory.StartsWith(
                    normalizedRoot + "/",
                    StringComparison.Ordinal) ||
                normalizedDirectory.Contains("/../") ||
                normalizedDirectory.EndsWith("/..", StringComparison.Ordinal) ||
                normalizedDirectory.IndexOf(':') >= 0 ||
                normalizedDirectory.StartsWith("/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Repair backup directories must remain below the approved backup root.");
            }

            string projectRoot =
                Directory.GetParent(Application.dataPath).FullName;
            string absoluteApprovedRoot = Path.GetFullPath(
                Path.Combine(projectRoot, normalizedRoot))
                    .TrimEnd(Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string absoluteDirectory = Path.GetFullPath(
                Path.Combine(projectRoot, normalizedDirectory));
            if (!absoluteDirectory.StartsWith(
                    absoluteApprovedRoot,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Repair backup directories cannot escape the approved backup root.");
            }

            return absoluteDirectory;
        }
    }

    internal sealed class EchoLaunchSetupRepairBackupStore :
        IEchoLaunchSetupRepairBackupStore
    {
        internal const string BackupRoot =
            "Library/EchoDevGames/FirstLight/RepairBackups";

        public EchoLaunchSetupRepairBackupSession CreateBackup(
            IEnumerable<string> projectAssetPaths)
        {
            if (projectAssetPaths == null)
            {
                throw new ArgumentNullException(nameof(projectAssetPaths));
            }

            string repairId =
                DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ") + "_" +
                Guid.NewGuid().ToString("N");
            string projectRoot =
                Directory.GetParent(Application.dataPath).FullName;
            string relativeDirectory = BackupRoot + "/" + repairId;
            string absoluteDirectory = Path.GetFullPath(
                Path.Combine(projectRoot, relativeDirectory));
            Directory.CreateDirectory(absoluteDirectory);

            List<string> unique = new List<string>();
            foreach (string rawPath in projectAssetPaths)
            {
                string normalized =
                    EchoLaunchSetupPathUtility.NormalizeSeparators(rawPath);
                if (!string.IsNullOrEmpty(normalized) &&
                    !unique.Contains(normalized))
                {
                    unique.Add(normalized);
                }
            }
            unique.Sort(StringComparer.Ordinal);

            List<EchoLaunchSetupRepairBackupEntry> entries =
                new List<EchoLaunchSetupRepairBackupEntry>();
            StringBuilder manifest = new StringBuilder();
            manifest.AppendLine("First Light repair backup");
            manifest.AppendLine("Repair ID: " + repairId);

            try
            {
                for (int index = 0; index < unique.Count; index++)
                {
                    string projectPath = unique[index];
                    string absoluteAsset =
                        EchoLaunchSetupRepairBackupPathUtility
                            .ResolveProjectAssetAbsolutePath(projectPath);
                    UnityEngine.Object targetAsset =
                        AssetDatabase.LoadMainAssetAtPath(projectPath);
                    if (targetAsset == null)
                    {
                        throw new FileNotFoundException(
                            "Existing asset is unavailable for backup.",
                            projectPath);
                    }
                    if (EditorUtility.IsDirty(targetAsset))
                    {
                        throw new InvalidOperationException(
                            "Save or revert unsaved changes before repairing '" +
                            projectPath + "'.");
                    }

                    if (!File.Exists(absoluteAsset))
                    {
                        throw new FileNotFoundException(
                            "Existing asset bytes are unavailable for backup.",
                            projectPath);
                    }

                    string stem = index.ToString("D3") + "_" +
                        Sanitize(Path.GetFileName(projectPath));
                    string assetBackup =
                        Path.Combine(absoluteDirectory, stem + ".assetbytes");
                    string metaBackup =
                        Path.Combine(absoluteDirectory, stem + ".metabytes");
                    string assetHash = ComputeHash(absoluteAsset);
                    File.Copy(absoluteAsset, assetBackup, false);
                    VerifyHash(assetBackup, assetHash, projectPath);

                    string absoluteMeta = absoluteAsset + ".meta";
                    bool metaExisted = File.Exists(absoluteMeta);
                    if (!metaExisted)
                    {
                        throw new FileNotFoundException(
                            "Existing asset metadata is unavailable for backup.",
                            projectPath + ".meta");
                    }

                    string metaHash = ComputeHash(absoluteMeta);
                    File.Copy(absoluteMeta, metaBackup, false);
                    VerifyHash(
                        metaBackup,
                        metaHash,
                        projectPath + ".meta");

                    entries.Add(
                        new EchoLaunchSetupRepairBackupEntry(
                            projectPath,
                            assetBackup,
                            metaBackup,
                            metaExisted,
                            assetHash,
                            metaHash));
                    manifest.AppendLine(
                        projectPath + " | asset=" + assetHash +
                        (metaExisted ? " | meta=" + metaHash : string.Empty));
                }

                File.WriteAllText(
                    Path.Combine(absoluteDirectory, "manifest.txt"),
                    manifest.ToString(),
                    new UTF8Encoding(false));
            }
            catch
            {
                try
                {
                    Directory.Delete(absoluteDirectory, true);
                }
                catch
                {
                }
                throw;
            }

            return new EchoLaunchSetupRepairBackupSession(
                repairId,
                relativeDirectory,
                entries);
        }

        private static void VerifyHash(
            string absolutePath,
            string expectedHash,
            string displayPath)
        {
            string actualHash = ComputeHash(absolutePath);
            if (!string.Equals(
                    actualHash,
                    expectedHash,
                    StringComparison.Ordinal))
            {
                throw new IOException(
                    "Backup verification failed for " + displayPath + ".");
            }
        }

        private static string ComputeHash(string absolutePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(absolutePath))
            {
                byte[] hash = sha256.ComputeHash(stream);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                for (int index = 0; index < hash.Length; index++)
                {
                    builder.Append(hash[index].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "asset";
            }

            char[] invalid = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(value.Length);
            for (int index = 0; index < value.Length; index++)
            {
                builder.Append(Array.IndexOf(invalid, value[index]) >= 0
                    ? '_'
                    : value[index]);
            }
            return builder.ToString();
        }
    }
}
