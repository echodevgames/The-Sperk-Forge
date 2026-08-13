using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Samples
{
    public sealed class StandaloneLaboratoryPackageTests
    {
        private const string PackageId =
            "com.echodevgames.echo-launch";

        private const string SampleDisplayName =
            "First Light Boot Splash Laboratory";

        private const string SampleRelativePath =
            "Samples~/FirstLight_Boot_Splash_Laboratory";

        private const string SampleAssemblyName =
            "EchoDevGames.EchoLaunch.Samples.StandaloneLab";

        private const string RuntimeAssemblyGuidReference =
            "GUID:6370d00c0cfa8144795d367cb689f221";

        private const string PresentationAssemblyGuidReference =
            "GUID:0b148482ba0e46c4084e2d36a24141de";

        private static readonly string[] RequiredSourceFiles =
        {
            "Runtime/Readout/LaboratoryReadout.cs",
            "Runtime/Steps/LaboratoryImmediateSuccessStep.cs",
            "Runtime/Steps/LaboratoryTimedProgressStep.cs",
            "Runtime/Steps/LaboratoryWarningStep.cs",
            "Runtime/Steps/LaboratoryRecoverableFailureStep.cs",
            "Runtime/Steps/LaboratoryBlockingFailureStep.cs"
        };

        private static readonly string[] ForbiddenPeerTokens =
        {
            "EchoDevGames.Jukebot",
            "EchoDevGames.EchoUI",
            "EchoDevGames.EchoSave",
            "EchoDevGames.EchoSettings",
            "EchoDevGames.EchoSceneFlow",
            "EchoDevGames.EchoGameState",
            "EchoDevGames.EchoInput",
            "EchoDevGames.EchoDiagnostics"
        };

        [Test]
        public void ManifestDeclaresExactlyOneApprovedSample()
        {
            Manifest manifest =
                JsonUtility.FromJson<Manifest>(
                    File.ReadAllText(
                        Path.Combine(
                            PackageRoot,
                            "package.json")));

            Assert.That(manifest, Is.Not.Null);
            Assert.That(manifest.samples, Is.Not.Null);
            Assert.That(manifest.samples, Has.Length.EqualTo(1));
            Assert.That(
                manifest.samples[0].displayName,
                Is.EqualTo(SampleDisplayName));
            Assert.That(
                manifest.samples[0].path,
                Is.EqualTo(SampleRelativePath));
        }

        [Test]
        public void SampleShellContainsRequiredRuntimeSources()
        {
            Assert.That(
                File.Exists(
                    Path.Combine(
                        SampleRoot,
                        "README.md")),
                Is.True);

            Assert.That(
                File.Exists(
                    Path.Combine(
                        SampleRoot,
                        "Runtime",
                        SampleAssemblyName + ".asmdef")),
                Is.True);

            foreach (string relativePath in RequiredSourceFiles)
            {
                Assert.That(
                    File.Exists(
                        Path.Combine(
                            SampleRoot,
                            relativePath)),
                    Is.True,
                    relativePath);
            }

            string readoutSource =
                File.ReadAllText(
                    Path.Combine(
                        SampleRoot,
                        "Runtime",
                        "Readout",
                        "LaboratoryReadout.cs"));

            Assert.That(
                readoutSource,
                Does.Contain("RequestSplashSkip()"));

            Assert.That(
                readoutSource,
                Does.Contain("MinimumDisplaySeconds"));

            Assert.That(
                readoutSource,
                Does.Contain("CanSkipNow"));
        }

        [Test]
        public void SampleRuntimeAssemblyReferencesOnlyRequiredFirstLightAssemblies()
        {
            AssemblyDefinition definition =
                JsonUtility.FromJson<AssemblyDefinition>(
                    File.ReadAllText(
                        Path.Combine(
                            SampleRoot,
                            "Runtime",
                            SampleAssemblyName + ".asmdef")));

            Assert.That(definition, Is.Not.Null);
            Assert.That(
                definition.name,
                Is.EqualTo(SampleAssemblyName));
            Assert.That(
                definition.references,
                Is.EquivalentTo(
                    new[]
                    {
                        RuntimeAssemblyGuidReference,
                        PresentationAssemblyGuidReference
                    }));
        }

        [Test]
        public void SampleContainsNoEditorAuthoringSurface()
        {
            Assert.That(
                Directory.Exists(
                    Path.Combine(
                        SampleRoot,
                        "Editor")),
                Is.False);

            string[] fileNames =
                Directory.GetFiles(
                    SampleRoot,
                    "*",
                    SearchOption.AllDirectories)
                .Select(Path.GetFileName)
                .ToArray();

            Assert.That(
                fileNames.Any(
                    fileName =>
                        fileName.IndexOf(
                            "Generator",
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        fileName.IndexOf(
                            "Authoring",
                            StringComparison.OrdinalIgnoreCase) >= 0),
                Is.False);
        }

        [Test]
        public void SampleSourceUsesNoForbiddenPeerNamespace()
        {
            foreach (string sourcePath in
                Directory.GetFiles(
                    SampleRoot,
                    "*.cs",
                    SearchOption.AllDirectories))
            {
                string source =
                    File.ReadAllText(sourcePath);

                foreach (string forbiddenToken in ForbiddenPeerTokens)
                {
                    Assert.That(
                        source,
                        Does.Not.Contain(forbiddenToken),
                        $"{sourcePath} references {forbiddenToken}.");
                }
            }
        }

        [Test]
        public void CoreAssembliesDoNotReferenceSampleAssembly()
        {
            foreach (string assemblyPath in
                Directory.GetFiles(
                    PackageRoot,
                    "*.asmdef",
                    SearchOption.AllDirectories))
            {
                if (IsInsideSampleRoot(assemblyPath))
                {
                    continue;
                }

                Assert.That(
                    File.ReadAllText(assemblyPath),
                    Does.Not.Contain(SampleAssemblyName),
                    assemblyPath);
            }
        }

        private static string ProjectRoot =>
            Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    ".."));

        private static string PackageRoot =>
            Path.Combine(
                ProjectRoot,
                "Packages",
                PackageId);

        private static string SampleRoot =>
            Path.Combine(
                PackageRoot,
                SampleRelativePath.Replace(
                    '/',
                    Path.DirectorySeparatorChar));

        private static bool IsInsideSampleRoot(
            string path)
        {
            string fullPath =
                Path.GetFullPath(path);

            string fullSampleRoot =
                Path.GetFullPath(SampleRoot)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            return fullPath.StartsWith(
                fullSampleRoot,
                StringComparison.OrdinalIgnoreCase);
        }

        [Serializable]
        private sealed class Manifest
        {
            public ManifestSample[] samples;
        }

        [Serializable]
        private sealed class ManifestSample
        {
            public string displayName;
            public string path;
        }

        [Serializable]
        private sealed class AssemblyDefinition
        {
            public string name;
            public string[] references;
        }
    }
}
