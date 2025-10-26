// ===================================================================
// File: Mana.cs
// Author: Callum Legendre
// Date: October 25, 2025
// Description: Script is designed to be attached to the mana bar UI object.
// Handles mana calculations and updates it to the bar
// ===================================================================



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDB
{
    public class Mana : MonoBehaviour
    {
        // =============
        // Fields
        // ==============
        
        // intialisation of max mana and current mana fields, current is set to max in start function
        [SerializeField] int MaxMana = 100;
        public Slider ManaBar;
        
        // =======================
        // Unity Lifecycle Methods
        // =======================
        void Start()
        {
            // gets the slider component
            ManaBar = GetComponent<Slider>();
            
            // sets maximum and minimum value of the bar, along with only using whole numbers
            ManaBar.maxValue = MaxMana;
            ManaBar.minValue = 0;
            ManaBar.wholeNumbers = true;
            
            // spawns the player in with a full mana bar
            ManaBar.value = MaxMana;
            
            
        }
        
        void Update()
        {
        
        }
        
        // ==============
        // Public Methods
        // ==============
        
        // takes a positive integer, does not allow current mana to go below 0
        public void UseMana(int mana)
        {
            // checks if mana would go into negatives
            if (ManaBar.value - mana < 0)
            {
                // sets it to 0 instead if it would
                ManaBar.value = 0;
            }
            else
            {
                // otherwise reduce mana by specified amount
                ManaBar.value -=  mana;
            }
        }

        // takes a positive integer, does not allow current mana to go above max mana
        public void RecoverMana(int mana)
        {
            // checks if mana would go above the max mana value
            if (ManaBar.value + mana > MaxMana)
            {
                // sets it to max mana if so
                ManaBar.value = MaxMana;
            }
            else
            {
                // otherwise adds mana by specified amount
                ManaBar.value += mana;
            }

        }

        // ================
        // Private Methods
        // ================
    }
}
