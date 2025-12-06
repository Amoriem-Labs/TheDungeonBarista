using System;
using Sirenix.OdinInspector;
using TDB.Player.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TDB.Player.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private InteractionController _primaryInteractionController;
        [SerializeField] private InteractionController _secondaryInteractionController;
        
        [Title("Input Actions")]
        [SerializeField] private InputActionReference _moveActionReference;
        [SerializeField] private InputActionReference _primaryActionReference;
        [SerializeField] private InputActionReference _secondaryActionReference;
        
        // the PlayerAttack script should listen to this event to handle attacks
        public Action AttackKeyPressed;
        // the PlayerMovement script should monitor this value to control movement
        public Vector2 Movement { get; private set; }
        
        private void OnEnable()
        {
            _moveActionReference.action.Enable();
            _primaryActionReference.action.Enable();
            _secondaryActionReference.action.Enable();
            
            _moveActionReference.action.performed += HandleMoveAction;
            _moveActionReference.action.canceled += HandleMoveAction;
            _primaryActionReference.action.performed += HandlePrimaryAction;
            _secondaryActionReference.action.performed += HandleSecondaryAction;
        }

        private void OnDisable()
        {
            _moveActionReference.action.Disable();
            _primaryActionReference.action.Disable();
            _secondaryActionReference.action.Disable();
            
            _moveActionReference.action.performed -= HandleMoveAction;
            _moveActionReference.action.canceled -= HandleMoveAction;
            _primaryActionReference.action.performed -= HandlePrimaryAction;
            _secondaryActionReference.action.performed -= HandleSecondaryAction;
        }

        private void HandleMoveAction(InputAction.CallbackContext context)
        {
            Movement = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
        }

        private void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (_primaryInteractionController.CanInteract)
            {
                _primaryInteractionController.Interact();
            }
            else
            {
                // attack if cannot interact
                AttackKeyPressed?.Invoke();
            }
        }
        
        private void HandleSecondaryAction(InputAction.CallbackContext obj)
        {
            if (_secondaryInteractionController.CanInteract)
            {
                _secondaryInteractionController.Interact();
            }
        }
    }
}