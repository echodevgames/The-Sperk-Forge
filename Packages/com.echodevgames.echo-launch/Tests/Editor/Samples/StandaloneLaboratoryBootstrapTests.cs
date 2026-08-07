using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor.PackageManager;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Samples
{
    public sealed class StandaloneLaboratoryBootstrapTests
    {
        private const string PackageName =
            "com.echodevgames.echo-launch";

        private const string SampleRelativePath =
            "Samples~/First Light Standalone Test Lab";

        [Test]
        public void ManifestDeclaresOneApprovedSample()
        {
            string json =
                File.ReadAllText(
                    Path.Combine(
                        GetPackageRoot(),
                        "package.json"));

            Assert.That(
                Count(
                    json,
                    "\"displayName\": \"First Light Standalone Test Lab\""),
                Is.EqualTo(1));

            Assert.That(
                Count(
                    json,
                    "\"path\": \"Samples~/First Light Standalone Test Lab\""),
                Is.EqualTo(1));

            Assert.That(
                json,
                Does.Contain("\"samples\""));
        }

        [Test]
        public void BootstrapInventoryIsPresent()
        {
            string root =
                Path.Combine(
                    GetPackageRoot(),
                    SampleRelativePath);

            string[] required =
            {
                "README.md",
                "Runtime/EchoDevGames.EchoLaunch.Samples.StandaloneLab.asmdef",
                "Runtime/Steps/LaboratoryImmediateSuccessStep.cs",
                "Runtime/Steps/LaboratoryTimedProgressStep.cs",
                "Runtime/Steps/LaboratoryWarningStep.cs",
                "Runtime/Steps/LaboratoryRecoverableFailureStep.cs",
                "Runtime/Steps/LaboratoryBlockingFailureStep.cs",
                "Runtime/Readout/LaboratoryReadout.cs",
                "Editor/EchoDevGames.EchoLaunch.Samples.StandaloneLab.Editor.asmdef",
                "Editor/LaboratorySampleAuthoring.cs"
            };

            foreach (string relative in required)
            {
                Assert.That(
                    File.Exists(
                        Path.Combine(root, relative)),
                    Is.True,
                    relative);
            }
        }

        [Test]
        public void SampleRuntimeAssemblyReferencesOnlyFirstLightRuntime()
        {
            string text =
                File.ReadAllText(
                    Path.Combine(
                        GetPackageRoot(),
                        SampleRelativePath,
                        "Runtime",
                        "EchoDevGames.EchoLaunch.Samples.StandaloneLab.asmdef"));

            Assert.That(
                text,
                Does.Contain(
                    "EchoDevGames.EchoLaunch.Runtime"));

            Assert.That(
                text,
                Does.Not.Contain("EchoSave"));

            Assert.That(
                text,
                Does.Not.Contain("EchoSceneFlow"));

            Assert.That(
                text,
                Does.Not.Contain("EchoDiagnostics"));

            Assert.That(
                text,
                Does.Not.Contain("Jukebot"));
        }

        [Test]
        public void CoreAssembliesDoNotReferenceSampleAssembly()
        {
            string root = GetPackageRoot();

            string runtime =
                File.ReadAllText(
                    Path.Combine(
                        root,
                        "Runtime",
                        "EchoDevGames.EchoLaunch.Runtime.asmdef"));

            string editor =
                File.ReadAllText(
                    Path.Combine(
                        root,
                        "Editor",
                        "EchoDevGames.EchoLaunch.Editor.asmdef"));

            Assert.That(
                runtime,
                Does.Not.Contain(
                    "Samples.StandaloneLab"));

            Assert.That(
                editor,
                Does.Not.Contain(
                    "Samples.StandaloneLab"));
        }

        [Test]
        public void SampleSourceUsesNoForbiddenDiscoveryOrFriendAccess()
        {
            string root =
                Path.Combine(
                    GetPackageRoot(),
                    SampleRelativePath);

            string source =
                string.Join(
                    "\n",
                    Directory.GetFiles(
                            root,
                            "*.cs",
                            SearchOption.AllDirectories)
                        .Select(File.ReadAllText));

            string[] forbidden =
            {
                "InternalsVisibleTo",
                "Resources.Load",
                "FindObjectOfType",
                "FindFirstObjectByType",
                "FindObjectsByType",
                "Assembly.Load",
                "Type.GetType"
            };

            foreach (string token in forbidden)
            {
                Assert.That(
                    source,
                    Does.Not.Contain(token),
                    token);
            }
        }

        [Test]
        public void GeneratedDestinationSceneBindsDirectSceneConfiguration()
        {
            string root =
                Path.Combine(
                    GetPackageRoot(),
                    SampleRelativePath);

            string scenePath =
                Path.Combine(
                    root,
                    "Generated",
                    "Scenes",
                    "FirstLight_Destination_Lab.unity");

            string configurationPath =
                Path.Combine(
                    root,
                    "Generated",
                    "Configuration",
                    "LaboratoryDirectSceneConfiguration.asset");

            Assert.That(
                File.Exists(scenePath),
                Is.True,
                scenePath);

            Assert.That(
                File.Exists(configurationPath),
                Is.True,
                configurationPath);

            Assert.That(
                File.Exists(configurationPath + ".meta"),
                Is.True,
                configurationPath + ".meta");

            string guid =
                ReadGuid(
                    configurationPath + ".meta");

            string expected =
                "directSceneConfiguration: " +
                "{fileID: 11400000, guid: " +
                guid +
                ", type: 2}";

            Assert.That(
                File.ReadAllText(scenePath),
                Does.Contain(expected));
        }

        [Test]
        public void AuthoringCommandIsExplicitAndNotAutomatic()
        {
            string text =
                File.ReadAllText(
                    Path.Combine(
                        GetPackageRoot(),
                        SampleRelativePath,
                        "Editor",
                        "LaboratorySampleAuthoring.cs"));

            Assert.That(
                text,
                Does.Contain("[MenuItem("));

            Assert.That(
                text,
                Does.Not.Contain("[InitializeOnLoad"));

            Assert.That(
                text,
                Does.Not.Contain("DidReloadScripts"));

            Assert.That(
                text,
                Does.Not.Contain("InitializeOnEnterPlayMode"));
        }

        private static string GetPackageRoot()
        {
            PackageInfo package =
                PackageInfo.GetAllRegisteredPackages()
                    .Single(
                        item =>
                            string.Equals(
                                item.name,
                                PackageName,
                                StringComparison.Ordinal));

            return package.resolvedPath;
        }

        private static string ReadGuid(
            string metaPath)
        {
            string[] lines =
                File.ReadAllLines(metaPath);

            foreach (string line in lines)
            {
                const string prefix = "guid: ";

                if (line.StartsWith(
                        prefix,
                        StringComparison.Ordinal))
                {
                    return line.Substring(
                            prefix.Length)
                        .Trim();
                }
            }

            Assert.Fail(
                "No GUID was found in " + metaPath);

            return string.Empty;
        }

        private static int Count(
            string value,
            string token)
        {
            int count = 0;
            int offset = 0;

            while (true)
            {
                int index =
                    value.IndexOf(
                        token,
                        offset,
                        StringComparison.Ordinal);

                if (index < 0)
                {
                    return count;
                }

                count++;
                offset = index + token.Length;
            }
        }
    }
}
