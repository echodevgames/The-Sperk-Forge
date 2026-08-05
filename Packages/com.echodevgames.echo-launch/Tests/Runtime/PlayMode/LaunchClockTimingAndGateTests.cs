//----- LaunchClockTimingAndGateTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class ClockGateTestReporter :
        IStartupStepProgressReporter
    {
        public int ReportCount { get; private set; }

        public StartupStepProgress LatestProgress
        {
            get;
            private set;
        } =
            StartupStepProgress.Indeterminate();

        public void Report(
            StartupStepProgress progress)
        {
            ReportCount++;
            LatestProgress = progress;
        }
    }

    internal sealed class ClockGateManualClock :
        ILaunchClock
    {
        internal ClockGateManualClock(
            double initialSeconds,
            double secondsPerTick)
        {
            CurrentSeconds = initialSeconds;
            SecondsPerTick = secondsPerTick;
        }

        internal double CurrentSeconds
        {
            get;
            private set;
        }

        internal double SecondsPerTick
        {
            get;
            set;
        }

        internal int TickCount
        {
            get;
            private set;
        }

        public double NowSeconds =>
            CurrentSeconds;

#pragma warning disable CS1998
        public async Awaitable NextTickAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            TickCount++;
            CurrentSeconds += SecondsPerTick;
        }
#pragma warning restore CS1998
    }

    internal sealed class ClockGateTestExecutor :
        IStartupStepExecutor
    {
        public Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context)
        {
            throw new NotSupportedException(
                "Clock and timing tests do not invoke this executor.");
        }
    }

    internal sealed class ClockGateTestDefinition :
        StartupStepDefinition
    {
        public override IStartupStepExecutor
            CreateExecutor()
        {
            return new ClockGateTestExecutor();
        }
    }

    public sealed class LaunchClockTimingAndGateTests
    {
        private static readonly FieldInfo
            EntryDefinitionField =
                typeof(StartupSequenceEntry).GetField(
                    "stepDefinition",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryPolicyField =
                typeof(StartupSequenceEntry).GetField(
                    "policy",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            DefinitionDisplayNameField =
                typeof(StartupStepDefinition).GetField(
                    "displayName",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                EntryDefinitionField,
                Is.Not.Null);

            Assert.That(
                EntryPolicyField,
                Is.Not.Null);

            Assert.That(
                DefinitionDisplayNameField,
                Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = createdAssets.Count - 1;
                 index >= 0;
                 index--)
            {
                Object asset =
                    createdAssets[index];

                if (asset != null)
                {
                    Object.DestroyImmediate(asset);
                }
            }

            createdAssets.Clear();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void ClockInterfaceHasApprovedShape()
        {
            PropertyInfo nowProperty =
                typeof(ILaunchClock).GetProperty(
                    nameof(ILaunchClock.NowSeconds));

            MethodInfo nextTickMethod =
                typeof(ILaunchClock).GetMethod(
                    nameof(ILaunchClock
                        .NextTickAsync));

            Assert.That(nowProperty, Is.Not.Null);
            Assert.That(
                nowProperty.PropertyType,
                Is.EqualTo(typeof(double)));

            Assert.That(nextTickMethod, Is.Not.Null);
            Assert.That(
                nextTickMethod.ReturnType,
                Is.EqualTo(typeof(Awaitable)));

            ParameterInfo[] parameters =
                nextTickMethod.GetParameters();

            Assert.That(
                parameters,
                Has.Length.EqualTo(1));

            Assert.That(
                parameters[0].ParameterType,
                Is.EqualTo(
                    typeof(CancellationToken)));
        }

        [Test]
        public void UnityClockImplementsClockSeam()
        {
            ILaunchClock clock =
                UnityLaunchClock.Shared;

            Assert.That(clock, Is.Not.Null);
            Assert.That(
                clock,
                Is.InstanceOf<UnityLaunchClock>());
        }

        [Test]
        public void UnityClockValueIsFiniteAndNonnegative()
        {
            double value =
                UnityLaunchClock.Shared.NowSeconds;

            Assert.That(
                double.IsNaN(value),
                Is.False);

            Assert.That(
                double.IsInfinity(value),
                Is.False);

            Assert.That(
                value,
                Is.GreaterThanOrEqualTo(0d));
        }

        [Test]
        public void ManualClockAdvancesMonotonically()
        {
            ClockGateManualClock clock =
                new ClockGateManualClock(
                    2d,
                    0.5d);

            Awaitable first =
                clock.NextTickAsync(
                    CancellationToken.None);

            Assert.That(
                first.GetAwaiter().IsCompleted,
                Is.True);

            first.GetAwaiter().GetResult();

            Awaitable second =
                clock.NextTickAsync(
                    CancellationToken.None);

            second.GetAwaiter().GetResult();

            Assert.That(
                clock.TickCount,
                Is.EqualTo(2));

            Assert.That(
                clock.NowSeconds,
                Is.EqualTo(3d));
        }

        [Test]
        public void TimingRejectsNonfiniteStart()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new StartupStepTiming(
                        double.NaN,
                        1d,
                        0d,
                        false,
                        false));

            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new StartupStepTiming(
                        double.PositiveInfinity,
                        double.PositiveInfinity,
                        0d,
                        false,
                        false));
        }

        [Test]
        public void TimingRejectsNegativeStart()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new StartupStepTiming(
                        -0.01d,
                        1d,
                        0d,
                        false,
                        false));
        }

        [Test]
        public void TimingRejectsSettlementBeforeStart()
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new StartupStepTiming(
                        5d,
                        4.999d,
                        0d,
                        false,
                        false));
        }

        [Test]
        public void TimingDerivesElapsedSeconds()
        {
            StartupStepTiming timing =
                new StartupStepTiming(
                    1.25d,
                    4.75d,
                    0d,
                    false,
                    false);

            Assert.That(
                timing.ElapsedSeconds,
                Is.EqualTo(3.5d));
        }

        [Test]
        public void TimingSupportsDisabledTimeout()
        {
            StartupStepTiming timing =
                new StartupStepTiming(
                    2d,
                    3d,
                    0d,
                    false,
                    false);

            Assert.That(
                timing.HasTimeout,
                Is.False);

            Assert.That(
                timing.TimedOut,
                Is.False);

            Assert.That(
                timing.CancellationRequested,
                Is.False);
        }

        [Test]
        public void TimingSupportsReachedTimeout()
        {
            StartupStepTiming timing =
                new StartupStepTiming(
                    10d,
                    12.5d,
                    2d,
                    true,
                    true);

            Assert.That(
                timing.HasTimeout,
                Is.True);

            Assert.That(
                timing.TimedOut,
                Is.True);

            Assert.That(
                timing.CancellationRequested,
                Is.True);
        }

        [Test]
        public void OpenProgressGateForwardsReport()
        {
            ClockGateTestReporter reporter =
                new ClockGateTestReporter();

            StartupStepProgressGate gate =
                new StartupStepProgressGate(
                    reporter);

            gate.Report(
                StartupStepProgress.Determinate(
                    0.4f,
                    "Forwarded"));

            Assert.That(gate.IsOpen, Is.True);
            Assert.That(
                reporter.ReportCount,
                Is.EqualTo(1));

            Assert.That(
                reporter.LatestProgress.Message,
                Is.EqualTo("Forwarded"));
        }

        [Test]
        public void ClosedProgressGateIgnoresLateReport()
        {
            ClockGateTestReporter reporter =
                new ClockGateTestReporter();

            StartupStepProgressGate gate =
                new StartupStepProgressGate(
                    reporter);

            gate.Report(
                StartupStepProgress.Determinate(
                    0.2f,
                    "Before"));

            gate.Close();

            Assert.DoesNotThrow(
                () =>
                    gate.Report(
                        StartupStepProgress
                            .Determinate(
                                0.9f,
                                "Late")));

            Assert.That(gate.IsOpen, Is.False);
            Assert.That(
                reporter.ReportCount,
                Is.EqualTo(1));

            Assert.That(
                reporter.LatestProgress.Message,
                Is.EqualTo("Before"));
        }

        [Test]
        public void ProgressGateCloseIsIdempotent()
        {
            StartupStepProgressGate gate =
                new StartupStepProgressGate(
                    new ClockGateTestReporter());

            Assert.DoesNotThrow(
                () =>
                {
                    gate.Close();
                    gate.Close();
                    gate.Close();
                });

            Assert.That(gate.IsOpen, Is.False);
        }

        [Test]
        public void ExecutionCapturesTimingExactlyOnce()
        {
            StartupStepExecution execution =
                CreateExecution();

            Assert.That(
                execution.HasTiming,
                Is.False);

            Assert.Throws<
                InvalidOperationException>(
                () =>
                {
                    StartupStepTiming unused =
                        execution.Timing;
                });

            execution.Begin();

            StartupStepTiming timing =
                new StartupStepTiming(
                    5d,
                    7.5d,
                    3d,
                    false,
                    false);

            StartupStepResult result =
                StartupStepResult.Success(
                    "Timed success");

            execution.Complete(
                result,
                timing);

            Assert.That(
                execution.HasTiming,
                Is.True);

            Assert.That(
                execution.Timing.ElapsedSeconds,
                Is.EqualTo(2.5d));

            Assert.That(
                execution.Result,
                Is.SameAs(result));

            Assert.Throws<
                InvalidOperationException>(
                () =>
                    execution.Complete(
                        StartupStepResult.Success(),
                        timing));
        }

        private StartupStepExecution CreateExecution()
        {
            ClockGateTestDefinition definition =
                ScriptableObject.CreateInstance<
                    ClockGateTestDefinition>();

            createdAssets.Add(definition);

            DefinitionDisplayNameField.SetValue(
                definition,
                "Timing Step");

            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryDefinitionField.SetValue(
                entry,
                definition);

            EntryPolicyField.SetValue(
                entry,
                StartupStepPolicy
                    .RequiredBlocking);

            return new StartupStepExecution(
                entry,
                0,
                1,
                new ClockGateTestExecutor());
        }
    }
}

//----- LaunchClockTimingAndGateTests.cs END -----
