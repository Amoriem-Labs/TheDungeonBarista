using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.ShopSystem
{
    public abstract class ShopData<T> where T : ScriptableObject, IShopItemDefinition
    {
        public abstract IEnumerable<ShopItemData<T>> AllItems { get; }
    }

    public abstract class ShopItemDataBase
    {
        public int InStockCount { get; protected set; }
        public abstract int Price { get; }
    }

    public abstract class ShopItemData<T> : ShopItemDataBase where T : ScriptableObject, IShopItemDefinition
    {
        public readonly T ItemDefinition;
        
        protected abstract void HandlePurchase();
        
        public void Purchase()
        {
            InStockCount--;
            HandlePurchase();
        }

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