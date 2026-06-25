// ===========================================================================================================
// File: SpikeScript.cs
// Author: Callum Legendre
// Date: November 3, 2025
// Description: Script for the spike trap prefab. Handles specifics of damage dealing and deactivation.
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace TDB
{
    public class SpikeScript : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // referance to the collider which will do damage
        [SerializeField] BoxCollider2D _spikesCollider;

        // the the number of seconds the trap will be active for
        [SerializeField] int _timeActive;

        // the timer for long the trap will be active for
        private float _timer = 0;

        // referance to the universal traps script attached to the spikes prefab
        private TrapUniversal _trapsUniversal;

        // size in x and y of the damaging area of the spikes
        private Vector2 _size;

        private LayerMask _targetLayers;




        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================

        // use awake instead of start so that way the varaibles initialise correctly before they are used
        void Awake()
        {
            // gets the size of the area by setting it equal to scale of the spikes
            _size = transform.localScale;

            // get referance to universal script
            _trapsUniversal = GetComponentInParent<TrapUniversal>();

            // instnatiates the layermaskj for use with detection of entities above spike trap
            _targetLayers = LayerMask.GetMask(TrapUniversal._playerLayer, TrapUniversal._enemyLayer);
        }

        // Update is called once per frame
        void Update()
        {
            // timer deacticates the spikes after the lifetime of the trap expires
            if (_timer < _timeActive)
            {
                // increase timer by change in time
                _timer += Time.deltaTime;
            }
            else if (_timer >= _timeActive)
            {
                // deactivate the spikes
                _trapsUniversal.DeactivateTrap();
                _timer = 0;
            }
        }

        void OnEnable()
        {
            // create an array of all entities in the spike trap on activation
            Collider2D[] inSpikes = Physics2D.OverlapBoxAll(transform.position, _size, 0f, _targetLayers);

            // iterate through all entites in the spike array and deal damage to them
            foreach (Collider2D entity in inSpikes)
            {
                // call universal deal damage method
                _trapsUniversal.DealDamage(entity.gameObject);
            }
        }

        // ================================
        // Public Methods
        // ================================


        // ================================
        // Private Methods
        // ================================


        // ================================
        // Events and Handlers
        // ================================
    }
}
