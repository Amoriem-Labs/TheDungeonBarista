// =====================================================================================
// File: HealthBarUI.cs
// Author: Ryan Lin
// Date: October 26, 2025
// Description: Health UI / visual indicator
// =====================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDB
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        public Slider slider;
        public Health health;

        // Start is called before the first frame update
        void Start()
        {
            if (health == null)
            {
                health = FindObjectOfType<Health>();
            }

            slider.maxValue = health.MaxHealth;
            slider.value = health.MaxHealth;
            slider.minValue = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (health != null)
            {
                slider.value = health.GetCurrentHealth();
            }
        }
    }
}
