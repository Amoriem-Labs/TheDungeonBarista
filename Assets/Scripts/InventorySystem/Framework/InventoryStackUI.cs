using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.Framework
{
    public abstract class InventoryStackUI<T> : MonoBehaviour where T : ResourceScriptableObject
    {
        public abstract void SetStack(InventoryStackData<T> stack);
    }
}