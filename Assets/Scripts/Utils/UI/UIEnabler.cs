using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.Utils.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIEnabler : MonoBehaviour
    {
        [SerializeField, ToggleLeft]
        private bool _displayOnAwake = true;
        
        [SerializeField, ToggleLeft]
        private bool _overrideDisplayParameter = false;

        [SerializeField, Range(0f, 1f), EnableIf(nameof(_overrideDisplayParameter))]
        private float _alpha = 1;
        [SerializeField, EnableIf(nameof(_overrideDisplayParameter))]
        private bool _interactable = true;
        [SerializeField, EnableIf(nameof(_overrideDisplayParameter))]
        private bool _blockRaycast = true;
        
        private CanvasGroup _canvasGroup;
        private IOnUIEnableHandler[] _onEnableHandlers;
        private IOnUIDisableHandler[] _onDisableHandlers;

        public bool Enabled { get; private set; }
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _onEnableHandlers = GetComponents<IOnUIEnableHandler>();
            _onDisableHandlers = GetComponents<IOnUIDisableHandler>();
            
            _alpha = _overrideDisplayParameter ? _alpha : _canvasGroup.alpha;
            _interactable = _overrideDisplayParameter ? _interactable : _canvasGroup.interactable;
            _blockRaycast = _overrideDisplayParameter ? _blockRaycast : _canvasGroup.blocksRaycasts;
            
            if (_displayOnAwake)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        
        public void Enable()
        {
            Enabled = true;
            
            _canvasGroup.alpha = _alpha;
            _canvasGroup.interactable = _interactable;
            _canvasGroup.blocksRaycasts = _blockRaycast;

            foreach (var handler in _onEnableHandlers)
            {
                handler.OnUIEnable();
            }
        }

        public void Disable()
        {
            Enabled = false;
            
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            foreach (var handler in _onDisableHandlers)
            {
                handler.OnUIDisable();
            }
        }

        public void Enable(float transitionTime)
        {
            // prevent click through immediately
            _canvasGroup.blocksRaycasts = _blockRaycast;
            
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(_alpha, transitionTime)
                .SetUpdate(true)
                .OnComplete(Enable);
        }

        public void Disable(float transitionTime)
        {
            // prevent interaction immediately
            _canvasGroup.interactable = false;
            
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0, transitionTime)
                .SetUpdate(true)
                .OnComplete(Disable);
        }
    }

    public interface IOnUIEnableHandler
    {
        public void OnUIEnable();
    }

    public interface IOnUIDisableHandler
    {
        public void OnUIDisable();
    }
}
