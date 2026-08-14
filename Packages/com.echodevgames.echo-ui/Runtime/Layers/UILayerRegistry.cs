using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Resolved runtime registry for project-authored layer hosts.
    /// </summary>
    public sealed class UILayerRegistry
    {
        private readonly Dictionary<string, UILayerHost> hostsById;
        private readonly List<UILayerHost> orderedHosts;

        private UILayerRegistry(
            Dictionary<string, UILayerHost> hostsById,
            List<UILayerHost> orderedHosts)
        {
            this.hostsById = hostsById;
            this.orderedHosts = orderedHosts;
        }

        public int Count =>
            orderedHosts.Count;

        public IReadOnlyList<UILayerHost> OrderedHosts =>
            orderedHosts;

        public bool TryGetHost(
            string layerId,
            out UILayerHost host)
        {
            host = null;
            string normalized =
                layerId == null
                    ? string.Empty
                    : layerId.Trim();

            return !string.IsNullOrWhiteSpace(normalized) &&
                hostsById.TryGetValue(
                    normalized,
                    out host) &&
                host != null;
        }

        public static bool TryCreate(
            IEnumerable<UILayerHost> hosts,
            out UILayerRegistry registry,
            out string error)
        {
            registry = null;
            error = string.Empty;

            if (hosts == null)
            {
                error = "Looking Glass layer hosts are required.";
                return false;
            }

            Dictionary<string, UILayerHost> byId =
                new Dictionary<string, UILayerHost>(
                    StringComparer.Ordinal);

            Dictionary<int, string> idByOrder =
                new Dictionary<int, string>();

            List<UILayerHost> ordered =
                new List<UILayerHost>();

            foreach (UILayerHost host in hosts)
            {
                if (host == null)
                {
                    error = "A Looking Glass layer host reference is missing.";
                    return false;
                }

                host.CaptureRuntimeSnapshot();

                UILayerId id =
                    host.LayerId;

                if (!id.IsValid)
                {
                    error = "Looking Glass layer IDs must be nonempty stable project-authored values.";
                    return false;
                }

                if (byId.ContainsKey(id.Value))
                {
                    error =
                        "Duplicate Looking Glass layer ID: " +
                        id.Value;
                    return false;
                }

                if (idByOrder.TryGetValue(
                        host.Order,
                        out string existingId))
                {
                    error =
                        "Looking Glass layer order " +
                        host.Order +
                        " is used by both '" +
                        existingId +
                        "' and '" +
                        id.Value +
                        "'.";
                    return false;
                }

                byId.Add(
                    id.Value,
                    host);

                idByOrder.Add(
                    host.Order,
                    id.Value);

                ordered.Add(host);
            }

            if (ordered.Count == 0)
            {
                error = "At least one project-authored Looking Glass layer host is required.";
                return false;
            }

            ordered.Sort(
                (left, right) =>
                    left.Order.CompareTo(
                        right.Order));

            registry =
                new UILayerRegistry(
                    byId,
                    ordered);

            return true;
        }
    }
}
