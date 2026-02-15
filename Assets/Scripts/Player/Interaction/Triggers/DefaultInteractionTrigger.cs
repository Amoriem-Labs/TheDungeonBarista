namespace TDB.Player.Interaction.Triggers
{
    public class DefaultInteractionTrigger : InteractionTrigger<DefaultInteractableHandler>
    {
        protected override void Interact(DefaultInteractableHandler interactable) => interactable.Interact();
    }
}