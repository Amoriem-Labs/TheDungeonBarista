using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CafeSystem.FurnitureSystem;
using TDB.Player.Interaction.Triggers;
using UnityEngine;

namespace TDB.Player.Interaction
{
    /// <summary>
    /// A single InteractionController controls multiple InteractionTriggers.
    /// The order of InteractionTriggers in the hierarchy determines the priority of the interaction.
    /// Disable InteractionTriggers to prevent certain interactions.
    /// Handles the player input.
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private InteractionTipUI _tipUI;
        
        private List<IInteractionTrigger> _triggers;
        private IInteractionTrigger _currentTrigger;
        
        /// <summary>
        /// Read by the player input controller to check if the player can interact right now.
        /// </summary>
        public bool CanInteract => _currentTrigger != null;

        private void Awake()
        {
            _triggers = GetComponentsInChildren<IInteractionTrigger>().ToList();
        }

        private void OnEnable()
        {
            foreach (var trigger in _triggers)
            {
                trigger.OnCurrentInteractableUpdated += HandleInteractableUpdate;
            }
        }

        private void OnDisable()
        {
            foreach (var trigger in _triggers)
            {
                trigger.OnCurrentInteractableUpdated -= HandleInteractableUpdate;
            }
        }

        private void HandleInteractableUpdate()
        {
            _currentTrigger = _triggers.Find(t => t.GetCanInteract());
            // Debug.Log($"{gameObject.name} interactable updated: {CanInteract}");
            // update animation/sprite so the player know there is something to interact
            if (CanInteract)
            {
                _tipUI.Display(_currentTrigger.InteractionTip);
            }
            else
            {
                _tipUI.Hide();
            }
        }

        /// <summary>
        /// Invoked by the player input controller to interact.
        /// </summary>
        public void Interact()
        {
            if (!CanInteract)
            {
                Debug.LogWarning("Should check CanInteract before invoking Interact.");
                return;
            }
            
            _currentTrigger.Interact();
        }
    }
}