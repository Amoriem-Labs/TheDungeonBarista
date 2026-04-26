using System;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;
using TDB.Utils;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class IngredientStackItemUIBase : InventoryStackUI<IngredientDefinition>, IUIHoverHandler
    {
        [SerializeField] private Image _ingredientIcon;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private TextMeshProUGUI _amountText;
        
        private IngredientDefinition _definition;
        private IInventoryInfoDisplayer<IngredientDefinition> _infoDisplayer;
        private RectTransform _rectTransform;
        private bool _isDisplayingInfo;

        private TextMeshProUGUI AmountText => _amountText;

        protected virtual void Awake()
        {
            _infoDisplayer = GetComponentInParent<IInventoryInfoDisplayer<IngredientDefinition>>();
            _rectTransform = transform as RectTransform;
        }

        private void OnDisable()
        {
            if (_isDisplayingInfo)
            {
                OnUIHoverExit();
            }
        }

        public override void SetStack(InventoryStackData<IngredientDefinition> stack)
        {
            base.SetStack(stack);
            
            _definition = stack.Definition;
            _ingredientIcon.sprite = _definition.IngredientSprite;
            _typeIcon.sprite = _definition.Type.Icon;

            AmountText.text = $"x{stack.Amount}";
        }

        public void OnUIHoverEnter()
        {
            if (_isDisplayingInfo) return;
            
            _isDisplayingInfo = true;
            _infoDisplayer?.DisplayIngredientInfo(new InventoryInfoDisplayData<IngredientDefinition>
            {
                Data = _definition,
                RootSize = _rectTransform.sizeDelta,
                RootPosition = transform.position,
            });
        }

        public void OnUIHoverExit()
        {
            if (!_isDisplayingInfo) return;
            
            _isDisplayingInfo = false;
            _infoDisplayer?.StopDisplaying();
        }

        public void UpdateDisplayedAmount(int amount)
        {
            AmountText.text = $"x{amount}";
        }
    }

    public interface IInventoryInfoDisplayer<T>
    {
        public void DisplayIngredientInfo(InventoryInfoDisplayData<T> info);
        public void StopDisplaying();
    }

    public struct InventoryInfoDisplayData<T>
    {
        public T Data;
        public Vector2 RootSize;
        public Vector3 RootPosition;
    }
}
