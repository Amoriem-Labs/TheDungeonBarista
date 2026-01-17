using System;
using TDB.GameManagers.SessionManagers;
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
        private MoneyManager _moneyManager;

        protected virtual void Awake()
        {
            _shopUI = FindObjectOfType<ShopUI<T>>();
            _moneyManager = FindObjectOfType<MoneyManager>();
        }

        protected abstract IShopData<T> RequestShopData();

        #region Interactable

        public override void OpenShop(Action closeShopCallback)
        {
            base.OpenShop(closeShopCallback);

            // get shop data
            var shopData = RequestShopData();   
            _shopUI.OpenShop(shopData, _moneyManager, OnCloseShop);
        }

        #endregion
    }

    public class TestMoneyData : IResourceDataHolder
    {
        private int _money = 10;
        
        public int GetResource() => _money;

        public void SetResource(int amount)
        {
            _money = amount;
            OnResourceUpdate?.Invoke();
        }

        public Action OnResourceUpdate { get; set; }
    }
}