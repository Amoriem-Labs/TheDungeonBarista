using System;
using Sirenix.OdinInspector;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    public class Collectible : SerializedMonoBehaviour, IInteractable
    {
        public Vector2Int size = Vector2Int.one;

        [SerializeField] public CollectibleDefinition Definition;

        public bool IsInteractable => true;
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // TODO: highlight
        }

        public void SetNotReady()
        {
            // TODO: highlight
        }

        private void Awake()
        {
            if (Definition?.Definition != null && TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.sprite = Definition.Definition.CollectibleSprite;
            }
        }

        public void GetCollected()
        {
            // TODO: potential animations
            GameObject.Destroy(gameObject);
        }
    }
}