using System.Collections;
using System.Collections.Generic;
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


        private const int _enemyLayer = 8;
        private const int _playerLayer = 7;
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
            if (collision.gameObject.layer == _enemyLayer)
            {
                dealDamage?.Invoke(collision.gameObject.transform.parent.gameObject);
            }
            

        }



    }
}
