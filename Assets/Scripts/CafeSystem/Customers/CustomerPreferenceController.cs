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
            _customerController.BindOnCustomerDataUpdatedCallback(OnInteractableUpdated);
        }

        /// <summary>
        /// Invoked by interaction trigger to display preference.
        /// </summary>
        public void RevealPreferences()
        {
            // TODO: reveal preferences
            var preference =
                _customerController.Preferences.ToDictionary(
                    p => p.Flavor.Name,
                    p => p.PreferenceLevel);
            var likes = preference
                .Where(p => p.Value > 0)
                .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat("❤️", p.Value)) + ")");
            var likeString = "I like " + string.Join(",", likes);
            var dislikes = preference
                .Where(p => p.Value < 0)
                .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat("👎", -p.Value)) + ")");
            var dislikeString = "I don't like " + string.Join(",", dislikes);
            Debug.Log(likeString);
            Debug.Log(dislikeString);
            
            // update data and callback
            _customerController.IsPreferenceRevealed = true;
        }

        #region Interaction

        public bool IsInteractable => !_customerController.IsPreferenceRevealed;
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // update sprite
            _customerController.OutlineController.ToggleOutline(true, this);
        }

        public void SetNotReady()
        {
            // update sprite
            _customerController.OutlineController.ToggleOutline(false, this);
        }

        #endregion
    }
}