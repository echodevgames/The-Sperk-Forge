using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public enum UIModalBackBehavior
    {
        Disabled = 0,
        CompleteWithResultId = 1
    }

    [Serializable]
    public sealed class UIModalBackPolicy
    {
        [SerializeField]
        private UIModalBackBehavior behavior =
            UIModalBackBehavior.Disabled;

        [SerializeField]
        private string resultId = string.Empty;

        public UIModalBackPolicy()
        {
        }

        public UIModalBackPolicy(
            UIModalBackBehavior behavior,
            string resultId = "")
        {
            this.behavior = behavior;
            this.resultId = resultId ?? string.Empty;
        }

        public UIModalBackBehavior Behavior =>
            behavior;

        public UIModalResultId ResultId =>
            new UIModalResultId(resultId);

        internal UIModalBackPolicy Snapshot() =>
            new UIModalBackPolicy(
                behavior,
                resultId);
    }
}
