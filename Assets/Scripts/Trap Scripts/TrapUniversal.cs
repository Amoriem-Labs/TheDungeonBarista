// ===========================================================================================================
// File: TrapUniversal.cs
// Author: Callum Legendre
// Date: October 30, 2025
// Description: Reusable script which handes the activation, deactivation, and damaging of the player by traps
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Modules;
using Unity.VisualScripting;
using UnityEngine;

namespace TDB
{
    public class TrapUniversal : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // the activated state of the trap, defined in the unity editor 
        [SerializeField] GameObject _activatedTrap;

        // the damage that the trap does
        [SerializeField] int _damage;

        // string constants for layers that we want to detect things on (player and enemy layers)
        [HideInInspector] public const string _playerLayer = "Player";
        [HideInInspector] public const string _enemyLayer = "Enemy";

        // the layer values of the player and enemy layers. This will break if the layers happen to change, so keep an eye out for that
        [HideInInspector] public const int _playerLayerInt = 7;
        [HideInInspector] public const int _enemyLayerInt = 8;


        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // ================================
        // Public Methods
        // ================================

        // called by the part of the trap which damages the entity it hits, called on contact with the an entity
        public void DealDamage(GameObject entity)
        {
            // if the entity is a player deal damage through the player health script system
            if (entity.layer == _playerLayerInt)
            {
                EntityData data = entity.GetComponentInParent<EntityData>();
                if (data != null)
                {
                    data.CurrentHealth -= _damage;

                    if (data.CurrentHealth <= 0)
                    {
                        Destroy(data.gameObject);
                    }
                }
            }
            // if the entity is an enemy, deal damage to them using thier health system thingy
            else if (entity.layer == _enemyLayerInt)
            {
                // the following code i just completely ripped from 'PlayerMovement.cs'
                // if there is a more elegant way of doing this let me know, but this s it for now

                // deals the damage to the entity
                entity.GetComponentInParent<EntityData>().CurrentHealth -= _damage;

                // knockback wouldnt work well with spike trap so disabled for now
                // entity.GetComponent<EntityData>().Velocity = _entityData.lastDirection * _entityData.Knockback;
                
                // if heals is 0 or below, kills it
                if (entity.GetComponentInParent<EntityData>().CurrentHealth <= 0)
                {
                    //run the die method
                    Destroy(entity.transform.parent.gameObject);
                }
            }
        }
        
        // called by the part of the trap that deals damage, condition different for each trap
        public void DeactivateTrap()
        {
            // deactivate trap
            _activatedTrap.SetActive(false);
        }

        // called by the part of the trap that triggers it, conditions different for each trap
        public void ActivateTrap()
        {
            // activate the trap
            _activatedTrap.SetActive(true);
        }


        // ================================
        // Private Methods
        // ================================


        // ================================
        // Events and Handlers
        // ================================


    }
}
