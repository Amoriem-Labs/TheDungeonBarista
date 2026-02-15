using TDB.CafeSystem.Customers;

namespace TDB.Player.Interaction.Triggers
{
    public class CustomerPreferenceInteractionTrigger : InteractionTrigger<CustomerPreferenceController>
    {
        public override string InteractionTip => "Ask for Order";

        protected override void Interact(CustomerPreferenceController customer)
        {
            customer.RevealPreferences();
        }
    }
}