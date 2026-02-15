using System.Collections.Generic;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.IngredientStorage;
using TMPro;
using UnityEngine;

namespace TDB.CraftSystem.UI
{
    public class ServingCountUI : MonoBehaviour, IIngredientStorageReceiver
    {
        [SerializeField] private TextMeshProUGUI _servingCountNumber;
        [SerializeField] private List<GameObject> _displayWhenReady;
        [SerializeField] private List<GameObject> _displayWhenNotReady;
        
        private IngredientStorageData _ingredientStorage;
        private string _servingCountNumberTemplate;
        
        private void Awake()
        {
            _servingCountNumberTemplate = _servingCountNumber.text;
        }
        
        public void UpdateServingCount(FinalRecipeData recipe, IngredientStorageData ingredientStorage = null)
        {
            ingredientStorage = ingredientStorage ?? _ingredientStorage;
            if (ingredientStorage == null)
            {
                Debug.LogError($"{gameObject.name}: UpdateServingCount() called without ingredientStorage");
                return;
            }
            
            var recipeReady = recipe.IsRecipeReady;
            _displayWhenReady.ForEach(g => g.SetActive(recipeReady));
            _displayWhenNotReady.ForEach(g => g.SetActive(!recipeReady));

            if (recipeReady)
            {
                _servingCountNumber.text = string.Format(_servingCountNumberTemplate,
                    recipe.GetServingsAvailable(ingredientStorage));
            }
        }

        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage)
        {
            _ingredientStorage = ingredientStorage;
        }
    }
}