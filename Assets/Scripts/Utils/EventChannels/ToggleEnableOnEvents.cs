using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.Utils.EventChannels
{
    public class ToggleEnableOnEvents : MonoBehaviour
    {
        [SerializeField] private List<VoidEvent> _enableOnEvents;
        [SerializeField] private List<VoidEvent> _disableOnEvents;

        private void Awake()
        {
            foreach (var eventChannel in _enableOnEvents)
            {
                eventChannel.AddListener(Enable);
            }

            foreach (var eventChannel in _disableOnEvents)
            {
                eventChannel.AddListener(Disable);
            }
        }

        private void OnDestroy()
        {
            foreach (var eventChannel in _enableOnEvents)
            {
                eventChannel.RemoveListener(Enable);
            }
            
            foreach (var eventChannel in _disableOnEvents)
            {
                eventChannel.RemoveListener(Disable);
            }
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        private void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}