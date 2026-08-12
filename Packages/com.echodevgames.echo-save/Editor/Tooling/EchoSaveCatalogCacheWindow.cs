
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoSave.Editor
{
    /// <summary>
    /// Explicit Preview/Rebuild surface for package-owned derived
    /// catalog.cache.json. Preview never creates a missing production root.
    /// </summary>
    public sealed class EchoSaveCatalogCacheWindow :
        EditorWindow
    {
        private EchoSaveConfiguration configuration;
        private SaveCatalogCachePreview preview;
        private SaveCatalogCacheRebuildResult rebuild;
        private string rootPath = string.Empty;

        [MenuItem(
            "Tools/Sperk’s Forge/The Chronicle/Catalog Cache",
            priority = 324)]
        private static void Open()
        {
            GetWindow<EchoSaveCatalogCacheWindow>(
                "Chronicle Catalog Cache");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "The Chronicle — Derived Catalog Cache",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "catalog.cache.json is derived acceleration only. Heads/manifests remain authoritative. Preview is zero-write; Rebuild may replace only the cache file.",
                MessageType.Info);

            configuration =
                (EchoSaveConfiguration)EditorGUILayout.ObjectField(
                    "Configuration",
                    configuration,
                    typeof(EchoSaveConfiguration),
                    false);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Preview Cache"))
            {
                PreviewCache();
            }

            EditorGUI.BeginDisabledGroup(
                preview == null ||
                !preview.CanRebuild);

            if (GUILayout.Button(
                    "Rebuild Catalog Cache"))
            {
                RebuildCache();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(
                    rootPath))
            {
                EditorGUILayout.LabelField(
                    "Production Root",
                    rootPath);
            }

            DrawPreview();
            DrawRebuild();
        }

        private void PreviewCache()
        {
            rebuild =
                null;

            if (!TryOpenCoordinator(
                    out SaveCatalogCacheCoordinator coordinator,
                    out LocalFileSaveStorageBackend backend,
                    out SaveCatalogCachePreview failure))
            {
                preview =
                    failure;
                return;
            }

            try
            {
                preview =
                    coordinator.Preview();
            }
            finally
            {
                backend.Shutdown();
            }
        }

        private void RebuildCache()
        {
            if (!TryOpenCoordinator(
                    out SaveCatalogCacheCoordinator coordinator,
                    out LocalFileSaveStorageBackend backend,
                    out SaveCatalogCachePreview failure))
            {
                preview =
                    failure;
                rebuild =
                    null;
                return;
            }

            try
            {
                rebuild =
                    coordinator.Rebuild();

                preview =
                    coordinator.Preview();
            }
            finally
            {
                backend.Shutdown();
            }
        }

        private bool TryOpenCoordinator(
            out SaveCatalogCacheCoordinator coordinator,
            out LocalFileSaveStorageBackend backend,
            out SaveCatalogCachePreview failure)
        {
            coordinator =
                null;

            backend =
                null;

            failure =
                null;

            rootPath =
                string.Empty;

            if (configuration == null)
            {
                failure =
                    Failure(
                        "Assign one Chronicle configuration.");
                return false;
            }

            if (!configuration.TryResolveRuntimePolicy(
                    out EchoSaveRuntimePolicy policy,
                    out string policyMessage))
            {
                failure =
                    Failure(
                        policyMessage);
                return false;
            }

            SaveStorageResult root =
                SaveStorageRootResolver.TryResolveProductionRoot(
                    configuration,
                    Application.persistentDataPath,
                    out rootPath);

            if (!root.Succeeded)
            {
                failure =
                    Failure(
                        root.Message);
                return false;
            }

            if (!Directory.Exists(
                    rootPath))
            {
                failure =
                    new SaveCatalogCachePreview(
                        SaveCatalogCacheState.Missing,
                        string.Empty,
                        "The production Chronicle root is absent. Cache Preview performed zero writes and Rebuild remains disabled.",
                        SaveSlotCatalogSnapshot.Empty,
                        0,
                        string.Empty,
                        string.Empty,
                        false);

                return false;
            }

            backend =
                new LocalFileSaveStorageBackend(
                    rootPath);

            SaveStorageResult initialized =
                backend.InitializeReadOnly();

            if (!initialized.Succeeded)
            {
                failure =
                    Failure(
                        initialized.Message);

                backend =
                    null;

                return false;
            }

            coordinator =
                new SaveCatalogCacheCoordinator(
                    backend,
                    new UnityJsonSaveSerializer(),
                    policy.Limits.CatalogScanLimit);

            return true;
        }

        private void DrawPreview()
        {
            if (preview == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Preview",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "State",
                preview.State.ToString());

            EditorGUILayout.LabelField(
                "Durable Slots",
                preview.DurableSnapshot.Count.ToString());

            EditorGUILayout.LabelField(
                "Cached Entries",
                preview.CachedEntryCount.ToString());

            EditorGUILayout.LabelField(
                "Durable Fingerprint",
                preview.DurableFingerprint);

            EditorGUILayout.LabelField(
                "Cache Fingerprint",
                preview.CacheFingerprint);

            EditorGUILayout.HelpBox(
                preview.Message,
                preview.State ==
                    SaveCatalogCacheState.Valid
                    ? MessageType.Info
                    : MessageType.Warning);
        }

        private void DrawRebuild()
        {
            if (rebuild == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Last Rebuild",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "Succeeded",
                rebuild.Succeeded
                    ? "Yes"
                    : "No");

            EditorGUILayout.LabelField(
                "State",
                rebuild.State.ToString());

            EditorGUILayout.LabelField(
                "Fingerprint",
                rebuild.Fingerprint);

            EditorGUILayout.HelpBox(
                rebuild.Message,
                rebuild.Succeeded
                    ? MessageType.Info
                    : MessageType.Error);
        }

        private static SaveCatalogCachePreview Failure(
            string message) =>
            new SaveCatalogCachePreview(
                SaveCatalogCacheState.DurableCatalogUnavailable,
                "ECHOSAVE-CACHE-EDITOR",
                message,
                SaveSlotCatalogSnapshot.Empty,
                0,
                string.Empty,
                string.Empty,
                false);
    }
}
