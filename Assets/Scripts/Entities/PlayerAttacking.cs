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
        
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();

            // InputManager.attackKeyPressed += AttackKeyPressed;
            _inputController = GetComponentInChildren<InputController>();
            _inputController.AttackKeyPressed += AttackKeyPressed;
            
            GetComponentInChildren<AttackHitbox>().dealDamage += GetComponent<EntityData>().DealDamage;
        }
        private void AttackKeyPressed()
        {
            _entityData.IsAttacking = true;

        }
    }
}
