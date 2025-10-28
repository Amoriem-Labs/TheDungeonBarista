using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem;
using TDB.GameManagers;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    /// <summary>
    /// Responsible for initializing the CafeScene.
    /// All managers in this folder are not singletons, and should be acquired by components depending on the managers
    /// through FindObjectOfType or the initialization of CafeSceneManager.
    /// </summary>
    public class CafeSceneManager : MonoBehaviour
    {
        /// <summary>
        /// Invoked once by the GameManager when the scene transition finishes.
        /// </summary>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public static void FindAndInitialize()
        {
            var manager = FindObjectOfType<CafeSceneManager>();
            if (!manager)
            {
                Debug.LogError("CafeSceneManager not found");
                return;
            }
            manager.Initialize();
        }
        
        private void Initialize()
        {
            #region Get Data

            // TODO: get furniture data from save data first, use default preset if non-existent
            var furnitureData = null ?? GameManager.Instance.GameConfig.DefaultFurniturePreset.FurnitureData;
            
            // test recipe book
            var recipeBookData = GameManager.Instance.GameConfig.ExtendedTestRecipeBook;
            // one-day ingredient storage
            var volatileIngredientStorage = GameManager.Instance.GameConfig.TestIngredientStorage;

            #endregion

            #region Initialize Controllers

            // initialize furniture (including storage entities and production device entities)
            TryFindObjectOfType<FurnitureManager>(out var furnitureManager);
            furnitureManager.Initialize(furnitureData);
            
            // initialize recipe book data
            TryFindObjectOfType<RecipeBookDataHolder>(out var recipeBook);
            recipeBook.Initialize(recipeBookData);
            
            // initialize ingredient storages
            TryFindObjectOfType<IngredientStorageManager>(out var ingredientStorage);
            ingredientStorage.InitializeVolatileStorage(volatileIngredientStorage);
            // TODO: initialize refrigerated storages
            // ingredientStorage.InitializeRefrigeratedStorages()
            
            #endregion
        }

        private bool TryFindObjectOfType<T>(out T obj) where T : Object
        {
            obj = FindObjectOfType<T>();
            if (!obj)
            {
                Debug.LogError($"Object of type {typeof(T).Name} could not be found.");
            }

            return obj;
        }
    }
}
