using System;
using UnityEngine;

namespace TDB.Player.Interaction
{
    public sealed class DefaultInteractableHandler : MonoBehaviour, IInteractable
    {
        private bool _isInteractable;

        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                if (_isInteractable == value) return;
                
                _isInteractable = value;
                OnInteractableUpdated?.Invoke();
            }
        }

        public Action OnInteractableUpdated { get; set; }

        public Action OnInteract;
        public Action OnSetReady;
        public Action OnSetNotReady;
        
        public void Interact() => OnInteract?.Invoke();

        public void SetReady() => OnSetReady?.Invoke();

        public void SetNotReady() => OnSetNotReady?.Invoke();
    }
}