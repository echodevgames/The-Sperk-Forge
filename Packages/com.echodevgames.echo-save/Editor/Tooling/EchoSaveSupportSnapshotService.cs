using System;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Payload-free bounded support snapshot builder.
    ///
    /// It never receives participant payload contents and hashes technical
    /// slot/generation/root identity before writing support JSON.
    /// </summary>
    public sealed class EchoSaveSupportSnapshotService
    {
        public const int MaximumCatalogEntries = 64;
        public const int MaximumGenerationEntries = 64;

        public EchoSaveSupportSnapshotResult Build(
            EchoSaveConfiguration configuration,
            SaveSlotCatalogSnapshot catalog,
            SaveGenerationInspectionSnapshot generations,
            string selectedSlotId)
        {
            if (configuration == null)
            {
                return Failure(
                    "M504-SUPPORT-CONFIG",
                    "A Chronicle configuration is required.");
            }

            if (!configuration.TryResolveRuntimePolicy(
                    out EchoSaveRuntimePolicy policy,
                    out string policyMessage))
            {
                return Failure(
                    "M504-SUPPORT-CONFIG",
                    policyMessage);
            }

            if (catalog == null)
            {
                return Failure(
                    "M504-SUPPORT-CATALOG",
                    "A payload-free Chronicle catalog snapshot is required.");
            }

            StringBuilder json =
                new StringBuilder();

            json.Append("{\n");
            AppendString(
                json,
                "schema",
                "echosave.support.snapshot.v1",
                true);

            AppendNumber(
                json,
                "configurationSchema",
                policy.SourceConfigurationSchema,
                true);

            AppendString(
                json,
                "serializerProviderId",
                policy.SerializerProviderId,
                true);

            AppendString(
                json,
                "storageProviderId",
                policy.StorageProviderId,
                true);

            AppendString(
                json,
                "slotPolicyMode",
                policy.SlotPolicy.Mode.ToString(),
                true);

            AppendNumber(
                json,
                "slotCapacity",
                policy.SlotPolicy.EffectiveCapacity,
                true);

            AppendString(
                json,
                "rootToken",
                Token(
                    "root|" +
                    configuration.StorageRootDirectoryName),
                true);

            AppendNumber(
                json,
                "catalogCount",
                catalog.Count,
                true);

            AppendNumber(
                json,
                "catalogHealthyCount",
                catalog.HealthyCount,
                true);

            AppendNumber(
                json,
                "catalogDegradedCount",
                catalog.DegradedCount,
                true);

            AppendString(
                json,
                "selectedSlotToken",
                string.IsNullOrWhiteSpace(
                    selectedSlotId)
                    ? string.Empty
                    : Token(
                        "slot|" +
                        selectedSlotId),
                true);

            json.Append("  \"slots\": [\n");

            int slotCount =
                Math.Min(
                    catalog.Count,
                    MaximumCatalogEntries);

            for (int i = 0;
                 i < slotCount;
                 i++)
            {
                SaveSlotCatalogEntry entry =
                    catalog.Entries[i];

                json.Append("    {");
                json.Append(
                    "\"slotToken\":\"" +
                    Escape(
                        Token(
                            "slot|" +
                            entry.SlotId.Value)) +
                    "\",");

                json.Append(
                    "\"health\":\"" +
                    Escape(
                        entry.Health.ToString()) +
                    "\",");

                json.Append(
                    "\"diagnosticCode\":\"" +
                    Escape(
                        entry.DiagnosticCode) +
                    "\",");

                json.Append(
                    "\"currentGenerationToken\":\"" +
                    Escape(
                        string.IsNullOrEmpty(
                            entry.CurrentGenerationId.Value)
                            ? string.Empty
                            : Token(
                                "generation|" +
                                entry.CurrentGenerationId.Value)) +
                    "\",");

                json.Append(
                    "\"participantCount\":" +
                    entry.ParticipantCount +
                    ",");

                json.Append(
                    "\"payloadByteLength\":" +
                    entry.PayloadByteLength);

                json.Append(
                    i + 1 < slotCount
                        ? "},\n"
                        : "}\n");
            }

            json.Append("  ],\n");
            json.Append(
                "  \"slotsTruncated\": " +
                (catalog.Count >
                 MaximumCatalogEntries
                    ? "true"
                    : "false") +
                ",\n");

            json.Append("  \"generations\": [\n");

            int generationCount =
                generations == null
                    ? 0
                    : Math.Min(
                        generations.Count,
                        MaximumGenerationEntries);

            for (int i = 0;
                 i < generationCount;
                 i++)
            {
                SaveGenerationInspectionEntry entry =
                    generations.Entries[i];

                json.Append("    {");
                json.Append(
                    "\"generationToken\":\"" +
                    Escape(
                        Token(
                            "generation|" +
                            entry.GenerationId)) +
                    "\",");

                json.Append(
                    "\"isCurrentHead\":" +
                    (entry.IsCurrentHead
                        ? "true"
                        : "false") +
                    ",");

                json.Append(
                    "\"status\":\"" +
                    Escape(
                        entry.Status.ToString()) +
                    "\",");

                json.Append(
                    "\"sourceManifestVersion\":\"" +
                    Escape(
                        entry.SourceManifestVersion) +
                    "\",");

                json.Append(
                    "\"currentManifestVersion\":\"" +
                    Escape(
                        entry.CurrentManifestVersion) +
                    "\",");

                json.Append(
                    "\"commitState\":\"" +
                    Escape(
                        entry.CommitState) +
                    "\",");

                json.Append(
                    "\"participantCount\":" +
                    entry.ParticipantCount +
                    ",");

                json.Append(
                    "\"payloadByteLength\":" +
                    entry.PayloadByteLength);

                json.Append(
                    i + 1 < generationCount
                        ? "},\n"
                        : "}\n");
            }

            json.Append("  ],\n");
            json.Append(
                "  \"generationsTruncated\": " +
                (generations != null &&
                 generations.Count >
                 MaximumGenerationEntries
                    ? "true"
                    : "false") +
                "\n");

            json.Append("}\n");

            return new EchoSaveSupportSnapshotResult(
                true,
                json.ToString(),
                string.Empty,
                "Built one bounded payload-free redacted Chronicle support snapshot.");
        }

        private static void AppendString(
            StringBuilder builder,
            string name,
            string value,
            bool trailingComma)
        {
            builder.Append(
                "  \"" +
                Escape(name) +
                "\": \"" +
                Escape(value) +
                "\"" +
                (trailingComma ? "," : string.Empty) +
                "\n");
        }

        private static void AppendNumber(
            StringBuilder builder,
            string name,
            long value,
            bool trailingComma)
        {
            builder.Append(
                "  \"" +
                Escape(name) +
                "\": " +
                value +
                (trailingComma ? "," : string.Empty) +
                "\n");
        }

        private static string Token(
            string value)
        {
            using (SHA256 sha =
                   SHA256.Create())
            {
                byte[] hash =
                    sha.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            value ?? string.Empty));

                StringBuilder builder =
                    new StringBuilder(16);

                for (int i = 0;
                     i < 8;
                     i++)
                {
                    builder.Append(
                        hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static string Escape(
            string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        private static EchoSaveSupportSnapshotResult Failure(
            string diagnosticCode,
            string message)
        {
            return new EchoSaveSupportSnapshotResult(
                false,
                string.Empty,
                diagnosticCode,
                message);
        }
    }
}
