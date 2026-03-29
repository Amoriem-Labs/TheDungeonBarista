using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.GameManagers.SessionManagers;
using TDB.ShopSystem.Framework;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.ShopSystem.IngredientShop
{
    [System.Serializable]
    public class IngredientShopData : IShopData<IngredientSource>
    {
        [SerializeField]
        private List<IngredientSource> _purchasableIngredients;
        [SerializeField]
        private int _shopSlots;
        [SerializeField, MinMaxSlider(1, 9999, true)]
        private Vector2Int _itemAmount;
        
        // needs to be set when requesting data
        private IngredientStorageManager _ingredientStorage;

        public IngredientShopData(IngredientShopData data)
        {
            _purchasableIngredients = new List<IngredientSource>(data._purchasableIngredients);
            _shopSlots = data._shopSlots;
            _itemAmount = data._itemAmount;
        }

        public void SetStorage(IngredientStorageManager storage) => _ingredientStorage = storage;

        public IEnumerable<ShopItemData<IngredientSource>> AllItems =>
            _purchasableIngredients.Shuffled()
                .Where((_, i) => i < _shopSlots)
                .Select(i =>
                    new IngredientShopItemData(i, Random.Range(_itemAmount.x, _itemAmount.y + 1), _ingredientStorage));
    }
    
    public class IngredientShopItemData : ShopItemData<IngredientSource>
    {
        private readonly IngredientStorageManager _ingredientStorage;

        // TODO: price
        public override int Price => 5;
        
        protected override void HandlePurchase()
        {
            _ingredientStorage.AddVolatileIngredient(_itemSource);
        }

        public IngredientShopItemData(IngredientSource itemSource, int inStockCount,
            IngredientStorageManager ingredientStorage) :
            base(itemSource, inStockCount)
        {
            _ingredientStorage = ingredientStorage;
        }

        private IngredientShopItemData(IngredientShopItemData data) : base(data)
        {
        }
    }
}