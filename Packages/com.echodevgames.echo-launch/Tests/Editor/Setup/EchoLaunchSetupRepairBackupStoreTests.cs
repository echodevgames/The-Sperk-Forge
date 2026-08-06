using System;
using System.IO;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupRepairBackupStoreTests
    {
        private string assetPath;
        private string absolutePath;
        private EchoLaunchSetupRepairBackupSession session;

        [SetUp]
        public void SetUp()
        {
            assetPath =
                "Assets/__EchoLaunch_FL_M5_03_Backup_" +
                Guid.NewGuid().ToString("N") + ".asset";
            absolutePath = ProjectAbsolute(assetPath);
            StartupSequence sequence =
                ScriptableObject.CreateInstance<StartupSequence>();
            AssetDatabase.CreateAsset(sequence, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(
                assetPath,
                ImportAssetOptions.ForceSynchronousImport);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (session != null)
            {
                string backup = ProjectAbsolute(session.BackupDirectory);
                if (Directory.Exists(backup))
                {
                    Directory.Delete(backup, true);
                }
            }
        }

        [Test]
        public void BackupAndRestorePreserveExactAssetAndMetaBytes()
        {
            byte[] originalAsset = File.ReadAllBytes(absolutePath);
            byte[] originalMeta = File.ReadAllBytes(absolutePath + ".meta");

            session =
                new EchoLaunchSetupRepairBackupStore().CreateBackup(
                    new[] { assetPath });

            File.WriteAllText(absolutePath, "damaged asset bytes");
            File.WriteAllText(absolutePath + ".meta", "damaged meta bytes");

            Assert.That(session.Entries.Count, Is.EqualTo(1));
            Assert.That(session.Entries[0].AssetHash.Length, Is.EqualTo(64));
            Assert.That(session.Entries[0].MetaHash.Length, Is.EqualTo(64));

            EchoLaunchSetupRollbackResult result = session.Restore();

            Assert.That(result.Completed, Is.True);
            Assert.That(
                File.ReadAllBytes(absolutePath),
                Is.EqualTo(originalAsset));
            Assert.That(
                File.ReadAllBytes(absolutePath + ".meta"),
                Is.EqualTo(originalMeta));
        }

        [Test]
        public void DeleteBackupRemovesSuccessfulTemporaryDirectory()
        {
            session =
                new EchoLaunchSetupRepairBackupStore().CreateBackup(
                    new[] { assetPath });
            string absoluteBackup =
                ProjectAbsolute(session.BackupDirectory);

            session.DeleteBackup();

            Assert.That(Directory.Exists(absoluteBackup), Is.False);
        }

        [Test]
        public void BackupDirectoryUsesApprovedLibraryBoundary()
        {
            session =
                new EchoLaunchSetupRepairBackupStore().CreateBackup(
                    new[] { assetPath });

            Assert.That(
                session.BackupDirectory,
                Does.StartWith(
                    EchoLaunchSetupRepairBackupStore.BackupRoot + "/"));
            Assert.That(
                File.Exists(
                    ProjectAbsolute(
                        session.BackupDirectory + "/manifest.txt")),
                Is.True);
        }

        [Test]
        public void BackupRejectsDirtyTargetWithoutSavingIt()
        {
            UnityEngine.Object asset =
                AssetDatabase.LoadMainAssetAtPath(assetPath);
            EditorUtility.SetDirty(asset);

            Assert.Throws<InvalidOperationException>(
                delegate
                {
                    new EchoLaunchSetupRepairBackupStore().CreateBackup(
                        new[] { assetPath });
                });
            Assert.That(EditorUtility.IsDirty(asset), Is.True);
        }

        [Test]
        public void BackupRejectsPathsOutsideProjectAssets()
        {
            Assert.Throws<InvalidOperationException>(
                delegate
                {
                    new EchoLaunchSetupRepairBackupStore().CreateBackup(
                        new[] { "Packages/NotProjectOwned.asset" });
                });
        }

        [Test]
        public void DeleteBackupRejectsDirectoryOutsideApprovedRoot()
        {
            EchoLaunchSetupRepairBackupSession unsafeSession =
                new EchoLaunchSetupRepairBackupSession(
                    "unsafe",
                    "Library/Elsewhere/unsafe",
                    null);

            Assert.Throws<InvalidOperationException>(
                delegate { unsafeSession.DeleteBackup(); });
        }

        private static string ProjectAbsolute(string projectRelativePath)
        {
            string projectRoot =
                Directory.GetParent(Application.dataPath).FullName;
            return Path.GetFullPath(
                Path.Combine(projectRoot, projectRelativePath));
        }
    }
}
