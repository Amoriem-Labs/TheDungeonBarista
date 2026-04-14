using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===========================================================================================================
// File: ChipmunkAttacking.cs
// Author: Callum Legendre
// Date: April 14, 2025
// Description: Script to control the attacking of the chipmunk enemy
// ===========================================================================================================

namespace TDB
{
    public class ChipmunkAttacking : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // target for the attack
        private EntityData _target;

        // reference to the chipmunk's entity data
        private EntityData _chipmunk;

        // ================================
        // Unity Lifecycle Methods
        // ================================
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        void Awake()
        {
            // get and assign entity data for the chipmunk
            _chipmunk = GetComponent<EntityData>();
        }

        // Update is called once per frame (this is set to run as update through the ChipmunkStateHandler)
        public void AttackUpdate()
        {
            Debug.Log(_chipmunk.Velocity);

            // stop movement
            _chipmunk.Velocity = new Vector2(Mathf.MoveTowards(_chipmunk.Velocity.x, 0, _chipmunk.Decceleration * Time.deltaTime)
                                          , Mathf.MoveTowards(_chipmunk.Velocity.y, 0, _chipmunk.Decceleration * Time.deltaTime));

            _chipmunk.Rb.velocity = _chipmunk.Velocity;

            Debug.Log(_chipmunk.Velocity);
            
            // face player

            // attack player
        }

        // ================================
        // Public Methods
        // ================================

        // updates the attack target
        public void SetAttackTarget(EntityData target)
        {
            _target = target;
        }


    }
}
