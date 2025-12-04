using System;
using UnityEngine;

namespace TDB.ShopSystem
{
    public abstract class ShopItemUIBase : MonoBehaviour
    {
        public Action<ShopItemDataBase> OnBindItemData;
        public Action<bool> OnPurchasableUpdate;
    }
    
    public abstract class ShopItemUI<T> : ShopItemUIBase where T : ScriptableObject, IShopItemDefinition
    {
        private ShopItemData<T> _boundItemData;
        
        private IMoneyDataHolder _moneyData;
        
        private void OnDisable()
        {
            if (_boundItemData != null)
            {
                _boundItemData = null;
            }
            
            if (_moneyData != null)
            {
                _moneyData.OnMoneyUpdate -= CheckCanPurchase;
                _moneyData = null;
            }
        }

        public virtual void BindItemData(ShopItemData<T> itemData, IMoneyDataHolder moneyData)
        {
            _boundItemData = itemData;

            _moneyData = moneyData;
            _moneyData.OnMoneyUpdate += CheckCanPurchase;
            
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

            var money = _moneyData.GetMoney();
            var price = _boundItemData.Price;
            _boundItemData.Purchase();
            _moneyData.SetMoney(money - price);
        }

        protected bool CanPurchase => _boundItemData.InStockCount > 0 && _boundItemData.Price <= _moneyData.GetMoney();
    }
}