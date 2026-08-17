using System;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    public interface IUIMotifNumberReceiver
    {
        bool TryApplyMotifNumber(
            UIMotifTokenId tokenId,
            float value);
    }

    [Serializable]
    public struct UIMotifGraphicColorBinding
    {
        [SerializeField] private Graphic target;
        [SerializeField] private string tokenId;
        [SerializeField] private UIMotifBindingMode mode;

        public UIMotifGraphicColorBinding(
            Graphic target,
            string tokenId,
            UIMotifBindingMode mode = UIMotifBindingMode.UseMotif)
        {
            this.target = target;
            this.tokenId = new UIMotifTokenId(tokenId).Value;
            this.mode = mode;
        }

        public Graphic Target => target;
        public UIMotifTokenId TokenId => new UIMotifTokenId(tokenId);
        public UIMotifBindingMode Mode => mode;
    }

    [Serializable]
    public struct UIMotifSelectableColorsBinding
    {
        [SerializeField] private Selectable target;
        [SerializeField] private string tokenId;
        [SerializeField] private UIMotifBindingMode mode;

        public UIMotifSelectableColorsBinding(
            Selectable target,
            string tokenId,
            UIMotifBindingMode mode = UIMotifBindingMode.UseMotif)
        {
            this.target = target;
            this.tokenId = new UIMotifTokenId(tokenId).Value;
            this.mode = mode;
        }

        public Selectable Target => target;
        public UIMotifTokenId TokenId => new UIMotifTokenId(tokenId);
        public UIMotifBindingMode Mode => mode;
    }

    [Serializable]
    public struct UIMotifImageSpriteBinding
    {
        [SerializeField] private Image target;
        [SerializeField] private string tokenId;
        [SerializeField] private UIMotifBindingMode mode;

        public UIMotifImageSpriteBinding(
            Image target,
            string tokenId,
            UIMotifBindingMode mode = UIMotifBindingMode.UseMotif)
        {
            this.target = target;
            this.tokenId = new UIMotifTokenId(tokenId).Value;
            this.mode = mode;
        }

        public Image Target => target;
        public UIMotifTokenId TokenId => new UIMotifTokenId(tokenId);
        public UIMotifBindingMode Mode => mode;
    }

    [Serializable]
    public struct UIMotifNumberBinding
    {
        [SerializeField] private UnityEngine.Object receiver;
        [SerializeField] private string tokenId;
        [SerializeField] private UIMotifBindingMode mode;

        public UIMotifNumberBinding(
            UnityEngine.Object receiver,
            string tokenId,
            UIMotifBindingMode mode = UIMotifBindingMode.UseMotif)
        {
            this.receiver = receiver;
            this.tokenId = new UIMotifTokenId(tokenId).Value;
            this.mode = mode;
        }

        public UnityEngine.Object Receiver => receiver;
        public UIMotifTokenId TokenId => new UIMotifTokenId(tokenId);
        public UIMotifBindingMode Mode => mode;
    }

    /// <summary>
    /// Explicit reusable uGUI Motif target. It applies only authored bindings
    /// and never scans its hierarchy or mutates the source Motif definition.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIMotifBindingTarget : MonoBehaviour, IUIMotifTarget
    {
        [SerializeField] private UIMotifGraphicColorBinding[] graphicColors =
            Array.Empty<UIMotifGraphicColorBinding>();
        [SerializeField] private UIMotifSelectableColorsBinding[] selectableColors =
            Array.Empty<UIMotifSelectableColorsBinding>();
        [SerializeField] private UIMotifImageSpriteBinding[] imageSprites =
            Array.Empty<UIMotifImageSpriteBinding>();
        [SerializeField] private UIMotifNumberBinding[] numbers =
            Array.Empty<UIMotifNumberBinding>();

        public int BindingCount =>
            Length(graphicColors) +
            Length(selectableColors) +
            Length(imageSprites) +
            Length(numbers);

        public void Configure(
            UIMotifGraphicColorBinding[] graphicColorBindings = null,
            UIMotifSelectableColorsBinding[] selectableColorBindings = null,
            UIMotifImageSpriteBinding[] imageSpriteBindings = null,
            UIMotifNumberBinding[] numberBindings = null)
        {
            graphicColors = Copy(graphicColorBindings);
            selectableColors = Copy(selectableColorBindings);
            imageSprites = Copy(imageSpriteBindings);
            numbers = Copy(numberBindings);
        }

        public UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return new UIMotifTargetApplyResult(
                    UIMotifTargetApplyStatus.Failed,
                    failedBindingCount: BindingCount,
                    message: "Resolved Motif snapshot is required.");
            }

            int applied = 0;
            int keptLocal = 0;
            int failed = 0;

            for (int i = 0; i < Length(graphicColors); i++)
            {
                UIMotifGraphicColorBinding binding = graphicColors[i];
                if (KeepLocal(binding.Mode, ref keptLocal)) continue;
                if (binding.Target == null ||
                    !snapshot.TryGetColor(binding.TokenId, out Color value))
                {
                    failed++;
                    continue;
                }

                binding.Target.color = value;
                applied++;
            }

            for (int i = 0; i < Length(selectableColors); i++)
            {
                UIMotifSelectableColorsBinding binding = selectableColors[i];
                if (KeepLocal(binding.Mode, ref keptLocal)) continue;
                if (binding.Target == null ||
                    !snapshot.TryGetSelectableColors(binding.TokenId, out ColorBlock value))
                {
                    failed++;
                    continue;
                }

                binding.Target.colors = value;
                applied++;
            }

            for (int i = 0; i < Length(imageSprites); i++)
            {
                UIMotifImageSpriteBinding binding = imageSprites[i];
                if (KeepLocal(binding.Mode, ref keptLocal)) continue;
                if (binding.Target == null ||
                    !snapshot.TryGetSprite(binding.TokenId, out Sprite value))
                {
                    failed++;
                    continue;
                }

                binding.Target.sprite = value;
                applied++;
            }

            for (int i = 0; i < Length(numbers); i++)
            {
                UIMotifNumberBinding binding = numbers[i];
                if (KeepLocal(binding.Mode, ref keptLocal)) continue;
                if (binding.Receiver == null ||
                    !(binding.Receiver is IUIMotifNumberReceiver receiver) ||
                    !snapshot.TryGetNumber(binding.TokenId, out float value) ||
                    !receiver.TryApplyMotifNumber(binding.TokenId, value))
                {
                    failed++;
                    continue;
                }

                applied++;
            }

            UIMotifTargetApplyStatus status = failed > 0
                ? applied + keptLocal > 0
                    ? UIMotifTargetApplyStatus.Partial
                    : UIMotifTargetApplyStatus.Failed
                : keptLocal > 0
                    ? UIMotifTargetApplyStatus.Partial
                    : UIMotifTargetApplyStatus.Applied;

            return new UIMotifTargetApplyResult(
                status,
                applied,
                keptLocal,
                failed);
        }

        private static bool KeepLocal(
            UIMotifBindingMode mode,
            ref int keptLocal)
        {
            if (mode != UIMotifBindingMode.KeepLocal)
                return false;

            keptLocal++;
            return true;
        }

        private static int Length<T>(T[] values) =>
            values == null ? 0 : values.Length;

        private static T[] Copy<T>(T[] values) =>
            values == null || values.Length == 0
                ? Array.Empty<T>()
                : (T[])values.Clone();
    }
}
