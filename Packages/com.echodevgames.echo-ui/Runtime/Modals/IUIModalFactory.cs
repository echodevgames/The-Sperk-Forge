using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public interface IUIModalFactory
    {
        bool TryCreate(
            UIModalDefinition definition,
            UILayerHost layerHost,
            out UISurface surface,
            out string error);

        void Release(
            UISurface surface);
    }

    internal sealed class DefaultUIModalPrefabFactory : IUIModalFactory
    {
        public bool TryCreate(
            UIModalDefinition definition,
            UILayerHost layerHost,
            out UISurface surface,
            out string error)
        {
            surface = null;
            error = string.Empty;

            if (definition == null ||
                definition.RootOwnedPrefab == null)
            {
                error = "RootOwned Modal definition has no prefab.";
                return false;
            }

            if (layerHost == null ||
                layerHost.ContentRoot == null)
            {
                error = "RootOwned Modal target layer host is unavailable.";
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

            DestroyObject(
                instance);

            error =
                "RootOwned Modal prefab must contain a UISurface on its root.";

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
                Object.Destroy(
                    value);
            }
            else
            {
                Object.DestroyImmediate(
                    value);
            }
        }
    }
}
