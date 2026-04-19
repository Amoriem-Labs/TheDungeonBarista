using System.Collections;
using System.Collections.Generic;
using TDB.Damage;
using UnityEngine;

namespace TDB
{
    public class ProjectileDamage : MonoBehaviour
    {
        public float lifetime = 3f;
        public int damageAmount = 1;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var damageable = other.GetComponent<IDamageable>();
                if (damageable == null) return;

                damageable.TakeDamage(new DamageData()
                {
                    Amount = damageAmount,
                    DamageSourceLayer = gameObject.layer,
                });

                Destroy(gameObject);
            }

            if (other.CompareTag("SafeWallCollision"))
            {
                Destroy(gameObject);
            }
        }

}
}
