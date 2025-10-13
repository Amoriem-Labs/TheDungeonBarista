using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDB.Utils.EventChannels
{
    [CreateAssetMenu(menuName = "Event/Event Group", fileName = "New Event Group")]
    public class EventGroup : VoidEvent
    {
        [SerializeField]
        private List<EventChannel> _channels = new();

        public override void AddListener(UnityAction listener)
        {
            foreach (var channel in _channels)
            {
                channel.AddListener(listener);
            }
        }

        public override void RemoveListener(UnityAction listener)
        {
            foreach (var channel in _channels)
            {
                channel.RemoveListener(listener);
            }
        }
    }
}