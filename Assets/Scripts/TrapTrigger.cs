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
    public class Traps : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // referance to the boxcollider is defined in the unity editor
        [SerializeField] BoxCollider2D _collider;

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

        void OnTriggerEnter2D(Collider2D collision)
        {
            // activates the trap
            _activatedTrap.SetActive(true);
        }

        // ================================
        // Public Methods
        // ================================

        // called by the part of the trap which damages the player, called on contact with the player
        public void DamagePlayer()
        {
            // TODO: correctly get referance to the function in the health script
            TakeDamage(_damage);
        }
        
        // called by the part of the trap that damages the player, condition different for each trap
        public void DeactivateTrap()
        {
            // deactivate trap
            _activatedTrap.SetActive(false);
        }


        // ================================
        // Private Methods
        // ================================


        // ================================
        // Events and Handlers
        // ================================


    }
}
