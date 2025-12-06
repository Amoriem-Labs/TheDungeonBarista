using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CafeSystem.UI.ProductUI;
using TDB.GameManagers.SessionManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class CustomerOrderUI : DynamicItemListUI<CustomerData>
    {
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _negativeColor;
        
        [Title("Animation")]
        [SerializeField] private float _stepTime = .4f;

        [SerializeField] private Vector2 _fadeOutOffset = new Vector2(10, 0);
        
        [Title("Events")]
        [SerializeField]
        private EventChannel _displayCustomerOrderEvent;

        private Dictionary<CustomerData, CustomerOrderItemUI> _orderItems = new();

        private void OnEnable()
        {
            _displayCustomerOrderEvent.AddListener<DisplayCustomerOrderInfo>(HandleDisplayCustomerOrder);
        }

        private void OnDisable()
        {
            _displayCustomerOrderEvent.RemoveListener<DisplayCustomerOrderInfo>(HandleDisplayCustomerOrder);
        }

        private void HandleDisplayCustomerOrder(DisplayCustomerOrderInfo info)
        {
            var customer = info.CustomerData;
            var position = info.CustomerPosition;

            var itemUI = AddItem(position, customer) as CustomerOrderItemUI;
            _orderItems.Add(customer, itemUI!);
        }

        public void ServeOrder(ServeProductInfo info, ProductItemUI productItemUI, MoneyManager moneyManager)
        {
            var customer = info.Customer;
            if (!_orderItems.Remove(customer, out var orderItemUI))
            {
                Debug.LogError($"Customer {customer.CustomerName} has no order item UI registered.");
                return;
            }

            StartCoroutine(ServeOrderCoroutine(orderItemUI, productItemUI, info, moneyManager));
        }

        private IEnumerator ServeOrderCoroutine(CustomerOrderItemUI orderItemUI, ProductItemUI productItemUI,
            ServeProductInfo info, MoneyManager moneyManager)
        {
            // force focus order item
            orderItemUI.ToggleInteractable(false);
            orderItemUI.ForceAnchorExpanded();
            // move product item to order item
            productItemUI.AnchorToOrderItem(orderItemUI.Anchor.transform,
                orderItemUI.AnchorWidth * orderItemUI.WorldSpaceScale / 2);
            yield return new WaitUntil(() => productItemUI.PositionedAtAnchor());
            // display per flavor bonus
            foreach (var flavor in productItemUI.GetActiveFlavors())
            {
                var effect = flavor.EffectDefinition;
                var bonus = info.FlavorMultipliers.GetValueOrDefault(effect, 0);
                orderItemUI.HighlightAttribute(flavor, _stepTime / 2);
                productItemUI.DisplayFlavorBonus(flavor,
                    (bonus >= 0 ? "+" : "-") + $"{Mathf.Abs(bonus):P0}",
                    bonus >= 0 ? _positiveColor : _negativeColor,
                    _stepTime / 2);
                yield return new WaitForSeconds(_stepTime);
            }
            // display total bonus
            productItemUI.DisplayTotalBonus(
                (info.TotalFlavorMultiplier >= 0 ? "+" : "-") + $"{Mathf.Abs(info.TotalFlavorMultiplier):P0}",
                info.TotalFlavorMultiplier >= 0 ? _positiveColor : _negativeColor, _stepTime / 2);
            yield return new WaitForSeconds(_stepTime);
            // display final price
            yield return productItemUI.DisplayFinalPrice(info.Product.Price, info.FinalPrice, _stepTime);
            yield return new WaitForSeconds(_stepTime);
            // update money UI
            moneyManager.ReceiveMoneyFrom(info.FinalPrice, orderItemUI.transform.position);
            
            // fade out and destroy
            productItemUI.FadeOut(_stepTime, _fadeOutOffset);
            orderItemUI.FadeOut(_stepTime, _fadeOutOffset);
            yield return new WaitForSeconds(_stepTime);
            productItemUI.DestroyItem();
            orderItemUI.DestroyItem();
        }
    }

    public struct DisplayCustomerOrderInfo
    {
        public CustomerData CustomerData;
        public Vector3 CustomerPosition;
    }
}