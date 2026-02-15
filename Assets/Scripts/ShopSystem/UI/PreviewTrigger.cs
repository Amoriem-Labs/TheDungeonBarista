using System;
using TDB.ShopSystem.Framework;
using TDB.Utils.UI.UIHover;
using UnityEngine;

namespace TDB.ShopSystem.UI
{
    public class PreviewTrigger : MonoBehaviour, IUIClickHandler
    {
        [SerializeField] private PreviewItem _previewItemPrefab;

        private PreviewItem _previewItem;
        private PreviewPanel _previewPanel;
        private IShopItemData _itemData;
        private bool _purchasable;
        private IShopItemUI _itemUI;
        
        public bool Purchasable => _purchasable;
        public bool IsSoldOut => _itemData.InStockCount <= 0;
        public int Price => _itemData.Price;

        private void Reset()
        {
            if (!_previewItemPrefab)
            {
                _previewItemPrefab = Resources.Load<PreviewItem>("Prefabs/UI/ShopUI/PreviewShopUI Template/PreviewItem");
            }
        }

        private void OnDisable()
        {
            if (_previewItem)
            {
                Destroy(_previewItem.gameObject);
                _previewItem = null;
            }
        }
        
        public void BindUI(IShopItemUI itemUI, PreviewPanel previewPanel)
        {
            _itemUI = itemUI;
            _itemUI.OnPurchasableUpdate += HandlePurchasableUpdate;
            _previewPanel = previewPanel;
        }

        public void BindData(IShopItemData itemData, IPreviewableShopItemDefinition itemDefinition)
        {
            _itemData = itemData;
            _previewItem = Instantiate(_previewItemPrefab, _previewPanel.transform);
            _previewItem.transform.SetAsFirstSibling();
            _previewItem.BindItem(itemDefinition);
            _previewItem.gameObject.SetActive(false);
        }

        private void HandlePurchasableUpdate(bool purchasable)
        {
            _purchasable = purchasable;
            
            // current item is not selected
            if (!_previewItem.gameObject.activeSelf) return;

            _previewPanel.HandlePurchasableUpdate();
        }

        public void OnUIClickStart() { }

        public void OnUIClickFinish()
        {
            _previewPanel.HideCurrentTrigger();
            _previewItem.gameObject.SetActive(true);
            _previewPanel.SetCurrentTrigger(this);
        }

        public void HideItem()
        {
            _previewItem.gameObject.SetActive(false);
        }

        public void HandlePurchase()
        {
            _itemUI.HandlePurchase();
        }
    }
}