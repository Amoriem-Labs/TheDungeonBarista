using System.Collections;
using System.Collections.Generic;
using TDB.Player.Input;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: PlayerStunned.cs
    // Author: Zach Lima
    // Date: 11/23/2025
    // Description: State for when Player is STUNNED
    //=================================================================================

    public class PlayerStunned : MonoBehaviour
    {
        // Start is called before the first frame update
        private EntityData _entityData;
        private InputController _inputController;
        private PlayerStateHandler _playerStateHandler;

        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _inputController = GetComponentInChildren<InputController>();
            _playerStateHandler = GetComponentInChildren<PlayerStateHandler>();

        }
        void Start()
        {
        
        }

        // Update is called once per frame
        public void StunnedUpdate()
        {
            _entityData.movementDirection.Set(0,0);
           

            _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
                                                  , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));

            _entityData.Rb.velocity = _entityData.Velocity;


            if (_entityData.Rb.velocity == new Vector2(0, 0))
            {
                _playerStateHandler.ChangeState(PlayerStateHandler.States.free);
            }
        }
    }
}
