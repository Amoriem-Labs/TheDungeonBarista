using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.Framework
{
    [System.Serializable]
    public class InventoryStackData<T> where T : ResourceScriptableObject
    {
        [field: SerializeField]
        public T Definition { get; private set; }
        
        [field: SerializeField]
        public int Amount { get; protected set; }

        public void Consume(int amount)
        {
            Amount -= amount;
            Amount = Mathf.Max(0, Amount);
        }

        public void Deposit(int amount)
        {
            Amount += amount;
        }
        
        public InventoryStackData(T definition)
        {
            Definition = definition;
            Amount = 0;
        }

        public InventoryStackData(InventoryStackData<T> stack)
        {
            Definition = stack.Definition;
            Amount = stack.Amount;
        }
    }
}