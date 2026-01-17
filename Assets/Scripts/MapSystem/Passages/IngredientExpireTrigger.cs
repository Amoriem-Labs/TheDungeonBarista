using System;
using System.Collections;
using TDB.GameManagers.SessionManagers;
using TDB.InventorySystem.IngredientStorage.UI;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class IngredientExpireTrigger : MonoBehaviour, IPassageHandler
    {
        private IngredientStorageManager _ingredientStorage;
        private EssenceManager _essenceManager;
        private IngredientExpirePopUpUI _expireUI;
        
        private void Awake()
        {
            _ingredientStorage = FindObjectOfType<IngredientStorageManager>();
            _essenceManager = FindObjectOfType<EssenceManager>();
            _expireUI = FindObjectOfType<IngredientExpirePopUpUI>();
        }

        public IEnumerator HandleEnterPassage(Action abort)
        {
            var expiringIngredients = _ingredientStorage.GetVolatileIngredientStorage();
            var obtainedEssence = _ingredientStorage.GetVolatileIngredientEssence();
            var confirmed = false;
            yield return _expireUI.RequestConfirmation(expiringIngredients, obtainedEssence,
                confirmAction: b => confirmed = b);
            if (!confirmed)
            {
                abort();
            }
        }

        public IEnumerator UndoEffect()
        {
            throw new NotImplementedException();
        }
    }
}