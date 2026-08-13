using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Samples
{
    public sealed class StandaloneLaboratoryAssetTests
    {
        private const string PackageId =
            "com.echodevgames.echo-launch";

        private const string SampleRelativeRoot =
            "Samples~/FirstLight_Boot_Splash_Laboratory";

        private const string ImportedDestinationScenePath =
            "Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/Scenes/FirstLight_Destination_Lab.unity";

        private const string InvalidDestinationScenePath =
            "Assets/Samples/First Light — Startup and Launch/0.1.0/First Light Boot Splash Laboratory/Scenes/Missing_Laboratory_Destination.unity";

        [Test]
        public void RequiredAuthoredAssetTreeExists()
        {
            string[] required =
            {
                "Art/FirstLight_Laboratory_Splash.png",
                "Configuration/SuccessConfiguration.asset",
                "Configuration/TimedProgressConfiguration.asset",
                "Configuration/WarningConfiguration.asset",
                "Configuration/RecoverableConfiguration.asset",
                "Configuration/BlockingConfiguration.asset",
                "Configuration/InvalidDestinationConfiguration.asset",
                "Configuration/LaboratoryDestination.asset",
                "Configuration/InvalidDestination.asset",
                "Configuration/LaboratoryDirectSceneConfiguration.asset",
                "Configuration/LaboratorySplashSequence.asset",
                "Configuration/SuccessSequence.asset",
                "Configuration/TimedProgressSequence.asset",
                "Configuration/WarningSequence.asset",
                "Configuration/RecoverableSequence.asset",
                "Configuration/BlockingSequence.asset",
                "Configuration/Steps/LaboratoryImmediateSuccessStep.asset",
                "Configuration/Steps/LaboratoryTimedProgressStep.asset",
                "Configuration/Steps/LaboratoryWarningStep.asset",
                "Configuration/Steps/LaboratoryRecoverableFailureStep.asset",
                "Configuration/Steps/LaboratoryBlockingFailureStep.asset",
                "Prefabs/EchoLaunchRoot_Laboratory.prefab",
                "Scenes/FirstLight_Boot_Lab.unity",
                "Scenes/FirstLight_Destination_Lab.unity"
            };

            foreach (string relative in required)
            {
                string path =
                    SamplePath(relative);

                Assert.That(
                    File.Exists(path),
                    Is.True,
                    relative);

                Assert.That(
                    File.Exists(path + ".meta"),
                    Is.True,
                    relative + ".meta");
            }
        }

        [Test]
        public void ValidDestinationTargetsNormalImportedSampleScene()
        {
            Assert.That(
                ReadQuotedYamlScalar(
                    "Configuration/LaboratoryDestination.asset",
                    "scenePath"),
                Is.EqualTo(
                    ImportedDestinationScenePath));

            string yaml =
                ReadSampleText(
                    "Configuration/LaboratoryDestination.asset");

            Assert.That(
                yaml,
                Does.Contain(
                    "displayName: First Light Laboratory Destination"));
        }

        [Test]
        public void InvalidDestinationRetainsDeliberatelyMissingScene()
        {
            Assert.That(
                ReadQuotedYamlScalar(
                    "Configuration/InvalidDestination.asset",
                    "scenePath"),
                Is.EqualTo(
                    InvalidDestinationScenePath));

            string yaml =
                ReadSampleText(
                    "Configuration/InvalidDestination.asset");

            Assert.That(
                yaml,
                Does.Contain(
                    "displayName: Missing Laboratory Destination"));
        }

        [Test]
        public void ScenarioConfigurationsRetainExpectedReferenceGraph()
        {
            AssertReferences(
                "Configuration/SuccessConfiguration.asset",
                "Configuration/SuccessSequence.asset",
                "Configuration/LaboratoryDestination.asset",
                "Configuration/LaboratorySplashSequence.asset");

            AssertReferences(
                "Configuration/TimedProgressConfiguration.asset",
                "Configuration/TimedProgressSequence.asset",
                "Configuration/LaboratoryDestination.asset");

            AssertReferences(
                "Configuration/WarningConfiguration.asset",
                "Configuration/WarningSequence.asset",
                "Configuration/LaboratoryDestination.asset");

            AssertReferences(
                "Configuration/RecoverableConfiguration.asset",
                "Configuration/RecoverableSequence.asset",
                "Configuration/LaboratoryDestination.asset");

            AssertReferences(
                "Configuration/BlockingConfiguration.asset",
                "Configuration/BlockingSequence.asset",
                "Configuration/LaboratoryDestination.asset");

            AssertReferences(
                "Configuration/InvalidDestinationConfiguration.asset",
                "Configuration/SuccessSequence.asset",
                "Configuration/InvalidDestination.asset");
        }

        [Test]
        public void SequenceAssetsRetainExpectedStepReferences()
        {
            AssertReferences(
                "Configuration/SuccessSequence.asset",
                "Configuration/Steps/LaboratoryImmediateSuccessStep.asset",
                "Configuration/Steps/LaboratoryTimedProgressStep.asset");

            AssertReferences(
                "Configuration/TimedProgressSequence.asset",
                "Configuration/Steps/LaboratoryTimedProgressStep.asset");

            AssertReferences(
                "Configuration/WarningSequence.asset",
                "Configuration/Steps/LaboratoryWarningStep.asset",
                "Configuration/Steps/LaboratoryImmediateSuccessStep.asset");

            AssertReferences(
                "Configuration/RecoverableSequence.asset",
                "Configuration/Steps/LaboratoryRecoverableFailureStep.asset",
                "Configuration/Steps/LaboratoryImmediateSuccessStep.asset");

            AssertReferences(
                "Configuration/BlockingSequence.asset",
                "Configuration/Steps/LaboratoryBlockingFailureStep.asset",
                "Configuration/Steps/LaboratoryImmediateSuccessStep.asset");
        }

        [Test]
        public void LaboratoryRootPrefabRetainsConfigurationAndDirectMode()
        {
            string yaml =
                ReadSampleText(
                    "Prefabs/EchoLaunchRoot_Laboratory.prefab");

            string successGuid =
                ReadGuid(
                    "Configuration/SuccessConfiguration.asset");

            Assert.That(
                yaml,
                Does.Contain(
                    "guid: " +
                    successGuid));

            Assert.That(
                yaml,
                Does.Contain(
                    "launchMode: 2"));

            Assert.That(
                yaml,
                Does.Contain(
                    "startAutomatically: 1"));
        }

        [Test]
        public void DirectSceneConfigurationAndScenesRetainReferences()
        {
            string rootGuid =
                ReadGuid(
                    "Prefabs/EchoLaunchRoot_Laboratory.prefab");

            string directGuid =
                ReadGuid(
                    "Configuration/LaboratoryDirectSceneConfiguration.asset");

            string readoutScriptGuid =
                ReadGuid(
                    "Runtime/Readout/LaboratoryReadout.cs");

            string directYaml =
                ReadSampleText(
                    "Configuration/LaboratoryDirectSceneConfiguration.asset");

            string bootYaml =
                ReadSampleText(
                    "Scenes/FirstLight_Boot_Lab.unity");

            string destinationYaml =
                ReadSampleText(
                    "Scenes/FirstLight_Destination_Lab.unity");

            Assert.That(
                directYaml,
                Does.Contain(
                    "guid: " +
                    rootGuid));

            Assert.That(
                bootYaml,
                Does.Contain(
                    "guid: " +
                    rootGuid));

            Assert.That(
                bootYaml,
                Does.Contain(
                    "guid: " +
                    readoutScriptGuid));

            Assert.That(
                ContainsNullObjectReferenceOverride(
                    bootYaml,
                    "configuration"),
                Is.False,
                "The canonical Boot scene must inherit the Laboratory root prefab configuration instead of overriding it with null.");

            Assert.That(
                destinationYaml,
                Does.Contain(
                    "guid: " +
                    directGuid));

            Assert.That(
                destinationYaml,
                Does.Contain(
                    "guid: " +
                    readoutScriptGuid));
        }

        [Test]
        public void SplashSequenceRetainsSplashSpriteReference()
        {
            string splashGuid =
                ReadGuid(
                    "Art/FirstLight_Laboratory_Splash.png");

            string yaml =
                ReadSampleText(
                    "Configuration/LaboratorySplashSequence.asset");

            Assert.That(
                yaml,
                Does.Contain(
                    "guid: " +
                    splashGuid));

            Assert.That(
                yaml,
                Does.Contain(
                    "minimumDisplaySeconds: 5"));

            Assert.That(
                yaml,
                Does.Contain(
                    "skipPolicy: 1"));
        }

        private static string ReadQuotedYamlScalar(
            string relativeAssetPath,
            string propertyName)
        {
            string[] lines =
                File.ReadAllLines(
                    SamplePath(
                        relativeAssetPath));

            string prefix =
                propertyName +
                ":";

            for (int index = 0;
                index < lines.Length;
                index++)
            {
                string trimmed =
                    lines[index]
                        .TrimStart();

                if (!trimmed.StartsWith(
                        prefix,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                string remainder =
                    trimmed.Substring(
                        prefix.Length)
                    .TrimStart();

                if (!remainder.StartsWith(
                        "\"",
                        StringComparison.Ordinal))
                {
                    Assert.Fail(
                        $"{relativeAssetPath}: {propertyName} is not a quoted YAML scalar.");
                }

                StringBuilder encoded =
                    new StringBuilder();

                remainder =
                    remainder.Substring(1);

                while (true)
                {
                    bool closes =
                        remainder.EndsWith(
                            "\"",
                            StringComparison.Ordinal);

                    string fragment =
                        closes
                            ? remainder.Substring(
                                0,
                                remainder.Length - 1)
                            : remainder;

                    if (encoded.Length > 0)
                    {
                        encoded.Append(' ');
                    }

                    encoded.Append(
                        fragment.Trim());

                    if (closes)
                    {
                        return DecodeUnityYamlEscapes(
                            encoded.ToString());
                    }

                    index++;

                    if (index >= lines.Length)
                    {
                        Assert.Fail(
                            $"{relativeAssetPath}: unterminated quoted YAML scalar '{propertyName}'.");
                    }

                    remainder =
                        lines[index].Trim();
                }
            }

            Assert.Fail(
                $"{relativeAssetPath}: YAML property '{propertyName}' was not found.");

            return string.Empty;
        }

        private static string DecodeUnityYamlEscapes(
            string encoded)
        {
            StringBuilder decoded =
                new StringBuilder(
                    encoded.Length);

            for (int index = 0;
                index < encoded.Length;
                index++)
            {
                char current =
                    encoded[index];

                if (current != '\\' ||
                    index + 1 >=
                    encoded.Length)
                {
                    decoded.Append(
                        current);

                    continue;
                }

                char escape =
                    encoded[index + 1];

                if (escape == 'u' &&
                    index + 5 <
                    encoded.Length)
                {
                    string hex =
                        encoded.Substring(
                            index + 2,
                            4);

                    if (ushort.TryParse(
                            hex,
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out ushort codePoint))
                    {
                        decoded.Append(
                            (char)codePoint);

                        index += 5;

                        continue;
                    }
                }

                switch (escape)
                {
                    case '\\':
                        decoded.Append('\\');
                        index++;
                        break;

                    case '"':
                        decoded.Append('"');
                        index++;
                        break;

                    case 'n':
                        decoded.Append('\n');
                        index++;
                        break;

                    case 'r':
                        decoded.Append('\r');
                        index++;
                        break;

                    case 't':
                        decoded.Append('\t');
                        index++;
                        break;

                    default:
                        decoded.Append(current);
                        break;
                }
            }

            return decoded.ToString();
        }

        private static bool ContainsNullObjectReferenceOverride(
            string yaml,
            string propertyPath)
        {
            string normalized =
                yaml.Replace(
                    "\r\n",
                    "\n");

            string[] lines =
                normalized.Split('\n');

            string expectedProperty =
                "propertyPath: " +
                propertyPath;

            for (int index = 0;
                index < lines.Length;
                index++)
            {
                if (!string.Equals(
                        lines[index].Trim(),
                        expectedProperty,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                int limit =
                    Math.Min(
                        lines.Length,
                        index + 5);

                for (int candidate = index + 1;
                    candidate < limit;
                    candidate++)
                {
                    if (string.Equals(
                            lines[candidate].Trim(),
                            "objectReference: {fileID: 0}",
                            StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void AssertReferences(
            string ownerRelativePath,
            params string[] referencedRelativePaths)
        {
            string yaml =
                ReadSampleText(
                    ownerRelativePath);

            foreach (string reference in referencedRelativePaths)
            {
                string guid =
                    ReadGuid(reference);

                Assert.That(
                    yaml,
                    Does.Contain(
                        "guid: " +
                        guid),
                    ownerRelativePath +
                    " -> " +
                    reference);
            }
        }

        private static string ReadGuid(
            string relativeAssetPath)
        {
            string metaPath =
                SamplePath(
                    relativeAssetPath) +
                ".meta";

            foreach (string line in
                File.ReadAllLines(metaPath))
            {
                const string prefix =
                    "guid: ";

                if (line.StartsWith(
                        prefix,
                        StringComparison.Ordinal))
                {
                    string guid =
                        line.Substring(
                            prefix.Length)
                        .Trim();

                    Assert.That(
                        guid.Length,
                        Is.EqualTo(32),
                        relativeAssetPath);

                    return guid;
                }
            }

            Assert.Fail(
                "No GUID was found in " +
                relativeAssetPath +
                ".meta");

            return string.Empty;
        }

        private static string ReadSampleText(
            string relativePath)
        {
            return File.ReadAllText(
                SamplePath(
                    relativePath));
        }

        private static string SamplePath(
            string relativePath)
        {
            return Path.Combine(
                PackageRoot,
                SampleRelativeRoot.Replace(
                    '/',
                    Path.DirectorySeparatorChar),
                relativePath.Replace(
                    '/',
                    Path.DirectorySeparatorChar));
        }

        private static string PackageRoot =>
            Path.Combine(
                Path.GetFullPath(
                    Path.Combine(
                        Application.dataPath,
                        "..")),
                "Packages",
                PackageId);
    }
}
