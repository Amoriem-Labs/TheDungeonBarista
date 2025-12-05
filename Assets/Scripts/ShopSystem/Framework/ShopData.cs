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

    public abstract class ShopItemData<T> : IShopItemData where T : ScriptableObject, IShopItemDefinition
    {
        public readonly T ItemDefinition;

        public int InStockCount { get; set; }

        public abstract int Price { get; }

        public void Purchase()
        {
            InStockCount--;
            HandlePurchase();
        }
        
        protected abstract void HandlePurchase();

        protected ShopItemData(T itemDefinition, int inStockCount)
        {
            ItemDefinition = itemDefinition;
            InStockCount = inStockCount;
        }
    }

    public interface IShopItemDefinition
    {
        
    }

    public interface IMoneyDataHolder
    {
        public int GetMoney();
        public void SetMoney(int money);

        public Action OnMoneyUpdate { get; set; }

        public static string MoneyToString(int money) => $"${money}";
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