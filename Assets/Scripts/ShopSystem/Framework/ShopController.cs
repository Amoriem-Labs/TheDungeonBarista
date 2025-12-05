using System;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    public abstract class ShopControllerBase : MonoBehaviour, IInteractable
    {
        private Action _closeShopCallback;

        public virtual void OpenShop(Action closeShopCallback)
        {
            _closeShopCallback = closeShopCallback;
        }

        protected void OnCloseShop()
        {
            _closeShopCallback?.Invoke();
        }
        
        public virtual bool IsInteractable => true;
        public Action OnInteractableUpdated { get; set; }

        public void SetReady()
        {
            // TODO:
        }

        public void SetNotReady()
        {
            // TODO:
        }
    }

    public abstract class ShopController<T> : ShopControllerBase where T : ScriptableObject, IShopItemDefinition
    {
        private ShopUI<T> _shopUI;

        protected virtual void Awake()
        {
            _shopUI = FindObjectOfType<ShopUI<T>>();
        }

        protected abstract IShopData<T> RequestShopData();

        #region Interactable

        public override void OpenShop(Action closeShopCallback)
        {
            base.OpenShop(closeShopCallback);

            
            // TODO: get money data
            var moneyData = new TestMoneyData();
            // get shop data
            var shopData = RequestShopData();   
            _shopUI.OpenShop(shopData, moneyData, OnCloseShop);
        }

        #endregion
    }

    public class TestMoneyData : IMoneyDataHolder
    {
        private int _money = 10;
        
        public int GetMoney() => _money;

        public void SetMoney(int money)
        {
            _money = money;
            OnMoneyUpdate?.Invoke();
        }

        public Action OnMoneyUpdate { get; set; }
    }
}