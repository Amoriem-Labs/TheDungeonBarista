using System.Collections;
using System.Collections.Generic;
using TDB.Player.Input;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: PlayerAttacking.cs
    // Author: Zach Lima
    // Date: 11/09/2025
    // Description: Handles Player Attacking
    //=================================================================================

    public class PlayerAttacking : MonoBehaviour
    {
        // Start is called before the first frame update
        private EntityData _entityData;
        private InputController _inputController;
        private PlayerStateHandler _playerStateHandler;
    
        public string currentProjectilePath  = "Orb";
        public GameObject prefabAsset;
        public Rigidbody2D rigidBody;
        
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _playerStateHandler = GetComponent<PlayerStateHandler>();
            _inputController = GetComponentInChildren<InputController>();

            _inputController.AttackKeyPressed += AttackKeyPressed;
          //  GetComponentInChildren<AttackHitbox>().dealDamage += GetComponent<EntityData>().DealDamage;
            prefabAsset = Resources.Load<GameObject>(currentProjectilePath);

            rigidBody = GetComponent<Rigidbody2D>();
        }




        private void AttackKeyPressed()
        {
            if ( _playerStateHandler.currentState == PlayerStateHandler.States.free)
            {
                
                GameObject _newProj = (GameObject)Instantiate(prefabAsset, rigidBody.position, Quaternion.identity);

                _newProj.GetComponent<Rigidbody2D>().velocity = GetComponent<EntityData>().lastDirection * _newProj.GetComponent<Projectile>().maxSpeed;
                
                _entityData.IsAttacking = true;
            }
        }
    }
}
