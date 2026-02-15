using TDB.MapSystem.Passages;

namespace TDB.Player.Interaction.Triggers
{
    public class PassageEntranceInteractionTrigger : InteractionTrigger<PassageEntrance>
    {
        public override string InteractionTip => CurrentInteractable?.interactable.EnterTipText ?? "Cannot Interact";

        protected override void Interact(PassageEntrance entrance) => entrance!.EnterPassage();
    }
}