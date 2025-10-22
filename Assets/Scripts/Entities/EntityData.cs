using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: EntityData.cs
    // Author: Zach Lima
    // Date: 10/21/2025
    // Description: The basic script all things with health should use. 
    //=================================================================================
    public class EntityData : MonoBehaviour
    {
        //===============================//

        // This object requires a rigidbody2d component!

        //===============================//

        public float Acceleration = 1;
        public float Decceleration = 1;
        public float MaxSpeed = 1;
        public float MaxHealth = 1;


        // for controlling exactly when velocity is applied!
        [HideInInspector] public Vector2 Velocity = Vector2.zero;

        [HideInInspector] public float CurrentHealth = 0;
        [HideInInspector] public Vector2 movementDirection = Vector2.zero;
        [HideInInspector] public Rigidbody2D Rb;
        [HideInInspector] public Vector2 lastDirection = new Vector2(0, 1);

        [HideInInspector] public bool IsAttacking = false;


        private void Awake()
        {
            CurrentHealth = MaxHealth;
            Rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {

        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<EntityData>() != null)
            {
               
            }

        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<EntityData>() != null)
            {
              
            }




        }



    }
}
