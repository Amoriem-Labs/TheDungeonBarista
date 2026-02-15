using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    /// <summary>
    /// There will only be one or very few ShopDoors.
    /// The interaction logic is implemented in ShopDoor.
    /// </summary>
    public class ShopDoorInteractionTrigger : InteractionTrigger<ShopDoor>
    {
        public override string InteractionTip => CurrentInteractable?.interactable?.InteractionTip ?? "Cannot Interact";

        protected override void Interact(ShopDoor door) => door!.Interact();
    }
}