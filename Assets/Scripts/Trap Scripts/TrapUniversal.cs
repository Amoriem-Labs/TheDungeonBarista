// ===========================================================================================================
// File: TrapUniversal.cs
// Author: Callum Legendre
// Date: October 30, 2025
// Description: Reusable script which handes the activation, deactivation, and damaging of the player by traps
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Modules;
using UnityEngine;

namespace TDB
{
    public class TrapUniversal : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // referance to the boxcollider is defined in the unity editor
        [SerializeField] BoxCollider2D _trigger;

        // the activated state of the trap, defined in the unity editor 
        [SerializeField] GameObject _activatedTrap;

        // the damage that the trap does
        [SerializeField] int _damage;


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

        // exists as kind of a shortcut to avoid having two scripts for just the spike trap
        void OnTriggerEnter2D(Collider2D collision)
        {
            // calls activated trap
            ActivateTrap();
        }

        // ================================
        // Public Methods
        // ================================

        // called by the part of the trap which damages the entity it hits, called on contact with the an entity
        public void DealDamage(GameObject entity)
        {
            // if the entity is a player deal damage through the player health script system
            if (entity.CompareTag("Player"))
            {
                // get the referance to the health script
                Health healthScript = entity.GetComponent<Health>();

                // use the script to deal damage
                healthScript.TakeDamage(_damage);
            }
            // if the entity is an enemy, deal damage to them using thier health system thingy
            else if (entity.CompareTag("Enemy"))
            {
                
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
