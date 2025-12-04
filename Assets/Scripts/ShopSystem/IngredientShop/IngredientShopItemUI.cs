using TDB.CraftSystem.Data;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.IngredientShop
{
    public class IngredientShopItemUI : ShopItemUI<IngredientDefinition>, IUIClickHandler
    {
        [SerializeField] private Image _ingredientIcon;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private TextMeshProUGUI _ingredientName;
        
        public override void BindItemData(ShopItemData<IngredientDefinition> itemData, IMoneyDataHolder moneyData)
        {
            base.BindItemData(itemData, moneyData);

            _ingredientIcon.sprite = itemData.ItemDefinition.IngredientSprite;
            _ingredientName.text = itemData.ItemDefinition.IngredientName;
            _typeIcon.sprite = itemData.ItemDefinition.IngredientSprite;
        }

        protected override void CheckCanPurchase()
        {
            // TODO: update UI
        }

        public void OnUIClickStart() { }

        public void OnUIClickFinish()
        {
            if (!CanPurchase)
            {
                Debug.Log("Notify player cannot purchase.");
                return;
            }
            HandlePurchase();
        }
    }
}