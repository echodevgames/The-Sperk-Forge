using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationTransientPlan :
        IDisposable
    {
        private readonly Object[] ownedObjects;
        private bool disposed;

        internal LaunchSimulationTransientPlan(
            LaunchSimulationPlan plan,
            EchoLaunchConfiguration configuration,
            StartupSequence sequence,
            LaunchSimulationLogicalClock clock,
            Object[] ownedObjects)
        {
            Plan =
                plan ??
                throw new ArgumentNullException(
                    nameof(plan));

            Configuration =
                configuration ??
                throw new ArgumentNullException(
                    nameof(configuration));

            Sequence =
                sequence ??
                throw new ArgumentNullException(
                    nameof(sequence));

            Clock =
                clock ??
                throw new ArgumentNullException(
                    nameof(clock));

            this.ownedObjects =
                ownedObjects ??
                throw new ArgumentNullException(
                    nameof(ownedObjects));
        }

        internal LaunchSimulationPlan Plan { get; }
        internal EchoLaunchConfiguration Configuration { get; }
        internal StartupSequence Sequence { get; }
        internal LaunchSimulationLogicalClock Clock { get; }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            for (int index = ownedObjects.Length - 1;
                 index >= 0;
                 index--)
            {
                Object item = ownedObjects[index];

                if (item != null)
                {
                    Object.DestroyImmediate(item);
                    LaunchSimulationTransientPlanBuilder
                        .NotifyDestroyed();
                }
            }
        }
    }
}
