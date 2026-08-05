//----- EchoLaunchAutomaticStartAndPresenterTests.cs START -----

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class
        RecordingLaunchStatusPresenter :
            ILaunchStatusPresenter
    {
        internal readonly List<LaunchProgressSnapshot>
            PresentedSnapshots =
                new List<LaunchProgressSnapshot>();

        internal LaunchProgressSnapshot
            BoundSnapshot
        {
            get;
            private set;
        }

        internal LaunchReport TerminalReport
        {
            get;
            private set;
        }

        internal int BindCallCount
        {
            get;
            private set;
        }

        internal int PresentCallCount
        {
            get;
            private set;
        }

        internal int TerminalCallCount
        {
            get;
            private set;
        }

        internal int UnbindCallCount
        {
            get;
            private set;
        }

        internal bool ThrowOnBind
        {
            get;
            set;
        }

        internal bool ThrowOnNextPresent
        {
            get;
            set;
        }

        internal bool ThrowOnTerminal
        {
            get;
            set;
        }

        internal bool ThrowOnUnbind
        {
            get;
            set;
        }

        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
            BindCallCount++;
            BoundSnapshot = initialSnapshot;

            if (ThrowOnBind)
            {
                throw new InvalidOperationException(
                    "Presenter bind failure.");
            }
        }

        public void Present(
            LaunchProgressSnapshot snapshot)
        {
            PresentCallCount++;
            PresentedSnapshots.Add(snapshot);

            if (ThrowOnNextPresent)
            {
                ThrowOnNextPresent = false;

                throw new InvalidOperationException(
                    "Presenter progress failure.");
            }
        }

        public void PresentTerminal(
            LaunchReport report)
        {
            TerminalCallCount++;
            TerminalReport = report;

            if (ThrowOnTerminal)
            {
                throw new InvalidOperationException(
                    "Presenter terminal failure.");
            }
        }

        public void Unbind()
        {
            UnbindCallCount++;

            if (ThrowOnUnbind)
            {
                throw new InvalidOperationException(
                    "Presenter unbind failure.");
            }
        }
    }

    public sealed class
        RecordingLaunchStatusPresenterComponent :
            MonoBehaviour,
            ILaunchStatusPresenter
    {
        internal int BindCallCount
        {
            get;
            private set;
        }

        internal int PresentCallCount
        {
            get;
            private set;
        }

        internal int TerminalCallCount
        {
            get;
            private set;
        }

        internal int UnbindCallCount
        {
            get;
            private set;
        }

        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
            BindCallCount++;
        }

        public void Present(
            LaunchProgressSnapshot snapshot)
        {
            PresentCallCount++;
        }

        public void PresentTerminal(
            LaunchReport report)
        {
            TerminalCallCount++;
        }

        public void Unbind()
        {
            UnbindCallCount++;
        }
    }

    public sealed class
        InvalidLaunchStatusPresenterComponent :
            MonoBehaviour
    {
    }

    internal sealed class
        CountingImmediateDestinationLoader :
            IInitialDestinationLoader
    {
        internal int LoadCallCount
        {
            get;
            private set;
        }

        public async Awaitable<
            InitialDestinationLoadResult>
            LoadAsync(
                LaunchDestination destination,
                IProgress<float> progress,
                CancellationToken cancellationToken)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(
                    nameof(destination));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(
                    nameof(progress));
            }

            LoadCallCount++;

            await Awaitable.MainThreadAsync();

            if (cancellationToken
                .IsCancellationRequested)
            {
                return InitialDestinationLoadResult
                    .Cancelled(
                        destination.DestinationId,
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode,
                        "Automatic destination loading was cancelled.");
            }

            progress.Report(1f);

            return InitialDestinationLoadResult
                .Success(
                    destination.DestinationId,
                    "Automatic destination activated.");
        }
    }

    /// <summary>
    /// FL-M4-01 proof for Unity Start-driven root execution and the neutral
    /// status-presenter contract.
    /// </summary>
    public sealed class
        EchoLaunchAutomaticStartAndPresenterTests
    {
        private const int MaximumFramesToWait = 120;

        private static readonly FieldInfo
            RootConfigurationField =
                typeof(EchoLaunchRoot).GetField(
                    "configuration",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            RootPresenterComponentField =
                typeof(EchoLaunchRoot).GetField(
                    "statusPresenterComponent",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationSequenceField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "startupSequence",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationDestinationField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "initialDestination",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationDisplayNameField =
                typeof(LaunchDestination)
                    .GetField(
                        "displayName",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationScenePathField =
                typeof(LaunchDestination)
                    .GetField(
                        "scenePath",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceEntriesField =
                typeof(StartupSequence).GetField(
                    "entries",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<GameObject>
            createdObjects =
                new List<GameObject>();

        private readonly List<Object>
            createdAssets =
                new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                RootConfigurationField,
                Is.Not.Null);

            Assert.That(
                RootPresenterComponentField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSequenceField,
                Is.Not.Null);

            Assert.That(
                ConfigurationDestinationField,
                Is.Not.Null);

            Assert.That(
                DestinationDisplayNameField,
                Is.Not.Null);

            Assert.That(
                DestinationScenePathField,
                Is.Not.Null);

            Assert.That(
                SequenceEntriesField,
                Is.Not.Null);

            LaunchAuthorityClaim.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            for (int index =
                     createdObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                GameObject target =
                    createdObjects[index];

                if (target != null)
                {
                    Object.DestroyImmediate(
                        target);
                }
            }

            createdObjects.Clear();

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
            LaunchAuthorityClaim.Reset();
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator
            AutomaticStartCompletesOnFirstEnabledLaunch()
        {
            CountingImmediateDestinationLoader loader =
                new CountingImmediateDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    loader);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.LastReport,
                Is.Not.Null);

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.IsLaunchActive,
                Is.False);
        }

        [UnityTest]
        public IEnumerator
            DisabledAutomaticStartRemainsAuthorityClaimed()
        {
            CountingImmediateDestinationLoader loader =
                new CountingImmediateDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    loader,
                    automaticStart: false);

            yield return null;
            yield return null;

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                root.LastReport,
                Is.Null);

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator
            ManualStartBeforeUnityStartDoesNotReenter()
        {
            CountingImmediateDestinationLoader loader =
                new CountingImmediateDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    loader);

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForCompletion(
                awaiter);

            yield return null;
            yield return null;

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator
            InjectedPresenterBindsBeforeValidation()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                presenter.BindCallCount,
                Is.EqualTo(1));

            Assert.That(
                presenter.BoundSnapshot.Status,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            Assert.That(
                presenter.PresentedSnapshots[0]
                    .Status,
                Is.EqualTo(
                    LaunchStatus.Validating));
        }

        [UnityTest]
        public IEnumerator
            PresenterReceivesAcceptedLifecycleOrder()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            List<LaunchStatus> observed =
                GetDistinctStatuses(
                    presenter
                        .PresentedSnapshots);

            CollectionAssert.AreEqual(
                new[]
                {
                    LaunchStatus.Validating,
                    LaunchStatus.Running,
                    LaunchStatus.Transitioning,
                    LaunchStatus.Completed
                },
                observed);
        }

        [UnityTest]
        public IEnumerator
            PresenterReceivesExactFinalizedReport()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                presenter.TerminalCallCount,
                Is.EqualTo(1));

            Assert.That(
                presenter.TerminalReport,
                Is.SameAs(
                    root.LastReport));

            Assert.That(
                presenter.TerminalReport
                    .FinalStatus,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [UnityTest]
        public IEnumerator
            MissingPresenterUsesSilentHeadlessFallback()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader());

            yield return WaitForTerminalState(
                root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.IsStatusPresenterBound,
                Is.True);
        }

        [UnityTest]
        public IEnumerator
            SerializedPresenterComponentIsResolved()
        {
            GameObject target =
                CreateInactiveObject(
                    "Serialized Presenter Root");

            RecordingLaunchStatusPresenterComponent
                presenter =
                    target.AddComponent<
                        RecordingLaunchStatusPresenterComponent>();

            EchoLaunchRoot root =
                AddConfiguredRoot(
                    target,
                    CreateConfiguration(),
                    presenter);

            CountingImmediateDestinationLoader loader =
                new CountingImmediateDestinationLoader();

            target.SetActive(true);

            root.SetInitialDestinationLoaderForTesting(
                loader);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                presenter.BindCallCount,
                Is.EqualTo(1));

            Assert.That(
                presenter.PresentCallCount,
                Is.GreaterThan(0));

            Assert.That(
                presenter.TerminalCallCount,
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator
            InvalidSerializedPresenterWarnsAndLaunchContinues()
        {
            GameObject target =
                CreateInactiveObject(
                    "Invalid Presenter Root");

            InvalidLaunchStatusPresenterComponent
                invalidPresenter =
                    target.AddComponent<
                        InvalidLaunchStatusPresenterComponent>();

            EchoLaunchRoot root =
                AddConfiguredRoot(
                    target,
                    CreateConfiguration(),
                    invalidPresenter);

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    "\\[ELAUNCH-VIEW-001\\].*" +
                    "does not implement ILaunchStatusPresenter"));

            target.SetActive(true);

            root.SetInitialDestinationLoaderForTesting(
                new CountingImmediateDestinationLoader());

            yield return WaitForTerminalState(
                root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [UnityTest]
        public IEnumerator
            PresenterBindFailureIsContained()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter
                {
                    ThrowOnBind = true
                };

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    "\\[ELAUNCH-VIEW-002\\].*Bind"));

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [UnityTest]
        public IEnumerator
            PresenterProgressFailureIsContained()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter
                {
                    ThrowOnNextPresent = true
                };

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    "\\[ELAUNCH-VIEW-002\\].*Present"));

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                presenter.PresentCallCount,
                Is.GreaterThan(1));
        }

        [UnityTest]
        public IEnumerator
            PresenterTerminalFailureDoesNotBlockCompletedEvent()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter
                {
                    ThrowOnTerminal = true
                };

            LaunchReport eventReport = null;

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    "\\[ELAUNCH-VIEW-002\\].*PresentTerminal"));

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            root.LaunchCompleted +=
                report => eventReport = report;

            yield return WaitForTerminalState(
                root);

            Assert.That(
                eventReport,
                Is.SameAs(
                    root.LastReport));

            Assert.That(
                presenter.TerminalCallCount,
                Is.EqualTo(1));
        }

        [Test]
        public void
            PresenterReplacementIsRejectedAfterLaunchAdvances()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    automaticStart: false);

            StartupSequenceRunResult result =
                GetImmediate(
                    root.StartLaunchAsync());

            Assert.That(
                result,
                Is.Not.Null);

            InvalidOperationException exception =
                Assert.Throws<
                    InvalidOperationException>(
                    () =>
                        root.SetStatusPresenterForTesting(
                            new RecordingLaunchStatusPresenter()));

            StringAssert.Contains(
                EchoLaunchRoot
                    .StartGateDiagnosticCode,
                exception.Message);
        }

        [Test]
        public void
            NullPresenterInjectionIsRejected()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    automaticStart: false);

            Assert.Throws<
                ArgumentNullException>(
                () =>
                    root.SetStatusPresenterForTesting(
                        null));
        }

        [UnityTest]
        public IEnumerator
            PresenterUnbindsOnceWhenRootIsDestroyed()
        {
            RecordingLaunchStatusPresenter presenter =
                new RecordingLaunchStatusPresenter();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    presenter: presenter);

            yield return WaitForTerminalState(
                root);

            GameObject target =
                root.gameObject;

            Object.DestroyImmediate(
                target);

            createdObjects.Remove(
                target);

            Assert.That(
                presenter.UnbindCallCount,
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator
            DuplicateRootNeverStartsOrBindsPresenter()
        {
            EchoLaunchRoot authority =
                CreateRoot(
                    CreateConfiguration(),
                    new CountingImmediateDestinationLoader(),
                    automaticStart: false,
                    name: "Presenter Authority");

            Assert.That(
                authority.State,
                Is.EqualTo(
                    LaunchStatus.AuthorityClaimed));

            GameObject duplicateObject =
                CreateInactiveObject(
                    "Presenter Duplicate");

            RecordingLaunchStatusPresenterComponent
                duplicatePresenter =
                    duplicateObject.AddComponent<
                        RecordingLaunchStatusPresenterComponent>();

            EchoLaunchRoot duplicate =
                AddConfiguredRoot(
                    duplicateObject,
                    CreateConfiguration(),
                    duplicatePresenter);

            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-ROOT-001] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.");

            duplicateObject.SetActive(true);

            yield return null;

            Assert.That(
                duplicate.WasRejectedAsDuplicate,
                Is.True);

            Assert.That(
                duplicatePresenter.BindCallCount,
                Is.EqualTo(0));

            Assert.That(
                duplicatePresenter.PresentCallCount,
                Is.EqualTo(0));

            Assert.That(
                duplicatePresenter.TerminalCallCount,
                Is.EqualTo(0));
        }

        private EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            IInitialDestinationLoader loader,
            ILaunchStatusPresenter presenter = null,
            bool automaticStart = true,
            string name = "Automatic Launch Root")
        {
            GameObject target =
                CreateInactiveObject(name);

            EchoLaunchRoot root =
                AddConfiguredRoot(
                    target,
                    configuration,
                    null);

            target.SetActive(true);

            if (root.IsAuthoritative)
            {
                root.SetAutomaticStartForTesting(
                    automaticStart);

                root.SetInitialDestinationLoaderForTesting(
                    loader);

                if (presenter != null)
                {
                    root.SetStatusPresenterForTesting(
                        presenter);
                }
            }

            return root;
        }

        private EchoLaunchRoot AddConfiguredRoot(
            GameObject target,
            EchoLaunchConfiguration configuration,
            MonoBehaviour presenterComponent)
        {
            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            RootPresenterComponentField.SetValue(
                root,
                presenterComponent);

            return root;
        }

        private GameObject CreateInactiveObject(
            string name)
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);
            target.SetActive(false);

            return target;
        }

        private EchoLaunchConfiguration
            CreateConfiguration()
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(configuration);

            ConfigurationSequenceField.SetValue(
                configuration,
                CreateEmptySequence());

            ConfigurationDestinationField.SetValue(
                configuration,
                CreateDestination());

            return configuration;
        }

        private StartupSequence CreateEmptySequence()
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<
                    StartupSequence>();

            createdAssets.Add(sequence);

            SequenceEntriesField.SetValue(
                sequence,
                new List<StartupSequenceEntry>());

            return sequence;
        }

        private LaunchDestination CreateDestination()
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<
                    LaunchDestination>();

            createdAssets.Add(destination);

            DestinationDisplayNameField.SetValue(
                destination,
                "Automatic Destination");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/AutomaticDestination.unity");

            return destination;
        }

        private static IEnumerator
            WaitForTerminalState(
                EchoLaunchRoot root)
        {
            int waitedFrames = 0;

            while (root != null &&
                   root.State !=
                       LaunchStatus.Completed &&
                   root.State !=
                       LaunchStatus.Failed &&
                   root.State !=
                       LaunchStatus.Interrupted &&
                   waitedFrames <
                       MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                root,
                Is.Not.Null);

            Assert.That(
                root.State ==
                    LaunchStatus.Completed ||
                root.State ==
                    LaunchStatus.Failed ||
                root.State ==
                    LaunchStatus.Interrupted,
                Is.True,
                "Automatic launch did not settle inside the bounded Play Mode window.");
        }

        private static IEnumerator WaitForCompletion(
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter)
        {
            int waitedFrames = 0;

            while (!awaiter.IsCompleted &&
                   waitedFrames <
                       MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "Manual launch did not settle inside the bounded Play Mode window.");
        }

        private static StartupSequenceRunResult
            GetImmediate(
                Awaitable<StartupSequenceRunResult>
                    awaitable)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    awaitable.GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The empty deterministic launch must settle synchronously.");

            return awaiter.GetResult();
        }

        private static List<LaunchStatus>
            GetDistinctStatuses(
                IReadOnlyList<
                    LaunchProgressSnapshot> snapshots)
        {
            List<LaunchStatus> result =
                new List<LaunchStatus>();

            for (int index = 0;
                 index < snapshots.Count;
                 index++)
            {
                LaunchStatus status =
                    snapshots[index].Status;

                if (result.Count == 0 ||
                    result[result.Count - 1] !=
                        status)
                {
                    result.Add(status);
                }
            }

            return result;
        }
    }
}

//----- EchoLaunchAutomaticStartAndPresenterTests.cs END -----
