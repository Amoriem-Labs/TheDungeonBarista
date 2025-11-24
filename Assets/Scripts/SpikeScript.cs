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

namespace TDB
{
    public class SpikeScript : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // referance to the collider which will do damage
        [SerializeField] BoxCollider2D _spikesCollider;

        // referance to the universal traps script attached to the spikes prefab
        [SerializeField] TrapUniversal _trapsUniversal;

        // the the number of seconds the trap will be active for
        [SerializeField] int _timeActive;

        // referance to the dormant spike trap
        [SerializeField] GameObject _dormantSpikes;

        // the timer for long the trap will be active for
        private float _timer = 0;



        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================

        // Start is called before the first frame update
        void Start()
        {
            // get correct referance to TrapsUniversal script
            _trapsUniversal = _dormantSpikes.GetComponent<TrapUniversal>();
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

        // when something enters the spikes, deal damage to it
        void OnCollisionEnter2D(Collision2D _collision)
        {
            // TODO: deal the damage to the entity which entered
            _trapsUniversal.DealDamage(_collision);
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
