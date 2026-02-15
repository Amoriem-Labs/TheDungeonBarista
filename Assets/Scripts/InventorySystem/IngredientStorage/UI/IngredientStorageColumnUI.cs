using System;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;
using UnityEngine;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class IngredientStorageColumnUI : MonoBehaviour, IIngredientStackTransferHandler
    {
        private IngredientStackItemUIContainer _ingredientContainer;
        
        private Action<IngredientStackItemUIClickable, int> _handler;
        private IngredientStorageData _storage;

        private void Awake()
        {
            _ingredientContainer = GetComponentInChildren<IngredientStackItemUIContainer>();
        }

        public void BindHandler(Action<IngredientStackItemUIClickable, int> handler) => _handler = handler;
        
        public void TransferIngredient(IngredientStackItemUIClickable ingredientItem, int amount)
        {
            _handler?.Invoke(ingredientItem, amount);
        }

        public void RemoveIngredient(IngredientStackItemUIClickable ingredientItem, int actualAmount)
        {
            // update storage data
            ingredientItem.Stack.Consume(actualAmount);
            // update UI
            if (CheckShouldHide(ingredientItem.Stack))
            {
                // hide empty stack item
                _ingredientContainer.HideItem(ingredientItem);
            }
            else
            {
                // update item amount text
                var remain = ingredientItem.Stack.Amount;
                ingredientItem.UpdateDisplayedAmount(remain);
            }
        }

        public void AddIngredient(IngredientDefinition ingredient, int actualAmount)
        {
            // update storage data
            _storage.Deposit(ingredient, actualAmount);
            // update UI
            var item = _ingredientContainer.FindItem(i => i == ingredient) as IngredientStackItemUIBase;
            if (item)
            {
                // update item amount text
                item.UpdateDisplayedAmount(item.Stack.Amount);
            }
            else
            {
                // add new stack
                var stack = _storage.GetStack(ingredient);
                _ingredientContainer.AddItem(stack);
            }
        }

        public void BindAndDisplay(IngredientStorageData storage)
        {
            _storage = storage;
            
            _ingredientContainer.Clear();
            _ingredientContainer.SetInventory(storage, hidePolicy: CheckShouldHide);
        }

        private bool CheckShouldHide(InventoryStackData<IngredientDefinition> stack) => stack.Amount <= 0;
    }
}