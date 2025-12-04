using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.IngredientStorageSystem.Data;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    public class IngredientStorageManager : MonoBehaviour
    {
        private IngredientStorageData _volatileStorage;
        
        public void InitializeVolatileStorage(IngredientStorageData volatileIngredientStorage)
        {
            _volatileStorage = volatileIngredientStorage;
        }

        public IngredientStorageData GetAllIngredientStorage()
        {
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
            _volatileStorage.AddIngredient(itemDefinition);
        }
    }
}
