using System;
using System.Collections;
using TDB.CraftSystem.Data;
using TDB.GameManagers.SessionManagers;
using TDB.InventorySystem.Framework;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class IngredientExpirePopUpUI : MonoBehaviour, IIngredientInfoDisplayer
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        [SerializeField] private IngredientStackItemUIContainer _ingredientContainer;
        [SerializeField] private TextMeshProUGUI _essenceText;
        
        [SerializeField] private IngredientInfoUI _ingredientInfoUI;
        
        private UIEnabler _enabler;

        private bool? _confirmed;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
            
            _confirmButton.onClick.AddListener(HandleConfirm);
            _cancelButton.onClick.AddListener(HandleCancel);
        }

        private void HandleConfirm()
        {
            _confirmed = true;
        }

        private void HandleCancel()
        {
            _confirmed = false;
        }

        public IEnumerator RequestConfirmation(IngredientStorageData expiringIngredients, int obtainedEssence,
            Action<bool> confirmAction)
        {
            _enabler.Enable();

            // display ingredient list
            _ingredientContainer.Clear();
            _ingredientContainer.SetInventory(expiringIngredients);
            // display essence
            _essenceText.text = EssenceManager.EssenceToString(obtainedEssence);

            // wait for confirmation
            _confirmed = null;
            yield return new WaitUntil(() => _confirmed.HasValue);
            // ReSharper disable once PossibleInvalidOperationException
            confirmAction.Invoke(_confirmed.Value);

            _enabler.Disable();
        }

        public void DisplayIngredientInfo(IngredientInfoDisplayInfo info)
        {
            _ingredientInfoUI.DisplayIngredientInfo(info);
        }

        public void StopDisplaying()
        {
            _ingredientInfoUI.StopDisplaying();
        }
    }
}
