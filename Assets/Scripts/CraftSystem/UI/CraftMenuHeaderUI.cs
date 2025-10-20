using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
        [SerializeField] private TMP_InputField _titleInputField;
        
        [SerializeField] private Button _recipeBookButton;
        [SerializeField] private Button _confirmRecipeButton;

        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        [SerializeField] private EventChannel _onRecipeUpdatedEvent;

        private FinalRecipeData _recipe;
        private CraftMenuUI _craftMenuUI;
        private bool _isEditingTitle;

        public string NoRecipeSelectedTitle => "No Recipe Selected";

        private void Awake()
        {
            _craftMenuUI = GetComponentInParent<CraftMenuUI>();
            
            _recipeBookButton.onClick.AddListener(HandleRecipeBookButtonClicked);
            _titleEditButton.onClick.AddListener(HandleTitleEditButtonClicked);
            _confirmRecipeButton.onClick.AddListener(HandleConfirmRecipeButtonClicked);
            
            _titleInputField.onEndEdit.AddListener(HandleTitleInputFieldEndEdit);
            
            _titleInputField.gameObject.SetActive(false);
        }

        #region InputHandlers

        private void HandleConfirmRecipeButtonClicked()
        {
            AbortEditingTitle();
            _craftMenuUI.ConfirmFinalRecipe(_recipe);
        }

        private void HandleTitleEditButtonClicked()
        {
            _isEditingTitle = true;
            
            _titleInputField.gameObject.SetActive(true);
            _titleInputField.text = _titleText.text;
            
            _titleText.gameObject.SetActive(false);
            
            _titleInputField.Select();
        }

        private void HandleRecipeBookButtonClicked()
        {
            AbortEditingTitle();
            _craftMenuUI.ToggleRecipeBook();
        }

        private void HandleTitleInputFieldEndEdit(string text)
        {
            if (!_isEditingTitle) return;
            _isEditingTitle = false;
            
            if (!text.IsNullOrWhitespace())
            {
                _titleText.text = text;
                _recipe.SetName(text);
            }
            
            _titleInputField.gameObject.SetActive(false);
            _titleText.gameObject.SetActive(true);
        }

        private void AbortEditingTitle()
        {
            if (!_isEditingTitle) return;
            _isEditingTitle = false;
            
            _titleInputField.gameObject.SetActive(false);
            _titleText.gameObject.SetActive(true);
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
            AbortEditingTitle();
            
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