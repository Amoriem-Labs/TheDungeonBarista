using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//=================================================================================
// File: InputManager.cs
// Author: Zach Lima
// Date: 10/21/2025
// Description: Takes input from Control for player input
//=================================================================================


namespace TDB
{
    public class InputManager : MonoBehaviour
    {
        public static Vector2 Movement;
        public static bool IsAttacking = false;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _attackAction;


        public delegate void ButtonDelegate();

        public static ButtonDelegate attackKeyPressed;
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _moveAction = _playerInput.actions["Move"];
            _attackAction = _playerInput.actions["Attack"];

            _attackAction.performed += AttackButtonPressed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            Movement = _moveAction.ReadValue<Vector2>();
            
            
        }

        private void AttackButtonPressed(InputAction.CallbackContext context) 
        {
            attackKeyPressed?.Invoke();
        }
    }
}
