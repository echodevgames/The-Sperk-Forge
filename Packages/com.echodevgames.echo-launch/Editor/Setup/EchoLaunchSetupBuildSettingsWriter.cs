using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal interface IEchoLaunchSetupBuildSettingsWriter
    {
        bool Apply(
            EchoLaunchBuildSettingsPolicy policy,
            string bootScenePath,
            bool approvePlaceFirst,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log);
    }

    internal sealed class EchoLaunchSetupBuildSettingsWriter :
        IEchoLaunchSetupBuildSettingsWriter
    {
        public bool Apply(
            EchoLaunchBuildSettingsPolicy policy,
            string bootScenePath,
            bool approvePlaceFirst,
            EchoLaunchSetupRollbackJournal journal,
            EchoLaunchSetupExecutionLog log)
        {
            string normalized =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    bootScenePath);

            EditorBuildSettingsScene[] current =
                Clone(EditorBuildSettings.scenes);

            if (policy == EchoLaunchBuildSettingsPolicy.DoNotChange)
            {
                return false;
            }

            if (policy ==
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd)
            {
                for (int index = 0; index < current.Length; index++)
                {
                    if (string.Equals(
                            current[index].path,
                            normalized,
                            StringComparison.Ordinal))
                    {
                        return false;
                    }
                }

                journal.CaptureBuildSettings();

                EditorBuildSettingsScene[] updated =
                    new EditorBuildSettingsScene[current.Length + 1];

                for (int index = 0; index < current.Length; index++)
                {
                    updated[index] =
                        new EditorBuildSettingsScene(
                            current[index].path,
                            current[index].enabled);
                }

                updated[current.Length] =
                    new EditorBuildSettingsScene(normalized, true);

                EditorBuildSettings.scenes = updated;
                journal.MarkBuildSettingsChanged();

                log.Add(
                    EchoLaunchSetupChangeKind.BuildSettingsChanged,
                    normalized,
                    "Appended one enabled Boot scene entry.");

                return true;
            }

            if (!approvePlaceFirst)
            {
                throw new InvalidOperationException(
                    "Place-first Build Settings mutation was not approved.");
            }

            List<EditorBuildSettingsScene> unrelated =
                new List<EditorBuildSettingsScene>();

            for (int index = 0; index < current.Length; index++)
            {
                if (!string.Equals(
                        current[index].path,
                        normalized,
                        StringComparison.Ordinal))
                {
                    unrelated.Add(
                        new EditorBuildSettingsScene(
                            current[index].path,
                            current[index].enabled));
                }
            }

            bool alreadyCanonical =
                current.Length > 0 &&
                string.Equals(
                    current[0].path,
                    normalized,
                    StringComparison.Ordinal) &&
                current[0].enabled &&
                unrelated.Count == current.Length - 1;

            if (alreadyCanonical)
            {
                return false;
            }

            journal.CaptureBuildSettings();

            EditorBuildSettingsScene[] promoted =
                new EditorBuildSettingsScene[unrelated.Count + 1];

            promoted[0] =
                new EditorBuildSettingsScene(normalized, true);

            for (int index = 0; index < unrelated.Count; index++)
            {
                promoted[index + 1] = unrelated[index];
            }

            EditorBuildSettings.scenes = promoted;
            journal.MarkBuildSettingsChanged();

            log.Add(
                EchoLaunchSetupChangeKind.BuildSettingsChanged,
                normalized,
                "Placed one enabled Boot scene entry at index zero.");

            return true;
        }

        internal static EditorBuildSettingsScene[] Clone(
            EditorBuildSettingsScene[] source)
        {
            if (source == null)
            {
                return Array.Empty<EditorBuildSettingsScene>();
            }

            EditorBuildSettingsScene[] result =
                new EditorBuildSettingsScene[source.Length];

            for (int index = 0; index < source.Length; index++)
            {
                result[index] =
                    new EditorBuildSettingsScene(
                        source[index].path,
                        source[index].enabled);
            }

            return result;
        }

        internal static string Summarize(
            EditorBuildSettingsScene[] scenes)
        {
            if (scenes == null || scenes.Length == 0)
            {
                return "None";
            }

            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < scenes.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(" | ");
                }

                builder.Append(index);
                builder.Append(':');
                builder.Append(scenes[index].enabled ? "On:" : "Off:");
                builder.Append(scenes[index].path);
            }

            return builder.ToString();
        }
    }
}
