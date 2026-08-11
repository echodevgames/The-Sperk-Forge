using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveEditorAssemblyBoundaryTests
    {
        private const string RuntimeAsmdef =
            "Packages/com.echodevgames.echo-save/Runtime/EchoDevGames.EchoSave.Runtime.asmdef";
        private const string EditorAsmdef =
            "Packages/com.echodevgames.echo-save/Editor/EchoDevGames.EchoSave.Editor.asmdef";

        [Test]
        public void EditorAsmdef_IsEditorOnly_AndReferencesRuntime()
        {
            AsmdefShape definition =
                ReadAsmdef(EditorAsmdef);

            Assert.That(
                definition.name,
                Is.EqualTo(
                    "EchoDevGames.EchoSave.Editor"));
            Assert.That(
                definition.includePlatforms,
                Is.EqualTo(
                    new[] { "Editor" }));
            Assert.That(
                definition.references,
                Does.Contain(
                    "EchoDevGames.EchoSave.Runtime"));
        }

        [Test]
        public void RuntimeAsmdef_DoesNotReferenceEditorAssemblyOrUnityEditor()
        {
            AsmdefShape definition =
                ReadAsmdef(RuntimeAsmdef);
            string text =
                ReadProjectText(RuntimeAsmdef);

            Assert.That(
                definition.references,
                Does.Not.Contain(
                    "EchoDevGames.EchoSave.Editor"));
            Assert.That(
                text,
                Does.Not.Contain(
                    "UnityEditor"));
        }

        [Test]
        public void EditorAsmdef_IntroducesNoPeerEchoPackageReference()
        {
            AsmdefShape definition =
                ReadAsmdef(EditorAsmdef);

            Assert.That(
                definition.references,
                Is.EqualTo(
                    new[]
                    {
                        "EchoDevGames.EchoSave.Runtime"
                    }));
        }

        private static AsmdefShape ReadAsmdef(
            string projectPath)
        {
            return JsonUtility.FromJson<
                AsmdefShape>(
                ReadProjectText(projectPath));
        }

        private static string ReadProjectText(
            string projectPath)
        {
            string projectRoot =
                Path.GetDirectoryName(
                    Application.dataPath) ??
                Directory.GetCurrentDirectory();

            string absolutePath =
                Path.GetFullPath(
                    Path.Combine(
                        projectRoot,
                        projectPath));

            Assert.That(
                File.Exists(absolutePath),
                Is.True,
                projectPath);

            return File.ReadAllText(
                absolutePath);
        }

        [Serializable]
        private sealed class AsmdefShape
        {
            public string name;
            public string[] references;
            public string[] includePlatforms;
        }
    }
}
