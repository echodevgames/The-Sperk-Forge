//----- EchoLaunchRoot.cs START -----

using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Scene-facing authority root for First Light.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EchoLaunchRoot : MonoBehaviour
    {
        internal const string DuplicateDiagnosticCode =
            "ELAUNCH-ROOT-001";

        /// <summary>
        /// Returns the currently authoritative First Light root.
        /// </summary>
        public static EchoLaunchRoot Current =>
            LaunchAuthorityClaim.Current as EchoLaunchRoot;

        /// <summary>
        /// Returns true when this component currently owns launch authority.
        /// </summary>
        public bool IsAuthoritative =>
            ReferenceEquals(Current, this);

        /// <summary>
        /// Returns true when this component was rejected as a duplicate.
        /// </summary>
        public bool WasRejectedAsDuplicate { get; private set; }

        private void Awake()
        {
            if (LaunchAuthorityClaim.TryClaim(this))
            {
                WasRejectedAsDuplicate = false;
                return;
            }

            WasRejectedAsDuplicate = true;

            // Disable before performing any future startup behavior.
            enabled = false;

            Debug.LogWarning(
                $"[{DuplicateDiagnosticCode}] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.",
                this);
        }

        private void OnDestroy()
        {
            LaunchAuthorityClaim.Release(this);
        }
    }
}

//----- EchoLaunchRoot.cs END -----