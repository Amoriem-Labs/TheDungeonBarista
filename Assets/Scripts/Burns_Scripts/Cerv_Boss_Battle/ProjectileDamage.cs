using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ProjectileDamage : MonoBehaviour
    {
        public float lifetime = 3f;
        public float damage = 1f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // INSERT PLAYER DAMAGE HERE
            Debug.Log("WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            Destroy(gameObject);
        }

}
}
