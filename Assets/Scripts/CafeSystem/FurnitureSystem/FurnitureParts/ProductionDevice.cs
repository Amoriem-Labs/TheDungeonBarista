using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI;
using TDB.Player.Interaction;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.FurnitureParts
{
    /// <summary>
    /// Handles how player interact with the production device.
    /// </summary>
    [RequireComponent(typeof(Furniture))]
    public class ProductionDevice : MonoBehaviour, IFurniturePartDataHolder, IInteractable
    {
        // [SerializeReference]
        private ProductionDeviceData _deviceData;

        [Title("Events")]
        [SerializeField] private EventChannel _configureRecipeEvent;

        private RecipeBookDataHolder _recipeBookHolder;
        private IngredientStorageManager _ingredientStorage;

        private void Awake()
        {
            _recipeBookHolder = FindObjectOfType<RecipeBookDataHolder>();
            _ingredientStorage = FindObjectOfType<IngredientStorageManager>();
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void ConfigureRecipe(Action finishCallback = null)
        {
            _configureRecipeEvent.RaiseEvent<OpenMenuInfo>(new OpenMenuInfo()
            {
                CurrentRecipe = _deviceData?.ConfiguredRecipe,
                IngredientStorage = _ingredientStorage.GetAllIngredientStorage(),
                RecipeBook = _recipeBookHolder.GetRecipeBook(),
                RecipeDecidedCallback = r =>
                {
                    HandleRecipeDecided(r);
                    finishCallback?.Invoke();
                }
            });
        }

        /// <summary>
        /// Check configured recipe and ingredient storage.
        /// Consume ingredients and retrieve recipe.
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns>Whether the production succeeds</returns>
        public bool TryCookProduct(out FinalRecipeData recipe)
        {
            recipe = _deviceData.ConfiguredRecipe;
            // not configured
            if (recipe == null) return false;
            // not enough ingredients
            var servings = recipe.GetServingsAvailable(_ingredientStorage.GetAllIngredientStorage());
            if (servings <= 0) return false;
            // consume ingredients
            var requirement = recipe.GetAddedIngredients();
            return _ingredientStorage.TryConsume(requirement);
        }

        private void HandleRecipeDecided(FinalRecipeData recipe)
        {
            _deviceData.ConfiguredRecipe = recipe;
            // TODO: update UI to indicate serving counts
        }

        #region Data Management

        public string PartID => "ProductionDevice";
        public Type DataType => typeof(ProductionDeviceData);
        public object CreateDefaultData() => new ProductionDeviceData();

        public void LoadData(object data) => _deviceData = new ProductionDeviceData(data);

        public object ExtractData() => _deviceData;

        #endregion

        #region Interaction
        
        // always interactable
        public bool IsInteractable => true;
        public Action OnInteractableUpdated { get; set; }

        public void SetReady()
        {
            // TODO:
        }

        public void SetNotReady()
        {
            // TODO:
        }

        #endregion
    }

    [System.Serializable]
    public class ProductionDeviceData
    {
        public FinalRecipeData ConfiguredRecipe = null;

        public ProductionDeviceData(object data)
        {
            if (data is ProductionDeviceData deviceData)
            {
                // use provided data only if the ConfiguredRecipe is a valid one (with RawRecipe)
                ConfiguredRecipe = deviceData.ConfiguredRecipe?.RawRecipe ? deviceData.ConfiguredRecipe : null;
            }
            else
            {
                ConfiguredRecipe = null;
            }
        }

        public ProductionDeviceData()
        {
            ConfiguredRecipe = null;
        }
    }
}
