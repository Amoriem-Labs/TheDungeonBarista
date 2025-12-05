using System;
using System.Collections.Generic;
using System.Linq;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    /// <summary>
    /// This class is responsible for instantiate the shop item list and bind data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(UIEnabler))]
    public class ShopUI<T> : MonoBehaviour where T : ScriptableObject, IShopItemDefinition
    {
        [SerializeField] private ShopItemUI<T> _shopItemUIPrefab;
        [SerializeField] private Transform _itemParent;
        // TODO: maybe use a global money UI
        [SerializeField] private TextMeshProUGUI _moneyText;
        
        private readonly List<ShopItemUI<T>> _itemUIs = new List<ShopItemUI<T>>();
        private UIEnabler _enabler;
        private Action _closeShopCallback;
        private IMoneyDataHolder _moneyData;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
        }

        private void OnDisable()
        {
            if (_moneyData != null)
            {
                _moneyData.OnMoneyUpdate -= HandleMoneyUpdate;
                _moneyData = null;
            }
        }
        
        public void OpenShop(IShopData<T> shopData, IMoneyDataHolder moneyData, Action closeShopCallback)
        {
            // bind money UI
            _moneyData = moneyData;
            _moneyData.OnMoneyUpdate += HandleMoneyUpdate;
            HandleMoneyUpdate();
            
            // enable/create items on demand
            foreach (var (itemData, index) in shopData.AllItems.Select((x, i) => (x, i)))
            {
                AddItem(itemData, index);
            }

            _closeShopCallback = closeShopCallback;
            
            _enabler.Enable(.2f);
        }

        public void CloseShop()
        {
            _enabler.Disable(.2f)
                .onComplete += () =>
            {
                _closeShopCallback?.Invoke();
            
                // disable all items
                foreach (var itemUI in _itemUIs)
                {
                    itemUI.gameObject.SetActive(false);
                }
            };
        }

        private void HandleMoneyUpdate()
        {
            var money = _moneyData.GetMoney();
            _moneyText.text = IMoneyDataHolder.MoneyToString(money);
        }

        private void AddItem(ShopItemData<T> itemData, int index)
        {
            if (index >= _itemUIs.Count)
            {
                var newItemUI = Instantiate(_shopItemUIPrefab, _itemParent);
                _itemUIs.Add(newItemUI);
            }

            var itemUI = _itemUIs[index];
            itemUI.gameObject.SetActive(true);
            itemUI.BindItemData(itemData, _moneyData);
        }
    }
}