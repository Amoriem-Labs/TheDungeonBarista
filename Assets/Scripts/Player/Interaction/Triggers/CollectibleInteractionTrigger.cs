using System;
using System.Collections.Generic;
using TDB.DungeonSystem.Collectibles;
using TDB.GameManagers.SessionManagers;
using TDB.InventorySystem.CollectibleInventory;
using TDB.InventorySystem.Framework;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    public class CollectibleInteractionTrigger : InteractionTrigger<Collectible>
    {
        private CollectibleInventoryData _collectibleInventory;
        public override string InteractionTip => "Pick Up";

        private void Start()
        {
            TryLoadInventory();
        }

        private void TryLoadInventory()
        {
            if (_collectibleInventory != null) return;
            
            var session = FindObjectOfType<SessionManager>();
            if (!session)
            {
                throw new NullReferenceException("SessionManager not found. Need to load Session scene.");
                return;
            }

            try
            {
                _collectibleInventory = session.CollectibleInventoryData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _collectibleInventory = new CollectibleInventoryData(new List<InventoryData<CollectibleDefinition>>());
            }
        }

        protected override void Interact(Collectible item)
        {
            TryLoadInventory();
            _collectibleInventory.Deposit(item.Definition);
            item.GetCollected();
        }
    }
}