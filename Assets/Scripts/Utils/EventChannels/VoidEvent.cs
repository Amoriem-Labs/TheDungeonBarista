using UnityEngine;
using UnityEngine.Events;

namespace TDB.Utils.EventChannels
{
    /// <summary>
    /// Event that can be invoked without arguments.
    /// </summary>
    public abstract class VoidEvent : ScriptableObject
    {
        public abstract void AddListener(UnityAction listener);
        public abstract void RemoveListener(UnityAction listener);
    }
}