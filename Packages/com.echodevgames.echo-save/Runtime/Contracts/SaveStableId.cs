using System;

namespace EchoDevGames.EchoSave
{
    internal static class SaveStableId
    {
        internal static string Normalize(
            string value,
            string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(
                    parameterName);
            }

            string normalized =
                value.Trim().ToLowerInvariant();

            if (normalized.Length == 0)
            {
                throw new ArgumentException(
                    "Stable save-provider IDs cannot be empty.",
                    parameterName);
            }

            if (normalized == "." ||
                normalized == "..")
            {
                throw new ArgumentException(
                    "Stable save-provider IDs cannot be traversal segments.",
                    parameterName);
            }

            for (int i = 0; i < normalized.Length; i++)
            {
                char character = normalized[i];

                if (char.IsControl(character) ||
                    character == '/' ||
                    character == '\\' ||
                    character == ':')
                {
                    throw new ArgumentException(
                        "Stable save-provider IDs contain an unsupported character.",
                        parameterName);
                }
            }

            return normalized;
        }
    }
}
