using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI.UIHover
{
    public class UIScaler : MonoBehaviour, IUIHoverHandler, IUIClickHandler
    {
        [SerializeField] private float _hoverScale = 1.1f;
        [SerializeField] private float _clickScale = 0.9f;
        [SerializeField] private float _smoothTime = 0.1f;

        [Tooltip("If set, the interactability of the UI element will decide whether this scaler is enabled or not.")]
        [SerializeField] private Selectable _controlElement;
        
        private Vector3 _originalScale;
        private bool _hovered;
        private bool _clicked;
        private float _currentMultiplier;
        private float _velocity;
        
        private float TargetScaleMultiplier => _clicked ? _clickScale : _hovered ? _hoverScale : 1;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _currentMultiplier = 1f;
        }

        private void Update()
        {
            if (Mathf.Approximately(_currentMultiplier, TargetScaleMultiplier))
            {
                return;
            }

            _currentMultiplier =
                Mathf.SmoothDamp(_currentMultiplier, TargetScaleMultiplier, ref _velocity, _smoothTime);
            transform.localScale = _originalScale * _currentMultiplier;
        }

        public void OnUIHoverEnter()
        {
            if (_controlElement?.interactable == false) return;
            _hovered = true;
        }

        public void OnUIHoverExit()
        {
            _hovered = false;
        }

        public void OnUIClickStart()
        {
            if (_controlElement?.interactable == false) return;
            _clicked = true;
        }

        public void OnUIClickFinish()
        {
            _clicked = false;
        }
    }
}