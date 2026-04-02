using TDB.DungeonSystem;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    public class TeleporterInteractionTrigger : InteractionTrigger<Teleporter>
    {
        public override string InteractionTip => CurrentInteractable?.interactable?.InteractionTip ?? "Teleport";

        protected override void Interact(Teleporter teleporter)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            teleporter.Teleport(player);
        }
    }
}
