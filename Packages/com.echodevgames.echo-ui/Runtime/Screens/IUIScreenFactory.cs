using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public interface IUIScreenFactory
    {
        bool TryCreate(
            UIScreenDefinition definition,
            UILayerHost layerHost,
            out UISurface surface,
            out string error);

        void Release(
            UISurface surface);
    }

    /// <summary>
    /// Package-local prefab factory used only for explicit RootOwned definitions.
    /// </summary>
    internal sealed class DefaultUIScreenPrefabFactory : IUIScreenFactory
    {
        public bool TryCreate(
            UIScreenDefinition definition,
            UILayerHost layerHost,
            out UISurface surface,
            out string error)
        {
            surface = null;
            error = string.Empty;

            if (definition == null ||
                definition.RootOwnedPrefab == null)
            {
                error = "RootOwned Screen definition has no prefab.";
                return false;
            }

            if (layerHost == null ||
                layerHost.ContentRoot == null)
            {
                error = "RootOwned Screen target layer host is unavailable.";
                return false;
            }

            GameObject instance =
                Object.Instantiate(
                    definition.RootOwnedPrefab,
                    layerHost.ContentRoot,
                    false);

            surface =
                instance.GetComponent<UISurface>();

            if (surface != null)
            {
                return true;
            }

            DestroyObject(instance);
            error = "RootOwned Screen prefab must contain a UISurface on its root.";
            return false;
        }

        public void Release(
            UISurface surface)
        {
            if (surface == null)
            {
                return;
            }

            DestroyObject(
                surface.gameObject);
        }

        private static void DestroyObject(
            Object value)
        {
            if (value == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(value);
            }
            else
            {
                Object.DestroyImmediate(value);
            }
        }
    }
}
