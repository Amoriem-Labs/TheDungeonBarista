using UnityEngine;

namespace TDB.Utils.ObjectPools.PooledObject
{
    public class PooledParticleSystem : MonoBehaviour, IPooledObject<PooledParticleSystem>
    {
        [SerializeField] protected PooledObjectTag _pooledObjectTag;
        
        private MonoObjectPool<PooledParticleSystem> _pool;
        public ParticleSystem Particle { get; private set; }
        public PooledObjectTag PooledObjectTag => _pooledObjectTag;

        // Start is called before the first frame update
        void Awake()
        {
            Particle = GetComponent<ParticleSystem>();
            var main = Particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void SetPool(MonoObjectPool<PooledParticleSystem> pool)
        {
            _pool = pool;
        }

        private void OnParticleSystemStopped()
        {
            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

