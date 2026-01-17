using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.InventorySystem.IngredientStorage;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.Framework
{
    [System.Serializable]
    public class InventoryData<T> : IEnumerable<InventoryStackData<T>> where T : ResourceScriptableObject
    {
        [TableList, SerializeField]
        private List<InventoryStackData<T>> _stacks;

        protected InventoryData(InventoryData<T> inventoryData)
        {
            _stacks = inventoryData.Stacks.Select(s => new InventoryStackData<T>(s)).ToList();
        }

        protected List<InventoryStackData<T>> Stacks => _stacks;

        public void TryConsume(Dictionary<T, int> requirement)
        {
            foreach (var stack in Stacks)
            {
                // not enough resource in the current stack
                if (stack.Amount <= 0) continue;
                
                var ingredient = stack.Definition;
                // current stack is not required or already satisfied
                if (!requirement.TryGetValue(ingredient, out var required) || required <= 0) continue;
                
                // consume from the stack and subtract from requirement
                var supply = Mathf.Min(stack.Amount, required);
                stack.Consume(supply);
                requirement[ingredient] = required - supply;
            }
        }

        public void Deposit(T itemDefinition, int amount = 1)
        {
            var stack = Stacks.Find(i => i.Definition == itemDefinition);
            if (stack == null)
            {
                stack = new InventoryStackData<T>(itemDefinition);
                Stacks.Add(stack);
            }
            stack.Deposit(amount);
        }

        public void Clear() => _stacks.Clear();

        IEnumerator<InventoryStackData<T>> IEnumerable<InventoryStackData<T>>.GetEnumerator() =>
            _stacks.GetEnumerator();

        public IEnumerator GetEnumerator() => _stacks.GetEnumerator();
    }
}