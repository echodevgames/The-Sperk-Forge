//----- EchoDirectSceneInitializerEditor.cs START -----

using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.DirectScene
{
    [CustomEditor(typeof(EchoDirectSceneInitializer))]
    internal sealed class EchoDirectSceneInitializerEditor :
        UnityEditor.Editor
    {
        private SerializedProperty configurationProperty;
        private SerializedProperty loggingProperty;

        private void OnEnable()
        {
            configurationProperty =
                serializedObject.FindProperty(
                    "directSceneConfiguration");

            loggingProperty =
                serializedObject.FindProperty(
                    "logSettlement");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "Direct Scene is a development entry helper. It reuses an existing First Light authority or creates one approved DirectSceneDevelopment root. A non-development player build can never create that root.",
                MessageType.Info);

            EditorGUILayout.PropertyField(
                configurationProperty,
                new GUIContent("Direct Configuration"));

            EditorGUILayout.PropertyField(
                loggingProperty,
                new GUIContent("Log Settlement"));

            serializedObject.ApplyModifiedProperties();

            DirectSceneConfiguration configuration =
                configurationProperty == null
                    ? null
                    : configurationProperty.objectReferenceValue
                        as DirectSceneConfiguration;

            DrawConfigurationEvidence(configuration);
        }

        private static void DrawConfigurationEvidence(
            DirectSceneConfiguration configuration)
        {
            if (configuration == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a project-owned DirectSceneConfiguration. The Validator reports a blocker while this reference is missing.",
                    MessageType.Warning);

                return;
            }

            DirectSceneEntryPolicy policy =
                configuration.EntryPolicy;

            if (policy ==
                DirectSceneEntryPolicy.EditorAndDevelopmentBuilds)
            {
                EditorGUILayout.HelpBox(
                    "Development-Build direct entry is explicitly enabled. The First Light Validator reports ELAUNCH-VAL-009 as a warning for an enabled build scene.",
                    MessageType.Warning);
            }
            else if (policy ==
                     DirectSceneEntryPolicy.BootRequired)
            {
                EditorGUILayout.HelpBox(
                    "BootRequired never creates a direct-scene root. Start this scene through canonical Boot.",
                    MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(
                    "Configuration ID",
                    configuration.DirectSceneConfigurationId);

                EditorGUILayout.IntField(
                    "Schema",
                    configuration.SchemaVersion);

                EditorGUILayout.EnumPopup(
                    "Entry Policy",
                    policy);

                EditorGUILayout.ObjectField(
                    "Root Prefab",
                    configuration.RootPrefab,
                    typeof(EchoLaunchRoot),
                    false);
            }

            EchoLaunchRoot root = configuration.RootPrefab;

            if (root == null)
            {
                return;
            }

            SerializedObject serializedRoot =
                new SerializedObject(root);

            SerializedProperty modeProperty =
                serializedRoot.FindProperty("launchMode");

            SerializedProperty launchConfigurationProperty =
                serializedRoot.FindProperty("configuration");

            EchoLaunchConfiguration launchConfiguration =
                launchConfigurationProperty == null
                    ? null
                    : launchConfigurationProperty.objectReferenceValue
                        as EchoLaunchConfiguration;

            using (new EditorGUI.DisabledScope(true))
            {
                if (modeProperty != null)
                {
                    EditorGUILayout.PropertyField(
                        modeProperty,
                        new GUIContent("Authored Launch Mode"));
                }

                EditorGUILayout.ObjectField(
                    "Launch Configuration",
                    launchConfiguration,
                    typeof(EchoLaunchConfiguration),
                    false);

                EditorGUILayout.ObjectField(
                    "Destination",
                    launchConfiguration == null
                        ? null
                        : launchConfiguration.InitialDestination,
                    typeof(LaunchDestination),
                    false);

                EditorGUILayout.TextField(
                    "Destination Scene",
                    launchConfiguration == null ||
                    launchConfiguration.InitialDestination == null
                        ? string.Empty
                        : launchConfiguration.InitialDestination.ScenePath);
            }
        }
    }
}

//----- EchoDirectSceneInitializerEditor.cs END -----
