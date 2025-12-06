using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.IngredientStorage;
using UnityEngine;

namespace TDB.GameManagers.SessionManagers
{
    public class IngredientStorageManager : MonoBehaviour
    {
        private IngredientStorageData _volatileStorage;
        private IngredientStorageData _refrigeratedStorage;

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
    }
}
