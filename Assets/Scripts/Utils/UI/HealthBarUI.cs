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
        public EntityData entity;

        // Start is called before the first frame update
        void Start()
        {
            if (entity == null)
            {
                entity = FindObjectOfType<EntityData>();
            }

            slider.maxValue = entity.MaxHealth;
            slider.value = entity.CurrentHealth;
            slider.minValue = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            slider.value = entity.CurrentHealth;
        }
    }
}
