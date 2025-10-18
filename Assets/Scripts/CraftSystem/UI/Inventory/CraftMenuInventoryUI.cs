using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
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

        private Dictionary<IngredientDefinition, CraftMenuInventoryItemUI> _items = new();

        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleRecipeSelected);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleRecipeSelected);
        }

        private void HandleRecipeSelected(FinalRecipeData recipe)
        {
            // TODO: clear filters
            // update ingredient number
            var addedIngredients = recipe.GetAddedIngredients();
            foreach (var ingredient in addedIngredients.Keys.Union(_items.Keys))
            {
                // add item UI if missing
                if (!_items.TryGetValue(ingredient, out var item))
                {
                    item = Instantiate(_craftMenuInventoryItemUIPrefab, _container);
                    item.BindIngredient(ingredient);
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
                    item.BindIngredient(ingredient);
                    _items.Add(ingredient, item);
                }
                // set in stock number
                item.SetInStockNumber(storedIngredients.GetValueOrDefault(ingredient, 0));
            }
        }
    }
}