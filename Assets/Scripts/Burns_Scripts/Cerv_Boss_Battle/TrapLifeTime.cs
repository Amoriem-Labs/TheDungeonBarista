using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class TrapLifteTime : MonoBehaviour
    {
        public float lifetime = 3f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // INSERT PLAYER DAMAGE HERE
            Destroy(gameObject);
        }
}
}
