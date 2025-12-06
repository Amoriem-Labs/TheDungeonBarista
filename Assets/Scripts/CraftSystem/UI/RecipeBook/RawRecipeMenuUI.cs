using System;
using System.Collections.Generic;
using TDB.CraftSystem.Data;
using TDB.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.CraftSystem.UI.RecipeBook
{
    public class RawRecipeMenuUI : MonoBehaviour, IOnUIEnableHandler, IPointerClickHandler
    {
        [SerializeField] private RawRecipeItemUI _rawRecipeItemUIPrefab;
        [SerializeField] private Transform _container;
        
        private RecipeBookData _recipeBook;
        
        private FinalRecipeMenuUI _finalRecipeMenu;
        private UIEnabler _finalRecipeMenuEnabler;
        
        private readonly List<RawRecipeItemUI> _trackedItems = new();

        private void Awake()
        {
            _finalRecipeMenu = GetComponentInChildren<FinalRecipeMenuUI>();
            _finalRecipeMenuEnabler = _finalRecipeMenu.GetComponent<UIEnabler>();
        }

        public void BindRecipeBook(RecipeBookData recipeBook)
        {
            _recipeBook = recipeBook;

            var allRawRecipes = recipeBook.AllObtainedRawRecipes;
            int i = 0;
            // ensure all raw recipes have any UI element
            for (; i < allRawRecipes.Count; i++)
            {
                RawRecipeItemUI item;
                if (i >= _trackedItems.Count)
                {
                    item = Instantiate(_rawRecipeItemUIPrefab, _container);
                    _trackedItems.Add(item);
                }
                else
                {
                    item = _trackedItems[i];
                    item.gameObject.SetActive(true);
                }

                item.BindRawRecipe(allRawRecipes[i], this);
            }
            // hide extra UI elements
            for (; i < _trackedItems.Count; i++)
            {
                var item = _trackedItems[i];
                item.gameObject.SetActive(false);
            }
        }

        public void OnUIEnable()
        {
            // close secondary menu everytime the menu opens
            // require manual open sub-menu to trigger DisplayFinalRecipeMenu,
            // which ensures all final recipe data are up-to-date
            _finalRecipeMenuEnabler.Disable();
        }

        public void DisplayFinalRecipeMenu(RawRecipeDefinition rawRecipe)
        {
            if (!rawRecipe)
            {
                _finalRecipeMenuEnabler.Disable();
                return;
            }
            
            _finalRecipeMenuEnabler.Enable(.2f);
            _finalRecipeMenu.DisplayFinalRecipes(_recipeBook, rawRecipe);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DisplayFinalRecipeMenu(null);
        }
    }
}