using TDB.CafeSystem.FurnitureSystem;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.FurnitureInventory
{
    [System.Serializable]
    public class FurnitureInventoryData : InventoryData<FurnitureDefinition>
    {
        public FurnitureInventoryData(InventoryData<FurnitureDefinition> inventoryData) : base(inventoryData)
        {
        }
    }
}