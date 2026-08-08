//----- EchoLaunchSplashPresentationA1Tests.cs START -----

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
        EchoLaunchSplashPresentationA1Tests
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
        public void SplashOnlyConfigurationStaysHiddenAtBind()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    true));

            rig.Bind();

            Assert.That(
                rig.View.IsVisible,
                Is.False);

            Assert.That(
                rig.StatusRoot.activeSelf,
                Is.False);
        }

        [Test]
        public void SplashOnlyFrameAppliesBackgroundScaleAndHidesStatus()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            Color background =
                new Color(
                    0.12f,
                    0.23f,
                    0.34f,
                    1f);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    background,
                    true));

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    0.8f,
                    SplashPresentationMode.SplashOnly,
                    background,
                    1.08f,
                    true));

            Assert.That(
                rig.View.IsVisible,
                Is.True);

            Assert.That(
                rig.StatusRoot.activeSelf,
                Is.False);

            Assert.That(
                rig.BackdropImage.color,
                Is.EqualTo(background));

            Assert.That(
                rig.SplashImage.rectTransform
                    .localScale,
                Is.EqualTo(
                    Vector3.one * 1.08f));
        }

        [Test]
        public void ClearSplashInSplashOnlyHidesCanvasAndResetsScale()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    true));

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    1.1f,
                    true));

            rig.View.ClearSplash();

            Assert.That(
                rig.View.IsVisible,
                Is.False);

            Assert.That(
                rig.StatusRoot.activeSelf,
                Is.False);

            Assert.That(
                rig.SplashImage.rectTransform
                    .localScale,
                Is.EqualTo(Vector3.one));
        }

        [Test]
        public void SplashAndStatusPreservesRoutineStatusSurface()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashAndStatus,
                    Color.black,
                    true));

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    SplashPresentationMode.SplashAndStatus,
                    Color.black,
                    1f,
                    true));

            Assert.That(
                rig.View.IsVisible,
                Is.True);

            Assert.That(
                rig.StatusRoot.activeSelf,
                Is.True);

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Showing splash."));
        }

        [Test]
        public void SplashOnlyFailureRevealsReadableStatusDiagnostics()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.white,
                    true));

            rig.Bind();

            rig.View.PresentTerminal(
                CreateFailedReport());

            Assert.That(
                rig.View.IsVisible,
                Is.True);

            Assert.That(
                rig.StatusRoot.activeSelf,
                Is.True);

            Assert.That(
                rig.StateText.text,
                Is.EqualTo(
                    "Launch blocked."));

            Assert.That(
                rig.MessageText.text,
                Is.EqualTo(
                    "[TEST-FAIL] Required launch work failed."));

            Assert.That(
                rig.BackdropImage.color,
                Is.EqualTo(
                    rig.OriginalBackdropColor));
        }

        [Test]
        public void DisabledAdvancementDoesNotRaisePublicRequest()
        {
            using SplashViewRig rig =
                new SplashViewRig(
                    createdAssets);

            rig.View.ConfigureSplashPresentation(
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    false));

            rig.Bind();

            rig.View.PresentSplash(
                CreateFrame(
                    rig.Sprite,
                    1f,
                    SplashPresentationMode.SplashOnly,
                    Color.black,
                    1f,
                    false));

            int requestCount = 0;

            rig.View.SkipRequested +=
                () => requestCount++;

            Assert.That(
                rig.View.RequestSplashSkip(),
                Is.False);

            Assert.That(
                requestCount,
                Is.EqualTo(0));
        }

        private SplashPresentationFrame CreateFrame(
            Sprite sprite,
            float alpha,
            SplashPresentationMode presentationMode,
            Color backgroundColor,
            float imageScale,
            bool allowUserAdvance)
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

            SplashPresentationSettings settings =
                new SplashPresentationSettings(
                    presentationMode,
                    backgroundColor,
                    allowUserAdvance);

            return new SplashPresentationFrame(
                SequenceId,
                entry,
                0,
                2,
                SplashPlaybackPhase.Hold,
                alpha,
                0.75d,
                0.5d,
                settings,
                imageScale,
                allowUserAdvance,
                allowUserAdvance,
                false);
        }

        private static LaunchReport
            CreateFailedReport()
        {
            return new LaunchReport(
                LaunchMode.CanonicalBoot,
                "configuration",
                "sequence",
                string.Empty,
                string.Empty,
                LaunchStatus.Failed,
                0d,
                1d,
                0,
                0,
                0,
                false,
                StartupStepResult
                    .BlockingFailure(
                        "TEST-FAIL",
                        "Required launch work failed."),
                Array.Empty<
                    LaunchStepReport>());
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
                        "A1 Splash View",
                        typeof(RectTransform),
                        typeof(CanvasGroup));

                View =
                    Root.AddComponent<
                        EchoLaunchStatusView>();

                BackdropImage =
                    CreateChild(
                            "Backdrop")
                        .AddComponent<Image>();

                BackdropImage.color =
                    new Color(
                        0.025f,
                        0.03f,
                        0.045f,
                        0.96f);

                OriginalBackdropColor =
                    BackdropImage.color;

                StatusRoot =
                    CreateChild(
                        "Status Root");

                StateText =
                    CreateStatusText(
                        "State");

                MessageText =
                    CreateStatusText(
                        "Message");

                StepText =
                    CreateStatusText(
                        "Step");

                ProgressText =
                    CreateStatusText(
                        "Progress");

                ElapsedText =
                    CreateStatusText(
                        "Elapsed");

                GameObject determinateRoot =
                    CreateStatusChild(
                        "Determinate");

                Slider slider =
                    determinateRoot
                        .AddComponent<Slider>();

                GameObject indeterminateRoot =
                    CreateStatusChild(
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

                View.ConfigurePresentationForTesting(
                    StatusRoot,
                    BackdropImage);

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

            internal GameObject StatusRoot
            {
                get;
            }

            internal Image BackdropImage
            {
                get;
            }

            internal Color OriginalBackdropColor
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

            private Text CreateStatusText(
                string objectName)
            {
                return CreateStatusChild(
                        objectName)
                    .AddComponent<Text>();
            }

            private GameObject CreateStatusChild(
                string objectName)
            {
                GameObject child =
                    new GameObject(
                        objectName,
                        typeof(RectTransform));

                child.transform.SetParent(
                    StatusRoot.transform,
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

            private Sprite CreateSprite()
            {
                Texture2D texture =
                    new Texture2D(
                        2,
                        2);

                createdAssets.Add(
                    texture);

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

                createdAssets.Add(
                    sprite);

                return sprite;
            }
        }
    }
}

//----- EchoLaunchSplashPresentationA1Tests.cs END -----
