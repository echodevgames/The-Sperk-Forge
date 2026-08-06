//----- DirectSceneConfiguration.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Project-owned immutable authoring data for one direct-scene development
    /// entry point.
    ///
    /// Runtime code reads this asset but never repairs, migrates, or rewrites
    /// its serialized values.
    /// </summary>
    [CreateAssetMenu(
        fileName = "DirectSceneConfiguration",
        menuName =
            "EchoDevGames/First Light/Direct Scene Configuration",
        order = 3)]
    public sealed class DirectSceneConfiguration :
        ScriptableObject
    {
        public const int CurrentSchemaVersion = 1;
        private const int CanonicalIdLength = 32;

        [SerializeField]
        [HideInInspector]
        private string directSceneConfigurationId =
            Guid.NewGuid().ToString("N");

        [SerializeField]
        [HideInInspector]
        private int schemaVersion =
            CurrentSchemaVersion;

        [SerializeField]
        private EchoLaunchRoot rootPrefab;

        [SerializeField]
        private DirectSceneEntryPolicy entryPolicy =
            DirectSceneEntryPolicy.EditorOnly;

        public string DirectSceneConfigurationId =>
            directSceneConfigurationId ?? string.Empty;

        public int SchemaVersion =>
            schemaVersion;

        public EchoLaunchRoot RootPrefab =>
            rootPrefab;

        public DirectSceneEntryPolicy EntryPolicy =>
            entryPolicy;

        internal bool HasValidIdentity =>
            IsCanonicalDirectSceneConfigurationId(
                directSceneConfigurationId);

        internal bool HasSupportedSchema =>
            schemaVersion == CurrentSchemaVersion;

        internal bool HasSupportedPolicy =>
            Enum.IsDefined(
                typeof(DirectSceneEntryPolicy),
                entryPolicy);

        internal static bool
            IsCanonicalDirectSceneConfigurationId(
                string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length != CanonicalIdLength)
            {
                return false;
            }

            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];

                bool isNumber =
                    character >= '0' &&
                    character <= '9';

                bool isLowercaseHexLetter =
                    character >= 'a' &&
                    character <= 'f';

                if (!isNumber && !isLowercaseHexLetter)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

//----- DirectSceneConfiguration.cs END -----
