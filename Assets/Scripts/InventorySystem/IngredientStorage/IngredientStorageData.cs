using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.IngredientStorage
{
    /// <summary>
    /// Separate the storage data for one-day storage and refrigerator.
    /// Only merge them before sending to the craft system.
    /// </summary>
    [System.Serializable]
    public class IngredientStorageData : InventoryData<IngredientDefinition>
    {
        public IngredientStorageData(IngredientStorageData ingredientStorage) : base(ingredientStorage)
        {
        }

        public Dictionary<IngredientDefinition, int> GetIngredientsCount =>
            Stacks
                .GroupBy(i => i.Definition)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

        public int TotalIngredients => Stacks.Sum(s => s.Amount);
    }
}