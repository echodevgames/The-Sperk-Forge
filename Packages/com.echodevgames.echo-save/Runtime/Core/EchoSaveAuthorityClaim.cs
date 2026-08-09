using UnityEngine;

namespace EchoDevGames.EchoSave
{
    internal static class EchoSaveAuthorityClaim
    {
        private static EchoSaveRoot current;

        internal static EchoSaveRoot Current =>
            current;

        internal static bool TryClaim(
            EchoSaveRoot candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            if (current == null)
            {
                current = candidate;
                return true;
            }

            return ReferenceEquals(
                current,
                candidate);
        }

        internal static void Release(
            EchoSaveRoot candidate)
        {
            if (ReferenceEquals(
                    current,
                    candidate))
            {
                current = null;
            }
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAtSubsystemRegistration()
        {
            current = null;
        }

        internal static void ResetForTesting()
        {
            current = null;
        }
    }
}
