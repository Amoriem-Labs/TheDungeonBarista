using System;
using System.Collections;
using Sirenix.OdinInspector;
using TDB.Utils.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.Utils.UI.Tooltip
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _displayDelay = 0.3f;
        [SerializeField, ShowIf(nameof(ShowTooltipDataInspector)), InlineProperty, HideLabel]
        protected TooltipData _data;
        private EventChannel _displayTooltipEvent;
        private bool _displayed;
        private Coroutine _displayCoroutine;
        private Func<string> _textGetter;

        protected virtual bool ShowTooltipDataInspector => true;
        
        private void Awake()
        {
            _displayTooltipEvent = Resources.Load<EventChannel>("Events/GeneralPurpose/DisplayTooltip");
            _data.TriggerTransform = transform;
        }

        public void SetTooltipText(string text)
        {
            _data.TooltipText = text;
            _textGetter = null;
        }

        public void SetTooltipText(Func<string> func)
        {
            _textGetter = func;
        }
        
        public void SetData(TooltipData data)
        {
            _data = data;
            _data.TriggerTransform = transform;
        }
        
        private void OnDisable()
        {
            if (_displayCoroutine != null)
            {
                StopCoroutine(_displayCoroutine);
            }
            if (_displayed)
            {
                _displayed = false;
                _displayTooltipEvent.RaiseEvent<TooltipData>(null);
            }
        }

        private IEnumerator DisplayCoroutine()
        {
            if (_textGetter != null)
            {
                _data.TooltipText = _textGetter();
            }
            yield return new WaitForSecondsRealtime(_displayDelay);
            _displayed = true;
            _displayCoroutine = null;
            _displayTooltipEvent.RaiseEvent<TooltipData>(_data);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _displayCoroutine = StartCoroutine((IEnumerator)DisplayCoroutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_displayCoroutine != null)
            {
                StopCoroutine(_displayCoroutine);
            }
            if (_displayed)
            {
                _displayed = false;
                _displayTooltipEvent.RaiseEvent<TooltipData>(null);
            }
        }
    }
}