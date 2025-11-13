using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        public delegate void UpdateDelegate();
        //===============================//

        // This object requires a rigidbody2d component!

        //===============================//

        public float Acceleration = 1;
        public float Decceleration = 1;
        public float MaxSpeed = 1;
        public float MaxHealth = 1;
        public float Knockback = 10;
        public UpdateDelegate updateDelegate;
        public Vector2 lastDirection = new Vector2(0, -1);


        // for controlling exactly when velocity is applied!
        [HideInInspector] public Vector2 Velocity = Vector2.zero;

        [HideInInspector] public float CurrentHealth = 0;
        [HideInInspector] public Vector2 movementDirection = Vector2.zero;
        [HideInInspector] public Rigidbody2D Rb;
        

        [HideInInspector] public bool IsAttacking = false;

        


        private void Awake()
        {
            CurrentHealth = MaxHealth;
            Rb = GetComponent<Rigidbody2D>();
            Rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
            
        }

        public void DealDamage(GameObject _damagedEntity)
        {
            
            _damagedEntity.GetComponent<EntityData>().CurrentHealth -= 1;

            _damagedEntity.GetComponent<EntityData>().Velocity = lastDirection * Knockback;

            if(_damagedEntity.layer == AttackHitbox._playerLayer)
            {
                _damagedEntity.GetComponent<PlayerStateHandler>().ChangeState(PlayerStateHandler.States.stunned);
            }
               


            if (_damagedEntity.GetComponent<EntityData>().CurrentHealth <= 0)
            {
                //run the die method
                Destroy(_damagedEntity);

            }
        }
        public void Update()
        {
            updateDelegate?.Invoke();
        }




    }
}
