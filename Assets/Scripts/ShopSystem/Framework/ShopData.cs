using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    public interface IShopData<T> where T : ScriptableObject, IShopItemDefinition
    {
        public IEnumerable<ShopItemData<T>> AllItems { get; }
    }

    public interface IShopItemData
    {
        public int InStockCount { get; set; }
        public int Price { get; }
        public void Purchase();
    }

    [System.Serializable]
    public abstract class ShopItemData<T> : IShopItemData where T : ScriptableObject, IShopItemDefinition
    {
        [SerializeField]
        public T _itemSource;

        [field: SerializeField]
        public int InStockCount { get; set; }

        public abstract int Price { get; }

        public void Purchase()
        {
            InStockCount--;
            HandlePurchase();
        }
        
        protected abstract void HandlePurchase();

        public ShopItemData(T itemSource, int inStockCount)
        {
            _itemSource = itemSource;
            InStockCount = inStockCount;
        }

        public ShopItemData(ShopItemData<T> data)
        {
            _itemSource = data._itemSource;
            InStockCount = data.InStockCount;
        }
    }

    public interface IClonableShopItemData<T>
    {
        public T Clone();
    }

    public interface IShopItemDefinition
    {
        
    }

    public interface IResourceDataHolder
    {
        public int GetResource();
        public void SetResource(int amount);

        public Action OnResourceUpdate { get; set; }
    }
    
    // assume only 1 type of resource: money
    
    // possible shop items:
    //      battle equipment
    //      furnitures
    //      ingredients
    //      recipes
    //      employee
    
    // Shop <- ShopData <- ShopItems
}