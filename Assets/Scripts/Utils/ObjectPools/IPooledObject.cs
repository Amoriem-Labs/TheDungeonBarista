using UnityEngine;

namespace TDB.Utils.ObjectPools
{
    public interface IPooledObject<T> where T: MonoBehaviour, IPooledObject<T>
    {
        public virtual PooledObjectTag PooledObjectTag => PooledObjectTag.Untagged;
        public void SetPool(MonoObjectPool<T> pool);
    }
}