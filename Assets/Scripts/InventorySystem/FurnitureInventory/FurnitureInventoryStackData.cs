using TDB.CafeSystem.FurnitureSystem;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.FurnitureInventory
{
    [System.Serializable]
    public class FurnitureInventoryStackData : InventoryStackData<FurnitureDefinition>  
    {
        public FurnitureInventoryStackData(FurnitureDefinition definition) : base(definition)
        {
        }

        public FurnitureInventoryStackData(InventoryStackData<FurnitureDefinition> stack) : base(stack)
        {
        }
    }
}