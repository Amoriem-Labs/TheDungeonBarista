using System.Collections;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class EnablePlayerInput : MonoBehaviour, IPassageHandler
    {
        [SerializeField] private EventChannel _enablePlayerInputEvent;

        public IEnumerator HandleEnterPassage()
        {
            _enablePlayerInputEvent.RaiseEvent();
            yield break;
        }
        
        protected virtual void OnValidate()
        {
            if (!_enablePlayerInputEvent)
            {
                _enablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/EnablePlayerInput");
            }
        }
    }
}