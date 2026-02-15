using Sirenix.OdinInspector;
using TDB.Utils.ObjectPools;
using UnityEngine;

namespace TDB.CafeSystem.UI.OrderUI
{
    public abstract class DynamicItemUI<T> : MonoBehaviour, IPooledObject<DynamicItemUI<T>>
    {
        [TitleGroup("Animation")]
        [SerializeField] private float _smoothTime = 0.2f;
        [TitleGroup("Animation")]
        [SerializeField, Min(0f)] private float _rotationMultiplier = 1f;
        [TitleGroup("Animation")]
        [SerializeField, Range(0, 90)] private float _maxRotationAngle = 30f;
        
        private MonoObjectPool<DynamicItemUI<T>> _pool;

        protected Vector3 Velocity;
        
        public abstract RectTransform Anchor { get; }
        private Vector3 DesiredPosition => Anchor.position;

        private float DesiredRotation =>
            Mathf.Clamp(Velocity.x * Velocity.y * _rotationMultiplier * SmoothTime, -1f, 1f) * _maxRotationAngle;

        public float SmoothTime => _smoothTime;

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
            if (Anchor.parent == transform)
            {
                Velocity = Vector3.zero;
                return;
            }
            
            var desiredPosition = DesiredPosition;
            if (PositionedAtAnchor(desiredPosition)) return;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref Velocity, SmoothTime);
        }

        public bool PositionedAtAnchor(Vector3 desiredPosition, float epsilon = 0.01f)
        {
            return Vector3.Distance(desiredPosition, transform.position) < epsilon;
        }

        public  bool PositionedAtAnchor(float epsilon = 0.01f) => PositionedAtAnchor(DesiredPosition, epsilon);

        public abstract void BindData(T data);

        public void SetPool(MonoObjectPool<DynamicItemUI<T>> pool)
        {
            _pool = pool;
        }

        public virtual void DestroyItem()
        {
            if (_pool)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnEnable()
        {
            Velocity = Vector3.zero;
        }
    }
}