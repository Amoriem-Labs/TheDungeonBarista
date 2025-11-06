using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.Utils.Misc;
using TDB.Utils.ObjectPools;
using TDB.Utils.UI.UIHover;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class CustomerOrderItemUI : MonoBehaviour, IPooledObject<CustomerOrderItemUI>, IUIHoverHandler
    {
        [SerializeField] private RectTransform _anchor;
        [SerializeField] private TextMeshProUGUI _customerName;
        [SerializeField] private List<TextMeshProUGUI> _likeTexts;
        [SerializeField] private List<TextMeshProUGUI> _dislikeTexts;

        [Title("Texts")]
        [SerializeField] private string _likeRichText;
        [SerializeField] private string _dislikeRichText;

        [Title("Animation")]
        [SerializeField] private Vector2 _selectedExtraAnchorHeight = new Vector2(10, 20);
        [SerializeField] private float _introTime = .4f;
        [SerializeField] private Vector2 _introVelocity = new Vector2(0, 1);
        [SerializeField] private float _smoothTime = 0.2f;

        [Title("Rotation")]
        [SerializeField, Min(0f)] private float _rotationMultiplier = 1f;
        [SerializeField, Range(0, 90)] private float _maxRotationAngle = 30f;
        
        private CustomerData _data;
        
        private Canvas _canvas;
        private Camera _canvasCamera;
        private MonoObjectPool<CustomerOrderItemUI> _pool;
        private Vector3 _velocity;
        private float _rotationSpeed;
        private ImageOutlineController _outline;
        private Vector2 _initialSize;
        private LayoutElement _anchorLayout;

        public RectTransform Anchor => _anchor;
        private Vector3 DesiredPosition => Anchor.position;

        private float DesiredRotation =>
            Mathf.Clamp(_velocity.x * _velocity.y * _rotationMultiplier * SmoothTime, -1f, 1f) * _maxRotationAngle;

        public float SmoothTime => _smoothTime;
        
        public string LikeRichText => _likeRichText;
        public string DislikeRichText => _dislikeRichText;
        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasCamera = _canvas.worldCamera;

            _outline = GetComponentInChildren<ImageOutlineController>();

            _anchorLayout = Anchor.GetComponent<LayoutElement>();
            _initialSize = new Vector2(_anchorLayout.preferredWidth, _anchorLayout.preferredHeight);
        }

        private void OnEnable()
        {
            transform.DOKill();
            transform.DOScale(1, _introTime)
                .From(0)
                .SetEase(Ease.OutBack);
            
            _velocity = _introVelocity;
            _rotationSpeed = 0;
        }

        private void OnDisable()
        {
            if (_data != null)
            {
                _data.OnReadyUpdate -= HandleReadyUpdate;

                _data = null;
            }
        }

        private void Update()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            var desiredRotation = DesiredRotation;
            var currentRotation = transform.rotation.eulerAngles.z;
            if (Mathf.Approximately(desiredRotation, currentRotation)) return;

            transform.rotation = Quaternion.Euler(0, 0, desiredRotation);
        }

        private void UpdatePosition()
        {
            var desiredPosition = DesiredPosition;
            if (Mathf.Approximately(Vector3.Distance(desiredPosition, transform.position), 0)) return;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, SmoothTime);
        }

        public void BindData(CustomerData data)
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
                    p => p.Flavor.Name,
                    p => p.PreferenceLevel);
            
            var likes = preference
                .Where(p => p.Value > 0)
                .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat(LikeRichText, p.Value)) + ")")
                .ToList();
            for (int i = 0; i < _likeTexts.Count; i++)
            {
                if (i < likes.Count)
                {
                    _likeTexts[i].gameObject.SetActive(true);
                    _likeTexts[i].text = likes[i];
                }
                else
                {
                    _likeTexts[i].gameObject.SetActive(false);
                }
            }
            
            var dislikes = preference
                .Where(p => p.Value < 0)
                .Select(p => p.Key + "(" + string.Join("", Enumerable.Repeat(DislikeRichText, -p.Value)) + ")")
                .ToList();
            for (int i = 0; i < _dislikeTexts.Count; i++)
            {
                if (i < dislikes.Count)
                {
                    _dislikeTexts[i].gameObject.SetActive(true);
                    _dislikeTexts[i].text = dislikes[i];
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
        
        public void SetPool(MonoObjectPool<CustomerOrderItemUI> pool)
        {
            _pool = pool;
        }

        public void DestroyItem()
        {
            _anchor.SetParent(transform);
            
            if (_pool)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnUIHoverEnter()
        {
            _data.SetReady(true, this);
        }

        public void OnUIHoverExit()
        {
            _data.SetReady(false, this);
        }
    }
}