using System;
using TDB.Utils.ObjectPools.ConcretePools;
using TDB.Utils.ObjectPools.PooledObject;
using UnityEngine;

namespace TDB.Utils.ObjectPools
{
    public class ExamplePoolUser : MonoBehaviour
    {
        [SerializeField] private PooledParticleSystem _prefab;
        [SerializeField] private PooledObjectTag _poolTag;

        private ParticleSystemPool _pool;
        
        // MUST get the pool in the ** Start ** method instead of Awake!!!
        // Otherwise, circular invocations might occur.
        private void Start()
        {
            // METHOD 1: get the pool by tag
            // reports an error if the pool is not instantiated
            _pool = ObjectPoolManager.Instance.GetPool<PooledParticleSystem, ParticleSystemPool>(_poolTag);
            // METHOD 2: get the pool by prefab
            // if the pool is not instantiated:
            // - creates the pool if it has the default constructor in GetPool method
            // - otherwise, reports an error 
            _pool = ObjectPoolManager.Instance.GetPool<PooledParticleSystem, ParticleSystemPool>(_prefab);
        }
    }
}