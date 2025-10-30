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
            var direction = Vector2.zero;
            if (context.canceled)
            {
                // TODO: update player movement to direction (stop move)
                return;
            }

            direction = context.ReadValue<Vector2>();
            // TODO: update player movement to direction
        }

        private void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (_primaryInteractionController.CanInteract)
            {
                _primaryInteractionController.Interact();
            }
            else
            {
                // TODO: attack if can
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