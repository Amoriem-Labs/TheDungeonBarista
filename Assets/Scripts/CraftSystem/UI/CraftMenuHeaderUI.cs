using System;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.Utils.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI
{
    public class CraftMenuHeaderUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _titleEditButton;
        [SerializeField] private ServingCountUI _servingCountUI;

        [SerializeField] private Button _recipeBookButton;
        [SerializeField] private Button _confirmRecipeButton;

        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        [SerializeField] private EventChannel _onRecipeUpdatedEvent;

        private FinalRecipeData _recipe;
        private CraftMenuUI _craftMenuUI;

        public string NoRecipeSelectedTitle => "No Recipe Selected";

        private void Awake()
        {
            _craftMenuUI = GetComponentInParent<CraftMenuUI>();
            
            _recipeBookButton.onClick.AddListener(HandleRecipeBookButtonClicked);
            _titleEditButton.onClick.AddListener(HandleTitleEditButtonClicked);
            _confirmRecipeButton.onClick.AddListener(HandleConfirmRecipeButtonClicked);
        }

        #region ButtonHandlers

        private void HandleConfirmRecipeButtonClicked()
        {
            _craftMenuUI.ConfirmFinalRecipe();
        }

        private void HandleTitleEditButtonClicked()
        {
            throw new NotImplementedException();
        }

        private void HandleRecipeBookButtonClicked()
        {
            _craftMenuUI.ToggleRecipeBook();
        }

        #endregion

        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleRecipeSelected);
            _onRecipeUpdatedEvent.AddListener(HandleRecipeUpdated);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleRecipeSelected);
            _onRecipeUpdatedEvent.RemoveListener(HandleRecipeUpdated);
        }

        private void HandleRecipeUpdated()
        {
            _servingCountUI.UpdateServingCount(_recipe);
        }

        private void HandleRecipeSelected(FinalRecipeData recipe)
        {
            _recipe = recipe;
            if (_recipe == null)
            {
                _titleText.text = NoRecipeSelectedTitle;
                _titleEditButton.gameObject.SetActive(false);
                
                _servingCountUI.gameObject.SetActive(false);
            }
            else
            {
                _titleText.text = _recipe.RecipeName;
                _titleEditButton.gameObject.SetActive(true);

                _servingCountUI.gameObject.SetActive(true);
                _servingCountUI.UpdateServingCount(_recipe);
            }
        }
    }
}