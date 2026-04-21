using System.Collections;
using System.Collections.Generic;
using TDB.Damage;
using UnityEngine;

namespace TDB
{
    public class BeamLife : MonoBehaviour
    {
        public int damageAmount = 1;
        private bool hasHit = false; // prevent hitting every frame

        private void OnTriggerStay2D(Collider2D other)
        {
            if (hasHit) return;

            if (other.CompareTag("Player"))
            {
                var damageable = other.GetComponent<IDamageable>();
                if (damageable == null) return;

                damageable.TakeDamage(new DamageData()
                {
                    Amount = damageAmount,
                    DamageSourceLayer = gameObject.layer,
                });

                hasHit = true; // only damage once per beam instance
            }
        }

        // reset if you ever want the beam to hit again (e.g. continuous damage)
        private void OnDisable()
        {
            hasHit = false;
        }
    }
}