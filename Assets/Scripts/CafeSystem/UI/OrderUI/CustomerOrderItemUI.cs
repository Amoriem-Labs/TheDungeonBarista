using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.Utils.Misc;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class CustomerOrderItemUI : DynamicItemUI<CustomerData>, IUIHoverHandler
    {
        [Title("References")]
        [SerializeField] private RectTransform _anchor;
        [SerializeField] private TextMeshProUGUI _customerName;
        [SerializeField] private List<AttributeItemUI> _likeTexts;
        [SerializeField] private List<AttributeItemUI> _dislikeTexts;

        [Title("Texts")]
        [SerializeField] private string _likeRichText;
        [SerializeField] private string _dislikeRichText;

        [TitleGroup("Animation")]
        [SerializeField] private Vector2 _selectedExtraAnchorHeight = new Vector2(10, 20);
        [TitleGroup("Animation")]
        [SerializeField] private float _introTime = .4f;
        [TitleGroup("Animation")]
        [SerializeField] private Vector2 _introVelocity = new Vector2(0, 1);

        private CustomerData _data;
        
        private Canvas _canvas;
        private ImageOutlineController _outline;
        private Vector2 _initialSize;
        private LayoutElement _anchorLayout;
        private bool _hoverable;
        private readonly Dictionary<FlavorDefinition, AttributeItemUI> _flavorToTexts = new();
        private CanvasGroup _canvasGroup;

        public override RectTransform Anchor => _anchor;

        public string LikeRichText => _likeRichText;
        public string DislikeRichText => _dislikeRichText;
        public float AnchorWidth => _anchorLayout.preferredWidth;
        public float WorldSpaceScale => _canvas.transform.localScale.x;
        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            _outline = GetComponentInChildren<ImageOutlineController>();

            _anchorLayout = Anchor.GetComponent<LayoutElement>();
            _initialSize = new Vector2(_anchorLayout.preferredWidth, _anchorLayout.preferredHeight);

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            transform.DOKill();
            transform.DOScale(1, _introTime)
                .From(0)
                .SetEase(Ease.OutBack);
            
            Velocity = _introVelocity;
            
            ToggleInteractable(true);
            
            _canvasGroup.alpha = 1;
            _anchorLayout.ignoreLayout = false;
        }

        private void OnDisable()
        {
            if (_data != null)
            {
                _data.OnReadyUpdate -= HandleReadyUpdate;

                _data = null;
            }
        }

        public override void BindData(CustomerData data)
        {
            _data = data;

            SetTexts(_data);
            _data.OnReadyUpdate += HandleReadyUpdate;
            HandleReadyUpdate();
        }

        private void HandleReadyUpdate()
        {
            SetHighlight(_data.IsReady);
        }

        private void SetTexts(CustomerData data)
        {
            _customerName.text = _data.CustomerName;
            
            var preference =
                data.Preferences.ToDictionary(
                    p => p.Flavor,
                    p => p.PreferenceLevel);

            _flavorToTexts.Clear();
            
            var likes = preference
                .Where(p => p.Value > 0)
                .Select(p => (Flavor: p.Key, Text: p.Key.Name + "(" + string.Join("", Enumerable.Repeat(LikeRichText, p.Value)) + ")"))
                .ToList();
            for (int i = 0; i < _likeTexts.Count; i++)
            {
                if (i < likes.Count)
                {
                    _likeTexts[i].gameObject.SetActive(true);
                    _likeTexts[i].SetText(likes[i].Text);
                    _flavorToTexts.Add(likes[i].Flavor, _likeTexts[i]);
                }
                else
                {
                    _likeTexts[i].gameObject.SetActive(false);
                }
            }
            
            var dislikes = preference
                .Where(p => p.Value < 0)
                .Select(p => (Flavor: p.Key, Text: p.Key.Name + "(" + string.Join("", Enumerable.Repeat(DislikeRichText, -p.Value)) + ")"))
                .ToList();
            for (int i = 0; i < _dislikeTexts.Count; i++)
            {
                if (i < dislikes.Count)
                {
                    _dislikeTexts[i].gameObject.SetActive(true);
                    _dislikeTexts[i].SetText(dislikes[i].Text);
                    _flavorToTexts.Add(dislikes[i].Flavor, _dislikeTexts[i]);
                }
                else
                {
                    _dislikeTexts[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetHighlight(bool enable)
        {
            _outline.ToggleOutline(enable);
            
            _anchorLayout.preferredWidth = _initialSize.x + (enable ? _selectedExtraAnchorHeight.x : 0f);
            _anchorLayout.preferredHeight = _initialSize.y + (enable ? _selectedExtraAnchorHeight.y : 0f);
        }

        public void ForceAnchorExpanded()
        {
            _anchorLayout.preferredWidth = _initialSize.x + _selectedExtraAnchorHeight.x;
            _anchorLayout.preferredHeight = _initialSize.y + _selectedExtraAnchorHeight.y;
        }

        public void OnUIHoverEnter()
        {
            if (!_hoverable) return;
                
            _data.SetReady(true, this);
        }

        public void OnUIHoverExit()
        {
            _data.SetReady(false, this);
        }

        public override void DestroyItem()
        {
            _anchor.SetParent(transform);
            base.DestroyItem();
        }
        
        public void FadeOut(float duration, Vector2 fadeOffset)
        {
            _anchorLayout.ignoreLayout = true;
            _anchorLayout.transform.SetParent(transform);
            _canvasGroup.DOFade(0, duration);
            transform.DOMove(fadeOffset, duration)
                .SetRelative(true);
        }

        public void ToggleInteractable(bool hoverable)
        {
            if (!hoverable)
            {
                OnUIHoverExit();
            }
            
            _hoverable = hoverable;
        }

        public void HighlightAttribute(FlavorDefinition flavor, float animTime)
        {
            if(!_flavorToTexts.TryGetValue(flavor, out var attribute)) return;
            attribute.TurnOnHighlight(animTime);
        }
    }
}