using DG.Tweening;
using TDB.Utils.Misc;
using TMPro;
using UnityEngine;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ItemSoldOutText : MonoBehaviour
    {
        private ShopItemUIBase _itemUI;
        private TextMeshProUGUI _text;
        private ShopItemDataBase _itemData;

        private void Awake()
        {
            _itemUI = GetComponentInParent<ShopItemUIBase>();
            _itemUI.OnBindItemData += HandleBindItemData;
            _itemUI.OnPurchasableUpdate += HandlePurchasableUpdate;

            _text = GetComponent<TextMeshProUGUI>();
            _text.color = _text.color.SetAlpha(0);
        }

        private void OnDisable()
        {
            _itemData = null;
        }

        private void HandlePurchasableUpdate(bool _)
        {
            if (_itemData == null) return;
            _text.DOKill();
            _text.DOFade(_itemData.InStockCount > 0 ? 0 : 1, 0.2f);
        }

        private void HandleBindItemData(ShopItemDataBase data)
        {
            _itemData = data;
            HandlePurchasableUpdate(true);
        }
    }
}