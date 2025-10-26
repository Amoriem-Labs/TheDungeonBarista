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
        
        // temporary for testing health bar
        void Update()
        {
            // Press Space to take 10 damage
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(10f);
                Debug.Log("Took 10 damage, current HP: " + _currentHealth);
            }
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;

            // Makes sure HP doesn't go below 0
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                // Die();
            }
        }

        public void RegenHealth(float amount)
        {
            _currentHealth += amount;

            // Makes sure HP doesn't go above MaxHealth
            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
