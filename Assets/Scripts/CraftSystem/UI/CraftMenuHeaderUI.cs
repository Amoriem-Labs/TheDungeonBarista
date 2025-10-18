using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI
{
    public class CraftMenuHeaderUI : MonoBehaviour, IIngredientStorageReceiver
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _titleEditButton;
        [SerializeField] private TextMeshProUGUI _servingCountNumber;
        [SerializeField] private List<GameObject> _displayWhenReady;
        [SerializeField] private List<GameObject> _displayWhenNotReady;

        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;

        private FinalRecipeData _recipe;
        private IngredientStorageData _ingredientStorage;
        private string _servingCountNumberTemplate;

        public string NoRecipeSelectedTitle => "No Recipe Selected";

        private void Awake()
        {
            _servingCountNumberTemplate = _servingCountNumber.text;
        }

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
            _recipe = recipe;
            if (_recipe == null)
            {
                _titleText.text = NoRecipeSelectedTitle;
                _titleEditButton.interactable = false;
                
                _displayWhenReady.ForEach(g => g.SetActive(false));
                _displayWhenNotReady.ForEach(g => g.SetActive(false));
            }
            else
            {
                _titleText.text = _recipe.RecipeName;
                _titleEditButton.interactable = true;

                var recipeReady = _recipe.IsRecipeReady;
                _displayWhenReady.ForEach(g => g.SetActive(recipeReady));
                _displayWhenNotReady.ForEach(g => g.SetActive(!recipeReady));

                _servingCountNumber.text = string.Format(_servingCountNumberTemplate,
                    _recipe.GetServingsAvailable(_ingredientStorage));
            }
        }
        
        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage)
        {
            _ingredientStorage = ingredientStorage;
        }
    }
}