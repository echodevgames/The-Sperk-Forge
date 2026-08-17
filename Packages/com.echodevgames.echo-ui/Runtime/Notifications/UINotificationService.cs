using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Root-owned bounded notification admission and channel state.
    /// Events and root wiring are added by later EUI-M4-02 slices.
    /// </summary>
    public sealed class UINotificationService
    {
        private sealed class Entry
        {
            public Entry(
                UINotificationRequest request,
                UINotificationHandle handle,
                long admissionSequence,
                double lifetimeSeconds)
            {
                Request = request;
                Handle = handle;
                AdmissionSequence = admissionSequence;
                LifetimeSeconds = lifetimeSeconds;
            }

            public UINotificationRequest Request { get; }

            public UINotificationHandle Handle { get; }

            public long AdmissionSequence { get; }

            public double LifetimeSeconds { get; }

            public bool HasVisibleLifetimeStart { get; private set; }

            public double VisibleSinceSeconds { get; private set; }

            public int Priority =>
                Request.Priority;

            public bool UsesAutomaticLifetime =>
                Request.LifetimeMode ==
                UINotificationLifetimeMode.Automatic;

            public bool HasLostOwner =>
                Request.HasOwner &&
                Request.Owner == null;

            public void BeginVisibleLifetime(
                double nowSeconds)
            {
                if (!UsesAutomaticLifetime ||
                    HasVisibleLifetimeStart)
                {
                    return;
                }

                VisibleSinceSeconds = nowSeconds;
                HasVisibleLifetimeStart = true;
            }

            public bool IsExpired(
                double nowSeconds) =>
                UsesAutomaticLifetime &&
                HasVisibleLifetimeStart &&
                nowSeconds - VisibleSinceSeconds >=
                LifetimeSeconds;
        }

        private sealed class ChannelState
        {
            public ChannelState(
                UINotificationChannelDefinition definition)
            {
                Definition = definition;
                Visible =
                    new List<Entry>(
                        definition.VisibleCapacity);

                Pending =
                    new List<Entry>(
                        definition.PendingCapacity);
            }

            public UINotificationChannelDefinition Definition { get; }

            public List<Entry> Visible { get; }

            public List<Entry> Pending { get; }
        }

        private readonly Dictionary<string, ChannelState> channels =
            new Dictionary<string, ChannelState>(
                StringComparer.Ordinal);

        private readonly IUINotificationClock clock;

        private long nextGeneration;
        private long nextAdmissionSequence;
        private double lastObservedNowSeconds;
        private bool mutationInProgress;
        private bool shutdown;

        public UINotificationService(
            IEnumerable<UINotificationChannelDefinition> definitions,
            out string validationError)
            : this(
                definitions,
                UnityUINotificationClock.Shared,
                out validationError)
        {
        }

        public UINotificationService(
            IEnumerable<UINotificationChannelDefinition> definitions,
            IUINotificationClock clock,
            out string validationError)
        {
            this.clock = clock;

            if (!TryReadInitialClock(
                    clock,
                    out lastObservedNowSeconds))
            {
                validationError =
                    "Notification clock must supply finite nonnegative monotonic seconds.";
                return;
            }

            validationError =
                ValidateAndSnapshotDefinitions(definitions);
        }

        public bool IsValid { get; private set; }

        public bool IsShutdown =>
            shutdown;

        public int ChannelCount =>
            channels.Count;

        public int VisibleCount
        {
            get
            {
                int count = 0;

                foreach (ChannelState channel in channels.Values)
                {
                    count += channel.Visible.Count;
                }

                return count;
            }
        }

        public int PendingCount
        {
            get
            {
                int count = 0;

                foreach (ChannelState channel in channels.Values)
                {
                    count += channel.Pending.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Publishes payload-free channel truth after an accepted mutation has
        /// fully committed. No event history is retained by the service.
        /// </summary>
        public event Action<UINotificationChannelSnapshot> ChannelChanged;

        public UINotificationHandle Admit(
            UINotificationRequest request)
        {
            long generation =
                ++nextGeneration;

            if (shutdown)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Shutdown,
                    "Notification service is shut down.");
            }

            if (mutationInProgress)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Unavailable,
                    "Notification mutation is already settling committed state.");
            }

            if (!IsValid)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Unavailable,
                    "Notification service is unavailable.");
            }

            if (request == null)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Invalid,
                    "Notification request is missing.");
            }

            if (!request.ChannelId.IsValid)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Invalid,
                    "Notification request requires a nonempty channel ID.");
            }

            if (!channels.TryGetValue(
                    request.ChannelId.Value,
                    out ChannelState channel))
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.UnknownChannel,
                    "Notification channel is not registered.");
            }

            string requestError =
                ValidateRequest(request);

            if (!string.IsNullOrWhiteSpace(requestError))
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.Invalid,
                    requestError);
            }

            if (request.CoalescingKey.IsValid &&
                TryFindCoalescingEntry(
                    channel,
                    request.CoalescingKey,
                    out bool coalescingVisible,
                    out int coalescingIndex))
            {
                return Coalesce(
                    channel,
                    request,
                    generation,
                    coalescingVisible,
                    coalescingIndex);
            }

            bool hasVisibleCapacity =
                channel.Visible.Count <
                channel.Definition.VisibleCapacity;

            bool hasPendingCapacity =
                channel.Pending.Count <
                channel.Definition.PendingCapacity;

            if (!hasVisibleCapacity &&
                !hasPendingCapacity)
            {
                return ApplyOverflow(
                    channel,
                    request,
                    generation);
            }

            long admissionSequence =
                ++nextAdmissionSequence;

            UINotificationAdmissionResult admission =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.Admitted,
                    request.ChannelId,
                    generation,
                    request.CoalescingKey,
                    request.CorrelationId,
                    "Notification generation admitted.");

            UINotificationHandle handle =
                new UINotificationHandle(admission);

            Entry entry =
                new Entry(
                    request,
                    handle,
                    admissionSequence,
                    ResolveLifetimeSeconds(
                        channel,
                        request));

            if (hasVisibleCapacity)
            {
                entry.BeginVisibleLifetime(
                    ObserveNowOrLast());
            }

            mutationInProgress = true;

            try
            {
                if (hasVisibleCapacity)
                {
                    channel.Visible.Add(entry);
                }
                else
                {
                    channel.Pending.Add(entry);
                }

                Publish(channel);
            }
            finally
            {
                mutationInProgress = false;
            }

            return handle;
        }

        /// <summary>
        /// Settles every visible automatic entry whose unscaled lifetime has
        /// elapsed, then promotes pending work deterministically.
        /// </summary>
        public int Tick()
        {
            if (shutdown ||
                !IsValid ||
                mutationInProgress ||
                !TryObserveNow(
                    out double nowSeconds))
            {
                return 0;
            }

            int expiredCount = 0;
            mutationInProgress = true;

            try
            {
                foreach (ChannelState channel in channels.Values)
                {
                    List<Entry> expiredEntries =
                        null;

                    int index = 0;

                    while (index < channel.Visible.Count)
                    {
                        Entry entry =
                            channel.Visible[index];

                        if (!entry.IsExpired(nowSeconds))
                        {
                            index++;
                            continue;
                        }

                        channel.Visible.RemoveAt(index);

                        if (expiredEntries == null)
                        {
                            expiredEntries =
                                new List<Entry>();
                        }

                        expiredEntries.Add(entry);
                    }

                    if (expiredEntries == null)
                    {
                        continue;
                    }

                    Promote(
                        channel,
                        nowSeconds);

                    CompleteEntries(
                        expiredEntries,
                        UINotificationOutcome.Expired,
                        "Notification generation reached its visible automatic lifetime.");

                    expiredCount +=
                        expiredEntries.Count;

                    Publish(channel);
                }
            }
            finally
            {
                mutationInProgress = false;
            }

            return expiredCount;
        }

        /// <summary>
        /// Settles generations whose captured Unity owner was destroyed, then
        /// promotes pending work independently within each affected channel.
        /// </summary>
        public int RefreshDestroyedOwners()
        {
            if (shutdown ||
                !IsValid ||
                mutationInProgress)
            {
                return 0;
            }

            double nowSeconds =
                ObserveNowOrLast();

            List<Entry> lostEntries =
                null;

            List<ChannelState> changedChannels =
                null;

            mutationInProgress = true;

            try
            {
                foreach (ChannelState channel in channels.Values)
                {
                    int removedCount =
                        RemoveLostOwners(
                            channel.Visible,
                            ref lostEntries);

                    removedCount +=
                        RemoveLostOwners(
                            channel.Pending,
                            ref lostEntries);

                    if (removedCount == 0)
                    {
                        continue;
                    }

                    Promote(
                        channel,
                        nowSeconds);

                    if (changedChannels == null)
                    {
                        changedChannels =
                            new List<ChannelState>();
                    }

                    changedChannels.Add(channel);
                }

                if (lostEntries == null)
                {
                    return 0;
                }

                CompleteEntries(
                    lostEntries,
                    UINotificationOutcome.OwnerLost,
                    "Notification generation owner was destroyed.");

                for (int index = 0;
                     index < changedChannels.Count;
                     index++)
                {
                    Publish(
                        changedChannels[index]);
                }
            }
            finally
            {
                mutationInProgress = false;
            }

            return lostEntries == null
                ? 0
                : lostEntries.Count;
        }

        /// <summary>
        /// Settles all live generations while preserving immutable channel
        /// definitions and monotonic generation identity for reuse.
        /// </summary>
        public int Reset()
        {
            if (shutdown ||
                !IsValid ||
                mutationInProgress)
            {
                return 0;
            }

            int liveCount =
                VisibleCount + PendingCount;

            if (liveCount == 0)
            {
                return 0;
            }

            List<Entry> resetEntries =
                new List<Entry>(
                    liveCount);

            List<ChannelState> changedChannels =
                new List<ChannelState>();

            foreach (ChannelState channel in channels.Values)
            {
                if (channel.Visible.Count > 0 ||
                    channel.Pending.Count > 0)
                {
                    changedChannels.Add(channel);
                }
            }

            mutationInProgress = true;

            try
            {
                DrainAllEntries(
                    resetEntries);

                CompleteEntries(
                    resetEntries,
                    UINotificationOutcome.Reset,
                    "Notification generation was settled by service reset.");

                for (int index = 0;
                     index < changedChannels.Count;
                     index++)
                {
                    Publish(
                        changedChannels[index]);
                }
            }
            finally
            {
                mutationInProgress = false;
            }

            return resetEntries.Count;
        }

        /// <summary>
        /// Settles all live generations exactly once, releases channel state,
        /// and permanently rejects further mutation.
        /// </summary>
        public int Shutdown()
        {
            if (shutdown ||
                !IsValid ||
                mutationInProgress)
            {
                return 0;
            }

            List<Entry> shutdownEntries =
                new List<Entry>(
                    VisibleCount + PendingCount);

            List<ChannelState> shutdownChannels =
                new List<ChannelState>(
                    channels.Values);

            shutdown = true;
            mutationInProgress = true;

            try
            {
                DrainAllEntries(
                    shutdownEntries);

                channels.Clear();
                IsValid = false;

                CompleteEntries(
                    shutdownEntries,
                    UINotificationOutcome.Shutdown,
                    "Notification generation was settled by service shutdown.");

                for (int index = 0;
                     index < shutdownChannels.Count;
                     index++)
                {
                    Publish(
                        shutdownChannels[index]);
                }

                ChannelChanged = null;
            }
            finally
            {
                mutationInProgress = false;
            }

            return shutdownEntries.Count;
        }

        private UINotificationHandle ApplyOverflow(
            ChannelState channel,
            UINotificationRequest request,
            long generation)
        {
            if (channel.Definition.OverflowPolicy ==
                UINotificationOverflowPolicy.RejectNewest)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.CapacityExceeded,
                    "Notification channel visible and pending capacity is full.");
            }

            if (channel.Pending.Count == 0)
            {
                return Reject(
                    request,
                    generation,
                    UINotificationAdmissionStatus.CapacityExceeded,
                    "Notification channel has no pending entry eligible for overflow replacement.");
            }

            switch (channel.Definition.OverflowPolicy)
            {
                case UINotificationOverflowPolicy.DropOldestPending:
                    return AdmitByEvictingPending(
                        channel,
                        request,
                        generation,
                        FindOldestPendingIndex(channel.Pending),
                        "Oldest pending notification was evicted by channel overflow policy.");

                case UINotificationOverflowPolicy.ReplaceLowestPriorityPending:
                    int victimIndex =
                        FindLowestRankedPendingIndex(
                            channel.Pending);

                    Entry victim =
                        channel.Pending[victimIndex];

                    if (request.Priority <= victim.Priority)
                    {
                        return Reject(
                            request,
                            generation,
                            UINotificationAdmissionStatus.InsufficientPriority,
                            "Incoming notification must strictly outrank the lowest-priority pending entry.");
                    }

                    return AdmitByEvictingPending(
                        channel,
                        request,
                        generation,
                        victimIndex,
                        "Lowest-priority pending notification was evicted by channel overflow policy.");

                default:
                    return Reject(
                        request,
                        generation,
                        UINotificationAdmissionStatus.Unavailable,
                        "Notification channel uses an unsupported overflow policy.");
            }
        }

        private UINotificationHandle AdmitByEvictingPending(
            ChannelState channel,
            UINotificationRequest request,
            long generation,
            int victimIndex,
            string victimMessage)
        {
            long admissionSequence =
                ++nextAdmissionSequence;

            UINotificationAdmissionResult admission =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.Admitted,
                    request.ChannelId,
                    generation,
                    request.CoalescingKey,
                    request.CorrelationId,
                    "Notification generation admitted through pending overflow policy.");

            UINotificationHandle handle =
                new UINotificationHandle(admission);

            Entry replacement =
                new Entry(
                    request,
                    handle,
                    admissionSequence,
                    ResolveLifetimeSeconds(
                        channel,
                        request));

            Entry victim =
                channel.Pending[victimIndex];

            mutationInProgress = true;

            try
            {
                victim.Handle.TryComplete(
                    new UINotificationResult(
                        UINotificationOutcome.OverflowEvicted,
                        victim.Request.ChannelId,
                        victim.Handle.Generation,
                        victim.Request.CoalescingKey,
                        victim.Request.CorrelationId,
                        victimMessage));

                channel.Pending[victimIndex] =
                    replacement;

                Publish(channel);
            }
            finally
            {
                mutationInProgress = false;
            }

            return handle;
        }

        public UINotificationOperationResult Dismiss(
            UINotificationHandle handle)
        {
            if (handle == null)
            {
                return new UINotificationOperationResult(
                    shutdown
                        ? UINotificationOperationStatus.Shutdown
                        : IsValid
                            ? UINotificationOperationStatus.Invalid
                            : UINotificationOperationStatus.Unavailable,
                    message: shutdown
                        ? "Notification service is shut down."
                        : IsValid
                            ? "Notification handle is missing."
                            : "Notification service is unavailable.");
            }

            if (handle.IsCompleted)
            {
                UINotificationOperationStatus completedStatus =
                    handle.Result.Outcome ==
                        UINotificationOutcome.Superseded
                        ? UINotificationOperationStatus.Stale
                        : UINotificationOperationStatus.AlreadySettled;

                return new UINotificationOperationResult(
                    completedStatus,
                    handle.ChannelId,
                    handle.Generation,
                    completedStatus == UINotificationOperationStatus.Stale
                        ? "Notification generation was superseded by a replacement."
                        : "Notification generation is already settled.");
            }

            if (shutdown)
            {
                return new UINotificationOperationResult(
                    UINotificationOperationStatus.Shutdown,
                    handle.ChannelId,
                    handle.Generation,
                    "Notification service is shut down.");
            }

            if (!IsValid)
            {
                return new UINotificationOperationResult(
                    UINotificationOperationStatus.Unavailable,
                    message: "Notification service is unavailable.");
            }

            if (mutationInProgress)
            {
                return new UINotificationOperationResult(
                    UINotificationOperationStatus.Unavailable,
                    message: "Notification mutation is already settling committed state.");
            }

            if (!channels.TryGetValue(
                    handle.ChannelId.Value,
                    out ChannelState channel))
            {
                return new UINotificationOperationResult(
                    UINotificationOperationStatus.UnknownChannel,
                    handle.ChannelId,
                    handle.Generation,
                    "Notification handle references an unknown channel.");
            }

            int visibleIndex =
                FindEntryIndex(
                    channel.Visible,
                    handle);

            int pendingIndex =
                FindEntryIndex(
                    channel.Pending,
                    handle);

            if (visibleIndex < 0 &&
                pendingIndex < 0)
            {
                return new UINotificationOperationResult(
                    UINotificationOperationStatus.Stale,
                    handle.ChannelId,
                    handle.Generation,
                    "Notification handle does not address a live generation.");
            }

            Entry entry;
            mutationInProgress = true;

            try
            {
                if (visibleIndex >= 0)
                {
                    entry = channel.Visible[visibleIndex];
                    channel.Visible.RemoveAt(visibleIndex);
                    Promote(
                        channel,
                        ObserveNowOrLast());
                }
                else
                {
                    entry = channel.Pending[pendingIndex];
                    channel.Pending.RemoveAt(pendingIndex);
                }

                entry.Handle.TryComplete(
                    new UINotificationResult(
                        UINotificationOutcome.Dismissed,
                        entry.Request.ChannelId,
                        entry.Handle.Generation,
                        entry.Request.CoalescingKey,
                        entry.Request.CorrelationId,
                        "Notification generation was explicitly dismissed."));

                Publish(channel);
            }
            finally
            {
                mutationInProgress = false;
            }

            return new UINotificationOperationResult(
                UINotificationOperationStatus.Completed,
                entry.Request.ChannelId,
                entry.Handle.Generation,
                "Notification generation dismissed.");
        }

        public bool TryGetEntryState(
            UINotificationHandle handle,
            out UINotificationEntryState state)
        {
            state = default;

            if (!IsValid ||
                handle == null ||
                handle.IsCompleted ||
                !channels.TryGetValue(
                    handle.ChannelId.Value,
                    out ChannelState channel))
            {
                return false;
            }

            if (FindEntryIndex(
                    channel.Visible,
                    handle) >= 0)
            {
                state =
                    UINotificationEntryState.Visible;
                return true;
            }

            if (FindEntryIndex(
                    channel.Pending,
                    handle) >= 0)
            {
                state =
                    UINotificationEntryState.Pending;
                return true;
            }

            return false;
        }

        public bool TryGetDefinition(
            string channelId,
            out UINotificationChannelDefinition definition) =>
            TryGetDefinition(
                new UINotificationChannelId(channelId),
                out definition);

        public bool TryGetDefinition(
            UINotificationChannelId channelId,
            out UINotificationChannelDefinition definition)
        {
            definition = null;

            if (!channelId.IsValid ||
                !channels.TryGetValue(
                    channelId.Value,
                    out ChannelState channel))
            {
                return false;
            }

            definition = channel.Definition;
            return true;
        }

        public bool TryGetSnapshot(
            string channelId,
            out UINotificationChannelSnapshot snapshot) =>
            TryGetSnapshot(
                new UINotificationChannelId(channelId),
                out snapshot);

        public bool TryGetSnapshot(
            UINotificationChannelId channelId,
            out UINotificationChannelSnapshot snapshot)
        {
            snapshot = default;

            if (!channelId.IsValid ||
                !channels.TryGetValue(
                    channelId.Value,
                    out ChannelState channel))
            {
                return false;
            }

            snapshot =
                CreateSnapshot(channel);

            return true;
        }

        private static UINotificationChannelSnapshot CreateSnapshot(
            ChannelState channel)
        {
            UINotificationChannelDefinition definition =
                channel.Definition;

            return new UINotificationChannelSnapshot(
                definition.ChannelId,
                definition.VisibleCapacity,
                definition.PendingCapacity,
                channel.Visible.Count,
                channel.Pending.Count,
                definition.OverflowPolicy);
        }

        private void Publish(
            ChannelState channel)
        {
            Action<UINotificationChannelSnapshot> handlers =
                ChannelChanged;

            if (handlers == null)
            {
                return;
            }

            UINotificationChannelSnapshot snapshot =
                CreateSnapshot(channel);

            Delegate[] invocationList =
                handlers.GetInvocationList();

            for (int index = 0;
                 index < invocationList.Length;
                 index++)
            {
                try
                {
                    ((Action<UINotificationChannelSnapshot>)invocationList[index])(
                        snapshot);
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogException(exception);
                }
            }
        }

        private static UINotificationHandle Reject(
            UINotificationRequest request,
            long generation,
            UINotificationAdmissionStatus status,
            string message)
        {
            UINotificationChannelId channelId =
                request == null
                    ? default
                    : request.ChannelId;

            UINotificationCoalescingKey coalescingKey =
                request == null
                    ? default
                    : request.CoalescingKey;

            UINotificationCorrelationId correlationId =
                request == null
                    ? default
                    : request.CorrelationId;

            return UINotificationHandle.Rejected(
                new UINotificationAdmissionResult(
                    status,
                    channelId,
                    generation,
                    coalescingKey,
                    correlationId,
                    message));
        }

        private UINotificationHandle Coalesce(
            ChannelState channel,
            UINotificationRequest request,
            long generation,
            bool isVisible,
            int index)
        {
            List<Entry> collection =
                isVisible
                    ? channel.Visible
                    : channel.Pending;

            Entry prior =
                collection[index];

            UINotificationAdmissionResult admission =
                new UINotificationAdmissionResult(
                    UINotificationAdmissionStatus.Coalesced,
                    request.ChannelId,
                    generation,
                    request.CoalescingKey,
                    request.CorrelationId,
                    "Notification generation replaced a matching live entry.");

            UINotificationHandle handle =
                new UINotificationHandle(admission);

            Entry replacement =
                new Entry(
                    request,
                    handle,
                    prior.AdmissionSequence,
                    ResolveLifetimeSeconds(
                        channel,
                        request));

            if (isVisible)
            {
                replacement.BeginVisibleLifetime(
                    ObserveNowOrLast());
            }

            mutationInProgress = true;

            try
            {
                prior.Handle.TryComplete(
                    new UINotificationResult(
                        UINotificationOutcome.Superseded,
                        prior.Request.ChannelId,
                        prior.Handle.Generation,
                        prior.Request.CoalescingKey,
                        prior.Request.CorrelationId,
                        "Notification generation was superseded by a coalesced replacement."));

                collection[index] = replacement;

                Publish(channel);
            }
            finally
            {
                mutationInProgress = false;
            }

            return handle;
        }

        private static string ValidateRequest(
            UINotificationRequest request)
        {
            if (request.Presentation == null)
            {
                return
                    "Notification request requires a presentation payload.";
            }

            switch (request.LifetimeMode)
            {
                case UINotificationLifetimeMode.Automatic:
                case UINotificationLifetimeMode.Manual:
                    break;

                default:
                    return
                        "Notification request uses an unsupported lifetime mode.";
            }

            if (float.IsNaN(request.DurationSeconds) ||
                float.IsInfinity(request.DurationSeconds) ||
                request.DurationSeconds < 0f)
            {
                return
                    "Notification request duration must be finite and nonnegative.";
            }

            return string.Empty;
        }

        private static int FindEntryIndex(
            List<Entry> entries,
            UINotificationHandle handle)
        {
            for (int index = 0;
                 index < entries.Count;
                 index++)
            {
                Entry entry =
                    entries[index];

                if (entry.Handle.Generation ==
                        handle.Generation &&
                    ReferenceEquals(
                        entry.Handle,
                        handle))
                {
                    return index;
                }
            }

            return -1;
        }

        private static int RemoveLostOwners(
            List<Entry> entries,
            ref List<Entry> lostEntries)
        {
            int index = 0;
            int removedCount = 0;

            while (index < entries.Count)
            {
                Entry entry =
                    entries[index];

                if (!entry.HasLostOwner)
                {
                    index++;
                    continue;
                }

                entries.RemoveAt(index);

                if (lostEntries == null)
                {
                    lostEntries =
                        new List<Entry>();
                }

                lostEntries.Add(entry);
                removedCount++;
            }

            return removedCount;
        }

        private void DrainAllEntries(
            List<Entry> entries)
        {
            foreach (ChannelState channel in channels.Values)
            {
                entries.AddRange(
                    channel.Visible);

                entries.AddRange(
                    channel.Pending);

                channel.Visible.Clear();
                channel.Pending.Clear();
            }
        }

        private static void CompleteEntries(
            List<Entry> entries,
            UINotificationOutcome outcome,
            string message)
        {
            for (int index = 0;
                 index < entries.Count;
                 index++)
            {
                Entry entry =
                    entries[index];

                entry.Handle.TryComplete(
                    new UINotificationResult(
                        outcome,
                        entry.Request.ChannelId,
                        entry.Handle.Generation,
                        entry.Request.CoalescingKey,
                        entry.Request.CorrelationId,
                        message));
            }
        }

        private static bool TryFindCoalescingEntry(
            ChannelState channel,
            UINotificationCoalescingKey key,
            out bool isVisible,
            out int index)
        {
            isVisible = false;
            index = -1;

            for (int visibleIndex = 0;
                 visibleIndex < channel.Visible.Count;
                 visibleIndex++)
            {
                if (channel.Visible[visibleIndex]
                        .Request.CoalescingKey == key)
                {
                    isVisible = true;
                    index = visibleIndex;
                    return true;
                }
            }

            for (int pendingIndex = 0;
                 pendingIndex < channel.Pending.Count;
                 pendingIndex++)
            {
                if (channel.Pending[pendingIndex]
                        .Request.CoalescingKey == key)
                {
                    index = pendingIndex;
                    return true;
                }
            }

            return false;
        }

        private static int FindOldestPendingIndex(
            List<Entry> pending)
        {
            int oldestIndex = 0;

            for (int index = 1;
                 index < pending.Count;
                 index++)
            {
                if (pending[index].AdmissionSequence <
                    pending[oldestIndex].AdmissionSequence)
                {
                    oldestIndex = index;
                }
            }

            return oldestIndex;
        }

        private static int FindLowestRankedPendingIndex(
            List<Entry> pending)
        {
            // Equal lowest priorities evict the newest entry so the earlier
            // entry retains its established FIFO precedence.
            int lowestIndex = 0;

            for (int index = 1;
                 index < pending.Count;
                 index++)
            {
                Entry lowest =
                    pending[lowestIndex];

                Entry candidate =
                    pending[index];

                if (candidate.Priority < lowest.Priority ||
                    candidate.Priority == lowest.Priority &&
                    candidate.AdmissionSequence >
                    lowest.AdmissionSequence)
                {
                    lowestIndex = index;
                }
            }

            return lowestIndex;
        }

        private static double ResolveLifetimeSeconds(
            ChannelState channel,
            UINotificationRequest request)
        {
            if (request.LifetimeMode ==
                UINotificationLifetimeMode.Manual)
            {
                return 0d;
            }

            return request.DurationSeconds > 0f
                ? request.DurationSeconds
                : channel.Definition.DefaultLifetimeSeconds;
        }

        private static bool TryReadInitialClock(
            IUINotificationClock clock,
            out double nowSeconds)
        {
            nowSeconds = 0d;

            if (clock == null)
            {
                return false;
            }

            try
            {
                nowSeconds = clock.NowSeconds;
            }
            catch
            {
                return false;
            }

            return !double.IsNaN(nowSeconds) &&
                   !double.IsInfinity(nowSeconds) &&
                   nowSeconds >= 0d;
        }

        private bool TryObserveNow(
            out double nowSeconds)
        {
            nowSeconds = lastObservedNowSeconds;
            double candidate;

            try
            {
                candidate = clock.NowSeconds;
            }
            catch
            {
                return false;
            }

            if (double.IsNaN(candidate) ||
                double.IsInfinity(candidate) ||
                candidate < lastObservedNowSeconds)
            {
                return false;
            }

            lastObservedNowSeconds = candidate;
            nowSeconds = candidate;
            return true;
        }

        private double ObserveNowOrLast()
        {
            TryObserveNow(
                out double nowSeconds);

            return nowSeconds;
        }

        private static void Promote(
            ChannelState channel,
            double nowSeconds)
        {
            while (channel.Visible.Count <
                       channel.Definition.VisibleCapacity &&
                   channel.Pending.Count > 0)
            {
                int winnerIndex = 0;

                for (int index = 1;
                     index < channel.Pending.Count;
                     index++)
                {
                    Entry winner =
                        channel.Pending[winnerIndex];

                    Entry candidate =
                        channel.Pending[index];

                    if (candidate.Priority > winner.Priority ||
                        candidate.Priority == winner.Priority &&
                        candidate.AdmissionSequence <
                        winner.AdmissionSequence)
                    {
                        winnerIndex = index;
                    }
                }

                Entry promoted =
                    channel.Pending[winnerIndex];

                channel.Pending.RemoveAt(winnerIndex);

                promoted.BeginVisibleLifetime(
                    nowSeconds);

                channel.Visible.Add(promoted);
            }
        }

        private string ValidateAndSnapshotDefinitions(
            IEnumerable<UINotificationChannelDefinition> source)
        {
            if (source == null)
            {
                return
                    "Notification channel definitions are required.";
            }

            Dictionary<string, ChannelState> candidates =
                new Dictionary<string, ChannelState>(
                    StringComparer.Ordinal);

            foreach (UINotificationChannelDefinition authored in source)
            {
                string validationError =
                    ValidateDefinition(authored);

                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    return validationError;
                }

                UINotificationChannelDefinition definition =
                    authored.Snapshot();

                if (candidates.ContainsKey(
                        definition.ChannelId.Value))
                {
                    return
                        "Duplicate notification channel ID: " +
                        definition.ChannelId.Value;
                }

                candidates.Add(
                    definition.ChannelId.Value,
                    new ChannelState(definition));
            }

            if (candidates.Count == 0)
            {
                return
                    "At least one notification channel definition is required.";
            }

            foreach (KeyValuePair<string, ChannelState> candidate in candidates)
            {
                channels.Add(
                    candidate.Key,
                    candidate.Value);
            }

            IsValid = true;
            return string.Empty;
        }

        private static string ValidateDefinition(
            UINotificationChannelDefinition definition)
        {
            if (definition == null)
            {
                return
                    "A notification channel definition reference is missing.";
            }

            if (!definition.ChannelId.IsValid)
            {
                return
                    "Notification channel IDs must be nonempty stable project-authored values.";
            }

            if (definition.VisibleCapacity < 1)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' requires visible capacity of at least one.";
            }

            if (definition.PendingCapacity < 0)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' cannot use a negative pending capacity.";
            }

            if (float.IsNaN(
                    definition.DefaultLifetimeSeconds) ||
                float.IsInfinity(
                    definition.DefaultLifetimeSeconds) ||
                definition.DefaultLifetimeSeconds <= 0f)
            {
                return
                    "Notification channel '" +
                    definition.ChannelId.Value +
                    "' requires a finite positive default lifetime.";
            }

            switch (definition.OverflowPolicy)
            {
                case UINotificationOverflowPolicy.RejectNewest:
                case UINotificationOverflowPolicy.DropOldestPending:
                case UINotificationOverflowPolicy.ReplaceLowestPriorityPending:
                    return string.Empty;

                default:
                    return
                        "Notification channel '" +
                        definition.ChannelId.Value +
                        "' uses an unsupported pending overflow policy.";
            }
        }
    }
}
