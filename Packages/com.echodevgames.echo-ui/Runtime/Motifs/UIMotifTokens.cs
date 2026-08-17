using System;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI
{
    public enum UIMotifTokenKind
    {
        None = 0,
        Color = 1,
        SelectableColors = 2,
        Sprite = 3,
        Number = 4
    }

    [Serializable]
    public struct UIMotifColorToken
    {
        [SerializeField]
        private string tokenId;

        [SerializeField]
        private Color value;

        public UIMotifColorToken(
            string tokenId,
            Color value)
        {
            this.tokenId = tokenId ?? string.Empty;
            this.value = value;
        }

        public UIMotifTokenId TokenId =>
            new UIMotifTokenId(tokenId);

        public Color Value =>
            value;
    }

    [Serializable]
    public struct UIMotifSelectableColorsToken
    {
        [SerializeField]
        private string tokenId;

        [SerializeField]
        private ColorBlock value;

        public UIMotifSelectableColorsToken(
            string tokenId,
            ColorBlock value)
        {
            this.tokenId = tokenId ?? string.Empty;
            this.value = value;
        }

        public UIMotifTokenId TokenId =>
            new UIMotifTokenId(tokenId);

        public ColorBlock Value =>
            value;
    }

    [Serializable]
    public struct UIMotifSpriteToken
    {
        [SerializeField]
        private string tokenId;

        [SerializeField]
        private Sprite value;

        public UIMotifSpriteToken(
            string tokenId,
            Sprite value)
        {
            this.tokenId = tokenId ?? string.Empty;
            this.value = value;
        }

        public UIMotifTokenId TokenId =>
            new UIMotifTokenId(tokenId);

        public Sprite Value =>
            value;
    }

    [Serializable]
    public struct UIMotifNumberToken
    {
        [SerializeField]
        private string tokenId;

        [SerializeField]
        private float value;

        public UIMotifNumberToken(
            string tokenId,
            float value)
        {
            this.tokenId = tokenId ?? string.Empty;
            this.value = value;
        }

        public UIMotifTokenId TokenId =>
            new UIMotifTokenId(tokenId);

        public float Value =>
            value;
    }
}
