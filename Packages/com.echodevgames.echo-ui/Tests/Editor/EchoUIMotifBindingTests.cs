using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifBindingTests
    {
        private readonly List<UnityEngine.Object> created =
            new List<UnityEngine.Object>();

        [TearDown]
        public void TearDown()
        {
            for (int i = created.Count - 1; i >= 0; i--)
                if (created[i] != null) UnityEngine.Object.DestroyImmediate(created[i]);
            created.Clear();
        }

        [Test]
        public void GraphicColorBindingAppliesResolvedColor()
        {
            Image image = CreateTarget().gameObject.AddComponent<Image>();
            image.color = Color.black;
            UIMotifBindingTarget target = image.GetComponent<UIMotifBindingTarget>();
            target.Configure(graphicColorBindings: new[]
            {
                new UIMotifGraphicColorBinding(image, "color.surface")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Applied));
            Assert.That(result.AppliedBindingCount, Is.EqualTo(1));
            Assert.That(image.color, Is.EqualTo(Color.red));
        }

        [Test]
        public void SelectableColorsBindingAppliesResolvedColorBlock()
        {
            UIMotifBindingTarget target = CreateTarget();
            Button button = target.gameObject.AddComponent<Button>();
            target.Configure(selectableColorBindings: new[]
            {
                new UIMotifSelectableColorsBinding(button, "selectable.primary")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Succeeded, Is.True);
            Assert.That(button.colors.normalColor, Is.EqualTo(Color.green));
        }

        [Test]
        public void ImageSpriteBindingAppliesResolvedSprite()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image image = target.gameObject.AddComponent<Image>();
            UIMotifSnapshot snapshot = CreateSnapshot(out Sprite expectedSprite);
            target.Configure(imageSpriteBindings: new[]
            {
                new UIMotifImageSpriteBinding(image, "sprite.panel")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(snapshot);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(image.sprite, Is.SameAs(expectedSprite));
        }

        [Test]
        public void NumericBindingUsesReplaceableProjectReceiver()
        {
            UIMotifBindingTarget target = CreateTarget();
            NumberReceiver receiver = Track(ScriptableObject.CreateInstance<NumberReceiver>());
            target.Configure(numberBindings: new[]
            {
                new UIMotifNumberBinding(receiver, "number.corner-radius")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Succeeded, Is.True);
            Assert.That(receiver.LastTokenId.Value, Is.EqualTo("number.corner-radius"));
            Assert.That(receiver.Value, Is.EqualTo(8f));
        }

        [Test]
        public void KeepLocalPreservesEveryExcludedField()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image image = target.gameObject.AddComponent<Image>();
            Button button = target.gameObject.AddComponent<Button>();
            NumberReceiver receiver = Track(ScriptableObject.CreateInstance<NumberReceiver>());
            image.color = Color.magenta;
            ColorBlock localColors = button.colors;
            localColors.normalColor = Color.cyan;
            button.colors = localColors;
            target.Configure(
                graphicColorBindings: new[]
                {
                    new UIMotifGraphicColorBinding(image, "color.surface", UIMotifBindingMode.KeepLocal)
                },
                selectableColorBindings: new[]
                {
                    new UIMotifSelectableColorsBinding(button, "selectable.primary", UIMotifBindingMode.KeepLocal)
                },
                imageSpriteBindings: new[]
                {
                    new UIMotifImageSpriteBinding(image, "sprite.panel", UIMotifBindingMode.KeepLocal)
                },
                numberBindings: new[]
                {
                    new UIMotifNumberBinding(receiver, "number.corner-radius", UIMotifBindingMode.KeepLocal)
                });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Partial));
            Assert.That(result.KeptLocalBindingCount, Is.EqualTo(4));
            Assert.That(result.AppliedBindingCount, Is.Zero);
            Assert.That(image.color, Is.EqualTo(Color.magenta));
            Assert.That(button.colors.normalColor, Is.EqualTo(Color.cyan));
            Assert.That(image.sprite, Is.Null);
            Assert.That(receiver.Calls, Is.Zero);
        }

        [Test]
        public void MissingTokenProducesPartialResultWithoutChangingField()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image image = target.gameObject.AddComponent<Image>();
            image.color = Color.yellow;
            target.Configure(graphicColorBindings: new[]
            {
                new UIMotifGraphicColorBinding(image, "color.surface"),
                new UIMotifGraphicColorBinding(image, "color.missing")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Partial));
            Assert.That(result.AppliedBindingCount, Is.EqualTo(1));
            Assert.That(result.FailedBindingCount, Is.EqualTo(1));
            Assert.That(image.color, Is.EqualTo(Color.red));
        }

        [Test]
        public void InvalidTargetAndReceiverRejectionAreCounted()
        {
            UIMotifBindingTarget target = CreateTarget();
            Texture2D invalidReceiver = Track(new Texture2D(1, 1));
            target.Configure(
                graphicColorBindings: new[]
                {
                    new UIMotifGraphicColorBinding(null, "color.surface")
                },
                numberBindings: new[]
                {
                    new UIMotifNumberBinding(invalidReceiver, "number.corner-radius")
                });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Failed));
            Assert.That(result.FailedBindingCount, Is.EqualTo(2));
        }

        [Test]
        public void ReceiverCanRejectNumericApplication()
        {
            UIMotifBindingTarget target = CreateTarget();
            NumberReceiver receiver = Track(ScriptableObject.CreateInstance<NumberReceiver>());
            receiver.Accept = false;
            target.Configure(numberBindings: new[]
            {
                new UIMotifNumberBinding(receiver, "number.corner-radius")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(CreateSnapshot());

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Failed));
            Assert.That(result.FailedBindingCount, Is.EqualTo(1));
        }

        [Test]
        public void NullSnapshotRejectsAllBindingsWithoutMutation()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image image = target.gameObject.AddComponent<Image>();
            image.color = Color.yellow;
            target.Configure(graphicColorBindings: new[]
            {
                new UIMotifGraphicColorBinding(image, "color.surface")
            });

            UIMotifTargetApplyResult result = target.ApplyMotif(null);

            Assert.That(result.Status, Is.EqualTo(UIMotifTargetApplyStatus.Failed));
            Assert.That(result.FailedBindingCount, Is.EqualTo(1));
            Assert.That(image.color, Is.EqualTo(Color.yellow));
        }

        [Test]
        public void ConfigurationCopiesCallerArrays()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image first = target.gameObject.AddComponent<Image>();
            GameObject otherObject = Track(new GameObject("Other image"));
            Image second = otherObject.AddComponent<Image>();
            UIMotifGraphicColorBinding[] bindings =
            {
                new UIMotifGraphicColorBinding(first, "color.surface")
            };
            target.Configure(graphicColorBindings: bindings);
            bindings[0] = new UIMotifGraphicColorBinding(second, "color.surface");

            target.ApplyMotif(CreateSnapshot());

            Assert.That(first.color, Is.EqualTo(Color.red));
            Assert.That(second.color, Is.Not.EqualTo(Color.red));
        }

        [Test]
        public void ServiceRegistrationAndSwitchDriveConcreteTarget()
        {
            UIMotifBindingTarget target = CreateTarget();
            Image image = target.gameObject.AddComponent<Image>();
            target.Configure(graphicColorBindings: new[]
            {
                new UIMotifGraphicColorBinding(image, "color.surface")
            });
            UIMotifService service = CreateService();

            UIMotifRegistrationHandle registration = service.RegisterTarget(target);
            UIMotifSwitchResult switched = service.Switch(new UIMotifId("motif.second"));

            Assert.That(registration.Result.Succeeded, Is.True);
            Assert.That(switched.AppliedTargetCount, Is.EqualTo(1));
            Assert.That(image.color, Is.EqualTo(Color.blue));
        }

        private UIMotifBindingTarget CreateTarget()
        {
            GameObject gameObject = Track(new GameObject("Motif target"));
            return gameObject.AddComponent<UIMotifBindingTarget>();
        }

        private UIMotifSnapshot CreateSnapshot() =>
            CreateSnapshot(out _);

        private UIMotifSnapshot CreateSnapshot(out Sprite sprite)
        {
            Texture2D texture = Track(new Texture2D(2, 2));
            sprite = Track(Sprite.Create(
                texture,
                new Rect(0f, 0f, 2f, 2f),
                new Vector2(0.5f, 0.5f)));
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.green;
            UIMotifDefinition definition = Track(UIMotifDefinition.CreateTransient(
                "motif.binding",
                colorTokens: new[]
                {
                    new UIMotifColorToken("color.surface", Color.red)
                },
                selectableColorTokens: new[]
                {
                    new UIMotifSelectableColorsToken("selectable.primary", colors)
                },
                spriteTokens: new[]
                {
                    new UIMotifSpriteToken("sprite.panel", sprite)
                },
                numberTokens: new[]
                {
                    new UIMotifNumberToken("number.corner-radius", 8f)
                }));
            return definition.CreateSnapshot(8).Snapshot;
        }

        private UIMotifService CreateService()
        {
            UIMotifDefinition first = Track(UIMotifDefinition.CreateTransient(
                "motif.first",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.red) }));
            UIMotifDefinition second = Track(UIMotifDefinition.CreateTransient(
                "motif.second",
                colorTokens: new[] { new UIMotifColorToken("color.surface", Color.blue) }));
            UIMotifCatalog catalog = Track(UIMotifCatalog.CreateTransient(
                "motif.first", "motif.second", new[] { first, second }));
            return new UIMotifService(catalog.CreateSnapshot(4, 4).Snapshot);
        }

        private T Track<T>(T value) where T : UnityEngine.Object
        {
            created.Add(value);
            return value;
        }

        public sealed class NumberReceiver : ScriptableObject, IUIMotifNumberReceiver
        {
            public bool Accept { get; set; } = true;
            public int Calls { get; private set; }
            public UIMotifTokenId LastTokenId { get; private set; }
            public float Value { get; private set; }

            public bool TryApplyMotifNumber(UIMotifTokenId tokenId, float value)
            {
                Calls++;
                LastTokenId = tokenId;
                Value = value;
                return Accept;
            }
        }
    }
}
