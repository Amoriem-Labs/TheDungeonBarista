using System.Collections.Generic;
using System.Linq;
using TDB.CafeSystem.FurnitureSystem;
using TDB.ShopSystem.Framework;
using UnityEngine;

namespace TDB.ShopSystem.FurnitureShop
{
    public class TestFurnitureShopData : IShopData<FurnitureDefinition>
    {
        private List<FurnitureDefinition> _allFurnitures;

        public TestFurnitureShopData(List<FurnitureDefinition> allFurnitures)
        {
            _allFurnitures = allFurnitures;
        }

        public IEnumerable<ShopItemData<FurnitureDefinition>> AllItems =>
            _allFurnitures.Select(f => new FurnitureShopItemData(f, 1));
    }
    
    public class FurnitureShopItemData : ShopItemData<FurnitureDefinition>
    {
        public FurnitureShopItemData(FurnitureDefinition itemDefinition, int inStockCount) :
            base(itemDefinition, inStockCount)
        {
        }

        // TODO:
        public override int Price => 5;
        
        // TODO:
        protected override void HandlePurchase()
        {
            Debug.Log($"{ItemDefinition.DefinitionID} Purchased");
        }
    }
}