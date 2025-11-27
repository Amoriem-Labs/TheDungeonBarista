// ===========================================================================================================
// File: DartScript.cs
// Author: Callum Legendre
// Date: November 26, 2025
// Description: Script to control the movement, damage and miscelaneous behaviors of the dart in the dart trap
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TDB
{
    public class DartScript : MonoBehaviour
    {

        // ================================
        // Fields
        // ================================

        // the referance for the boxcollider used as hitbox
        [SerializeField] BoxCollider2D _hitbox;

        // referance for the unversal trap script to access the public functions
        [SerializeField] TrapUniversal _trapUniversal;

        // referance to the rigidbody element of the dart
        [SerializeField] Rigidbody2D _rigidbody;

        // referance to the parent object
        [SerializeField] GameObject _parent;

        // speed used to adjust how fast the dart moves
        [SerializeField] float _speed;

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

        void OnEnable()
        {
            _rigidbody.velocity = transform.parent.forward * _speed;
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
