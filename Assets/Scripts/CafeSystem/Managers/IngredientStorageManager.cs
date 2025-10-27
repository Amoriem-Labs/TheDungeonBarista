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
    }
}
