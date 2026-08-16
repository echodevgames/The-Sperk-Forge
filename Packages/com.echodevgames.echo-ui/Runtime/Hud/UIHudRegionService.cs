using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public sealed class UIHudRegionService
    {
        private sealed class RegionEntry
        {
            public UIHudRegionHost Host;
            public UIHudRegionDefinition Definition;
            public long Generation;
            public UnityEngine.Object Owner;
            public bool HasOwner;
            public UIHudOwnershipMode OwnershipMode;
            public bool EffectiveVisibility;
            public readonly Dictionary<string, WidgetEntry> Widgets =
                new Dictionary<string, WidgetEntry>(StringComparer.Ordinal);
            public readonly Dictionary<long, VisibilityEntry> Visibility =
                new Dictionary<long, VisibilityEntry>();
        }

        private sealed class WidgetEntry
        {
            public UIHudWidgetId WidgetId;
            public UISurface View;
            public UnityEngine.Object Owner;
            public bool HasOwner;
            public int Order;
            public long Generation;
        }

        private sealed class VisibilityEntry
        {
            public long Generation;
            public string ReasonId;
            public bool Visible;
            public int Priority;
            public UnityEngine.Object Owner;
            public bool HasOwner;
        }

        private readonly Dictionary<string, RegionEntry> regions =
            new Dictionary<string, RegionEntry>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> regionGeneration =
            new Dictionary<string, long>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> widgetGeneration =
            new Dictionary<string, long>(StringComparer.Ordinal);

        private readonly int globalWidgetCapacity;
        private readonly int visibilityCapacity;
        private long nextVisibilityGeneration;
        private bool shutdown;

        public UIHudRegionService(
            int globalWidgetCapacity = 64,
            int visibilityCapacity = 64)
        {
            this.globalWidgetCapacity =
                Mathf.Max(1, globalWidgetCapacity);
            this.visibilityCapacity =
                Mathf.Max(1, visibilityCapacity);
        }

        public bool IsValid => !shutdown;
        public int RegionCount => regions.Count;

        public int WidgetCount
        {
            get
            {
                int count = 0;
                foreach (RegionEntry entry in regions.Values)
                {
                    count += entry.Widgets.Count;
                }
                return count;
            }
        }

        public int VisibilityLeaseCount
        {
            get
            {
                int count = 0;
                foreach (RegionEntry entry in regions.Values)
                {
                    count += entry.Visibility.Count;
                }
                return count;
            }
        }

        public event Action<UIHudRegionSnapshot> RegionChanged;

        public UIHudOperationResult RegisterRegion(
            UIHudRegionHost host,
            UnityEngine.Object owner = null,
            UIHudOwnershipMode ownershipMode = UIHudOwnershipMode.ExternalOwned)
        {
            if (shutdown)
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Shutdown,
                    message: "HUD region service is shut down.");
            }

            if (host == null)
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Invalid,
                    message: "HUD region host is missing.");
            }

            host.CaptureRuntimeSnapshot();

            UIHudRegionDefinition definition =
                host.Definition;
            UIHudRegionId regionId =
                definition == null
                    ? new UIHudRegionId(string.Empty)
                    : definition.RegionId;

            if (!regionId.IsValid)
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Invalid,
                    regionId,
                    message: "HUD region stable ID must be nonempty.");
            }

            if (regions.ContainsKey(regionId.Value))
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Duplicate,
                    regionId,
                    message: "Duplicate HUD region stable ID '" + regionId.Value + "'.");
            }

            long generation =
                NextGeneration(
                    regionGeneration,
                    regionId.Value);

            RegionEntry entry =
                new RegionEntry
                {
                    Host = host,
                    Definition = definition.Snapshot(),
                    Generation = generation,
                    Owner = owner != null ? owner : host,
                    HasOwner = true,
                    OwnershipMode = ownershipMode,
                    EffectiveVisibility = definition.StartVisible
                };

            regions.Add(regionId.Value, entry);
            host.SetVisible(entry.EffectiveVisibility);
            Publish(entry);

            return UIHudOperationResult.Success(
                regionId,
                generation: generation,
                message: "HUD region registered.");
        }

        public UIHudOperationResult UnregisterRegion(
            UIHudRegionId regionId,
            long generation = 0)
        {
            if (!TryGetRegion(regionId, out RegionEntry entry, out UIHudOperationResult failure))
            {
                return failure;
            }

            if (generation > 0 &&
                entry.Generation != generation)
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Stale,
                    regionId,
                    generation: generation,
                    message: "HUD region generation is stale.");
            }

            regions.Remove(regionId.Value);
            entry.Widgets.Clear();
            entry.Visibility.Clear();

            return UIHudOperationResult.Success(
                regionId,
                generation: entry.Generation,
                message: "HUD region unregistered.");
        }

        public UIHudWidgetHandle RegisterWidget(
            UIHudRegionId regionId,
            UIHudWidgetId widgetId,
            UISurface view,
            UnityEngine.Object owner = null,
            int order = 0)
        {
            if (!TryGetRegion(regionId, out RegionEntry region, out UIHudOperationResult failure))
            {
                return UIHudWidgetHandle.Rejected(
                    regionId,
                    widgetId,
                    failure.Status,
                    failure.Message);
            }

            if (!widgetId.IsValid ||
                view == null ||
                view.Role != UISurfaceRole.Hud)
            {
                return UIHudWidgetHandle.Rejected(
                    regionId,
                    widgetId,
                    UIHudOperationStatus.Invalid,
                    "HUD widget requires a nonempty stable ID and a live UISurface with Hud role.");
            }

            if (region.Widgets.ContainsKey(widgetId.Value))
            {
                return UIHudWidgetHandle.Rejected(
                    regionId,
                    widgetId,
                    UIHudOperationStatus.Duplicate,
                    "Duplicate HUD widget ID in region.");
            }

            if (WidgetCount >= globalWidgetCapacity ||
                region.Widgets.Count >= region.Definition.WidgetCapacity)
            {
                return UIHudWidgetHandle.Rejected(
                    regionId,
                    widgetId,
                    UIHudOperationStatus.CapacityExceeded,
                    "HUD widget capacity is full.");
            }

            string generationKey =
                regionId.Value + "|" + widgetId.Value;

            long generation =
                NextGeneration(
                    widgetGeneration,
                    generationKey);

            region.Widgets.Add(
                widgetId.Value,
                new WidgetEntry
                {
                    WidgetId = widgetId,
                    View = view,
                    Owner = owner != null ? owner : view,
                    HasOwner = true,
                    Order = order,
                    Generation = generation
                });

            Publish(region);

            UIHudOperationResult result =
                UIHudOperationResult.Success(
                    regionId,
                    widgetId,
                    generation,
                    "HUD widget registered.");

            return new UIHudWidgetHandle(
                this,
                regionId,
                widgetId,
                generation,
                true,
                result);
        }

        public UIHudVisibilityLease RequestVisibility(
            UIHudRegionId regionId,
            string reasonId,
            bool visible,
            int priority = 0,
            UnityEngine.Object owner = null)
        {
            if (!TryGetRegion(regionId, out RegionEntry region, out UIHudOperationResult failure))
            {
                return UIHudVisibilityLease.Rejected(
                    regionId,
                    reasonId,
                    failure.Status,
                    failure.Message);
            }

            string normalizedReason =
                reasonId == null
                    ? string.Empty
                    : reasonId.Trim();

            if (string.IsNullOrWhiteSpace(normalizedReason))
            {
                return UIHudVisibilityLease.Rejected(
                    regionId,
                    normalizedReason,
                    UIHudOperationStatus.Invalid,
                    "HUD visibility reason ID must be nonempty.");
            }

            if (VisibilityLeaseCount >= visibilityCapacity)
            {
                return UIHudVisibilityLease.Rejected(
                    regionId,
                    normalizedReason,
                    UIHudOperationStatus.CapacityExceeded,
                    "HUD visibility lease capacity is full.");
            }

            long generation =
                ++nextVisibilityGeneration;

            region.Visibility.Add(
                generation,
                new VisibilityEntry
                {
                    Generation = generation,
                    ReasonId = normalizedReason,
                    Visible = visible,
                    Priority = priority,
                    Owner = owner,
                    HasOwner = owner != null
                });

            Recalculate(region);

            UIHudOperationResult result =
                UIHudOperationResult.Success(
                    regionId,
                    generation: generation,
                    message: "HUD visibility lease acquired.");

            return new UIHudVisibilityLease(
                this,
                regionId,
                normalizedReason,
                generation,
                true,
                result);
        }

        internal UIHudOperationResult ReleaseWidget(
            UIHudRegionId regionId,
            UIHudWidgetId widgetId,
            long generation)
        {
            if (!TryGetRegion(regionId, out RegionEntry region, out UIHudOperationResult failure))
            {
                return failure;
            }

            if (!region.Widgets.TryGetValue(widgetId.Value, out WidgetEntry entry))
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Stale,
                    regionId,
                    widgetId,
                    generation,
                    "HUD widget generation is no longer live.");
            }

            if (entry.Generation != generation)
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Stale,
                    regionId,
                    widgetId,
                    generation,
                    "HUD widget handle cannot release a newer generation.");
            }

            region.Widgets.Remove(widgetId.Value);
            Publish(region);

            return UIHudOperationResult.Success(
                regionId,
                widgetId,
                generation,
                "HUD widget released.");
        }

        internal UIHudOperationResult ReleaseVisibility(
            UIHudRegionId regionId,
            long generation)
        {
            if (!TryGetRegion(regionId, out RegionEntry region, out UIHudOperationResult failure))
            {
                return failure;
            }

            if (!region.Visibility.Remove(generation))
            {
                return new UIHudOperationResult(
                    UIHudOperationStatus.Stale,
                    regionId,
                    generation: generation,
                    message: "HUD visibility lease is no longer live.");
            }

            Recalculate(region);

            return UIHudOperationResult.Success(
                regionId,
                generation: generation,
                message: "HUD visibility lease released.");
        }

        public bool TryGetSnapshot(
            UIHudRegionId regionId,
            out UIHudRegionSnapshot snapshot)
        {
            snapshot = default;

            if (!regionId.IsValid ||
                !regions.TryGetValue(regionId.Value, out RegionEntry entry) ||
                entry.Host == null)
            {
                return false;
            }

            snapshot = CreateSnapshot(entry);
            return true;
        }

        public void RefreshDestroyedOwners()
        {
            if (shutdown)
            {
                return;
            }

            List<string> deadRegions =
                new List<string>();

            foreach (KeyValuePair<string, RegionEntry> pair in regions)
            {
                RegionEntry region = pair.Value;

                if (region.Host == null ||
                    region.HasOwner &&
                    region.Owner == null)
                {
                    deadRegions.Add(pair.Key);
                    continue;
                }

                List<string> deadWidgets =
                    new List<string>();

                foreach (KeyValuePair<string, WidgetEntry> widgetPair in region.Widgets)
                {
                    WidgetEntry widget = widgetPair.Value;
                    if (widget.View == null ||
                        widget.HasOwner &&
                        widget.Owner == null)
                    {
                        deadWidgets.Add(widgetPair.Key);
                    }
                }

                for (int index = 0; index < deadWidgets.Count; index++)
                {
                    region.Widgets.Remove(deadWidgets[index]);
                }

                List<long> deadVisibility =
                    new List<long>();

                foreach (KeyValuePair<long, VisibilityEntry> visibilityPair in region.Visibility)
                {
                    VisibilityEntry visibility = visibilityPair.Value;
                    if (visibility.HasOwner &&
                        visibility.Owner == null)
                    {
                        deadVisibility.Add(visibilityPair.Key);
                    }
                }

                for (int index = 0; index < deadVisibility.Count; index++)
                {
                    region.Visibility.Remove(deadVisibility[index]);
                }

                if (deadWidgets.Count > 0 ||
                    deadVisibility.Count > 0)
                {
                    Recalculate(region);
                }
            }

            for (int index = 0; index < deadRegions.Count; index++)
            {
                regions.Remove(deadRegions[index]);
            }
        }

        public void Shutdown()
        {
            if (shutdown)
            {
                return;
            }

            shutdown = true;
            regions.Clear();
            regionGeneration.Clear();
            widgetGeneration.Clear();
            RegionChanged = null;
        }

        private bool TryGetRegion(
            UIHudRegionId regionId,
            out RegionEntry entry,
            out UIHudOperationResult failure)
        {
            entry = null;

            if (shutdown)
            {
                failure = new UIHudOperationResult(
                    UIHudOperationStatus.Shutdown,
                    regionId,
                    message: "HUD region service is shut down.");
                return false;
            }

            if (!regionId.IsValid)
            {
                failure = new UIHudOperationResult(
                    UIHudOperationStatus.Invalid,
                    regionId,
                    message: "HUD region stable ID must be nonempty.");
                return false;
            }

            if (!regions.TryGetValue(regionId.Value, out entry) ||
                entry.Host == null)
            {
                failure = new UIHudOperationResult(
                    UIHudOperationStatus.UnknownRegion,
                    regionId,
                    message: "HUD region is not registered.");
                return false;
            }

            failure = default;
            return true;
        }

        private void Recalculate(RegionEntry region)
        {
            bool effective =
                region.Definition.StartVisible;

            VisibilityEntry winner = null;

            foreach (VisibilityEntry candidate in region.Visibility.Values)
            {
                if (winner == null ||
                    candidate.Priority > winner.Priority ||
                    candidate.Priority == winner.Priority &&
                    candidate.Generation > winner.Generation)
                {
                    winner = candidate;
                }
            }

            if (winner != null)
            {
                effective = winner.Visible;
            }

            region.EffectiveVisibility = effective;

            if (region.Host != null)
            {
                region.Host.SetVisible(effective);
            }

            Publish(region);
        }

        private void Publish(RegionEntry region)
        {
            Action<UIHudRegionSnapshot> handlers =
                RegionChanged;

            if (handlers == null)
            {
                return;
            }

            UIHudRegionSnapshot snapshot =
                CreateSnapshot(region);

            Delegate[] invocationList =
                handlers.GetInvocationList();

            for (int index = 0; index < invocationList.Length; index++)
            {
                try
                {
                    ((Action<UIHudRegionSnapshot>)invocationList[index])(
                        snapshot);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static UIHudRegionSnapshot CreateSnapshot(
            RegionEntry region) =>
            new UIHudRegionSnapshot(
                region.Definition.RegionId,
                region.Generation,
                region.EffectiveVisibility,
                region.Widgets.Count,
                region.Visibility.Count,
                region.OwnershipMode);

        private static long NextGeneration(
            Dictionary<string, long> generations,
            string key)
        {
            generations.TryGetValue(key, out long current);
            long next = current + 1;
            generations[key] = next;
            return next;
        }
    }
}
