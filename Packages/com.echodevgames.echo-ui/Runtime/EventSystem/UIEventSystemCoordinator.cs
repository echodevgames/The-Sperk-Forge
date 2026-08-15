using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Non-destructive EventSystem adoption/creation coordinator.
    /// It never disables, renames, destroys, or replaces externally owned systems.
    /// </summary>
    public sealed class UIEventSystemCoordinator
    {
        private EventSystem coordinated;
        private EventSystem created;

        public UIEventSystemCoordinationStatus Status { get; private set; } =
            UIEventSystemCoordinationStatus.Uninitialized;

        public EventSystem CoordinatedEventSystem =>
            coordinated;

        public bool OwnsCreatedEventSystem =>
            created != null &&
            coordinated == created;

        public UIEventSystemCoordinationResult Coordinate(
            UIEventSystemCoordinationMode mode,
            EventSystem assigned,
            Transform createdParent = null)
        {
            if (created != null)
            {
                DestroyCreated();
            }

            coordinated = null;
            Status = UIEventSystemCoordinationStatus.Uninitialized;

            EventSystem[] eligible =
                FindEligibleEventSystems();

            switch (mode)
            {
                case UIEventSystemCoordinationMode.AdoptAssigned:
                    return AdoptAssigned(
                        assigned,
                        eligible.Length);

                case UIEventSystemCoordinationMode.AdoptExisting:
                    return AdoptExisting(
                        eligible,
                        allowCreation: false,
                        requireExternal: false,
                        createdParent: createdParent);

                case UIEventSystemCoordinationMode.CreateIfMissing:
                    return AdoptExisting(
                        eligible,
                        allowCreation: true,
                        requireExternal: false,
                        createdParent: createdParent);

                case UIEventSystemCoordinationMode.RequireExternal:
                    return AdoptExisting(
                        eligible,
                        allowCreation: false,
                        requireExternal: true,
                        createdParent: createdParent);

                default:
                    Status =
                        UIEventSystemCoordinationStatus.Missing;

                    return new UIEventSystemCoordinationResult(
                        Status,
                        null,
                        false,
                        eligible.Length,
                        "Unsupported EventSystem coordination mode.");
            }
        }

        public void Shutdown()
        {
            DestroyCreated();
            coordinated = null;
            Status =
                UIEventSystemCoordinationStatus.Uninitialized;
        }

        private UIEventSystemCoordinationResult AdoptAssigned(
            EventSystem assigned,
            int eligibleCount)
        {
            if (assigned == null ||
                !assigned.isActiveAndEnabled)
            {
                Status =
                    UIEventSystemCoordinationStatus.InvalidAssigned;

                return new UIEventSystemCoordinationResult(
                    Status,
                    null,
                    false,
                    eligibleCount,
                    "AdoptAssigned requires an active enabled project-assigned EventSystem.");
            }

            coordinated = assigned;
            Status =
                UIEventSystemCoordinationStatus.Ready;

            return new UIEventSystemCoordinationResult(
                Status,
                coordinated,
                false,
                eligibleCount,
                "Looking Glass adopted the explicitly assigned EventSystem.");
        }

        private UIEventSystemCoordinationResult AdoptExisting(
            EventSystem[] eligible,
            bool allowCreation,
            bool requireExternal,
            Transform createdParent)
        {
            if (eligible.Length > 1)
            {
                Status =
                    UIEventSystemCoordinationStatus.Ambiguous;

                return new UIEventSystemCoordinationResult(
                    Status,
                    null,
                    false,
                    eligible.Length,
                    "Multiple active eligible EventSystems were found. Looking Glass will not choose an arbitrary winner.");
            }

            if (eligible.Length == 1)
            {
                coordinated = eligible[0];
                Status =
                    UIEventSystemCoordinationStatus.Ready;

                return new UIEventSystemCoordinationResult(
                    Status,
                    coordinated,
                    false,
                    1,
                    requireExternal
                        ? "Looking Glass adopted the required external EventSystem."
                        : "Looking Glass adopted the unambiguous existing EventSystem.");
            }

            if (!allowCreation)
            {
                Status =
                    UIEventSystemCoordinationStatus.Missing;

                return new UIEventSystemCoordinationResult(
                    Status,
                    null,
                    false,
                    0,
                    requireExternal
                        ? "RequireExternal found no active eligible EventSystem and will not create one."
                        : "No active eligible EventSystem exists to adopt.");
            }

            GameObject objectInstance =
                new GameObject(
                    "EchoUI EventSystem");

            if (createdParent != null)
            {
                objectInstance.transform.SetParent(
                    createdParent,
                    false);
            }

            created =
                objectInstance.AddComponent<EventSystem>();

            coordinated = created;
            Status =
                UIEventSystemCoordinationStatus.Ready;

            return new UIEventSystemCoordinationResult(
                Status,
                coordinated,
                true,
                0,
                "Looking Glass created a root-owned EventSystem because CreateIfMissing was explicitly configured.");
        }

        private static EventSystem[] FindEligibleEventSystems()
        {
            EventSystem[] all =
                Object.FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.InstanceID);

            if (all == null ||
                all.Length == 0)
            {
                return System.Array.Empty<EventSystem>();
            }

            int count = 0;
            for (int index = 0;
                 index < all.Length;
                 index++)
            {
                if (all[index] != null &&
                    all[index].isActiveAndEnabled)
                {
                    count++;
                }
            }

            if (count == all.Length)
            {
                return all;
            }

            EventSystem[] eligible =
                new EventSystem[count];

            int writeIndex = 0;
            for (int index = 0;
                 index < all.Length;
                 index++)
            {
                EventSystem item = all[index];
                if (item == null ||
                    !item.isActiveAndEnabled)
                {
                    continue;
                }

                eligible[writeIndex++] = item;
            }

            return eligible;
        }

        private void DestroyCreated()
        {
            if (created == null)
            {
                return;
            }

            GameObject objectInstance =
                created.gameObject;

            created = null;
            if (coordinated != null &&
                coordinated.gameObject ==
                    objectInstance)
            {
                coordinated = null;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(
                    objectInstance);
            }
            else
            {
                Object.DestroyImmediate(
                    objectInstance);
            }
        }
    }
}
