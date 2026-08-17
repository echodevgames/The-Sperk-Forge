using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Project-authored Motif asset. Runtime consumers receive only a detached,
    /// completely validated snapshot and never write into this asset.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Echo Dev Games/Echo UI/Motif Definition",
        fileName = "UIMotifDefinition")]
    public sealed class UIMotifDefinition : ScriptableObject
    {
        [SerializeField]
        private string motifId = "motif.default";

        [SerializeField]
        private UIMotifColorToken[] colorTokens =
            new UIMotifColorToken[0];

        [SerializeField]
        private UIMotifSelectableColorsToken[] selectableColorTokens =
            new UIMotifSelectableColorsToken[0];

        [SerializeField]
        private UIMotifSpriteToken[] spriteTokens =
            new UIMotifSpriteToken[0];

        [SerializeField]
        private UIMotifNumberToken[] numberTokens =
            new UIMotifNumberToken[0];

        public UIMotifId MotifId =>
            new UIMotifId(motifId);

        public int ColorTokenCount =>
            Length(colorTokens);

        public int SelectableColorsTokenCount =>
            Length(selectableColorTokens);

        public int SpriteTokenCount =>
            Length(spriteTokens);

        public int NumberTokenCount =>
            Length(numberTokens);

        public int TokenCount =>
            ColorTokenCount +
            SelectableColorsTokenCount +
            SpriteTokenCount +
            NumberTokenCount;

        /// <summary>
        /// Creates a non-persistent, fully initialized definition for tests,
        /// samples, or project code that supplies authored data programmatically.
        /// Input arrays are copied and cannot mutate the returned definition.
        /// </summary>
        public static UIMotifDefinition CreateTransient(
            string motifId,
            UIMotifColorToken[] colorTokens = null,
            UIMotifSelectableColorsToken[] selectableColorTokens = null,
            UIMotifSpriteToken[] spriteTokens = null,
            UIMotifNumberToken[] numberTokens = null)
        {
            UIMotifDefinition definition =
                CreateInstance<UIMotifDefinition>();

            definition.hideFlags = HideFlags.DontSave;
            definition.motifId = new UIMotifId(motifId).Value;
            definition.colorTokens = Copy(colorTokens);
            definition.selectableColorTokens =
                Copy(selectableColorTokens);
            definition.spriteTokens = Copy(spriteTokens);
            definition.numberTokens = Copy(numberTokens);
            return definition;
        }

        public UIMotifDefinitionResult CreateSnapshot(
            int maximumTokenCount) =>
            CreateSnapshot(this, maximumTokenCount);

        public static UIMotifDefinitionResult CreateSnapshot(
            UIMotifDefinition definition,
            int maximumTokenCount)
        {
            if (definition == null)
            {
                return Failure(
                    UIMotifDefinitionStatus.MissingDefinition,
                    message: "Motif definition is required.");
            }

            UIMotifId motifId = definition.MotifId;
            if (maximumTokenCount <= 0)
            {
                return Failure(
                    UIMotifDefinitionStatus.InvalidCapacity,
                    motifId,
                    message: "Maximum token count must be positive.");
            }

            if (!motifId.IsValid)
            {
                return Failure(
                    UIMotifDefinitionStatus.InvalidMotifId,
                    motifId,
                    message: "Motif ID is required.");
            }

            UIMotifColorToken[] colors =
                Copy(definition.colorTokens);
            UIMotifSelectableColorsToken[] selectableColors =
                Copy(definition.selectableColorTokens);
            UIMotifSpriteToken[] sprites =
                Copy(definition.spriteTokens);
            UIMotifNumberToken[] numbers =
                Copy(definition.numberTokens);

            long totalTokenCount =
                (long)colors.Length +
                selectableColors.Length +
                sprites.Length +
                numbers.Length;

            if (totalTokenCount > maximumTokenCount)
            {
                return Failure(
                    UIMotifDefinitionStatus.CapacityExceeded,
                    motifId,
                    tokenCount: ClampToInt(totalTokenCount),
                    message: "Motif token capacity was exceeded.");
            }

            HashSet<UIMotifTokenId> observedTokenIds =
                new HashSet<UIMotifTokenId>();

            for (int i = 0; i < colors.Length; i++)
            {
                UIMotifColorToken token = colors[i];
                if (!TryRegisterToken(
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.Color,
                        observedTokenIds,
                        out UIMotifDefinitionResult failure))
                {
                    return failure;
                }

                if (!IsFinite(token.Value))
                {
                    return Failure(
                        UIMotifDefinitionStatus.InvalidTokenValue,
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.Color,
                        message: "Color token value must be finite.");
                }
            }

            for (int i = 0; i < selectableColors.Length; i++)
            {
                UIMotifSelectableColorsToken token =
                    selectableColors[i];

                if (!TryRegisterToken(
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.SelectableColors,
                        observedTokenIds,
                        out UIMotifDefinitionResult failure))
                {
                    return failure;
                }

                if (!IsValid(token.Value))
                {
                    return Failure(
                        UIMotifDefinitionStatus.InvalidTokenValue,
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.SelectableColors,
                        message:
                            "Selectable color token value must be finite and non-negative.");
                }
            }

            for (int i = 0; i < sprites.Length; i++)
            {
                UIMotifSpriteToken token = sprites[i];
                if (!TryRegisterToken(
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.Sprite,
                        observedTokenIds,
                        out UIMotifDefinitionResult failure))
                {
                    return failure;
                }
            }

            for (int i = 0; i < numbers.Length; i++)
            {
                UIMotifNumberToken token = numbers[i];
                if (!TryRegisterToken(
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.Number,
                        observedTokenIds,
                        out UIMotifDefinitionResult failure))
                {
                    return failure;
                }

                if (!IsFinite(token.Value))
                {
                    return Failure(
                        UIMotifDefinitionStatus.InvalidTokenValue,
                        motifId,
                        token.TokenId,
                        UIMotifTokenKind.Number,
                        message: "Number token value must be finite.");
                }
            }

            UIMotifSnapshot snapshot =
                new UIMotifSnapshot(
                    motifId,
                    colors,
                    selectableColors,
                    sprites,
                    numbers);

            return new UIMotifDefinitionResult(
                UIMotifDefinitionStatus.Ready,
                motifId,
                tokenCount: snapshot.TokenCount,
                snapshot: snapshot);
        }

        private static bool TryRegisterToken(
            UIMotifId motifId,
            UIMotifTokenId tokenId,
            UIMotifTokenKind tokenKind,
            HashSet<UIMotifTokenId> observedTokenIds,
            out UIMotifDefinitionResult failure)
        {
            if (!tokenId.IsValid)
            {
                failure = Failure(
                    UIMotifDefinitionStatus.InvalidTokenId,
                    motifId,
                    tokenId,
                    tokenKind,
                    message: "Motif token ID is required.");
                return false;
            }

            if (!observedTokenIds.Add(tokenId))
            {
                failure = Failure(
                    UIMotifDefinitionStatus.DuplicateTokenId,
                    motifId,
                    tokenId,
                    tokenKind,
                    message:
                        "Motif token IDs must be unique across all token families.");
                return false;
            }

            failure = default;
            return true;
        }

        private static UIMotifDefinitionResult Failure(
            UIMotifDefinitionStatus status,
            UIMotifId motifId = default,
            UIMotifTokenId tokenId = default,
            UIMotifTokenKind tokenKind = UIMotifTokenKind.None,
            int tokenCount = 0,
            string message = "") =>
            new UIMotifDefinitionResult(
                status,
                motifId,
                tokenId,
                tokenKind,
                tokenCount,
                snapshot: null,
                message: message);

        private static bool IsValid(ColorBlock value) =>
            IsFinite(value.normalColor) &&
            IsFinite(value.highlightedColor) &&
            IsFinite(value.pressedColor) &&
            IsFinite(value.selectedColor) &&
            IsFinite(value.disabledColor) &&
            IsFinite(value.colorMultiplier) &&
            value.colorMultiplier >= 0f &&
            IsFinite(value.fadeDuration) &&
            value.fadeDuration >= 0f;

        private static bool IsFinite(Color value) =>
            IsFinite(value.r) &&
            IsFinite(value.g) &&
            IsFinite(value.b) &&
            IsFinite(value.a);

        private static bool IsFinite(float value) =>
            !float.IsNaN(value) &&
            !float.IsInfinity(value);

        private static int Length<T>(T[] values) =>
            values == null
                ? 0
                : values.Length;

        private static T[] Copy<T>(T[] values) =>
            values == null || values.Length == 0
                ? new T[0]
                : (T[])values.Clone();

        private static int ClampToInt(long value) =>
            value > int.MaxValue
                ? int.MaxValue
                : (int)value;
    }
}
