using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// One designer-authored context rule. List order defines priority.
    /// </summary>
    [Serializable]
    public sealed class UISurfaceContextRule
    {
        [SerializeField]
        private string contextId = string.Empty;

        [SerializeField]
        private UISurfaceContextResponse response;

        public UISurfaceContextRule()
        {
        }

        public UISurfaceContextRule(
            string contextId,
            UISurfaceContextResponse response)
        {
            this.contextId = UIContextId.Normalize(contextId);
            this.response = response;
        }

        public UIContextId ContextId =>
            new UIContextId(contextId);

        public UISurfaceContextResponse Response =>
            response;

        public bool IsValid =>
            ContextId.IsValid;
    }
}
