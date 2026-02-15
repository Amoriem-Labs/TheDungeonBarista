using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.CraftSystem.UI.RecipeBook
{
    public class AddFinalRecipeItemUI : MonoBehaviour, IPointerClickHandler
    {
        private FinalRecipeMenuUI _parentMenu;

        public void BindFinalRecipeMenu(FinalRecipeMenuUI finalRecipeMenuUI)
        {
            _parentMenu = finalRecipeMenuUI;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _parentMenu.AddNewRecipe();
        }
    }
}