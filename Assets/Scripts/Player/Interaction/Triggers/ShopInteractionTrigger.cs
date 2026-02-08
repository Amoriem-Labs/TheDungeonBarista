using TDB.ShopSystem;
using TDB.ShopSystem.Framework;

namespace TDB.Player.Interaction.Triggers
{
    public class ShopInteractionTrigger : InteractionTrigger<ShopControllerBase>
    {
        protected override void Interact(ShopControllerBase shopController)
        {
            ToggleBlockingPlayerInput(true);
            shopController.OpenShop(() =>
            {
                ToggleBlockingPlayerInput(false);
            });
        }
    }
}