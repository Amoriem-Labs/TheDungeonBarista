using System;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.DungeonSystem
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Teleporter : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionTip = "Teleport";
        [SerializeField] private Teleporter linkedTeleporter;
        [SerializeField] private Vector3 exitOffset;
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool requireRoomCleared = true;
        [SerializeField] private DungeonRoomController owningRoom;
        [SerializeField] private DungeonRoomController targetRoom;

        public string InteractionTip => interactionTip;
        public bool IsInteractable =>
            isInteractable &&
            linkedTeleporter != null &&
            (!requireRoomCleared || owningRoom == null || owningRoom.IsCleared);
        public Action OnInteractableUpdated { get; set; }

        public void LinkTo(Teleporter other)
        {
            if (linkedTeleporter == other) return;
            linkedTeleporter = other;
            OnInteractableUpdated?.Invoke();
        }

        public void SetRoomLinks(DungeonRoomController owning, DungeonRoomController target)
        {
            owningRoom = owning;
            targetRoom = target;
            OnInteractableUpdated?.Invoke();
        }

        public void NotifyInteractableUpdated()
        {
            OnInteractableUpdated?.Invoke();
        }

        public void Teleport(GameObject player)
        {
            if (!IsInteractable || linkedTeleporter == null || player == null) return;

            Vector3 destination = linkedTeleporter.transform.position + exitOffset;
            destination.z = player.transform.position.z;
            player.transform.position = destination;

            targetRoom?.HandlePlayerEntered();
        }

        public void SetReady()
        {
        }

        public void SetNotReady()
        {
        }

        private void OnValidate()
        {
            if (TryGetComponent(out Collider2D collider2D))
            {
                collider2D.isTrigger = true;
            }
        }
    }
}
