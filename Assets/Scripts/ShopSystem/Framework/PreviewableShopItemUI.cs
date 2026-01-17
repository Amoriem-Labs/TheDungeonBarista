using TDB.ShopSystem.UI;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    [RequireComponent(typeof(PreviewTrigger))]
    public class PreviewableShopItemUI<T> : ShopItemUI<T>
        where T : ScriptableObject, IShopItemDefinition, IPreviewableShopItemDefinition
    {
        private PreviewTrigger _previewTrigger;

        private void Awake()
        {
            var shopUI = GetComponentInParent<PreviewableShopUI<T>>();
            if (!shopUI)
            {
                Debug.LogError("PreviewableShopItemUI<T> needs to be a child of a PreviewableShopUI component");
                return;
            }

            _previewTrigger = GetComponent<PreviewTrigger>();
            _previewTrigger.BindUI(this, shopUI.PreviewPanel);
        }

        public override void BindItemData(ShopItemData<T> itemData, IResourceDataHolder moneyData)
        {
            _previewTrigger.BindData(itemData, itemData.ItemDefinition);
            
            // base implementation contains check purchasable
            base.BindItemData(itemData, moneyData);
        }
    }
}