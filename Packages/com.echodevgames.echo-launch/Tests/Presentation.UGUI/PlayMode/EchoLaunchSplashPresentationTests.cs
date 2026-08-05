//----- EchoLaunchSplashPresentationTests.cs START -----

using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch;
using EchoDevGames.EchoLaunch.Presentation.UGUI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Presentation.UGUI
{
    public sealed class
        EchoLaunchSplashPresentationTests
    {
        private const string SequenceId =
            "11111111111111111111111111111111";

        private const string EntryId =
            "22222222222222222222222222222222";

        private readonly List<Object> createdAssets =
            new List<Object>();

        [TearDown]
        public void TearDown()
        {
            for (int index =
                     createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object asset =
                    createdAssets[index];

                if (asset != null)
                {
                    Object.DestroyImmediate(
                        asset);
                }
            }

            createdAssets.Clear();
        }

        [Test]
        public void ViewImplementsImageSplashPresenter()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            Assert.That(
                rig.View,
                Is.InstanceOf<
                    IImageSplashPresenter>());
        }

        [Test]
        public void PresentSplashBeforeBindIsNoOp()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    true));

            Assert.That(
                rig.View.IsShowingSplash,
                Is.False);

            Assert.That(
                rig.View.LastSplashFrame,
                Is.Null);
        }

        [Test]
        public void PresentSplashShowsImageLabelAndCopy()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            SplashPresentationFrame frame =
                CreateFrame(
                    rig.Sprite,
                    0.75f,
                    false);

            rig.View.PresentSplash(frame);

            Assert.That(
                rig.View.IsShowingSplash,
                Is.True);

            Assert.That(
                rig.View.LastSplashFrame,
                Is.SameAs(frame));

            Assert.That(
                rig.SplashImage.sprite,
                Is.SameAs(
                    rig.Sprite));

            Assert.That(
                rig.SplashLabel.text,
                Is.EqualTo(
                    "Forge Light"));

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Showing splash."));

            Assert.That(
                rig.StepText.text,
                Is.EqualTo(
                    "Splash 1 of 2"));
        }

        [Test]
        public void PresentSplashAppliesFrameAlpha()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    0.35f,
                    false));

            Assert.That(
                rig.SplashImage.color.a,
                Is.EqualTo(0.35f)
                    .Within(0.0001f));
        }

        [Test]
        public void RequestSkipWithoutSubscriberReturnsFalse()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    true));

            Assert.That(
                rig.View.RequestSplashSkip(),
                Is.False);
        }

        [Test]
        public void RequestSkipRaisesPublicEvent()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    true));

            int requestCount = 0;

            rig.View.SkipRequested +=
                () => requestCount++;

            Assert.That(
                rig.View.RequestSplashSkip(),
                Is.True);

            Assert.That(
                requestCount,
                Is.EqualTo(1));
        }

        [Test]
        public void ClearSplashHidesAndClearsImage()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    true));

            rig.View.ClearSplash();

            Assert.That(
                rig.View.IsShowingSplash,
                Is.False);

            Assert.That(
                rig.View.LastSplashFrame,
                Is.Null);

            Assert.That(
                rig.SplashImage.sprite,
                Is.Null);

            Assert.That(
                rig.SplashLabel.text,
                Is.Empty);
        }

        [Test]
        public void UnbindClearsSplashAndSkipHandlers()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    true));

            int requestCount = 0;

            rig.View.SkipRequested +=
                () => requestCount++;

            rig.View.Unbind();

            Assert.That(
                rig.View.IsShowingSplash,
                Is.False);

            Assert.That(
                rig.View.RequestSplashSkip(),
                Is.False);

            Assert.That(
                requestCount,
                Is.EqualTo(0));
        }

        [Test]
        public void NullSplashFrameThrowsWhenBound()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.Bind();

            Assert.Throws<
                ArgumentNullException>(
                () =>
                    rig.View
                        .PresentSplash(
                            null));
        }

        [Test]
        public void MissingSplashReferencesRemainSafe()
        {
            GameObject target =
                new GameObject(
                    "Minimal Splash View");

            createdAssets.Add(target);

            EchoLaunchStatusView view =
                target.AddComponent<
                    EchoLaunchStatusView>();

            view.Bind(
                LaunchProgressSnapshot.Empty);

            Assert.DoesNotThrow(
                () =>
                    view.PresentSplash(
                        CreateFrame(
                            CreateSprite(),
                            1f,
                            true)));

            Assert.DoesNotThrow(
                view.ClearSplash);
        }

        private SplashPresentationFrame
            CreateFrame(
                Sprite sprite,
                float alpha,
                bool canSkip)
        {
            SplashEntry entry =
                new SplashEntry(
                    EntryId,
                    sprite,
                    "Forge Light",
                    0.25d,
                    1d,
                    0.25d,
                    0.5d,
                    SplashSkipPolicy
                        .AfterMinimumDisplay);

            return new SplashPresentationFrame(
                SequenceId,
                entry,
                0,
                2,
                SplashPlaybackPhase.Hold,
                alpha,
                0.75d,
                0.5d,
                canSkip,
                false);
        }

        private Sprite CreateSprite()
        {
            Texture2D texture =
                new Texture2D(
                    2,
                    2);

            createdAssets.Add(texture);

            Sprite sprite =
                Sprite.Create(
                    texture,
                    new Rect(
                        0f,
                        0f,
                        2f,
                        2f),
                    new Vector2(
                        0.5f,
                        0.5f));

            createdAssets.Add(sprite);

            return sprite;
        }

        private sealed class SplashViewRig :
            IDisposable
        {
            private readonly List<Object>
                createdAssets;

            internal SplashViewRig(
                List<Object> createdAssets)
            {
                this.createdAssets =
                    createdAssets;

                Root =
                    new GameObject(
                        "Splash View",
                        typeof(RectTransform),
                        typeof(CanvasGroup));

                View =
                    Root.AddComponent<
                        EchoLaunchStatusView>();

                StateText =
                    CreateText(
                        "State");

                MessageText =
                    CreateText(
                        "Message");

                StepText =
                    CreateText(
                        "Step");

                ProgressText =
                    CreateText(
                        "Progress");

                ElapsedText =
                    CreateText(
                        "Elapsed");

                GameObject determinateRoot =
                    CreateChild(
                        "Determinate");

                Slider slider =
                    determinateRoot
                        .AddComponent<Slider>();

                GameObject indeterminateRoot =
                    CreateChild(
                        "Indeterminate");

                SplashRoot =
                    CreateChild(
                        "Splash");

                SplashImage =
                    SplashRoot
                        .AddComponent<Image>();

                SplashLabel =
                    CreateText(
                        "Splash Label");

                SplashLabel.transform
                    .SetParent(
                        SplashRoot.transform,
                        false);

                View.ConfigureForTesting(
                    Root.GetComponent<
                        CanvasGroup>(),
                    StateText,
                    MessageText,
                    StepText,
                    ProgressText,
                    ElapsedText,
                    slider,
                    determinateRoot,
                    indeterminateRoot);

                View.ConfigureSplashForTesting(
                    SplashRoot,
                    SplashImage,
                    SplashLabel);

                Sprite =
                    CreateSprite();
            }

            internal GameObject Root
            {
                get;
            }

            internal EchoLaunchStatusView View
            {
                get;
            }

            internal Text StateText
            {
                get;
            }

            internal Text MessageText
            {
                get;
            }

            internal Text StepText
            {
                get;
            }

            internal Text ProgressText
            {
                get;
            }

            internal Text ElapsedText
            {
                get;
            }

            internal GameObject SplashRoot
            {
                get;
            }

            internal Image SplashImage
            {
                get;
            }

            internal Text SplashLabel
            {
                get;
            }

            internal Sprite Sprite
            {
                get;
            }

            internal void Bind()
            {
                View.Bind(
                    LaunchProgressSnapshot
                        .Empty);
            }

            public void Dispose()
            {
                Object.DestroyImmediate(
                    Root);
            }

            private GameObject CreateChild(
                string objectName)
            {
                GameObject child =
                    new GameObject(
                        objectName,
                        typeof(RectTransform));

                child.transform.SetParent(
                    Root.transform,
                    false);

                return child;
            }

            private Text CreateText(
                string objectName)
            {
                return CreateChild(
                        objectName)
                    .AddComponent<Text>();
            }

            private Sprite CreateSprite()
            {
                Texture2D texture =
                    new Texture2D(
                        2,
                        2);

                createdAssets.Add(texture);

                Sprite sprite =
                    Sprite.Create(
                        texture,
                        new Rect(
                            0f,
                            0f,
                            2f,
                            2f),
                        new Vector2(
                            0.5f,
                            0.5f));

                createdAssets.Add(sprite);

                return sprite;
            }
        }
    }
}

//----- EchoLaunchSplashPresentationTests.cs END -----
