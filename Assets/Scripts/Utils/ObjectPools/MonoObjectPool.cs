using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace TDB.Utils.ObjectPools
{
    public class MonoObjectPool<T> : MonoBehaviour where T: MonoBehaviour, IPooledObject<T>
    {
        [SerializeField] protected T _pooledPrefab;
        [SerializeField] private Transform _pooledObjectParent;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxSize = 500;
        [Header("Debug")]
        [SerializeField, ReadOnly]
        private int _totalPooledObjects;
        [SerializeField, ReadOnly]
        private int _activePooledObjects;
        [SerializeField, ReadOnly]
        private int _inactivePooledObjects;
        [SerializeField, ReadOnly]
        private int _maxActivePooledObjects;

        public Action<T> OnPooledObjectCreateCallback;
        public Action<T> OnPooledObjectGetCallback;
        public Action<T> OnPooledObjectReleaseCallback;
        public Action<T> OnPooledObjectDestroyCallback;

        protected ObjectPool<T> _pool;
        public T Prefab => _pooledPrefab;
        
        protected virtual void Awake()
        {
            _pool = new ObjectPool<T>(OnPooledObjectCreate, OnPooledObjectGet, OnPooledObjectRelease, OnPooledObjectDestroy,
                                      _collectionCheck, _defaultCapacity, _maxSize);
            _maxActivePooledObjects = 0;
            _pooledObjectParent = _pooledObjectParent ?? transform;

            if (_pooledPrefab)
            {
                InitializePool();
            }
        }

        public void InitializePool()
        {
            InitializePoolMemory(_defaultCapacity);
            var poolManager = GetComponentInParent<ObjectPoolManager>();
            poolManager?.RegisterPool(this);
        }
        
        public void InitializePoolMemory(int count)
        {
            List<T> buffer = new();
            for (int i = 0; i < count; i++)
            {
                var obj = Get();
                buffer.Add(obj);
            }
            foreach (var obj in buffer)
            {
                Release(obj);
            }
            buffer.Clear();
        }

        public static MonoObjectPool<T> Create<T0>(T prefab, Transform parent,
            int defaultCapacity = 10, int maxSize = 500, bool collectionCheck = true) where T0 : MonoObjectPool<T>
        {
            var go = new GameObject(prefab.name + " Pool", typeof(T0));
            var pool = go.GetComponent<T0>();
            pool.SetPrefab(prefab);
            pool.transform.SetParent(parent);
            pool.InitializePool();
            return pool;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            _totalPooledObjects = _pool.CountAll;
            _activePooledObjects = _pool.CountActive;
            _inactivePooledObjects = _pool.CountInactive;
            _maxActivePooledObjects = Mathf.Max(_activePooledObjects, _maxActivePooledObjects);
#endif
        }

        public void SetPrefab(T prefab)
        {
            if (_pooledPrefab)
            {
                Debug.LogWarning($"Pool {gameObject.name} is already set to prefab {prefab.name}.");
                return;
            }
            
            _pooledPrefab = prefab;
            InitializePool();
        }

        public void SetTag(PooledObjectTag tag)
        {
            
        }

        protected virtual T OnPooledObjectCreate()
        {
            T obj = GameObject.Instantiate(_pooledPrefab, _pooledObjectParent);
            obj.SetPool(this);
            OnPooledObjectCreateCallback?.Invoke(obj);
            return obj;
        }

        protected virtual void OnPooledObjectGet(T pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
            OnPooledObjectGetCallback?.Invoke(pooledObject);
        }

        protected virtual void OnPooledObjectRelease(T pooledObject)
        {
            OnPooledObjectReleaseCallback?.Invoke(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }

        protected virtual void OnPooledObjectDestroy(T pooledObject)
        {
            OnPooledObjectDestroyCallback?.Invoke(pooledObject);
            Destroy(pooledObject.gameObject);
        }
        
        public virtual T Get(bool inSceneRoot = false, Transform parent = null)
        {
            T obj = _pool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(parent ?? (inSceneRoot ? null : _pooledObjectParent));
            return obj;
        }

        public virtual T Get(Vector3 position, Quaternion rotation, bool inSceneRoot = false, Transform parent = null)
        {
            T obj = Get(inSceneRoot, parent);
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        public virtual void Release(T obj)
        {
            obj.transform.SetParent(_pooledObjectParent);
            obj.gameObject.SetActive(false);
            _pool.Release(obj);
        }

        public virtual void Release(T obj, float delay)
        {
            StartCoroutine(ReleaseAfterSeconds(obj, delay));
        }

        protected virtual IEnumerator ReleaseAfterSeconds(T obj, float delay, bool fixedUpdate=false)
        {
            if (fixedUpdate)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            Release(obj);
        }
    }
}
