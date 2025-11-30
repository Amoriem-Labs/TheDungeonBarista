// ===========================================================================================================
// File: SpikeActivationScript.cs
// Author: Callum Legendre
// Date: November 29, 2025
// Description: Script which controls the activation of the spike trap.
// ===========================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

namespace TDB
{
    public class SpikeActivationScript : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================

        // referance to the univeral trap script
        private TrapUniversal _trapUniversal;

        // for the timer which activates the trap after a delay
        private bool _activating = false;

        // delay before activating the trap, adjustable in unity
        [SerializeField] float _activationDelay;

        // timer 
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
            // get referance to the universal trap script
            _trapUniversal = GetComponent<TrapUniversal>();
        }

        // Update is called once per frame
        void Update()
        {
            // if the trap has been activated
            if (_activating == true)
            {
                // increase the timer
                _timer += Time.deltaTime;

                // if the timer is done
                if (_timer >= _activationDelay)
                {
                    // activate the spikes
                    _trapUniversal.ActivateTrap();

                    // reset the timer
                    _timer = 0;

                    // put activating to false
                    _activating = false;
                }
            }
        }


        // exists as kind of a shortcut to avoid having two scripts for just the spike trap
        void OnTriggerEnter2D(Collider2D collision)
        {
            // checks if the trigger was a player or enemy
            if ((_trapUniversal._targetLayers.value & (1 << collision.gameObject.layer)) != 0)
            {
                _activating = true;
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
