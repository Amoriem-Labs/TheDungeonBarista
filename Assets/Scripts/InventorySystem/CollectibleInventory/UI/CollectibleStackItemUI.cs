using TDB.DungeonSystem.Collectibles;
using TDB.InventorySystem.Framework;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class CollectibleStackItemUI : InventoryStackUI<CollectibleDefinition>, IUIHoverHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _amountText;
        
        private CollectibleDefinition _definition;
        private IInventoryInfoDisplayer<CollectibleDefinition> _infoDisplayer;
        private RectTransform _rectTransform;
        private bool _isDisplayingInfo;

        private TextMeshProUGUI AmountText => _amountText;

        protected virtual void Awake()
        {
            _infoDisplayer = GetComponentInParent<IInventoryInfoDisplayer<CollectibleDefinition>>();
            _rectTransform = transform as RectTransform;
        }

        private void OnDisable()
        {
            if (_isDisplayingInfo)
            {
                OnUIHoverExit();
            }
        }

        public override void SetStack(InventoryStackData<CollectibleDefinition> stack)
        {
            base.SetStack(stack);
            
            _definition = stack.Definition;
            _itemIcon.sprite = _definition.Definition.CollectibleSprite;

            AmountText.text = $"x{stack.Amount}";
        }

        public void OnUIHoverEnter()
        {
            if (_isDisplayingInfo) return;
            
            _isDisplayingInfo = true;
            _infoDisplayer?.DisplayIngredientInfo(new InventoryInfoDisplayData<CollectibleDefinition>
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
}