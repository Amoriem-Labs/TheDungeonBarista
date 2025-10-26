// ===================================================================
// File: Mana.cs
// Author: Callum Legendre
// Date: October 25, 2025
// Description: Handles the mana calculations for the player character
// ===================================================================



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class Mana : MonoBehaviour
    {
        // =============
        // Fields
        // ==============
        
        // intialisation of max mana and current mana fields, current is set to max in start function
        public int MaxMana = 100;
        public int CurrentMana;
        
        // =======================
        // Unity Lifecycle Methods
        // =======================
        void Start()
        {
            // spawns the player in with a full mana bar
            CurrentMana = MaxMana;
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
            if (CurrentMana - mana < 0)
            {
                // sets it to 0 instead if it would
                CurrentMana = 0;
            }
            else
            {
                // otherwise reduce mana by specified amount
                CurrentMana -=  mana;
            }
        }

        // takes a positive integer, does not allow current mana to go above max mana
        public void RecoverMana(int mana)
        {
            // checks if mana would go above the max mana value
            if (CurrentMana + mana > MaxMana)
            {
                // sets it to max mana if so
                CurrentMana = MaxMana;
            }
            else
            {
                // otherwise adds mana by specified amount
                CurrentMana += mana;
            }

        }

        // ================
        // Private Methods
        // ================
    }
}
