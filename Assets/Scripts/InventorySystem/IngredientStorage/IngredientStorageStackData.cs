using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.IngredientStorage
{
    [System.Serializable]
    public class IngredientStorageStackData : InventoryStackData<IngredientDefinition>
    {
        public IngredientStorageStackData(IngredientDefinition definition) : base(definition)
        {
        }
    }
}