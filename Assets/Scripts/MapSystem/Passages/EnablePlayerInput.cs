using System;
using System.Collections;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class EnablePlayerInput : MonoBehaviour, IPassageHandler
    {
        [SerializeField] private EventChannel _enablePlayerInputEvent;
        [SerializeField] private EventChannel _disablePlayerInputEvent;

        public IEnumerator HandleEnterPassage(Action abort)
        {
            _enablePlayerInputEvent.RaiseEvent();
            yield break;
        }

        public IEnumerator UndoEffect()
        {
            _disablePlayerInputEvent.RaiseEvent();
            yield break;
        }

        private void OnValidate()
        {
            if (!_enablePlayerInputEvent)
            {
                _enablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/EnablePlayerInput");
            }

            if (!_disablePlayerInputEvent)
            {
                _disablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/DisablePlayerInput");
            }
        }
    }
}