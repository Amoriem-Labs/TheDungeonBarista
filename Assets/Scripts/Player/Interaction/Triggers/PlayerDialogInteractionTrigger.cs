using System;

namespace TDB.Player.Interaction.Triggers
{
    public class PlayerDialogInteractionTrigger : InteractionTrigger<DialogControllerBase>
    {
        public override string InteractionTip => "Talk";
        protected override void Interact(DialogControllerBase interactable)
        {
            ToggleBlockingPlayerInput(true);
            var finishCallback = new Action(() =>
            {
                ToggleBlockingPlayerInput(false);
            });

            StartCoroutine(interactable.StartDialog(finishCallback));
        }
    }
}