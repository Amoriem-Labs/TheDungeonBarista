using System;
using System.Collections;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class DisablePlayerInput : MonoBehaviour, IPassageHandler
    {
        [SerializeField] private EventChannel _disablePlayerInputEvent;
        [SerializeField] private EventChannel _enablePlayerInputEvent;

        public IEnumerator HandleEnterPassage(Action abort)
        {
            _disablePlayerInputEvent.RaiseEvent();
            yield break;
        }

        public IEnumerator UndoEffect()
        {
            _enablePlayerInputEvent.RaiseEvent();
            yield break;
        }

        private void OnValidate()
        {
            if (!_disablePlayerInputEvent)
            {
                _disablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/DisablePlayerInput");
            }

            if (!_enablePlayerInputEvent)
            {
                _enablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/EnablePlayerInput");
            }
        }
    }
}