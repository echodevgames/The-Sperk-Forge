//----- LaunchAuthorityClaim.cs START -----

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Owns the process-wide claim for the active First Light authority.
    /// </summary>
    internal static class LaunchAuthorityClaim
    {
        private static Object current;

        /// <summary>
        /// Returns the current live authority object, or null when none exists.
        /// </summary>
        internal static Object Current =>
            current == null ? null : current;

        /// <summary>
        /// Attempts to claim launch authority for the supplied object.
        /// </summary>
        internal static bool TryClaim(Object candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (current == null)
            {
                current = candidate;
                return true;
            }

            return ReferenceEquals(current, candidate);
        }

        /// <summary>
        /// Releases authority only when the supplied object is the current owner.
        /// </summary>
        internal static void Release(Object owner)
        {
            if (ReferenceEquals(current, owner))
            {
                current = null;
            }
        }

        /// <summary>
        /// Clears stale static state whenever Unity registers runtime subsystems.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Reset()
        {
            current = null;
        }
    }
}

//----- LaunchAuthorityClaim.cs END -----