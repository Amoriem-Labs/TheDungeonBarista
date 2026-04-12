using System.Collections.Generic;
using TDB.DungeonSystem.Collectibles;
using TDB.InventorySystem.Framework;
using UnityEngine;

namespace TDB.InventorySystem.CollectibleInventory
{
    [System.Serializable]
    public class CollectibleInventoryData : InventoryData<CollectibleDefinition>
    {
        protected CollectibleInventoryData(InventoryData<CollectibleDefinition> inventoryData) : base(inventoryData)
        {
        }

        public CollectibleInventoryData(List<InventoryData<CollectibleDefinition>> inventories) : base(inventories)
        {
        }

        public CollectibleInventoryData(CollectibleInventoryData collectibleInventoryData) : base(collectibleInventoryData)
        {
        }
    }
}