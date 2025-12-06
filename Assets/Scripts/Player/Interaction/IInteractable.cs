using System;

namespace TDB.Player.Interaction
{
    public interface IInteractable
    {
        public bool IsInteractable { get; }
        public Action OnInteractableUpdated { get; set; }
        
        /// <summary>
        /// Update animation/sprite so the player knows the object is ready to interact.
        /// </summary>
        public void SetReady();
        
        /// <summary>
        /// Update animation/sprite so the player knows the object is not ready to interact.
        /// </summary>
        public void SetNotReady();
    }
}