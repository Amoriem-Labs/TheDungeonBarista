using System;
using TDB.GameManagers.SessionManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.InventorySystem.IngredientStorage
{
    public class IngredientExpireController : MonoBehaviour
    {
        [SerializeField] private EventChannel _dungeonPreparationEndEvent;
        
        private IngredientStorageManager _ingredientStorage;
        private EssenceManager _essenceManager;

        private void Awake()
        {
            _ingredientStorage = FindObjectOfType<IngredientStorageManager>();
            _essenceManager = FindObjectOfType<EssenceManager>();
        }

        private void OnEnable()
        {
            _dungeonPreparationEndEvent.AddListener(HandleDungeonPreparationEnd);
        }

        private void OnDisable()
        {
            _dungeonPreparationEndEvent.RemoveListener(HandleDungeonPreparationEnd);
        }

        private void HandleDungeonPreparationEnd()
        {
            var newEssence = _ingredientStorage.GetVolatileIngredientEssence();
            _ingredientStorage.ClearVolatileIngredients();
            _essenceManager.AddEssence(newEssence);
        }
    }
}