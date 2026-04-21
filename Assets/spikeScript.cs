using TDB.Damage;
using UnityEngine;

namespace TDB
{
    public class SpikeLife : MonoBehaviour
    {
        public int damageAmount = 1;
        public float damageCooldown = 0.5f; // seconds between each hit
        private float lastHitTime = -Mathf.Infinity;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (Time.time - lastHitTime < damageCooldown) return;

                var damageable = other.GetComponentInParent<IDamageable>() 
                              ?? other.GetComponentInChildren<IDamageable>();
                if (damageable == null) return;

                damageable.TakeDamage(new DamageData()
                {
                    Amount = damageAmount,
                    DamageSourceLayer = gameObject.layer,
                });

                lastHitTime = Time.time;
            }
        }
    }
}