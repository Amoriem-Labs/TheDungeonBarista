using TDB.Utils.EventChannels;
using TDB.Utils.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TDB.Utils.UI.Tooltip
{
    [RequireComponent(typeof(LayoutElement))]
    public class FloatingTooltip : MonoBehaviour
    {
        [SerializeField] private EventChannel _displayTooltipEvent;
        [SerializeField] private Camera _mainCamera;
        [SerializeField, Tooltip("In screen space.")] private Vector2 _offset;
        
        private LayoutGroup _layoutGroup;
        private TextMeshProUGUI _text;
        private LayoutElement _layoutElement;
        private float _maxWidth;
        private RectTransform _rectTransform;
        private Transform _anchorTransform;
        private float _padding;

        private void Awake()
        {
            _layoutGroup = GetComponent<LayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _layoutElement = GetComponent<LayoutElement>();
            
            _padding = _layoutGroup.padding.left + _layoutGroup.padding.right;
            _maxWidth = _layoutElement.preferredWidth;
            
            _displayTooltipEvent.AddListener<TooltipData>(HandleDisplayTooltip);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _displayTooltipEvent.RemoveListener<TooltipData>(HandleDisplayTooltip);
        }

        private void Update()
        {
            UpdateUI();
        }

        private void HandleDisplayTooltip(TooltipData data)
        {
            if (data is not { TooltipType: TooltipType.Floating })
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            _text.text = data.TooltipText;
            _layoutElement.preferredWidth = Mathf.Min(_text.preferredWidth + _padding, _maxWidth);
            _anchorTransform = data.TriggerTransform;

            UpdateUI();
        }

        private void UpdateUI()
        {
            var screenPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : _mainCamera.WorldToScreenPoint(_anchorTransform.position.SetZ(0)).ToVector2();
            
            var viewPos = _mainCamera.ScreenToViewportPoint(screenPos);
            _rectTransform.pivot = new Vector2(viewPos.x > 0.5f ? 1 : 0, viewPos.y > 0.5f ? 1 : 0);
            
            var offsetDir = new Vector2(viewPos.x > 0.5f ? -1 : 1, viewPos.y > 0.5f ? -1 : 1);
            _rectTransform.position = _mainCamera.ScreenToWorldPoint(screenPos + offsetDir * _offset).SetZ(0);
        }
    }
}