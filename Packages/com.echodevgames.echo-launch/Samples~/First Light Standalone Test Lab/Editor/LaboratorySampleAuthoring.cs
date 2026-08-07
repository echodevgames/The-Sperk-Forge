using System;
using System.Collections.Generic;
using System.IO;
using EchoDevGames.EchoLaunch.Samples.StandaloneLab;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab.Editor
{
    internal static class LaboratorySampleAuthoring
    {
        private const string PackageName =
            "com.echodevgames.echo-launch";

        private const string PackageSampleRelativePath =
            "Samples~/First Light Standalone Test Lab";

        private const string PackageRootPrefabPath =
            "Packages/com.echodevgames.echo-launch/" +
            "Presentation.UGUI/Prefabs/EchoLaunchRoot.prefab";

        private const string GeneratedFolderName =
            "Generated";

        [MenuItem(
            "Tools/Sperk's Forge/First Light/Laboratory/" +
            "Build Imported Laboratory",
            priority = 2500)]
        private static void BuildImportedLaboratory()
        {
            string sampleRoot =
                ResolveImportedSampleRoot();

            if (string.IsNullOrEmpty(sampleRoot))
            {
                EditorUtility.DisplayDialog(
                    "First Light Laboratory",
                    "Import the First Light Standalone Test Lab sample " +
                    "through Package Manager before running this command.",
                    "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                    "Build First Light Laboratory",
                    "This explicit command will replace only the Generated " +
                    "folder inside the imported sample and export that " +
                    "folder back to the embedded package sample. " +
                    "Build Settings and ProjectSettings are not changed.",
                    "Build",
                    "Cancel"))
            {
                return;
            }

            string generatedRoot =
                sampleRoot + "/" + GeneratedFolderName;

            try
            {
                if (AssetDatabase.IsValidFolder(generatedRoot))
                {
                    AssetDatabase.DeleteAsset(generatedRoot);
                }

                CreateFolderTree(generatedRoot);

                Sprite splashSprite =
                    CreateSplashSprite(generatedRoot);

                LaboratoryImmediateSuccessStep immediate =
                    CreateStep<LaboratoryImmediateSuccessStep>(
                        generatedRoot,
                        "Immediate Success",
                        "11111111111111111111111111111111");

                LaboratoryTimedProgressStep timed =
                    CreateStep<LaboratoryTimedProgressStep>(
                        generatedRoot,
                        "Timed Progress",
                        "22222222222222222222222222222222");

                LaboratoryWarningStep warning =
                    CreateStep<LaboratoryWarningStep>(
                        generatedRoot,
                        "Warning Continues",
                        "33333333333333333333333333333333");

                LaboratoryRecoverableFailureStep recoverable =
                    CreateStep<LaboratoryRecoverableFailureStep>(
                        generatedRoot,
                        "Recoverable Failure Continues",
                        "44444444444444444444444444444444");

                LaboratoryBlockingFailureStep blocking =
                    CreateStep<LaboratoryBlockingFailureStep>(
                        generatedRoot,
                        "Blocking Failure Stops",
                        "55555555555555555555555555555555");

                StartupSequence successSequence =
                    CreateSequence(
                        generatedRoot,
                        "SuccessSequence",
                        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                        new[]
                        {
                            Entry(
                                "a1111111111111111111111111111111",
                                immediate,
                                true),
                            Entry(
                                "a2222222222222222222222222222222",
                                timed,
                                true)
                        });

                StartupSequence warningSequence =
                    CreateSequence(
                        generatedRoot,
                        "WarningSequence",
                        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                        new[]
                        {
                            Entry(
                                "b1111111111111111111111111111111",
                                warning,
                                true),
                            Entry(
                                "b2222222222222222222222222222222",
                                immediate,
                                true)
                        });

                StartupSequence recoverableSequence =
                    CreateSequence(
                        generatedRoot,
                        "RecoverableSequence",
                        "cccccccccccccccccccccccccccccccc",
                        new[]
                        {
                            Entry(
                                "c1111111111111111111111111111111",
                                recoverable,
                                false),
                            Entry(
                                "c2222222222222222222222222222222",
                                immediate,
                                true)
                        });

                StartupSequence blockingSequence =
                    CreateSequence(
                        generatedRoot,
                        "BlockingSequence",
                        "dddddddddddddddddddddddddddddddd",
                        new[]
                        {
                            Entry(
                                "d1111111111111111111111111111111",
                                blocking,
                                true),
                            Entry(
                                "d2222222222222222222222222222222",
                                immediate,
                                true)
                        });

                string destinationScenePath =
                    generatedRoot +
                    "/Scenes/FirstLight_Destination_Lab.unity";

                LaunchDestination destination =
                    CreateDestination(
                        generatedRoot,
                        "LaboratoryDestination",
                        "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                        "First Light Laboratory Destination",
                        destinationScenePath);

                LaunchDestination invalidDestination =
                    CreateDestination(
                        generatedRoot,
                        "InvalidDestination",
                        "ffffffffffffffffffffffffffffffff",
                        "Missing Laboratory Destination",
                        generatedRoot +
                        "/Scenes/Missing_Laboratory_Destination.unity");

                SplashSequence splash =
                    CreateSplashSequence(
                        generatedRoot,
                        splashSprite);

                EchoLaunchConfiguration successConfiguration =
                    CreateConfiguration(
                        generatedRoot,
                        "SuccessConfiguration",
                        "0123456789abcdef0123456789abcdef",
                        successSequence,
                        destination,
                        splash);

                EchoLaunchConfiguration warningConfiguration =
                    CreateConfiguration(
                        generatedRoot,
                        "WarningConfiguration",
                        "1123456789abcdef0123456789abcdef",
                        warningSequence,
                        destination,
                        splash);

                EchoLaunchConfiguration recoverableConfiguration =
                    CreateConfiguration(
                        generatedRoot,
                        "RecoverableConfiguration",
                        "2123456789abcdef0123456789abcdef",
                        recoverableSequence,
                        destination,
                        splash);

                EchoLaunchConfiguration blockingConfiguration =
                    CreateConfiguration(
                        generatedRoot,
                        "BlockingConfiguration",
                        "3123456789abcdef0123456789abcdef",
                        blockingSequence,
                        destination,
                        splash);

                EchoLaunchConfiguration invalidDestinationConfiguration =
                    CreateConfiguration(
                        generatedRoot,
                        "InvalidDestinationConfiguration",
                        "4123456789abcdef0123456789abcdef",
                        successSequence,
                        invalidDestination,
                        splash);

                GameObject canonicalRootPrefab =
                    CreateRootPrefab(
                        generatedRoot,
                        "EchoLaunchRoot_Laboratory.prefab",
                        successConfiguration,
                        LaunchMode.CanonicalBoot);

                GameObject directRootPrefab =
                    CreateRootPrefab(
                        generatedRoot,
                        "EchoLaunchDirectRoot_Laboratory.prefab",
                        successConfiguration,
                        LaunchMode.DirectSceneDevelopment);

                DirectSceneConfiguration directConfiguration =
                    CreateDirectConfiguration(
                        generatedRoot,
                        directRootPrefab);

                CreateBootScene(
                    generatedRoot,
                    canonicalRootPrefab);

                CreateDestinationScene(
                    generatedRoot,
                    directConfiguration,
                    directRootPrefab);

                CreateScenarioGuide(
                    generatedRoot,
                    successConfiguration,
                    warningConfiguration,
                    recoverableConfiguration,
                    blockingConfiguration,
                    invalidDestinationConfiguration);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ExportGeneratedFolderToPackage(
                    sampleRoot,
                    generatedRoot);

                EditorUtility.DisplayDialog(
                    "First Light Laboratory",
                    "The imported Laboratory and embedded package " +
                    "distribution payload were generated successfully.\n\n" +
                    "Next: remove this imported sample and reimport it " +
                    "through Package Manager.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "First Light Laboratory",
                    "Laboratory generation failed. See the Console for " +
                    "the complete exception. Build Settings and " +
                    "ProjectSettings were not changed.",
                    "OK");
            }
        }

        private static string ResolveImportedSampleRoot()
        {
            string[] guids =
                AssetDatabase.FindAssets(
                    "LaboratorySampleAuthoring t:MonoScript",
                    new[] { "Assets" });

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid)
                    .Replace('\\', '/');

                if (!path.EndsWith(
                        "/Editor/LaboratorySampleAuthoring.cs",
                        StringComparison.Ordinal))
                {
                    continue;
                }

                return path.Substring(
                    0,
                    path.Length -
                    "/Editor/LaboratorySampleAuthoring.cs".Length);
            }

            return string.Empty;
        }

        private static void CreateFolderTree(
            string generatedRoot)
        {
            EnsureFolder(generatedRoot);
            EnsureFolder(generatedRoot + "/Art");
            EnsureFolder(generatedRoot + "/Configuration");
            EnsureFolder(generatedRoot + "/Configuration/Steps");
            EnsureFolder(generatedRoot + "/Prefabs");
            EnsureFolder(generatedRoot + "/Scenes");
        }

        private static void EnsureFolder(string path)
        {
            string normalized =
                path.Replace('\\', '/');

            if (AssetDatabase.IsValidFolder(normalized))
            {
                return;
            }

            string parent =
                Path.GetDirectoryName(normalized)
                ?.Replace('\\', '/');

            string name =
                Path.GetFileName(normalized);

            if (string.IsNullOrEmpty(parent) ||
                string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException(
                    "Cannot create Laboratory folder: " +
                    normalized);
            }

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static Sprite CreateSplashSprite(
            string generatedRoot)
        {
            string pngPath =
                generatedRoot +
                "/Art/FirstLight_Laboratory_Splash.png";

            Texture2D texture =
                new Texture2D(
                    32,
                    32,
                    TextureFormat.RGBA32,
                    false);

            Color[] pixels =
                new Color[32 * 32];

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    float u = x / 31f;
                    float v = y / 31f;

                    pixels[y * 32 + x] =
                        new Color(
                            0.08f + 0.45f * u,
                            0.08f + 0.25f * v,
                            0.2f + 0.55f * (1f - u),
                            1f);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            File.WriteAllBytes(
                ToAbsolutePath(pngPath),
                texture.EncodeToPNG());

            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(
                pngPath,
                ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer =
                AssetImporter.GetAtPath(pngPath)
                as TextureImporter;

            if (importer == null)
            {
                throw new InvalidOperationException(
                    "The Laboratory splash texture importer is unavailable.");
            }

            importer.textureType =
                TextureImporterType.Sprite;

            importer.spriteImportMode =
                SpriteImportMode.Single;

            importer.SaveAndReimport();

            Sprite sprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(
                    pngPath);

            if (sprite == null)
            {
                throw new InvalidOperationException(
                    "The Laboratory splash sprite was not imported.");
            }

            return sprite;
        }

        private static T CreateStep<T>(
            string generatedRoot,
            string displayName,
            string stepId)
            where T : StartupStepDefinition
        {
            T asset =
                ScriptableObject.CreateInstance<T>();

            asset.name = displayName;

            SerializedObject serialized =
                new SerializedObject(asset);

            SetString(serialized, "stepId", stepId);
            SetInt(
                serialized,
                "schemaVersion",
                StartupStepDefinition.CurrentSchemaVersion);
            SetString(
                serialized,
                "displayName",
                displayName);

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/Steps/" +
                typeof(T).Name +
                ".asset";

            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static StartupSequence CreateSequence(
            string generatedRoot,
            string fileName,
            string sequenceId,
            EntryData[] entries)
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<
                    StartupSequence>();

            sequence.name = fileName;

            SerializedObject serialized =
                new SerializedObject(sequence);

            SetString(
                serialized,
                "sequenceId",
                sequenceId);

            SetInt(
                serialized,
                "schemaVersion",
                StartupSequence.CurrentSchemaVersion);

            SerializedProperty list =
                Require(serialized, "entries");

            list.arraySize = entries.Length;

            for (int index = 0;
                 index < entries.Length;
                 index++)
            {
                EntryData data = entries[index];
                SerializedProperty entry =
                    list.GetArrayElementAtIndex(index);

                entry.FindPropertyRelative("entryId")
                    .stringValue = data.EntryId;

                entry.FindPropertyRelative("activation")
                    .enumValueIndex = 0;

                entry.FindPropertyRelative("stepDefinition")
                    .objectReferenceValue =
                        data.Step;

                SerializedProperty policy =
                    entry.FindPropertyRelative("policy");

                policy.FindPropertyRelative("requirement")
                    .enumValueIndex =
                        data.Required ? 0 : 1;

                policy.FindPropertyRelative("failureAction")
                    .enumValueIndex =
                        data.Required ? 0 : 1;

                policy.FindPropertyRelative("timeoutSeconds")
                    .floatValue = 0f;

                policy.FindPropertyRelative("cancellation")
                    .enumValueIndex = 0;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/" +
                fileName +
                ".asset";

            AssetDatabase.CreateAsset(sequence, path);
            return sequence;
        }

        private static LaunchDestination CreateDestination(
            string generatedRoot,
            string fileName,
            string destinationId,
            string displayName,
            string scenePath)
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<
                    LaunchDestination>();

            destination.name = fileName;

            SerializedObject serialized =
                new SerializedObject(destination);

            SetString(
                serialized,
                "destinationId",
                destinationId);

            SetInt(
                serialized,
                "schemaVersion",
                LaunchDestination.CurrentSchemaVersion);

            SetString(
                serialized,
                "displayName",
                displayName);

            SetString(
                serialized,
                "scenePath",
                scenePath);

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/" +
                fileName +
                ".asset";

            AssetDatabase.CreateAsset(destination, path);
            return destination;
        }

        private static SplashSequence CreateSplashSequence(
            string generatedRoot,
            Sprite sprite)
        {
            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            sequence.name =
                "LaboratorySplashSequence";

            SerializedObject serialized =
                new SerializedObject(sequence);

            SetString(
                serialized,
                "sequenceId",
                "99999999999999999999999999999999");

            SetInt(
                serialized,
                "schemaVersion",
                SplashSequence.CurrentSchemaVersion);

            SerializedProperty entries =
                Require(serialized, "entries");

            entries.arraySize = 1;

            SerializedProperty entry =
                entries.GetArrayElementAtIndex(0);

            entry.FindPropertyRelative("entryId")
                .stringValue =
                    "91111111111111111111111111111111";

            entry.FindPropertyRelative("image")
                .objectReferenceValue = sprite;

            entry.FindPropertyRelative("displayLabel")
                .stringValue =
                    "FIRST LIGHT STANDALONE TEST LAB";

            entry.FindPropertyRelative("fadeInSeconds")
                .floatValue = 0.2f;

            entry.FindPropertyRelative("holdSeconds")
                .floatValue = 0.6f;

            entry.FindPropertyRelative("fadeOutSeconds")
                .floatValue = 0.2f;

            entry.FindPropertyRelative("minimumDisplaySeconds")
                .floatValue = 0.75f;

            entry.FindPropertyRelative("skipPolicy")
                .enumValueIndex = 1;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/LaboratorySplashSequence.asset";

            AssetDatabase.CreateAsset(sequence, path);
            return sequence;
        }

        private static EchoLaunchConfiguration
            CreateConfiguration(
                string generatedRoot,
                string fileName,
                string configurationId,
                StartupSequence sequence,
                LaunchDestination destination,
                SplashSequence splash)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            configuration.name = fileName;

            SerializedObject serialized =
                new SerializedObject(configuration);

            SetString(
                serialized,
                "configurationId",
                configurationId);

            SetInt(
                serialized,
                "schemaVersion",
                EchoLaunchConfiguration.CurrentSchemaVersion);

            Require(serialized, "startupSequence")
                .objectReferenceValue = sequence;

            Require(serialized, "initialDestination")
                .objectReferenceValue = destination;

            Require(serialized, "splashSequence")
                .objectReferenceValue = splash;

            Require(serialized, "useReducedMotionForSplash")
                .boolValue = false;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/" +
                fileName +
                ".asset";

            AssetDatabase.CreateAsset(configuration, path);
            return configuration;
        }

        private static GameObject CreateRootPrefab(
            string generatedRoot,
            string fileName,
            EchoLaunchConfiguration configuration,
            LaunchMode launchMode)
        {
            GameObject template =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    PackageRootPrefabPath);

            if (template == null)
            {
                throw new InvalidOperationException(
                    "The package root prefab template is missing: " +
                    PackageRootPrefabPath);
            }

            GameObject instance =
                PrefabUtility.InstantiatePrefab(template)
                as GameObject;

            if (instance == null)
            {
                throw new InvalidOperationException(
                    "The Laboratory could not instantiate the root template.");
            }

            try
            {
                instance.name =
                    Path.GetFileNameWithoutExtension(
                        fileName);

                EchoLaunchRoot root =
                    instance.GetComponent<EchoLaunchRoot>();

                if (root == null)
                {
                    throw new InvalidOperationException(
                        "The package root template does not contain EchoLaunchRoot.");
                }

                SerializedObject serialized =
                    new SerializedObject(root);

                Require(serialized, "configuration")
                    .objectReferenceValue =
                        configuration;

                Require(serialized, "launchMode")
                    .enumValueIndex =
                        (int)launchMode;

                Require(serialized, "startAutomatically")
                    .boolValue = true;

                serialized.ApplyModifiedPropertiesWithoutUndo();

                string path =
                    generatedRoot +
                    "/Prefabs/" +
                    fileName;

                return PrefabUtility.SaveAsPrefabAsset(
                    instance,
                    path);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    instance);
            }
        }

        private static DirectSceneConfiguration
            CreateDirectConfiguration(
                string generatedRoot,
                GameObject directRootPrefab)
        {
            EchoLaunchRoot root =
                directRootPrefab
                    .GetComponent<EchoLaunchRoot>();

            if (root == null)
            {
                throw new InvalidOperationException(
                    "The Direct Scene root prefab is invalid.");
            }

            DirectSceneConfiguration configuration =
                ScriptableObject.CreateInstance<
                    DirectSceneConfiguration>();

            configuration.name =
                "LaboratoryDirectSceneConfiguration";

            SerializedObject serialized =
                new SerializedObject(configuration);

            SetString(
                serialized,
                "directSceneConfigurationId",
                "81111111111111111111111111111111");

            SetInt(
                serialized,
                "schemaVersion",
                DirectSceneConfiguration.CurrentSchemaVersion);

            Require(serialized, "rootPrefab")
                .objectReferenceValue = root;

            Require(serialized, "entryPolicy")
                .enumValueIndex =
                    (int)DirectSceneEntryPolicy.EditorOnly;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            string path =
                generatedRoot +
                "/Configuration/LaboratoryDirectSceneConfiguration.asset";

            AssetDatabase.CreateAsset(configuration, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(
                path,
                ImportAssetOptions.ForceSynchronousImport);

            DirectSceneConfiguration persistentConfiguration =
                AssetDatabase.LoadAssetAtPath<
                    DirectSceneConfiguration>(path);

            if (persistentConfiguration == null)
            {
                throw new InvalidOperationException(
                    "The Direct Scene configuration could not be reloaded.");
            }

            return persistentConfiguration;
        }

        private static void CreateBootScene(
            string generatedRoot,
            GameObject canonicalRootPrefab)
        {
            Scene scene =
                EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);

            GameObject root =
                PrefabUtility.InstantiatePrefab(
                    canonicalRootPrefab,
                    scene)
                as GameObject;

            if (root == null)
            {
                throw new InvalidOperationException(
                    "The canonical Laboratory root could not be instantiated.");
            }

            root.name =
                "First Light Laboratory Root";

            GameObject readout =
                new GameObject(
                    "Laboratory Readout");

            readout.AddComponent<
                LaboratoryReadout>();

            GameObject duplicate =
                PrefabUtility.InstantiatePrefab(
                    canonicalRootPrefab,
                    scene)
                as GameObject;

            if (duplicate == null)
            {
                throw new InvalidOperationException(
                    "The duplicate-root fixture could not be created.");
            }

            duplicate.name =
                "DUPLICATE ROOT FIXTURE — ENABLE FOR ELAUNCH-LAB-006";

            duplicate.SetActive(false);

            string path =
                generatedRoot +
                "/Scenes/FirstLight_Boot_Lab.unity";

            EditorSceneManager.SaveScene(
                scene,
                path);
        }

        private static void CreateDestinationScene(
            string generatedRoot,
            DirectSceneConfiguration configuration,
            GameObject directRootPrefab)
        {
            Scene scene =
                EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);

            GameObject readoutObject =
                new GameObject(
                    "Laboratory Destination Readout");

            LaboratoryReadout readout =
                readoutObject.AddComponent<
                    LaboratoryReadout>();

            SerializedObject readoutSerialized =
                new SerializedObject(readout);

            Require(
                readoutSerialized,
                "destinationIsActive")
                .boolValue = true;

            Require(
                readoutSerialized,
                "title")
                .stringValue =
                    "First Light Laboratory Destination";

            readoutSerialized
                .ApplyModifiedPropertiesWithoutUndo();

            GameObject initializerObject =
                new GameObject(
                    "First Light Direct Scene Initializer");

            EchoDirectSceneInitializer initializer =
                initializerObject.AddComponent<
                    EchoDirectSceneInitializer>();

            SerializedObject initializerSerialized =
                new SerializedObject(initializer);

            Require(
                initializerSerialized,
                "directSceneConfiguration")
                .objectReferenceValue =
                    configuration;

            Require(
                initializerSerialized,
                "logSettlement")
                .boolValue = true;

            initializerSerialized
                .ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(initializer);
            EditorSceneManager.MarkSceneDirty(scene);

            GameObject existingRoot =
                PrefabUtility.InstantiatePrefab(
                    directRootPrefab,
                    scene)
                as GameObject;

            if (existingRoot == null)
            {
                throw new InvalidOperationException(
                    "The existing-root fixture could not be created.");
            }

            existingRoot.name =
                "EXISTING DIRECT ROOT FIXTURE — ENABLE FOR ELAUNCH-LAB-009";

            existingRoot.SetActive(false);

            string path =
                generatedRoot +
                "/Scenes/FirstLight_Destination_Lab.unity";

            if (!EditorSceneManager.SaveScene(
                    scene,
                    path))
            {
                throw new InvalidOperationException(
                    "The destination Laboratory scene could not be saved.");
            }

            VerifyDirectSceneConfigurationReference(
                path,
                configuration);
        }

        private static void VerifyDirectSceneConfigurationReference(
            string scenePath,
            DirectSceneConfiguration configuration)
        {
            string configurationPath =
                AssetDatabase.GetAssetPath(configuration);

            string configurationGuid =
                AssetDatabase.AssetPathToGUID(configurationPath);

            if (string.IsNullOrWhiteSpace(configurationGuid))
            {
                throw new InvalidOperationException(
                    "The Direct Scene configuration has no persistent GUID.");
            }

            string expected =
                "directSceneConfiguration: {fileID: 11400000, guid: " +
                configurationGuid +
                ", type: 2}";

            string sceneText =
                File.ReadAllText(
                    ToAbsolutePath(scenePath));

            if (!sceneText.Contains(expected))
            {
                throw new InvalidOperationException(
                    "The destination Laboratory scene did not serialize " +
                    "its Direct Scene configuration reference.");
            }
        }

        private static void CreateScenarioGuide(
            string generatedRoot,
            EchoLaunchConfiguration success,
            EchoLaunchConfiguration warning,
            EchoLaunchConfiguration recoverable,
            EchoLaunchConfiguration blocking,
            EchoLaunchConfiguration invalidDestination)
        {
            string path =
                ToAbsolutePath(
                    generatedRoot +
                    "/Configuration/SCENARIO_GUIDE.txt");

            File.WriteAllText(
                path,
                string.Join(
                    Environment.NewLine,
                    "First Light Standalone Test Lab",
                    string.Empty,
                    "Assign one configuration to the Boot root prefab or scene root before Play:",
                    "- SuccessConfiguration.asset",
                    "- WarningConfiguration.asset",
                    "- RecoverableConfiguration.asset",
                    "- BlockingConfiguration.asset",
                    "- InvalidDestinationConfiguration.asset",
                    string.Empty,
                    "Missing configuration:",
                    "Clear the root Configuration field before Play, then restore it.",
                    string.Empty,
                    "Duplicate root:",
                    "Enable the inactive duplicate fixture in the Boot scene.",
                    string.Empty,
                    "Direct Scene reuse:",
                    "Enable the inactive existing-root fixture in the Destination scene.",
                    string.Empty,
                    "Generated asset IDs:",
                    success.ConfigurationId,
                    warning.ConfigurationId,
                    recoverable.ConfigurationId,
                    blocking.ConfigurationId,
                    invalidDestination.ConfigurationId));

            AssetDatabase.ImportAsset(
                generatedRoot +
                "/Configuration/SCENARIO_GUIDE.txt",
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static void ExportGeneratedFolderToPackage(
            string sampleRoot,
            string generatedRoot)
        {
            string packageRoot =
                ResolveEmbeddedPackageRoot();

            string packageSampleRoot =
                packageRoot +
                "/" +
                PackageSampleRelativePath;

            string packageGeneratedRoot =
                packageSampleRoot +
                "/" +
                GeneratedFolderName;

            string sourceAbsolute =
                ToAbsolutePath(generatedRoot);

            string destinationAbsolute =
                ToAbsolutePath(packageGeneratedRoot);

            if (Directory.Exists(destinationAbsolute))
            {
                Directory.Delete(
                    destinationAbsolute,
                    true);
            }

            CopyDirectory(
                sourceAbsolute,
                destinationAbsolute);

            AssetDatabase.Refresh(
                ImportAssetOptions.ForceSynchronousImport);
        }

        private static string ResolveEmbeddedPackageRoot()
        {
            UnityEditor.PackageManager.PackageInfo[]
                packages =
                    UnityEditor.PackageManager
                        .PackageInfo
                        .GetAllRegisteredPackages();

            foreach (
                UnityEditor.PackageManager.PackageInfo
                    package in packages)
            {
                if (!string.Equals(
                        package.name,
                        PackageName,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                string resolved =
                    package.resolvedPath
                    .Replace('\\', '/');

                string project =
                    Path.GetFullPath(
                        Path.Combine(
                            Application.dataPath,
                            ".."))
                    .Replace('\\', '/')
                    .TrimEnd('/');

                if (!resolved.StartsWith(
                        project + "/",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "The First Light package is not writable as an embedded project package.");
                }

                return resolved.Substring(
                    project.Length + 1);
            }

            throw new InvalidOperationException(
                "The embedded First Light package is not registered.");
        }

        private static void CopyDirectory(
            string source,
            string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                File.Copy(
                    file,
                    Path.Combine(
                        destination,
                        Path.GetFileName(file)),
                    true);
            }

            foreach (
                string directory in
                    Directory.GetDirectories(source))
            {
                CopyDirectory(
                    directory,
                    Path.Combine(
                        destination,
                        Path.GetFileName(directory)));
            }
        }

        private static string ToAbsolutePath(
            string projectRelativePath)
        {
            string projectRoot =
                Path.GetFullPath(
                    Path.Combine(
                        Application.dataPath,
                        ".."));

            return Path.GetFullPath(
                Path.Combine(
                    projectRoot,
                    projectRelativePath));
        }

        private static EntryData Entry(
            string entryId,
            StartupStepDefinition step,
            bool required)
        {
            return new EntryData(
                entryId,
                step,
                required);
        }

        private static SerializedProperty Require(
            SerializedObject serialized,
            string propertyName)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(
                    "Required serialized property is missing: " +
                    serialized.targetObject.GetType().Name +
                    "." +
                    propertyName);
            }

            return property;
        }

        private static void SetString(
            SerializedObject serialized,
            string propertyName,
            string value)
        {
            Require(
                serialized,
                propertyName)
                .stringValue = value;
        }

        private static void SetInt(
            SerializedObject serialized,
            string propertyName,
            int value)
        {
            Require(
                serialized,
                propertyName)
                .intValue = value;
        }

        private readonly struct EntryData
        {
            internal EntryData(
                string entryId,
                StartupStepDefinition step,
                bool required)
            {
                EntryId = entryId;
                Step = step;
                Required = required;
            }

            internal string EntryId { get; }

            internal StartupStepDefinition Step { get; }

            internal bool Required { get; }
        }
    }
}
