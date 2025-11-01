using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.Player.Interaction;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.CafeSystem.Customers
{
    /// <summary>
    /// Controls how to serve a customer.
    /// </summary>
    [RequireComponent(typeof(CustomerController))]
    public class CustomerServiceController : MonoBehaviour, IInteractable
    {
        private CustomerController _customerController;

        private void Awake()
        {
            _customerController = GetComponent<CustomerController>();
            _customerController.BindOnCustomerDataUpdatedCallback(OnInteractableUpdated);
        }
        
        public int ChooseAndServeFood(List<ProductData> productsToServe)
        {
            if (_customerController.Status != CustomerStatus.Waiting)
            {
                Debug.LogError("Can only serve food when the customer is Waiting.");
                return -1;
            }
            
            // TODO: find the food the customer like the most from the list
            var servedProductIdx = 0;
            var servedProduct = productsToServe[servedProductIdx];
            
            // compute income
            ComputeIncome(servedProduct);
            
            // update data, which triggers interaction callback and StartEating function
            _customerController.Status = CustomerStatus.Eating;

            return servedProductIdx;
        }

        private void ComputeIncome(ProductData product)
        {
            var bonusPerFlavorLevel = GameManager.Instance.GameConfig.BonusPerFlavorLevel;
            var punishmentPerFlavorLevel = GameManager.Instance.GameConfig.PunishmentPerFlavorLevel;
            var bonusPerQualityLevel = GameManager.Instance.GameConfig.BonusPerQualityLevel;

            var effects = product.RecipeData.GetAllEffectData();
            var preference =
                _customerController.Preferences.ToDictionary(
                    p => p.Flavor.EffectDefinition,
                    p => p.PreferenceLevel);

            var flavorBonusMultiplier = 0f;
            foreach (var effect in effects)
            {
                if (!preference.TryGetValue(effect.Definition, out var preferenceLevel)) continue;
                var multiplier = preferenceLevel < 0 ? punishmentPerFlavorLevel : bonusPerFlavorLevel;
                flavorBonusMultiplier += multiplier * Mathf.Abs(preferenceLevel);
            }
            
            var qualityBonusMultiplier = product.QualityLevel * bonusPerQualityLevel;

            var basicPrice = product.RecipeData.GetBasicPrice();
            var finalPrice = basicPrice * (1 + flavorBonusMultiplier) * (1 + qualityBonusMultiplier);

            // TODO: display the result
            Debug.Log(
                $"Income from order {product.RecipeData.RecipeName} is " +
                $"{finalPrice} = {basicPrice} * (100% + {flavorBonusMultiplier:P0}) * (100% + {qualityBonusMultiplier:P0})");
        }

        #region Interaction

        public bool IsInteractable => 
            _customerController.Status == CustomerStatus.Waiting
            // TODO: to simplify things, must ask for order before serving
            && _customerController.IsPreferenceRevealed;
        
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // update sprite
            _customerController.OutlineController.ToggleOutline(true, this);
        }

        public void SetNotReady()
        {
            // update sprite
            _customerController.OutlineController.ToggleOutline(false, this);
        }

        #endregion
    }
}