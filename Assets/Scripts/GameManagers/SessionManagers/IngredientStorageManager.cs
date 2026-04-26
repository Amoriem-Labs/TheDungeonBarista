using System.Collections.Generic;
using System.Linq;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;
using TDB.InventorySystem.IngredientStorage;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.GameManagers.SessionManagers
{
    public class IngredientStorageManager : MonoBehaviour
    {
        [SerializeField] private EventChannel _cafePreparationStartEvent;

        private IngredientStorageData _volatileStorage;
        private IngredientStorageData _refrigeratedStorage;
        
        private IRefrigeratorCapacityCalculator _capacityCalculator;

        public int RefrigeratorCapacity => _capacityCalculator?.GetCapacity() ?? 0;

        private void OnEnable()
        {
            _cafePreparationStartEvent?.AddListener(HandleCafePreparationStart);
        }

        private void OnDisable()
        {
            _cafePreparationStartEvent?.RemoveListener(HandleCafePreparationStart);
        }

        public void InitializeStorages(IngredientStorageData volatileIngredientStorage,
            IngredientStorageData refrigeratedIngredientStorage)
        {
            _volatileStorage = volatileIngredientStorage;
            _refrigeratedStorage = refrigeratedIngredientStorage;
        }

        public void BindRefrigeratorCapacityManager(IRefrigeratorCapacityCalculator calculator)
        {
            _capacityCalculator = calculator;
        }

        /// <summary>
        /// This method provides a read-only copy of the merged storage.
        /// Any modification to the returned storage may not be reflected in the persisted data.
        /// </summary>
        /// <returns></returns>
        public IngredientStorageData GetMergedIngredientStorage()
        {
            return new IngredientStorageData(new List<InventoryData<IngredientDefinition>>
                { _volatileStorage, _refrigeratedStorage });
        }

        public IngredientStorageData VolatileIngredientStorage => _volatileStorage;

        public IngredientStorageData RefrigeratedIngredientStorage => _refrigeratedStorage;

        public bool TryConsume(Dictionary<IngredientDefinition, int> requirement)
        {
            // consume volatile storage first
            _volatileStorage.TryConsume(requirement);

            if (requirement.Values.Sum() >= 0)
            {
                // TODO: consume refrigerated storage if not satisfied yet 
            }
            
            // failed to supply all required ingredients
            return requirement.Values.Sum() >= 0;
        }

        public void AddVolatileIngredient(IngredientDefinition itemDefinition)
        {
            AddVolatileIngredient(itemDefinition, 1);
        }

        public void AddVolatileIngredient(IngredientDefinition itemDefinition, int amount)
        {
            if (_volatileStorage == null)
            {
                Debug.LogError($"{nameof(IngredientStorageManager)} is missing volatile storage.", this);
                return;
            }

            if (itemDefinition == null || amount <= 0) return;

            _volatileStorage.Deposit(itemDefinition, amount);
        }

        public int GetVolatileIngredientEssence()
        {
            var storedIngredients = _volatileStorage.GetIngredientsCount;
            int essence = 0;
            foreach (var (ingredient, count) in storedIngredients)
            {
                essence += ingredient.GetEssence() * count;
            }
            return essence;
        }

        public void ClearVolatileIngredients()
        {
            _volatileStorage.Clear();
        }

        private void HandleCafePreparationStart()
        {
            var dailyFreeIngredients = GameManager.Instance?.GameConfig?.DailyFreeIngredients;
            if (dailyFreeIngredients == null) return;

            foreach (var entry in dailyFreeIngredients)
            {
                AddVolatileIngredient(entry.Key, entry.Value);
            }
        }
    }

    public interface IRefrigeratorCapacityCalculator
    {
        public int GetCapacity();
    }
}
