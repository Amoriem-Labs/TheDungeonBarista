using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.IngredientStorage;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.GameManagers.SessionManagers
{
    public class IngredientStorageManager : MonoBehaviour
    {
        private IngredientStorageData _volatileStorage;
        private IngredientStorageData _refrigeratedStorage;

        // TODO:
        public int RefrigeratorCapacity => 5;

        public void InitializeStorages(IngredientStorageData volatileIngredientStorage,
            IngredientStorageData refrigeratedIngredientStorage)
        {
            _volatileStorage = volatileIngredientStorage;
            _refrigeratedStorage = refrigeratedIngredientStorage;
        }

        public IngredientStorageData GetAllIngredientStorage()
        {
            // TODO: merge with _refrigeratedStorage
            return _volatileStorage;
        }

        public IngredientStorageData GetVolatileIngredientStorage() => _volatileStorage;
        
        public IngredientStorageData GetRefrigeratedIngredientStorage() => _refrigeratedStorage;

        public bool TryConsume(Dictionary<IngredientDefinition, int> requirement)
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

        public void AddVolatileIngredient(IngredientDefinition itemDefinition)
        {
            _volatileStorage.Deposit(itemDefinition);
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
}
