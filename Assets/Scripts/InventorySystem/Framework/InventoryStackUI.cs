using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.Framework
{
    public class InventoryStackUI<T> : MonoBehaviour where T : ResourceScriptableObject
    {
        private InventoryStackData<T> _stack;

        public InventoryStackData<T> Stack => _stack;
        
        public virtual void SetStack(InventoryStackData<T> stack)
        {
            _stack = stack;
        }
    }
}