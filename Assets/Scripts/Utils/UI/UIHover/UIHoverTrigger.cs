using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.Utils.UI.UIHover
{
    /// <summary>
    /// This class has similar functions as the Unity Event Trigger, but with a bit more automation.
    /// Compared to IPointerEnterHandler, IPointerExitHandler, it controls IUIHoverHandler in child objects as well.
    /// </summary>
    public class UIHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IUIHoverHandler[] _handlers;

        private void Awake()
        {
            _handlers = GetComponentsInChildren<IUIHoverHandler>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var handler in _handlers)
            {
                handler.OnUIHoverEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var handler in _handlers)
            {
                handler.OnUIHoverExit();
            }
        }
    }
    
    public interface IUIHoverHandler
    {
        public void OnUIHoverEnter();
        public void OnUIHoverExit();
    }
}