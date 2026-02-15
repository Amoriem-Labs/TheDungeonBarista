using System;
using TDB.CraftSystem.Data;
using TDB.ShopSystem.Framework;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.IngredientShop
{
    public class IngredientShopItemUI : ShopItemUI<IngredientDefinition>
    {
        [SerializeField] private Image _ingredientIcon;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private TextMeshProUGUI _ingredientName;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandlePurchase);
        }

        public override void BindItemData(ShopItemData<IngredientDefinition> itemData, IResourceDataHolder moneyData)
        {
            base.BindItemData(itemData, moneyData);

            _ingredientIcon.sprite = itemData.ItemDefinition.IngredientSprite;
            _ingredientName.text = itemData.ItemDefinition.IngredientName;
            _typeIcon.sprite = itemData.ItemDefinition.Type.Icon;
        }

        protected override void CheckCanPurchase()
        {
            base.CheckCanPurchase();
            
            // update UI
            _button.interactable = CanPurchase;
        }
    }
}