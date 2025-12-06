using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem;
using TDB.GameManagers;
using TDB.Utils.DataPersistence;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    /// <summary>
    /// Session scene serves as the game data holder.
    /// Most of the objects will access game data from specific child managers.
    /// </summary>
    public class SessionManager : MonoBehaviour, IDataWriterDestination
    {
        [ShowInInspector, ReadOnly]
        private GameData _currentSessionGameData;
        
        private readonly List<IGameDataWriter> _dataWriters = new();
        
        public List<FurnitureData> AllInstalledFurnitureData => _currentSessionGameData.AllInstalledFurnitureData;

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void TestInitializeWithNewGame()
        {
            Initialize(GameManager.Instance.GameConfig.NewGameData);
        }
        
        public void Initialize(GameData gameData)
        {
            #region Get Data

            _currentSessionGameData = gameData;
            
            // test recipe book
            var recipeBookData = gameData.RecipeBookData;
            // one-day ingredient storage
            var volatileIngredientStorage = gameData.VolatileIngredientStorageData;
            // refrigerated ingredient storage
            var refrigeratedIngredientStorage = gameData.RefrigeratedIngredientStorageData;

            #endregion

            #region Initialize Managers

            // initialize recipe book data
            TryFindObjectOfType<RecipeBookManager>(out var recipeBook);
            recipeBook.Initialize(recipeBookData);
            
            // initialize ingredient storages
            TryFindObjectOfType<IngredientStorageManager>(out var ingredientStorage);
            ingredientStorage.InitializeStorages(volatileIngredientStorage, refrigeratedIngredientStorage);
            
            #endregion
        }

        public static void FindAndOverwriteSave()
        {
            var sessionManager = FindObjectOfType<SessionManager>();
            if (!sessionManager)
            {
                Debug.Log("Session scene not loaded.");
                return;
            }
            sessionManager.OverwriteAndSave();
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        private void OverwriteAndSave()
        {
            if (_currentSessionGameData == null)
            {
                Debug.LogError("Session data is missing.");
                return;
            }
            
            foreach (var dataWriter in _dataWriters)
            {
                dataWriter.WriteToData(_currentSessionGameData);
            }
            
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void RegisterDataWriter(IGameDataWriter dataWriter)
        {
            _dataWriters.Add(dataWriter);
        }

        public void UnregisterDataWriter(IGameDataWriter dataWriter)
        {
            _dataWriters.Remove(dataWriter);
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