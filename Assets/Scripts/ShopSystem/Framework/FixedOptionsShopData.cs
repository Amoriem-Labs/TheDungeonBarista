using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using TDB.CafeSystem.FurnitureSystem;
using TDB.ShopSystem.FurnitureShop;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    [System.Serializable]
    public abstract class FixedOptionsShopData<TO, T1> : IShopData<TO>
        where TO : ResourceScriptableObject, IShopItemDefinition
        where T1 : ShopItemData<TO>, IClonableShopItemData<T1>
    {
        [SerializeField] private List<T1> _availableItems;
        
        public IEnumerable<ShopItemData<TO>> AllItems => _availableItems;

        public void AddPurchasableItem(TO definition, int amount)
        {
            var targetStack = _availableItems.Find(f => f.ItemDefinition == definition);
            if (targetStack == null)
            {
                targetStack = CreateNewEmptyStack(definition);
                _availableItems.Add(targetStack);
            }

            targetStack.InStockCount += amount;
        }

        protected FixedOptionsShopData(FixedOptionsShopData<TO, T1> data) =>
            _availableItems = data._availableItems.Select(i => i.Clone()).ToList();

        protected abstract T1 CreateNewEmptyStack(TO definition);
    }
}