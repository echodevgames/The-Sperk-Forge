using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupAssetWriter
    {
        void EnsureFolder(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);

        void CreateStartupSequence(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);

        void CreateLaunchDestination(
            string path,
            string destinationScenePath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);

        void CreateSplashSequence(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);

        void CreateConfiguration(
            string path,
            string startupSequencePath,
            string launchDestinationPath,
            string splashSequencePath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);
    }

    internal sealed class EchoLaunchSetupAssetWriter :
        IEchoLaunchSetupAssetWriter
    {
        public void EnsureFolder(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (AssetDatabase.IsValidFolder(normalized))
            {
                return;
            }

            if (AssetDatabase.LoadMainAssetAtPath(normalized) != null)
            {
                throw new InvalidOperationException(
                    "An asset already occupies folder path '" +
                    normalized +
                    "'.");
            }

            int slashIndex = normalized.LastIndexOf('/');

            if (slashIndex <= 0)
            {
                throw new InvalidOperationException(
                    "The folder path is invalid: " + normalized);
            }

            string parent = normalized.Substring(0, slashIndex);
            string folderName = normalized.Substring(slashIndex + 1);

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent, journal, log);
            }

            string guid = AssetDatabase.CreateFolder(parent, folderName);

            if (string.IsNullOrEmpty(guid) ||
                !AssetDatabase.IsValidFolder(normalized))
            {
                throw new InvalidOperationException(
                    "Unity could not create folder '" +
                    normalized +
                    "'.");
            }

            journal.RecordCreatedFolder(normalized);
            log.Add(
                EchoLaunchSetupChangeKind.CreatedFolder,
                normalized,
                "Created project-owned setup folder.");
        }

        public void CreateStartupSequence(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            CreateAsset(
                ScriptableObject.CreateInstance<StartupSequence>(),
                path,
                journal,
                log,
                "Created empty startup sequence.");
        }

        public void CreateLaunchDestination(
            string path,
            string destinationScenePath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<LaunchDestination>();

            SerializedObject serialized =
                new SerializedObject(destination);

            SerializedProperty displayName =
                serialized.FindProperty("displayName");

            SerializedProperty scenePath =
                serialized.FindProperty("scenePath");

            if (displayName == null || scenePath == null)
            {
                UnityEngine.Object.DestroyImmediate(destination);

                throw new InvalidOperationException(
                    "LaunchDestination serialized fields are unavailable.");
            }

            displayName.stringValue =
                Path.GetFileNameWithoutExtension(destinationScenePath);

            scenePath.stringValue =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    destinationScenePath);

            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateAsset(
                destination,
                path,
                journal,
                log,
                "Created launch destination bound to the selected scene.");
        }

        public void CreateSplashSequence(
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            CreateAsset(
                ScriptableObject.CreateInstance<SplashSequence>(),
                path,
                journal,
                log,
                "Created empty optional splash sequence.");
        }

        public void CreateConfiguration(
            string path,
            string startupSequencePath,
            string launchDestinationPath,
            string splashSequencePath,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            StartupSequence startupSequence =
                AssetDatabase.LoadAssetAtPath<StartupSequence>(
                    startupSequencePath);

            LaunchDestination launchDestination =
                AssetDatabase.LoadAssetAtPath<LaunchDestination>(
                    launchDestinationPath);

            SplashSequence splashSequence =
                string.IsNullOrEmpty(splashSequencePath)
                    ? null
                    : AssetDatabase.LoadAssetAtPath<SplashSequence>(
                        splashSequencePath);

            if (startupSequence == null)
            {
                throw new InvalidOperationException(
                    "The resolved startup sequence is unavailable.");
            }

            if (launchDestination == null)
            {
                throw new InvalidOperationException(
                    "The resolved launch destination is unavailable.");
            }

            if (!string.IsNullOrEmpty(splashSequencePath) &&
                splashSequence == null)
            {
                throw new InvalidOperationException(
                    "The resolved splash sequence is unavailable.");
            }

            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<EchoLaunchConfiguration>();

            SerializedObject serialized =
                new SerializedObject(configuration);

            SerializedProperty sequenceProperty =
                serialized.FindProperty("startupSequence");

            SerializedProperty destinationProperty =
                serialized.FindProperty("initialDestination");

            SerializedProperty splashProperty =
                serialized.FindProperty("splashSequence");

            if (sequenceProperty == null ||
                destinationProperty == null ||
                splashProperty == null)
            {
                UnityEngine.Object.DestroyImmediate(configuration);

                throw new InvalidOperationException(
                    "EchoLaunchConfiguration serialized fields are unavailable.");
            }

            sequenceProperty.objectReferenceValue = startupSequence;
            destinationProperty.objectReferenceValue = launchDestination;
            splashProperty.objectReferenceValue = splashSequence;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateAsset(
                configuration,
                path,
                journal,
                log,
                "Created launch configuration with resolved references.");
        }

        private static void CreateAsset(
            ScriptableObject asset,
            string path,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log,
            string message)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (AssetDatabase.LoadMainAssetAtPath(normalized) != null ||
                AssetDatabase.IsValidFolder(normalized))
            {
                UnityEngine.Object.DestroyImmediate(asset);

                throw new InvalidOperationException(
                    "Create-only setup cannot overwrite '" +
                    normalized +
                    "'.");
            }

            try
            {
                AssetDatabase.CreateAsset(asset, normalized);
                journal.RecordCreatedAsset(normalized);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(
                    normalized,
                    ImportAssetOptions.ForceSynchronousImport);

                if (AssetDatabase.LoadMainAssetAtPath(normalized) == null)
                {
                    throw new InvalidOperationException(
                        "Unity did not import created asset '" +
                        normalized +
                        "'.");
                }

                log.Add(
                    EchoLaunchSetupChangeKind.CreatedAsset,
                    normalized,
                    message);
            }
            catch
            {
                if (!EditorUtility.IsPersistent(asset))
                {
                    UnityEngine.Object.DestroyImmediate(asset);
                }

                throw;
            }
        }
    }
}
