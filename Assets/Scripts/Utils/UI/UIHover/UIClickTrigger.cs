using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.Utils.UI.UIHover
{
    /// <summary>
    /// This class has similar functions as the Unity Event Trigger, but with a bit more automation.
    /// It is not a replacement for Button, which only handles the click event.
    /// This class is meant to handle pointer down and up events.
    /// This class controls IUIHoverHandler in child objects as well.
    /// </summary>
    public class UIClickTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private IUIClickHandler[] _handlers;

        private void Awake()
        {
            _handlers = GetComponentsInChildren<IUIClickHandler>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            foreach (var handler in _handlers)
            {
                handler.OnUIClickStart();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            foreach (var handler in _handlers)
            {
                handler.OnUIClickFinish();
            }
        }
    }

    public interface IUIClickHandler
    {
        public void OnUIClickStart();
        public void OnUIClickFinish();
    }
}
