using System;
using System.Linq;
using TDB.Player.Interaction;
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
            _customerController.OnCustomerDataUpdated += OnInteractableUpdated;
        }

        /// <summary>
        /// Invoked by interaction trigger to display preference.
        /// </summary>
        public void RevealPreferences()
        {
            // TODO: reveal preferences
            var preference =
                _customerController.Data.Preferences.ToDictionary(
                    p => p.Flavor.Name,
                    p => p.PreferenceLevel);
            var likeString = "I like " + string.Join(",",
                preference
                    .Where(p => p.Value > 0)
                    .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat("❤️", p.Value)) + ")"));
            var dislikes = preference
                .Where(p => p.Value < 0)
                .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat("👎", -p.Value)) + ")");
            var dislikeString = "I don't like " + string.Join(",", dislikes);
            Debug.Log(likeString);
            Debug.Log(dislikeString);
            
            // update data and callback
            _customerController.Data.IsPreferenceRevealed = true;
            _customerController.OnCustomerDataUpdated?.Invoke();
        }

        #region Interaction

        public bool IsInteractable => !_customerController.Data.IsPreferenceRevealed;
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // TODO: update sprite
        }

        public void SetNotReady()
        {
            // TODO: update sprite
        }

        #endregion
    }
}