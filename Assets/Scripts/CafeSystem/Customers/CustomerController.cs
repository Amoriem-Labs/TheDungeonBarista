using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.GameManagers;
using TDB.Utils.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDB.CafeSystem.Customers
{
    /// <summary>
    /// Top-level controller of the customer and holds the CustomerData
    /// </summary>
    public class CustomerController : MonoBehaviour
    {
        [SerializeField, ReadOnly, InlineProperty, HideLabel, BoxGroup("Customer Data")]
        private CustomerData _customerData;

        public System.Action OnCustomerDataUpdated;

        public CustomerData Data
        {
            get => _customerData;
            set
            {
                _customerData = value;
                OnCustomerDataUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Handles the initialization of the customer.
        /// Invoked by customer spawner to initialize data and start behavior.
        /// </summary>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void SpawnCustomer()
        {
            // initialize data
            InitializeRandomData();
            
            // TODO: walk to a table before entering the Waiting state
            Data.Status = CustomerStatus.Waiting;
            OnCustomerDataUpdated?.Invoke();
        }
        
        private void InitializeRandomData()
        {
            var allFlavors = GameManager.Instance.GameConfig.AllFlavors.ToList();
            allFlavors.Shuffle();
            var currentFlavorIdx = 0;

            var preferences = new List<CustomerPreferenceData>();
            var (minLevel, maxLevel) = (-3, 3);
            var (minStep, maxStep) = (2, 3);
            for (int currentLevel = Random.Range(minLevel, 0);
                 currentLevel <= maxLevel && currentFlavorIdx < allFlavors.Count;
                 currentLevel += Random.Range(minStep, maxStep + 1))
            {
                if (currentLevel == 0)
                {
                    // instead of randomly decide, always go up
                    // currentLevel = Random.Range(0f, 1f) < 0.5f ? -1 : 1;
                    currentLevel = 1;
                }
                preferences.Add(new CustomerPreferenceData()
                {
                    Flavor = allFlavors[currentFlavorIdx],
                    PreferenceLevel = currentLevel
                });
                currentFlavorIdx++;
            }
            
            Data = new CustomerData(preferences);
        }
    }
}