using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveRecoveryPlannerM504Tests
    {
        [Test]
        public void BuildRecoveryPlan_MissingProductionRoot_PerformsZeroWrites()
        {
            string rootName =
                "EchoSave-M504-Recovery-" +
                Guid.NewGuid().ToString("N");

            string absoluteRoot =
                Path.Combine(
                    Application.persistentDataPath,
                    rootName);

            EchoSaveConfiguration configuration =
                ScriptableObject.CreateInstance<EchoSaveConfiguration>();

            try
            {
                configuration.SetDefinitionForTesting(
                    EchoSaveConfiguration.CurrentSchemaVersion,
                    rootName);

                EchoSaveInspectionOpenResult open =
                    EchoSaveInspectionSession.TryOpen(
                        configuration,
                        out EchoSaveInspectionSession session);

                Assert.That(
                    open.Succeeded,
                    Is.True,
                    open.Message);

                using (session)
                {
                    SaveRecoveryPlan plan =
                        session.BuildRecoveryPlan(
                            SaveSlotId.NewId());

                    Assert.That(
                        plan.Status,
                        Is.EqualTo(
                            SaveRecoveryPlanStatus.ServiceNotReady));
                }

                Assert.That(
                    Directory.Exists(
                        absoluteRoot),
                    Is.False,
                    "Recovery Planner preview must not create a missing production root.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);

                if (Directory.Exists(
                        absoluteRoot))
                {
                    Directory.Delete(
                        absoluteRoot,
                        true);
                }
            }
        }
    }
}
