using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.Utils.EventChannels;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.Info
{
    [RequireComponent(typeof(UIEnabler))]
    public class IngredientInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Transform _effectItemContainer;
        
        [SerializeField] private EffectPairItemUI _effectPairItemPrefab;
        
        [Title("Events")]
        [SerializeField] private EventChannel _displayCraftMenuInventoryItemInfoEvent;
        [SerializeField] private EventChannel _displayIngredientInfoEvent;

        private List<EffectPairItemUI> _trackedItems = new();
        private UIEnabler _enabler;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
        }

        private void OnEnable()
        {
            _displayCraftMenuInventoryItemInfoEvent.AddListener<DisplayIngredientInfo>(HandleDisplayIngredientStack);
            _displayIngredientInfoEvent.AddListener<DisplayIngredientInfo>(HandleDisplayIngredient);
        }

        private void OnDisable()
        {
            _displayCraftMenuInventoryItemInfoEvent.RemoveListener<DisplayIngredientInfo>(HandleDisplayIngredientStack);
            _displayIngredientInfoEvent.RemoveListener<DisplayIngredientInfo>(HandleDisplayIngredient);
        }
        
        private void HandleDisplayIngredient(DisplayIngredientInfo info)
        {
            if (info.Ingredient == null)
            {
                _enabler.Disable(.2f);
                return;
            }
            
            _enabler.Enable(.2f);
            DisplayIngredientDefinition(info.Ingredient);
        }
        
        private void HandleDisplayIngredientStack(DisplayIngredientInfo stack)
        {
            if (stack.Ingredient == null)
            {
                _enabler.Disable(.2f);
                return;
            }
            
            _enabler.Enable(.2f);
            DisplayIngredientDefinition(stack.Ingredient);
            // TODO: maybe display stack info as well
        }

        private void DisplayIngredientDefinition(IngredientDefinition ingredient)
        {
            _headerText.text = ingredient.IngredientName;
            _iconImage.sprite = ingredient.IngredientSprite;

            int i = 0;
            for (; i < ingredient.Effects.Count; i++)
            {
                EffectPairItemUI item = null;
                if (i >= _trackedItems.Count)
                {
                    item = Instantiate(_effectPairItemPrefab, _effectItemContainer);
                    _trackedItems.Add(item);
                }
                else
                {
                    item = _trackedItems[i];
                    item.gameObject.SetActive(true);
                }
                
                var effect = ingredient.Effects[i];
                item.BindEffectParamPair(effect);
            }

            for (; i < _trackedItems.Count; i++)
            {
                _trackedItems[i].gameObject.SetActive(false);
            }
        }
    }

    public struct DisplayIngredientInfo
    {
        public IngredientDefinition Ingredient;
        public int InStock;
        public int Required;
    }
}