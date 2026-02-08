using System;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.FurnitureParts
{
    public class IngredientStorageEntryPoint : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;

        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // TODO:
        }

        public void SetNotReady()
        {
            // TODO:
        }
    }
}