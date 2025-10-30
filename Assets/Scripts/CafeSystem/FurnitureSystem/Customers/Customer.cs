using System;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.Customers
{
    public class Customer : MonoBehaviour, IInteractable
    {
        public bool IsInteractable { get; }
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            throw new NotImplementedException();
        }

        public void SetNotReady()
        {
            throw new NotImplementedException();
        }
    }
}