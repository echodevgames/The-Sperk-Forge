using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifContractTests
    {
        private readonly List<UIMotifDefinition> definitions =
            new List<UIMotifDefinition>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < definitions.Count; i++)
            {
                if (definitions[i] != null)
                {
                    Object.DestroyImmediate(definitions[i]);
                }
            }

            definitions.Clear();
        }

        [Test]
        public void StableIdentitiesNormalizeAndCompareOrdinally()
        {
            UIMotifId motif =
                new UIMotifId(" motif.midnight ");
            UIMotifTokenId token =
                new UIMotifTokenId(" color.surface ");

            Assert.That(motif.IsValid, Is.True);
            Assert.That(motif.Value, Is.EqualTo("motif.midnight"));
            Assert.That(
                motif,
                Is.EqualTo(new UIMotifId("motif.midnight")));
            Assert.That(
                motif,
                Is.Not.EqualTo(new UIMotifId("Motif.Midnight")));
            Assert.That(token.Value, Is.EqualTo("color.surface"));
            Assert.That(new UIMotifId("   ").IsEmpty, Is.True);
            Assert.That(new UIMotifTokenId(null).IsEmpty, Is.True);
        }

        [Test]
        public void ValidDefinitionCreatesDeterministicTypedSnapshot()
        {
            ColorBlock selectable = ColorBlock.defaultColorBlock;
            selectable.normalColor = new Color(0.1f, 0.2f, 0.3f, 1f);

            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient(
                    " motif.midnight ",
                    colorTokens: new[]
                    {
                        new UIMotifColorToken(
                            "color.surface",
                            new Color(0.05f, 0.06f, 0.07f, 1f))
                    },
                    selectableColorTokens: new[]
                    {
                        new UIMotifSelectableColorsToken(
                            "selectable.primary",
                            selectable)
                    },
                    spriteTokens: new[]
                    {
                        new UIMotifSpriteToken("sprite.panel", null)
                    },
                    numberTokens: new[]
                    {
                        new UIMotifNumberToken("number.corner-radius", 8f)
                    }));

            UIMotifDefinitionResult result =
                definition.CreateSnapshot(maximumTokenCount: 8);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Status, Is.EqualTo(UIMotifDefinitionStatus.Ready));
            Assert.That(result.MotifId.Value, Is.EqualTo("motif.midnight"));
            Assert.That(result.TokenCount, Is.EqualTo(4));
            Assert.That(result.Snapshot.TokenCount, Is.EqualTo(4));

            Assert.That(
                result.Snapshot.TryGetColor(
                    new UIMotifTokenId("color.surface"),
                    out Color color),
                Is.True);
            Assert.That(color.r, Is.EqualTo(0.05f));

            Assert.That(
                result.Snapshot.TryGetSelectableColors(
                    new UIMotifTokenId("selectable.primary"),
                    out ColorBlock resolvedSelectable),
                Is.True);
            Assert.That(
                resolvedSelectable.normalColor,
                Is.EqualTo(selectable.normalColor));

            Assert.That(
                result.Snapshot.TryGetSprite(
                    new UIMotifTokenId("sprite.panel"),
                    out Sprite sprite),
                Is.True);
            Assert.That(sprite, Is.Null);

            Assert.That(
                result.Snapshot.TryGetNumber(
                    new UIMotifTokenId("number.corner-radius"),
                    out float number),
                Is.True);
            Assert.That(number, Is.EqualTo(8f));
            Assert.That(
                result.Snapshot.ContainsToken(
                    new UIMotifTokenId("Color.Surface")),
                Is.False);
        }

        [Test]
        public void RuntimeSnapshotRemainsDetachedFromAuthoredArrays()
        {
            UIMotifColorToken[] authored =
            {
                new UIMotifColorToken("color.surface", Color.red)
            };

            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.detached",
                    colorTokens: authored));

            authored[0] =
                new UIMotifColorToken("color.surface", Color.green);

            UIMotifSnapshot snapshot =
                definition.CreateSnapshot(4).Snapshot;

            SetPrivateField(
                definition,
                "colorTokens",
                new[]
                {
                    new UIMotifColorToken("color.surface", Color.blue)
                });

            Assert.That(
                snapshot.TryGetColor(
                    new UIMotifTokenId("color.surface"),
                    out Color value),
                Is.True);
            Assert.That(value, Is.EqualTo(Color.red));
        }

        [Test]
        public void NullDefinitionAndInvalidCapacityReturnStructuredFailure()
        {
            UIMotifDefinitionResult missing =
                UIMotifDefinition.CreateSnapshot(
                    definition: null,
                    maximumTokenCount: 4);

            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient("motif.valid"));

            UIMotifDefinitionResult invalidCapacity =
                definition.CreateSnapshot(maximumTokenCount: 0);

            Assert.That(
                missing.Status,
                Is.EqualTo(UIMotifDefinitionStatus.MissingDefinition));
            Assert.That(missing.Succeeded, Is.False);
            Assert.That(missing.Snapshot, Is.Null);
            Assert.That(
                invalidCapacity.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidCapacity));
            Assert.That(invalidCapacity.Snapshot, Is.Null);
        }

        [Test]
        public void InvalidMotifIdRejectsWithoutSnapshot()
        {
            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient(
                    "   ",
                    colorTokens: new[]
                    {
                        new UIMotifColorToken("color.surface", Color.white)
                    }));

            UIMotifDefinitionResult result =
                definition.CreateSnapshot(maximumTokenCount: 4);

            Assert.That(
                result.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidMotifId));
            Assert.That(result.Snapshot, Is.Null);
            Assert.That(result.TokenCount, Is.EqualTo(0));
        }

        [Test]
        public void InvalidTokenIdRejectsWithoutSnapshot()
        {
            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.invalid-token",
                    numberTokens: new[]
                    {
                        new UIMotifNumberToken("  ", 1f)
                    }));

            UIMotifDefinitionResult result =
                definition.CreateSnapshot(maximumTokenCount: 4);

            Assert.That(
                result.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidTokenId));
            Assert.That(result.TokenKind, Is.EqualTo(UIMotifTokenKind.Number));
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void DuplicateIdsAcrossTokenFamiliesRejectGlobally()
        {
            UIMotifDefinition definition = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.duplicate",
                    colorTokens: new[]
                    {
                        new UIMotifColorToken("shared.token", Color.white)
                    },
                    numberTokens: new[]
                    {
                        new UIMotifNumberToken(" shared.token ", 1f)
                    }));

            UIMotifDefinitionResult result =
                definition.CreateSnapshot(maximumTokenCount: 4);

            Assert.That(
                result.Status,
                Is.EqualTo(UIMotifDefinitionStatus.DuplicateTokenId));
            Assert.That(result.TokenId.Value, Is.EqualTo("shared.token"));
            Assert.That(result.TokenKind, Is.EqualTo(UIMotifTokenKind.Number));
            Assert.That(result.Snapshot, Is.Null);
        }

        [Test]
        public void CapacityAndNonFiniteValuesRejectBeforeSnapshotCommit()
        {
            ColorBlock invalidSelectable = ColorBlock.defaultColorBlock;
            invalidSelectable.fadeDuration = -1f;

            UIMotifDefinition overCapacity = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.full",
                    colorTokens: new[]
                    {
                        new UIMotifColorToken("color.one", Color.white),
                        new UIMotifColorToken("color.two", Color.black)
                    }));

            UIMotifDefinition invalidNumber = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.nan",
                    numberTokens: new[]
                    {
                        new UIMotifNumberToken("number.invalid", float.NaN)
                    }));

            UIMotifDefinition invalidColor = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.infinite-color",
                    colorTokens: new[]
                    {
                        new UIMotifColorToken(
                            "color.invalid",
                            new Color(float.PositiveInfinity, 0f, 0f, 1f))
                    }));

            UIMotifDefinition invalidSelectableColors = Track(
                UIMotifDefinition.CreateTransient(
                    "motif.invalid-selectable",
                    selectableColorTokens: new[]
                    {
                        new UIMotifSelectableColorsToken(
                            "selectable.invalid",
                            invalidSelectable)
                    }));

            UIMotifDefinitionResult capacity =
                overCapacity.CreateSnapshot(maximumTokenCount: 1);
            UIMotifDefinitionResult number =
                invalidNumber.CreateSnapshot(maximumTokenCount: 1);
            UIMotifDefinitionResult color =
                invalidColor.CreateSnapshot(maximumTokenCount: 1);
            UIMotifDefinitionResult selectableColors =
                invalidSelectableColors.CreateSnapshot(
                    maximumTokenCount: 1);

            Assert.That(
                capacity.Status,
                Is.EqualTo(UIMotifDefinitionStatus.CapacityExceeded));
            Assert.That(capacity.TokenCount, Is.EqualTo(2));
            Assert.That(capacity.Snapshot, Is.Null);
            Assert.That(
                number.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidTokenValue));
            Assert.That(number.TokenId.Value, Is.EqualTo("number.invalid"));
            Assert.That(number.Snapshot, Is.Null);
            Assert.That(
                color.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidTokenValue));
            Assert.That(color.TokenKind, Is.EqualTo(UIMotifTokenKind.Color));
            Assert.That(color.Snapshot, Is.Null);
            Assert.That(
                selectableColors.Status,
                Is.EqualTo(UIMotifDefinitionStatus.InvalidTokenValue));
            Assert.That(
                selectableColors.TokenKind,
                Is.EqualTo(UIMotifTokenKind.SelectableColors));
            Assert.That(selectableColors.Snapshot, Is.Null);
        }

        private UIMotifDefinition Track(UIMotifDefinition definition)
        {
            definitions.Add(definition);
            return definition;
        }

        private static void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field =
                target.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
