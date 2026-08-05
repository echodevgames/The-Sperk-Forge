//----- StartupSequenceDefinitionTests.cs START -----

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    internal sealed class TestStartupStepExecutor :
        IStartupStepExecutor
    {
        public Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context)
        {
            throw new NotSupportedException(
                "The FL-M2-07 definition tests do not execute startup steps.");
        }
    }

    internal sealed class TestStartupStepDefinition :
        StartupStepDefinition
    {
        public override IStartupStepExecutor
            CreateExecutor()
        {
            return new TestStartupStepExecutor();
        }
    }

    public sealed class StartupSequenceDefinitionTests
    {
        private static readonly FieldInfo
            StepIdField =
                typeof(StartupStepDefinition).GetField(
                    "stepId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            StepSchemaVersionField =
                typeof(StartupStepDefinition).GetField(
                    "schemaVersion",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            StepDisplayNameField =
                typeof(StartupStepDefinition).GetField(
                    "displayName",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryIdField =
                typeof(StartupSequenceEntry).GetField(
                    "entryId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryActivationField =
                typeof(StartupSequenceEntry).GetField(
                    "activation",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            EntryStepDefinitionField =
                typeof(StartupSequenceEntry).GetField(
                    "stepDefinition",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceIdField =
                typeof(StartupSequence).GetField(
                    "sequenceId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceSchemaVersionField =
                typeof(StartupSequence).GetField(
                    "schemaVersion",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            SequenceEntriesField =
                typeof(StartupSequence).GetField(
                    "entries",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private static readonly FieldInfo
            ConfigurationStartupSequenceField =
                typeof(EchoLaunchConfiguration).GetField(
                    "startupSequence",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

        private readonly List<Object> createdAssets =
            new List<Object>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(
                StepIdField,
                Is.Not.Null);

            Assert.That(
                StepSchemaVersionField,
                Is.Not.Null);

            Assert.That(
                StepDisplayNameField,
                Is.Not.Null);

            Assert.That(
                EntryIdField,
                Is.Not.Null);

            Assert.That(
                EntryActivationField,
                Is.Not.Null);

            Assert.That(
                EntryStepDefinitionField,
                Is.Not.Null);

            Assert.That(
                SequenceIdField,
                Is.Not.Null);

            Assert.That(
                SequenceSchemaVersionField,
                Is.Not.Null);

            Assert.That(
                SequenceEntriesField,
                Is.Not.Null);

            Assert.That(
                ConfigurationStartupSequenceField,
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
        public void NewStepIdUsesCanonicalFormat()
        {
            TestStartupStepDefinition step =
                CreateStep();

            Assert.That(
                step.StepId,
                Does.Match("^[0-9a-f]{32}$"));
        }

        [Test]
        public void SeparateStepDefinitionsReceiveDifferentIds()
        {
            TestStartupStepDefinition first =
                CreateStep();

            TestStartupStepDefinition second =
                CreateStep();

            Assert.That(
                second.StepId,
                Is.Not.EqualTo(first.StepId));
        }

        [Test]
        public void StepIdRemainsStableAcrossRepeatedReads()
        {
            TestStartupStepDefinition step =
                CreateStep();

            string firstRead =
                step.StepId;

            string secondRead =
                step.StepId;

            Assert.That(
                secondRead,
                Is.EqualTo(firstRead));
        }

        [Test]
        public void NewStepUsesCurrentSchemaVersion()
        {
            TestStartupStepDefinition step =
                CreateStep();

            Assert.That(
                step.SchemaVersion,
                Is.EqualTo(
                    StartupStepDefinition
                        .CurrentSchemaVersion));
        }

        [Test]
        public void DisplayNameIsPreservedAndSeparateFromIdentity()
        {
            TestStartupStepDefinition step =
                CreateStep();

            string originalId =
                step.StepId;

            StepDisplayNameField.SetValue(
                step,
                "Initialize Audio");

            Assert.That(
                step.DisplayName,
                Is.EqualTo("Initialize Audio"));

            Assert.That(
                step.StepId,
                Is.EqualTo(originalId));
        }

        [Test]
        public void MalformedStepIdIsInvalidWithoutRepair()
        {
            TestStartupStepDefinition step =
                CreateStep();

            const string malformedId =
                "INVALID-STEP-ID";

            StepIdField.SetValue(
                step,
                malformedId);

            Assert.That(
                step.HasValidIdentity,
                Is.False);

            Assert.That(
                step.StepId,
                Is.EqualTo(malformedId));
        }

        [Test]
        public void UnsupportedStepSchemaIsUnsupportedWithoutRewrite()
        {
            TestStartupStepDefinition step =
                CreateStep();

            int unsupportedVersion =
                StartupStepDefinition
                    .CurrentSchemaVersion +
                1;

            StepSchemaVersionField.SetValue(
                step,
                unsupportedVersion);

            Assert.That(
                step.HasSupportedSchema,
                Is.False);

            Assert.That(
                step.SchemaVersion,
                Is.EqualTo(unsupportedVersion));
        }

        [Test]
        public void NewEntryIdUsesCanonicalFormat()
        {
            StartupSequenceEntry entry =
                CreateEntry();

            Assert.That(
                entry.EntryId,
                Does.Match("^[0-9a-f]{32}$"));
        }

        [Test]
        public void SeparateEntriesReceiveDifferentIds()
        {
            StartupSequenceEntry first =
                CreateEntry();

            StartupSequenceEntry second =
                CreateEntry();

            Assert.That(
                second.EntryId,
                Is.Not.EqualTo(first.EntryId));
        }

        [Test]
        public void EntryDefaultsToEnabled()
        {
            StartupSequenceEntry entry =
                CreateEntry();

            Assert.That(
                entry.IsEnabled,
                Is.True);
        }

        [Test]
        public void EntryPreservesAssignedStepReference()
        {
            TestStartupStepDefinition step =
                CreateStep();

            StartupSequenceEntry entry =
                CreateEntry(step);

            Assert.That(
                entry.StepDefinition,
                Is.SameAs(step));
        }

        [Test]
        public void MalformedEntryIdIsInvalidWithoutRepair()
        {
            StartupSequenceEntry entry =
                CreateEntry();

            const string malformedId =
                "INVALID-ENTRY-ID";

            EntryIdField.SetValue(
                entry,
                malformedId);

            Assert.That(
                entry.HasValidIdentity,
                Is.False);

            Assert.That(
                entry.EntryId,
                Is.EqualTo(malformedId));
        }

        [Test]
        public void NewSequenceIdUsesCanonicalFormat()
        {
            StartupSequence sequence =
                CreateSequence();

            Assert.That(
                sequence.SequenceId,
                Does.Match("^[0-9a-f]{32}$"));
        }

        [Test]
        public void SeparateSequencesReceiveDifferentIds()
        {
            StartupSequence first =
                CreateSequence();

            StartupSequence second =
                CreateSequence();

            Assert.That(
                second.SequenceId,
                Is.Not.EqualTo(first.SequenceId));
        }

        [Test]
        public void SequenceIdRemainsStableAcrossRepeatedReads()
        {
            StartupSequence sequence =
                CreateSequence();

            string firstRead =
                sequence.SequenceId;

            string secondRead =
                sequence.SequenceId;

            Assert.That(
                secondRead,
                Is.EqualTo(firstRead));
        }

        [Test]
        public void NewSequenceUsesCurrentSchemaVersion()
        {
            StartupSequence sequence =
                CreateSequence();

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(
                    StartupSequence
                        .CurrentSchemaVersion));
        }

        [Test]
        public void NewSequenceIdentityAndSchemaAreSupported()
        {
            StartupSequence sequence =
                CreateSequence();

            Assert.That(
                sequence.HasValidIdentity,
                Is.True);

            Assert.That(
                sequence.HasSupportedSchema,
                Is.True);
        }

        [Test]
        public void MalformedSequenceIdIsInvalidWithoutRepair()
        {
            StartupSequence sequence =
                CreateSequence();

            const string malformedId =
                "INVALID-SEQUENCE-ID";

            SequenceIdField.SetValue(
                sequence,
                malformedId);

            Assert.That(
                sequence.HasValidIdentity,
                Is.False);

            Assert.That(
                sequence.SequenceId,
                Is.EqualTo(malformedId));
        }

        [Test]
        public void UnsupportedSequenceSchemaIsUnsupportedWithoutRewrite()
        {
            StartupSequence sequence =
                CreateSequence();

            int unsupportedVersion =
                StartupSequence
                    .CurrentSchemaVersion +
                1;

            SequenceSchemaVersionField.SetValue(
                sequence,
                unsupportedVersion);

            Assert.That(
                sequence.HasSupportedSchema,
                Is.False);

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(unsupportedVersion));
        }

        [Test]
        public void EmptySequenceHasZeroEntries()
        {
            StartupSequence sequence =
                CreateSequence();

            Assert.That(
                sequence.EntryCount,
                Is.EqualTo(0));
        }

        [Test]
        public void IndexedReadsPreserveAuthoredOrder()
        {
            TestStartupStepDefinition firstStep =
                CreateStep();

            TestStartupStepDefinition secondStep =
                CreateStep();

            StartupSequenceEntry firstEntry =
                CreateEntry(firstStep);

            StartupSequenceEntry secondEntry =
                CreateEntry(secondStep);

            StartupSequence sequence =
                CreateSequence(
                    firstEntry,
                    secondEntry);

            Assert.That(
                sequence.EntryCount,
                Is.EqualTo(2));

            Assert.That(
                sequence.GetEntry(0),
                Is.SameAs(firstEntry));

            Assert.That(
                sequence.GetEntry(1),
                Is.SameAs(secondEntry));
        }

        [Test]
        public void InvalidSequenceIndexThrowsRangeException()
        {
            StartupSequence sequence =
                CreateSequence(
                    CreateEntry());

            Assert.Throws<ArgumentOutOfRangeException>(
                () => sequence.GetEntry(-1));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => sequence.GetEntry(1));
        }

        [Test]
        public void ConfigurationExposesAssignedStartupSequence()
        {
            StartupSequence sequence =
                CreateSequence();

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            Assert.That(
                configuration.StartupSequence,
                Is.SameAs(sequence));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(
                    EchoLaunchConfiguration
                        .CurrentSchemaVersion));
        }

        [Test]
        public void DefinitionLifecycleDoesNotMutateAssets()
        {
            TestStartupStepDefinition step =
                CreateStep();

            StartupSequenceEntry entry =
                CreateEntry(step);

            StartupSequence sequence =
                CreateSequence(entry);

            EchoLaunchConfiguration configuration =
                CreateConfiguration(sequence);

            string originalStepId =
                step.StepId;

            int originalStepSchema =
                step.SchemaVersion;

            string originalEntryId =
                entry.EntryId;

            bool originalEnabled =
                entry.IsEnabled;

            string originalSequenceId =
                sequence.SequenceId;

            int originalSequenceSchema =
                sequence.SchemaVersion;

            string originalConfigurationId =
                configuration.ConfigurationId;

            int originalConfigurationSchema =
                configuration.SchemaVersion;

            Assert.That(
                configuration.StartupSequence,
                Is.SameAs(sequence));

            Assert.That(
                sequence.GetEntry(0),
                Is.SameAs(entry));

            Assert.That(
                entry.StepDefinition,
                Is.SameAs(step));

            Assert.That(
                step.StepId,
                Is.EqualTo(originalStepId));

            Assert.That(
                step.SchemaVersion,
                Is.EqualTo(originalStepSchema));

            Assert.That(
                entry.EntryId,
                Is.EqualTo(originalEntryId));

            Assert.That(
                entry.IsEnabled,
                Is.EqualTo(originalEnabled));

            Assert.That(
                sequence.SequenceId,
                Is.EqualTo(originalSequenceId));

            Assert.That(
                sequence.SchemaVersion,
                Is.EqualTo(originalSequenceSchema));

            Assert.That(
                configuration.ConfigurationId,
                Is.EqualTo(originalConfigurationId));

            Assert.That(
                configuration.SchemaVersion,
                Is.EqualTo(originalConfigurationSchema));
        }

        private TestStartupStepDefinition CreateStep()
        {
            TestStartupStepDefinition step =
                ScriptableObject.CreateInstance<
                    TestStartupStepDefinition>();

            createdAssets.Add(step);

            return step;
        }

        private static StartupSequenceEntry CreateEntry(
            StartupStepDefinition stepDefinition = null)
        {
            StartupSequenceEntry entry =
                new StartupSequenceEntry();

            EntryStepDefinitionField.SetValue(
                entry,
                stepDefinition);

            return entry;
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

        private EchoLaunchConfiguration
            CreateConfiguration(
                StartupSequence sequence)
        {
            EchoLaunchConfiguration configuration =
                ScriptableObject.CreateInstance<
                    EchoLaunchConfiguration>();

            createdAssets.Add(configuration);

            ConfigurationStartupSequenceField.SetValue(
                configuration,
                sequence);

            return configuration;
        }
    }
}

//----- StartupSequenceDefinitionTests.cs END -----
