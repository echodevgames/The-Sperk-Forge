
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveLaboratoryM506Tests
    {
        private const string PackageRoot =
            "Packages/com.echodevgames.echo-save";

        private const string SampleRoot =
            PackageRoot +
            "/Samples~/Chronicle Save Laboratory";

        [Test]
        public void PackageJson_DeclaresSingleChronicleLaboratorySample()
        {
            string text =
                File.ReadAllText(
                    PackageRoot +
                    "/package.json");

            StringAssert.Contains(
                "\"displayName\": \"Chronicle Save Laboratory\"",
                text);

            StringAssert.Contains(
                "\"path\": \"Samples~/Chronicle Save Laboratory\"",
                text);
        }

        [Test]
        public void Laboratory_DistributionContainsRequiredSceneConfigurationAndRuntimeSources()
        {
            string[] required =
            {
                "/README.md",
                "/Configuration/ChronicleSaveLaboratoryConfiguration.asset",
                "/Scenes/Chronicle_Save_Laboratory.unity",
                "/Runtime/EchoDevGames.EchoSave.Samples.ChronicleLaboratory.asmdef",
                "/Runtime/ChronicleSaveLaboratoryState.cs",
                "/Runtime/ChronicleSaveLaboratoryParticipant.cs",
                "/Runtime/ChronicleSaveLaboratoryHarness.cs"
            };

            for (int i = 0;
                 i < required.Length;
                 i++)
            {
                Assert.That(
                    File.Exists(
                        SampleRoot +
                        required[i]),
                    Is.True,
                    "Missing Chronicle Laboratory sample file: " +
                    required[i]);
            }
        }

        [Test]
        public void LaboratoryConfiguration_IsSchemaThreeAndUsesExactOwnedRoot()
        {
            string text =
                File.ReadAllText(
                    SampleRoot +
                    "/Configuration/ChronicleSaveLaboratoryConfiguration.asset");

            StringAssert.Contains(
                "schemaVersion: 3",
                text);

            StringAssert.Contains(
                "storageRootDirectoryName: EchoSave-M5-06-Laboratory",
                text);

            StringAssert.Contains(
                "slotPolicyMode: 2",
                text);
        }

        [Test]
        public void LaboratoryRuntime_DoesNotDependOnLookingGlassResonanceOrEditor()
        {
            string runtime =
                ReadRuntimeSources();

            StringAssert.DoesNotContain(
                "UnityEngine.UI",
                runtime);

            StringAssert.DoesNotContain(
                "TMPro",
                runtime);

            StringAssert.DoesNotContain(
                "EchoUI",
                runtime);

            StringAssert.DoesNotContain(
                "Resonance",
                runtime);

            StringAssert.DoesNotContain(
                "UnityEditor",
                runtime);
        }

        [Test]
        public void LaboratoryHarness_UsesRealChroniclePublicOperationsAndSperkProofState()
        {
            string text =
                File.ReadAllText(
                    SampleRoot +
                    "/Runtime/ChronicleSaveLaboratoryHarness.cs");

            StringAssert.Contains(
                "CreateSlotAsync",
                text);

            StringAssert.Contains(
                "SaveAsync",
                text);

            StringAssert.Contains(
                "LoadAndApplyAsync",
                text);

            StringAssert.Contains(
                "PrepareLoadAsync",
                text);

            StringAssert.Contains(
                "ApplyPreparedLoadAsync",
                text);

            StringAssert.Contains(
                "DuplicateSlotAsync",
                text);

            StringAssert.Contains(
                "PrepareDeleteSlotAsync",
                text);

            StringAssert.Contains(
                "ConfirmDeleteSlotAsync",
                text);

            StringAssert.Contains(
                "GALACTIC RUPEES",
                text);

            StringAssert.Contains(
                "THE CHRONICLE REMEMBERS",
                text);
        }

        [Test]
        public void LaboratoryScene_BindsRealEchoSaveRootConfigurationAndHarness()
        {
            string text =
                File.ReadAllText(
                    SampleRoot +
                    "/Scenes/Chronicle_Save_Laboratory.unity");

            StringAssert.Contains(
                "guid: 15eee5b2a9314270b7bd315739ae929e",
                text);

            StringAssert.Contains(
                "guid: eba53d6c4916419ebb0023dbe2201022",
                text);

            StringAssert.Contains(
                "guid: 2cca2a4536df43fc898ad5b65329dc39",
                text);

            StringAssert.Contains(
                "autoInitialize: 0",
                text);
        }

        [Test]
        public void LaboratoryReset_RequiresExactRootAndOwnershipMarker()
        {
            string text =
                File.ReadAllText(
                    SampleRoot +
                    "/Runtime/ChronicleSaveLaboratoryHarness.cs");

            StringAssert.Contains(
                "\"EchoSave-M5-06-Laboratory\"",
                text);

            StringAssert.Contains(
                "\"m506-laboratory-owned.txt\"",
                text);

            StringAssert.Contains(
                "\"ECHOSAVE-M5-06-LABORATORY\"",
                text);

            StringAssert.Contains(
                "RESET REFUSED",
                text);

            StringAssert.Contains(
                "Directory.Delete",
                text);
        }

        [Test]
        public void LaboratoryReadme_PreservesEngineeringAndReferenceShowcaseBoundary()
        {
            string text =
                File.ReadAllText(
                    SampleRoot +
                    "/README.md");

            StringAssert.Contains(
                "not a production save menu",
                text);

            StringAssert.Contains(
                "The Looking Glass",
                text);

            StringAssert.Contains(
                "Resonance",
                text);

            StringAssert.Contains(
                "Chronicle Reference Showcase",
                text);

            StringAssert.Contains(
                "LAB-001 through LAB-032",
                text);
        }

        private static string ReadRuntimeSources()
        {
            string[] files =
                Directory.GetFiles(
                    SampleRoot +
                    "/Runtime",
                    "*.cs",
                    SearchOption.TopDirectoryOnly);

            return string.Join(
                "\n",
                System.Array.ConvertAll(
                    files,
                    File.ReadAllText));
        }
    }
}
