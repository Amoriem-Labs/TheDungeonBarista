using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    /// <summary>
    /// Use different InteractionTriggers to adjust the priority of interaction.
    /// Disabling an InteractionTrigger makes CanInteract = false.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class InteractionTrigger<T> : MonoBehaviour, IInteractionTrigger where T : class, IInteractable
    {
        [SerializeField] private EventChannel _enablePlayerInputEvent;
        [SerializeField] private EventChannel _disablePlayerInputEvent;
        
        private (GameObject go, T interactable)? _currentInteractable = null;
        private readonly List<(GameObject go, T interactable)> _allInteractablesInRange = new();

        public System.Action OnCurrentInteractableUpdated { get; set; }
        public virtual string InteractionTip => "Interact";

        // disabling an InteractionTrigger makes CanInteract = false because all colliders exit
        public bool GetCanInteract() => GetCanInteract(CurrentInteractable?.interactable);

        // override this to add extra conditions
        protected virtual bool GetCanInteract(T interactable) =>
            interactable?.IsInteractable == true;

        protected (GameObject go, T interactable)? CurrentInteractable
        {
            get => _currentInteractable;
            private set
            {
                // Debug.Log($"Change from {_currentInteractable?.go?.name} to {value?.go?.name}");
                // no change
                if (_currentInteractable?.interactable == value?.interactable) return;
                // current interactable becomes not ready
                _currentInteractable?.interactable?.SetNotReady();
                // update value
                _currentInteractable = value;
                // new interactable becomes ready
                _currentInteractable?.interactable?.SetReady();
                // update callback
                OnCurrentInteractableUpdated?.Invoke();
            }
        }

        public void Interact()
        {
            if (CurrentInteractable?.interactable == null)
            {
                Debug.LogError("Should ensure there is interactable object.");
                return;
            }
            
            Interact(CurrentInteractable?.interactable);
        }

        /// <summary>
        /// Let concrete interaction triggers determine how to interact.
        /// Remember to block/unblock properly through ToggleBlockingPlayerInput.
        /// </summary>
        /// <param name="interactable"></param>
        protected abstract void Interact([NotNull] T interactable);

        protected void TryUpdateCurrentInteractable() => TryUpdateCurrentInteractable(false);

        protected void ToggleBlockingPlayerInput(bool isBlocked)
        {
            var ev = isBlocked ? _disablePlayerInputEvent : _enablePlayerInputEvent;
            ev.RaiseEvent();
        }
        
        protected void TryUpdateCurrentInteractable(bool forceUpdate)
        {
            // no update if the current interactable is still interactable
            if (!forceUpdate && GetCanInteract()) return;
                
            // try finding new interactable
            var candidate = _allInteractablesInRange.Find(p => GetCanInteract(p.interactable));
            // set current interactable
            CurrentInteractable = candidate.interactable == null ? null : candidate;
            // no need to do callbacks
            // if (!CanInteract) return;
            // candidate.interactable!.SetReady();
            // OnCurrentInteractableUpdated?.Invoke();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log("trigger enter");
            var go = other.gameObject;
            // already tracked
            if (_allInteractablesInRange.Any(p => p.go == go)) return;
            // non-interactable
            if (!go.TryGetComponent(out T interactable)) return;
            // track new interactable
            _allInteractablesInRange.Add((go, interactable));
            interactable.OnInteractableUpdated += TryUpdateCurrentInteractable;
            // only need to update if there is no current interactable
            if (CurrentInteractable.HasValue) return;
            TryUpdateCurrentInteractable();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Debug.Log("trigger exit");
            var go = other.gameObject;
            var idx = _allInteractablesInRange.FindIndex(p => p.go == go);
            // not-tracked
            if (idx < 0 || idx >= _allInteractablesInRange.Count) return;
            // pop interactable
            var (_, interactable) = _allInteractablesInRange[idx];
            _allInteractablesInRange.RemoveAt(idx);
            interactable.OnInteractableUpdated -= TryUpdateCurrentInteractable;
            // only update when the current interactable is exiting
            if (CurrentInteractable?.interactable != interactable) return;
            // force update because the current interactable is no longer in range
            TryUpdateCurrentInteractable(forceUpdate: true);
        }

        protected virtual void OnValidate()
        {
            if (TryGetComponent(out Collider2D coll))
            {
                coll.isTrigger = true;
            }

            if (!_enablePlayerInputEvent)
            {
                _enablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/EnablePlayerInput");
            }

            if (!_disablePlayerInputEvent)
            {
                _disablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/DisablePlayerInput");
            }
        }
    }
    
    public interface IInteractionTrigger
    {
        System.Action OnCurrentInteractableUpdated { get; set; }
        string InteractionTip { get; }
        bool GetCanInteract();
        void Interact();
    }
}