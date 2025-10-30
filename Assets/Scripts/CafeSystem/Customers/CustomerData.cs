using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.EffectSystem.Data;
using UnityEngine;

namespace TDB.CafeSystem.Customers
{
    [System.Serializable]
    public class CustomerData
    {
        [TableList(AlwaysExpanded = true)]
        public List<CustomerPreferenceData> Preferences;

        public bool IsPreferenceRevealed;

        public CustomerStatus Status;

        public CustomerData(List<CustomerPreferenceData> preferences)
        {
            Preferences = preferences;
            IsPreferenceRevealed = false;
            Status = CustomerStatus.Invalid;
        }
    }

    public enum CustomerStatus
    {
        // cannot interact, usually walking
        Invalid,
        // can be served with dishes
        Waiting,
        // eating and cannot interact
        Eating
    }
    
    [System.Serializable]
    public class CustomerPreferenceData
    {
        public FlavorDefinition Flavor;
        // positive values for like, negative values for dislike
        public int PreferenceLevel;
    }
}