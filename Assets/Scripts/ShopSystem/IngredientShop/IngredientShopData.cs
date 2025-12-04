using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.ShopSystem.IngredientShop
{
    [System.Serializable]
    public class IngredientShopData : ShopData<IngredientDefinition>
    {
        [SerializeField]
        private List<IngredientDefinition> _purchasableIngredients;
        [SerializeField]
        private int _shopSlots;
        [SerializeField, MinMaxSlider(1, 9999, true)]
        private Vector2Int _itemAmount;
        
        // needs to be set when requesting data
        private IngredientStorageManager _ingredientStorage;
        
        public void SetStorage(IngredientStorageManager storage) => _ingredientStorage = storage;

        public override IEnumerable<ShopItemData<IngredientDefinition>> AllItems =>
            _purchasableIngredients.Shuffled()
                .Where((_, i) => i < _shopSlots)
                .Select(i =>
                    new IngredientShopItemData(i, Random.Range(_itemAmount.x, _itemAmount.y), _ingredientStorage));
    }
    
    public class IngredientShopItemData : ShopItemData<IngredientDefinition>
    {
        private readonly IngredientStorageManager _ingredientStorage;

        // TODO: price
        public override int Price => 5;
        
        protected override void HandlePurchase()
        {
            _ingredientStorage.AddVolatileIngredient(ItemDefinition);
        }

        public IngredientShopItemData(IngredientDefinition itemDefinition, int inStockCount,
            IngredientStorageManager ingredientStorage) :
            base(itemDefinition, inStockCount)
        {
            _ingredientStorage = ingredientStorage;
        }
    }
}