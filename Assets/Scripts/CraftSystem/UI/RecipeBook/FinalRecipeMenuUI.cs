using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private readonly List<FinalRecipeItemUI> _trackedItems = new();
        private RecipeBookData _recipeBook;
        private RawRecipeDefinition _rawRecipe;
        private IngredientStorageData _ingredientStorage;
        private FinalRecipeData _selectedRecipe;
        private List<(FinalRecipeData recipe, FinalRecipeItemUI itemUI)> _displayedFinalRecipes;
        private CraftMenuUI _craftMenuUI;

        private void Awake()
        {
            _addFinalRecipeItem.BindFinalRecipeMenu(this);
            _craftMenuUI = GetComponentInParent<CraftMenuUI>();
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

            _displayedFinalRecipes = _recipeBook.GetFinalRecipesDerivedFrom(_rawRecipe)
                .Select<FinalRecipeData, (FinalRecipeData recipe, FinalRecipeItemUI itemUI)>(r => (r, null)).ToList();
            int i = 0;
            // ensure all raw recipes have any UI element
            for (; i < _displayedFinalRecipes.Count; i++)
            {
                var recipe = _displayedFinalRecipes[i].recipe;
                var item = GetNewItem(i);
                _displayedFinalRecipes[i] = (recipe, item);

                item.BindFinalRecipe(recipe, this, _ingredientStorage);
                item.HandleRecipeSelected(_selectedRecipe == recipe);
            }
            // hide extra UI elements
            for (; i < _trackedItems.Count; i++)
            {
                var item = _trackedItems[i];
                item.gameObject.SetActive(false);
            }
            // move the add button to the last
            _addFinalRecipeItem.transform.SetAsLastSibling();

            CheckConsistency();
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
            _displayedFinalRecipes.Add((newRecipe, item));
            
            // move the add button to the last
            _addFinalRecipeItem.transform.SetAsLastSibling();
            
            // select new recipe
            _onRecipeSelectedEvent.RaiseEvent<FinalRecipeData>(newRecipe);

            CheckConsistency();
        }

        public void DeleteRecipe(FinalRecipeData finalRecipe)
        {
            // deselect if the deleted one is the selected one
            if (_selectedRecipe == finalRecipe)
            {
                _onRecipeSelectedEvent.RaiseEvent<FinalRecipeData>(null);
            }

            var recipeIdx = _displayedFinalRecipes.FindIndex(r => r.recipe == finalRecipe);
            if (recipeIdx < 0 || recipeIdx >= _trackedItems.Count)
            {
                Debug.LogError($"Recipe {finalRecipe.RecipeName} not displayed");
            }
            else
            {
                // hide item UI
                var item = _trackedItems[recipeIdx];
                // move to the last
                item.transform.SetAsLastSibling();
                _trackedItems.RemoveAt(recipeIdx);
                _trackedItems.Add(item);
                item.gameObject.SetActive(false);
                // remove recipe from tracked list
                var idx = _displayedFinalRecipes.FindIndex(t => t.recipe == finalRecipe);
                _displayedFinalRecipes.RemoveAt(idx);
                // delete recipe from book
                _recipeBook.DeleteRecipe(finalRecipe);

                CheckConsistency();
            }
        }

        private void CheckConsistency()
        {
#if UNITY_EDITOR
            if (_displayedFinalRecipes == null) return;
            for (var i = 0; i < _displayedFinalRecipes.Count; i++)
            {
                var item = _displayedFinalRecipes[i].itemUI;
                var idx = _trackedItems.FindIndex(i => i == item);
                Debug.Assert(i == idx, $"Index for displayed recipe and item UI mismatch: {i} - {idx}");
            }
#endif
        }

        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage)
        {
            _ingredientStorage = ingredientStorage;
        }
        
        private void HandleFinalRecipeSelected(FinalRecipeData recipe)
        {
            _selectedRecipe = recipe;

            if (_displayedFinalRecipes == null) return;
            for (int i = 0; i < _displayedFinalRecipes.Count; i++)
            {
                var boundRecipe = _displayedFinalRecipes[i].recipe;
                var item = _trackedItems[i];
                item.HandleRecipeSelected(_selectedRecipe == boundRecipe);
            }
        }

        public void HandleRecipeEdit()
        {
            _craftMenuUI.ToggleRecipeBook();
        }

        public void HandleRecipeSubmit()
        {
            _craftMenuUI.ConfirmFinalRecipe();
        }

        /// <summary>
        /// Do nothing. Only for blocking click events.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData) { }
    }
}