//----- LaunchDestinationAndCompletedHandoffTests.cs START -----

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
    /// <summary>
    /// Shared deterministic destination loader used by retained success-path
    /// tests after configuration schema 3 made the destination mandatory.
    /// </summary>
    internal sealed class
        ImmediateSuccessInitialDestinationLoader :
            IInitialDestinationLoader
    {
        internal static
            ImmediateSuccessInitialDestinationLoader Shared
        {
            get;
        } =
            new ImmediateSuccessInitialDestinationLoader();

        private ImmediateSuccessInitialDestinationLoader()
        {
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

            await Awaitable.MainThreadAsync();

            if (cancellationToken
                .IsCancellationRequested)
            {
                return InitialDestinationLoadResult
                    .Cancelled(
                        destination.DestinationId,
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode,
                        "Initial destination loading was cancelled before completion.");
            }

            progress.Report(1f);

            return InitialDestinationLoadResult
                .Success(
                    destination.DestinationId,
                    $"Initial destination '{destination.DisplayName}' activated.");
        }
    }

    /// <summary>
    /// Controlled loader used to prove validation, progress, settlement,
    /// failure, cancellation, and invalid-result handling.
    /// </summary>
    internal sealed class
        ControlledInitialDestinationLoader :
            IInitialDestinationLoader,
            IInitialDestinationPreflightValidator
    {
        internal bool ValidatorAccepts
        {
            get;
            set;
        } = true;

        internal string ValidatorFailureMessage
        {
            get;
            set;
        } = "The controlled destination loader rejected the destination.";

        internal InitialDestinationLoadResult
            ResultToReturn
        {
            get;
            set;
        }

        internal bool ReturnNull
        {
            get;
            set;
        }

        internal int FramesToWait
        {
            get;
            set;
        }

        internal int ValidationCallCount
        {
            get;
            private set;
        }

        internal int LoadCallCount
        {
            get;
            private set;
        }

        internal int FramesCompleted
        {
            get;
            private set;
        }

        internal bool CancellationObserved
        {
            get;
            private set;
        }

        internal bool Settled
        {
            get;
            private set;
        }

        public bool TryValidate(
            LaunchDestination destination,
            out string failureMessage)
        {
            ValidationCallCount++;

            failureMessage =
                ValidatorAccepts
                    ? string.Empty
                    : ValidatorFailureMessage;

            return ValidatorAccepts;
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

            try
            {
                await Awaitable.MainThreadAsync();

                progress.Report(0.25f);

                for (int index = 0;
                     index < FramesToWait;
                     index++)
                {
                    if (cancellationToken
                        .IsCancellationRequested)
                    {
                        CancellationObserved = true;
                    }

                    await Awaitable.NextFrameAsync(
                        CancellationToken.None);

                    FramesCompleted++;
                }

                if (cancellationToken
                    .IsCancellationRequested)
                {
                    CancellationObserved = true;

                    return InitialDestinationLoadResult
                        .Cancelled(
                            destination.DestinationId,
                            EchoLaunchRoot
                                .LifecycleDiagnosticCode,
                            "Controlled destination loading settled after cancellation.");
                }

                progress.Report(0.75f);

                if (ReturnNull)
                {
                    return null;
                }

                return ResultToReturn ??
                       InitialDestinationLoadResult
                           .Success(
                               destination.DestinationId,
                               "Controlled destination activated.");
            }
            finally
            {
                Settled = true;
            }
        }
    }

    /// <summary>
    /// FL-M3-08 proof for project-owned destination data, configuration schema
    /// 3, injected handoff results, Completed lifecycle/report publication,
    /// failure containment, cancellation settlement, and asset immutability.
    /// </summary>
    public sealed class
        LaunchDestinationAndCompletedHandoffTests
    {
        private const int MaximumFramesToWait = 120;

        private static readonly FieldInfo
            RootConfigurationField =
                typeof(EchoLaunchRoot).GetField(
                    "configuration",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            RootLoaderField =
                typeof(EchoLaunchRoot).GetField(
                    "initialDestinationLoader",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationSchemaField =
                typeof(EchoLaunchConfiguration)
                    .GetField(
                        "schemaVersion",
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
            DestinationIdField =
                typeof(LaunchDestination)
                    .GetField(
                        "destinationId",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DestinationSchemaField =
                typeof(LaunchDestination)
                    .GetField(
                        "schemaVersion",
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

        private static readonly FieldInfo
            EntryActivationField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "activation",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryDefinitionField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "stepDefinition",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryPolicyField =
                typeof(StartupSequenceEntry)
                    .GetField(
                        "policy",
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionDisplayNameField =
                typeof(StartupStepDefinition)
                    .GetField(
                        "displayName",
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
                RootLoaderField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSchemaField,
                Is.Not.Null);

            Assert.That(
                ConfigurationSequenceField,
                Is.Not.Null);

            Assert.That(
                ConfigurationDestinationField,
                Is.Not.Null);

            Assert.That(
                DestinationIdField,
                Is.Not.Null);

            Assert.That(
                DestinationSchemaField,
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

            Assert.That(
                EntryActivationField,
                Is.Not.Null);

            Assert.That(
                EntryDefinitionField,
                Is.Not.Null);

            Assert.That(
                EntryPolicyField,
                Is.Not.Null);

            Assert.That(
                DefinitionDisplayNameField,
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

        [Test]
        public void NewDestinationUsesSchemaOne()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.That(
                LaunchDestination.CurrentSchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                destination.SchemaVersion,
                Is.EqualTo(1));

            Assert.That(
                destination.HasSupportedSchema,
                Is.True);
        }

        [Test]
        public void NewDestinationIdentityUsesCanonicalFormat()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.That(
                destination.DestinationId,
                Does.Match("^[0-9a-f]{32}$"));

            Assert.That(
                destination.HasValidIdentity,
                Is.True);
        }

        [Test]
        public void ValidDestinationMetadataIsAccepted()
        {
            LaunchDestination destination =
                CreateDestination(
                    "  Bright Harbor  ",
                    " Assets/Scenes/BrightHarbor.unity ");

            Assert.That(
                destination.HasValidDisplayName,
                Is.False,
                "Authored metadata must already be normalized; runtime does not trim or repair assets.");

            Assert.That(
                destination.HasValidScenePath,
                Is.False);

            DestinationDisplayNameField.SetValue(
                destination,
                "Bright Harbor");

            DestinationScenePathField.SetValue(
                destination,
                "Assets/Scenes/BrightHarbor.unity");

            Assert.That(
                destination.HasValidDisplayName,
                Is.True);

            Assert.That(
                destination.HasValidScenePath,
                Is.True);
        }

        [Test]
        public void MalformedDestinationIdentityIsRejectedWithoutRepair()
        {
            LaunchDestination destination =
                CreateDestination();

            DestinationIdField.SetValue(
                destination,
                "NOT-A-DESTINATION-ID");

            Assert.That(
                destination.HasValidIdentity,
                Is.False);

            Assert.That(
                destination.DestinationId,
                Is.EqualTo(
                    "NOT-A-DESTINATION-ID"));
        }

        [Test]
        public void UnsupportedDestinationSchemaIsRejectedWithoutRewrite()
        {
            LaunchDestination destination =
                CreateDestination();

            DestinationSchemaField.SetValue(
                destination,
                2);

            Assert.That(
                destination.HasSupportedSchema,
                Is.False);

            Assert.That(
                destination.SchemaVersion,
                Is.EqualTo(2));
        }

        [Test]
        public void BlankDestinationDisplayNameIsInvalid()
        {
            LaunchDestination destination =
                CreateDestination();

            DestinationDisplayNameField.SetValue(
                destination,
                "   ");

            Assert.That(
                destination.HasValidDisplayName,
                Is.False);
        }

        [Test]
        public void BlankDestinationScenePathIsInvalid()
        {
            LaunchDestination destination =
                CreateDestination();

            DestinationScenePathField.SetValue(
                destination,
                string.Empty);

            Assert.That(
                destination.HasValidScenePath,
                Is.False);
        }

        [Test]
        public void ConfigurationSchemaIsThree()
        {
            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(),
                    CreateDestination());

            Assert.That(
                EchoLaunchConfiguration
                    .CurrentSchemaVersion,
                Is.EqualTo(3));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(3));

            Assert.That(
                configuration.HasSupportedSchema,
                Is.True);
        }

        [Test]
        public void HistoricalSchemaTwoFailsBeforeFactoryAndLoader()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Historical Schema Step");

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(
                        CreateEntry(definition)),
                    CreateDestination());

            ConfigurationSchemaField.SetValue(
                configuration,
                2);

            EchoLaunchRoot root =
                CreateRoot(
                    configuration,
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .ConfigurationSchemaDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.Zero);

            Assert.That(
                loader.LoadCallCount,
                Is.Zero);

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(2));
        }

        [Test]
        public void MissingDestinationFailsBeforeFactoryAndLoader()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Missing Destination Step");

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        null),
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationPreflightDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.Zero);

            Assert.That(
                loader.LoadCallCount,
                Is.Zero);
        }

        [Test]
        public void InvalidDestinationFailsBeforeFactoryAndLoader()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Invalid Destination Step");

            LaunchDestination destination =
                CreateDestination();

            DestinationScenePathField.SetValue(
                destination,
                "Scenes/Invalid.unity");

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        destination),
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationPreflightDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.Zero);

            Assert.That(
                loader.ValidationCallCount,
                Is.Zero,
                "Intrinsic destination validation must finish before loader-specific validation.");

            Assert.That(
                loader.LoadCallCount,
                Is.Zero);
        }

        [Test]
        public void LoaderValidatorRejectionFailsBeforeFactoryAndLoad()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Validator Rejection Step");

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    ValidatorAccepts = false,
                    ValidatorFailureMessage =
                        "The test destination is not build-loadable."
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        CreateDestination()),
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationPreflightDiagnosticCode));

            Assert.That(
                root.LastReport.FinalResult.Message,
                Is.EqualTo(
                    "The test destination is not build-loadable."));

            Assert.That(
                loader.ValidationCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.LoadCallCount,
                Is.Zero);

            Assert.That(
                definition.FactoryCallCount,
                Is.Zero);
        }

        [Test]
        public void MissingLoaderFailsBeforeFactory()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Missing Loader Step");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        CreateDestination()),
                    ImmediateSuccessInitialDestinationLoader
                        .Shared);

            RootLoaderField.SetValue(
                root,
                null);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationPreflightDiagnosticCode));

            Assert.That(
                definition.FactoryCallCount,
                Is.Zero);
        }

        [Test]
        public void SuccessfulLoadResultIsImmutableAndNormalized()
        {
            LaunchDestination destination =
                CreateDestination();

            InitialDestinationLoadResult result =
                InitialDestinationLoadResult.Success(
                    destination.DestinationId,
                    "  Destination active.  ",
                    "  Stable details.  ");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    InitialDestinationLoadStatus
                        .Succeeded));

            Assert.That(
                result.IsSucceeded,
                Is.True);

            Assert.That(
                result.Message,
                Is.EqualTo(
                    "Destination active."));

            Assert.That(
                result.Details,
                Is.EqualTo(
                    "Stable details."));
        }

        [Test]
        public void FailedLoadResultRequiresDiagnosticCode()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.Throws<ArgumentException>(
                () => InitialDestinationLoadResult
                    .Failed(
                        destination.DestinationId,
                        " ",
                        "Destination failed."));
        }

        [Test]
        public void CancelledLoadResultRequiresMessage()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.Throws<ArgumentException>(
                () => InitialDestinationLoadResult
                    .Cancelled(
                        destination.DestinationId,
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode,
                        " "));
        }

        [Test]
        public void UndefinedLoadStatusIsRejected()
        {
            LaunchDestination destination =
                CreateDestination();

            Assert.Throws<
                ArgumentOutOfRangeException>(
                () => new InitialDestinationLoadResult(
                    (InitialDestinationLoadStatus)99,
                    destination.DestinationId,
                    "ELAUNCH-DEST-099",
                    "Undefined status.",
                    string.Empty));
        }

        [Test]
        public void ProgressRelayAcceptsNormalizedValuesAndIgnoresLateReports()
        {
            List<float> observed =
                new List<float>();

            InitialDestinationProgressRelay relay =
                new InitialDestinationProgressRelay(
                    observed.Add);

            relay.Report(0.25f);
            relay.Report(1f);
            relay.Close();
            relay.Report(0.5f);

            Assert.That(
                relay.IsClosed,
                Is.True);

            Assert.That(
                observed,
                Is.EqualTo(
                    new[] { 0.25f, 1f }));
        }

        [Test]
        public void ProgressRelayRejectsNonFiniteValues()
        {
            InitialDestinationProgressRelay relay =
                new InitialDestinationProgressRelay(
                    _ => { });

            Assert.Throws<
                ArgumentOutOfRangeException>(
                () => relay.Report(
                    float.NaN));

            Assert.Throws<
                ArgumentOutOfRangeException>(
                () => relay.Report(
                    float.PositiveInfinity));
        }

        [Test]
        public void SuccessfulHandoffCompletesAndInvokesLoaderOnce()
        {
            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    loader);

            StartImmediate(root);

            Assert.That(
                loader.ValidationCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                root.LastReport,
                Is.Not.Null);
        }

        [Test]
        public void SuccessfulStateOrderIncludesTransitioningThenCompleted()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    new ControlledInitialDestinationLoader());

            List<LaunchStatus> observed =
                new List<LaunchStatus>();

            root.LaunchStateChanged += change =>
                observed.Add(change.CurrentState);

            StartImmediate(root);

            Assert.That(
                observed,
                Is.EqualTo(
                    new[]
                    {
                        LaunchStatus.Validating,
                        LaunchStatus.Running,
                        LaunchStatus.Transitioning,
                        LaunchStatus.Completed
                    }));
        }

        [Test]
        public void DestinationProgressIsPublishedWhileTransitioning()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    new ControlledInitialDestinationLoader());

            List<LaunchProgressSnapshot> transition =
                new List<LaunchProgressSnapshot>();

            root.LaunchProgressChanged += change =>
            {
                if (change.Current.Status ==
                    LaunchStatus.Transitioning)
                {
                    transition.Add(
                        change.Current);
                }
            };

            StartImmediate(root);

            Assert.That(
                transition.Count,
                Is.GreaterThanOrEqualTo(3));

            Assert.That(
                transition[0].IsProgressIndeterminate,
                Is.True);

            Assert.That(
                transition.Exists(
                    snapshot =>
                        !snapshot.IsProgressIndeterminate &&
                        snapshot.Progress01 == 0.25f),
                Is.True);

            Assert.That(
                transition.Exists(
                    snapshot =>
                        !snapshot.IsProgressIndeterminate &&
                        snapshot.Progress01 == 0.75f),
                Is.True);
        }

        [Test]
        public void CompletedReportContainsDestinationAndSequenceAccounting()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Completed Report Step");

            LaunchDestination destination =
                CreateDestination(
                    "The Bright Harbor",
                    "Assets/Scenes/BrightHarbor.unity");

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        destination),
                    new ControlledInitialDestinationLoader());

            StartImmediate(root);

            LaunchReport report =
                root.LastReport;

            Assert.That(
                report.FinalStatus,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                report.DestinationId,
                Is.EqualTo(
                    destination.DestinationId));

            Assert.That(
                report.DestinationDisplayName,
                Is.EqualTo(
                    "The Bright Harbor"));

            Assert.That(
                report.AuthoredEntryCount,
                Is.EqualTo(1));

            Assert.That(
                report.AttemptedStepCount,
                Is.EqualTo(1));

            Assert.That(
                report.StepReportCount,
                Is.EqualTo(1));

            Assert.That(
                report.GetStepReport(0)
                    .StepDisplayName,
                Is.EqualTo(
                    "Completed Report Step"));
        }

        [Test]
        public void LastReportIsExactCompletedEventPayload()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    new ControlledInitialDestinationLoader());

            LaunchReport payload = null;
            LaunchStatus stateAtEvent =
                LaunchStatus.None;

            root.LaunchCompleted += report =>
            {
                payload = report;
                stateAtEvent = root.State;
            };

            StartImmediate(root);

            Assert.That(
                payload,
                Is.SameAs(
                    root.LastReport));

            Assert.That(
                stateAtEvent,
                Is.EqualTo(
                    LaunchStatus.Completed));
        }

        [Test]
        public void CompletedEventFiresExactlyOnceWithoutFailureEvents()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    new ControlledInitialDestinationLoader());

            int completedCalls = 0;
            int failedCalls = 0;
            int interruptedCalls = 0;

            root.LaunchCompleted += _ =>
                completedCalls++;

            root.LaunchFailed += _ =>
                failedCalls++;

            root.LaunchInterrupted += _ =>
                interruptedCalls++;

            StartImmediate(root);

            Assert.That(
                completedCalls,
                Is.EqualTo(1));

            Assert.That(
                failedCalls,
                Is.Zero);

            Assert.That(
                interruptedCalls,
                Is.Zero);
        }

        [Test]
        public void CompletedListenerFailureDoesNotBlockLaterListener()
        {
            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    new ControlledInitialDestinationLoader());

            int laterCalls = 0;

            LogAssert.Expect(
                LogType.Warning,
                new Regex(
                    @"\[ELAUNCH-EVENT-001\].*'LaunchCompleted'.*completion boom",
                    RegexOptions.Singleline));

            root.LaunchCompleted += _ =>
                throw new InvalidOperationException(
                    "completion boom");

            root.LaunchCompleted += _ =>
                laterCalls++;

            StartImmediate(root);

            Assert.That(
                laterCalls,
                Is.EqualTo(1));
        }

        [Test]
        public void DestinationLoadFailureProducesFailedReportAndNoCompletion()
        {
            LaunchDestination destination =
                CreateDestination();

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    ResultToReturn =
                        InitialDestinationLoadResult
                            .Failed(
                                destination.DestinationId,
                                EchoLaunchRoot
                                    .DestinationLoadDiagnosticCode,
                                "Controlled load failed.",
                                "Stable failure details.")
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        destination),
                    loader);

            int completedCalls = 0;
            root.LaunchCompleted += _ =>
                completedCalls++;

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalStatus,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationLoadDiagnosticCode));

            Assert.That(
                completedCalls,
                Is.Zero);
        }

        [Test]
        public void NullLoadResultProducesDestinationFailure()
        {
            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    ReturnNull = true
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationLoadDiagnosticCode));
        }

        [Test]
        public void MismatchedSuccessDestinationProducesFailure()
        {
            LaunchDestination destination =
                CreateDestination();

            LaunchDestination other =
                CreateDestination(
                    "Other Destination",
                    "Assets/Scenes/OtherDestination.unity");

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    ResultToReturn =
                        InitialDestinationLoadResult
                            .Success(
                                other.DestinationId,
                                "Wrong destination activated.")
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        destination),
                    loader);

            StartImmediate(root);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Failed));

            Assert.That(
                root.LastReport.FinalResult.Code,
                Is.EqualTo(
                    EchoLaunchRoot
                        .DestinationLoadDiagnosticCode));
        }

        [UnityTest]
        public IEnumerator CancellationBeforeDestinationLoadPreventsLoaderInvocation()
        {
            RootLifecycleTestDefinition definition =
                CreateDefinition(
                    "Cancel Before Handoff",
                    framesToWait: 20);

            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader();

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(
                            CreateEntry(definition)),
                        CreateDestination()),
                    loader);

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForDefinitionFrame(
                definition,
                awaiter);

            Assert.That(
                root.CancelLaunch(
                    "Cancel before destination load."),
                Is.True);

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                loader.LoadCallCount,
                Is.Zero);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Interrupted));
        }

        [UnityTest]
        public IEnumerator CancellationDuringLoadWaitsForSettlementThenInterrupts()
        {
            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    FramesToWait = 4
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    loader);

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForLoaderStart(
                loader,
                awaiter);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Transitioning));

            Assert.That(
                root.CancelLaunch(
                    "Cancel during destination loading."),
                Is.True);

            Assert.That(
                awaiter.IsCompleted,
                Is.False);

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                loader.CancellationObserved,
                Is.True);

            Assert.That(
                loader.Settled,
                Is.True);

            Assert.That(
                root.State,
                Is.EqualTo(
                    LaunchStatus.Interrupted));

            Assert.That(
                root.LastReport.WasCancelled,
                Is.True);
        }

        [UnityTest]
        public IEnumerator DestroyedRootPublishesNoLateCompletionEvent()
        {
            ControlledInitialDestinationLoader loader =
                new ControlledInitialDestinationLoader
                {
                    FramesToWait = 4
                };

            EchoLaunchRoot root =
                CreateRoot(
                    CreateConfiguration(
                        CreateSequence(),
                        CreateDestination()),
                    loader);

            int completedCalls = 0;
            root.LaunchCompleted += _ =>
                completedCalls++;

            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            yield return WaitForLoaderStart(
                loader,
                awaiter);

            Object.DestroyImmediate(
                root.gameObject);

            yield return WaitForCompletion(
                awaiter);

            awaiter.GetResult();

            Assert.That(
                loader.CancellationObserved,
                Is.True);

            Assert.That(
                loader.Settled,
                Is.True);

            Assert.That(
                completedCalls,
                Is.Zero);

            Assert.That(
                EchoLaunchRoot.Current,
                Is.Null);
        }

        [Test]
        public void DefaultLoaderRejectsSceneOutsideBuildSettings()
        {
            LaunchDestination destination =
                CreateDestination(
                    "Not In Build",
                    "Assets/Scenes/DefinitelyNotInBuildSettings.unity");

            bool accepted =
                UnityInitialDestinationLoader
                    .Shared
                    .TryValidate(
                        destination,
                        out string failureMessage);

            Assert.That(
                accepted,
                Is.False);

            Assert.That(
                failureMessage,
                Does.Contain(
                    "not included in the player build settings"));
        }

        [Test]
        public void DefaultLoaderHonorsCancellationBeforeStart()
        {
            LaunchDestination destination =
                CreateDestination();

            CancellationTokenSource source =
                new CancellationTokenSource();

            try
            {
                source.Cancel();

                InitialDestinationProgressRelay progress =
                    new InitialDestinationProgressRelay(
                        _ => { });

                Awaitable<InitialDestinationLoadResult>.Awaiter
                    awaiter =
                        UnityInitialDestinationLoader
                            .Shared
                            .LoadAsync(
                                destination,
                                progress,
                                source.Token)
                            .GetAwaiter();

                Assert.That(
                    awaiter.IsCompleted,
                    Is.True);

                InitialDestinationLoadResult result =
                    awaiter.GetResult();

                Assert.That(
                    result.IsCancelled,
                    Is.True);

                Assert.That(
                    result.Code,
                    Is.EqualTo(
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode));
            }
            finally
            {
                source.Dispose();
            }
        }

        [Test]
        public void CompletedReportRequiresDestinationMetadata()
        {
            Assert.Throws<ArgumentException>(
                () => new LaunchReport(
                    LaunchMode.CanonicalBoot,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    LaunchStatus.Completed,
                    0d,
                    1d,
                    0,
                    0,
                    0,
                    false,
                    StartupStepResult.Success(
                        "Completed."),
                    Array.Empty<LaunchStepReport>()));
        }

        [Test]
        public void BuilderFinalizesCompletedReportOnlyOnce()
        {
            LaunchDestination destination =
                CreateDestination();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    CreateSequence(),
                    destination);

            EchoLaunchRoot root =
                CreateRoot(
                    configuration,
                    ImmediateSuccessInitialDestinationLoader
                        .Shared);

            StartupSequenceRunResult runResult =
                StartImmediate(root);

            LaunchReportBuilder builder =
                new LaunchReportBuilder(
                    LaunchMode.CanonicalBoot,
                    configuration,
                    0d);

            builder.MarkTransitionPending(
                runResult);

            LaunchReport report =
                builder.FinalizeCompletedReport(
                    destination,
                    StartupStepResult.Success(
                        "Destination activated."),
                    1d);

            Assert.That(
                report.FinalStatus,
                Is.EqualTo(
                    LaunchStatus.Completed));

            Assert.That(
                builder.IsFinalized,
                Is.True);

            Assert.Throws<InvalidOperationException>(
                () => builder
                    .FinalizeCompletedReport(
                        destination,
                        StartupStepResult.Success(
                            "Second completion."),
                        2d));
        }

        [Test]
        public void CompletedLaunchDoesNotMutateAuthoredAssets()
        {
            LaunchDestination destination =
                CreateDestination();

            StartupSequence sequence =
                CreateSequence();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(
                    sequence,
                    destination);

            string configurationId =
                configuration.ConfigurationId;

            int configurationSchema =
                configuration.SchemaVersion;

            string destinationId =
                destination.DestinationId;

            int destinationSchema =
                destination.SchemaVersion;

            string destinationName =
                destination.DisplayName;

            string scenePath =
                destination.ScenePath;

            EchoLaunchRoot root =
                CreateRoot(
                    configuration,
                    ImmediateSuccessInitialDestinationLoader
                        .Shared);

            StartImmediate(root);

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(
                    configurationId));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    configurationSchema));

            Assert.That(
                configuration.StartupSequence,
                Is.SameAs(sequence));

            Assert.That(
                configuration.InitialDestination,
                Is.SameAs(destination));

            Assert.That(
                destination.DestinationId,
                Is.EqualTo(
                    destinationId));

            Assert.That(
                destination.SchemaVersion,
                Is.EqualTo(
                    destinationSchema));

            Assert.That(
                destination.DisplayName,
                Is.EqualTo(
                    destinationName));

            Assert.That(
                destination.ScenePath,
                Is.EqualTo(
                    scenePath));
        }

        private static StartupSequenceRunResult
            StartImmediate(
                EchoLaunchRoot root)
        {
            Awaitable<StartupSequenceRunResult>.Awaiter
                awaiter =
                    root.StartLaunchAsync()
                        .GetAwaiter();

            Assert.That(
                awaiter.IsCompleted,
                Is.True,
                "The immediate destination fixture must settle synchronously.");

            return awaiter.GetResult();
        }

        private static IEnumerator
            WaitForDefinitionFrame(
                RootLifecycleTestDefinition definition,
                Awaitable<StartupSequenceRunResult>.Awaiter
                    awaiter)
        {
            int waitedFrames = 0;

            while (definition.FramesCompleted < 1 &&
                   !awaiter.IsCompleted &&
                   waitedFrames <
                   MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                definition.FramesCompleted,
                Is.GreaterThanOrEqualTo(1));

            Assert.That(
                awaiter.IsCompleted,
                Is.False);
        }

        private static IEnumerator
            WaitForLoaderStart(
                ControlledInitialDestinationLoader loader,
                Awaitable<StartupSequenceRunResult>.Awaiter
                    awaiter)
        {
            int waitedFrames = 0;

            while ((loader.LoadCallCount == 0 ||
                    loader.FramesCompleted == 0) &&
                   !awaiter.IsCompleted &&
                   waitedFrames <
                   MaximumFramesToWait)
            {
                waitedFrames++;
                yield return null;
            }

            Assert.That(
                loader.LoadCallCount,
                Is.EqualTo(1));

            Assert.That(
                loader.FramesCompleted,
                Is.GreaterThanOrEqualTo(1));

            Assert.That(
                awaiter.IsCompleted,
                Is.False);
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
                "The destination handoff did not settle inside the bounded Play Mode window.");
        }

        private EchoLaunchRoot CreateRoot(
            EchoLaunchConfiguration configuration,
            IInitialDestinationLoader loader,
            string name = "Destination Handoff Root")
        {
            GameObject target =
                new GameObject(name);

            createdObjects.Add(target);
            target.SetActive(false);

            EchoLaunchRoot root =
                target.AddComponent<EchoLaunchRoot>();

            RootConfigurationField.SetValue(
                root,
                configuration);

            target.SetActive(true);

            if (root.IsAuthoritative)
            {
                root.SetAutomaticStartForTesting(
                    false);

                root.SetInitialDestinationLoaderForTesting(
                    loader);
            }

            return root;
        }

        private EchoLaunchConfiguration
            CreateConfiguration(
                StartupSequence sequence,
                LaunchDestination destination)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(configuration);

            ConfigurationSequenceField.SetValue(
                configuration,
                sequence);

            ConfigurationDestinationField.SetValue(
                configuration,
                destination);

            return configuration;
        }

        private LaunchDestination CreateDestination(
            string displayName =
                "Initial Destination",
            string scenePath =
                "Assets/Scenes/InitialDestination.unity")
        {
            LaunchDestination destination =
                ScriptableObject.CreateInstance<
                    LaunchDestination>();

            createdAssets.Add(destination);

            DestinationDisplayNameField.SetValue(
                destination,
                displayName);

            DestinationScenePathField.SetValue(
                destination,
                scenePath);

            return destination;
        }

        private StartupSequence CreateSequence(
            params StartupSequenceEntry[] entries)
        {
            StartupSequence sequence =
                ScriptableObject.CreateInstance<
                    StartupSequence>();

            createdAssets.Add(sequence);

            SequenceEntriesField.SetValue(
                sequence,
                new List<StartupSequenceEntry>(
                    entries));

            return sequence;
        }

        private RootLifecycleTestDefinition
            CreateDefinition(
                string displayName,
                int framesToWait = 0)
        {
            RootLifecycleTestDefinition definition =
                ScriptableObject.CreateInstance<
                    RootLifecycleTestDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                displayName);

            definition.FramesToWait =
                framesToWait;

            return definition;
        }

        private static StartupSequenceEntry CreateEntry(
            StartupStepDefinition definition,
            StartupStepPolicy? policy = null,
            bool enabled = true)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                policy ??
                StartupStepPolicy.RequiredBlocking);

            EntryActivationField.SetValue(
                entry,
                Enum.ToObject(
                    EntryActivationField.FieldType,
                    enabled
                        ? 0
                        : 1));

            return entry;
        }
    }
}

//----- LaunchDestinationAndCompletedHandoffTests.cs END -----
