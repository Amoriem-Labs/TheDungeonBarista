using TDB.DungeonSystem.Collectibles;
using TDB.InventorySystem.CollectibleInventory;
using TDB.InventorySystem.Framework;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class CollectibleStackItemUIContainer : InventoryStackContainerUI<CollectibleDefinition>
    {
        public void BindAndDisplay(CollectibleInventoryData collectibleInventory)
        {
            Clear();
            SetInventory(collectibleInventory, hidePolicy: CheckShouldHide);
        }

        private bool CheckShouldHide(InventoryStackData<CollectibleDefinition> stack) => stack.Amount <= 0;
    }
}