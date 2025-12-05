using TDB.CafeSystem.FurnitureSystem;
using TDB.ShopSystem.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.FurnitureShop
{
    public class FurnitureShopItemUI : PreviewableShopItemUI<FurnitureDefinition>
    {
        [SerializeField] private TextMeshProUGUI _furnitureName;

        public override void BindItemData(ShopItemData<FurnitureDefinition> itemData, IMoneyDataHolder moneyData)
        {
            base.BindItemData(itemData, moneyData);

            _furnitureName.text = itemData.ItemDefinition.FurnitureName;
        }
    }
}