using System;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.GameManagers.SessionManagers;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class IngredientStorageUI : MonoBehaviour, IIngredientInfoDisplayer
    {
        [SerializeField] private IngredientStorageColumnUI _refrigeratedColumnUI;
        [SerializeField] private IngredientStorageColumnUI _volatileColumnUI;
        [SerializeField] private TextMeshProUGUI _refrigeratorCapacityText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private IngredientInfoUI _ingredientInfoUI;

        private UIEnabler _enabler;
        private IngredientStorageManager _storageManager;
        private IngredientStorageData _volatileStorage;
        private IngredientStorageData _refrigeratedStorage;
        private RectTransform _rectTransform;
        private float _fullWidth;
        private Action _onExitMenu;

        private int RefrigeratorCapacity => _storageManager.RefrigeratorCapacity;
        
        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _enabler = GetComponent<UIEnabler>();

            _storageManager = FindObjectOfType<IngredientStorageManager>();
            
            _refrigeratedColumnUI.BindHandler(TransferFromRefrigerator);
            _volatileColumnUI.BindHandler(TransferToRefrigerator);
            
            _closeButton.onClick.AddListener(Hide);

            _fullWidth = _rectTransform!.sizeDelta.x;
        }

        [Button(ButtonSizes.Large)]
        public void Display(Action onExitMenu)
        {
            _onExitMenu = onExitMenu;
            _enabler.Enable();
            
            _volatileStorage = _storageManager.VolatileIngredientStorage;
            _volatileColumnUI.BindAndDisplay(_volatileStorage);

            _refrigeratedStorage = _storageManager.RefrigeratedIngredientStorage;
            var displayRefrigerator = RefrigeratorCapacity > 0;
            _refrigeratedColumnUI.gameObject.SetActive(displayRefrigerator);
            if (displayRefrigerator)
            {
                _refrigeratedColumnUI.BindAndDisplay(_refrigeratedStorage);
                UpdateCapacityText();
            }

            _rectTransform.sizeDelta =
                new Vector2(_fullWidth * (displayRefrigerator ? 1f : .5f), _rectTransform.sizeDelta.y);
        }

        private void Hide()
        {
            if (_onExitMenu != null)
            {
                _onExitMenu?.Invoke();
                _onExitMenu = null;
            }
            _enabler.Disable();
        }

        private void TransferToRefrigerator(IngredientStackItemUIClickable item, int amount) =>
            Transfer(
                fromStorage: _volatileStorage, fromColumn: _volatileColumnUI,
                toStorage: _refrigeratedStorage, toColumn: _refrigeratedColumnUI,
                ingredientItem: item, amount: amount, toStorageCapacity: RefrigeratorCapacity
            );

        private void TransferFromRefrigerator(IngredientStackItemUIClickable item, int amount) =>
            Transfer(
                fromStorage: _refrigeratedStorage, fromColumn: _refrigeratedColumnUI,
                toStorage: _volatileStorage, toColumn: _volatileColumnUI,
                ingredientItem: item, amount: amount, toStorageCapacity: -1 // infinite capacity for volatile storage
            );

        /// <summary>
        /// 1. Check capacity and decide the actual amount
        /// 2. Remove from source storage
        /// 3. Update source stack item
        /// 4. Add to target storage
        /// 5. Update target stack item
        /// 6. Update capacity text
        /// </summary>
        private void Transfer(IngredientStorageData fromStorage, IngredientStorageColumnUI fromColumn,
            IngredientStorageData toStorage, IngredientStorageColumnUI toColumn,
            IngredientStackItemUIClickable ingredientItem, int amount, int toStorageCapacity)
        {
            var ingredient = ingredientItem.Stack.Definition;
            // check capacity and decide the actual amount
            var current = toStorage.TotalIngredients;
            var actualAmount = toStorageCapacity < 0 ? amount : Mathf.Min(toStorageCapacity - current, amount);

            if (actualAmount <= 0)
            {
                // TODO: notify player
                Debug.Log("Not enough space in refrigerator");
                return;
            }
            
            // remove from source storage & update source stack item
            fromColumn.RemoveIngredient(ingredientItem, actualAmount);
            
            // TODO: potential animation
            
            // add to target storage & update target stack item
            toColumn.AddIngredient(ingredient, actualAmount);
            
            // update capacity text
            UpdateCapacityText();
        }

        private void UpdateCapacityText()
        {
            var current = _refrigeratedStorage.TotalIngredients;
            _refrigeratorCapacityText.text = $"{current}/{RefrigeratorCapacity}";
        }

        public void DisplayIngredientInfo(IngredientInfoDisplayInfo info) => _ingredientInfoUI.DisplayIngredientInfo(info);

        public void StopDisplaying() => _ingredientInfoUI.StopDisplaying();
    }
}