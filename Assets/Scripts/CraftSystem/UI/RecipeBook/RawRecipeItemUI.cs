using TDB.CraftSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.CraftSystem.UI.RecipeBook
{
    public class RawRecipeItemUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _recipeNameText;
        
        private RawRecipeDefinition _rawRecipe;
        private RawRecipeMenuUI _rawRecipeMenu;

        public void BindRawRecipe(RawRecipeDefinition rawRecipe, RawRecipeMenuUI rawRecipeMenu)
        {
            _rawRecipe = rawRecipe;
            _rawRecipeMenu = rawRecipeMenu;

            _recipeNameText.text = _rawRecipe.RecipeName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _rawRecipeMenu.DisplayFinalRecipeMenu(_rawRecipe);
        }
    }
}