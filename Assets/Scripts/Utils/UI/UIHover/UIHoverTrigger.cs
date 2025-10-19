using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.Utils.UI.UIHover
{
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