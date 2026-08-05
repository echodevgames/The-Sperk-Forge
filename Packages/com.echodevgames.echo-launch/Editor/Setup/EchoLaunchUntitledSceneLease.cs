using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchUntitledSceneLease :
        IDisposable
    {
        private static readonly MethodInfo SetPathAndGuidMethod =
            typeof(Scene).GetMethod(
                "SetPathAndGuid",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[]
                {
                    typeof(string),
                    typeof(string)
                },
                null);

        private static readonly MethodInfo ClearSceneDirtinessMethod =
            typeof(EditorSceneManager).GetMethod(
                "ClearSceneDirtiness",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[]
                {
                    typeof(Scene)
                },
                null);

        private readonly Scene leasedScene;
        private readonly string originalName;
        private readonly bool originalDirty;
        private readonly string temporaryAssetPath;
        private bool disposed;

        private EchoLaunchUntitledSceneLease()
        {
            leasedScene = default(Scene);
            originalName = string.Empty;
            originalDirty = false;
            temporaryAssetPath = string.Empty;
        }

        private EchoLaunchUntitledSceneLease(
            Scene scene,
            string sceneName,
            bool wasDirty,
            string temporaryPath)
        {
            leasedScene = scene;
            originalName = sceneName ?? string.Empty;
            originalDirty = wasDirty;
            temporaryAssetPath = temporaryPath ?? string.Empty;
        }

        internal static EchoLaunchUntitledSceneLease Acquire()
        {
            Scene untitledScene = FindUntitledScene();

            if (!untitledScene.IsValid())
            {
                return new EchoLaunchUntitledSceneLease();
            }

            ValidateInternalRestoreSupport();

            string temporaryPath = CreateUniqueTemporaryPath();
            string originalSceneName = untitledScene.name;
            bool wasDirty = untitledScene.isDirty;

            if (!EditorSceneManager.SaveScene(
                    untitledScene,
                    temporaryPath,
                    false))
            {
                throw new InvalidOperationException(
                    "Unity could not temporarily lease the Untitled scene slot.");
            }

            if (string.IsNullOrEmpty(untitledScene.path))
            {
                AssetDatabase.DeleteAsset(temporaryPath);

                throw new InvalidOperationException(
                    "Unity did not assign the temporary Untitled-scene lease path.");
            }

            return new EchoLaunchUntitledSceneLease(
                untitledScene,
                originalSceneName,
                wasDirty,
                temporaryPath);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (!leasedScene.IsValid())
            {
                return;
            }

            Scene scene = leasedScene;
            bool identityRestored = false;

            try
            {
                SetPathAndGuidMethod.Invoke(
                    scene,
                    new object[]
                    {
                        string.Empty,
                        string.Empty
                    });

                scene.name = originalName;

                if (originalDirty)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                }
                else
                {
                    ClearSceneDirtinessMethod.Invoke(
                        null,
                        new object[]
                        {
                            scene
                        });
                }

                identityRestored =
                    string.IsNullOrEmpty(scene.path) &&
                    scene.name == originalName &&
                    scene.isDirty == originalDirty;

                if (!identityRestored)
                {
                    throw new InvalidOperationException(
                        "Unity could not restore the original Untitled scene state.");
                }
            }
            catch (TargetInvocationException exception)
            {
                throw new InvalidOperationException(
                    "Unity could not restore the original Untitled scene identity.",
                    exception.InnerException ?? exception);
            }
            finally
            {
                if (identityRestored)
                {
                    DeleteTemporaryAsset();
                }
            }
        }

        private static Scene FindUntitledScene()
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene scene = SceneManager.GetSceneAt(index);

                if (scene.IsValid() &&
                    scene.isLoaded &&
                    !EditorSceneManager.IsPreviewScene(scene) &&
                    string.IsNullOrEmpty(scene.path))
                {
                    return scene;
                }
            }

            return default(Scene);
        }

        private static void ValidateInternalRestoreSupport()
        {
            if (SetPathAndGuidMethod == null ||
                ClearSceneDirtinessMethod == null)
            {
                throw new InvalidOperationException(
                    "This Unity Editor version cannot safely lease an Untitled scene.");
            }
        }

        private static string CreateUniqueTemporaryPath()
        {
            string path;

            do
            {
                path =
                    "Assets/__EchoLaunch_UntitledLease_" +
                    Guid.NewGuid().ToString("N") +
                    ".unity";
            }
            while (AssetDatabase.LoadMainAssetAtPath(path) != null);

            return path;
        }

        private void DeleteTemporaryAsset()
        {
            if (string.IsNullOrEmpty(temporaryAssetPath))
            {
                return;
            }

            if (AssetDatabase.LoadMainAssetAtPath(temporaryAssetPath) != null &&
                !AssetDatabase.DeleteAsset(temporaryAssetPath))
            {
                throw new InvalidOperationException(
                    "Unity restored the Untitled scene but could not remove " +
                    "its temporary lease asset at '" +
                    temporaryAssetPath +
                    "'.");
            }

            AssetDatabase.Refresh(
                ImportAssetOptions.ForceSynchronousImport);
        }
    }
}
