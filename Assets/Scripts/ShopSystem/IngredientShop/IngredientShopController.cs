using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using UnityEngine;

namespace TDB.ShopSystem.IngredientShop
{
    public class IngredientShopController : ShopController<IngredientDefinition>
    {
        private IngredientStorageManager _ingredientStorage;

        protected override void Awake()
        {
            base.Awake();
            
            _ingredientStorage = FindObjectOfType<IngredientStorageManager>();
        }

        protected override ShopData<IngredientDefinition> RequestShopData()
        {
            var data = GameManager.Instance.GameConfig.TestIngredientShopData;
            data.SetStorage(_ingredientStorage);
            return data;
        }
    }
}