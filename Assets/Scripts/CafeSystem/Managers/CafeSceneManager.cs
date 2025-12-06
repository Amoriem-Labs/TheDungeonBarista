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
        /// Invoked once by the GameManager when the scene loading finishes.
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

            if (!TryFindObjectOfType<SessionManager>(out var sessionManager))
            {
                Debug.LogError("Session scene should be loaded before initializing.");
                return;
            }
            
            var furnitureData = sessionManager.AllInstalledFurnitureData;

            #endregion

            #region Initialize Controllers

            // initialize furniture (including storage entities and production device entities)
            TryFindObjectOfType<FurnitureManager>(out var furnitureManager);
            furnitureManager.Initialize(furnitureData, sessionManager);
            
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
