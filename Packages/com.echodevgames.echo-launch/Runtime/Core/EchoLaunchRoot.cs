//----- EchoLaunchRoot.cs START -----

using System;
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

        [Header("Launch")]
        [SerializeField]
        private LaunchMode launchMode =
            LaunchMode.CanonicalBoot;

        private LaunchSession session;

        /// <summary>
        /// Returns the currently authoritative First Light root.
        /// </summary>
        public static EchoLaunchRoot Current =>
            LaunchAuthorityClaim.Current
                as EchoLaunchRoot;

        /// <summary>
        /// Returns true when this component currently owns launch authority.
        /// </summary>
        public bool IsAuthoritative =>
            ReferenceEquals(Current, this);

        /// <summary>
        /// Returns true when this component was rejected as a duplicate.
        /// </summary>
        public bool WasRejectedAsDuplicate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current authoritative launch state.
        /// </summary>
        public LaunchStatus State =>
            IsAuthoritative &&
            session != null
                ? session.State
                : LaunchStatus.None;

        /// <summary>
        /// Gets the latest authoritative launch progress snapshot.
        /// </summary>
        public LaunchProgressSnapshot Progress =>
            IsAuthoritative &&
            session != null
                ? session.Progress
                : LaunchProgressSnapshot.Empty;

        private void Awake()
        {
            if (LaunchAuthorityClaim.TryClaim(this))
            {
                WasRejectedAsDuplicate = false;

                try
                {
                    session =
                        new LaunchSession(launchMode);
                }
                catch
                {
                    LaunchAuthorityClaim.Release(this);
                    throw;
                }

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
            session = null;
            LaunchAuthorityClaim.Release(this);
        }

        /// <summary>
        /// Replaces the authoritative root's current progress snapshot.
        /// </summary>
        internal void PublishProgress(
            LaunchProgressSnapshot snapshot)
        {
            if (!IsAuthoritative)
            {
                throw new InvalidOperationException(
                    "Only the authoritative EchoLaunchRoot may publish launch progress.");
            }

            if (session == null)
            {
                throw new InvalidOperationException(
                    "The authoritative EchoLaunchRoot does not have an active launch session.");
            }

            session.Publish(snapshot);
        }
    }
}

//----- EchoLaunchRoot.cs END -----