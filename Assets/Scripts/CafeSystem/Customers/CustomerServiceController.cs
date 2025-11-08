using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CafeSystem.UI.ProductUI;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
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
            _customerController.BindOnCustomerDataUpdatedCallback(OnCustomerDataUpdated);
        }

        private void OnCustomerDataUpdated()
        {
            OnInteractableUpdated?.Invoke();
        }

        public ServeProductInfo ChooseAndServeFood(List<ProductData> productsToServe)
        {
            if (_customerController.Status != CustomerStatus.Waiting)
            {
                Debug.LogError("Can only serve food when the customer is Waiting.");
                return default;
            }
            
            // TODO: find the food the customer like the most from the list
            var servedProductIdx = 0;
            var servedProduct = productsToServe[servedProductIdx];
            
            // update data, which triggers interaction callback and StartEating function
            _customerController.Status = CustomerStatus.Eating;

            return ComputeOutcome(servedProduct);
        }

        private ServeProductInfo ComputeOutcome(ProductData product)
        {
            var bonusPerFlavorLevel = GameManager.Instance.GameConfig.BonusPerFlavorLevel;
            var punishmentPerFlavorLevel = GameManager.Instance.GameConfig.PunishmentPerFlavorLevel;
            var bonusPerQualityLevel = GameManager.Instance.GameConfig.BonusPerQualityLevel;

            var effects = product.RecipeData.GetAllEffectData();
            var preference =
                _customerController.Preferences.ToDictionary(
                    p => p.Flavor.EffectDefinition,
                    p => p.PreferenceLevel);

            var flavorBonusMultipliers = new Dictionary<EffectDefinition, float>();
            var totalFlavorMultiplier = 0f;
            foreach (var effect in effects)
            {
                if (!preference.TryGetValue(effect.Definition, out var preferenceLevel)) continue;
                var multiplier = preferenceLevel < 0 ? -punishmentPerFlavorLevel : bonusPerFlavorLevel;
                flavorBonusMultipliers.Add(effect.Definition, multiplier * Mathf.Abs(preferenceLevel));
                totalFlavorMultiplier += multiplier * Mathf.Abs(preferenceLevel);
            }
            
            // var qualityBonusMultiplier = product.QualityLevel * bonusPerQualityLevel;

            var basicPrice = product.RecipeData.GetBasicPrice();
            var finalPrice = Mathf.CeilToInt(basicPrice * (1 + totalFlavorMultiplier) * product.MinigamePriceMultiplier);

            // Debug.Log(
            //     $"Income from order {product.RecipeData.RecipeName} is " +
            //     $"{finalPrice} = {basicPrice} * (100% + {totalFlavorMultiplier:P0}) * ({product.MinigamePriceMultiplier:P0})");
            
            return new ServeProductInfo()
            {
                Product = product,
                Customer = _customerController.GetData(),
                FlavorMultipliers = flavorBonusMultipliers,
                TotalFlavorMultiplier = totalFlavorMultiplier,
                FinalPrice = finalPrice
            };
        }

        #region Interaction

        public bool IsInteractable => 
            _customerController.Status == CustomerStatus.Waiting
            // TODO: to simplify things, must ask for order before serving
            && _customerController.IsPreferenceRevealed;
        
        public Action OnInteractableUpdated { get; set; }
        public void SetReady() => _customerController.SetReady(this);

        public void SetNotReady() => _customerController.SetNotReady(this);

        #endregion
    }
}