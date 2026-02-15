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

        private EntityData _entityData;

        public BaseDelegate dealDamage;


        public static int _enemyLayer = 8;
        public static int _playerLayer = 7;

        
        // Start is called before the first frame update
        private void Awake()
        {
            //_entityData = GetComponent<EntityData>();

            //var hitbox = GetComponentInChildren<AttackHitbox>();
            //if (hitbox == null)
            //    Debug.LogError("Enemy has no AttackHitbox attached in children!");

            //hitbox.dealDamage += _entityData.DealDamage;

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

            ////if we're a player and we're registering the enemy on hit
            //if (GetComponentInParent<EntityData>().GetComponentInChildren<Hurtbox>().gameObject.layer == _playerLayer && collision.gameObject.layer == _enemyLayer)
            //{
            //    dealDamage?.Invoke(collision.gameObject.transform.parent.gameObject);
            //    // print("PLAYA");
            //}

            ////if we're an enemy and we're registering the player on hit
            //if (GetComponentInParent<EntityData>().GetComponentInChildren<Hurtbox>().gameObject.layer == _enemyLayer && collision.gameObject.layer == _playerLayer)
            //{
            //    dealDamage?.Invoke(collision.gameObject.transform.parent.gameObject);
            //    //print("ENEMY");
            //}

            //// GetComponentInParent<EntityData>().gameObject.
   

            EntityData ownerData = GetComponentInParent<EntityData>();
            EntityData targetData = collision.GetComponentInParent<EntityData>();

    

            if (ownerData == null || targetData == null)
                return;

            int ownerLayer = ownerData.GetComponentInChildren<Hurtbox>().gameObject.layer;
            int targetLayer = targetData.GetComponentInChildren<Hurtbox>().gameObject.layer;

            bool playerHitsEnemy = ownerLayer == _playerLayer && targetLayer == _enemyLayer;
            bool enemyHitsPlayer = ownerLayer == _enemyLayer && targetLayer == _playerLayer;

            if (playerHitsEnemy || enemyHitsPlayer)
            {
                targetData.CurrentHealth -= 1;
                //Debug.Log($"Damage applied to {targetData.name}, CurrentHealth={targetData.CurrentHealth}");
                if (targetData.CurrentHealth <= 0)
                    Destroy(targetData.gameObject);
            }
        }
    }
}
