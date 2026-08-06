using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch.Editor.Setup;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal interface IEchoLaunchValidationEvidenceSource
    {
        EchoLaunchValidationEvidence Collect(
            EchoLaunchValidationRequest request);
    }

    internal sealed class EchoLaunchValidationEvidenceCollector :
        IEchoLaunchValidationEvidenceSource
    {
        public EchoLaunchValidationEvidence Collect(
            EchoLaunchValidationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!EchoLaunchSetupPathUtility.TryNormalizeProjectRoot(
                    request.ProjectRootPath,
                    out string normalizedRoot,
                    out string pathError))
            {
                throw new ArgumentException(
                    pathError,
                    nameof(request));
            }

            EchoLaunchSetupPathSet paths =
                new EchoLaunchSetupPathSet(
                    normalizedRoot,
                    normalizedRoot + "/Scenes/Boot.unity");

            List<string> issues = new List<string>();

            EchoLaunchConfiguration configuration =
                AssetDatabase.LoadAssetAtPath<EchoLaunchConfiguration>(
                    paths.ConfigurationAssetPath);

            StartupSequence sequence =
                AssetDatabase.LoadAssetAtPath<StartupSequence>(
                    paths.StartupSequenceAssetPath);

            LaunchDestination destination =
                AssetDatabase.LoadAssetAtPath<LaunchDestination>(
                    paths.LaunchDestinationAssetPath);

            SplashSequence splash =
                AssetDatabase.LoadAssetAtPath<SplashSequence>(
                    paths.SplashSequenceAssetPath);

            EchoLaunchValidationAssetEvidence configurationEvidence =
                CreateAssetEvidence(
                    paths.ConfigurationAssetPath,
                    configuration,
                    configuration == null
                        ? string.Empty
                        : configuration.ConfigurationId,
                    configuration == null
                        ? 0
                        : configuration.SchemaVersion);

            EchoLaunchValidationAssetEvidence sequenceEvidence =
                CreateAssetEvidence(
                    paths.StartupSequenceAssetPath,
                    sequence,
                    sequence == null
                        ? string.Empty
                        : sequence.SequenceId,
                    sequence == null
                        ? 0
                        : sequence.SchemaVersion);

            EchoLaunchValidationAssetEvidence destinationEvidence =
                CreateAssetEvidence(
                    paths.LaunchDestinationAssetPath,
                    destination,
                    destination == null
                        ? string.Empty
                        : destination.DestinationId,
                    destination == null
                        ? 0
                        : destination.SchemaVersion);

            EchoLaunchValidationAssetEvidence splashEvidence =
                CreateAssetEvidence(
                    paths.SplashSequenceAssetPath,
                    splash,
                    splash == null
                        ? string.Empty
                        : splash.SequenceId,
                    splash == null
                        ? 0
                        : splash.SchemaVersion);

            string configurationSequencePath =
                configuration == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(
                        configuration.StartupSequence);

            string configurationDestinationPath =
                configuration == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(
                        configuration.InitialDestination);

            string configurationSplashPath =
                configuration == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(
                        configuration.SplashSequence);

            List<EchoLaunchValidationSequenceEntryEvidence> sequenceEntries =
                CollectSequenceEntries(sequence, issues);

            List<EchoLaunchValidationSplashEntryEvidence> splashEntries =
                CollectSplashEntries(splash, issues);

            EchoLaunchValidationRootPrefabEvidence rootPrefab =
                CollectRootPrefab(paths.RootPrefabPath, issues);

            List<EchoLaunchValidationBuildSceneEvidence> buildScenes =
                CollectBuildSettings();

            List<EchoLaunchValidationSceneEvidence> scenes =
                CollectScenes(
                    paths.BootScenePath,
                    buildScenes,
                    issues);

            bool templateAvailable =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath) != null;

            return new EchoLaunchValidationEvidence(
                request,
                paths,
                templateAvailable,
                configurationEvidence,
                sequenceEvidence,
                destinationEvidence,
                splashEvidence,
                rootPrefab,
                configurationSequencePath,
                configurationDestinationPath,
                configurationSplashPath,
                sequenceEntries,
                destination == null
                    ? string.Empty
                    : destination.ScenePath,
                destination == null
                    ? string.Empty
                    : destination.DisplayName,
                splashEntries,
                buildScenes,
                scenes,
                issues);
        }

        private static EchoLaunchValidationAssetEvidence CreateAssetEvidence(
            string path,
            UnityEngine.Object loadedObject,
            string stableId,
            int schemaVersion)
        {
            Type type = AssetDatabase.GetMainAssetTypeAtPath(path);

            return new EchoLaunchValidationAssetEvidence(
                path,
                type != null,
                type == null ? string.Empty : type.FullName,
                stableId,
                schemaVersion);
        }

        private static List<EchoLaunchValidationSequenceEntryEvidence>
            CollectSequenceEntries(
                StartupSequence sequence,
                List<string> issues)
        {
            List<EchoLaunchValidationSequenceEntryEvidence> result =
                new List<EchoLaunchValidationSequenceEntryEvidence>();

            if (sequence == null)
            {
                return result;
            }

            for (int index = 0; index < sequence.EntryCount; index++)
            {
                try
                {
                    StartupSequenceEntry entry = sequence.GetEntry(index);

                    if (entry == null)
                    {
                        result.Add(
                            new EchoLaunchValidationSequenceEntryEvidence(
                                index,
                                string.Empty,
                                false,
                                string.Empty,
                                string.Empty,
                                0,
                                false,
                                false,
                                int.MinValue,
                                double.NaN,
                                false));
                        continue;
                    }

                    StartupStepDefinition definition =
                        entry.StepDefinition;

                    StartupStepPolicy policy = entry.Policy;

                    result.Add(
                        new EchoLaunchValidationSequenceEntryEvidence(
                            index,
                            entry.EntryId,
                            entry.IsEnabled,
                            AssetDatabase.GetAssetPath(definition),
                            definition == null
                                ? string.Empty
                                : definition.StepId,
                            definition == null
                                ? 0
                                : definition.SchemaVersion,
                            policy.IsRequired,
                            policy.IsOptional,
                            (int)policy.FailureAction,
                            policy.TimeoutSeconds,
                            policy.SupportsCancellation));
                }
                catch (Exception exception)
                {
                    issues.Add(
                        "Startup sequence entry " +
                        index +
                        " could not be inspected (" +
                        exception.GetType().Name +
                        ").");
                }
            }

            return result;
        }

        private static List<EchoLaunchValidationSplashEntryEvidence>
            CollectSplashEntries(
                SplashSequence splash,
                List<string> issues)
        {
            List<EchoLaunchValidationSplashEntryEvidence> result =
                new List<EchoLaunchValidationSplashEntryEvidence>();

            if (splash == null)
            {
                return result;
            }

            for (int index = 0; index < splash.EntryCount; index++)
            {
                try
                {
                    SplashEntry entry = splash.GetEntry(index);

                    if (entry == null)
                    {
                        result.Add(
                            new EchoLaunchValidationSplashEntryEvidence(
                                index,
                                string.Empty,
                                string.Empty,
                                double.NaN,
                                double.NaN,
                                double.NaN,
                                double.NaN,
                                int.MinValue));
                        continue;
                    }

                    result.Add(
                        new EchoLaunchValidationSplashEntryEvidence(
                            index,
                            entry.EntryId,
                            AssetDatabase.GetAssetPath(entry.Image),
                            entry.FadeInSeconds,
                            entry.HoldSeconds,
                            entry.FadeOutSeconds,
                            entry.MinimumDisplaySeconds,
                            (int)entry.SkipPolicy));
                }
                catch (Exception exception)
                {
                    issues.Add(
                        "Splash entry " +
                        index +
                        " could not be inspected (" +
                        exception.GetType().Name +
                        ").");
                }
            }

            return result;
        }

        private static EchoLaunchValidationRootPrefabEvidence CollectRootPrefab(
            string path,
            List<string> issues)
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                return new EchoLaunchValidationRootPrefabEvidence(
                    path,
                    false,
                    false,
                    Array.Empty<EchoLaunchValidationRootEvidence>());
            }

            try
            {
                EchoLaunchRoot[] roots =
                    prefab.GetComponentsInChildren<EchoLaunchRoot>(true);

                List<EchoLaunchValidationRootEvidence> evidence =
                    new List<EchoLaunchValidationRootEvidence>(roots.Length);

                for (int index = 0; index < roots.Length; index++)
                {
                    evidence.Add(CollectRoot(roots[index]));
                }

                return new EchoLaunchValidationRootPrefabEvidence(
                    path,
                    true,
                    ReachesPackageTemplate(prefab),
                    evidence);
            }
            catch (Exception exception)
            {
                issues.Add(
                    "The canonical root prefab could not be inspected (" +
                    exception.GetType().Name +
                    ").");

                return new EchoLaunchValidationRootPrefabEvidence(
                    path,
                    true,
                    false,
                    Array.Empty<EchoLaunchValidationRootEvidence>());
            }
        }

        private static bool ReachesPackageTemplate(GameObject prefab)
        {
            GameObject current = prefab;

            for (int depth = 0; depth < 32 && current != null; depth++)
            {
                string currentPath = AssetDatabase.GetAssetPath(current);

                if (string.Equals(
                        currentPath,
                        EchoLaunchSetupPathSet.PackageRootPrefabTemplatePath,
                        StringComparison.Ordinal))
                {
                    return true;
                }

                current =
                    PrefabUtility.GetCorrespondingObjectFromSource(current)
                    as GameObject;
            }

            return false;
        }

        private static List<EchoLaunchValidationBuildSceneEvidence>
            CollectBuildSettings()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            List<EchoLaunchValidationBuildSceneEvidence> result =
                new List<EchoLaunchValidationBuildSceneEvidence>(
                    scenes.Length);

            for (int index = 0; index < scenes.Length; index++)
            {
                result.Add(
                    new EchoLaunchValidationBuildSceneEvidence(
                        scenes[index].path,
                        scenes[index].enabled,
                        index));
            }

            return result;
        }

        private static List<EchoLaunchValidationSceneEvidence> CollectScenes(
            string bootScenePath,
            IList<EchoLaunchValidationBuildSceneEvidence> buildScenes,
            List<string> issues)
        {
            List<string> paths = new List<string>();
            AddUnique(paths, bootScenePath);

            for (int index = 0; index < buildScenes.Count; index++)
            {
                if (buildScenes[index].Enabled)
                {
                    AddUnique(paths, buildScenes[index].Path);
                }
            }

            List<EchoLaunchValidationSceneEvidence> result =
                new List<EchoLaunchValidationSceneEvidence>(paths.Count);

            Scene activeBefore = SceneManager.GetActiveScene();
            Dictionary<string, bool> dirtyBefore = CaptureDirtyScenes();

            for (int index = 0; index < paths.Count; index++)
            {
                result.Add(InspectScene(paths[index], issues));
            }

            RestoreActiveScene(activeBefore, issues);
            VerifyDirtyScenes(dirtyBefore, issues);

            return result;
        }

        private static EchoLaunchValidationSceneEvidence InspectScene(
            string path,
            List<string> issues)
        {
            SceneAsset sceneAsset =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (sceneAsset == null)
            {
                return new EchoLaunchValidationSceneEvidence(
                    path,
                    false,
                    false,
                    Array.Empty<EchoLaunchValidationRootEvidence>());
            }

            Scene scene = SceneManager.GetSceneByPath(path);
            bool openedByValidator = !scene.IsValid() || !scene.isLoaded;

            try
            {
                if (openedByValidator)
                {
                    scene =
                        EditorSceneManager.OpenScene(
                            path,
                            OpenSceneMode.Additive);
                }

                List<EchoLaunchValidationRootEvidence> roots =
                    new List<EchoLaunchValidationRootEvidence>();

                List<EchoLaunchValidationDirectSceneEvidence>
                    directInitializers =
                        new List<
                            EchoLaunchValidationDirectSceneEvidence>();

                GameObject[] sceneRoots = scene.GetRootGameObjects();

                for (int rootIndex = 0;
                     rootIndex < sceneRoots.Length;
                     rootIndex++)
                {
                    EchoLaunchRoot[] launchRoots =
                        sceneRoots[rootIndex]
                            .GetComponentsInChildren<EchoLaunchRoot>(true);

                    for (int launchIndex = 0;
                         launchIndex < launchRoots.Length;
                         launchIndex++)
                    {
                        roots.Add(CollectRoot(launchRoots[launchIndex]));
                    }

                    EchoDirectSceneInitializer[] initializers =
                        sceneRoots[rootIndex]
                            .GetComponentsInChildren<
                                EchoDirectSceneInitializer>(true);

                    for (int initializerIndex = 0;
                         initializerIndex < initializers.Length;
                         initializerIndex++)
                    {
                        directInitializers.Add(
                            CollectDirectInitializer(
                                path,
                                initializers[initializerIndex]));
                    }
                }

                directInitializers.Sort(
                    CompareDirectInitializerEvidence);

                return new EchoLaunchValidationSceneEvidence(
                    path,
                    true,
                    true,
                    roots,
                    directInitializers);
            }
            catch (Exception exception)
            {
                issues.Add(
                    "Scene '" +
                    path +
                    "' could not be inspected (" +
                    exception.GetType().Name +
                    ").");

                return new EchoLaunchValidationSceneEvidence(
                    path,
                    true,
                    false,
                    Array.Empty<EchoLaunchValidationRootEvidence>());
            }
            finally
            {
                if (openedByValidator &&
                    scene.IsValid() &&
                    scene.isLoaded)
                {
                    try
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                    catch (Exception exception)
                    {
                        issues.Add(
                            "Scene '" +
                            path +
                            "' could not be closed after inspection (" +
                            exception.GetType().Name +
                            ").");
                    }
                }
            }
        }

        private static EchoLaunchValidationRootEvidence CollectRoot(
            EchoLaunchRoot root)
        {
            SerializedObject serialized = new SerializedObject(root);

            SerializedProperty configurationProperty =
                serialized.FindProperty("configuration");

            SerializedProperty presenterProperty =
                serialized.FindProperty("statusPresenterComponent");

            UnityEngine.Object configuration =
                configurationProperty == null
                    ? null
                    : configurationProperty.objectReferenceValue;

            MonoBehaviour presenter =
                presenterProperty == null
                    ? null
                    : presenterProperty.objectReferenceValue as MonoBehaviour;

            string sourcePath =
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                    root.gameObject);

            return new EchoLaunchValidationRootEvidence(
                AssetDatabase.GetAssetPath(configuration),
                sourcePath,
                presenter is ILaunchStatusPresenter,
                presenter is IImageSplashPresenter);
        }

        private static EchoLaunchValidationDirectSceneEvidence
            CollectDirectInitializer(
                string containingScenePath,
                EchoDirectSceneInitializer initializer)
        {
            DirectSceneConfiguration directConfiguration =
                initializer == null
                    ? null
                    : initializer.Configuration;

            string directConfigurationPath =
                AssetDatabase.GetAssetPath(directConfiguration);

            Type directConfigurationType =
                string.IsNullOrEmpty(directConfigurationPath)
                    ? null
                    : AssetDatabase.GetMainAssetTypeAtPath(
                        directConfigurationPath);

            EchoLaunchRoot rootPrefab =
                directConfiguration == null
                    ? null
                    : directConfiguration.RootPrefab;

            string rootPrefabPath =
                AssetDatabase.GetAssetPath(rootPrefab);

            EchoLaunchRoot[] roots =
                rootPrefab == null
                    ? Array.Empty<EchoLaunchRoot>()
                    : rootPrefab.gameObject
                        .GetComponentsInChildren<EchoLaunchRoot>(true);

            int activeRootCount = 0;

            for (int index = 0; index < roots.Length; index++)
            {
                if (roots[index] != null &&
                    roots[index].enabled &&
                    IsActiveWithinPrefab(roots[index].transform))
                {
                    activeRootCount++;
                }
            }

            EchoLaunchRoot inspectedRoot =
                roots.Length == 1
                    ? roots[0]
                    : null;

            int launchModeValue = int.MinValue;
            EchoLaunchConfiguration launchConfiguration = null;

            if (inspectedRoot != null)
            {
                SerializedObject serialized =
                    new SerializedObject(inspectedRoot);

                SerializedProperty launchModeProperty =
                    serialized.FindProperty("launchMode");

                SerializedProperty configurationProperty =
                    serialized.FindProperty("configuration");

                if (launchModeProperty != null)
                {
                    launchModeValue =
                        launchModeProperty.enumValueIndex;
                }

                launchConfiguration =
                    configurationProperty == null
                        ? null
                        : configurationProperty.objectReferenceValue
                            as EchoLaunchConfiguration;
            }

            LaunchDestination destination =
                launchConfiguration == null
                    ? null
                    : launchConfiguration.InitialDestination;

            return new EchoLaunchValidationDirectSceneEvidence(
                containingScenePath,
                initializer != null && initializer.enabled,
                directConfiguration == null
                    ? int.MinValue
                    : (int)directConfiguration.EntryPolicy,
                directConfigurationPath,
                directConfigurationType == null
                    ? string.Empty
                    : directConfigurationType.FullName,
                directConfiguration == null
                    ? string.Empty
                    : directConfiguration
                        .DirectSceneConfigurationId,
                directConfiguration == null
                    ? 0
                    : directConfiguration.SchemaVersion,
                rootPrefabPath,
                roots.Length,
                activeRootCount,
                rootPrefab != null &&
                ReachesPackageTemplate(rootPrefab.gameObject),
                launchModeValue,
                AssetDatabase.GetAssetPath(launchConfiguration),
                launchConfiguration == null
                    ? 0
                    : launchConfiguration.SchemaVersion,
                AssetDatabase.GetAssetPath(destination),
                destination == null
                    ? 0
                    : destination.SchemaVersion,
                destination == null
                    ? string.Empty
                    : destination.ScenePath);
        }

        private static int CompareDirectInitializerEvidence(
            EchoLaunchValidationDirectSceneEvidence left,
            EchoLaunchValidationDirectSceneEvidence right)
        {
            int comparison = CompareText(
                left.DirectConfigurationPath,
                right.DirectConfigurationPath);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison = CompareText(
                left.RootPrefabPath,
                right.RootPrefabPath);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.PolicyValue.CompareTo(right.PolicyValue);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.ComponentEnabled.CompareTo(
                    right.ComponentEnabled);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison = CompareText(
                left.DirectConfigurationTypeName,
                right.DirectConfigurationTypeName);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison = CompareText(
                left.DirectConfigurationId,
                right.DirectConfigurationId);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.DirectConfigurationSchema.CompareTo(
                    right.DirectConfigurationSchema);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.RootCount.CompareTo(right.RootCount);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.ActiveRootCount.CompareTo(
                    right.ActiveRootCount);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.ReachesPackageTemplate.CompareTo(
                    right.ReachesPackageTemplate);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.LaunchModeValue.CompareTo(
                    right.LaunchModeValue);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison = CompareText(
                left.LaunchConfigurationPath,
                right.LaunchConfigurationPath);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.LaunchConfigurationSchema.CompareTo(
                    right.LaunchConfigurationSchema);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison = CompareText(
                left.DestinationAssetPath,
                right.DestinationAssetPath);

            if (comparison != 0)
            {
                return comparison;
            }

            comparison =
                left.DestinationSchema.CompareTo(
                    right.DestinationSchema);

            return comparison != 0
                ? comparison
                : CompareText(
                    left.DestinationScenePath,
                    right.DestinationScenePath);
        }

        private static int CompareText(
            string left,
            string right)
        {
            return string.Compare(
                left,
                right,
                StringComparison.Ordinal);
        }

        private static bool IsActiveWithinPrefab(Transform target)
        {
            Transform current = target;

            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    return false;
                }

                current = current.parent;
            }

            return true;
        }

        private static Dictionary<string, bool> CaptureDirtyScenes()
        {
            Dictionary<string, bool> result =
                new Dictionary<string, bool>(StringComparer.Ordinal);

            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene scene = SceneManager.GetSceneAt(index);

                if (scene.IsValid() && scene.isLoaded)
                {
                    result[SceneKey(scene)] = scene.isDirty;
                }
            }

            return result;
        }

        private static void VerifyDirtyScenes(
            IDictionary<string, bool> expected,
            List<string> issues)
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene scene = SceneManager.GetSceneAt(index);

                if (!scene.IsValid() || !scene.isLoaded)
                {
                    continue;
                }

                string key = SceneKey(scene);

                if (expected.TryGetValue(key, out bool wasDirty) &&
                    scene.isDirty != wasDirty)
                {
                    issues.Add(
                        "Validation changed the dirty state of scene '" +
                        key +
                        "'.");
                }
            }
        }

        private static void RestoreActiveScene(
            Scene activeBefore,
            List<string> issues)
        {
            if (!activeBefore.IsValid() ||
                !activeBefore.isLoaded ||
                SceneManager.GetActiveScene() == activeBefore)
            {
                return;
            }

            if (!SceneManager.SetActiveScene(activeBefore))
            {
                issues.Add(
                    "Validation could not restore the previously active scene.");
            }
        }

        private static string SceneKey(Scene scene)
        {
            return string.IsNullOrEmpty(scene.path)
                ? "<Untitled:" + scene.name + ">"
                : scene.path;
        }

        private static void AddUnique(
            List<string> paths,
            string path)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(path);

            if (!string.IsNullOrEmpty(normalized) &&
                !paths.Contains(normalized))
            {
                paths.Add(normalized);
            }
        }
    }
}
