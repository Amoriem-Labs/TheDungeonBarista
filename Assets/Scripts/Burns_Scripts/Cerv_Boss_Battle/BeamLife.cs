using System.Collections;
using System.Collections.Generic;
using TDB.Damage;
using UnityEngine;

namespace TDB
{
    public class BeamLife : MonoBehaviour
    {
        public int damageAmount = 1;

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
            }
        }
    }
}