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
        private TrapUniversal _trapUniversal;

        // referance to the rigidbody element of the dart
        [SerializeField] Rigidbody2D _rigidbody;

        // speed used to adjust how fast the dart moves
        [SerializeField] float _speed;

        // layer values to tell what the dart hits
        private const int _wallLayer = 6;
        private const int _playerLayer = 7;
        private const int _enemyLayer = 8;
        

        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================


        // Start is called before the first frame update
        void Start()
        {
            // gets value of the universal script
            _trapUniversal = GetComponentInParent<TrapUniversal>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnEnable()
        {
            // fires dart when it is enabled
            _rigidbody.velocity = transform.parent.forward * _speed;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            // check if entity and deal damage if yes
            if (collision.gameObject.layer == _playerLayer || collision.gameObject.layer == _enemyLayer)
            {
                // deal damage to the entity it collided with
                _trapUniversal.DealDamage(collision.gameObject);

                // deactivate and set position to launcher
                transform.position = transform.parent.position;
                _trapUniversal.DeactivateTrap();
            }
            // if collider a wall, reset trap
            else if (collision.gameObject.layer == _wallLayer)
            {
                // deactivate and set position to launcher
                transform.position = transform.parent.position;
                _trapUniversal.DeactivateTrap();
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
