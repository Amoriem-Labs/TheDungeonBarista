// =====================================================================================
// File: Health.cs
// Author: Ryan Lin
// Date: October 25, 2025
// Description: Handles health, damage taken, and die features


// NOTE: I (Callum Legendre) redid a bunch of the logic here, if you wanna see the old one look at the 'dungeon-hp-dmg' branch'
// =====================================================================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDB
{
    public class Health : MonoBehaviour
    {
        // =============
        // Fields
        // ==============

        // intialisation of max mana and current mana fields
        [SerializeField] int MaxHealth = 100;
        
        // the variable where the referance to the UI element is stored. All operations to adjust mana happen on the value element of the slider.
        [SerializeField] Slider HealthBar;

        // Start is called before the first frame update
        void Start()
        {
            // gets the slider component
            HealthBar = GetComponent<Slider>();
            
            // sets maximum and minimum value of the bar, along with only using whole numbers
            HealthBar.maxValue = MaxHealth;
            HealthBar.minValue = 0;
            HealthBar.wholeNumbers = true;
            
            // spawns the player in with a full health bar
            HealthBar.value = MaxHealth;
        }
        
        // temporary for testing health bar
        void Update()
        {

        }

        // ==============
        // Public Methods
        // ==============

        public void TakeDamage(float amount)
        {
            HealthBar.value -= amount;

            // Makes sure HP doesn't go below 0
            if (HealthBar.value <= 0)
            {
                HealthBar.value = 0;
                // Die();
            }
        }

        public void RegenHealth(float amount)
        {
            HealthBar.value += amount;

            // Makes sure HP doesn't go above MaxHealth
            if (HealthBar.value > MaxHealth) 
            {
                HealthBar.value = MaxHealth;
            }
        }

        public float GetCurrentHealth()
        {
            return HealthBar.value;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
