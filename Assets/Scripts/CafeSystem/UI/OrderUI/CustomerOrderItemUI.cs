using System;
using DG.Tweening;
using TDB.CafeSystem.Customers;
using TDB.Utils.ObjectPools;
using UnityEngine;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class CustomerOrderItemUI : MonoBehaviour, IPooledObject<CustomerOrderItemUI>
    {
        [SerializeField] private RectTransform _anchor;

        [SerializeField] private float _introTime = .4f;
        [SerializeField] private Vector2 _introVelocity = new Vector2(0, 1);
        [SerializeField] private float _smoothTime = 0.2f;

        [Header("Rotation")]
        [SerializeField, Min(0f)] private float _rotationMultiplier = 1f;
        [SerializeField, Range(0, 90)] private float _maxRotationAngle = 30f;
        
        private CustomerData _data;
        
        private Canvas _canvas;
        private Camera _canvasCamera;
        private MonoObjectPool<CustomerOrderItemUI> _pool;
        private Vector3 _velocity;
        private float _rotationSpeed;

        public RectTransform Anchor => _anchor;
        private Vector3 DesiredPosition => Anchor.position;

        private float DesiredRotation =>
            Mathf.Clamp(_velocity.x * _velocity.y * _rotationMultiplier * SmoothTime, -1f, 1f) * _maxRotationAngle;

        public float SmoothTime => _smoothTime;
        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasCamera = _canvas.worldCamera;
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
    }
}