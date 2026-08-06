//----- DirectSceneTestSupport.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class DirectSceneTestEnvironment :
        IDirectSceneRuntimeEnvironment
    {
        internal DirectSceneTestEnvironment(
            bool isEditor,
            bool isDevelopmentBuild)
        {
            IsEditor = isEditor;
            IsDevelopmentBuild = isDevelopmentBuild;
        }

        public bool IsEditor { get; }

        public bool IsDevelopmentBuild { get; }
    }

    internal sealed class RecordingDirectSceneRootFactory :
        IDirectSceneRootFactory
    {
        private readonly DirectSceneTestFixture fixture;

        internal RecordingDirectSceneRootFactory(
            DirectSceneTestFixture fixture)
        {
            this.fixture =
                fixture ??
                throw new ArgumentNullException(nameof(fixture));
        }

        internal int InstantiateCallCount { get; private set; }

        internal int DestroyCallCount { get; private set; }

        internal bool ThrowOnInstantiate { get; set; }

        public EchoLaunchRoot Instantiate(EchoLaunchRoot prefab)
        {
            InstantiateCallCount++;

            if (ThrowOnInstantiate)
            {
                throw new InvalidOperationException(
                    "Controlled direct-scene instantiation failure.");
            }

            return fixture.CreateRoot(
                prefab.AuthoredConfiguration,
                prefab.AuthoredLaunchMode,
                keepAuthority: true,
                name: "Created Direct Scene Root");
        }

        public void Destroy(EchoLaunchRoot root)
        {
            DestroyCallCount++;
            fixture.DestroyRootNow(root);
        }
    }

    internal sealed class DirectSceneTestFixture : IDisposable
    {
        private const string DefaultScenePath =
            "Assets/Scenes/DirectTestScene.unity";

        private static readonly FieldInfo RootConfigurationField =
            GetRequiredField(
                typeof(EchoLaunchRoot),
                "configuration");

        private static readonly FieldInfo RootLaunchModeField =
            GetRequiredField(
                typeof(EchoLaunchRoot),
                "launchMode");

        private static readonly FieldInfo RootAutomaticStartField =
            GetRequiredField(
                typeof(EchoLaunchRoot),
                "startAutomatically");

        private static readonly FieldInfo ConfigurationSequenceField =
            GetRequiredField(
                typeof(EchoLaunchConfiguration),
                "startupSequence");

        private static readonly FieldInfo ConfigurationDestinationField =
            GetRequiredField(
                typeof(EchoLaunchConfiguration),
                "initialDestination");

        private static readonly FieldInfo DestinationDisplayNameField =
            GetRequiredField(
                typeof(LaunchDestination),
                "displayName");

        private static readonly FieldInfo DestinationScenePathField =
            GetRequiredField(
                typeof(LaunchDestination),
                "scenePath");

        private static readonly FieldInfo DirectRootPrefabField =
            GetRequiredField(
                typeof(DirectSceneConfiguration),
                "rootPrefab");

        private static readonly FieldInfo DirectPolicyField =
            GetRequiredField(
                typeof(DirectSceneConfiguration),
                "entryPolicy");

        private static readonly FieldInfo DirectIdField =
            GetRequiredField(
                typeof(DirectSceneConfiguration),
                "directSceneConfigurationId");

        private readonly List<GameObject> createdObjects =
            new List<GameObject>();

        private readonly List<Object> createdAssets =
            new List<Object>();

        internal string ScenePath => DefaultScenePath;

        internal StartupSequence CreateSequence()
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<StartupSequence>();

            createdAssets.Add(sequence);
            return sequence;
        }

        internal LaunchDestination CreateDestination(
            string scenePath = DefaultScenePath,
            string displayName = "Direct Test Scene")
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<LaunchDestination>();

            DestinationDisplayNameField.SetValue(
                destination,
                displayName);

            DestinationScenePathField.SetValue(
                destination,
                scenePath);

            createdAssets.Add(destination);
            return destination;
        }

        internal EchoLaunchConfiguration CreateLaunchConfiguration(
            string scenePath = DefaultScenePath)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            ConfigurationSequenceField.SetValue(
                configuration,
                CreateSequence());

            ConfigurationDestinationField.SetValue(
                configuration,
                CreateDestination(scenePath));

            createdAssets.Add(configuration);
            return configuration;
        }

        internal EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            LaunchMode launchMode,
            bool keepAuthority,
            string name = "Direct Root Template")
        {
            GameObject target = new GameObject(name);
            createdObjects.Add(target);
            target.SetActive(false);

            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            RootLaunchModeField.SetValue(
                root,
                launchMode);

            RootAutomaticStartField.SetValue(
                root,
                false);

            target.SetActive(true);

            if (!keepAuthority)
            {
                LaunchAuthorityClaim.Reset();
            }

            return root;
        }

        internal EchoLaunchRoot CreateDirectRootTemplate(
            string destinationScenePath = DefaultScenePath,
            LaunchMode launchMode =
                LaunchMode.DirectSceneDevelopment)
        {
            return CreateRoot(
                CreateLaunchConfiguration(destinationScenePath),
                launchMode,
                keepAuthority: false);
        }

        internal DirectSceneConfiguration CreateDirectConfiguration(
            EchoLaunchRoot rootPrefab,
            DirectSceneEntryPolicy policy =
                DirectSceneEntryPolicy.EditorOnly)
        {
            DirectSceneConfiguration configuration =
                ScriptableObject.CreateInstance<
                    DirectSceneConfiguration>();

            DirectRootPrefabField.SetValue(
                configuration,
                rootPrefab);

            DirectPolicyField.SetValue(
                configuration,
                policy);

            createdAssets.Add(configuration);
            return configuration;
        }

        internal void SetDirectPolicy(
            DirectSceneConfiguration configuration,
            DirectSceneEntryPolicy policy)
        {
            DirectPolicyField.SetValue(configuration, policy);
        }

        internal void SetDirectId(
            DirectSceneConfiguration configuration,
            string value)
        {
            DirectIdField.SetValue(configuration, value);
        }

        internal EchoDirectSceneInitializer CreateInitializer(
            DirectSceneConfiguration configuration,
            IDirectSceneRuntimeEnvironment environment,
            IDirectSceneRootFactory factory,
            string scenePath = DefaultScenePath)
        {
            GameObject target =
                new GameObject("Direct Scene Initializer");

            createdObjects.Add(target);

            EchoDirectSceneInitializer initializer =
                target.AddComponent<EchoDirectSceneInitializer>();

            initializer.SetConfigurationForTesting(configuration);
            initializer.SetRuntimeEnvironmentForTesting(environment);
            initializer.SetRootFactoryForTesting(factory);
            initializer.SetContainingScenePathForTesting(scenePath);
            initializer.SetLoggingForTesting(false);

            return initializer;
        }

        internal void DestroyRootNow(EchoLaunchRoot root)
        {
            if (root == null)
            {
                return;
            }

            GameObject target = root.gameObject;
            createdObjects.Remove(target);
            Object.DestroyImmediate(target);
        }

        public void Dispose()
        {
            for (int index = createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target = createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            createdObjects.Clear();

            for (int index = createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object target = createdAssets[index];

                if (target != null)
                {
                    Object.DestroyImmediate(target);
                }
            }

            createdAssets.Clear();
            LaunchAuthorityClaim.Reset();
            LogAssert.NoUnexpectedReceived();
        }

        private static FieldInfo GetRequiredField(
            Type type,
            string fieldName)
        {
            FieldInfo field = type.GetField(
                fieldName,
                BindingFlags.Instance |
                BindingFlags.NonPublic);

            if (field == null)
            {
                throw new MissingFieldException(
                    type.FullName,
                    fieldName);
            }

            return field;
        }
    }
}

//----- DirectSceneTestSupport.cs END -----
