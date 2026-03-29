using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.IngredientStorage
{
    [System.Serializable]
    public class IngredientStorageStackData : InventoryStackData<IngredientSource>
    {
        public IngredientStorageStackData(IngredientSource source) : base(source)
        {
        }

        public IngredientStorageStackData(InventoryStackData<IngredientSource> stack) : base(stack)
        {
        }
    }
}