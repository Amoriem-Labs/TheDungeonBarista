using System;
using TDB.ShopSystem.Framework;
using TMPro;
using UnityEngine;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ItemPriceUI : MonoBehaviour
    {
        private IShopItemUI _itemUI;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _itemUI = GetComponentInParent<IShopItemUI>();
            _itemUI.OnBindItemData += HandleBindItemData;

            _text = GetComponent<TextMeshProUGUI>();
        }

        private void HandleBindItemData(IShopItemData data)
        {
            _text.text = IMoneyDataHolder.MoneyToString(data.Price);
        }
    }
}