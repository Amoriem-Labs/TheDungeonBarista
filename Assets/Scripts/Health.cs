// =====================================================================================
// File: Health.cs
// Author: Ryan Lin
// Date: October 25, 2025
// Description: Handles health, damage taken, and die features
// =====================================================================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        public float MaxHealth = 100f;
        private float _currentHealth;

        // Start is called before the first frame update
        void Start()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;

            // Makes sure HP doesn't go below 0
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }

            // Makes sure HP doesn't go above MaxHealth
            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
