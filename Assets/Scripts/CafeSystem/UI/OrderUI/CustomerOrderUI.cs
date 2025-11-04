using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class CustomerOrderUI : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _headerSpace;
        [SerializeField] private Transform _footerSpace;
        [SerializeField] private CustomerOrderItemUIPool _orderItemPool;
        
        [Title("Events")]
        [SerializeField]
        private EventChannel _displayCustomerOrderEvent;

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
            var itemUI = _orderItemPool.Get(info.CustomerPosition, Quaternion.identity);
            itemUI.transform.SetAsLastSibling();
            itemUI.Anchor.SetParent(_content);
            _headerSpace.SetAsFirstSibling();
            _footerSpace.SetAsLastSibling();
            itemUI.BindData(info.CustomerData);
        }
    }

    public struct DisplayCustomerOrderInfo
    {
        public CustomerData CustomerData;
        public Vector3 CustomerPosition;
    }
}