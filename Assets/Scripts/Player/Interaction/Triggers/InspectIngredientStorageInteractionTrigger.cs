using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using TDB.InventorySystem.IngredientStorage.UI;

namespace TDB.Player.Interaction.Triggers
{
    public class InspectIngredientStorageInteractionTrigger : InteractionTrigger<IngredientStorageEntryPoint>
    {
        private InventoryStorageUI _inventoryStorageUI;

        private void Awake()
        {
            _inventoryStorageUI = FindObjectOfType<InventoryStorageUI>();
        }

        protected override void Interact(IngredientStorageEntryPoint interactable)
        {
            ToggleBlockingPlayerInput(true);
            _inventoryStorageUI.Display(onExitMenu: () => ToggleBlockingPlayerInput(false));
        }

        public override string InteractionTip => "Ingredient Storage";
    }
}