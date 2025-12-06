using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.GameManagers.SessionManagers;
using TDB.ShopSystem.Framework;
using UnityEngine;

namespace TDB.ShopSystem.IngredientShop
{
    public class IngredientShopController : ShopController<IngredientDefinition>
    {
        private IngredientStorageManager _ingredientStorage;
        private SessionManager _sessionManager;

        protected override void Awake()
        {
            base.Awake();
            
            _ingredientStorage = FindObjectOfType<IngredientStorageManager>();
            _sessionManager = FindObjectOfType<SessionManager>();
        }

        protected override IShopData<IngredientDefinition> RequestShopData()
        {
            var data = _sessionManager.IngredientShopData;
            data.SetStorage(_ingredientStorage);
            return data;
        }
    }
}