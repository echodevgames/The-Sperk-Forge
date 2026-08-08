//----- EchoLaunchRootSplashPresentationSettingsTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class
        EchoLaunchRootSplashPresentationSettingsTests
    {
        private readonly List<Object> createdAssets =
            new List<Object>();

        [TearDown]
        public void TearDown()
        {
            for (int index = 0;
                 index < createdAssets.Count;
                 index++)
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
        public void RootConfiguresSplashOnlyBeforePresenterBind()
        {
            EchoLaunchRoot root =
                CreateRoot();

            SplashSequence sequence =
                ScriptableObject.CreateInstance<
                    SplashSequence>();

            createdAssets.Add(
                sequence);

            SplashPresentationSettings settings =
                new SplashPresentationSettings(
                    SplashPresentationMode.SplashOnly,
                    new Color(
                        0.1f,
                        0.2f,
                        0.3f,
                        1f),
                    true);

            sequence
                .SetPresentationSettingsForTesting(
                    settings);

            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(
                configuration);

            SetPrivateField(
                configuration,
                "splashSequence",
                sequence);

            SetPrivateField(
                root,
                "configuration",
                configuration);

            RecordingPresenter presenter =
                new RecordingPresenter();

            root.SetStatusPresenterForTesting(
                presenter);

            InvokePrivate(
                root,
                "BindStatusPresenter");

            Assert.That(
                presenter.Calls,
                Is.EqualTo(
                    new[]
                    {
                        "Configure",
                        "Bind",
                    }));

            Assert.That(
                presenter.Settings,
                Is.SameAs(settings));
        }

        [Test]
        public void RootUsesLegacyPresentationWhenNoSequenceIsConfigured()
        {
            EchoLaunchRoot root =
                CreateRoot();

            RecordingPresenter presenter =
                new RecordingPresenter();

            root.SetStatusPresenterForTesting(
                presenter);

            InvokePrivate(
                root,
                "BindStatusPresenter");

            Assert.That(
                presenter.Settings,
                Is.Not.Null);

            Assert.That(
                presenter.Settings.PresentationMode,
                Is.EqualTo(
                    SplashPresentationMode
                        .SplashAndStatus));

            Assert.That(
                presenter.Settings.BackgroundColor,
                Is.EqualTo(Color.black));

            Assert.That(
                presenter.Settings.AllowUserAdvance,
                Is.True);
        }

        private EchoLaunchRoot CreateRoot()
        {
            GameObject target =
                new GameObject(
                    "A1 Presentation Root");

            createdAssets.Add(
                target);

            EchoLaunchRoot root =
                target.AddComponent<
                    EchoLaunchRoot>();

            root.SetAutomaticStartForTesting(
                false);

            return root;
        }

        private static void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field =
                target.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                field,
                Is.Not.Null,
                $"Missing field {fieldName}.");

            field.SetValue(
                target,
                value);
        }

        private static void InvokePrivate(
            object target,
            string methodName)
        {
            MethodInfo method =
                target.GetType().GetMethod(
                    methodName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                method,
                Is.Not.Null,
                $"Missing method {methodName}.");

            method.Invoke(
                target,
                null);
        }

        private sealed class RecordingPresenter :
            ILaunchStatusPresenter,
            ISplashPresentationSettingsReceiver
        {
            internal readonly List<string> Calls =
                new List<string>();

            internal SplashPresentationSettings Settings
            {
                get;
                private set;
            }

            public void ConfigureSplashPresentation(
                SplashPresentationSettings settings)
            {
                Calls.Add(
                    "Configure");

                Settings =
                    settings;
            }

            public void Bind(
                LaunchProgressSnapshot initialSnapshot)
            {
                Calls.Add(
                    "Bind");
            }

            public void Present(
                LaunchProgressSnapshot snapshot)
            {
            }

            public void PresentTerminal(
                LaunchReport report)
            {
            }

            public void Unbind()
            {
            }
        }
    }
}

//----- EchoLaunchRootSplashPresentationSettingsTests.cs END -----
