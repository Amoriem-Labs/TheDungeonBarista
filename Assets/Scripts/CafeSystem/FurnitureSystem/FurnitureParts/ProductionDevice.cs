using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.FurnitureParts
{
    /// <summary>
    /// Handles how player interact with the production device.
    /// </summary>
    [RequireComponent(typeof(Furniture))]
    public class ProductionDevice : MonoBehaviour, IFurniturePartDataHolder
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
        private void ConfigureRecipe()
        {
            _configureRecipeEvent.RaiseEvent<OpenMenuInfo>(new OpenMenuInfo()
            {
                CurrentRecipe = _deviceData?.ConfiguredRecipe,
                IngredientStorage = _ingredientStorage.GetAllIngredientStorage(),
                RecipeBook = _recipeBookHolder.GetRecipeBook(),
                RecipeDecidedCallback = HandleRecipeDecided
            });
        }

        private void HandleRecipeDecided(FinalRecipeData recipe)
        {
            _deviceData.ConfiguredRecipe = recipe;
        }

        #region Data Management

        public string PartID => "ProductionDevice";
        public Type DataType => typeof(ProductionDeviceData);
        public object CreateDefaultData() => new ProductionDeviceData();

        public void LoadData(object data) => _deviceData = new ProductionDeviceData(data);

        public object ExtractData() => _deviceData;

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
