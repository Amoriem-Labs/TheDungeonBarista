using System;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.RecipeBook
{
    public class FinalRecipeItemUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _recipeNameText;
        [SerializeField] private ServingCountUI _servingCountUI;
        [SerializeField] private Button _deleteButton;
        
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        
        private FinalRecipeData _finalRecipe;
        private FinalRecipeMenuUI _parentMenu;

        private void Awake()
        {
            _deleteButton.onClick.AddListener(HandleDeleteButtonClicked);
        }

        private void OnDisable()
        {
            if (_finalRecipe != null) _finalRecipe.OnNameChange -= HandleNameChange;
            _finalRecipe = null;
        }

        public void BindFinalRecipe(FinalRecipeData displayedFinalRecipe, FinalRecipeMenuUI finalRecipeMenuUI,
            IngredientStorageData ingredientStorage)
        {
            // unbind before binding
            if (_finalRecipe != null) _finalRecipe.OnNameChange -= HandleNameChange;
            
            _finalRecipe = displayedFinalRecipe;
            _parentMenu = finalRecipeMenuUI;

            _recipeNameText.text = _finalRecipe.RecipeName;
            _servingCountUI.UpdateServingCount(_finalRecipe, ingredientStorage);
            
            // bind callbacks to listen for name change
            _finalRecipe.OnNameChange += HandleNameChange;
        }

        private void HandleNameChange()
        {
            _recipeNameText.text = _finalRecipe.RecipeName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onRecipeSelectedEvent.RaiseEvent(_finalRecipe);
        }

        private void HandleDeleteButtonClicked()
        {
            // TODO: confirmation
            _parentMenu.DeleteRecipe(_finalRecipe);
        }
    }
}