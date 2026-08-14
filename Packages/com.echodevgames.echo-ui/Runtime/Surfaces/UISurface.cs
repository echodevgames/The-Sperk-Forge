using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Declares one project-authored UI surface using a stable ID and behavioral role.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UISurface : MonoBehaviour
    {
        [SerializeField]
        private string surfaceId = "surface";

        [SerializeField]
        private string displayLabel = string.Empty;

        [SerializeField]
        private UISurfaceRole role =
            UISurfaceRole.Screen;

        [SerializeField]
        private string navigationScopeId =
            "default";

        [SerializeField]
        private bool startVisible;

        [Header("External Context")]
        [SerializeField]
        private bool allowExternalContext = true;

        [SerializeField]
        private CanvasGroup interactionGroup;

        [SerializeField]
        private List<UISurfaceContextRule> contextRules =
            new List<UISurfaceContextRule>();

        [Header("Selection")]
        [SerializeField]
        private UISurfaceSelectionPolicy selectionPolicy =
            new UISurfaceSelectionPolicy();

        private UISurfaceRuntimeOverride runtimeOverride =
            UISurfaceRuntimeOverride.None;

        public string SurfaceId =>
            surfaceId == null
                ? string.Empty
                : surfaceId.Trim();

        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(displayLabel)
                ? SurfaceId
                : displayLabel.Trim();

        public UISurfaceRole Role =>
            role;

        public string NavigationScopeId =>
            navigationScopeId == null
                ? string.Empty
                : navigationScopeId.Trim();

        public bool StartVisible =>
            startVisible;

        public bool IsVisible =>
            gameObject.activeSelf;

        public bool AllowExternalContext =>
            allowExternalContext;

        public IReadOnlyList<UISurfaceContextRule> ContextRules =>
            contextRules;

        public UISurfaceSelectionPolicy SelectionPolicy =>
            selectionPolicy;

        public UISurfaceRuntimeOverride RuntimeOverride =>
            runtimeOverride;

        public bool IsInteractable
        {
            get
            {
                CanvasGroup group =
                    ResolveInteractionGroup();
                return group == null ||
                    group.interactable;
            }
        }

        internal void SetVisible(
            bool visible)
        {
            if (gameObject.activeSelf != visible)
            {
                gameObject.SetActive(visible);
            }
        }

        internal bool SetInteractable(
            bool interactable)
        {
            CanvasGroup group =
                ResolveInteractionGroup();
            if (group == null)
            {
                return false;
            }

            group.interactable =
                interactable;
            return true;
        }

        internal UISurfaceContextResponse ResolveContextResponse(
            UIContextState contextState)
        {
            return UISurfaceContextResolver.Resolve(
                allowExternalContext
                    ? contextRules
                    : null,
                allowExternalContext
                    ? contextState
                    : null,
                runtimeOverride);
        }

        internal void SetRuntimeOverride(
            UISurfaceRuntimeOverride value)
        {
            runtimeOverride = value;
        }

        internal void ClearRuntimeOverride()
        {
            runtimeOverride =
                UISurfaceRuntimeOverride.None;
        }

        private CanvasGroup ResolveInteractionGroup()
        {
            return interactionGroup != null
                ? interactionGroup
                : GetComponent<CanvasGroup>();
        }
    }
}
