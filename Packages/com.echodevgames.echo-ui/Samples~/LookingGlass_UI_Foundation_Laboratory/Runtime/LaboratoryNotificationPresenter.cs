using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI.Samples
{
    /// <summary>
    /// Plain sample-owned payload consumed by the Laboratory presenter.
    /// Production projects replace this model and presenter with their own.
    /// </summary>
    public sealed class LaboratoryNotificationCard
    {
        public LaboratoryNotificationCard(
            string title,
            string detail,
            Color accent)
        {
            Title = title ?? string.Empty;
            Detail = detail ?? string.Empty;
            Accent = accent;
        }

        public string Title { get; }

        public string Detail { get; }

        public Color Accent { get; }

        public override string ToString() =>
            string.IsNullOrWhiteSpace(Detail)
                ? Title
                : Title + " — " + Detail;
    }

    /// <summary>
    /// Sample-owned reference presenter. It retains only the latest bounded
    /// read model for each channel and draws deliberately plain IMGUI cards.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LaboratoryNotificationPresenter :
        MonoBehaviour,
        IUINotificationPresenter
    {
        private readonly Dictionary<
            string,
            UINotificationPresentationSnapshot> snapshots =
                new Dictionary<
                    string,
                    UINotificationPresentationSnapshot>(
                        StringComparer.Ordinal);

        private readonly List<string> channelOrder =
            new List<string>();

        private EchoUIRoot root;

        public int ApplyCount { get; private set; }

        public int ChannelCount =>
            channelOrder.Count;

        public int TotalVisibleCount
        {
            get
            {
                int count = 0;

                for (int index = 0;
                     index < channelOrder.Count;
                     index++)
                {
                    count +=
                        snapshots[channelOrder[index]]
                            .VisibleCount;
                }

                return count;
            }
        }

        public void Initialize(
            EchoUIRoot value)
        {
            root = value;
        }

        public void ApplyChannel(
            UINotificationPresentationSnapshot snapshot)
        {
            string channelId =
                snapshot.ChannelId.Value;

            if (!snapshots.ContainsKey(channelId))
            {
                channelOrder.Add(channelId);
            }

            snapshots[channelId] = snapshot;
            ApplyCount++;
        }

        public bool HasChannel(
            string channelId) =>
            channelId != null &&
            snapshots.ContainsKey(
                channelId.Trim());

        public int VisibleCount(
            string channelId) =>
            TryGetSnapshot(
                channelId,
                out UINotificationPresentationSnapshot snapshot)
                ? snapshot.VisibleCount
                : -1;

        public bool Contains(
            UINotificationHandle handle)
        {
            if (handle == null ||
                !TryGetSnapshot(
                    handle.ChannelId.Value,
                    out UINotificationPresentationSnapshot snapshot))
            {
                return false;
            }

            for (int index = 0;
                 index < snapshot.VisibleCount;
                 index++)
            {
                if (ReferenceEquals(
                        snapshot.VisibleEntries[index].Handle,
                        handle))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetSnapshot(
            string channelId,
            out UINotificationPresentationSnapshot snapshot)
        {
            snapshot = default;

            return channelId != null &&
                snapshots.TryGetValue(
                    channelId.Trim(),
                    out snapshot);
        }

        public void ResetEvidenceCounter()
        {
            ApplyCount = 0;
        }

        public void Clear()
        {
            snapshots.Clear();
            channelOrder.Clear();
            ApplyCount = 0;
            root = null;
        }

        private void OnGUI()
        {
            int visibleCount =
                TotalVisibleCount;

            if (visibleCount == 0)
            {
                return;
            }

            const float width = 390f;
            const float margin = 20f;
            float height =
                Mathf.Min(
                    80f + (visibleCount * 92f),
                    Screen.height * 0.48f);

            float left =
                Mathf.Max(
                    margin,
                    (Screen.width - width) * 0.5f);

            float top =
                Mathf.Max(
                    margin,
                    Screen.height - height - margin);

            Color priorBackground =
                GUI.backgroundColor;

            Color priorContent =
                GUI.contentColor;

            GUILayout.BeginArea(
                new Rect(
                    left,
                    top,
                    width,
                    height),
                "M4-02 SAMPLE-OWNED NOTIFICATION PRESENTER",
                GUI.skin.window);

            GUILayout.Label(
                "Plain reference view — project visuals replace this.");

            for (int channelIndex = 0;
                 channelIndex < channelOrder.Count;
                 channelIndex++)
            {
                string channelId =
                    channelOrder[channelIndex];

                UINotificationPresentationSnapshot snapshot =
                    snapshots[channelId];

                if (snapshot.VisibleCount == 0)
                {
                    continue;
                }

                GUILayout.Label(
                    channelId +
                    "  •  visible=" +
                    snapshot.VisibleCount);

                for (int entryIndex = 0;
                     entryIndex < snapshot.VisibleCount;
                     entryIndex++)
                {
                    DrawEntry(
                        snapshot.VisibleEntries[entryIndex]);
                }
            }

            GUILayout.EndArea();

            GUI.backgroundColor =
                priorBackground;

            GUI.contentColor =
                priorContent;
        }

        private void DrawEntry(
            UINotificationPresentationEntry entry)
        {
            LaboratoryNotificationCard card =
                entry.Presentation as
                    LaboratoryNotificationCard;

            Color accent =
                card == null
                    ? new Color32(
                        58,
                        210,
                        225,
                        255)
                    : card.Accent;

            GUI.backgroundColor = accent;

            GUILayout.BeginVertical(
                GUI.skin.box);

            GUI.contentColor = Color.white;

            string title =
                card == null
                    ? PresentationLabel(
                        entry.Presentation)
                    : card.Title;

            string detail =
                card == null
                    ? "Opaque project payload"
                    : card.Detail;

            GUILayout.Label(
                title +
                "  [priority " +
                entry.Priority +
                ", generation " +
                entry.Generation +
                "]");

            if (!string.IsNullOrWhiteSpace(detail))
            {
                GUILayout.Label(detail);
            }

            if (root != null &&
                entry.Handle != null &&
                !entry.Handle.IsCompleted &&
                GUILayout.Button(
                    "Dismiss generation " +
                    entry.Generation))
            {
                root.DismissNotification(
                    entry.Handle);
            }

            GUILayout.EndVertical();
        }

        private static string PresentationLabel(
            object presentation)
        {
            UnityEngine.Object unityPresentation =
                presentation as UnityEngine.Object;

            if (!ReferenceEquals(
                    unityPresentation,
                    null) &&
                unityPresentation == null)
            {
                return "<destroyed Unity presentation>";
            }

            return presentation == null
                ? "<missing presentation>"
                : presentation.ToString();
        }
    }
}
