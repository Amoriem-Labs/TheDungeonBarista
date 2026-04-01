using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI.Info;
using TDB.CraftSystem.UI.RecipeGraph;
using TDB.Utils.EventChannels;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.Inventory
{
    public class CraftMenuInventoryItemUI : MonoBehaviour, IUIHoverHandler
    {
        [FormerlySerializedAs("_inStockNumber")] [SerializeField] private TextMeshProUGUI _inStockNumberText;
        [FormerlySerializedAs("_requireNumber")] [SerializeField] private TextMeshProUGUI _requireNumberText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _typeIconImage;
        [SerializeField] private Image _dragIconImage;
        
        [Title("Events")]
        [SerializeField] private EventChannel _displayCraftMenuInventoryItemInfoEvent;
        
        private IngredientDefinition _ingredient;
        private int _inStockNumber;
        private int _requiredNumber;
        private CraftMenuInventoryUI _inventoryUI;

        public void SetInStockNumber(int number)
        {
            _inStockNumber = number;
            _inStockNumberText.text = _inStockNumber.ToString();
        }

        public void SetRequiredNumber(int number)
        {
            _requiredNumber = number;
            _requireNumberText.text = _requiredNumber.ToString();
        }

        public void BindData(IngredientDefinition ingredient, CraftMenuInventoryUI craftMenuInventoryUI)
        {
            _ingredient = ingredient;
            _iconImage.sprite = _ingredient.IngredientSprite;
            _typeIconImage.sprite = _ingredient.Type.Icon;
            _dragIconImage.sprite = _ingredient.IngredientSprite;

            _inventoryUI = craftMenuInventoryUI;
        }

        public bool TryAddIngredient(IngredientNodeUI nodeUI, Vector2 eventDataPosition)
        {
            if (!nodeUI) return false;

            var success = nodeUI.TryAddIngredientFrom(_ingredient, eventDataPosition);
            if (success)
            {
                SetRequiredNumber(_requiredNumber + 1);
            }
            
            return success;
        }

        public void ReturnIngredientFrom(Vector3 returnPosition)
        {
            SetRequiredNumber(_requiredNumber - 1);
            
            _dragIconImage.gameObject.SetActive(true);
            _dragIconImage.transform.DOKill();
            _dragIconImage.transform.DOMove(transform.position, 0.2f)
                .From(returnPosition)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _dragIconImage.gameObject.SetActive(false);
                });
        }

        public bool TryQuickAdd()
        {
            var selectedNode = _inventoryUI.GetSelectedNode();
            if (!selectedNode) return false;
            
            var success = selectedNode.TryAddIngredientFrom(_ingredient, transform.position);
            if (success)
            {
                SetRequiredNumber(_requiredNumber + 1);
            }
            else
            {
                // TODO: add failed animation
            }

            return true;
        }

        public void OnUIHoverEnter()
        {
            _displayCraftMenuInventoryItemInfoEvent.RaiseEvent(new DisplayIngredientInfo
            {
                Ingredient = _ingredient,
                InStock = _inStockNumber,
                Required = _requiredNumber
            });
        }

        public void OnUIHoverExit()
        {
            _displayCraftMenuInventoryItemInfoEvent.RaiseEvent(new DisplayIngredientInfo
            {
                Ingredient = null
            });
        }
    }
}