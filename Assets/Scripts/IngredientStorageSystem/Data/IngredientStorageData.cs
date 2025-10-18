using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.IngredientStorageSystem.Data
{
    /// <summary>
    /// Separate the storage data for one-day storage and refrigerator.
    /// Only merge them before sending to the craft system.
    /// </summary>
    [System.Serializable]
    public class IngredientStorageData
    {
        [field: TableList]
        [field: SerializeField]
        public List<IngredientStorageStackData> Ingredients { get; private set; }

        public Dictionary<IngredientDefinition, int> GetIngredientsCount =>
            Ingredients
                .GroupBy(i => i.Definition)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));
    }
}