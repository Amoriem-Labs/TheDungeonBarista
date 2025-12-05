using System.Collections.Generic;
using TDB.CafeSystem.FurnitureSystem;
using TDB.ShopSystem.Framework;
using UnityEngine;

namespace TDB.ShopSystem.FurnitureShop
{
    public class FurnitureShopController : ShopController<FurnitureDefinition>
    {
        [SerializeField] private List<FurnitureDefinition> _testAllFurnitures;
        
        protected override IShopData<FurnitureDefinition> RequestShopData()
        {
            return new TestFurnitureShopData(_testAllFurnitures);
        }
    }
}