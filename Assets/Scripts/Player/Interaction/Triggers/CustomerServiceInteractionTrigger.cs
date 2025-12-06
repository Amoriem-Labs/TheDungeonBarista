using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.Player.Interaction.Triggers
{
    public class CustomerServiceInteractionTrigger : InteractionTrigger<CustomerServiceController>
    {
        [Title("Events")]
        [SerializeField] private EventChannel _addProductEvent;
        [SerializeField] private EventChannel _serveProductEvent;
        
        [TableList, ReadOnly, SerializeField]
        private List<ProductData> _productsToServe = new();

        private int _productCapacity;

        public bool CanAddProduct => _productsToServe.Count < _productCapacity;
        
        protected override bool GetCanInteract(CustomerServiceController interactable) =>
            _productsToServe.Count > 0 && base.GetCanInteract(interactable);

        public override string InteractionTip => "Serve";

        private void Awake()
        {
            _productCapacity = GameManager.Instance.GameConfig.ProductListCapacity;
        }

        protected override void Interact(CustomerServiceController customer)
        {
            if (_productsToServe.Count < 0)
            {
                Debug.LogError("No food can be served.");
                return;
            }
            // serve and remove product
            var serveInfo = customer.ChooseAndServeFood(_productsToServe);
            if (serveInfo.Product == null) return;
            _productsToServe.Remove(serveInfo.Product);
            OnCurrentInteractableUpdated?.Invoke();
            _serveProductEvent.RaiseEvent(serveInfo);
            // TODO: update money data
            //      do not update UI
            Debug.Log($"The product {serveInfo.Product.RecipeName} is sold at price ${serveInfo.FinalPrice}.");
        }

        public void AddProductToServe(ProductData product)
        {
            _productsToServe.Add(product);
            OnCurrentInteractableUpdated?.Invoke();
            _addProductEvent.RaiseEvent(product);
        }
    }
}