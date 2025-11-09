using System;
using System.Collections.Generic;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class EnablePassageByEvent : MonoBehaviour, IPassageCondition
    {
        [SerializeField] private List<VoidEvent> _enableEvents = new();
        [SerializeField] private List<VoidEvent> _disableEvents = new();
        [SerializeField] private bool _initiallyEnabled = true;

        private bool _isSatisfied;
        
        public Action OnConditionChanged { get; set; }

        public bool IsSatisfied
        {
            get => _isSatisfied;
            private set
            {
                if (value == _isSatisfied) return;
                _isSatisfied = value;
                OnConditionChanged?.Invoke();
            }
        }

        private void OnEnable()
        {
            foreach (var eventChannel in _enableEvents)
            {
                eventChannel.AddListener(HandleEnable);
            }

            foreach (var eventChannel in _disableEvents)
            {
                eventChannel.AddListener(HandleDisable);
            }
        }

        private void OnDisable()
        {
            foreach (var eventChannel in _disableEvents)
            {
                eventChannel.RemoveListener(HandleDisable);
            }

            foreach (var eventChannel in _enableEvents)
            {
                eventChannel.RemoveListener(HandleEnable);
            }
        }

        private void HandleEnable() => IsSatisfied = true;

        private void HandleDisable() => IsSatisfied = false;

        private void Start()
        {
            IsSatisfied = true;
        }
    }
}