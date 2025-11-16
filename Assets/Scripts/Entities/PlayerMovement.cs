using System.Collections;
using System.Collections.Generic;
using TDB.Player.Input;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TDB
{
    public class PlayerMovement : MonoBehaviour
    {
        //=================================================================================
        // File: PlayerMovement.cs
        // Author: Zach Lima
        // Date: 10/21/2025
        // Description: Player Movement State, pretty self explanatory. 
        //=================================================================================

        private EntityData _entityData;
        private InputController _inputController;
        private PlayerStateHandler _playerStateHandler;
       
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _inputController = GetComponentInChildren<InputController>();
            _playerStateHandler = GetComponentInChildren<PlayerStateHandler>();
        }

        // Start is called before the first frame update
        void Start()
        {
           
        }

        public void MovementUpdate()
        {
            //first, update movement for this frame.
            // _entityData.movementDirection.Set(InputManager.Movement.x, InputManager.Movement.y);
            // read from input controller
            _entityData.movementDirection.Set(_inputController.Movement.x, _inputController.Movement.y);

            //finally, update velocity for this frame. 

            if (_entityData.IsAttacking)
            {
                _entityData.Velocity = Vector2.zero;
                
            }
            else
            {
                DoBasicPlayerMovement();
            }

            _entityData.Rb.velocity = _entityData.Velocity;
            
           
        }
        
        private void DoBasicPlayerMovement()
        {
            //if the player is pressing an input
            if (_entityData.movementDirection != Vector2.zero)
            {
                
                //updates the last direction the player was facing
                _entityData.lastDirection = new Vector2(_entityData.movementDirection.x, _entityData.movementDirection.y);

                _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, _entityData.MaxSpeed * _entityData.movementDirection.x,_entityData.Acceleration * Time.deltaTime)
                                                    ,Mathf.MoveTowards(_entityData.Velocity.y, _entityData.MaxSpeed * _entityData.movementDirection.y, _entityData.Acceleration * Time.deltaTime));
            }
            else
            {
               
                _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
                                                  , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));
            }

        }

       
    }
}
