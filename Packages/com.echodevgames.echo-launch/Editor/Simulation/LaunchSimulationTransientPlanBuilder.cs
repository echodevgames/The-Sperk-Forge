using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal static class LaunchSimulationTransientPlanBuilder
    {
        private static int liveObjectCount;

        internal static int LiveObjectCount =>
            Volatile.Read(ref liveObjectCount);

        internal static LaunchSimulationTransientPlan Build(
            LaunchSimulationPlan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(
                    nameof(plan));
            }

            List<Object> owned =
                new List<Object>();

            try
            {
                LaunchSimulationStepDefinition[] definitions =
                    new LaunchSimulationStepDefinition[
                        plan.StepCount];

                for (int index = 0;
                     index < definitions.Length;
                     index++)
                {
                    LaunchSimulationStepPlan step =
                        plan.GetStep(index);

                    LaunchSimulationStepDefinition definition =
                        CreateTracked<
                            LaunchSimulationStepDefinition>(
                            owned);

                    ConfigureDefinition(
                        definition,
                        step);

                    definition.Configure(step);
                    definitions[index] = definition;
                }

                StartupSequence sequence =
                    CreateTracked<StartupSequence>(owned);

                ConfigureSequence(
                    sequence,
                    plan,
                    definitions);

                EchoLaunchConfiguration configuration =
                    CreateTracked<EchoLaunchConfiguration>(
                        owned);

                ConfigureConfiguration(
                    configuration,
                    sequence,
                    plan.Request.RequestFingerprint);

                LaunchSimulationLogicalClock clock =
                    new LaunchSimulationLogicalClock(
                        plan.ClockTickSeconds);

                return new LaunchSimulationTransientPlan(
                    plan,
                    configuration,
                    sequence,
                    clock,
                    owned.ToArray());
            }
            catch
            {
                DestroyOwnedObjects(owned);
                throw;
            }
        }

        internal static void NotifyDestroyed()
        {
            Interlocked.Decrement(ref liveObjectCount);
        }

        private static T CreateTracked<T>(
            ICollection<Object> owned)
            where T : ScriptableObject
        {
            T item = ScriptableObject.CreateInstance<T>();
            item.hideFlags = HideFlags.HideAndDontSave;
            owned.Add(item);
            Interlocked.Increment(ref liveObjectCount);
            return item;
        }

        private static void DestroyOwnedObjects(
            IList<Object> owned)
        {
            for (int index = owned.Count - 1;
                 index >= 0;
                 index--)
            {
                Object item = owned[index];

                if (item != null)
                {
                    Object.DestroyImmediate(item);
                    NotifyDestroyed();
                }
            }
        }

        private static void ConfigureDefinition(
            LaunchSimulationStepDefinition definition,
            LaunchSimulationStepPlan step)
        {
            SerializedObject serialized =
                new SerializedObject(definition);

            RequireProperty(
                    serialized,
                    "stepId")
                .stringValue = step.StepId;

            RequireProperty(
                    serialized,
                    "schemaVersion")
                .intValue =
                    StartupStepDefinition.CurrentSchemaVersion;

            RequireProperty(
                    serialized,
                    "displayName")
                .stringValue = step.DisplayName;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureSequence(
            StartupSequence sequence,
            LaunchSimulationPlan plan,
            LaunchSimulationStepDefinition[] definitions)
        {
            SerializedObject serialized =
                new SerializedObject(sequence);

            RequireProperty(
                    serialized,
                    "sequenceId")
                .stringValue =
                    LaunchSimulationFingerprint.StableId(
                        plan.Request.RequestFingerprint +
                        "|Sequence");

            RequireProperty(
                    serialized,
                    "schemaVersion")
                .intValue =
                    StartupSequence.CurrentSchemaVersion;

            SerializedProperty entries =
                RequireProperty(serialized, "entries");

            entries.arraySize = plan.StepCount;

            for (int index = 0;
                 index < plan.StepCount;
                 index++)
            {
                LaunchSimulationStepPlan step =
                    plan.GetStep(index);

                SerializedProperty entry =
                    entries.GetArrayElementAtIndex(index);

                RequireRelative(entry, "entryId")
                    .stringValue = step.EntryId;

                RequireRelative(entry, "activation")
                    .enumValueIndex = 0;

                RequireRelative(entry, "stepDefinition")
                    .objectReferenceValue = definitions[index];

                SerializedProperty policy =
                    RequireRelative(entry, "policy");

                RequireRelative(policy, "requirement")
                    .enumValueIndex =
                        step.IsRequired ? 0 : 1;

                RequireRelative(policy, "failureAction")
                    .enumValueIndex =
                        (int)step.FailureAction;

                RequireRelative(policy, "timeoutSeconds")
                    .floatValue = (float)step.TimeoutSeconds;

                RequireRelative(policy, "cancellation")
                    .enumValueIndex =
                        step.SupportsCancellation ? 0 : 1;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureConfiguration(
            EchoLaunchConfiguration configuration,
            StartupSequence sequence,
            string requestFingerprint)
        {
            SerializedObject serialized =
                new SerializedObject(configuration);

            RequireProperty(
                    serialized,
                    "configurationId")
                .stringValue =
                    LaunchSimulationFingerprint.StableId(
                        requestFingerprint +
                        "|Configuration");

            RequireProperty(
                    serialized,
                    "schemaVersion")
                .intValue =
                    EchoLaunchConfiguration.CurrentSchemaVersion;

            RequireProperty(
                    serialized,
                    "startupSequence")
                .objectReferenceValue = sequence;

            RequireProperty(
                    serialized,
                    "initialDestination")
                .objectReferenceValue = null;

            RequireProperty(
                    serialized,
                    "splashSequence")
                .objectReferenceValue = null;

            RequireProperty(
                    serialized,
                    "useReducedMotionForSplash")
                .boolValue = false;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static SerializedProperty RequireProperty(
            SerializedObject serialized,
            string propertyName)
        {
            SerializedProperty property =
                serialized.FindProperty(propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Transient Launch Simulator authoring could not find serialized property '{0}'.",
                        propertyName));
            }

            return property;
        }

        private static SerializedProperty RequireRelative(
            SerializedProperty property,
            string relativeName)
        {
            SerializedProperty relative =
                property.FindPropertyRelative(relativeName);

            if (relative == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Transient Launch Simulator authoring could not find relative serialized property '{0}'.",
                        relativeName));
            }

            return relative;
        }
    }
}
