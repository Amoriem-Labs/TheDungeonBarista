using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;
using TDB.InventorySystem.IngredientStorage;
using TDB.Utils.EventChannels;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TDB.GameManagers.SessionManagers
{
    public class IngredientStorageManager : MonoBehaviour
    {
        private IngredientStorageData _volatileStorage;
        private IngredientStorageData _refrigeratedStorage;
        
        private IRefrigeratorCapacityCalculator _capacityCalculator;

        public int RefrigeratorCapacity => _capacityCalculator?.GetCapacity() ?? 0;

        public void InitializeStorages(IngredientStorageData volatileIngredientStorage,
            IngredientStorageData refrigeratedIngredientStorage)
        {
            _volatileStorage = volatileIngredientStorage;
            _refrigeratedStorage = refrigeratedIngredientStorage;
        }

        public void BindRefrigeratorCapacityManager(IRefrigeratorCapacityCalculator calculator)
        {
            _capacityCalculator = calculator;
        }

        /// <summary>
        /// This method provides a read-only copy of the merged storage.
        /// Any modification to the returned storage may not be reflected in the persisted data.
        /// </summary>
        /// <returns></returns>
        public IngredientStorageData GetMergedIngredientStorage()
        {
            return new IngredientStorageData(new List<InventoryData<IngredientSource>>
                { _volatileStorage, _refrigeratedStorage });
        }

        public IngredientStorageData VolatileIngredientStorage => _volatileStorage;

        public IngredientStorageData RefrigeratedIngredientStorage => _refrigeratedStorage;

        public bool TryConsume(Dictionary<IngredientSource, int> requirement)
        {
            // consume volatile storage first
            _volatileStorage.TryConsume(requirement);

            if (requirement.Values.Sum() >= 0)
            {
                // TODO: consume refrigerated storage if not satisfied yet 
            }
            
            // failed to supply all required ingredients
            return requirement.Values.Sum() >= 0;
        }

        public void AddVolatileIngredient(IngredientSource itemSource)
        {
            _volatileStorage.Deposit(itemSource);
        }

        public int GetVolatileIngredientEssence()
        {
            var storedIngredients = _volatileStorage.GetIngredientsCount;
            int essence = 0;
            foreach (var (ingredient, count) in storedIngredients)
            {
                essence += ingredient.GetEssence() * count;
            }
            return essence;
        }

        public void ClearVolatileIngredients()
        {
            _volatileStorage.Clear();
        }
    }

    public interface IRefrigeratorCapacityCalculator
    {
        public int GetCapacity();
    }
}
