using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using TDB.InventorySystem.IngredientStorage.UI;

namespace TDB.Player.Interaction.Triggers
{
    public class InspectIngredientStorageInteractionTrigger : InteractionTrigger<IngredientStorageEntryPoint>
    {
        private IngredientStorageUI _ingredientStorageUI;

        private void Awake()
        {
            _ingredientStorageUI = FindObjectOfType<IngredientStorageUI>();
        }

        protected override void Interact(IngredientStorageEntryPoint interactable)
        {
            ToggleBlockingPlayerInput(true);
            _ingredientStorageUI.Display(onExitMenu: () => ToggleBlockingPlayerInput(false));
        }

        public override string InteractionTip => "Ingredient Storage";
    }
}