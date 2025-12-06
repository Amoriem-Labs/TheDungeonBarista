using System;
using System.Collections.Generic;
using System.Linq;
using TDB.CafeSystem.FurnitureSystem;
using TDB.InventorySystem.FurnitureInventory;
using TDB.ShopSystem.Framework;
using UnityEngine;

namespace TDB.ShopSystem.FurnitureShop
{
    [System.Serializable]
    public class FurnitureShopData : FixedOptionsShopData<FurnitureDefinition, FurnitureShopItemData>
    {
        private FurnitureInventoryData _furnitureInventory;

        public FurnitureShopData(FurnitureShopData data) : base(data)
        {
        }

        protected override FurnitureShopItemData CreateNewEmptyStack(FurnitureDefinition definition)
        {
            return new FurnitureShopItemData(definition, 0, _furnitureInventory);
        }

        public void SetInventory(FurnitureInventoryData inventory)
        {
            _furnitureInventory = inventory;
            foreach (var item in AllItems)
            {
                var furnitureItem = (item as FurnitureShopItemData)!;
                furnitureItem.SetInventory(_furnitureInventory);
            }
        }
    }
    
    // public class TestFurnitureShopData : IShopData<FurnitureDefinition>
    // {
    //     private List<FurnitureDefinition> _allFurnitures;
    //
    //     public TestFurnitureShopData(List<FurnitureDefinition> allFurnitures)
    //     {
    //         _allFurnitures = allFurnitures;
    //     }
    //
    //     public IEnumerable<ShopItemData<FurnitureDefinition>> AllItems =>
    //         _allFurnitures.Select(f => new FurnitureShopItemData(f, 1));
    // }
    
    [System.Serializable]
    public class FurnitureShopItemData : ShopItemData<FurnitureDefinition>, IClonableShopItemData<FurnitureShopItemData>
    {
        private FurnitureInventoryData _furnitureInventory;

        public FurnitureShopItemData(FurnitureDefinition itemDefinition, int inStockCount,
            FurnitureInventoryData furnitureInventory) :
            base(itemDefinition, inStockCount)
        {
            _furnitureInventory = furnitureInventory;
        }

        public FurnitureShopItemData(FurnitureShopItemData data) : base(data)
        {
        }

        // TODO:
        public override int Price => 5;
        
        protected override void HandlePurchase()
        {
            if (_furnitureInventory == null)
            {
                Debug.LogError("Furniture Shop is not bound to Furniture Inventory");
                return;
            }
            _furnitureInventory.Deposit(ItemDefinition);
        }

        public FurnitureShopItemData Clone() => new FurnitureShopItemData(this);

        public void SetInventory(FurnitureInventoryData furnitureInventory)
        {
            _furnitureInventory = furnitureInventory;
        }
    }
}