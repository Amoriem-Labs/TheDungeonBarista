using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: PlayerMovement.cs
    // Author: Zach Lima
    // Date: 1/25/2026
    // Description: Projectile Data
    //=================================================================================

    public class Projectile : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float maxSpeed = 5f;
        public float lifeSpan = 2f;
        private float currentLifeLeft;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            currentLifeLeft = lifeSpan;
           
            
        }


        public void Update()
        {
            currentLifeLeft -= Time.deltaTime;
            if (currentLifeLeft <= 0)
            {
                
                Destroy(gameObject);
            }
           
                
            
            
        }

       
        // Update is called once per frame
     
    }
}
