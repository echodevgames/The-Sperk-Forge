//----- SplashPresentationSettings.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores optional sequence-owned splash presentation intent.
    ///
    /// Missing settings on a schema-1 SplashSequence resolve through
    /// LegacyDefaults without rewriting the project-owned asset.
    /// </summary>
    [Serializable]
    public sealed class SplashPresentationSettings
    {
        private static readonly SplashPresentationSettings legacyDefaults =
            new SplashPresentationSettings(
                SplashPresentationMode.SplashAndStatus,
                Color.black,
                true);

        [SerializeField]
        private SplashPresentationMode presentationMode =
            SplashPresentationMode.SplashOnly;

        [SerializeField]
        private Color backgroundColor =
            Color.black;

        [SerializeField]
        private bool allowUserAdvance =
            true;

        /// <summary>
        /// Creates explicit new-authoring defaults.
        /// </summary>
        public SplashPresentationSettings()
        {
        }

        internal SplashPresentationSettings(
            SplashPresentationMode authoredPresentationMode,
            Color authoredBackgroundColor,
            bool authoredAllowUserAdvance)
        {
            if (!Enum.IsDefined(
                    typeof(SplashPresentationMode),
                    authoredPresentationMode))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredPresentationMode));
            }

            if (!IsFiniteColor(
                    authoredBackgroundColor))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredBackgroundColor));
            }

            presentationMode =
                authoredPresentationMode;

            backgroundColor =
                authoredBackgroundColor;

            allowUserAdvance =
                authoredAllowUserAdvance;
        }

        public SplashPresentationMode PresentationMode =>
            presentationMode;

        public Color BackgroundColor =>
            backgroundColor;

        public bool AllowUserAdvance =>
            allowUserAdvance;

        internal static SplashPresentationSettings LegacyDefaults =>
            legacyDefaults;

        internal bool HasValidDefinition =>
            Enum.IsDefined(
                typeof(SplashPresentationMode),
                presentationMode) &&
            IsFiniteColor(
                backgroundColor);

        private static bool IsFiniteColor(
            Color value)
        {
            return
                IsFinite(value.r) &&
                IsFinite(value.g) &&
                IsFinite(value.b) &&
                IsFinite(value.a);
        }

        private static bool IsFinite(
            float value)
        {
            return
                !float.IsNaN(value) &&
                !float.IsInfinity(value);
        }
    }
}

//----- SplashPresentationSettings.cs END -----
