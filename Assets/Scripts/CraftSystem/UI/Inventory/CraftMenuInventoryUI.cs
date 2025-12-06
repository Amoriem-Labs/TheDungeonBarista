using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI.RecipeGraph;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CraftSystem.UI.Inventory
{
    public class CraftMenuInventoryUI : MonoBehaviour, IIngredientStorageReceiver
    {
        [SerializeField] private Transform _container;
        [SerializeField] private CraftMenuInventoryItemUI _craftMenuInventoryItemUIPrefab;
        
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        [SerializeField] private EventChannel _returnIngredientEvent;
        [SerializeField] private EventChannel _onNodeSelectionEvent;

        private Dictionary<IngredientDefinition, CraftMenuInventoryItemUI> _items = new();
        private IngredientNodeUI _currentSelectedNode;

        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleRecipeSelected);
            // _onRecipeUpdatedEvent.AddListener(HandleRecipeUpdated);
            _returnIngredientEvent.AddListener<ReturnIngredientInfo>(HandleReturnIngredient);
            _onNodeSelectionEvent.AddListener<IngredientNodeUI>(HandleNodeSelection);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleRecipeSelected);
            // _onRecipeSelectedEvent.RemoveListener(HandleRecipeUpdated);
            _returnIngredientEvent.RemoveListener<ReturnIngredientInfo>(HandleReturnIngredient);
            _onNodeSelectionEvent.RemoveListener<IngredientNodeUI>(HandleNodeSelection);
        }

        private void HandleNodeSelection(IngredientNodeUI node)
        {
            if (_currentSelectedNode == node) return;
            _currentSelectedNode = node;
            // TODO: update filtering
        }

        private void HandleReturnIngredient(ReturnIngredientInfo info)
        {
            if (!_items.TryGetValue(info.Ingredient, out var item))
            {
                Debug.LogError(
                    $"Ingredient {info.Ingredient.IngredientName} was returned from recipe but was not in the inventory.");
                return;
            }

            item.ReturnIngredientFrom(info.Position);
        }

        private void HandleRecipeSelected(FinalRecipeData recipe)
        {
            // TODO: clear filters
            // update ingredient number
            var addedIngredients =
                recipe?.GetAddedIngredients() ?? new Dictionary<IngredientDefinition, int>();
            foreach (var ingredient in addedIngredients.Keys.Union(_items.Keys))
            {
                // add item UI if missing
                if (!_items.TryGetValue(ingredient, out var item))
                {
                    item = Instantiate(_craftMenuInventoryItemUIPrefab, _container);
                    item.BindData(ingredient, this);
                    _items.Add(ingredient, item);
                    // supposed to be 0 since it was not loaded in the first place
                    item.SetInStockNumber(0);
                }
                // set required amount
                item.SetRequiredNumber(addedIngredients.GetValueOrDefault(ingredient, 0));
            }
            // TODO: update sorting
        }

        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage)
        {
            // aggregate
            var storedIngredients = ingredientStorage.GetIngredientsCount;
            // update UI
            foreach (var ingredient in storedIngredients.Keys.Union(_items.Keys))
            {
                // add item UI if missing
                if (!_items.TryGetValue(ingredient, out var item))
                {
                    item = Instantiate(_craftMenuInventoryItemUIPrefab, _container);
                    item.BindData(ingredient, this);
                    _items.Add(ingredient, item);
                }
                // set in stock number
                item.SetInStockNumber(storedIngredients.GetValueOrDefault(ingredient, 0));
            }
        }

        public IngredientNodeUI GetSelectedNode() => _currentSelectedNode;
    }
}