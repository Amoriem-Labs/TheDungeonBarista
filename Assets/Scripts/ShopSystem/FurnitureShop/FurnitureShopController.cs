using System.Collections.Generic;
using TDB.CafeSystem.FurnitureSystem;
using TDB.GameManagers.SessionManagers;
using TDB.ShopSystem.Framework;
using UnityEngine;

namespace TDB.ShopSystem.FurnitureShop
{
    public class FurnitureShopController : ShopController<FurnitureDefinition>
    {
        [SerializeField] private List<FurnitureDefinition> _testAllFurnitures;
        private SessionManager _sessionManager;

        protected override void Awake()
        {
            base.Awake();

            _sessionManager = FindObjectOfType<SessionManager>();
        }

        protected override IShopData<FurnitureDefinition> RequestShopData()
        {
            var data = _sessionManager.FurnitureShopData;
            var inventory = _sessionManager.FurnitureInventoryData;
            data.SetInventory(inventory);
            return data;
        }
    }
}