using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class BeamLife : MonoBehaviour
    {
        
        public int Damage = 5;

        void Start()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EntityData player = other.GetComponent<EntityData>();
                if (player != null)
                {
                    // INSERT DAMAGE HERE
                }
            }
        }
    }
}
