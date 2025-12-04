using System;
using TMPro;
using UnityEngine;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ItemPriceUI : MonoBehaviour
    {
        private ShopItemUIBase _itemUI;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _itemUI = GetComponentInParent<ShopItemUIBase>();
            _itemUI.OnBindItemData += HandleBindItemData;

            _text = GetComponent<TextMeshProUGUI>();
        }

        private void HandleBindItemData(ShopItemDataBase data)
        {
            _text.text = IMoneyDataHolder.MoneyToString(data.Price);
        }
    }
}