using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: AttackHitbox.cs
    // Author: Zach Lima
    // Date: 10/23/2025
    // Description: The basic script all things with health should use. 
    //=================================================================================
    public class AttackHitbox : MonoBehaviour
    {
       
        public delegate void BaseDelegate(GameObject entity);
       
        public BaseDelegate dealDamage;


        public static int _enemyLayer = 8;
        public static int _playerLayer = 7;

        
        // Start is called before the first frame update
        private void Awake()
        {
           
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //print(GetComponentInParent<EntityData>().GetComponentInChildren<Hurtbox>().gameObject.layer);
            // print(collision.gameObject.layer);

                //if we're a player and we're registering the enemy on hit
                if (GetComponentInParent<EntityData>().GetComponentInChildren<Hurtbox>().gameObject.layer == _playerLayer && collision.gameObject.layer == _enemyLayer) 
                {
                    dealDamage?.Invoke(collision.gameObject.transform.parent.gameObject);
                   // print("PLAYA");
                }

                //if we're an enemy and we're registering the player on hit
                if (GetComponentInParent<EntityData>().GetComponentInChildren<Hurtbox>().gameObject.layer == _enemyLayer && collision.gameObject.layer == _playerLayer)
                {
                    dealDamage?.Invoke(collision.gameObject.transform.parent.gameObject);
                    //print("ENEMY");
                }

            // GetComponentInParent<EntityData>().gameObject.
          


        }



    }
}
