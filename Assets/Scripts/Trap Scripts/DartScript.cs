// ===========================================================================================================
// File: DartScript.cs
// Author: Callum Legendre
// Date: November 26, 2025
// Description: Script to control the movement, damage and miscelaneous behaviors of the dart in the dart trap
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        // lifetime for the dart, just in case it doesnt hit anything
        [SerializeField] float _lifetime;
        private float _timer = 0;

        // layer value for walls (i think) FIXME: just in case
        private const int _wallLayer = 6;

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
            // timer for the lifetime of the dart, just in case it fails to collide
            if (_lifetime > _timer)
            {
                // increase timer by deltatime
                _timer += Time.deltaTime;
            }
            else
            {
                // reset timer to 0
                _timer = 0;

                // set position of dart to the dart launcher and deactivate it.
                transform.position = transform.parent.position;
                _trapUniversal.DeactivateTrap();

                UnityEngine.Debug.Log("reset dart");
            }
        }

        void OnEnable()
        {
            // fires dart when it is enabled
            _rigidbody.velocity = transform.parent.forward * _speed;
        }

        // when the dart collides with something
        void OnTriggerEnter2D(Collider2D collision)
        {
            // check if entity and deal damage if yes
            if (collision.gameObject.layer == TrapUniversal._playerLayerInt || collision.gameObject.layer == TrapUniversal._enemyLayerInt)
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
