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

        // intialisation of max mana and current mana fields
        [SerializeField] int MaxMana = 100;
        
        // the variable where the referance to the UI element is stored. All operations to adjust mana happen on the value element of the slider.
        [SerializeField] Slider ManaBar;
        
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
        
        // takes a positive integer, decreases mana by specified value
        public void UseMana(int mana)
        {
            // reduce mana by specified value
            ManaBar.value -= mana;
        }

        // takes a positive integer, decreases mana by specified value
        public void RecoverMana(int mana)
        {
            // increase mana by specified value
            ManaBar.value += mana;
        }

        // ================
        // Private Methods
        // ================
    }
}
