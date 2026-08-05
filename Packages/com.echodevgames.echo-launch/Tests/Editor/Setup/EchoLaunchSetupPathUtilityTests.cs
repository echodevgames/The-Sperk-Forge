
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupPathUtilityTests
    {
        [Test]
        public void DefaultPathSetUsesApprovedRoot()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            Assert.That(
                paths.ProjectRootPath,
                Is.EqualTo("Assets/EchoDevGames/FirstLight"));
        }

        [Test]
        public void DefaultPathSetUsesApprovedBootScene()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            Assert.That(
                paths.BootScenePath,
                Is.EqualTo(
                    "Assets/EchoDevGames/FirstLight/Scenes/Boot.unity"));
        }

        [Test]
        public void DefaultPathSetBuildsConfigurationPath()
        {
            Assert.That(
                EchoLaunchSetupPathSet.CreateDefault().ConfigurationAssetPath,
                Does.EndWith(
                    "/Configuration/EchoLaunchConfiguration.asset"));
        }

        [Test]
        public void DefaultPathSetBuildsRootPrefabPath()
        {
            Assert.That(
                EchoLaunchSetupPathSet.CreateDefault().RootPrefabPath,
                Does.EndWith("/Prefabs/EchoLaunchRoot.prefab"));
        }

        [Test]
        public void BackslashesAreNormalized()
        {
            bool valid =
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    @"Assets\Echo\Boot.unity",
                    ".unity",
                    out string normalized,
                    out _);

            Assert.That(valid, Is.True);
            Assert.That(normalized, Is.EqualTo("Assets/Echo/Boot.unity"));
        }

        [Test]
        public void DuplicateSeparatorsAreCollapsed()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    "Assets//Echo///Boot.unity"),
                Is.EqualTo("Assets/Echo/Boot.unity"));
        }

        [Test]
        public void AbsolutePathIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    "C:/Project/Assets/Boot.unity",
                    ".unity",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void PackagesPathIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    "Packages/example/Boot.unity",
                    ".unity",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void ProjectSettingsPathIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    "ProjectSettings/Boot.unity",
                    ".unity",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void TraversalSegmentIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    "Assets/Echo/../Boot.unity",
                    ".unity",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void WrongExtensionIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectAssetPath(
                    "Assets/Echo/Boot.asset",
                    ".unity",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void NestedAssetsRootIsAccepted()
        {
            bool valid =
                EchoLaunchSetupPathUtility.TryNormalizeProjectRoot(
                    "Assets/Studio/Game/FirstLight",
                    out string normalized,
                    out _);

            Assert.That(valid, Is.True);
            Assert.That(
                normalized,
                Is.EqualTo("Assets/Studio/Game/FirstLight"));
        }

        [Test]
        public void AssetsRootAloneIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectRoot(
                    "Assets",
                    out _,
                    out _),
                Is.False);
        }

        [Test]
        public void FileLikeProjectRootIsRejected()
        {
            Assert.That(
                EchoLaunchSetupPathUtility.TryNormalizeProjectRoot(
                    "Assets/Echo/Root.asset",
                    out _,
                    out _),
                Is.False);
        }
    }
}
