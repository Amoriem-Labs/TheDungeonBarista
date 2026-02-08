using System;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    
    public abstract class ShopItemUI<T> : MonoBehaviour, IShopItemUI where T : ScriptableObject, IShopItemDefinition
    {
        private ShopItemData<T> _boundItemData;
        
        private IResourceDataHolder _moneyData;
        
        public Action<IShopItemData> OnBindItemData { get; set; }
        public Action<bool> OnPurchasableUpdate{ get; set; }

        protected bool CanPurchase => _boundItemData.InStockCount > 0 && _boundItemData.Price <= _moneyData.GetResource();
        
        protected virtual void OnDisable()
        {
            if (_boundItemData != null)
            {
                _boundItemData = null;
            }
            
            if (_moneyData != null)
            {
                _moneyData.OnResourceUpdate -= CheckCanPurchase;
                _moneyData = null;
            }
        }

        public virtual void BindItemData(ShopItemData<T> itemData, IResourceDataHolder moneyData)
        {
            _boundItemData = itemData;

            _moneyData = moneyData;
            _moneyData.OnResourceUpdate += CheckCanPurchase;
            
            OnBindItemData?.Invoke(itemData);
            
            CheckCanPurchase();
        }

        protected virtual void CheckCanPurchase()
        {
            OnPurchasableUpdate?.Invoke(CanPurchase);
        }

        public void HandlePurchase()
        {
            if (!CanPurchase)
            {
                Debug.LogError("Can't purchase shop item. Should have disabled purchasing some where.");
                return;
            }

            var money = _moneyData.GetResource();
            var price = _boundItemData.Price;
            _boundItemData.Purchase();
            _moneyData.SetResource(money - price);
        }
    }
    
    public interface IShopItemUI
    {
        Action<IShopItemData> OnBindItemData { get; set; }
        Action<bool> OnPurchasableUpdate { get; set; }
        public void HandlePurchase();
    }
}