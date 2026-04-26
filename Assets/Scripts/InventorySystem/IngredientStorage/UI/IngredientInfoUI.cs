using System;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.CraftSystem.UI.Info;
using TDB.Utils.UI;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class IngredientInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private Transform _effectItemContainer;
        
        [SerializeField] private EffectPairItemUI _effectPairItemPrefab;
        
        private InventoryInfoDisplayDirection _pivotControl;
        private RectTransform _rectTransform;
        
        private IngredientEffectListDisplay _effectListDisplay;
        private IngredientDefinition _ingredient;
        private UIEnabler _enabler;

        private void Awake()
        {
            _effectListDisplay = new IngredientEffectListDisplay(_effectItemContainer, _effectPairItemPrefab);
            _enabler = GetComponent<UIEnabler>();
            _pivotControl = GetComponentInChildren<InventoryInfoDisplayDirection>();
            _rectTransform = transform as RectTransform;
        }
        
        public void SetIngredient(IngredientDefinition ingredient)
        {
            _ingredient = ingredient;
            _headerText.text = ingredient.IngredientName;
            _effectListDisplay.DisplayIngredientEffectList(ingredient);
        }

        public void DisplayIngredientInfo(InventoryInfoDisplayData<IngredientDefinition> info)
        {
            _enabler.Enable();
            transform.position = info.RootPosition;
            _rectTransform.sizeDelta = info.RootSize;
            SetIngredient(info.Data);
        }

        public void StopDisplaying()
        {
            _enabler.Disable();
        }
    }
}
