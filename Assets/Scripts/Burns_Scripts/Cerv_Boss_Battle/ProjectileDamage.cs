using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ProjectileDamage : MonoBehaviour
    {
        public float lifetime = 3f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // damage player here
            Destroy(gameObject);
        }
}
}
