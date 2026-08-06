//----- DirectSceneActiveDestinationTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class DirectSceneActiveDestinationTests
    {
        private static readonly FieldInfo DestinationDisplayNameField =
            GetRequiredField("displayName");

        private static readonly FieldInfo DestinationScenePathField =
            GetRequiredField("scenePath");

        private LaunchDestination destination;

        [SetUp]
        public void SetUp()
        {
            destination =
                ScriptableObject.CreateInstance<LaunchDestination>();

            DestinationDisplayNameField.SetValue(
                destination,
                "Already Active Scene");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/AlreadyActive.unity");
        }

        [TearDown]
        public void TearDown()
        {
            if (destination != null)
            {
                UnityEngine.Object.DestroyImmediate(destination);
            }

            destination = null;
        }

        [Test]
        public void ActiveDestinationValidatesWithoutBuildEntry()
        {
            UnityInitialDestinationLoader loader =
                new UnityInitialDestinationLoader(
                    _ => true,
                    _ => -1,
                    _ => null);

            bool accepted = loader.TryValidate(
                destination,
                out string failureMessage);

            Assert.That(accepted, Is.True);
            Assert.That(failureMessage, Is.Empty);
        }

        [Test]
        public void ActiveDestinationSucceedsWithoutStartingSceneLoad()
        {
            int loadCallCount = 0;
            RecordingProgress progress = new RecordingProgress();

            UnityInitialDestinationLoader loader =
                new UnityInitialDestinationLoader(
                    _ => true,
                    _ => -1,
                    _ =>
                    {
                        loadCallCount++;
                        return null;
                    });

            Awaitable<InitialDestinationLoadResult>.Awaiter awaiter =
                loader.LoadAsync(
                        destination,
                        progress,
                        CancellationToken.None)
                    .GetAwaiter();

            Assert.That(awaiter.IsCompleted, Is.True);

            InitialDestinationLoadResult result =
                awaiter.GetResult();

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(loadCallCount, Is.Zero);
            Assert.That(progress.Values, Is.EqualTo(new[] { 1f }));
        }

        [Test]
        public void CancellationBeforeActiveSettlementStillCancels()
        {
            CancellationTokenSource source =
                new CancellationTokenSource();

            source.Cancel();

            RecordingProgress progress = new RecordingProgress();

            UnityInitialDestinationLoader loader =
                new UnityInitialDestinationLoader(
                    _ => true,
                    _ => -1,
                    _ => null);

            Awaitable<InitialDestinationLoadResult>.Awaiter awaiter =
                loader.LoadAsync(
                        destination,
                        progress,
                        source.Token)
                    .GetAwaiter();

            Assert.That(awaiter.IsCompleted, Is.True);

            InitialDestinationLoadResult result =
                awaiter.GetResult();

            Assert.That(result.IsCancelled, Is.True);
            Assert.That(progress.Values, Is.Empty);
        }

        [Test]
        public void NonActiveDestinationStillUsesConfiguredLoadPath()
        {
            int validationCallCount = 0;
            int loadCallCount = 0;

            UnityInitialDestinationLoader loader =
                new UnityInitialDestinationLoader(
                    _ => false,
                    _ =>
                    {
                        validationCallCount++;
                        return 3;
                    },
                    _ =>
                    {
                        loadCallCount++;
                        return null;
                    });

            bool accepted = loader.TryValidate(
                destination,
                out string failureMessage);

            Assert.That(accepted, Is.True);
            Assert.That(failureMessage, Is.Empty);
            Assert.That(validationCallCount, Is.EqualTo(1));

            Awaitable<InitialDestinationLoadResult>.Awaiter awaiter =
                loader.LoadAsync(
                        destination,
                        new RecordingProgress(),
                        CancellationToken.None)
                    .GetAwaiter();

            Assert.That(awaiter.IsCompleted, Is.True);

            InitialDestinationLoadResult result =
                awaiter.GetResult();

            Assert.That(result.IsFailed, Is.True);
            Assert.That(loadCallCount, Is.EqualTo(1));
        }

        private static FieldInfo GetRequiredField(string fieldName)
        {
            FieldInfo field =
                typeof(LaunchDestination).GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            if (field == null)
            {
                throw new MissingFieldException(
                    typeof(LaunchDestination).FullName,
                    fieldName);
            }

            return field;
        }

        private sealed class RecordingProgress : IProgress<float>
        {
            private readonly List<float> values =
                new List<float>();

            internal IReadOnlyList<float> Values => values;

            public void Report(float value)
            {
                values.Add(value);
            }
        }
    }
}

//----- DirectSceneActiveDestinationTests.cs END -----
