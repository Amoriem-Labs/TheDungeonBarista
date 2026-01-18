using System;
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

        /// <summary>
        /// Try to consume items from the requirement dictionary.
        /// Subtract satisfied amount from the requirement dictionary.
        /// </summary>
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

        /// <summary>
        /// Deposit items to the inventory. Returns the new stack if allocated.
        /// </summary>
        public void Deposit(T itemDefinition, int amount = 1)
        {
            var stack = GetStack(itemDefinition, allocateNewStack: true);
            stack.Deposit(amount);
        }

        /// <summary>
        /// Consume items from the inventory.
        /// </summary>
        public void Consume(T itemDefinition, int amount = 1)
        {
            var stack = Stacks.Find(i => i.Definition == itemDefinition);
            if (stack == null) throw new ArgumentOutOfRangeException(nameof(itemDefinition), "Item not in inventory.");
            if (stack.Amount < amount) throw new ArgumentOutOfRangeException(nameof(amount), "Not enough amount.");
            stack.Consume(amount);
        }

        public InventoryStackData<T> GetStack(T itemDefinition, bool allocateNewStack = false)
        {
            var stack = Stacks.Find(i => i.Definition == itemDefinition);
            if (stack == null && allocateNewStack)
            {
                stack = new InventoryStackData<T>(itemDefinition);
                Stacks.Add(stack);
            }
            return stack;
        }

        /// <summary>
        /// Resets the inventory.
        /// </summary>
        public void Clear() => _stacks.Clear();

        IEnumerator<InventoryStackData<T>> IEnumerable<InventoryStackData<T>>.GetEnumerator() =>
            _stacks.GetEnumerator();

        public IEnumerator GetEnumerator() => _stacks.GetEnumerator();
    }
}