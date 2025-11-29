// ===========================================================================================================
// File: DartTrigger.cs
// Author: Callum Legendre
// Date: November 27, 2025
// Description: Controls the activation of the dart trap
// ===========================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class DartTrigger : MonoBehaviour
    {

        // ================================
        // Fields
        // ================================

        // referance to the collider that is used to trigger the trap
        [SerializeField] BoxCollider2D _trigger;

        // referance to the universal trap script
        private TrapUniversal _trapUniversal;


        // ================================
        // Properties
        // ================================


        // ================================
        // Unity Lifecycle Methods
        // ================================


        // Start is called before the first frame update
        void Start()
        {
            // gets referance to the trap universal script
            _trapUniversal = GetComponentInParent<TrapUniversal>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            // activates trap by calling activate trap function in universal script
            _trapUniversal.ActivateTrap();
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
