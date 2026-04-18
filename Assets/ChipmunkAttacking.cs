using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

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

        // timer for the attack windup
        private float timer = 0;

        [SerializeField] float windup = 5;

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
            // stop movement
            _chipmunk.Velocity = new Vector2(Mathf.MoveTowards(_chipmunk.Velocity.x, 0, _chipmunk.Decceleration * Time.deltaTime)
                                          , Mathf.MoveTowards(_chipmunk.Velocity.y, 0, _chipmunk.Decceleration * Time.deltaTime));

            _chipmunk.Rb.velocity = _chipmunk.Velocity;

            // check if enemy has come to a complete stop and of the projectile is not already fired
            if (_chipmunk.Rb.velocity == Vector2.zero && GetComponentInChildren<ChipmunkProjectile>(true).gameObject.activeSelf == false)
            {
                // activate attack wind up timer
                if (timer < windup)
                {
                    timer += Time.deltaTime;

                    Debug.Log(timer);
                }
                else
                {
                    // fire projectile
                    GetComponentInChildren<ChipmunkProjectile>(true).FireProjectile(_target);

                    // reset timer
                    timer = 0;
                }
            }
        }

        // ================================
        // Public Methods
        // ================================

        // updates the attack target
        public void SetAttackTarget(EntityData target)
        {
            _target = target;
        }

        // ================================
        // Private Methods
        // ================================

    }
}
