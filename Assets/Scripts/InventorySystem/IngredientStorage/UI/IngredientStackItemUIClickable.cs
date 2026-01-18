using System.Collections;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.Framework;
using TDB.Utils.UI.UIHover;
using UnityEngine;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class IngredientStackItemUIClickable : IngredientStackItemUIBase, IUIClickHandler
    {
        private IIngredientStackTransferHandler _transferHandler;
        private Coroutine _transferCoroutine;

        protected override void Awake()
        {
            base.Awake();

            _transferHandler = GetComponentInParent<IIngredientStackTransferHandler>();
        }

        public void OnUIClickStart()
        {
            Transfer(1);
            
            if (!gameObject.activeInHierarchy) return;
            _transferCoroutine = StartCoroutine(ContinuousTransferCoroutine());
        }

        private IEnumerator ContinuousTransferCoroutine()
        {
            // TODO:
            yield break;
        }

        public void OnUIClickFinish()
        {
            if (_transferCoroutine != null) StopCoroutine(_transferCoroutine);
        }

        private void Transfer(int amount) => _transferHandler.TransferIngredient(this, amount);
    }

    public interface IIngredientStackTransferHandler
    {
        public void TransferIngredient(IngredientStackItemUIClickable ingredientItem, int amount);
    }
}