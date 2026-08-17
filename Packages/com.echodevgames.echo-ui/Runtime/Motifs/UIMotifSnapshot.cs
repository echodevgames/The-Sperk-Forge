using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Detached read-only runtime lookup for one completely validated Motif.
    /// </summary>
    public sealed class UIMotifSnapshot
    {
        private readonly Dictionary<UIMotifTokenId, Color> colors;
        private readonly Dictionary<UIMotifTokenId, ColorBlock> selectableColors;
        private readonly Dictionary<UIMotifTokenId, Sprite> sprites;
        private readonly Dictionary<UIMotifTokenId, float> numbers;

        internal UIMotifSnapshot(
            UIMotifId motifId,
            UIMotifColorToken[] colorTokens,
            UIMotifSelectableColorsToken[] selectableColorTokens,
            UIMotifSpriteToken[] spriteTokens,
            UIMotifNumberToken[] numberTokens)
        {
            MotifId = motifId;
            colors =
                new Dictionary<UIMotifTokenId, Color>(
                    colorTokens.Length);
            selectableColors =
                new Dictionary<UIMotifTokenId, ColorBlock>(
                    selectableColorTokens.Length);
            sprites =
                new Dictionary<UIMotifTokenId, Sprite>(
                    spriteTokens.Length);
            numbers =
                new Dictionary<UIMotifTokenId, float>(
                    numberTokens.Length);

            for (int i = 0; i < colorTokens.Length; i++)
            {
                UIMotifColorToken token = colorTokens[i];
                colors.Add(token.TokenId, token.Value);
            }

            for (int i = 0; i < selectableColorTokens.Length; i++)
            {
                UIMotifSelectableColorsToken token =
                    selectableColorTokens[i];
                selectableColors.Add(token.TokenId, token.Value);
            }

            for (int i = 0; i < spriteTokens.Length; i++)
            {
                UIMotifSpriteToken token = spriteTokens[i];
                sprites.Add(token.TokenId, token.Value);
            }

            for (int i = 0; i < numberTokens.Length; i++)
            {
                UIMotifNumberToken token = numberTokens[i];
                numbers.Add(token.TokenId, token.Value);
            }
        }

        public UIMotifId MotifId { get; }

        public int ColorCount =>
            colors.Count;

        public int SelectableColorsCount =>
            selectableColors.Count;

        public int SpriteCount =>
            sprites.Count;

        public int NumberCount =>
            numbers.Count;

        public int TokenCount =>
            ColorCount +
            SelectableColorsCount +
            SpriteCount +
            NumberCount;

        public bool ContainsToken(UIMotifTokenId tokenId) =>
            colors.ContainsKey(tokenId) ||
            selectableColors.ContainsKey(tokenId) ||
            sprites.ContainsKey(tokenId) ||
            numbers.ContainsKey(tokenId);

        public bool TryGetColor(
            UIMotifTokenId tokenId,
            out Color value) =>
            colors.TryGetValue(tokenId, out value);

        public bool TryGetSelectableColors(
            UIMotifTokenId tokenId,
            out ColorBlock value) =>
            selectableColors.TryGetValue(tokenId, out value);

        public bool TryGetSprite(
            UIMotifTokenId tokenId,
            out Sprite value) =>
            sprites.TryGetValue(tokenId, out value);

        public bool TryGetNumber(
            UIMotifTokenId tokenId,
            out float value) =>
            numbers.TryGetValue(tokenId, out value);
    }
}
