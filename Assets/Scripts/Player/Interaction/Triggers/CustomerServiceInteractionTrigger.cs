using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    public class CustomerServiceInteractionTrigger : InteractionTrigger<CustomerServiceController>
    {
        [TableList, ReadOnly, SerializeField]
        private List<ProductData> _productsToServe = new();

        protected override bool GetCanInteract(CustomerServiceController interactable) =>
            _productsToServe.Count > 0 && base.GetCanInteract(interactable);

        public override string InteractionTip => "Serve";

        protected override void Interact(CustomerServiceController customer)
        {
            if (_productsToServe.Count < 0)
            {
                Debug.LogError("No food can be served.");
                return;
            }
            // serve and remove product
            var servedProductIdx = customer.ChooseAndServeFood(_productsToServe);
            if (servedProductIdx < 0 || servedProductIdx >= _productsToServe.Count) return;
            _productsToServe.RemoveAt(servedProductIdx);
            OnCurrentInteractableUpdated?.Invoke();
        }

        public void AddProductToServe(ProductData product)
        {
            _productsToServe.Add(product);
            OnCurrentInteractableUpdated?.Invoke();
        }
    }
}