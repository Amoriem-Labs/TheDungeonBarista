using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.CraftSystem.UI.RecipeBook
{
    public class FinalRecipeMenuUI : MonoBehaviour, IIngredientStorageReceiver, IPointerClickHandler
    {
        [SerializeField] private FinalRecipeItemUI _finalRecipeItemUIPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private AddFinalRecipeItemUI _addFinalRecipeItem;

        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        
        private List<FinalRecipeItemUI> _trackedItems = new();
        private RecipeBookData _recipeBook;
        private RawRecipeDefinition _rawRecipe;
        private IngredientStorageData _ingredientStorage;
        private FinalRecipeData _selectedRecipe;
        private List<FinalRecipeData> _displayedFinalRecipes;

        private void Awake()
        {
            _addFinalRecipeItem.BindFinalRecipeMenu(this);
        }

        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleFinalRecipeSelected);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleFinalRecipeSelected);
        }

        public void DisplayFinalRecipes(RecipeBookData recipeBook, RawRecipeDefinition rawRecipe)
        {
            _recipeBook = recipeBook;
            _rawRecipe = rawRecipe;

            _displayedFinalRecipes = _recipeBook.GetFinalRecipesDerivedFrom(_rawRecipe);
            int i = 0;
            // ensure all raw recipes have any UI element
            for (; i < _displayedFinalRecipes.Count; i++)
            {
                FinalRecipeItemUI item;
                item = GetNewItem(i);

                item.BindFinalRecipe(_displayedFinalRecipes[i], this, _ingredientStorage);
            }
            // hide extra UI elements
            for (; i < _trackedItems.Count; i++)
            {
                var item = _trackedItems[i];
                item.gameObject.SetActive(false);
            }
            // move the add button to the last
            _addFinalRecipeItem.transform.SetAsLastSibling();
        }

        private FinalRecipeItemUI GetNewItem(int i)
        {
            FinalRecipeItemUI item;
            if (i >= _trackedItems.Count)
            {
                item = Instantiate(_finalRecipeItemUIPrefab, _container);
                _trackedItems.Add(item);
            }
            else
            {
                item = _trackedItems[i];
                item.gameObject.SetActive(true);
            }

            return item;
        }

        public void AddNewRecipe()
        {
            // create new recipe and add to book
            var newRecipe = new FinalRecipeData(_rawRecipe);
            _recipeBook.AddRecipe(newRecipe);
            // add a new item UI
            var item = GetNewItem(_displayedFinalRecipes.Count);
            item.BindFinalRecipe(newRecipe, this, _ingredientStorage);
            // track new displayed recipe
            _displayedFinalRecipes.Add(newRecipe);
            
            // move the add button to the last
            _addFinalRecipeItem.transform.SetAsLastSibling();
            
            // select new recipe
            _onRecipeSelectedEvent.RaiseEvent<FinalRecipeData>(newRecipe);
        }

        public void DeleteRecipe(FinalRecipeData finalRecipe)
        {
            // deselect if the deleted one is the selected one
            if (_selectedRecipe == finalRecipe)
            {
                _onRecipeSelectedEvent.RaiseEvent<FinalRecipeData>(null);
            }

            var recipeIdx = _displayedFinalRecipes.FindIndex(r => r == finalRecipe);
            if (recipeIdx < 0 || recipeIdx >= _trackedItems.Count)
            {
                Debug.LogError($"Recipe {finalRecipe.RecipeName} not displayed");
            }
            else
            {
                // hide item UI
                var item = _trackedItems[recipeIdx];
                item.gameObject.SetActive(false);
                // remove recipe from tracked list
                _displayedFinalRecipes.Remove(finalRecipe);
                // delete recipe from book
                _recipeBook.DeleteRecipe(finalRecipe);
            }
        }

        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage)
        {
            _ingredientStorage = ingredientStorage;
        }
        
        private void HandleFinalRecipeSelected(FinalRecipeData recipe)
        {
            _selectedRecipe = recipe;
        }

        /// <summary>
        /// Do nothing. Only for blocking click events.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData) { }
    }
}