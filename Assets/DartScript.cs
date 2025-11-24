// ===========================================================================================================
// File: DartScript.cs
// Author: Callum Legendre
// Date: November 26, 2025
// Description: Script to control the movement, damage and miscelaneous behaviors of the dart in the dart trap
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TDB
{
    public class DartScript : MonoBehaviour
    {

        // ================================
        // Fields
        // ================================

        [SerializeField] BoxCollider2D _hitbox;

        [SerializeField] TrapUniversal _trapUniversal;

        [SerializeField] GameObject _launcher;

        [SerializeField] Transform _transform;

        private enum direction {right, up, left, down};


        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================


        // Start is called before the first frame update
        void Start()
        {
            float _rotation; // get rotation of the parent

        }

        // Update is called once per frame
        void Update()
        {
            _transform.position += _transform.forward * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            // check if entity and deal damage if yes

            // deactivate and set position to launcher
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
