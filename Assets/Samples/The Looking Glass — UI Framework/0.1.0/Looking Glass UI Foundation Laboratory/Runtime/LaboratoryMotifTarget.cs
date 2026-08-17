using System;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Sample-owned explicit Motif target used only by the Looking Glass Laboratory.
    /// It demonstrates uGUI color, Selectable color-state, sprite, number, and
    /// KeepLocal behavior without hierarchy scanning or authored-asset mutation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LaboratoryMotifTarget : MonoBehaviour, IUIMotifTarget
    {
        [SerializeField] private Image surfaceGraphic;
        [SerializeField] private Button actionButton;
        [SerializeField] private Image badgeImage;
        [SerializeField] private CanvasGroup opacityGroup;
        [SerializeField] private string surfaceColorTokenId = "color.surface";
        [SerializeField] private string selectableColorsTokenId = "selectable.action";
        [SerializeField] private string badgeSpriteTokenId = "sprite.badge";
        [SerializeField] private string opacityTokenId = "number.opacity";
        [SerializeField] private UIMotifBindingMode surfaceColorMode =
            UIMotifBindingMode.UseMotif;

        private bool throwOnApply;

        public int ApplicationCount { get; private set; }
        public UIMotifId LastMotifId { get; private set; }
        public UIMotifTargetApplyResult LastResult { get; private set; }
        public Color SurfaceColor => surfaceGraphic == null ? Color.clear : surfaceGraphic.color;
        public ColorBlock SelectableColors =>
            actionButton == null ? ColorBlock.defaultColorBlock : actionButton.colors;
        public Sprite BadgeSprite => badgeImage == null ? null : badgeImage.sprite;
        public float Opacity => opacityGroup == null ? 1f : opacityGroup.alpha;
        public UIMotifBindingMode SurfaceColorMode => surfaceColorMode;

        public void Configure(
            Image surface,
            Button action,
            Image badge,
            CanvasGroup opacity,
            UIMotifBindingMode colorMode,
            Color localSurfaceColor,
            string colorTokenId = "color.surface")
        {
            surfaceGraphic = surface;
            actionButton = action;
            badgeImage = badge;
            opacityGroup = opacity;
            surfaceColorMode = colorMode;
            surfaceColorTokenId = new UIMotifTokenId(colorTokenId).Value;
            selectableColorsTokenId = "selectable.action";
            badgeSpriteTokenId = "sprite.badge";
            opacityTokenId = "number.opacity";

            if (surfaceGraphic != null)
                surfaceGraphic.color = localSurfaceColor;
        }

        public void SetThrowOnApply(bool value) =>
            throwOnApply = value;

        public UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot)
        {
            ApplicationCount++;
            if (throwOnApply)
                throw new InvalidOperationException(
                    "Laboratory-injected Motif target failure.");

            if (snapshot == null)
            {
                LastResult = new UIMotifTargetApplyResult(
                    UIMotifTargetApplyStatus.Failed,
                    failedBindingCount: 4,
                    message: "Resolved Motif snapshot is required.");
                return LastResult;
            }

            LastMotifId = snapshot.MotifId;
            int applied = 0;
            int keptLocal = 0;
            int failed = 0;

            if (surfaceColorMode == UIMotifBindingMode.KeepLocal)
            {
                keptLocal++;
            }
            else if (surfaceGraphic != null &&
                     snapshot.TryGetColor(
                         new UIMotifTokenId(surfaceColorTokenId),
                         out Color surfaceColor))
            {
                surfaceGraphic.color = surfaceColor;
                applied++;
            }
            else
            {
                failed++;
            }

            if (actionButton != null &&
                snapshot.TryGetSelectableColors(
                    new UIMotifTokenId(selectableColorsTokenId),
                    out ColorBlock selectableColors))
            {
                actionButton.colors = selectableColors;
                applied++;
            }
            else
            {
                failed++;
            }

            if (badgeImage != null &&
                snapshot.TryGetSprite(
                    new UIMotifTokenId(badgeSpriteTokenId),
                    out Sprite badgeSprite))
            {
                badgeImage.sprite = badgeSprite;
                applied++;
            }
            else
            {
                failed++;
            }

            if (opacityGroup != null &&
                snapshot.TryGetNumber(
                    new UIMotifTokenId(opacityTokenId),
                    out float opacity))
            {
                opacityGroup.alpha = Mathf.Clamp01(opacity);
                applied++;
            }
            else
            {
                failed++;
            }

            UIMotifTargetApplyStatus status = failed > 0
                ? applied + keptLocal > 0
                    ? UIMotifTargetApplyStatus.Partial
                    : UIMotifTargetApplyStatus.Failed
                : keptLocal > 0
                    ? UIMotifTargetApplyStatus.Partial
                    : UIMotifTargetApplyStatus.Applied;

            LastResult = new UIMotifTargetApplyResult(
                status,
                applied,
                keptLocal,
                failed);
            return LastResult;
        }
    }
}
