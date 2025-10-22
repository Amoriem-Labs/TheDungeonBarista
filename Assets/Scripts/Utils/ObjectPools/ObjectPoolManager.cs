using System;
using System.Collections.Generic;
using TDB.Utils.ObjectPools.ConcretePools;
using TDB.Utils.ObjectPools.PooledObject;
using TDB.Utils.Singletons;
using UnityEngine;

namespace TDB.Utils.ObjectPools
{
    public class ObjectPoolManager : PassiveSingleton<ObjectPoolManager>
    {
        private Dictionary<PooledObjectTag, MonoBehaviour> _objectPools = new();

        public override void Initialize()
        {
            base.Initialize();
        }

        public void RegisterPool<T>(MonoObjectPool<T> pool) where T : MonoBehaviour, IPooledObject<T>
        {
            var pooledObjectTag = pool.Prefab.PooledObjectTag;
            // ignore untagged pools
            if (pooledObjectTag == PooledObjectTag.Untagged) return;
            
            if (!_objectPools.ContainsKey(pooledObjectTag))
            {
                _objectPools.Add(pooledObjectTag, pool);
            }
            else
            {
                Debug.LogWarning($"Pool with tag {pooledObjectTag} is already registered.");
            }
        }

        /// <summary>
        /// Must invoke this in the Start function.
        /// </summary>
        /// <param name="pooledObjectTag"></param>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public T1 GetPool<T0, T1>(PooledObjectTag pooledObjectTag) 
            where T0 : MonoBehaviour, IPooledObject<T0> where T1 : MonoObjectPool<T0>
        {
            if (pooledObjectTag == PooledObjectTag.Untagged)
            {
                Debug.LogError($"Cannot get pool without any tag.");
                return null;
            }
            
            if (_objectPools.TryGetValue(pooledObjectTag, out MonoBehaviour pool))
            {
                if (pool is T1 typedPool) return typedPool;
                Debug.LogError(
                    $"Object pool {pool.gameObject.name} with tag {pooledObjectTag} is not of type {typeof(T1).Name}.");
                return null;
            }

            Debug.LogError($"Object pool with tag {pooledObjectTag} is not registered.");
            return null;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Get the registered pool for the prefab.
        /// If not register, create one for PooledParticleSystem and PooledAudioSource.
        /// Must invoke this in the Start function.
        /// </summary>
        /// <param name="prefab"></param>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public T1 GetPool<T0, T1>(T0 prefab) 
            where T0 : MonoBehaviour, IPooledObject<T0> where T1 : MonoObjectPool<T0>
        {
            var pooledObjectTag = prefab.PooledObjectTag;
            if (pooledObjectTag == PooledObjectTag.Untagged)
            {
                Debug.LogError($"Prefab {prefab.name} does not have a PooledObjectTag assigned.");
                return null;
            }
            
            if (_objectPools.TryGetValue(pooledObjectTag, out MonoBehaviour pool))
            {
                if (pool is T1 typedPool) return typedPool;
                Debug.LogError($"Object pool {pool.gameObject.name} with tag {pooledObjectTag}" +
                               $" is not registered with type {typeof(T1).Name}.");
                return null;
            }

            if (TryCreatePool<T0, T1>(prefab, typeof(PooledParticleSystem), typeof(ParticleSystemPool),
                    out var particlePool))
                return particlePool;
            
            // if (TryCreatePool<T0, T1>(prefab, typeof(ProjectileBase), typeof(ProjectilePool),
            //         out var projectilePool))
            //     return projectilePool;
            
            if (TryCreatePool<T0, T1>(prefab, typeof(PooledAudioSource), typeof(AudioSourcePool),
                    out var audioSourcePool))
                return audioSourcePool;
            
            // if (TryCreatePool<T0, T1>(prefab, typeof(EnemyController), typeof(EnemyPool),
            //         out var enemyPool))
            //     return enemyPool;
            
            Debug.LogError($"Object pool with tag {pooledObjectTag} is not registered" +
                           $" and type {typeof(T0).Name} does not have default pool type.");
            return null;
        }

        private bool TryCreatePool<T0, T1>(T0 prefab, Type prefabType, Type poolType, out T1 typedPool)
            where T0 : MonoBehaviour, IPooledObject<T0> where T1 : MonoObjectPool<T0>
        {
            typedPool = null;
            if (typeof(T0) != prefabType) return false;

            var pooledObjectTag = prefab.PooledObjectTag;
            var go = new GameObject(pooledObjectTag.ToString() + "-Pool", poolType);
            if (go.TryGetComponent(out typedPool))
            {
                // set parent first
                go.transform.SetParent(transform);
                // so that the pool register itself on set prefab
                typedPool.SetPrefab(prefab);
                return true;
            }
            Debug.LogWarning($"Failed to create pool for prefab {prefab.name}.");
            Destroy(go);
            return false;
        }
    }
}