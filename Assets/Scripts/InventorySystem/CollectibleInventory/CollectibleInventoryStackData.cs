using TDB.DungeonSystem.Collectibles;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.CollectibleInventory
{
    public class CollectibleInventoryStackData : InventoryStackData<CollectibleDefinition>
    {
        public CollectibleInventoryStackData(CollectibleDefinition source) : base(source)
        {
        }

        public CollectibleInventoryStackData(InventoryStackData<CollectibleDefinition> stack) : base(stack)
        {
        }
    }
}