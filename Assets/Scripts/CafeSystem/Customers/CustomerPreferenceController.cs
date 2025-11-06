using System;
using System.Linq;
using TDB.Player.Interaction;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.Customers
{
    /// <summary>
    /// Controls how to access the preference of a customer.
    /// </summary>
    [RequireComponent(typeof(CustomerController))]
    public class CustomerPreferenceController : MonoBehaviour, IInteractable
    {
        private CustomerController _customerController;

        private void Awake()
        {
            _customerController = GetComponent<CustomerController>();
            _customerController.BindOnCustomerDataUpdatedCallback(OnCustomerDataUpdated);
        }

        private void OnCustomerDataUpdated()
        {
            // Debug.Log("Customer data updated");
            OnInteractableUpdated?.Invoke();
        }

        /// <summary>
        /// Invoked by interaction trigger to display preference.
        /// </summary>
        public void RevealPreferences()
        {
            // reveal preferences
            _customerController.RevealPreferences();
            
            // update data and callback
            _customerController.IsPreferenceRevealed = true;
        }

        #region Interaction

        public bool IsInteractable => !_customerController.IsPreferenceRevealed;
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            _customerController.SetReady(this);
            // // update sprite
            // _customerController.OutlineController.ToggleOutline(true, this);
        }

        public void SetNotReady()
        {
            _customerController.SetNotReady(this);
            // // update sprite
            // _customerController.OutlineController.ToggleOutline(false, this);
        }

        #endregion
    }
}