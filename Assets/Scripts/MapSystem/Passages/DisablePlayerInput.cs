using System.Collections;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public sealed class DisablePlayerInput : MonoBehaviour, IPassageHandler
    {
        [SerializeField] private EventChannel _disablePlayerInputEvent;

        public IEnumerator HandleEnterPassage()
        {
            _disablePlayerInputEvent.RaiseEvent();
            yield break;
        }

        private void OnValidate()
        {
            if (!_disablePlayerInputEvent)
            {
                _disablePlayerInputEvent = Resources.Load<EventChannel>("Events/Player/Input/DisablePlayerInput");
            }
        }
    }
}