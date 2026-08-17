using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public enum UIMotifCatalogStatus
    {
        Ready = 0,
        MissingCatalog = 1,
        InvalidCapacity = 2,
        CapacityExceeded = 3,
        MissingDefinition = 4,
        DefinitionRejected = 5,
        DuplicateMotifId = 6,
        InvalidDefaultMotifId = 7,
        DefaultMotifUnavailable = 8,
        FallbackMotifUnavailable = 9
    }

    public enum UIMotifResolutionStatus
    {
        Resolved = 0,
        FallbackApplied = 1,
        Unavailable = 2
    }

    public readonly struct UIMotifCatalogResult
    {
        public UIMotifCatalogResult(
            UIMotifCatalogStatus status,
            UIMotifId motifId = default,
            UIMotifDefinitionStatus definitionStatus = UIMotifDefinitionStatus.Ready,
            int definitionCount = 0,
            UIMotifCatalogSnapshot snapshot = null,
            string message = "")
        {
            Status = status;
            MotifId = motifId;
            DefinitionStatus = definitionStatus;
            DefinitionCount = definitionCount;
            Snapshot = snapshot;
            Message = message ?? string.Empty;
        }

        public UIMotifCatalogStatus Status { get; }
        public UIMotifId MotifId { get; }
        public UIMotifDefinitionStatus DefinitionStatus { get; }
        public int DefinitionCount { get; }
        public UIMotifCatalogSnapshot Snapshot { get; }
        public string Message { get; }
        public bool Succeeded => Status == UIMotifCatalogStatus.Ready && Snapshot != null;
    }

    public readonly struct UIMotifResolutionResult
    {
        public UIMotifResolutionResult(
            UIMotifResolutionStatus status,
            UIMotifId requestedMotifId,
            UIMotifSnapshot snapshot = null)
        {
            Status = status;
            RequestedMotifId = requestedMotifId;
            Snapshot = snapshot;
        }

        public UIMotifResolutionStatus Status { get; }
        public UIMotifId RequestedMotifId { get; }
        public UIMotifSnapshot Snapshot { get; }
        public UIMotifId EffectiveMotifId => Snapshot == null ? default : Snapshot.MotifId;
        public bool Succeeded => Snapshot != null;
    }

    public sealed class UIMotifCatalogSnapshot
    {
        private readonly Dictionary<UIMotifId, UIMotifSnapshot> motifs;

        internal UIMotifCatalogSnapshot(
            UIMotifId defaultMotifId,
            UIMotifId fallbackMotifId,
            Dictionary<UIMotifId, UIMotifSnapshot> motifs)
        {
            DefaultMotifId = defaultMotifId;
            FallbackMotifId = fallbackMotifId;
            this.motifs = new Dictionary<UIMotifId, UIMotifSnapshot>(motifs);
        }

        public UIMotifId DefaultMotifId { get; }
        public UIMotifId FallbackMotifId { get; }
        public int Count => motifs.Count;

        public bool TryGet(UIMotifId motifId, out UIMotifSnapshot snapshot) =>
            motifs.TryGetValue(motifId, out snapshot);

        public UIMotifResolutionResult ResolveDefault() =>
            new UIMotifResolutionResult(
                UIMotifResolutionStatus.Resolved,
                DefaultMotifId,
                motifs[DefaultMotifId]);

        public UIMotifResolutionResult Resolve(UIMotifId requestedMotifId)
        {
            if (requestedMotifId.IsValid &&
                motifs.TryGetValue(requestedMotifId, out UIMotifSnapshot resolved))
            {
                return new UIMotifResolutionResult(
                    UIMotifResolutionStatus.Resolved,
                    requestedMotifId,
                    resolved);
            }

            if (FallbackMotifId.IsValid &&
                motifs.TryGetValue(FallbackMotifId, out UIMotifSnapshot fallback))
            {
                return new UIMotifResolutionResult(
                    UIMotifResolutionStatus.FallbackApplied,
                    requestedMotifId,
                    fallback);
            }

            return new UIMotifResolutionResult(
                UIMotifResolutionStatus.Unavailable,
                requestedMotifId);
        }
    }

    [CreateAssetMenu(
        menuName = "Echo Dev Games/Echo UI/Motif Catalog",
        fileName = "UIMotifCatalog")]
    public sealed class UIMotifCatalog : ScriptableObject
    {
        [SerializeField] private string defaultMotifId = "motif.default";
        [SerializeField] private string fallbackMotifId = "motif.default";
        [SerializeField] private UIMotifDefinition[] definitions = new UIMotifDefinition[0];

        public UIMotifId DefaultMotifId => new UIMotifId(defaultMotifId);
        public UIMotifId FallbackMotifId => new UIMotifId(fallbackMotifId);
        public int DefinitionCount => definitions == null ? 0 : definitions.Length;

        public static UIMotifCatalog CreateTransient(
            string defaultMotifId,
            string fallbackMotifId,
            UIMotifDefinition[] definitions)
        {
            UIMotifCatalog catalog = CreateInstance<UIMotifCatalog>();
            catalog.hideFlags = HideFlags.DontSave;
            catalog.defaultMotifId = new UIMotifId(defaultMotifId).Value;
            catalog.fallbackMotifId = new UIMotifId(fallbackMotifId).Value;
            catalog.definitions = definitions == null
                ? new UIMotifDefinition[0]
                : (UIMotifDefinition[])definitions.Clone();
            return catalog;
        }

        public UIMotifCatalogResult CreateSnapshot(
            int maximumDefinitionCount,
            int maximumTokenCountPerDefinition) =>
            CreateSnapshot(this, maximumDefinitionCount, maximumTokenCountPerDefinition);

        public static UIMotifCatalogResult CreateSnapshot(
            UIMotifCatalog catalog,
            int maximumDefinitionCount,
            int maximumTokenCountPerDefinition)
        {
            if (catalog == null)
                return Failure(UIMotifCatalogStatus.MissingCatalog, message: "Motif catalog is required.");

            if (maximumDefinitionCount <= 0 || maximumTokenCountPerDefinition <= 0)
                return Failure(UIMotifCatalogStatus.InvalidCapacity, message: "Catalog capacities must be positive.");

            UIMotifDefinition[] definitions = catalog.definitions == null
                ? new UIMotifDefinition[0]
                : (UIMotifDefinition[])catalog.definitions.Clone();

            if (definitions.Length > maximumDefinitionCount)
                return Failure(UIMotifCatalogStatus.CapacityExceeded, definitionCount: definitions.Length);

            UIMotifId defaultId = catalog.DefaultMotifId;
            UIMotifId fallbackId = catalog.FallbackMotifId;
            if (!defaultId.IsValid)
                return Failure(UIMotifCatalogStatus.InvalidDefaultMotifId, defaultId);

            Dictionary<UIMotifId, UIMotifSnapshot> resolved =
                new Dictionary<UIMotifId, UIMotifSnapshot>(definitions.Length);

            for (int i = 0; i < definitions.Length; i++)
            {
                UIMotifDefinition definition = definitions[i];
                if (definition == null)
                    return Failure(UIMotifCatalogStatus.MissingDefinition, definitionCount: i);

                UIMotifDefinitionResult result =
                    definition.CreateSnapshot(maximumTokenCountPerDefinition);
                if (!result.Succeeded)
                {
                    return Failure(
                        UIMotifCatalogStatus.DefinitionRejected,
                        result.MotifId,
                        result.Status,
                        i);
                }

                if (resolved.ContainsKey(result.MotifId))
                    return Failure(UIMotifCatalogStatus.DuplicateMotifId, result.MotifId, definitionCount: i + 1);

                resolved.Add(result.MotifId, result.Snapshot);
            }

            if (!resolved.ContainsKey(defaultId))
                return Failure(UIMotifCatalogStatus.DefaultMotifUnavailable, defaultId, definitionCount: resolved.Count);

            if (fallbackId.IsValid && !resolved.ContainsKey(fallbackId))
                return Failure(UIMotifCatalogStatus.FallbackMotifUnavailable, fallbackId, definitionCount: resolved.Count);

            UIMotifCatalogSnapshot snapshot =
                new UIMotifCatalogSnapshot(defaultId, fallbackId, resolved);
            return new UIMotifCatalogResult(
                UIMotifCatalogStatus.Ready,
                defaultId,
                definitionCount: snapshot.Count,
                snapshot: snapshot);
        }

        private static UIMotifCatalogResult Failure(
            UIMotifCatalogStatus status,
            UIMotifId motifId = default,
            UIMotifDefinitionStatus definitionStatus = UIMotifDefinitionStatus.Ready,
            int definitionCount = 0,
            string message = "") =>
            new UIMotifCatalogResult(
                status,
                motifId,
                definitionStatus,
                definitionCount,
                snapshot: null,
                message: message);
    }
}
